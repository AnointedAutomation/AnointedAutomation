// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Option validation shared by the web and Express adapters.

import { AnointedSsoError } from "../errors.js";

/** Default query parameter the start handlers read the return target from. */
export const DEFAULT_RETURN_TO_PARAM = "returnTo";

/**
 * @summary    Validate adapter options (fail fast) and return the return-target query parameter name.
 * @description
 *   `onSignedIn` is optional here: only the callback handler needs it, and it fails with a `config` error
 *   when called without one. A start-only route can therefore be built with no options at all.
 * @param {unknown} options
 * @returns {string}  The return parameter name (default `returnTo`).
 * @throws {AnointedSsoError} `config`
 */
export function validateCommonOptions(options) {
  if (typeof options !== "object" || options === null) throw new AnointedSsoError("config", "options must be an object");
  for (const key of ["onSignedIn", "onError", "onLogout"]) {
    if (options[key] !== undefined && typeof options[key] !== "function") {
      throw new AnointedSsoError("config", `${key} must be a function`);
    }
  }
  if (options.returnToParam === undefined) return DEFAULT_RETURN_TO_PARAM;
  if (typeof options.returnToParam !== "string" || options.returnToParam.trim() === "") {
    throw new AnointedSsoError("config", "returnToParam must be a non-empty string");
  }
  return options.returnToParam;
}

/**
 * @summary    Throw when the callback handler runs without `onSignedIn`.
 * @param {{onSignedIn?: unknown}} options
 * @returns {void}
 * @throws {AnointedSsoError} `config`
 */
export function requireOnSignedIn(options) {
  if (typeof options.onSignedIn !== "function") {
    throw new AnointedSsoError("config", "onSignedIn is required for the callback handler");
  }
}
