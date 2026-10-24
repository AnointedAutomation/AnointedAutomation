// Copyright © Anointed Automation Corporation., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Tests against the BUILT package (npm test builds first): server render of the button, the service, the provider, and the pure URL builder.

import "@angular/compiler";
import { test } from "node:test";
import assert from "node:assert/strict";
import { Component, createEnvironmentInjector, EnvironmentInjector, Injector, provideZonelessChangeDetection } from "@angular/core";
import { bootstrapApplication } from "@angular/platform-browser";
import { renderApplication, provideServerRendering } from "@angular/platform-server";
import {
  ANOINTED_SIGN_IN_CONFIG,
  AnointedSignInButtonComponent,
  AnointedSignInService,
  DEFAULT_LABEL,
  buildStartUrl,
  provideAnointedSignIn,
} from "../dist/fesm2022/anointedautomation-sso-angular.mjs";

const START = "/api/auth/anointed/start";

/**
 * Server render a host template containing the button.
 * @param {string} template Host template.
 * @param {object} config provideAnointedSignIn config.
 * @returns {Promise<string>} HTML.
 */
function render(template, config) {
  class Host {}
  Component({ selector: "app-root", standalone: true, imports: [AnointedSignInButtonComponent], template })(Host);
  const bootstrap = (context) =>
    bootstrapApplication(Host, { providers: [provideZonelessChangeDetection(), provideServerRendering(), provideAnointedSignIn(config)] }, context);
  return renderApplication(bootstrap, { document: "<html><body><app-root></app-root></body></html>" });
}

test("button renders an accessible <button> with the default label and decorative logo", async () => {
  const html = await render("<anointed-sign-in-button />", { startUrl: START });
  assert.match(html, /<button[^>]*type="button"/);
  assert.ok(html.includes(DEFAULT_LABEL));
  assert.match(html, /<img[^>]*alt=""[^>]*aria-hidden="true"/);
  assert.ok(html.includes("data-anointed-sign-in"));
});

test("button honors label and disabled", async () => {
  const html = await render('<anointed-sign-in-button label="Continue" [disabled]="true" />', { startUrl: START });
  assert.ok(html.includes(">Continue<"));
  assert.match(html, /<button[^>]*disabled/);
});

/**
 * Build an injector with the provider and return the service.
 * @param {object} config provideAnointedSignIn config.
 * @returns {AnointedSignInService} The service.
 */
function serviceFor(config) {
  const injector = createEnvironmentInjector([provideAnointedSignIn(config), AnointedSignInService], Injector.NULL);
  return injector.get(AnointedSignInService);
}

test("service signIn navigates to the start route, with and without returnTo", () => {
  const visited = [];
  const service = serviceFor({ startUrl: START, navigate: (url) => visited.push(url) });
  service.signIn();
  service.signIn("/orders?tab=open");
  assert.deepEqual(visited, [START, `${START}?returnTo=%2Forders%3Ftab%3Dopen`]);
});

test("service honors returnToParam from config and per call", () => {
  const visited = [];
  const service = serviceFor({ startUrl: START, returnToParam: "next", navigate: (url) => visited.push(url) });
  service.signIn("/a");
  service.signIn("/b", "to");
  assert.deepEqual(visited, [`${START}?next=%2Fa`, `${START}?to=%2Fb`]);
});

test("provider fails fast without a usable config", () => {
  assert.throws(() => provideAnointedSignIn({}), /startUrl is required/);
  assert.throws(() => provideAnointedSignIn({ startUrl: "  " }), /startUrl is required/);
  assert.throws(() => provideAnointedSignIn(undefined), /requires a config object/);
  assert.throws(() => provideAnointedSignIn({ startUrl: START, navigate: "x" }), /navigate must be a function/);
  assert.ok(ANOINTED_SIGN_IN_CONFIG);
});

test("buildStartUrl keeps query and hash, encodes, and validates", () => {
  assert.equal(buildStartUrl(START), START);
  assert.equal(buildStartUrl(`${START}?x=1#h`, "/a b"), `${START}?x=1&returnTo=%2Fa%20b#h`);
  assert.equal(buildStartUrl(START, "/a", "next"), `${START}?next=%2Fa`);
  assert.throws(() => buildStartUrl(""), /startUrl is required/);
  assert.throws(() => buildStartUrl(START, 5), /returnTo must be a string/);
  assert.throws(() => buildStartUrl(START, "/a", " "), /returnToParam/);
});
