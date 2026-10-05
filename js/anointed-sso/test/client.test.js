// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Unit tests for createAnointedClient plus a full mocked flow against the local fake provider.

import { after, before, beforeEach, test } from "node:test";
import assert from "node:assert/strict";
import crypto from "node:crypto";
import {
  ANOINTED_ISSUER,
  AnointedSsoError,
  BACKCHANNEL_LOGOUT_EVENT,
  DEFAULT_SCOPE,
  createAnointedClient,
  safeReturnTo,
} from "../src/index.js";
import { CLIENT_ID, CLIENT_SECRET, REDIRECT_URI, startFakeIdp } from "./helpers/fakeIdp.js";

let idp;

before(async () => {
  idp = await startFakeIdp();
});

after(async () => {
  await idp.close();
});

beforeEach(() => {
  idp.state.claims = { email: "jane@example.com", email_verified: true, name: "Jane D", preferred_username: "janed" };
  idp.state.idTokenTransform = null;
  idp.state.tokenError = null;
  idp.state.discoveryPatch = {};
});

/**
 * @summary    A client wired to the fake provider.
 * @param {object} [overrides]
 * @returns {ReturnType<typeof createAnointedClient>}
 */
function makeClient(overrides = {}) {
  return createAnointedClient({
    issuer: idp.issuer,
    clientId: CLIENT_ID,
    clientSecret: CLIENT_SECRET,
    redirectUri: REDIRECT_URI,
    ...overrides,
  });
}

/**
 * @summary    Assert a promise rejects with an AnointedSsoError of the given code.
 * @param {Promise<unknown>} promise
 * @param {string} code
 * @returns {Promise<void>}
 */
async function rejectsWith(promise, code) {
  await assert.rejects(promise, (err) => {
    assert.ok(err instanceof AnointedSsoError, `expected AnointedSsoError, got ${err}`);
    assert.equal(err.code, code, err.message);
    return true;
  });
}

/**
 * @summary    Run start + browser authorize and return the pieces the callback needs.
 * @param {ReturnType<typeof createAnointedClient>} client
 * @param {object} [opts]
 * @returns {Promise<{auth: any, callbackUrl: string}>}
 */
async function startAndAuthorize(client, opts = {}) {
  const auth = await client.createAuthorizeRequest(opts);
  const callbackUrl = await idp.authorize(auth.url);
  return { auth, callbackUrl };
}

test("exports the production issuer and the email-only default scope", () => {
  assert.equal(ANOINTED_ISSUER, "https://api.anointedautomation.net/");
  assert.equal(DEFAULT_SCOPE, "openid email");
});

test("config is validated up front (fail fast)", () => {
  const base = { issuer: ANOINTED_ISSUER, clientId: CLIENT_ID, clientSecret: CLIENT_SECRET, redirectUri: "https://app.example/cb" };
  assert.doesNotThrow(() => createAnointedClient(base));
  const bad = [
    undefined,
    null,
    { ...base, issuer: undefined },
    { ...base, issuer: "" },
    { ...base, issuer: "http://api.anointedautomation.net/" },
    { ...base, issuer: "not a url" },
    { ...base, clientId: " " },
    { ...base, clientSecret: "" },
    { ...base, clientSecret: 7 },
    { ...base, redirectUri: undefined },
    { ...base, redirectUri: 7 },
    { ...base, allowedRedirectUris: [] },
    { ...base, allowedRedirectUris: "https://app.example/cb" },
    { ...base, allowedRedirectUris: ["http://app.example/cb"] },
    { ...base, allowedRedirectUris: ["https://other.example/cb"] },
    { ...base, allowedReturnOrigins: "https://app.example" },
    { ...base, allowedReturnOrigins: ["http://app.example"] },
    { ...base, allowedReturnOrigins: ["https://app.example/path"] },
    { ...base, allowedReturnOrigins: ["https://*.com"] },
    { ...base, allowedReturnOrigins: ["http://*.localhost"] },
    { ...base, allowedReturnOrigins: ["javascript:alert(1)"] },
    { ...base, allowedReturnOrigins: [""] },
    { ...base, isAllowedReturnTo: true },
    { ...base, redirectUri: "http://app.example/cb" },
    { ...base, redirectUri: "https://app.example/cb#frag" },
    { ...base, scope: "email profile" },
    { ...base, scope: 7 },
    { ...base, cookieName: "bad name" },
    { ...base, cookiePath: "relative" },
    { ...base, flowTtlSeconds: 0 },
    { ...base, clockSkewSeconds: -1 },
    { ...base, metadataTtlSeconds: Number.NaN },
    { ...base, fetch: "nope" },
    { ...base, now: 5 },
    { ...base, requireVerifiedEmail: "yes" },
    { ...base, cookieDomain: "" },
  ];
  for (const config of bad) {
    assert.throws(() => createAnointedClient(config), (err) => err instanceof AnointedSsoError && err.code === "config", JSON.stringify(config));
  }
});

test("the client never exposes the secret", () => {
  const client = makeClient();
  assert.equal(JSON.stringify(client).includes(CLIENT_SECRET), false);
  assert.equal(Object.values(client).includes(CLIENT_SECRET), false);
  assert.ok(Object.isFrozen(client));
});

test("safeReturnTo keeps same-site paths and drops open-redirect shapes", () => {
  assert.equal(safeReturnTo("/account?tab=1#x"), "/account?tab=1#x");
  for (const value of [undefined, "", "account", "https://evil.example", "//evil.example", "/\\evil.example", "/a\\b", "/a\nb", "/" + "a".repeat(2048), 5]) {
    assert.equal(safeReturnTo(value), null, String(value));
  }
});

test("createAuthorizeRequest builds a PKCE S256 authorize URL and an httpOnly flow cookie", async () => {
  const client = makeClient();
  const auth = await client.createAuthorizeRequest({ returnTo: "/account", prompt: "login", loginHint: "jane@example.com" });
  const url = new URL(auth.url);
  assert.equal(`${url.origin}${url.pathname}`, `${idp.issuer}connect/authorize`);
  const q = url.searchParams;
  assert.equal(q.get("client_id"), CLIENT_ID);
  assert.equal(q.get("redirect_uri"), REDIRECT_URI);
  assert.equal(q.get("response_type"), "code");
  assert.equal(q.get("scope"), "openid email");
  assert.equal(q.get("code_challenge_method"), "S256");
  assert.equal(q.get("prompt"), "login");
  assert.equal(q.get("login_hint"), "jane@example.com");
  assert.equal(q.get("state"), auth.state);
  assert.equal(q.get("state").length, 43);
  assert.equal(q.get("nonce").length, 43);

  const flow = client.decodeFlowCookie(auth.cookie.value);
  assert.equal(flow.state, auth.state);
  assert.equal(flow.nonce, q.get("nonce"));
  assert.equal(flow.verifier.length, 64);
  assert.equal(flow.returnTo, "/account");
  assert.equal(crypto.createHash("sha256").update(flow.verifier).digest("base64url"), q.get("code_challenge"));

  assert.equal(auth.cookie.name, "aa_sso_flow");
  assert.deepEqual(auth.cookie.options, { path: "/", maxAge: 600, httpOnly: true, secure: false, sameSite: "Lax" });
  assert.match(auth.setCookieHeader, /^aa_sso_flow=[^;]+; Path=\/; Max-Age=600; HttpOnly; SameSite=Lax$/);
});

test("flow cookie is Secure for an https redirect URI and carries a configured domain", async () => {
  const client = makeClient({ redirectUri: "https://app.example/cb", cookieDomain: "app.example", cookiePath: "/auth" });
  const auth = await client.createAuthorizeRequest();
  assert.match(auth.setCookieHeader, /Path=\/auth; Max-Age=600; Domain=app\.example; HttpOnly; Secure; SameSite=Lax$/);
  assert.match(client.clearFlowCookieHeader(), /^aa_sso_flow=; Path=\/auth; Max-Age=0;/);
  assert.equal(client.clearFlowCookie().options.maxAge, 0);
});

test("every authorize request gets fresh state, nonce and verifier", async () => {
  const client = makeClient();
  const a = await client.createAuthorizeRequest();
  const b = await client.createAuthorizeRequest();
  assert.notEqual(a.state, b.state);
  assert.notEqual(client.decodeFlowCookie(a.cookie.value).verifier, client.decodeFlowCookie(b.cookie.value).verifier);
});

test("unsafe returnTo is dropped from the flow, extraParams cannot override protocol parameters", async () => {
  const client = makeClient();
  const auth = await client.createAuthorizeRequest({ returnTo: "https://evil.example", extraParams: { ui_locales: "es" } });
  assert.equal(client.decodeFlowCookie(auth.cookie.value).returnTo, null);
  assert.equal(new URL(auth.url).searchParams.get("ui_locales"), "es");
  for (const key of ["scope", "state", "nonce", "redirect_uri", "client_id", "code_challenge", "code_challenge_method", "response_type"]) {
    await rejectsWith(client.createAuthorizeRequest({ extraParams: { [key]: "x" } }), "config");
  }
});

test("full flow: start, authorize, callback returns the verified user, tokens and returnTo", async () => {
  const client = makeClient();
  const { auth, callbackUrl } = await startAndAuthorize(client, { returnTo: "/account" });
  const result = await client.handleCallback({ url: callbackUrl, cookieValue: auth.cookie.value });
  assert.equal(result.user.sub, "4kT9xQ2mZ");
  assert.equal(result.user.email, "jane@example.com");
  assert.equal(result.user.emailVerified, true);
  assert.equal(result.user.name, "Jane D");
  assert.equal(result.user.preferredUsername, "janed");
  assert.equal(result.user.givenName, null);
  assert.equal(typeof result.user.sid, "string");
  assert.equal(result.returnTo, "/account");
  assert.equal(result.tokens.tokenType, "Bearer");
  assert.equal(typeof result.tokens.accessToken, "string");
  assert.equal(typeof result.tokens.refreshToken, "string");
  assert.ok(result.tokens.expiresAt > Math.floor(Date.now() / 1000));

  const sent = idp.state.tokenRequests.at(-1);
  assert.equal(sent.grant_type, "authorization_code");
  assert.equal(sent.redirect_uri, REDIRECT_URI);
  assert.equal(sent.client_secret, CLIENT_SECRET);
  assert.equal(sent.code_verifier, client.decodeFlowCookie(auth.cookie.value).verifier);
});

test("callback accepts a query object or URLSearchParams instead of a URL", async () => {
  const client = makeClient();
  const first = await startAndAuthorize(client);
  const params = new URL(first.callbackUrl).searchParams;
  assert.equal((await client.handleCallback({ query: params, cookieValue: first.auth.cookie.value })).user.sub, "4kT9xQ2mZ");
  const second = await startAndAuthorize(client);
  const query = Object.fromEntries(new URL(second.callbackUrl).searchParams);
  assert.equal((await client.handleCallback({ query, cookieValue: second.auth.cookie.value })).user.sub, "4kT9xQ2mZ");
  await rejectsWith(client.handleCallback({ cookieValue: second.auth.cookie.value }), "config");
  await rejectsWith(client.handleCallback(undefined), "config");
});

test("a code can only be redeemed once", async () => {
  const client = makeClient();
  const { auth, callbackUrl } = await startAndAuthorize(client);
  await client.handleCallback({ url: callbackUrl, cookieValue: auth.cookie.value });
  await assert.rejects(client.handleCallback({ url: callbackUrl, cookieValue: auth.cookie.value }), (err) => {
    assert.equal(err.code, "token_exchange");
    assert.equal(err.oauthError, "invalid_grant");
    assert.equal(err.status, 400);
    return true;
  });
});

test("state is checked first: missing cookie, foreign cookie and tampered state never reach the token endpoint", async () => {
  const client = makeClient();
  const { auth, callbackUrl } = await startAndAuthorize(client);
  const before = idp.state.tokenRequests.length;
  await rejectsWith(client.handleCallback({ url: callbackUrl, cookieValue: undefined }), "flow_missing");
  await rejectsWith(client.handleCallback({ url: callbackUrl, cookieValue: "garbage!" }), "flow_missing");
  const other = await client.createAuthorizeRequest();
  await rejectsWith(client.handleCallback({ url: callbackUrl, cookieValue: other.cookie.value }), "state_mismatch");
  const tampered = new URL(callbackUrl);
  tampered.searchParams.set("state", `${auth.state.slice(0, -1)}${auth.state.endsWith("A") ? "B" : "A"}`);
  await rejectsWith(client.handleCallback({ url: tampered.toString(), cookieValue: auth.cookie.value }), "state_mismatch");
  const noState = new URL(callbackUrl);
  noState.searchParams.delete("state");
  await rejectsWith(client.handleCallback({ url: noState.toString(), cookieValue: auth.cookie.value }), "state_mismatch");
  // An error response with a forged state is still a state failure, not access_denied.
  await rejectsWith(client.handleCallback({ url: `${REDIRECT_URI}?error=access_denied&state=forged`, cookieValue: auth.cookie.value }), "state_mismatch");
  assert.equal(idp.state.tokenRequests.length, before);
});

test("an expired flow cookie is refused", async () => {
  let now = Math.floor(Date.now() / 1000);
  const client = makeClient({ now: () => now, flowTtlSeconds: 600 });
  const { auth, callbackUrl } = await startAndAuthorize(client);
  now += 601;
  await rejectsWith(client.handleCallback({ url: callbackUrl, cookieValue: auth.cookie.value }), "flow_expired");
});

test("provider errors map to access_denied / provider_error after the state check", async () => {
  const client = makeClient();
  const auth = await client.createAuthorizeRequest();
  await rejectsWith(
    client.handleCallback({ url: `${REDIRECT_URI}?error=access_denied&state=${auth.state}`, cookieValue: auth.cookie.value }),
    "access_denied",
  );
  await assert.rejects(
    client.handleCallback({
      url: `${REDIRECT_URI}?error=server_error&error_description=boom&state=${auth.state}`,
      cookieValue: auth.cookie.value,
    }),
    (err) => err.code === "provider_error" && err.oauthError === "server_error" && /boom/.test(err.message),
  );
});

test("an iss parameter naming another issuer is refused (RFC 9207 mix-up defense)", async () => {
  const client = makeClient();
  const { auth, callbackUrl } = await startAndAuthorize(client);
  const url = new URL(callbackUrl);
  url.searchParams.set("iss", "https://evil.example/");
  await rejectsWith(client.handleCallback({ url: url.toString(), cookieValue: auth.cookie.value }), "issuer_mismatch");
});

test("a callback without a code is refused", async () => {
  const client = makeClient();
  const auth = await client.createAuthorizeRequest();
  await rejectsWith(client.handleCallback({ url: `${REDIRECT_URI}?state=${auth.state}`, cookieValue: auth.cookie.value }), "missing_code");
});

test("ID token claim failures surface as id_token_invalid", async () => {
  const client = makeClient();
  const transforms = [
    (c) => ({ ...c, nonce: "replayed" }),
    (c) => ({ ...c, aud: "aa_someone_else" }),
    (c) => ({ ...c, iss: "https://evil.example/" }),
    (c) => ({ ...c, exp: Math.floor(Date.now() / 1000) - 3600 }),
    (c) => ({ ...c, sub: "" }),
  ];
  for (const transform of transforms) {
    idp.state.idTokenTransform = transform;
    const { auth, callbackUrl } = await startAndAuthorize(client);
    await rejectsWith(client.handleCallback({ url: callbackUrl, cookieValue: auth.cookie.value }), "id_token_invalid");
  }
});

test("a verified email is required by default with the email scope, and an unverified one is never returned", async () => {
  idp.state.claims = { email: "jane@example.com", email_verified: false };
  const strict = makeClient();
  const first = await startAndAuthorize(strict);
  await rejectsWith(strict.handleCallback({ url: first.callbackUrl, cookieValue: first.auth.cookie.value }), "email_not_verified");

  const lenient = makeClient({ requireVerifiedEmail: false });
  const second = await startAndAuthorize(lenient);
  const result = await lenient.handleCallback({ url: second.callbackUrl, cookieValue: second.auth.cookie.value });
  assert.equal(result.user.email, null);
  assert.equal(result.user.emailVerified, false);

  idp.state.claims = {};
  const openidOnly = makeClient({ scope: "openid" });
  const third = await startAndAuthorize(openidOnly);
  assert.equal((await openidOnly.handleCallback({ url: third.callbackUrl, cookieValue: third.auth.cookie.value })).user.email, null);
});

test("token endpoint errors and a missing id_token are token_exchange errors", async () => {
  const client = makeClient();
  idp.state.tokenError = { status: 429, error: "slow_down" };
  const first = await startAndAuthorize(client);
  await assert.rejects(client.handleCallback({ url: first.callbackUrl, cookieValue: first.auth.cookie.value }), (err) => {
    return err.code === "token_exchange" && err.status === 429 && err.oauthError === "slow_down";
  });
  idp.state.tokenError = null;

  const fetchNoIdToken = async (url, init) => {
    if (String(url).endsWith("/connect/token")) return new Response(JSON.stringify({ access_token: "x" }), { status: 200 });
    return fetch(url, init);
  };
  const noId = makeClient({ fetch: fetchNoIdToken });
  const second = await startAndAuthorize(noId);
  await rejectsWith(noId.handleCallback({ url: second.callbackUrl, cookieValue: second.auth.cookie.value }), "token_exchange");
});

test("a network failure is a coded error, not a raw TypeError", async () => {
  const client = makeClient({
    fetch: async () => {
      throw new TypeError("fetch failed");
    },
  });
  await rejectsWith(client.createAuthorizeRequest(), "discovery");
});

test("discovery must name the configured issuer exactly and advertise S256", async () => {
  idp.state.discoveryPatch = { issuer: "https://evil.example/" };
  await rejectsWith(makeClient().discover(), "discovery");
  idp.state.discoveryPatch = { code_challenge_methods_supported: ["plain"] };
  await rejectsWith(makeClient().discover(), "discovery");
  idp.state.discoveryPatch = { token_endpoint: "http://evil.example/token" };
  await rejectsWith(makeClient().discover(), "discovery");
  idp.state.discoveryPatch = { jwks_uri: undefined };
  await rejectsWith(makeClient().discover(), "discovery");
});

test("discovery and JWKS are cached, and concurrent callers share one fetch", async () => {
  const client = makeClient();
  const discoveries = idp.state.discoveryFetches;
  const jwks = idp.state.jwksFetches;
  await Promise.all([client.discover(), client.discover(), client.discover()]);
  await Promise.all([client.getJwks(), client.getJwks()]);
  await client.discover();
  await client.getJwks();
  assert.equal(idp.state.discoveryFetches - discoveries, 1);
  assert.equal(idp.state.jwksFetches - jwks, 1);
  await client.discover({ force: true });
  assert.equal(idp.state.discoveryFetches - discoveries, 2);
});

test("cached metadata expires after metadataTtlSeconds", async () => {
  let now = Math.floor(Date.now() / 1000);
  const client = makeClient({ now: () => now, metadataTtlSeconds: 10 });
  const before = idp.state.discoveryFetches;
  await client.discover();
  now += 11;
  await client.discover();
  assert.equal(idp.state.discoveryFetches - before, 2);
});

test("key rotation: an unknown kid refetches the JWKS once and then verifies", async () => {
  const client = makeClient();
  await client.getJwks();
  const fetchesBefore = idp.state.jwksFetches;
  idp.rotateKey(`k-${crypto.randomUUID()}`);
  const { auth, callbackUrl } = await startAndAuthorize(client);
  const result = await client.handleCallback({ url: callbackUrl, cookieValue: auth.cookie.value });
  assert.equal(result.user.sub, "4kT9xQ2mZ");
  assert.equal(idp.state.jwksFetches - fetchesBefore, 1);
});

test("a token signed by an unpublished key fails after one refetch", async () => {
  const client = makeClient();
  idp.rotateKey(`k-${crypto.randomUUID()}`, { publish: false });
  const { auth, callbackUrl } = await startAndAuthorize(client);
  await rejectsWith(client.handleCallback({ url: callbackUrl, cookieValue: auth.cookie.value }), "id_token_invalid");
  idp.rotateKey(`k-${crypto.randomUUID()}`);
});

test("refresh rotates the refresh token, validates the new ID token and enforces expectedSub", async () => {
  const client = makeClient();
  const { auth, callbackUrl } = await startAndAuthorize(client);
  const signedIn = await client.handleCallback({ url: callbackUrl, cookieValue: auth.cookie.value });
  const refreshed = await client.refresh(signedIn.tokens.refreshToken, { expectedSub: "4kT9xQ2mZ" });
  assert.notEqual(refreshed.tokens.refreshToken, signedIn.tokens.refreshToken);
  assert.equal(refreshed.claims.sub, "4kT9xQ2mZ");
  await assert.rejects(client.refresh(signedIn.tokens.refreshToken), (err) => err.code === "token_exchange" && err.oauthError === "invalid_grant");
  await rejectsWith(client.refresh(refreshed.tokens.refreshToken, { expectedSub: "someone-else" }), "id_token_invalid");
  await rejectsWith(client.refresh(""), "config");
});

test("revoke posts RFC 7009 parameters with client authentication", async () => {
  const client = makeClient();
  const { auth, callbackUrl } = await startAndAuthorize(client);
  const { tokens } = await client.handleCallback({ url: callbackUrl, cookieValue: auth.cookie.value });
  await client.revoke(tokens.refreshToken);
  assert.deepEqual(idp.state.revoked.at(-1), { token: tokens.refreshToken, hint: "refresh_token" });
  await client.revoke(tokens.accessToken, { tokenTypeHint: "access_token" });
  assert.equal(idp.state.revoked.at(-1).hint, "access_token");
  await assert.rejects(client.refresh(tokens.refreshToken), (err) => err.oauthError === "invalid_grant");
  await rejectsWith(client.revoke(undefined), "config");
});

test("fetchUserInfo returns standard claims for a valid access token", async () => {
  const client = makeClient();
  const { auth, callbackUrl } = await startAndAuthorize(client);
  const { tokens } = await client.handleCallback({ url: callbackUrl, cookieValue: auth.cookie.value });
  const info = await client.fetchUserInfo(tokens.accessToken);
  assert.equal(info.sub, "4kT9xQ2mZ");
  assert.equal(info.email_verified, true);
  await assert.rejects(client.fetchUserInfo("bogus"), (err) => err.code === "http" && err.status === 401);
});

test("buildLogoutUrl uses end_session_endpoint and validates the post-logout URI", async () => {
  const client = makeClient();
  const url = new URL(await client.buildLogoutUrl({ idTokenHint: "id.tok.en", postLogoutRedirectUri: "https://app.example/bye", state: "s1" }));
  assert.equal(`${url.origin}${url.pathname}`, `${idp.issuer}connect/logout`);
  assert.equal(url.searchParams.get("client_id"), CLIENT_ID);
  assert.equal(url.searchParams.get("id_token_hint"), "id.tok.en");
  assert.equal(url.searchParams.get("post_logout_redirect_uri"), "https://app.example/bye");
  assert.equal(url.searchParams.get("state"), "s1");
  await rejectsWith(client.buildLogoutUrl({ postLogoutRedirectUri: "http://app.example/bye" }), "config");
  idp.state.discoveryPatch = { end_session_endpoint: undefined };
  await rejectsWith(makeClient().buildLogoutUrl(), "discovery");
});

/**
 * @summary    A valid back-channel Logout Token for the fake provider, with optional overrides.
 * @param {object} [patch]
 * @param {object} [header]
 * @returns {string}
 */
function logoutToken(patch = {}, header = { typ: "logout+jwt" }) {
  const now = Math.floor(Date.now() / 1000);
  return idp.sign(
    {
      iss: idp.issuer,
      aud: CLIENT_ID,
      iat: now,
      exp: now + 120,
      jti: crypto.randomUUID(),
      sub: "4kT9xQ2mZ",
      sid: "grant-1",
      events: { [BACKCHANNEL_LOGOUT_EVENT]: {} },
      ...patch,
    },
    header,
  );
}

test("verifyLogoutToken accepts a valid Logout Token", async () => {
  const result = await makeClient().verifyLogoutToken(logoutToken());
  assert.equal(result.sub, "4kT9xQ2mZ");
  assert.equal(result.sid, "grant-1");
  assert.equal(typeof result.jti, "string");
  const sidOnly = await makeClient().verifyLogoutToken(logoutToken({ sub: undefined }));
  assert.equal(sidOnly.sub, null);
});

test("verifyLogoutToken rejects every rule the spec requires", async () => {
  const client = makeClient();
  const now = Math.floor(Date.now() / 1000);
  const bad = [
    logoutToken({}, { typ: "JWT" }),
    logoutToken({ iss: "https://evil.example/" }),
    logoutToken({ aud: "aa_other" }),
    logoutToken({ iat: now - 3600, exp: now + 120 }),
    logoutToken({ iat: now + 3600 }),
    logoutToken({ iat: undefined }),
    logoutToken({ exp: now - 3600 }),
    logoutToken({ events: undefined }),
    logoutToken({ events: { other: {} } }),
    logoutToken({ nonce: "n" }),
    logoutToken({ jti: undefined }),
    logoutToken({ sub: undefined, sid: undefined }),
    "not-a-jwt",
  ];
  for (const token of bad) {
    await rejectsWith(client.verifyLogoutToken(token), "logout_token_invalid");
  }
});
