// Copyright © Anointed Automation Corporation., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// The composable: `signIn(returnTo?)` sends the browser to YOUR server's start route. The server (using
// @anointedautomation/sso) holds the client secret, builds the PKCE request and redirects to Anointed Automation.

import { buildStartUrl, requireStartUrl } from "./startUrl.js";

/**
 * @summary    Default navigation: a full page load, because the start route answers with a redirect to another site.
 * @param {string} url
 * @returns {void}
 */
function assignLocation(url) {
  window.location.assign(url);
}

/**
 * @summary    Sign in with Anointed Automation from a Vue 3 component or composable.
 * @param {object} options
 * @param {string} options.startUrl        Required. Path or URL of your server's start route.
 * @param {string} [options.returnToParam] Query parameter your server reads. Default `returnTo`.
 * @param {(url: string) => void} [options.navigate]  Override how the browser navigates (tests, custom routers). Default `window.location.assign`.
 * @returns {{signIn: (returnTo?: string) => void}}
 * @throws {Error} when `startUrl` is missing
 */
export function useAnointedSignIn(options) {
  if (typeof options !== "object" || options === null) {
    throw new Error("@anointedautomation/sso-vue: useAnointedSignIn requires an options object with startUrl");
  }
  const startUrl = requireStartUrl(options.startUrl);
  const returnToParam = options.returnToParam;
  const navigate = options.navigate === undefined ? assignLocation : options.navigate;
  if (typeof navigate !== "function") throw new Error("@anointedautomation/sso-vue: navigate must be a function");

  /**
   * @summary    Navigate the browser to the start route, carrying `returnTo` when given.
   * @param {string} [returnTo]
   * @returns {void}
   */
  function signIn(returnTo) {
    navigate(returnToParam === undefined ? buildStartUrl(startUrl, returnTo) : buildStartUrl(startUrl, returnTo, returnToParam));
  }

  return { signIn };
}
