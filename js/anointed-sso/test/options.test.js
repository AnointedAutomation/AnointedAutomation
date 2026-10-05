// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// 0.2.0 options: returnTo origin allowlist, per-request redirect URI, configurable return parameter, secret
// needed only at the token endpoint, normalized email, and start handlers that need no onSignedIn.

import { after, before, beforeEach, test } from "node:test";
import assert from "node:assert/strict";
import {
  AnointedSsoError,
  createAnointedClient,
  createExpressHandlers,
  createWebHandlers,
  safeReturnTo,
} from "../src/index.js";
import { CLIENT_ID, CLIENT_SECRET, REDIRECT_URI, startFakeIdp } from "./helpers/fakeIdp.js";

const SHOP_CALLBACK = "http://127.0.0.1:4001/auth/anointed/callback";
const MART_ORIGINS = ["https://www.mart.club", "https://*.mart.club", "http://localhost:3000"];

let idp;

before(async () => {
  idp = await startFakeIdp();
  idp.state.redirectUris.add(SHOP_CALLBACK);
});

after(async () => {
  await idp.close();
});

beforeEach(() => {
  idp.state.claims = { email: "jane@example.com", email_verified: true, name: "Jane D", preferred_username: "janed" };
  idp.state.tokenError = null;
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
 * @summary    A minimal node:http-style response that records headers.
 * @returns {{res: any, headers: Map<string, any>}}
 */
function fakeNodeResponse() {
  const headers = new Map();
  const res = { statusCode: 200, getHeader: (n) => headers.get(n), setHeader: (n, v) => headers.set(n, v), end() {} };
  return { res, headers };
}

// 1. returnTo allowlist

test("returnTo: the default stays same-site paths only", () => {
  const client = makeClient();
  assert.equal(client.validateReturnTo("/orders?x=1"), "/orders?x=1");
  assert.equal(client.validateReturnTo("https://www.mart.club/"), null);
  assert.equal(safeReturnTo("https://www.mart.club/"), null);
});

test("returnTo: allowlisted origins (exact and wildcard) are kept, normalized", () => {
  const client = makeClient({ allowedReturnOrigins: MART_ORIGINS });
  assert.equal(client.validateReturnTo("https://www.mart.club/cart"), "https://www.mart.club/cart");
  assert.equal(client.validateReturnTo("https://sneazonin.mart.club/products/x?y=1"), "https://sneazonin.mart.club/products/x?y=1");
  assert.equal(client.validateReturnTo("https://a.b.mart.club"), "https://a.b.mart.club/");
  assert.equal(client.validateReturnTo("HTTPS://Shop.Mart.Club/p"), "https://shop.mart.club/p");
  assert.equal(client.validateReturnTo("http://localhost:3000/dev"), "http://localhost:3000/dev");
  assert.equal(client.validateReturnTo("/still-fine"), "/still-fine");
  assert.equal(safeReturnTo("https://x.mart.club/a", { allowedReturnOrigins: MART_ORIGINS }), "https://x.mart.club/a");
});

test("returnTo: cross-site, look-alike and dangerous shapes are rejected even with an allowlist", () => {
  const client = makeClient({ allowedReturnOrigins: MART_ORIGINS });
  const rejected = [
    "https://evil.example/",
    "https://mart.club/", // the bare base is not covered by *.mart.club
    "https://evilmart.club/",
    "https://mart.club.evil.example/",
    "https://www.mart.club.evil.example/",
    "https://shop.mart.club:8443/", // the port must match the entry
    "http://www.mart.club/", // not https
    "http://shop.mart.club/",
    "http://localhost:4000/", // only the listed localhost port
    "//www.mart.club/",
    "///www.mart.club/",
    "/\\www.mart.club",
    "\\\\www.mart.club",
    "javascript:alert(1)",
    "JavaScript://www.mart.club/%0aalert(1)",
    "data:text/html,<script>alert(1)</script>",
    "vbscript:msgbox(1)",
    "ftp://www.mart.club/",
    "https://user:pass@www.mart.club/",
    "https://www.mart.club@evil.example/",
    "https:/www.mart.club/",
    "https:www.mart.club",
    " https://www.mart.club/",
    "https://www.mart.club/\nSet-Cookie:x",
    "https://www.mart.club/a\\b",
    `https://www.mart.club/${"a".repeat(2048)}`,
    7,
    null,
  ];
  for (const value of rejected) assert.equal(client.validateReturnTo(value), null, String(value));
});

test("returnTo: isAllowedReturnTo decides, but never past the https, credentials and scheme rules", () => {
  const seen = [];
  const client = makeClient({
    isAllowedReturnTo(url) {
      seen.push(url.href);
      return url.hostname === "partner.example" || url.hostname === "localhost";
    },
  });
  assert.equal(client.validateReturnTo("https://partner.example/x"), "https://partner.example/x");
  assert.equal(client.validateReturnTo("http://localhost:5173/"), "http://localhost:5173/");
  assert.equal(client.validateReturnTo("https://evil.example/"), null);
  for (const value of ["http://partner.example/", "//partner.example/", "javascript:alert(1)", "https://u:p@partner.example/"]) {
    assert.equal(client.validateReturnTo(value), null, value);
  }
  assert.deepEqual(seen, ["https://partner.example/x", "http://localhost:5173/", "https://evil.example/"]);
});

test("returnTo: an allowlisted absolute URL rides the flow and comes back from the callback", async () => {
  const client = makeClient({ allowedReturnOrigins: MART_ORIGINS });
  const auth = await client.createAuthorizeRequest({ returnTo: "https://shop.mart.club/cart" });
  const callbackUrl = await idp.authorize(auth.url);
  const result = await client.handleCallback({ url: callbackUrl, cookieValue: auth.cookie.value });
  assert.equal(result.returnTo, "https://shop.mart.club/cart");
});

test("returnTo: a cross-site value is dropped at start, and a forged cookie value is dropped at callback", async () => {
  const client = makeClient({ allowedReturnOrigins: MART_ORIGINS });
  const auth = await client.createAuthorizeRequest({ returnTo: "https://evil.example/" });
  assert.equal(client.decodeFlowCookie(auth.cookie.value).returnTo, null);

  // A flow cookie written by a client that trusted more origins is re-validated by this one.
  const loose = makeClient({ allowedReturnOrigins: ["https://evil.example"] });
  const forged = await loose.createAuthorizeRequest({ returnTo: "https://evil.example/" });
  assert.equal(loose.decodeFlowCookie(forged.cookie.value).returnTo, "https://evil.example/");
  assert.equal(client.decodeFlowCookie(forged.cookie.value).returnTo, null);
});

// 2. per-request redirect URI

test("redirectUri: a function of the request picks the URI per host, used at authorize and at exchange", async () => {
  const requests = [];
  const client = makeClient({
    redirectUri(request) {
      requests.push(request);
      return `${new URL(request.url).origin}/auth/anointed/callback`;
    },
  });
  assert.equal(client.redirectUri, null);
  const startRequest = new Request("http://127.0.0.1:4001/auth/anointed/start");
  const auth = await client.createAuthorizeRequest({ request: startRequest });
  assert.equal(auth.redirectUri, SHOP_CALLBACK);
  assert.equal(new URL(auth.url).searchParams.get("redirect_uri"), SHOP_CALLBACK);
  assert.equal(auth.cookie.options.secure, false);
  const callbackUrl = await idp.authorize(auth.url);
  const callbackRequest = new Request(callbackUrl);
  idp.state.tokenRequests.length = 0;
  const result = await client.handleCallback({ url: callbackUrl, cookieValue: auth.cookie.value, request: callbackRequest });
  assert.equal(result.user.sub, "4kT9xQ2mZ");
  assert.equal(idp.state.tokenRequests[0].redirect_uri, SHOP_CALLBACK);
  assert.deepEqual(requests, [startRequest, callbackRequest]);
  assert.equal(client.resolveRedirectUri(new Request("https://shop.example/x")), "https://shop.example/auth/anointed/callback");
});

test("redirectUri: an https per-request URI gets a Secure flow cookie", async () => {
  const client = makeClient({ redirectUri: () => "https://www.mart.club/api/auth/anointed/callback" });
  const auth = await client.createAuthorizeRequest();
  assert.equal(auth.cookie.options.secure, true);
  assert.match(auth.setCookieHeader, /; Secure;/);
});

test("redirectUri: a per-call value overrides the configured one", async () => {
  const client = makeClient();
  const auth = await client.createAuthorizeRequest({ redirectUri: SHOP_CALLBACK });
  assert.equal(new URL(auth.url).searchParams.get("redirect_uri"), SHOP_CALLBACK);
  const callbackUrl = await idp.authorize(auth.url);
  const result = await client.handleCallback({ url: callbackUrl, cookieValue: auth.cookie.value, redirectUri: SHOP_CALLBACK });
  assert.equal(result.user.sub, "4kT9xQ2mZ");
});

test("redirectUri: non-https, fragment, junk, throwing and non-allowlisted results are rejected before any network call", async () => {
  const allowed = ["https://www.mart.club/api/auth/anointed/callback", "https://shop.mart.club/api/auth/anointed/callback"];
  const hostOf = (request) => `https://${request.headers.get("host")}/api/auth/anointed/callback`;
  const client = makeClient({ redirectUri: hostOf, allowedRedirectUris: allowed });
  const req = (host) => new Request("https://placeholder.invalid/start", { headers: { host } });

  const ok = await client.createAuthorizeRequest({ request: req("shop.mart.club") });
  assert.equal(ok.redirectUri, allowed[1]);

  idp.state.tokenRequests.length = 0;
  // A spoofed Host header must not steer the redirect URI anywhere unlisted.
  await rejectsWith(client.createAuthorizeRequest({ request: req("evil.example") }), "redirect_uri_rejected");
  await rejectsWith(client.createAuthorizeRequest({ redirectUri: "https://evil.example/cb" }), "redirect_uri_rejected");
  await rejectsWith(
    client.handleCallback({ url: "/cb?code=x&state=y", cookieValue: ok.cookie.value, request: req("evil.example") }),
    "redirect_uri_rejected",
  );
  assert.equal(idp.state.tokenRequests.length, 0);

  const open = makeClient({ redirectUri: (r) => r });
  for (const bad of ["http://www.mart.club/cb", "https://www.mart.club/cb#x", "not a url", "", 7, undefined, "javascript:alert(1)"]) {
    await rejectsWith(open.createAuthorizeRequest({ request: bad }), "redirect_uri_rejected");
  }
  const throwing = makeClient({
    redirectUri: () => {
      throw new Error("no host");
    },
  });
  await rejectsWith(throwing.createAuthorizeRequest(), "redirect_uri_rejected");
});

test("redirectUri: a static URI must itself be on allowedRedirectUris", () => {
  assert.doesNotThrow(() => makeClient({ allowedRedirectUris: [REDIRECT_URI] }));
  assert.throws(() => makeClient({ allowedRedirectUris: ["https://other.example/cb"] }), (err) => err.code === "config");
});

// 3. return parameter name

test("returnToParam: the web start handler reads the configured parameter", async () => {
  const client = makeClient({ allowedReturnOrigins: MART_ORIGINS });
  const handlers = createWebHandlers(client, { returnToParam: "next" });
  const flowOf = (res) => client.decodeFlowCookie(decodeURIComponent(res.headers.getSetCookie()[0].split(";")[0].split("=")[1]));

  const res = await handlers.start(new Request("http://localhost:3000/start?next=https%3A%2F%2Fshop.mart.club%2Fcart&returnTo=/ignored"));
  assert.equal(res.status, 302);
  assert.equal(flowOf(res).returnTo, "https://shop.mart.club/cart");

  const evil = await handlers.start(new Request("http://localhost:3000/start?next=https%3A%2F%2Fevil.example%2F"));
  assert.equal(flowOf(evil).returnTo, null);
});

test("returnToParam: the Express start handler reads the configured parameter and passes req to redirectUri", async () => {
  const seen = [];
  const client = makeClient({
    redirectUri: (req) => {
      seen.push(req.headers.host);
      return SHOP_CALLBACK;
    },
  });
  const handlers = createExpressHandlers(client, { returnToParam: "next" });
  const { res, headers } = fakeNodeResponse();
  await handlers.start({ url: "/start?next=/orders", headers: { host: "127.0.0.1:4001" } }, res);
  assert.equal(res.statusCode, 302);
  assert.deepEqual(seen, ["127.0.0.1:4001"]);
  const value = decodeURIComponent(headers.get("set-cookie")[0].split(";")[0].split("=")[1]);
  assert.equal(client.decodeFlowCookie(value).returnTo, "/orders");
});

// 4. secret only at the token endpoint

test("clientSecret: start works without it; the exchange, refresh and revoke fail fast with a config error", async () => {
  const client = makeClient({ clientSecret: undefined });
  const auth = await client.createAuthorizeRequest({ returnTo: "/x" });
  assert.match(auth.url, /code_challenge_method=S256/);
  const callbackUrl = await idp.authorize(auth.url);
  idp.state.tokenRequests.length = 0;
  await assert.rejects(client.handleCallback({ url: callbackUrl, cookieValue: auth.cookie.value }), (err) => {
    assert.equal(err.code, "config");
    assert.match(err.message, /clientSecret is required/);
    return true;
  });
  await rejectsWith(client.refresh("r"), "config");
  await rejectsWith(client.revoke("r"), "config");
  assert.equal(idp.state.tokenRequests.length, 0);
});

test("clientSecret: the state check still runs first on a secret-less client", async () => {
  const client = makeClient({ clientSecret: undefined });
  const auth = await client.createAuthorizeRequest();
  await rejectsWith(client.handleCallback({ url: "/cb?code=x&state=forged", cookieValue: auth.cookie.value }), "state_mismatch");
  await rejectsWith(client.handleCallback({ url: "/cb?code=x&state=forged", cookieValue: undefined }), "flow_missing");
});

// 5. normalized email

test("email: user.email is trimmed and lowercase, the raw claim stays on claims.email", async () => {
  idp.state.claims = { email: "  Jane.Doe@Example.COM ", email_verified: true };
  const client = makeClient();
  const auth = await client.createAuthorizeRequest();
  const callbackUrl = await idp.authorize(auth.url);
  const result = await client.handleCallback({ url: callbackUrl, cookieValue: auth.cookie.value });
  assert.equal(result.user.email, "jane.doe@example.com");
  assert.equal(result.user.emailVerified, true);
  assert.equal(result.claims.email, "  Jane.Doe@Example.COM ");
});

test("email: an unverified mixed-case email is still never returned", async () => {
  idp.state.claims = { email: "Jane@Example.com", email_verified: false };
  const client = makeClient({ requireVerifiedEmail: false });
  const auth = await client.createAuthorizeRequest();
  const callbackUrl = await idp.authorize(auth.url);
  const result = await client.handleCallback({ url: callbackUrl, cookieValue: auth.cookie.value });
  assert.equal(result.user.email, null);
  assert.equal(result.user.emailVerified, false);
});

// 6. start without onSignedIn

test("onSignedIn: start handlers need none; callback without one is a config error", async () => {
  const client = makeClient();
  const web = createWebHandlers(client);
  const res = await web.start(new Request("http://localhost:3000/start"));
  assert.equal(res.status, 302);
  await rejectsWith(web.callback(new Request("http://localhost:3000/cb?code=x&state=y")), "config");

  const express = createExpressHandlers(client, { onError: () => {} });
  const node = fakeNodeResponse();
  await express.start({ url: "/start", headers: {} }, node.res);
  assert.equal(node.res.statusCode, 302);
  await rejectsWith(express.callback({ url: "/cb", headers: {} }, node.res), "config");
});
