// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Button and hook, server-rendered with a react-native stub (the real module only exists inside an app).
import { test, before } from "node:test";
import assert from "node:assert/strict";
import { register } from "node:module";
import { createElement } from "react";
import { renderToStaticMarkup } from "react-dom/server";

let kit;
before(async () => {
  register("./stubs/hooks.mjs", import.meta.url);
  kit = await import("../src/index.js");
});

test("button renders a labelled accessible button with a hidden decorative logo", () => {
  const html = renderToStaticMarkup(createElement(kit.AnointedSignInButton, { onPress: () => {} }));
  assert.match(html, /data-role="button"/);
  assert.ok(html.includes('aria-label="' + kit.DEFAULT_LABEL + '"'));
  assert.ok(html.includes(">" + kit.DEFAULT_LABEL + "<"));
  assert.ok(html.includes('src="' + kit.LOGO_URL + '"'));
  assert.ok(html.includes('data-accessible="false"'));
  assert.ok(html.includes("minHeight&quot;:48") || html.includes('minHeight":48'));
  assert.ok(html.includes('data-has-press="true"'));
});

test("button honors label, disabled and loading (busy, press blocked)", () => {
  const off = renderToStaticMarkup(createElement(kit.AnointedSignInButton, { onPress: () => {}, label: "Continue", disabled: true }));
  assert.ok(off.includes(">Continue<"));
  assert.match(off, /disabled=""/);
  assert.ok(off.includes('data-has-press="false"'));
  const busy = renderToStaticMarkup(createElement(kit.AnointedSignInButton, { onPress: () => {}, loading: true }));
  assert.ok(busy.includes('data-busy="true"'));
  assert.ok(busy.includes('data-has-press="false"'));
});

test("button fails fast without onPress", () => {
  assert.throws(() => renderToStaticMarkup(createElement(kit.AnointedSignInButton, {})), /requires onPress/);
});

test("hook exposes idle state and fails fast on bad config", () => {
  let hook;
  function Probe() {
    hook = kit.useAnointedSignIn({ start: "https://api.yourapp.com/s", callback: kit.schemeCallback("myapp", "https://api.yourapp.com/v"), opener: async () => ({ type: "cancel" }) });
    return null;
  }
  renderToStaticMarkup(createElement(Probe));
  assert.equal(hook.status, "idle");
  assert.equal(hook.result, null);
  assert.equal(hook.error, null);
  assert.equal(typeof hook.signIn, "function");
  function Bad() { kit.useAnointedSignIn({ start: "http://x", callback: kit.schemeCallback("myapp", "https://x.com/v") }); return null; }
  assert.throws(() => renderToStaticMarkup(createElement(Bad)), /https URL/);
});
