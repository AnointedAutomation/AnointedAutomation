// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Pure core: request building, state check, openers and the runner (opener mocked).
import { test } from "node:test";
import assert from "node:assert/strict";
import { AnointedSignInError, createSignInRequest, httpsCallback, randomState, schemeCallback } from "../src/request.js";
import { createAnointedSignIn } from "../src/signIn.js";
import { createExpoOpener, createLinkingOpener } from "../src/openers.js";

const START = "https://api.yourapp.com/auth/anointed/start";
const fixed = (extra = {}) => ({ start: START, callback: httpsCallback("app.yourapp.com", "/anointed/done"), state: "abc_123", ...extra });
const code = (fn) => { try { fn(); } catch (e) { return e.code; } return null; };

test("https callback: returnTo carries the callback and aa_state", () => {
  const r = createSignInRequest(fixed());
  assert.equal(r.callbackUrl, "https://app.yourapp.com/anointed/done");
  assert.equal(r.returnTo, "https://app.yourapp.com/anointed/done?aa_state=abc_123");
  assert.equal(r.startUrl, START + "?returnTo=" + encodeURIComponent(r.returnTo));
});

test("scheme callback: returnTo is the via route, callback is scheme://anointed-sign-in", () => {
  const r = createSignInRequest(fixed({ callback: schemeCallback("myapp", "https://api.yourapp.com/auth/anointed/app-done") }));
  assert.equal(r.callbackUrl, "myapp://anointed-sign-in");
  assert.equal(r.returnTo, "https://api.yourapp.com/auth/anointed/app-done?aa_state=abc_123");
});

test("an existing returnTo on the start URL is replaced, other query and hash kept", () => {
  const r = createSignInRequest(fixed({ start: START + "?x=1&returnTo=%2Fold#h" }));
  assert.equal(r.startUrl, START + "?x=1&returnTo=" + encodeURIComponent(r.returnTo) + "#h");
});

test("config errors fail fast", () => {
  assert.equal(code(() => createSignInRequest(fixed({ start: "http://x.com/s" }))), "config");
  assert.equal(code(() => createSignInRequest(fixed({ start: undefined }))), "config");
  assert.equal(code(() => createSignInRequest(fixed({ callback: undefined }))), "config");
  assert.equal(code(() => createSignInRequest(fixed({ callback: httpsCallback("a/b", "/p") }))), "config");
  assert.equal(code(() => createSignInRequest(fixed({ callback: schemeCallback("https", "https://x.com/v") }))), "config");
  assert.equal(code(() => createSignInRequest(fixed({ callback: schemeCallback("1app", "https://x.com/v") }))), "config");
  assert.equal(code(() => createSignInRequest(fixed({ callback: schemeCallback("myapp", "myapp://v") }))), "config");
  assert.equal(code(() => createSignInRequest(fixed({ state: "" }))), "config");
});

test("parseCallback returns items minus state, and rejects foreign or failed callbacks", () => {
  const r = createSignInRequest(fixed());
  assert.deepEqual(r.parseCallback("myapp://x?code=ZZ%20y&aa_state=abc_123"), { code: "ZZ y" });
  assert.equal(code(() => r.parseCallback("myapp://x?code=1")), "missing_state");
  assert.equal(code(() => r.parseCallback("myapp://x?aa_state=other&code=1")), "state_mismatch");
  assert.equal(code(() => r.parseCallback("myapp://x?aa_state=abc_123&error=access_denied")), "server_error");
  assert.equal(code(() => r.parseCallback(undefined)), "presentation");
});

test("randomState is 43 base64url chars, differs per call, and never falls back to Math.random", () => {
  const a = randomState();
  assert.match(a, /^[A-Za-z0-9_-]{43}$/);
  assert.notEqual(a, randomState());
  assert.equal(randomState(() => new Uint8Array(32)), "A".repeat(43));
  const saved = Object.getOwnPropertyDescriptor(globalThis, "crypto");
  Object.defineProperty(globalThis, "crypto", { value: undefined, configurable: true });
  try { assert.equal(code(() => randomState()), "config"); } finally { Object.defineProperty(globalThis, "crypto", saved); }
});

/** Opener that echoes the returnTo it was given back as the callback. */
const echoOpener = (extra = "") => async (startUrl, callbackUrl) => {
  const returnTo = decodeURIComponent(startUrl.split("returnTo=")[1]);
  return { type: "success", url: callbackUrl + "?" + returnTo.split("?")[1] + extra };
};

test("signIn opens start route and returns the server's items", async () => {
  const seen = [];
  const opener = async (...args) => { seen.push(args); return echoOpener("&code=ONE")(...args); };
  const { signIn } = createAnointedSignIn({ start: START, callback: schemeCallback("myapp", "https://api.yourapp.com/done"), opener, ephemeral: true });
  assert.deepEqual(await signIn(), { code: "ONE" });
  assert.match(seen[0][0], /^https:\/\/api\.yourapp\.com\/auth\/anointed\/start\?returnTo=/);
  assert.equal(seen[0][1], "myapp://anointed-sign-in");
  assert.deepEqual(seen[0][2], { ephemeral: true });
});

test("each signIn uses a fresh state", async () => {
  const states = [];
  const opener = async (u, c) => { states.push(u); return echoOpener()(u, c); };
  const { signIn } = createAnointedSignIn({ start: START, callback: httpsCallback("a.com", "/d"), opener });
  await signIn(); await signIn();
  assert.notEqual(states[0], states[1]);
});

test("signIn maps cancel, dismiss, foreign state, server error and opener faults", async () => {
  const run = (opener) => createAnointedSignIn({ start: START, callback: httpsCallback("a.com", "/d"), opener }).signIn().then(() => null, (e) => e);
  assert.equal((await run(async () => ({ type: "cancel" }))).code, "cancelled");
  assert.equal((await run(async () => ({ type: "dismiss" }))).code, "cancelled");
  assert.equal((await run(async () => ({ type: "success", url: "https://a.com/d?aa_state=nope" }))).code, "state_mismatch");
  assert.equal((await run(echoOpener("&error=denied"))).code, "server_error");
  assert.equal((await run(async () => { throw new Error("boom"); })).code, "presentation");
  assert.equal((await run(async () => ({ type: "locked" }))).code, "presentation");
  assert.ok((await run(async () => { throw new Error("boom"); })) instanceof AnointedSignInError);
});

test("createAnointedSignIn validates config up front", () => {
  assert.throws(() => createAnointedSignIn({ callback: httpsCallback("a.com", "/d"), opener: async () => ({}) }), /https URL/);
  assert.throws(() => createAnointedSignIn({ start: START, callback: httpsCallback("a.com", "/d"), opener: 5 }), /opener must be a function/);
});

test("expo opener forwards to openAuthSessionAsync with the ephemeral flag", async () => {
  const calls = [];
  const opener = createExpoOpener({ openAuthSessionAsync: async (...a) => { calls.push(a); return { type: "cancel" }; } });
  assert.deepEqual(await opener("https://s", "myapp://anointed-sign-in", { ephemeral: true }), { type: "cancel" });
  assert.deepEqual(calls[0], ["https://s", "myapp://anointed-sign-in", { preferEphemeralSession: true }]);
});

test("expo opener without expo-web-browser installed says so", async () => {
  await assert.rejects(createExpoOpener()("https://s", "myapp://x"), /install expo-web-browser/);
});

test("linking opener resolves on the matching url event and ignores others", async () => {
  let handler; let removed = 0; const opened = [];
  const linking = {
    openURL: async (u) => { opened.push(u); handler({ url: "other://x" }); handler({ url: "myapp://anointed-sign-in?aa_state=s" }); },
    addEventListener: (_n, h) => { handler = h; return { remove: () => { removed++; } }; },
  };
  assert.deepEqual(await createLinkingOpener(linking)("https://s", "myapp://anointed-sign-in"), { type: "success", url: "myapp://anointed-sign-in?aa_state=s" });
  assert.deepEqual(opened, ["https://s"]);
  assert.equal(removed, 1);
  assert.throws(() => createLinkingOpener({}), /Linking/);
});
