// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// End-to-end through the adapters: a simulated browser carries cookies between our handlers and the fake
// provider, exactly as a real one would.

import { after, before, test } from "node:test";
import assert from "node:assert/strict";
import http from "node:http";
import crypto from "node:crypto";
import {
  AnointedSsoError,
  BACKCHANNEL_LOGOUT_EVENT,
  createAnointedClient,
  createExpressHandlers,
  createWebHandlers,
} from "../src/index.js";
import { CLIENT_ID, CLIENT_SECRET, REDIRECT_URI, startFakeIdp } from "./helpers/fakeIdp.js";

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
 * @summary    Every Set-Cookie header on a Response.
 * @param {Headers} headers
 * @returns {string[]}
 */
function setCookies(headers) {
  return headers.getSetCookie();
}

/**
 * @summary    A valid Logout Token signed by the fake provider.
 * @param {object} [patch]
 * @returns {string}
 */
function logoutToken(patch = {}) {
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
    { typ: "logout+jwt" },
  );
}

test("web adapter: start -> provider -> callback signs the user in and clears the flow cookie", async () => {
  const handlers = createWebHandlers(client, {
    onSignedIn: (result) =>
      new Response(null, {
        status: 302,
        headers: { location: result.returnTo === null ? "/" : result.returnTo, "set-cookie": `app_session=${result.user.sub}; Path=/; HttpOnly` },
      }),
  });

  const startRes = await handlers.start(new Request("http://localhost:3000/auth/anointed/start?returnTo=/orders"));
  assert.equal(startRes.status, 302);
  assert.equal(startRes.headers.get("cache-control"), "no-store");
  const flowCookie = setCookies(startRes.headers)[0].split(";")[0];
  assert.match(flowCookie, /^aa_sso_flow=/);

  const callbackUrl = await idp.authorize(startRes.headers.get("location"));
  const callbackRes = await handlers.callback(new Request(callbackUrl, { headers: { cookie: `other=1; ${flowCookie}` } }));
  assert.equal(callbackRes.status, 302);
  assert.equal(callbackRes.headers.get("location"), "/orders");
  const cookies = setCookies(callbackRes.headers);
  assert.ok(cookies.includes("app_session=4kT9xQ2mZ; Path=/; HttpOnly"));
  assert.ok(cookies.some((c) => /^aa_sso_flow=; Path=\/; Max-Age=0/.test(c)));
});

test("web adapter: a failed callback goes to onError with a coded error and still clears the flow cookie", async () => {
  const seen = [];
  const handlers = createWebHandlers(client, {
    onSignedIn: () => new Response("unreachable"),
    onError: (err) => {
      seen.push(err);
      return Response.redirect("http://localhost:3000/login?error=anointed", 302);
    },
  });
  const res = await handlers.callback(new Request(`${REDIRECT_URI}?code=x&state=y`));
  assert.equal(res.status, 302);
  assert.ok(seen[0] instanceof AnointedSsoError);
  assert.equal(seen[0].code, "flow_missing");
  assert.ok(setCookies(res.headers).some((c) => c.startsWith("aa_sso_flow=;")));
});

test("web adapter: default onError is a generic 400 that leaks no detail", async () => {
  const handlers = createWebHandlers(client, { onSignedIn: () => new Response("x") });
  const res = await handlers.callback(new Request(`${REDIRECT_URI}?code=x&state=y`));
  assert.equal(res.status, 400);
  const text = await res.text();
  assert.equal(text.includes("flow"), false);
});

test("web adapter: an onSignedIn failure (your database is down) goes to onError", async () => {
  const handlers = createWebHandlers(client, {
    onSignedIn: () => {
      throw new Error("db down");
    },
    onError: (err) => new Response(err.message, { status: 503 }),
  });
  const startRes = await handlers.start(new Request("http://localhost:3000/auth/anointed/start"));
  const flowCookie = setCookies(startRes.headers)[0].split(";")[0];
  const callbackUrl = await idp.authorize(startRes.headers.get("location"));
  const res = await handlers.callback(new Request(callbackUrl, { headers: { cookie: flowCookie } }));
  assert.equal(res.status, 503);
});

test("web adapter: start failure (provider unreachable) goes to onError", async () => {
  const broken = createAnointedClient({
    issuer: idp.issuer,
    clientId: CLIENT_ID,
    clientSecret: CLIENT_SECRET,
    redirectUri: REDIRECT_URI,
    fetch: async () => {
      throw new TypeError("fetch failed");
    },
  });
  const handlers = createWebHandlers(broken, { onSignedIn: () => new Response("x"), onError: (err) => new Response(err.code, { status: 502 }) });
  const res = await handlers.start(new Request("http://localhost:3000/auth/anointed/start"));
  assert.equal(res.status, 502);
  assert.equal(await res.text(), "discovery");
});

test("web adapter: options are validated and backchannelLogout exists only with onLogout", () => {
  assert.doesNotThrow(() => createWebHandlers(client, {}));
  assert.doesNotThrow(() => createWebHandlers(client));
  assert.throws(() => createWebHandlers(client, null), (err) => err.code === "config");
  assert.throws(() => createWebHandlers(client, { onSignedIn: "x" }), (err) => err.code === "config");
  assert.throws(() => createWebHandlers(client, { returnToParam: "" }), (err) => err.code === "config");
  assert.throws(() => createWebHandlers(client, { returnToParam: 5 }), (err) => err.code === "config");
  assert.throws(() => createWebHandlers(client, { onSignedIn: () => null, onError: 1 }), (err) => err.code === "config");
  assert.throws(() => createWebHandlers(client, { onSignedIn: () => null, onLogout: "x" }), (err) => err.code === "config");
  assert.equal(createWebHandlers(client, { onSignedIn: () => null }).backchannelLogout, undefined);
});

test("web adapter: back-channel logout answers 200 / 400 / 500 per the spec", async () => {
  const ended = [];
  let failNext = false;
  const handlers = createWebHandlers(client, {
    onSignedIn: () => new Response("x"),
    onLogout: (logout) => {
      if (failNext) throw new Error("store down");
      ended.push(logout);
    },
  });
  /**
   * @summary    POST a logout_token form body.
   * @param {string} body
   * @returns {Promise<Response>}
   */
  const post = (body) =>
    handlers.backchannelLogout(
      new Request("http://localhost:3000/auth/anointed/backchannel-logout", {
        method: "POST",
        headers: { "content-type": "application/x-www-form-urlencoded" },
        body,
      }),
    );
  assert.equal((await post(`logout_token=${logoutToken()}`)).status, 200);
  assert.equal(ended[0].sub, "4kT9xQ2mZ");
  assert.equal((await post(`logout_token=${logoutToken({ nonce: "n" })}`)).status, 400);
  assert.equal((await post("")).status, 400);
  assert.equal((await post("logout_token=")).status, 400);
  failNext = true;
  assert.equal((await post(`logout_token=${logoutToken()}`)).status, 500);
});

/**
 * @summary    Run an Express-style app (plain node:http with an `originalUrl`) on an ephemeral port.
 * @param {(req: any, res: any) => Promise<void> | void} handler
 * @returns {Promise<{base: string, close: () => Promise<void>}>}
 */
async function serve(handler) {
  const server = http.createServer((req, res) => {
    req.originalUrl = req.url;
    Promise.resolve(handler(req, res)).catch((err) => {
      res.statusCode = 599;
      res.end(String(err));
    });
  });
  await new Promise((resolve) => server.listen(0, "127.0.0.1", resolve));
  return {
    base: `http://127.0.0.1:${server.address().port}`,
    close: () => new Promise((resolve) => server.close(() => resolve())),
  };
}

test("express adapter: full flow over real HTTP, with onSignedIn setting the app session", async () => {
  const handlers = createExpressHandlers(client, {
    onSignedIn: (result, req, res) => {
      res.setHeader("set-cookie", res.getHeader("set-cookie").concat(`app_session=${result.user.sub}; Path=/; HttpOnly`));
      res.statusCode = 302;
      res.setHeader("location", result.returnTo === null ? "/" : result.returnTo);
      res.end();
    },
  });
  const app = await serve((req, res) => {
    if (req.url.startsWith("/auth/anointed/start")) return handlers.start(req, res);
    if (req.url.startsWith("/auth/anointed/callback")) return handlers.callback(req, res);
    res.statusCode = 404;
    res.end();
  });
  try {
    const startRes = await fetch(`${app.base}/auth/anointed/start?returnTo=/orders`, { redirect: "manual" });
    assert.equal(startRes.status, 302);
    const flowCookie = startRes.headers.getSetCookie()[0].split(";")[0];

    const providerCallback = new URL(await idp.authorize(startRes.headers.get("location")));
    const callbackRes = await fetch(`${app.base}${providerCallback.pathname}${providerCallback.search}`, {
      redirect: "manual",
      headers: { cookie: flowCookie },
    });
    assert.equal(callbackRes.status, 302);
    assert.equal(callbackRes.headers.get("location"), "/orders");
    const cookies = callbackRes.headers.getSetCookie();
    assert.ok(cookies.some((c) => c.startsWith("aa_sso_flow=;")));
    assert.ok(cookies.includes("app_session=4kT9xQ2mZ; Path=/; HttpOnly"));
  } finally {
    await app.close();
  }
});

test("express adapter: errors go to onError, default is a generic 400", async () => {
  const seen = [];
  const withHandler = createExpressHandlers(client, {
    onSignedIn: () => {},
    onError: (err, req, res) => {
      seen.push(err.code);
      res.statusCode = 302;
      res.setHeader("location", "/login?error=anointed");
      res.end();
    },
  });
  const bare = createExpressHandlers(client, { onSignedIn: () => {} });
  const app = await serve((req, res) => (req.url.startsWith("/a") ? withHandler.callback(req, res) : bare.callback(req, res)));
  try {
    const a = await fetch(`${app.base}/a?code=x&state=y`, { redirect: "manual" });
    assert.equal(a.status, 302);
    assert.deepEqual(seen, ["flow_missing"]);
    const b = await fetch(`${app.base}/b?code=x&state=y`, { redirect: "manual" });
    assert.equal(b.status, 400);
    assert.ok(b.headers.getSetCookie().some((c) => c.startsWith("aa_sso_flow=;")));
  } finally {
    await app.close();
  }
});

test("express adapter: back-channel logout reads a raw body or a pre-parsed req.body", async () => {
  const ended = [];
  const handlers = createExpressHandlers(client, { onSignedIn: () => {}, onLogout: (logout) => ended.push(logout.jti) });
  const app = await serve((req, res) => {
    if (req.url === "/parsed") {
      req.body = { logout_token: logoutToken() };
    }
    return handlers.backchannelLogout(req, res);
  });
  try {
    /**
     * @summary    POST a form body to the app.
     * @param {string} path
     * @param {string} body
     * @returns {Promise<Response>}
     */
    const post = (path, body) =>
      fetch(`${app.base}${path}`, { method: "POST", headers: { "content-type": "application/x-www-form-urlencoded" }, body });
    assert.equal((await post("/raw", `logout_token=${logoutToken()}`)).status, 200);
    assert.equal((await post("/parsed", "")).status, 200);
    assert.equal(ended.length, 2);
    assert.equal((await post("/raw", `logout_token=${logoutToken({ jti: undefined })}`)).status, 400);
    assert.equal((await post("/raw", "nothing=here")).status, 400);
    assert.equal((await post("/raw", `logout_token=${"x".repeat(70 * 1024)}`)).status, 400);
  } finally {
    await app.close();
  }
});

test("express adapter: options are validated", () => {
  assert.doesNotThrow(() => createExpressHandlers(client, {}));
  assert.throws(() => createExpressHandlers(client, null), (err) => err.code === "config");
  assert.throws(() => createExpressHandlers(client, { onSignedIn: 1 }), (err) => err.code === "config");
  assert.throws(() => createExpressHandlers(client, { returnToParam: " " }), (err) => err.code === "config");
  assert.throws(() => createExpressHandlers(client, { onSignedIn: () => {}, onError: 2 }), (err) => err.code === "config");
  assert.throws(() => createExpressHandlers(client, { onSignedIn: () => {}, onLogout: 2 }), (err) => err.code === "config");
  assert.equal(createExpressHandlers(client, { onSignedIn: () => {} }).backchannelLogout, undefined);
});
