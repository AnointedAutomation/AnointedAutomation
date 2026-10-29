// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
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
  for (const key of ["onSignedIn", "onError", "onLogout", "logger"]) {
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

/** Error codes caused by the user or their browser (stale tab, declined consent): logged at "warn", not "error". */
const USER_CAUSED_CODES = new Set([
  "flow_missing",
  "flow_expired",
  "state_mismatch",
  "access_denied",
  "missing_code",
  "email_not_verified",
  "redirect_uri_rejected",
  "logout_token_invalid",
]);

/** Error codes that mean the provider (or the network to it) failed: answered 502, not 400. */
const UPSTREAM_CODES = new Set(["http", "discovery"]);

/**
 * @summary    The default logger: one console line per failure, at the level given.
 * @param {"warn" | "error"} level
 * @param {string} message
 * @param {Record<string, unknown>} details
 * @returns {void}
 */
function consoleLogger(level, message, details) {
  if (level === "warn") {
    console.warn(message, details);
  } else {
    console.error(message, details);
  }
}

/**
 * @summary    The logger an adapter reports failures to: `options.logger` when given, else the console.
 * @description The logger is called as `logger(level, message, details)`, `level` being "warn" or "error".
 * @param {{logger?: (level: "warn" | "error", message: string, details: Record<string, unknown>) => void}} options
 * @returns {(level: "warn" | "error", message: string, details: Record<string, unknown>) => void}
 */
export function resolveLogger(options) {
  return options.logger === undefined ? consoleLogger : options.logger;
}

/**
 * @summary    What a log line needs from a failure: code, status, OAuth error, message, body snippet and cause.
 * @param {unknown} err
 * @returns {Record<string, unknown>}
 */
export function describeError(err) {
  if (err instanceof AnointedSsoError) {
    const details = { code: err.code, message: err.message };
    if (err.status !== undefined) details.status = err.status;
    if (err.oauthError !== undefined) details.oauthError = err.oauthError;
    if (err.bodySnippet !== undefined) details.bodySnippet = err.bodySnippet;
    if (err.cause !== undefined) details.cause = err.cause instanceof Error ? `${err.cause.name}: ${err.cause.message}` : String(err.cause);
    return details;
  }
  if (err instanceof Error) return { name: err.name, message: err.message, stack: err.stack };
  return { value: String(err) };
}

/**
 * @summary    "warn" for user-caused failures, "error" for everything else (our config, the provider, the network).
 * @param {unknown} err
 * @returns {"warn" | "error"}
 */
export function levelOf(err) {
  return err instanceof AnointedSsoError && USER_CAUSED_CODES.has(err.code) ? "warn" : "error";
}

/**
 * @summary    The status of the default failure response: 502 when the provider failed, 400 otherwise.
 * @param {unknown} err
 * @returns {number}
 */
export function defaultFailureStatus(err) {
  return err instanceof AnointedSsoError && UPSTREAM_CODES.has(err.code) ? 502 : 400;
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
