// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// A local, in-process OpenID Connect provider that mimics api.anointedautomation.net closely enough to run
// the whole sign-in flow in tests: discovery, JWKS (with key rotation), authorize (PKCE S256, state, nonce,
// RFC 9207 iss), token (authorization_code + refresh_token with rotation), userinfo, revoke and logout.

import http from "node:http";
import crypto from "node:crypto";

export const CLIENT_ID = "aa_6ac0d092b22e3a5d3e042836";
export const CLIENT_SECRET = "test-only-secret-not-real";
export const REDIRECT_URI = "http://localhost:3000/auth/anointed/callback";

/**
 * @summary    Encode a JSON value as base64url.
 * @param {unknown} value
 * @returns {string}
 */
function b64json(value) {
  return Buffer.from(JSON.stringify(value)).toString("base64url");
}

/**
 * @summary    Read a request body as text.
 * @param {http.IncomingMessage} req
 * @returns {Promise<string>}
 */
async function readBody(req) {
  const chunks = [];
  for await (const chunk of req) chunks.push(chunk);
  return Buffer.concat(chunks).toString("utf8");
}

/**
 * @summary    Start the fake provider on an ephemeral loopback port.
 * @returns {Promise<FakeIdp>}
 */
export async function startFakeIdp() {
  const keys = new Map();
  const state = {
    signingKid: "k1",
    publishedKids: ["k1"],
    sub: "4kT9xQ2mZ",
    claims: { email: "jane@example.com", email_verified: true, name: "Jane D", preferred_username: "janed" },
    idTokenTransform: null,
    tokenError: null,
    discoveryPatch: {},
    redirectUris: new Set([REDIRECT_URI]),
    codes: new Map(),
    refreshTokens: new Map(),
    accessTokens: new Map(),
    revoked: [],
    tokenRequests: [],
    discoveryFetches: 0,
    jwksFetches: 0,
  };

  /**
   * @summary    Generate an RSA key under a kid.
   * @param {string} kid
   * @returns {void}
   */
  function addKey(kid) {
    keys.set(kid, crypto.generateKeyPairSync("rsa", { modulusLength: 2048 }));
  }
  addKey("k1");

  let issuer = "";

  /**
   * @summary    Sign claims as a compact RS256 JWS.
   * @param {Record<string, unknown>} claims
   * @param {Record<string, unknown>} [header]
   * @param {string} [kid]
   * @returns {string}
   */
  function sign(claims, header = {}, kid = state.signingKid) {
    const h = b64json({ alg: "RS256", kid, typ: "JWT", ...header });
    const p = b64json(claims);
    const s = crypto.sign("RSA-SHA256", Buffer.from(`${h}.${p}`), keys.get(kid).privateKey).toString("base64url");
    return `${h}.${p}.${s}`;
  }

  /**
   * @summary    Issue an ID token for a code record.
   * @param {{clientId: string, nonce: string | null, sid: string}} record
   * @returns {string}
   */
  function issueIdToken(record) {
    const now = Math.floor(Date.now() / 1000);
    let claims = { iss: issuer, aud: record.clientId, sub: state.sub, iat: now, exp: now + 300, sid: record.sid, ...state.claims };
    if (record.nonce !== null) claims.nonce = record.nonce;
    if (state.idTokenTransform !== null) claims = state.idTokenTransform(claims);
    return sign(claims);
  }

  /**
   * @summary    Issue an access + refresh token pair.
   * @returns {{access: string, refresh: string}}
   */
  function issuePair() {
    const access = crypto.randomBytes(16).toString("hex");
    const refresh = crypto.randomBytes(16).toString("hex");
    state.accessTokens.set(access, { sub: state.sub });
    state.refreshTokens.set(refresh, { sub: state.sub });
    return { access, refresh };
  }

  /**
   * @summary    Send JSON.
   * @param {http.ServerResponse} res
   * @param {number} status
   * @param {unknown} body
   * @returns {void}
   */
  function json(res, status, body) {
    res.writeHead(status, { "content-type": "application/json" });
    res.end(JSON.stringify(body));
  }

  const server = http.createServer(async (req, res) => {
    const url = new URL(req.url, issuer);
    const body = req.method === "POST" ? new URLSearchParams(await readBody(req)) : new URLSearchParams();

    if (url.pathname === "/.well-known/openid-configuration") {
      state.discoveryFetches += 1;
      return json(res, 200, {
        issuer,
        authorization_endpoint: `${issuer}connect/authorize`,
        token_endpoint: `${issuer}connect/token`,
        userinfo_endpoint: `${issuer}connect/userinfo`,
        revocation_endpoint: `${issuer}connect/revoke`,
        end_session_endpoint: `${issuer}connect/logout`,
        jwks_uri: `${issuer}.well-known/jwks`,
        code_challenge_methods_supported: ["S256"],
        backchannel_logout_supported: true,
        ...state.discoveryPatch,
      });
    }
    if (url.pathname === "/.well-known/jwks") {
      state.jwksFetches += 1;
      const published = state.publishedKids.map((kid) => ({
        ...keys.get(kid).publicKey.export({ format: "jwk" }),
        kid,
        use: "sig",
        alg: "RS256",
      }));
      return json(res, 200, { keys: published });
    }
    if (url.pathname === "/connect/authorize") {
      const q = url.searchParams;
      if (q.get("client_id") !== CLIENT_ID || !state.redirectUris.has(q.get("redirect_uri"))) return json(res, 400, { error: "invalid_client" });
      if (q.get("code_challenge_method") !== "S256" || !q.get("state")) return json(res, 400, { error: "invalid_request" });
      const code = crypto.randomBytes(16).toString("hex");
      state.codes.set(code, {
        clientId: CLIENT_ID,
        redirectUri: q.get("redirect_uri"),
        challenge: q.get("code_challenge"),
        nonce: q.get("nonce"),
        scope: q.get("scope"),
        sid: crypto.randomBytes(12).toString("hex"),
        used: false,
      });
      const back = new URL(q.get("redirect_uri"));
      back.searchParams.set("code", code);
      back.searchParams.set("state", q.get("state"));
      back.searchParams.set("iss", issuer);
      res.writeHead(302, { location: back.toString() });
      return res.end();
    }
    if (url.pathname === "/connect/token") {
      state.tokenRequests.push(Object.fromEntries(body));
      if (state.tokenError !== null) return json(res, state.tokenError.status, { error: state.tokenError.error });
      if (body.get("client_id") !== CLIENT_ID || body.get("client_secret") !== CLIENT_SECRET) {
        return json(res, 401, { error: "invalid_client" });
      }
      if (body.get("grant_type") === "authorization_code") {
        const record = state.codes.get(body.get("code"));
        if (record === undefined || record.used || record.redirectUri !== body.get("redirect_uri")) {
          return json(res, 400, { error: "invalid_grant" });
        }
        const challenge = crypto.createHash("sha256").update(String(body.get("code_verifier"))).digest("base64url");
        if (challenge !== record.challenge) return json(res, 400, { error: "invalid_grant" });
        record.used = true;
        const pair = issuePair();
        return json(res, 200, {
          access_token: pair.access,
          token_type: "Bearer",
          expires_in: 900,
          id_token: issueIdToken(record),
          refresh_token: pair.refresh,
          scope: record.scope,
        });
      }
      if (body.get("grant_type") === "refresh_token") {
        const token = body.get("refresh_token");
        if (!state.refreshTokens.has(token)) return json(res, 400, { error: "invalid_grant" });
        state.refreshTokens.delete(token);
        const pair = issuePair();
        return json(res, 200, {
          access_token: pair.access,
          token_type: "Bearer",
          expires_in: 900,
          id_token: issueIdToken({ clientId: CLIENT_ID, nonce: null, sid: "refresh-sid" }),
          refresh_token: pair.refresh,
        });
      }
      return json(res, 400, { error: "unsupported_grant_type" });
    }
    if (url.pathname === "/connect/userinfo") {
      const auth = String(req.headers.authorization);
      const entry = state.accessTokens.get(auth.replace(/^Bearer /, ""));
      if (entry === undefined) return json(res, 401, { error: "invalid_token" });
      return json(res, 200, { sub: entry.sub, ...state.claims });
    }
    if (url.pathname === "/connect/revoke") {
      if (body.get("client_secret") !== CLIENT_SECRET) return json(res, 401, { error: "invalid_client" });
      state.revoked.push({ token: body.get("token"), hint: body.get("token_type_hint") });
      state.refreshTokens.delete(body.get("token"));
      res.writeHead(200);
      return res.end();
    }
    return json(res, 404, { error: "not_found" });
  });

  await new Promise((resolve) => server.listen(0, "127.0.0.1", resolve));
  issuer = `http://127.0.0.1:${server.address().port}/`;

  return {
    issuer,
    state,
    sign,
    /**
     * @summary    Start signing with a new key; publish it too unless told not to.
     * @param {string} kid
     * @param {{publish?: boolean, dropOld?: boolean}} [opts]
     * @returns {void}
     */
    rotateKey(kid, opts = {}) {
      addKey(kid);
      state.signingKid = kid;
      if (opts.dropOld === true) state.publishedKids = [];
      if (opts.publish !== false) state.publishedKids.push(kid);
    },
    /**
     * @summary    Play the browser on the authorize URL: returns the callback URL the provider redirects to.
     * @param {string} authorizeUrl
     * @returns {Promise<string>}
     */
    async authorize(authorizeUrl) {
      const res = await fetch(authorizeUrl, { redirect: "manual" });
      if (res.status !== 302) throw new Error(`authorize returned ${res.status}: ${await res.text()}`);
      return res.headers.get("location");
    },
    /**
     * @summary    Stop the server.
     * @returns {Promise<void>}
     */
    close() {
      return new Promise((resolve) => server.close(() => resolve()));
    },
  };
}

/** @typedef {Awaited<ReturnType<typeof startFakeIdp>>} FakeIdp */
