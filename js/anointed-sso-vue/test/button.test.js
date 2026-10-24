// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Server render of the button and the composable, plus the pure URL builder.

import { test } from "node:test";
import assert from "node:assert/strict";
import { createSSRApp, h } from "vue";
import { renderToString } from "@vue/server-renderer";
import { AnointedSignInButton, DEFAULT_LABEL, buildStartUrl, useAnointedSignIn } from "../src/index.js";

const START = "/api/auth/anointed/start";

/**
 * @summary    Server render a root component built by `render`.
 * @param {() => import("vue").VNode} render
 * @returns {Promise<string>}
 */
function renderRoot(render) {
  return renderToString(createSSRApp({ render }));
}

test("button renders an accessible <button> with the default label and logo", async () => {
  const html = await renderRoot(() => h(AnointedSignInButton, { startUrl: START }));
  assert.match(html, /<button[^>]*type="button"/);
  assert.ok(html.includes(DEFAULT_LABEL));
  assert.match(html, /<img[^>]* alt[ =>][^>]*aria-hidden="true"/);
  assert.ok(html.includes("style="));
  assert.ok(html.includes("data-anointed-sign-in"));
});

test("button honors label, class (drops inline look) and disabled", async () => {
  const html = await renderRoot(() => h(AnointedSignInButton, { startUrl: START, label: "Continue", class: "my-btn", disabled: true }));
  assert.ok(html.includes(">Continue<"));
  assert.ok(html.includes('class="my-btn"'));
  assert.ok(!html.includes("border-radius"));
  assert.match(html, /disabled/);
});

test("button fails fast without startUrl", async () => {
  await assert.rejects(() => renderRoot(() => h(AnointedSignInButton, {})), /startUrl is required/);
  await assert.rejects(() => renderRoot(() => h(AnointedSignInButton, { startUrl: "  " })), /startUrl is required/);
});

test("button click emits click then navigates to the start route with returnTo", () => {
  const visited = [];
  const emitted = [];
  const render = AnointedSignInButton.setup(
    { startUrl: START, returnTo: "/dashboard", disabled: false, navigate: (url) => visited.push(url) },
    { attrs: {}, emit: (name) => emitted.push(name) },
  );
  const vnode = render();
  assert.equal(vnode.type, "button");
  vnode.props.onClick({});
  assert.deepEqual(emitted, ["click"]);
  assert.deepEqual(visited, [`${START}?returnTo=%2Fdashboard`]);
});

/**
 * @summary    Render a component that calls the composable and hands back its result.
 * @param {object} options
 * @returns {Promise<{signIn: (returnTo?: string) => void}>}
 */
async function captureComposable(options) {
  let captured = null;
  await renderRoot(() =>
    h({
      setup() {
        captured = useAnointedSignIn(options);
        return () => null;
      },
    }),
  );
  return captured;
}

test("composable signIn navigates to the start route, with and without returnTo", async () => {
  const visited = [];
  const { signIn } = await captureComposable({ startUrl: START, navigate: (url) => visited.push(url) });
  signIn();
  signIn("/orders?tab=open");
  assert.deepEqual(visited, [START, `${START}?returnTo=%2Forders%3Ftab%3Dopen`]);
});

test("composable honors returnToParam and validates its options", async () => {
  const visited = [];
  const { signIn } = await captureComposable({ startUrl: START, returnToParam: "next", navigate: (url) => visited.push(url) });
  signIn("/a");
  assert.deepEqual(visited, [`${START}?next=%2Fa`]);
  await assert.rejects(() => captureComposable(undefined), /options object/);
  await assert.rejects(() => captureComposable({}), /startUrl is required/);
  await assert.rejects(() => captureComposable({ startUrl: START, navigate: "no" }), /navigate must be a function/);
});

test("buildStartUrl keeps existing query and hash, and rejects bad input", () => {
  assert.equal(buildStartUrl("https://api.example/start?x=1#h", "/p"), "https://api.example/start?x=1&returnTo=%2Fp#h");
  assert.equal(buildStartUrl(START, null), START);
  assert.throws(() => buildStartUrl(START, 5), /returnTo must be a string/);
  assert.throws(() => buildStartUrl(START, "/p", ""), /returnToParam/);
});
