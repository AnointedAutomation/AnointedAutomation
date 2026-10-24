// Copyright © Anointed Automation Corporation., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Pure core: builds one sign-in attempt (start URL + state) and checks the callback. Same contract as the
// Swift and Android kits: the app opens YOUR server's start route with returnTo=<callback + aa_state>,
// and the callback must carry the same aa_state back. No secrets, no network, no platform imports.

export const STATE_ITEM = "aa_state";
export const SCHEME_HOST = "anointed-sign-in";

export const ERROR_CODES = Object.freeze({
  CONFIG: "config",
  CANCELLED: "cancelled",
  STATE_MISMATCH: "state_mismatch",
  MISSING_STATE: "missing_state",
  SERVER_ERROR: "server_error",
  PRESENTATION: "presentation",
});

/** An error raised by the kit. `code` is one of ERROR_CODES; none is retried for you. */
export class AnointedSignInError extends Error {
  /**
   * @param {string} code     One of ERROR_CODES.
   * @param {string} message
   */
  constructor(code, message) {
    super(message);
    this.name = "AnointedSignInError";
    this.code = code;
  }
}

/**
 * @summary    Shorthand for a configuration error.
 * @param {string} message
 * @returns {AnointedSignInError}
 */
function configError(message) {
  return new AnointedSignInError(ERROR_CODES.CONFIG, "@anointedautomation/sso-react-native: " + message);
}

/**
 * @summary    Callback built from an https universal / app link on your own domain (preferred when the platform and opener support it).
 * @param {string} host   Host name only, for example app.yourapp.com.
 * @param {string} path   Path, for example /anointed/done.
 * @returns {{kind: "https", host: string, path: string}}
 */
export function httpsCallback(host, path) {
  return { kind: "https", host, path };
}

/**
 * @summary    Callback built from a custom URL scheme. Works everywhere.
 * @param {string} scheme  Your app's scheme, for example myapp.
 * @param {string} via     An https route on YOUR server that forwards every query item to <scheme>://anointed-sign-in.
 * @returns {{kind: "scheme", scheme: string, via: string}}
 */
export function schemeCallback(scheme, via) {
  return { kind: "scheme", scheme, via };
}

/**
 * @summary    Encode bytes as base64url without padding (no Buffer or btoa needed, works on Hermes).
 * @param {Uint8Array} bytes
 * @returns {string}
 */
export function base64Url(bytes) {
  const alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_";
  let out = "";
  for (let i = 0; i < bytes.length; i += 3) {
    const b0 = bytes[i];
    const b1 = i + 1 < bytes.length ? bytes[i + 1] : 0;
    const b2 = i + 2 < bytes.length ? bytes[i + 2] : 0;
    out += alphabet[b0 >> 2] + alphabet[((b0 & 3) << 4) | (b1 >> 4)];
    if (i + 1 < bytes.length) out += alphabet[((b1 & 15) << 2) | (b2 >> 6)];
    if (i + 2 < bytes.length) out += alphabet[b2 & 63];
  }
  return out;
}

/**
 * @summary    32 random bytes as base64url. There is no weak fallback: without a secure source this throws.
 * @param {(length: number) => Uint8Array} [randomBytes]  Secure random source, for example expo-crypto's getRandomBytes. Default `globalThis.crypto.getRandomValues`.
 * @returns {string}
 * @throws {AnointedSignInError} config, when no secure random source exists
 */
export function randomState(randomBytes) {
  if (randomBytes !== undefined) {
    if (typeof randomBytes !== "function") throw configError("randomBytes must be a function (length) => Uint8Array");
    return base64Url(randomBytes(32));
  }
  const source = globalThis.crypto;
  if (typeof source !== "object" || source === null || typeof source.getRandomValues !== "function") {
    throw configError("no secure random source. Pass randomBytes (for example getRandomBytes from expo-crypto) or install a crypto.getRandomValues polyfill");
  }
  return base64Url(source.getRandomValues(new Uint8Array(32)));
}

/**
 * @summary    Parse the query string of a URL into an object (no URL class needed, works on Hermes). Fragment ignored.
 * @param {string} url
 * @returns {Record<string, string>}
 */
export function parseQuery(url) {
  const values = {};
  const noHash = url.split("#")[0];
  const at = noHash.indexOf("?");
  if (at === -1) return values;
  for (const pair of noHash.slice(at + 1).split("&")) {
    if (pair === "") continue;
    const eq = pair.indexOf("=");
    const rawKey = eq === -1 ? pair : pair.slice(0, eq);
    const rawValue = eq === -1 ? "" : pair.slice(eq + 1);
    values[decodeURIComponent(rawKey.replace(/\+/g, " "))] = decodeURIComponent(rawValue.replace(/\+/g, " "));
  }
  return values;
}

/**
 * @summary    Drop the named query parameter from a URL and keep the rest, hash included.
 * @param {string} url
 * @param {string} name
 * @returns {string}
 */
function withoutParam(url, name) {
  const hashAt = url.indexOf("#");
  const hash = hashAt === -1 ? "" : url.slice(hashAt);
  const base = hashAt === -1 ? url : url.slice(0, hashAt);
  const at = base.indexOf("?");
  if (at === -1) return url;
  const kept = base.slice(at + 1).split("&").filter((pair) => pair !== "" && decodeURIComponent(pair.split("=")[0]) !== name);
  return base.slice(0, at) + (kept.length > 0 ? "?" + kept.join("&") : "") + hash;
}

/**
 * @summary    Build one sign-in attempt.
 * @param {object} options
 * @param {string} options.start       Your server's start route. Absolute https URL.
 * @param {object} options.callback    From httpsCallback(...) or schemeCallback(...).
 * @param {string} [options.state]     Random per attempt; leave unset to generate one.
 * @param {(length: number) => Uint8Array} [options.randomBytes]  Secure random source used when `state` is unset.
 * @returns {{startUrl: string, callbackUrl: string, returnTo: string, state: string, parseCallback: (url: string) => Record<string, string>}}
 * @throws {AnointedSignInError} config, when a value is unusable
 */
export function createSignInRequest(options) {
  if (typeof options !== "object" || options === null) throw configError("options are required");
  const { start, callback } = options;
  if (typeof start !== "string" || !/^https:\/\/[^/?#\s]+/.test(start)) throw configError("start URL must be an absolute https URL");
  if (typeof callback !== "object" || callback === null) throw configError("callback is required (httpsCallback or schemeCallback)");

  let callbackUrl;
  let returnBase;
  if (callback.kind === "https") {
    const { host, path } = callback;
    if (typeof host !== "string" || host === "" || /[/:\s]/.test(host)) throw configError("callback host is not a valid host name");
    if (typeof path !== "string" || path === "") throw configError("callback path is required");
    callbackUrl = "https://" + host + (path.startsWith("/") ? path : "/" + path);
    returnBase = callbackUrl;
  } else if (callback.kind === "scheme") {
    const { scheme, via } = callback;
    if (typeof scheme !== "string" || !/^[A-Za-z][A-Za-z0-9+.-]*$/.test(scheme) || ["http", "https"].includes(scheme.toLowerCase())) {
      throw configError("callback scheme '" + String(scheme) + "' is not a valid custom scheme");
    }
    if (typeof via !== "string" || !/^https:\/\/[^/?#\s]+/.test(via)) throw configError("the scheme callback's via URL must be an absolute https URL");
    callbackUrl = scheme + "://" + SCHEME_HOST;
    returnBase = via;
  } else {
    throw configError("callback must come from httpsCallback or schemeCallback");
  }

  const state = options.state === undefined ? randomState(options.randomBytes) : options.state;
  if (typeof state !== "string" || state === "") throw configError("state must not be empty");

  const returnTo = returnBase + (returnBase.includes("?") ? "&" : "?") + STATE_ITEM + "=" + encodeURIComponent(state);
  const startBase = withoutParam(start, "returnTo");
  const hashAt = startBase.indexOf("#");
  const hash = hashAt === -1 ? "" : startBase.slice(hashAt);
  const noHash = hashAt === -1 ? startBase : startBase.slice(0, hashAt);
  const startUrl = noHash + (noHash.includes("?") ? "&" : "?") + "returnTo=" + encodeURIComponent(returnTo) + hash;

  /**
   * @summary    Check a callback URL and return its query items minus the state.
   * @param {string} url
   * @returns {Record<string, string>}
   * @throws {AnointedSignInError} missing_state, state_mismatch or server_error
   */
  function parseCallback(url) {
    if (typeof url !== "string") throw new AnointedSignInError(ERROR_CODES.PRESENTATION, "no callback URL");
    const values = parseQuery(url);
    if (!(STATE_ITEM in values)) throw new AnointedSignInError(ERROR_CODES.MISSING_STATE, "the callback carried no state, so it is not ours");
    if (!constantTimeEqual(values[STATE_ITEM], state)) throw new AnointedSignInError(ERROR_CODES.STATE_MISMATCH, "the callback state does not match this sign-in");
    delete values[STATE_ITEM];
    if ("error" in values) throw new AnointedSignInError(ERROR_CODES.SERVER_ERROR, values.error);
    return values;
  }

  return { startUrl, callbackUrl, returnTo, state, parseCallback };
}

/**
 * @summary    Compare two strings without bailing at the first difference.
 * @param {string} a
 * @param {string} b
 * @returns {boolean}
 */
function constantTimeEqual(a, b) {
  let diff = a.length ^ b.length;
  const length = Math.max(a.length, b.length);
  for (let i = 0; i < length; i++) diff |= (a.charCodeAt(i) || 0) ^ (b.charCodeAt(i) || 0);
  return diff === 0;
}
