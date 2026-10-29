// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// The four handlers against the fake provider from the core kit's tests: fake h3 v2 style events ({req}) for the
// flow, and a real h3 v1 app on a local HTTP server to prove the toWebRequest path and Response passthrough.

import { after, before, test } from "node:test";
import assert from "node:assert/strict";
import crypto from "node:crypto";
import http from "node:http";
import { createApp, eventHandler, toNodeListener } from "h3";
import { BACKCHANNEL_LOGOUT_EVENT, createAnointedClient } from "@anointedautomation/sso";
import { createAnointedEventHandlers, redirectTo } from "../src/index.js";
import { CLIENT_ID, CLIENT_SECRET, REDIRECT_URI, startFakeIdp } from "../../anointed-sso/test/helpers/fakeIdp.js";

let idp;
let client;

before(async () => {
  idp = await startFakeIdp();
  client = createAnointedClient({ issuer: idp.issuer, clientId: CLIENT_ID, clientSecret: CLIENT_SECRET, redirectUri: REDIRECT_URI });
});

after(async () => {
  await idp.close();
});

/**
 * @summary    A fake h3 v2 style event wrapping a web Request.
 * @param {string} url
 * @param {RequestInit} [init]
 * @returns {{req: Request}}
 */
function eventFor(url, init) {
  return { req: new Request(url, init) };
}

/**
 * @summary    A valid Logout Token signed by the fake provider.
 * @returns {string}
 */
function logoutToken() {
  const now = Math.floor(Date.now() / 1000);
  return idp.sign(
    { iss: idp.issuer, aud: CLIENT_ID, iat: now, exp: now + 120, jti: crypto.randomUUID(), sub: "4kT9xQ2mZ", sid: "grant-1", events: { [BACKCHANNEL_LOGOUT_EVENT]: {} } },
    { typ: "logout+jwt" },
  );
}

test("start -> provider -> callback signs in, passes client and event to onSignedIn and clears the flow cookie", async () => {
  let seen = null;
  const handlers = createAnointedEventHandlers({
    client,
    onSignedIn: (result, request, context) => {
      seen = context;
      return redirectTo(request, result.returnTo === null ? "/" : result.returnTo, [`app_session=${result.user.sub}; Path=/; HttpOnly`]);
    },
  });

  const startRes = await handlers.start(eventFor("http://localhost:3000/api/auth/anointed/start?returnTo=/orders"));
  assert.equal(startRes.status, 302);
  const flowCookie = startRes.headers.getSetCookie()[0].split(";")[0];

  const callbackUrl = await idp.authorize(startRes.headers.get("location"));
  const callbackEvent = eventFor(callbackUrl, { headers: { cookie: flowCookie } });
  const res = await handlers.callback(callbackEvent);
  assert.equal(res.status, 302);
  assert.equal(res.headers.get("location"), "http://localhost:3000/orders");
  const cookies = res.headers.getSetCookie();
  assert.ok(cookies.some((c) => c.startsWith("app_session=4kT9xQ2mZ")));
  assert.ok(cookies.some((c) => c.startsWith("aa_sso_flow=") && /Max-Age=0|Expires=/i.test(c)));
  assert.equal(seen.client, client);
  assert.equal(seen.event, callbackEvent);
});

test("returnToParam renames the start query parameter", async () => {
  const seen = [];
  const handlers = createAnointedEventHandlers({
    client,
    onSignedIn: (r, request) => {
      seen.push(r.returnTo);
      return redirectTo(request, "/");
    },
    returnToParam: "next",
  });
  const startRes = await handlers.start(eventFor("http://localhost:3000/start?next=/billing"));
  const flowCookie = startRes.headers.getSetCookie()[0].split(";")[0];
  const callbackUrl = await idp.authorize(startRes.headers.get("location"));
  await handlers.callback(eventFor(callbackUrl, { headers: { cookie: flowCookie } }));
  assert.deepEqual(seen, ["/billing"]);
});

test("callback without a flow cookie goes to onError, with the default 400 when none is given", async () => {
  const errors = [];
  const withHandler = createAnointedEventHandlers({
    client,
    onSignedIn: (_r, request) => redirectTo(request, "/"),
    onError: (err) => {
      errors.push(err.code);
      return new Response("nope", { status: 401 });
    },
  });
  const url = "http://localhost:3000/api/auth/anointed/callback?code=x&state=y";
  assert.equal((await withHandler.callback(eventFor(url))).status, 401);
  assert.equal(errors.length, 1);

  const plain = createAnointedEventHandlers({ client, onSignedIn: (_r, request) => redirectTo(request, "/") });
  assert.equal((await plain.callback(eventFor(url))).status, 400);
});

test("backchannelLogout verifies the token and calls onLogout; bad token is 400", async () => {
  const notices = [];
  const handlers = createAnointedEventHandlers({
    client,
    onSignedIn: (_r, request) => redirectTo(request, "/"),
    onLogout: (logout) => {
      notices.push(logout.sub);
    },
  });
  const post = (body) =>
    handlers.backchannelLogout(eventFor("http://localhost:3000/api/auth/anointed/backchannel-logout", { method: "POST", headers: { "content-type": "application/x-www-form-urlencoded" }, body }));
  assert.equal((await post(`logout_token=${logoutToken()}`)).status, 200);
  assert.deepEqual(notices, ["4kT9xQ2mZ"]);
  assert.equal((await post("logout_token=garbage")).status, 400);
  assert.equal((await post("")).status, 400);
});

test("backchannelLogout and logout throw a config error when their callbacks are missing", async () => {
  const handlers = createAnointedEventHandlers({ client, onSignedIn: (_r, request) => redirectTo(request, "/") });
  await assert.rejects(() => handlers.backchannelLogout(eventFor("http://localhost:3000/x", { method: "POST" })), { code: "config" });
  await assert.rejects(() => handlers.logout(eventFor("http://localhost:3000/x")), { code: "config" });
});

test("logout clears the app session and redirects to the provider end-session endpoint", async () => {
  const handlers = createAnointedEventHandlers({
    client,
    onSignedIn: (_r, request) => redirectTo(request, "/"),
    onSignOut: () => ({ setCookies: ["app_session=; Path=/; Max-Age=0"], idTokenHint: "id.tok.en" }),
    postLogoutRedirectUri: "https://app.example/bye",
  });
  const res = await handlers.logout(eventFor("http://localhost:3000/api/auth/anointed/logout"));
  assert.equal(res.status, 302);
  const location = new URL(res.headers.get("location"));
  assert.equal(location.origin + location.pathname, `${idp.issuer}connect/logout`);
  assert.equal(location.searchParams.get("client_id"), CLIENT_ID);
  assert.equal(location.searchParams.get("id_token_hint"), "id.tok.en");
  assert.equal(location.searchParams.get("post_logout_redirect_uri"), "https://app.example/bye");
  assert.deepEqual(res.headers.getSetCookie(), ["app_session=; Path=/; Max-Age=0"]);
});

test("logout failures go to onError or a plain 400", async () => {
  const failing = { client, onSignedIn: (_r, request) => redirectTo(request, "/"), onSignOut: () => null };
  const plain = createAnointedEventHandlers(failing);
  assert.equal((await plain.logout(eventFor("http://localhost:3000/x"))).status, 400);
  const codes = [];
  const custom = createAnointedEventHandlers({ ...failing, onError: (e) => { codes.push(e.code); return new Response("x", { status: 500 }); } });
  assert.equal((await custom.logout(eventFor("http://localhost:3000/x"))).status, 500);
  assert.deepEqual(codes, ["config"]);
});

test("logout failures are logged before they are answered", async () => {
  const calls = [];
  const handlers = createAnointedEventHandlers({
    client,
    onSignedIn: (_r, request) => redirectTo(request, "/"),
    onSignOut: () => null,
    logger: (level, message, details) => calls.push({ level, message, details }),
  });
  assert.equal((await handlers.logout(eventFor("http://localhost:3000/x"))).status, 400);
  assert.equal(calls.length, 1);
  assert.equal(calls[0].level, "error");
  assert.equal(calls[0].message, "anointed-sso-nuxt sign-out failed");
  assert.equal(calls[0].details.code, "config");
  assert.throws(() => createAnointedEventHandlers({ client, onSignedIn: () => null, logger: "console" }), (err) => err.code === "config");
});

test("a real h3 v1 app: the start route works through toWebRequest and a returned Response", async () => {
  const handlers = createAnointedEventHandlers({ client, onSignedIn: (_r, request) => redirectTo(request, "/") });
  const app = createApp();
  app.use("/api/auth/anointed/start", eventHandler(handlers.start));
  const server = http.createServer(toNodeListener(app));
  await new Promise((resolve) => server.listen(0, "127.0.0.1", resolve));
  try {
    const { port } = server.address();
    const res = await fetch(`http://127.0.0.1:${port}/api/auth/anointed/start?returnTo=/orders`, { redirect: "manual" });
    assert.equal(res.status, 302);
    assert.ok(res.headers.get("location").startsWith(idp.issuer));
    assert.ok(res.headers.getSetCookie().some((c) => c.startsWith("aa_sso_flow=")));
  } finally {
    await new Promise((resolve) => server.close(resolve));
  }
});

test("options are validated up front and events must be objects", async () => {
  assert.throws(() => createAnointedEventHandlers(undefined), { code: "config" });
  assert.throws(() => createAnointedEventHandlers({ onSignedIn: () => {} }), { code: "config" });
  assert.throws(() => createAnointedEventHandlers({ client: {}, onSignedIn: () => {} }), { code: "config" });
  assert.throws(() => createAnointedEventHandlers({ client }), { code: "config" });
  assert.throws(() => createAnointedEventHandlers({ client, onSignedIn: () => {}, onSignOut: "no" }), { code: "config" });
  assert.throws(() => createAnointedEventHandlers({ client, onSignedIn: () => {}, postLogoutRedirectUri: "" }), { code: "config" });
  assert.throws(() => createAnointedEventHandlers({ client, onSignedIn: () => {}, returnToParam: "" }), { code: "config" });
  const handlers = createAnointedEventHandlers({ client, onSignedIn: () => {} });
  await assert.rejects(() => handlers.start(undefined), { code: "config" });
});

test("redirectTo resolves against the request, appends cookies and rejects an empty target", () => {
  const res = redirectTo(new Request("https://app.example/api/auth/anointed/callback"), "/home", ["a=1", "b=2"]);
  assert.equal(res.status, 302);
  assert.equal(res.headers.get("location"), "https://app.example/home");
  assert.deepEqual(res.headers.getSetCookie(), ["a=1", "b=2"]);
  assert.throws(() => redirectTo(new Request("https://app.example/"), ""), { code: "config" });
});
