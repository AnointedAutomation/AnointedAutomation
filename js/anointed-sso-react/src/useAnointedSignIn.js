// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
"use client";
// The hook: `signIn(returnTo?)` sends the browser to YOUR server's start route. The server (using
// @anointedautomation/sso) holds the client secret, builds the PKCE request and redirects to Anointed Automation.

import { useCallback } from "react";
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
 * @summary    Sign in with Anointed Automation from a React component.
 * @param {object} options
 * @param {string} options.startUrl        Required. Path or URL of your server's start route.
 * @param {string} [options.returnToParam] Query parameter your server reads. Default `returnTo`.
 * @param {(url: string) => void} [options.navigate]  Override how the browser navigates (tests, custom routers). Default `window.location.assign`.
 * @returns {{signIn: (returnTo?: string) => void}}
 * @throws {Error} when `startUrl` is missing
 */
export function useAnointedSignIn(options) {
  if (typeof options !== "object" || options === null) {
    throw new Error("@anointedautomation/sso-react: useAnointedSignIn requires an options object with startUrl");
  }
  const startUrl = requireStartUrl(options.startUrl);
  const returnToParam = options.returnToParam;
  const navigate = options.navigate === undefined ? assignLocation : options.navigate;
  if (typeof navigate !== "function") throw new Error("@anointedautomation/sso-react: navigate must be a function");

  const signIn = useCallback(
    (returnTo) => {
      navigate(returnToParam === undefined ? buildStartUrl(startUrl, returnTo) : buildStartUrl(startUrl, returnTo, returnToParam));
    },
    [startUrl, returnToParam, navigate],
  );

  return { signIn };
}
