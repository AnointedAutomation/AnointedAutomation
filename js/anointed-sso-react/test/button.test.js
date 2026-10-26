// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Server render of the button and the hook, plus the pure URL builder.

import { test } from "node:test";
import assert from "node:assert/strict";
import { createElement } from "react";
import { renderToString } from "react-dom/server";
import { AnointedSignInButton, DEFAULT_LABEL, buildStartUrl, useAnointedSignIn } from "../src/index.js";

const START = "/api/auth/anointed/start";

test("button renders an accessible <button> with the default label and logo", () => {
  const html = renderToString(createElement(AnointedSignInButton, { startUrl: START }));
  assert.match(html, /<button type="button"/);
  assert.ok(html.includes(DEFAULT_LABEL));
  assert.match(html, /<img[^>]*alt=""[^>]*aria-hidden="true"/);
  assert.ok(html.includes("style="));
});

test("button honors label, className (drops inline look) and disabled", () => {
  const html = renderToString(createElement(AnointedSignInButton, { startUrl: START, label: "Continue", className: "my-btn", disabled: true }));
  assert.ok(html.includes(">Continue<"));
  assert.ok(html.includes('class="my-btn"'));
  assert.ok(!html.includes("border-radius"));
  assert.match(html, /disabled/);
});

test("button and hook fail fast without startUrl", () => {
  assert.throws(() => renderToString(createElement(AnointedSignInButton, {})), /startUrl is required/);
  assert.throws(() => renderToString(createElement(AnointedSignInButton, { startUrl: "  " })), /startUrl is required/);
});

/**
 * @summary    Render a component that calls the hook and hands back its result.
 * @param {object} options
 * @returns {{signIn: (returnTo?: string) => void}}
 */
function captureHook(options) {
  let captured = null;
  function Probe() {
    captured = useAnointedSignIn(options);
    return null;
  }
  renderToString(createElement(Probe));
  return captured;
}

test("hook signIn navigates to the start route, with and without returnTo", () => {
  const visited = [];
  const { signIn } = captureHook({ startUrl: START, navigate: (url) => visited.push(url) });
  signIn();
  signIn("/orders?tab=open");
  assert.deepEqual(visited, [START, `${START}?returnTo=%2Forders%3Ftab%3Dopen`]);
});

test("hook honors returnToParam and validates its options", () => {
  const visited = [];
  const { signIn } = captureHook({ startUrl: START, returnToParam: "next", navigate: (url) => visited.push(url) });
  signIn("/a");
  assert.deepEqual(visited, [`${START}?next=%2Fa`]);
  assert.throws(() => captureHook(undefined), /options object/);
  assert.throws(() => captureHook({}), /startUrl is required/);
  assert.throws(() => captureHook({ startUrl: START, navigate: "no" }), /navigate must be a function/);
});

test("buildStartUrl keeps existing query and hash, and rejects bad input", () => {
  assert.equal(buildStartUrl("https://api.example/start?x=1#h", "/p"), "https://api.example/start?x=1&returnTo=%2Fp#h");
  assert.equal(buildStartUrl(START, null), START);
  assert.throws(() => buildStartUrl(START, 5), /returnTo must be a string/);
  assert.throws(() => buildStartUrl(START, "/p", ""), /returnToParam/);
});
