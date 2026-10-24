// Copyright © Anointed Automation Corporation., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Runs one sign-in: open the start route in a system auth session, read the deep-link callback, check the state.

import { AnointedSignInError, ERROR_CODES, createSignInRequest } from "./request.js";
import { createExpoOpener } from "./openers.js";

/**
 * @summary    Create the sign-in runner.
 * @param {object} options
 * @param {string} options.start       Your server's https start route (the route @anointedautomation/sso mounts). Required.
 * @param {object} options.callback    httpsCallback(...) or schemeCallback(...). Required.
 * @param {Function} [options.opener]  Pluggable opener. Default: expo-web-browser's openAuthSessionAsync.
 * @param {boolean} [options.ephemeral]  true = no shared browser cookies (the person signs in every time).
 * @param {(length: number) => Uint8Array} [options.randomBytes]  Secure random source if `crypto.getRandomValues` is missing.
 * @returns {{signIn: () => Promise<Record<string, string>>}}
 */
export function createAnointedSignIn(options) {
  if (typeof options !== "object" || options === null) {
    throw new AnointedSignInError(ERROR_CODES.CONFIG, "@anointedautomation/sso-react-native: options with start and callback are required");
  }
  const opener = options.opener === undefined ? createExpoOpener() : options.opener;
  if (typeof opener !== "function") throw new AnointedSignInError(ERROR_CODES.CONFIG, "@anointedautomation/sso-react-native: opener must be a function");
  // Validate the configuration now, not at the first tap.
  createSignInRequest({ start: options.start, callback: options.callback, randomBytes: options.randomBytes });

  /**
   * @summary    Run the sign-in and return the query items your server put on the callback (for example `{code}`).
   * @returns {Promise<Record<string, string>>}
   * @throws {AnointedSignInError} cancelled, state_mismatch, missing_state, server_error, presentation or config
   */
  async function signIn() {
    const request = createSignInRequest({ start: options.start, callback: options.callback, randomBytes: options.randomBytes });
    let result;
    try {
      result = await opener(request.startUrl, request.callbackUrl, { ephemeral: options.ephemeral === true });
    } catch (error) {
      if (error instanceof AnointedSignInError) throw error;
      throw new AnointedSignInError(ERROR_CODES.PRESENTATION, error instanceof Error ? error.message : String(error));
    }
    if (typeof result !== "object" || result === null) throw new AnointedSignInError(ERROR_CODES.PRESENTATION, "the opener returned nothing");
    if (result.type === "cancel" || result.type === "dismiss") throw new AnointedSignInError(ERROR_CODES.CANCELLED, "the person closed the sign-in");
    if (result.type !== "success") throw new AnointedSignInError(ERROR_CODES.PRESENTATION, "the sign-in did not finish (" + String(result.type) + ")");
    return request.parseCallback(result.url);
  }

  return { signIn };
}
