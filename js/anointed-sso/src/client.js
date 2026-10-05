// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// "Sign in with Anointed Automation": a server-side OpenID Connect client (authorization code + PKCE S256,
// state and nonce) for the Anointed Automation identity provider. SERVER ONLY: it holds the client secret
// and performs the token exchange. Never ship it to a browser or mobile bundle.
//
// The flow secrets (state, nonce, PKCE code_verifier and an optional same-site return path) ride ONE
// short-lived httpOnly cookie between the start and callback requests, so no server session store is needed.

import { AnointedSsoError } from "./errors.js";
import { base64UrlToJson, jsonToBase64Url } from "./encoding.js";
import { randomBase64Url, sha256Base64Url, timingSafeEqual } from "./crypto.js";
import { assertAudience, validateIdTokenClaims, verifyJwsRs256 } from "./jwt.js";
import { serializeCookie } from "./cookies.js";

/** The production issuer. Pass it explicitly as `issuer`; it is exported so apps do not retype it. */
export const ANOINTED_ISSUER = "https://api.anointedautomation.net/";

/**
 * Default scope. `profile` is deliberately left out: requesting it makes the consent screen stop users who
 * have no unique handle yet and ask them to pick a username first. Add it only when the app needs a name.
 */
export const DEFAULT_SCOPE = "openid email";

/** The member a back-channel Logout Token's `events` claim must carry. */
export const BACKCHANNEL_LOGOUT_EVENT = "http://schemas.openid.net/event/backchannel-logout";

const DEFAULTS = Object.freeze({
  scope: DEFAULT_SCOPE,
  cookieName: "aa_sso_flow",
  cookiePath: "/",
  flowTtlSeconds: 600,
  clockSkewSeconds: 60,
  metadataTtlSeconds: 3600,
});

const RESERVED_AUTHORIZE_PARAMS = new Set([
  "client_id",
  "redirect_uri",
  "response_type",
  "scope",
  "state",
  "nonce",
  "code_challenge",
  "code_challenge_method",
]);

/**
 * @summary    Whether a value is a non-null, non-array object.
 * @param {unknown} value
 * @returns {boolean}
 */
function isPlainObject(value) {
  return typeof value === "object" && value !== null && !Array.isArray(value);
}

/**
 * @summary    A non-empty string claim, or null.
 * @param {unknown} value
 * @returns {string | null}
 */
function stringOrNull(value) {
  return typeof value === "string" && value !== "" ? value : null;
}

/**
 * @summary    Whether a URL points at the local machine (http is allowed only there, for development).
 * @param {URL} url
 * @returns {boolean}
 */
function isLoopback(url) {
  return url.hostname === "localhost" || url.hostname === "127.0.0.1" || url.hostname === "[::1]";
}

/**
 * @summary    Parse an absolute https URL (http only for loopback hosts) or throw a coded error.
 * @param {unknown} value
 * @param {string} label  What the URL is, for the error message.
 * @param {string} code   Error code to raise.
 * @returns {URL}
 */
function parseSecureUrl(value, label, code) {
  let url;
  try {
    url = new URL(String(value));
  } catch {
    throw new AnointedSsoError(code, `${label} is not an absolute URL`);
  }
  if (url.protocol !== "https:" && !(url.protocol === "http:" && isLoopback(url))) {
    throw new AnointedSsoError(code, `${label} must use https (http is allowed only for localhost)`);
  }
  if (url.hash !== "") throw new AnointedSsoError(code, `${label} must not contain a fragment`);
  return url;
}

/**
 * @summary    Read a required non-empty string from the config.
 * @param {Record<string, unknown>} config
 * @param {string} key
 * @returns {string}
 */
function requireConfigString(config, key) {
  const value = config[key];
  if (typeof value !== "string" || value.trim() === "") {
    throw new AnointedSsoError("config", `${key} is required`);
  }
  return value;
}

/**
 * @summary    Read an optional config value, using the documented default when it is not supplied.
 * @param {Record<string, unknown>} config
 * @param {keyof typeof DEFAULTS} key
 * @returns {any}
 */
function optionOrDefault(config, key) {
  return config[key] === undefined ? DEFAULTS[key] : config[key];
}

/**
 * @summary    Read a positive finite number option.
 * @param {Record<string, unknown>} config
 * @param {keyof typeof DEFAULTS} key
 * @returns {number}
 */
function positiveNumberOption(config, key) {
  const value = optionOrDefault(config, key);
  if (typeof value !== "number" || !Number.isFinite(value) || value <= 0) {
    throw new AnointedSsoError("config", `${key} must be a positive number`);
  }
  return value;
}

/**
 * @summary    Keep a return path only when it is a same-site absolute path (blocks open redirects).
 * @description
 *   Accepts "/account?tab=1"; rejects absolute URLs, protocol-relative "//evil.example", backslash tricks,
 *   control characters and anything over 2048 characters.
 * @param {unknown} value
 * @returns {string | null}
 */
export function safeReturnTo(value) {
  if (typeof value !== "string" || value === "" || value.length > 2048) return null;
  if (!value.startsWith("/") || value.startsWith("//") || value.startsWith("/\\")) return null;
  if (/[\u0000-\u001f\u007f\\]/.test(value)) return null;
  return value;
}

/**
 * @summary    Create a "Sign in with Anointed Automation" client.
 * @description
 *   Validates the configuration immediately (fail fast). Discovery and JWKS are fetched lazily and cached
 *   for `metadataTtlSeconds`; the JWKS is refetched once when a token names an unknown `kid` (key rotation).
 *
 * @param {object} config
 * @param {string} config.issuer        `https://api.anointedautomation.net/` (export ANOINTED_ISSUER).
 * @param {string} config.clientId      `aa_<your key id>`.
 * @param {string} config.clientSecret  From the admin "Sign-in" dialog. Server only.
 * @param {string} config.redirectUri   The exact registered callback URL.
 * @param {string} [config.scope]       Space separated; must include `openid`. Default `openid email`.
 * @param {boolean} [config.requireVerifiedEmail]  Default: true when the scope includes `email`.
 * @param {string} [config.cookieName]  Flow cookie name. Default `aa_sso_flow`.
 * @param {string} [config.cookiePath]  Flow cookie path. Default `/`.
 * @param {string} [config.cookieDomain] Flow cookie domain. Default: host-only.
 * @param {number} [config.flowTtlSeconds]     Flow cookie lifetime. Default 600.
 * @param {number} [config.clockSkewSeconds]   Allowed clock skew. Default 60.
 * @param {number} [config.metadataTtlSeconds] Discovery/JWKS cache lifetime. Default 3600.
 * @param {typeof fetch} [config.fetch]  Fetch implementation. Default `globalThis.fetch`.
 * @param {() => number} [config.now]    Clock in seconds since the epoch (tests).
 * @returns {AnointedClient}
 * @throws {AnointedSsoError} `config` when anything is missing or invalid.
 */
export function createAnointedClient(config) {
  if (!isPlainObject(config)) throw new AnointedSsoError("config", "config object is required");

  const issuer = requireConfigString(config, "issuer");
  parseSecureUrl(issuer, "issuer", "config");
  const clientId = requireConfigString(config, "clientId");
  const clientSecret = requireConfigString(config, "clientSecret");
  const redirectUri = requireConfigString(config, "redirectUri");
  const redirectUrl = parseSecureUrl(redirectUri, "redirectUri", "config");

  const scope = optionOrDefault(config, "scope");
  if (typeof scope !== "string") throw new AnointedSsoError("config", "scope must be a string");
  const scopes = scope.split(" ").filter((part) => part !== "");
  if (!scopes.includes("openid")) throw new AnointedSsoError("config", "scope must include openid");

  const cookieName = optionOrDefault(config, "cookieName");
  const cookiePath = optionOrDefault(config, "cookiePath");
  if (typeof cookieName !== "string" || cookieName === "") throw new AnointedSsoError("config", "cookieName must be a string");
  if (typeof cookiePath !== "string" || !cookiePath.startsWith("/")) throw new AnointedSsoError("config", "cookiePath must start with /");
  const flowTtlSeconds = positiveNumberOption(config, "flowTtlSeconds");
  const clockSkewSeconds = positiveNumberOption(config, "clockSkewSeconds");
  const metadataTtlSeconds = positiveNumberOption(config, "metadataTtlSeconds");

  const fetchImpl = config.fetch === undefined ? globalThis.fetch : config.fetch;
  if (typeof fetchImpl !== "function") throw new AnointedSsoError("config", "no fetch implementation is available");
  const clock = config.now === undefined ? () => Math.floor(Date.now() / 1000) : config.now;
  if (typeof clock !== "function") throw new AnointedSsoError("config", "now must be a function");
  if (config.requireVerifiedEmail !== undefined && typeof config.requireVerifiedEmail !== "boolean") {
    throw new AnointedSsoError("config", "requireVerifiedEmail must be a boolean");
  }
  const requireVerifiedEmail =
    config.requireVerifiedEmail === undefined ? scopes.includes("email") : config.requireVerifiedEmail;

  const cookieOptions = {
    path: cookiePath,
    maxAge: flowTtlSeconds,
    httpOnly: true,
    secure: redirectUrl.protocol === "https:",
    sameSite: "Lax",
  };
  if (config.cookieDomain !== undefined) {
    if (typeof config.cookieDomain !== "string" || config.cookieDomain === "") {
      throw new AnointedSsoError("config", "cookieDomain must be a non-empty string");
    }
    cookieOptions.domain = config.cookieDomain;
  }
  // Throws now (config time) on an invalid cookie name rather than on the first sign-in.
  try {
    serializeCookie(cookieName, "", cookieOptions);
  } catch (err) {
    throw new AnointedSsoError("config", `cookieName ${cookieName} is not a valid cookie name`, { cause: err });
  }

  let discoveryCache = null;
  let discoveryPending = null;
  let jwksCache = null;
  let jwksPending = null;

  /**
   * @summary    Fetch and parse a JSON response, raising a coded error on transport or HTTP failure.
   * @param {string} url
   * @param {RequestInit} init
   * @param {string} failCode
   * @returns {Promise<any>} Parsed body, or null for an empty body.
   */
  async function requestJson(url, init, failCode) {
    let response;
    try {
      response = await fetchImpl(url, init);
    } catch (err) {
      throw new AnointedSsoError(failCode, `${url} request failed: ${err instanceof Error ? err.message : String(err)}`, {
        cause: err,
      });
    }
    const text = await response.text();
    let body = null;
    if (text !== "") {
      try {
        body = JSON.parse(text);
      } catch {
        body = null;
      }
    }
    if (!response.ok) {
      const oauthError = isPlainObject(body) && typeof body.error === "string" ? body.error : undefined;
      const detail = oauthError === undefined ? "" : ` ${oauthError}`;
      throw new AnointedSsoError(failCode, `${url} -> ${response.status}${detail}`, {
        status: response.status,
        oauthError,
      });
    }
    return body;
  }

  /**
   * @summary    POST an application/x-www-form-urlencoded body authenticated with the client secret.
   * @param {string} url
   * @param {Record<string, string>} fields
   * @param {string} failCode
   * @returns {Promise<any>}
   */
  function postForm(url, fields, failCode) {
    const body = new URLSearchParams({ ...fields, client_id: clientId, client_secret: clientSecret });
    return requestJson(
      url,
      {
        method: "POST",
        headers: { "content-type": "application/x-www-form-urlencoded", accept: "application/json" },
        body: body.toString(),
      },
      failCode,
    );
  }

  /**
   * @summary    The issuer's discovery document (cached).
   * @param {{force?: boolean}} [opts]
   * @returns {Promise<Record<string, any>>}
   */
  async function discover(opts = {}) {
    if (opts.force !== true && discoveryCache !== null && discoveryCache.expiresAt > clock()) return discoveryCache.value;
    if (discoveryPending !== null) return discoveryPending;
    discoveryPending = (async () => {
      const url = `${issuer.replace(/\/+$/, "")}/.well-known/openid-configuration`;
      const doc = await requestJson(url, { headers: { accept: "application/json" } }, "discovery");
      if (!isPlainObject(doc)) throw new AnointedSsoError("discovery", "discovery document is not a JSON object");
      if (doc.issuer !== issuer) {
        throw new AnointedSsoError("discovery", `discovery issuer ${String(doc.issuer)} does not match ${issuer}`);
      }
      for (const key of ["authorization_endpoint", "token_endpoint", "jwks_uri"]) {
        if (typeof doc[key] !== "string") throw new AnointedSsoError("discovery", `discovery document has no ${key}`);
        parseSecureUrl(doc[key], key, "discovery");
      }
      if (Array.isArray(doc.code_challenge_methods_supported) && !doc.code_challenge_methods_supported.includes("S256")) {
        throw new AnointedSsoError("discovery", "provider does not advertise PKCE S256");
      }
      discoveryCache = { value: doc, expiresAt: clock() + metadataTtlSeconds };
      return doc;
    })();
    try {
      return await discoveryPending;
    } finally {
      discoveryPending = null;
    }
  }

  /**
   * @summary    The issuer's JWKS (cached).
   * @param {{force?: boolean}} [opts]
   * @returns {Promise<{keys: Array<Record<string, any>>}>}
   */
  async function getJwks(opts = {}) {
    if (opts.force !== true && jwksCache !== null && jwksCache.expiresAt > clock()) return jwksCache.value;
    if (jwksPending !== null) return jwksPending;
    jwksPending = (async () => {
      const doc = await discover();
      const jwks = await requestJson(doc.jwks_uri, { headers: { accept: "application/json" } }, "http");
      if (!isPlainObject(jwks) || !Array.isArray(jwks.keys)) throw new AnointedSsoError("http", "JWKS has no keys array");
      jwksCache = { value: jwks, expiresAt: clock() + metadataTtlSeconds };
      return jwks;
    })();
    try {
      return await jwksPending;
    } finally {
      jwksPending = null;
    }
  }

  /**
   * @summary    Verify a JWS against the JWKS, refetching once on an unknown `kid` (key rotation).
   * @param {string} token
   * @returns {Promise<{header: Record<string, any>, payload: Record<string, any>}>}
   */
  async function verifyWithJwks(token) {
    try {
      return await verifyJwsRs256(token, await getJwks());
    } catch (err) {
      if (err instanceof Error && err.unknownKid === true) return verifyJwsRs256(token, await getJwks({ force: true }));
      throw err;
    }
  }

  /**
   * @summary    Verify an ID token's signature and claims.
   * @param {string} idToken
   * @param {string | null} nonce  Expected nonce, or null to skip (refresh).
   * @returns {Promise<Record<string, any>>} The claims.
   */
  async function validateIdToken(idToken, nonce) {
    try {
      const { payload } = await verifyWithJwks(idToken);
      validateIdTokenClaims(payload, { issuer, clientId, nonce, now: clock(), clockSkewSeconds });
      return payload;
    } catch (err) {
      if (err instanceof AnointedSsoError) throw err;
      throw new AnointedSsoError("id_token_invalid", `id_token rejected: ${err instanceof Error ? err.message : String(err)}`, {
        cause: err,
      });
    }
  }

  /**
   * @summary    The app-facing user from validated ID token claims.
   * @param {Record<string, any>} claims
   * @returns {AnointedUser}
   */
  function userFromClaims(claims) {
    const emailVerified = claims.email_verified === true;
    const email = emailVerified && typeof claims.email === "string" && claims.email.trim() !== "" ? claims.email.trim() : null;
    if (requireVerifiedEmail && email === null) {
      throw new AnointedSsoError("email_not_verified", "the ID token carries no verified email");
    }
    return {
      sub: claims.sub,
      email,
      emailVerified: email !== null,
      name: stringOrNull(claims.name),
      preferredUsername: stringOrNull(claims.preferred_username),
      givenName: stringOrNull(claims.given_name),
      familyName: stringOrNull(claims.family_name),
      picture: stringOrNull(claims.picture),
      sid: stringOrNull(claims.sid),
    };
  }

  /**
   * @summary    Normalize a token endpoint response.
   * @param {Record<string, any>} raw
   * @returns {AnointedTokens}
   */
  function normalizeTokens(raw) {
    return {
      accessToken: stringOrNull(raw.access_token),
      idToken: stringOrNull(raw.id_token),
      refreshToken: stringOrNull(raw.refresh_token),
      tokenType: stringOrNull(raw.token_type),
      scope: stringOrNull(raw.scope),
      expiresAt: typeof raw.expires_in === "number" ? clock() + raw.expires_in : null,
    };
  }

  /**
   * @summary    Decode the flow cookie value; null when absent or malformed.
   * @param {unknown} value
   * @returns {{state: string, nonce: string, verifier: string, createdAt: number, returnTo: string | null} | null}
   */
  function decodeFlowCookie(value) {
    if (typeof value !== "string" || value === "") return null;
    let body;
    try {
      body = base64UrlToJson(value);
    } catch {
      return null;
    }
    if (!isPlainObject(body)) return null;
    if (typeof body.s !== "string" || typeof body.n !== "string" || typeof body.v !== "string" || typeof body.t !== "number") {
      return null;
    }
    if (body.s === "" || body.n === "" || body.v === "") return null;
    return { state: body.s, nonce: body.n, verifier: body.v, createdAt: body.t, returnTo: safeReturnTo(body.r) };
  }

  /**
   * @summary    Start a sign-in: the authorize URL plus the flow cookie to set on the redirect response.
   * @param {object} [opts]
   * @param {string} [opts.returnTo]    Same-site path to land on after sign-in (dropped if unsafe).
   * @param {string} [opts.prompt]      e.g. "login" to force re-authentication.
   * @param {string} [opts.loginHint]   Pre-fills the sign-in page.
   * @param {Record<string, string>} [opts.extraParams] Additional authorize parameters (not the reserved ones).
   * @returns {Promise<{url: string, state: string, cookie: {name: string, value: string, options: object}, setCookieHeader: string}>}
   */
  async function createAuthorizeRequest(opts = {}) {
    if (!isPlainObject(opts)) throw new AnointedSsoError("config", "options must be an object");
    const doc = await discover();
    const state = await randomBase64Url(32);
    const nonce = await randomBase64Url(32);
    const verifier = await randomBase64Url(48);
    const challenge = await sha256Base64Url(verifier);

    const url = new URL(doc.authorization_endpoint);
    url.searchParams.set("client_id", clientId);
    url.searchParams.set("redirect_uri", redirectUri);
    url.searchParams.set("response_type", "code");
    url.searchParams.set("scope", scopes.join(" "));
    url.searchParams.set("state", state);
    url.searchParams.set("nonce", nonce);
    url.searchParams.set("code_challenge", challenge);
    url.searchParams.set("code_challenge_method", "S256");
    if (opts.prompt !== undefined) url.searchParams.set("prompt", String(opts.prompt));
    if (opts.loginHint !== undefined) url.searchParams.set("login_hint", String(opts.loginHint));
    if (opts.extraParams !== undefined) {
      if (!isPlainObject(opts.extraParams)) throw new AnointedSsoError("config", "extraParams must be an object");
      for (const [key, value] of Object.entries(opts.extraParams)) {
        if (RESERVED_AUTHORIZE_PARAMS.has(key)) throw new AnointedSsoError("config", `extraParams may not set ${key}`);
        url.searchParams.set(key, String(value));
      }
    }

    const flow = { s: state, n: nonce, v: verifier, t: clock() };
    const returnTo = safeReturnTo(opts.returnTo);
    if (returnTo !== null) flow.r = returnTo;
    const value = jsonToBase64Url(flow);
    return {
      url: url.toString(),
      state,
      cookie: { name: cookieName, value, options: { ...cookieOptions } },
      setCookieHeader: serializeCookie(cookieName, value, cookieOptions),
    };
  }

  /**
   * @summary    Finish a sign-in on the callback request.
   * @description
   *   In order: the flow cookie must exist; `state` must match it (constant time) before anything else,
   *   on success and error alike; the flow must not be older than `flowTtlSeconds`; `iss`, when sent, must
   *   be our issuer (RFC 9207); a provider `error` is raised as `access_denied` / `provider_error`; the
   *   code is exchanged with the client secret and PKCE verifier; the ID token is verified (RS256 via
   *   JWKS, iss, aud/azp, exp/iat with skew, nonce, sub) and, when required, a verified email is enforced.
   *   Always clear the flow cookie afterwards (`clearFlowCookieHeader()`), on success or failure.
   * @param {object} input
   * @param {string} [input.url]  The full (or path-relative) callback request URL.
   * @param {URLSearchParams | Record<string, string>} [input.query]  The callback query, instead of `url`.
   * @param {string | undefined} input.cookieValue  The flow cookie value from the request.
   * @returns {Promise<{user: AnointedUser, claims: Record<string, any>, tokens: AnointedTokens, returnTo: string | null}>}
   */
  async function handleCallback(input) {
    if (!isPlainObject(input)) throw new AnointedSsoError("config", "handleCallback needs { url | query, cookieValue }");
    let params;
    if (typeof input.url === "string") {
      params = new URL(input.url, redirectUri).searchParams;
    } else if (input.query instanceof URLSearchParams) {
      params = input.query;
    } else if (isPlainObject(input.query)) {
      params = new URLSearchParams(Object.entries(input.query).filter(([, value]) => typeof value === "string"));
    } else {
      throw new AnointedSsoError("config", "handleCallback needs url or query");
    }

    const flow = decodeFlowCookie(input.cookieValue);
    if (flow === null) throw new AnointedSsoError("flow_missing", "the sign-in flow cookie is missing or malformed");
    if (!timingSafeEqual(params.get("state"), flow.state)) {
      throw new AnointedSsoError("state_mismatch", "state does not match the flow cookie");
    }
    if (clock() - flow.createdAt > flowTtlSeconds) throw new AnointedSsoError("flow_expired", "the sign-in flow expired");
    const iss = params.get("iss");
    if (iss !== null && iss !== issuer) throw new AnointedSsoError("issuer_mismatch", `callback iss ${iss} does not match ${issuer}`);
    const error = params.get("error");
    if (error !== null) {
      const description = params.get("error_description");
      const message = description === null ? error : `${error}: ${description}`;
      throw new AnointedSsoError(error === "access_denied" ? "access_denied" : "provider_error", message, { oauthError: error });
    }
    const code = params.get("code");
    if (code === null || code === "") throw new AnointedSsoError("missing_code", "the callback has no code");

    const doc = await discover();
    const raw = await postForm(
      doc.token_endpoint,
      { grant_type: "authorization_code", code, redirect_uri: redirectUri, code_verifier: flow.verifier },
      "token_exchange",
    );
    if (!isPlainObject(raw) || typeof raw.id_token !== "string") {
      throw new AnointedSsoError("token_exchange", "the token response has no id_token");
    }
    const claims = await validateIdToken(raw.id_token, flow.nonce);
    return { user: userFromClaims(claims), claims, tokens: normalizeTokens(raw), returnTo: flow.returnTo };
  }

  /**
   * @summary    Refresh tokens (needs the `offline_access` scope). Each refresh returns a NEW refresh token.
   * @description
   *   `invalid_grant` means the user disconnected the app, was suspended, or the app was disabled: treat it
   *   as signed out (the error has code `token_exchange` and `oauthError: "invalid_grant"`).
   * @param {string} refreshToken
   * @param {{expectedSub?: string}} [opts]  When set, a returned ID token must name this `sub`.
   * @returns {Promise<{tokens: AnointedTokens, claims: Record<string, any> | null}>}
   */
  async function refresh(refreshToken, opts = {}) {
    if (typeof refreshToken !== "string" || refreshToken === "") throw new AnointedSsoError("config", "refreshToken is required");
    const doc = await discover();
    const raw = await postForm(doc.token_endpoint, { grant_type: "refresh_token", refresh_token: refreshToken }, "token_exchange");
    if (!isPlainObject(raw)) throw new AnointedSsoError("token_exchange", "the token response is not a JSON object");
    let claims = null;
    if (typeof raw.id_token === "string") {
      claims = await validateIdToken(raw.id_token, null);
      if (opts.expectedSub !== undefined && claims.sub !== opts.expectedSub) {
        throw new AnointedSsoError("id_token_invalid", "refreshed id_token names a different sub");
      }
    }
    return { tokens: normalizeTokens(raw), claims };
  }

  /**
   * @summary    Revoke a token (RFC 7009). Call it with the refresh token when the user signs out.
   * @param {string} token
   * @param {{tokenTypeHint?: "refresh_token" | "access_token"}} [opts]
   * @returns {Promise<void>}
   */
  async function revoke(token, opts = {}) {
    if (typeof token !== "string" || token === "") throw new AnointedSsoError("config", "token is required");
    const doc = await discover();
    if (typeof doc.revocation_endpoint !== "string") throw new AnointedSsoError("discovery", "provider has no revocation_endpoint");
    const tokenTypeHint = opts.tokenTypeHint === undefined ? "refresh_token" : opts.tokenTypeHint;
    await postForm(doc.revocation_endpoint, { token, token_type_hint: tokenTypeHint }, "http");
  }

  /**
   * @summary    Call UserInfo with an access token.
   * @param {string} accessToken
   * @returns {Promise<Record<string, any>>} Standard OIDC claims for the approved scopes.
   */
  async function fetchUserInfo(accessToken) {
    if (typeof accessToken !== "string" || accessToken === "") throw new AnointedSsoError("config", "accessToken is required");
    const doc = await discover();
    if (typeof doc.userinfo_endpoint !== "string") throw new AnointedSsoError("discovery", "provider has no userinfo_endpoint");
    const info = await requestJson(
      doc.userinfo_endpoint,
      { headers: { authorization: `Bearer ${accessToken}`, accept: "application/json" } },
      "http",
    );
    if (!isPlainObject(info) || typeof info.sub !== "string") throw new AnointedSsoError("http", "userinfo response has no sub");
    return info;
  }

  /**
   * @summary    The provider logout (end session) URL to send the browser to.
   * @param {{idTokenHint?: string, postLogoutRedirectUri?: string, state?: string}} [opts]
   * @returns {Promise<string>}
   */
  async function buildLogoutUrl(opts = {}) {
    const doc = await discover();
    if (typeof doc.end_session_endpoint !== "string") throw new AnointedSsoError("discovery", "provider has no end_session_endpoint");
    const url = new URL(doc.end_session_endpoint);
    url.searchParams.set("client_id", clientId);
    if (opts.idTokenHint !== undefined) url.searchParams.set("id_token_hint", opts.idTokenHint);
    if (opts.postLogoutRedirectUri !== undefined) {
      parseSecureUrl(opts.postLogoutRedirectUri, "postLogoutRedirectUri", "config");
      url.searchParams.set("post_logout_redirect_uri", opts.postLogoutRedirectUri);
    }
    if (opts.state !== undefined) url.searchParams.set("state", opts.state);
    return url.toString();
  }

  /**
   * @summary    Validate a back-channel Logout Token (OpenID Connect Back-Channel Logout 1.0).
   * @description
   *   Checks: RS256 signature via JWKS, header `typ: logout+jwt`, iss, aud/azp, iat recent (maxAgeSeconds
   *   plus skew) and not in the future, exp (when present) not passed, the back-channel logout event, no
   *   `nonce`, a `jti`, and `sub` and/or `sid`. Replay protection is the caller's job: remember each `jti`
   *   until its `exp` and answer 200 to a repeat without acting again.
   * @param {string} logoutToken  The `logout_token` form field.
   * @param {{maxAgeSeconds?: number}} [opts]  Default 300.
   * @returns {Promise<{sub: string | null, sid: string | null, jti: string, exp: number | null, claims: Record<string, any>}>}
   * @throws {AnointedSsoError} `logout_token_invalid` (answer 400) or `http` when the JWKS cannot be read (answer 5xx).
   */
  async function verifyLogoutToken(logoutToken, opts = {}) {
    const maxAgeSeconds = opts.maxAgeSeconds === undefined ? 300 : opts.maxAgeSeconds;
    try {
      const { header, payload } = await verifyWithJwks(logoutToken);
      if (header.typ !== "logout+jwt") throw new Error("typ is not logout+jwt");
      if (payload.iss !== issuer) throw new Error(`iss ${String(payload.iss)} does not match ${issuer}`);
      assertAudience(payload, clientId);
      const now = clock();
      if (typeof payload.iat !== "number") throw new Error("iat is missing");
      if (payload.iat - clockSkewSeconds > now) throw new Error("token was issued in the future");
      if (now - payload.iat > maxAgeSeconds + clockSkewSeconds) throw new Error("token is too old");
      if (payload.exp !== undefined && (typeof payload.exp !== "number" || now > payload.exp + clockSkewSeconds)) {
        throw new Error("token is expired");
      }
      if (!isPlainObject(payload.events) || !isPlainObject(payload.events[BACKCHANNEL_LOGOUT_EVENT])) {
        throw new Error("events does not carry the back-channel logout event");
      }
      if (Object.prototype.hasOwnProperty.call(payload, "nonce")) throw new Error("a logout token must not carry a nonce");
      if (typeof payload.jti !== "string" || payload.jti === "") throw new Error("jti is missing");
      const sub = stringOrNull(payload.sub);
      const sid = stringOrNull(payload.sid);
      if (sub === null && sid === null) throw new Error("neither sub nor sid is present");
      return { sub, sid, jti: payload.jti, exp: typeof payload.exp === "number" ? payload.exp : null, claims: payload };
    } catch (err) {
      if (err instanceof AnointedSsoError) throw err;
      throw new AnointedSsoError(
        "logout_token_invalid",
        `logout token rejected: ${err instanceof Error ? err.message : String(err)}`,
        { cause: err },
      );
    }
  }

  /**
   * @summary    The cookie (name, empty value, Max-Age 0) that clears the flow cookie.
   * @returns {{name: string, value: string, options: object}}
   */
  function clearFlowCookie() {
    return { name: cookieName, value: "", options: { ...cookieOptions, maxAge: 0 } };
  }

  /**
   * @summary    `Set-Cookie` header value that clears the flow cookie.
   * @returns {string}
   */
  function clearFlowCookieHeader() {
    return serializeCookie(cookieName, "", { ...cookieOptions, maxAge: 0 });
  }

  return Object.freeze({
    issuer,
    clientId,
    redirectUri,
    scope: scopes.join(" "),
    cookieName,
    discover,
    getJwks,
    createAuthorizeRequest,
    handleCallback,
    refresh,
    revoke,
    fetchUserInfo,
    buildLogoutUrl,
    verifyLogoutToken,
    decodeFlowCookie,
    clearFlowCookie,
    clearFlowCookieHeader,
  });
}

/**
 * @typedef {object} AnointedUser
 * @property {string} sub                     Stable Anointed Automation user id. Key your users on this.
 * @property {string | null} email            Verified email only; null when absent or unverified.
 * @property {boolean} emailVerified
 * @property {string | null} name             Needs the `profile` scope.
 * @property {string | null} preferredUsername Needs the `profile` scope.
 * @property {string | null} givenName        Needs the `profile` scope.
 * @property {string | null} familyName       Needs the `profile` scope.
 * @property {string | null} picture          Needs the `profile` scope.
 * @property {string | null} sid              The grant id that back-channel logout names.
 */

/**
 * @typedef {object} AnointedTokens
 * @property {string | null} accessToken
 * @property {string | null} idToken
 * @property {string | null} refreshToken  Present with the `offline_access` scope.
 * @property {string | null} tokenType
 * @property {string | null} scope
 * @property {number | null} expiresAt     Seconds since the epoch.
 */

/** @typedef {ReturnType<typeof createAnointedClient>} AnointedClient */
