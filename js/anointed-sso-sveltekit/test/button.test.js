// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// The button: compile the Svelte 5 component, render it on the server and check the markup; plus the URL helper.

import { test } from "node:test";
import assert from "node:assert/strict";
import { mkdtempSync, readFileSync, rmSync, writeFileSync } from "node:fs";
import { fileURLToPath } from "node:url";
import { compile } from "svelte/compiler";
import { render } from "svelte/server";
import { buildStartUrl, requireStartUrl } from "../src/startUrl.js";

const root = fileURLToPath(new URL("..", import.meta.url));
const source = readFileSync(new URL("../src/AnointedSignInButton.svelte", import.meta.url), "utf8");

/**
 * @summary    Compile the component for the server into a temp module inside the package (so svelte resolves) and import it.
 * @returns {Promise<{component: unknown, cleanup: () => void}>}
 */
async function loadButton() {
  const dir = mkdtempSync(root + "/.tmp-button-");
  const { js } = compile(source.replace("./startUrl.js", fileURLToPath(new URL("../src/startUrl.js", import.meta.url))), { generate: "server", filename: "AnointedSignInButton.svelte" });
  writeFileSync(dir + "/Button.js", js.code);
  const mod = await import(dir + "/Button.js");
  return { component: mod.default, cleanup: () => rmSync(dir, { recursive: true, force: true }) };
}

test("renders an accessible link to the start route with the brand label", async () => {
  const { component, cleanup } = await loadButton();
  try {
    const { body } = render(component, { props: { startUrl: "/auth/anointed/start", returnTo: "/dashboard" } });
    assert.match(body, /<a [^>]*href="\/auth\/anointed\/start\?returnTo=%2Fdashboard"/);
    assert.match(body, /Sign in with Anointed Automation/);
    assert.match(body, /<img [^>]*alt=""[^>]*aria-hidden="true"/);
    assert.match(body, /data-sveltekit-reload/);
    assert.doesNotMatch(body, /secret/i);
  } finally {
    cleanup();
  }
});

test("label, class and disabled are honoured", async () => {
  const { component, cleanup } = await loadButton();
  try {
    const custom = render(component, { props: { startUrl: "/s", label: "Continue", class: "mine" } }).body;
    assert.match(custom, /Continue/);
    assert.match(custom, /class="[^"]*\bmine\b/);
    const off = render(component, { props: { startUrl: "/s", disabled: true } }).body;
    assert.doesNotMatch(off, /href=/);
    assert.match(off, /aria-disabled="true"/);
  } finally {
    cleanup();
  }
});

test("a missing startUrl fails fast", async () => {
  const { component, cleanup } = await loadButton();
  try {
    assert.throws(() => render(component, { props: {} }).body, /startUrl is required/);
  } finally {
    cleanup();
  }
});

test("buildStartUrl keeps query and hash, encodes returnTo and honours returnToParam", () => {
  assert.equal(buildStartUrl("/s"), "/s");
  assert.equal(buildStartUrl("/s", "/a b"), "/s?returnTo=%2Fa%20b");
  assert.equal(buildStartUrl("/s?x=1#h", "/o", "next"), "/s?x=1&next=%2Fo#h");
  assert.throws(() => buildStartUrl("", "/o"), /startUrl is required/);
  assert.throws(() => requireStartUrl(undefined), /startUrl is required/);
  assert.throws(() => buildStartUrl("/s", 5), /returnTo must be a string/);
  assert.throws(() => buildStartUrl("/s", "/o", ""), /returnToParam/);
});
