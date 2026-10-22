// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// The three route handlers against the fake provider from the core kit's tests, driven with standard Request objects.

import { after, before, test } from "node:test";
import assert from "node:assert/strict";
import crypto from "node:crypto";
import { BACKCHANNEL_LOGOUT_EVENT, createAnointedClient } from "@anointedautomation/sso";
import { createAnointedRouteHandlers, redirectTo } from "../src/index.js";
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

test("start -> provider -> callback signs in, passes the client to onSignedIn and clears the flow cookie", async () => {
  let seenContextClient = null;
  const handlers = createAnointedRouteHandlers({
    client,
    onSignedIn: (result, request, context) => {
      seenContextClient = context.client;
      return redirectTo(request, result.returnTo === null ? "/" : result.returnTo, [`app_session=${result.user.sub}; Path=/; HttpOnly`]);
    },
  });

  const startRes = await handlers.start(new Request("http://localhost:3000/api/auth/anointed/start?returnTo=/orders"));
  assert.equal(startRes.status, 302);
  const flowCookie = startRes.headers.getSetCookie()[0].split(";")[0];

  const callbackUrl = await idp.authorize(startRes.headers.get("location"));
  const res = await handlers.callback(new Request(callbackUrl, { headers: { cookie: flowCookie } }));
  assert.equal(res.status, 302);
  assert.equal(res.headers.get("location"), "http://localhost:3000/orders");
  const cookies = res.headers.getSetCookie();
  assert.ok(cookies.some((c) => c.startsWith("app_session=4kT9xQ2mZ")));
  assert.ok(cookies.some((c) => c.startsWith("aa_sso_flow=") && /Max-Age=0|Expires=/i.test(c)));
  assert.equal(seenContextClient, client);
});

test("returnToParam renames the start query parameter", async () => {
  const handlers = createAnointedRouteHandlers({ client, onSignedIn: (_r, request) => redirectTo(request, "/"), returnToParam: "next" });
  const startRes = await handlers.start(new Request("http://localhost:3000/start?next=/billing"));
  const callbackUrl = await idp.authorize(startRes.headers.get("location"));
  const flowCookie = startRes.headers.getSetCookie()[0].split(";")[0];
  const seen = [];
  const again = createAnointedRouteHandlers({ client, onSignedIn: (r, request) => { seen.push(r.returnTo); return redirectTo(request, "/"); }, returnToParam: "next" });
  await again.callback(new Request(callbackUrl, { headers: { cookie: flowCookie } }));
  assert.deepEqual(seen, ["/billing"]);
});

test("callback without a flow cookie goes to onError, with the default 400 when none is given", async () => {
  const errors = [];
  const withHandler = createAnointedRouteHandlers({
    client,
    onSignedIn: (_r, request) => redirectTo(request, "/"),
    onError: (err) => {
      errors.push(err.code);
      return new Response("nope", { status: 401 });
    },
  });
  const url = "http://localhost:3000/api/auth/anointed/callback?code=x&state=y";
  const res = await withHandler.callback(new Request(url));
  assert.equal(res.status, 401);
  assert.equal(errors.length, 1);

  const plain = createAnointedRouteHandlers({ client, onSignedIn: (_r, request) => redirectTo(request, "/") });
  assert.equal((await plain.callback(new Request(url))).status, 400);
});

test("backchannelLogout verifies the token and calls onLogout; bad token is 400", async () => {
  const notices = [];
  const handlers = createAnointedRouteHandlers({ client, onSignedIn: (_r, request) => redirectTo(request, "/"), onLogout: (logout) => { notices.push(logout.sub); } });
  const post = (body) =>
    handlers.backchannelLogout(new Request("http://localhost:3000/api/auth/anointed/backchannel-logout", { method: "POST", headers: { "content-type": "application/x-www-form-urlencoded" }, body }));
  assert.equal((await post(`logout_token=${logoutToken()}`)).status, 200);
  assert.deepEqual(notices, ["4kT9xQ2mZ"]);
  assert.equal((await post("logout_token=garbage")).status, 400);
  assert.equal((await post("")).status, 400);
});

test("backchannelLogout throws a config error when onLogout is missing", async () => {
  const handlers = createAnointedRouteHandlers({ client, onSignedIn: (_r, request) => redirectTo(request, "/") });
  await assert.rejects(() => handlers.backchannelLogout(new Request("http://localhost:3000/x", { method: "POST" })), { code: "config" });
});

test("options are validated up front", () => {
  assert.throws(() => createAnointedRouteHandlers(undefined), { code: "config" });
  assert.throws(() => createAnointedRouteHandlers({ onSignedIn: () => {} }), { code: "config" });
  assert.throws(() => createAnointedRouteHandlers({ client: {}, onSignedIn: () => {} }), { code: "config" });
  assert.throws(() => createAnointedRouteHandlers({ client }), { code: "config" });
  assert.throws(() => createAnointedRouteHandlers({ client, onSignedIn: () => {}, returnToParam: "" }), { code: "config" });
});

test("redirectTo resolves against the request, appends cookies and rejects an empty target", () => {
  const res = redirectTo(new Request("https://app.example/api/auth/anointed/callback"), "/home", ["a=1", "b=2"]);
  assert.equal(res.status, 302);
  assert.equal(res.headers.get("location"), "https://app.example/home");
  assert.deepEqual(res.headers.getSetCookie(), ["a=1", "b=2"]);
  assert.throws(() => redirectTo(new Request("https://app.example/"), ""), { code: "config" });
});
