// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Handlers and the handle hook against the fake provider from the core kit's tests, driven with standard Request objects.

import { after, before, test } from "node:test";
import assert from "node:assert/strict";
import crypto from "node:crypto";
import { BACKCHANNEL_LOGOUT_EVENT, createAnointedClient } from "@anointedautomation/sso";
import { createAnointedHandle, createAnointedHandlers, redirectTo } from "../src/index.js";
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

const signedIn = (result, request) => redirectTo(request, result.returnTo === null ? "/" : result.returnTo, [`app_session=${result.user.sub}; Path=/; HttpOnly`]);

test("start -> provider -> callback signs in, passes the client to onSignedIn and clears the flow cookie", async () => {
  let seenClient = null;
  const handlers = createAnointedHandlers({ client, onSignedIn: (r, req, ctx) => { seenClient = ctx.client; return signedIn(r, req); } });
  const startRes = await handlers.start(new Request("http://localhost:5173/auth/anointed/start?returnTo=/orders"));
  assert.equal(startRes.status, 302);
  const flowCookie = startRes.headers.getSetCookie()[0].split(";")[0];
  const callbackUrl = await idp.authorize(startRes.headers.get("location"));
  const res = await handlers.callback(new Request(callbackUrl, { headers: { cookie: flowCookie } }));
  assert.equal(res.status, 302);
  assert.equal(res.headers.get("location"), "http://localhost:3000/orders");
  const cookies = res.headers.getSetCookie();
  assert.ok(cookies.some((c) => c.startsWith("app_session=4kT9xQ2mZ")));
  assert.ok(cookies.some((c) => c.startsWith("aa_sso_flow=") && /Max-Age=0|Expires=/i.test(c)));
  assert.equal(seenClient, client);
});

test("callback without a flow cookie goes to onError, default 400 otherwise", async () => {
  const url = "http://localhost:5173/auth/anointed/callback?code=x&state=y";
  const codes = [];
  const custom = createAnointedHandlers({ client, onSignedIn: signedIn, onError: (e) => { codes.push(e.code); return new Response("no", { status: 401 }); } });
  assert.equal((await custom.callback(new Request(url))).status, 401);
  assert.equal(codes.length, 1);
  assert.equal((await createAnointedHandlers({ client, onSignedIn: signedIn }).callback(new Request(url))).status, 400);
});

test("backchannelLogout verifies the token and calls onLogout; bad token is 400", async () => {
  const notices = [];
  const handlers = createAnointedHandlers({ client, onSignedIn: signedIn, onLogout: (l) => { notices.push(l.sub); } });
  const post = (body) => handlers.backchannelLogout(new Request("http://localhost:5173/x", { method: "POST", headers: { "content-type": "application/x-www-form-urlencoded" }, body }));
  assert.equal((await post(`logout_token=${logoutToken()}`)).status, 200);
  assert.deepEqual(notices, ["4kT9xQ2mZ"]);
  assert.equal((await post("logout_token=garbage")).status, 400);
  assert.equal((await post("")).status, 400);
});

test("logout and backchannelLogout throw a config error when their callback is missing", async () => {
  const handlers = createAnointedHandlers({ client, onSignedIn: signedIn });
  await assert.rejects(() => handlers.logout(new Request("http://localhost:5173/x", { method: "POST" })), { code: "config" });
  await assert.rejects(() => handlers.backchannelLogout(new Request("http://localhost:5173/x", { method: "POST" })), { code: "config" });
});

test("logout runs onSignOut with the client", async () => {
  const handlers = createAnointedHandlers({ client, onSignedIn: signedIn, onSignOut: (req, ctx) => { assert.equal(ctx.client, client); return redirectTo(req, "/", ["app_session=; Path=/; Max-Age=0"]); } });
  const res = await handlers.logout(new Request("http://localhost:5173/auth/anointed/logout", { method: "POST" }));
  assert.equal(res.status, 302);
  assert.deepEqual(res.headers.getSetCookie(), ["app_session=; Path=/; Max-Age=0"]);
});

test("options are validated up front", () => {
  assert.throws(() => createAnointedHandlers(undefined), { code: "config" });
  assert.throws(() => createAnointedHandlers({ onSignedIn: () => {} }), { code: "config" });
  assert.throws(() => createAnointedHandlers({ client: {}, onSignedIn: () => {} }), { code: "config" });
  assert.throws(() => createAnointedHandlers({ client }), { code: "config" });
  assert.throws(() => createAnointedHandlers({ client, onSignedIn: () => {}, onSignOut: 5 }), { code: "config" });
  assert.throws(() => createAnointedHandlers({ client, onSignedIn: () => {}, returnToParam: "" }), { code: "config" });
});

test("handle hook routes the four paths and passes everything else to resolve", async () => {
  const calls = [];
  const handlers = createAnointedHandlers({ client, onSignedIn: signedIn, onLogout: () => {}, onSignOut: (req) => redirectTo(req, "/") });
  const handle = createAnointedHandle({ handlers, basePath: "/auth/anointed" });
  const resolve = () => { calls.push("resolve"); return new Response("page"); };
  const run = (method, path) => { const request = new Request(`http://localhost:5173${path}`, { method }); return handle({ event: { request, url: new URL(request.url) }, resolve }); };

  assert.equal((await run("GET", "/auth/anointed/start")).status, 302);
  assert.equal((await run("POST", "/auth/anointed/logout")).status, 302);
  assert.equal((await run("POST", "/auth/anointed/backchannel-logout")).status, 400);
  assert.equal((await run("GET", "/auth/anointed/callback")).status, 400);
  assert.equal(calls.length, 0);
  assert.equal(await (await run("GET", "/dashboard")).text(), "page");
  assert.equal(await (await run("GET", "/auth/anointed/logout")).text(), "page");
  assert.equal(await (await run("POST", "/auth/anointed/start")).text(), "page");
  assert.equal(calls.length, 3);
});

test("createAnointedHandle validates its options", () => {
  const handlers = createAnointedHandlers({ client, onSignedIn: signedIn });
  assert.throws(() => createAnointedHandle(undefined), { code: "config" });
  assert.throws(() => createAnointedHandle({ basePath: "/a" }), { code: "config" });
  assert.throws(() => createAnointedHandle({ handlers, basePath: "a" }), { code: "config" });
  assert.throws(() => createAnointedHandle({ handlers, basePath: "/a/" }), { code: "config" });
  assert.throws(() => createAnointedHandle({ handlers }), { code: "config" });
});

test("redirectTo resolves against the request, appends cookies and rejects an empty target", () => {
  const res = redirectTo(new Request("https://app.example/auth/anointed/callback"), "/home", ["a=1", "b=2"]);
  assert.equal(res.headers.get("location"), "https://app.example/home");
  assert.deepEqual(res.headers.getSetCookie(), ["a=1", "b=2"]);
  assert.throws(() => redirectTo(new Request("https://app.example/"), ""), { code: "config" });
});
