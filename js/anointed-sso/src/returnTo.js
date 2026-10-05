// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Open-redirect safe return targets. By default only same-site absolute paths ("/account") are kept. A site
// can additionally trust absolute URLs on origins it names (`allowedReturnOrigins`, e.g. "https://*.example.com")
// or decides itself (`isAllowedReturnTo`). Even then: https only (http only for a loopback host), no
// credentials in the URL, no protocol-relative "//host", no javascript:/data: or any other scheme.

import { AnointedSsoError } from "./errors.js";

const MAX_RETURN_TO_LENGTH = 2048;
const UNSAFE_CHARS = /[\u0000- \u007f\\]/;
const WILDCARD_PLACEHOLDER = "wildcard-placeholder.";

/**
 * @summary    Whether a hostname is the local machine.
 * @param {string} hostname  As URL.hostname reports it (IPv6 in brackets).
 * @returns {boolean}
 */
export function isLoopbackHost(hostname) {
  return hostname === "localhost" || hostname === "127.0.0.1" || hostname === "[::1]";
}

/**
 * @summary    Keep a return path only when it is a same-site absolute path (blocks open redirects).
 * @param {unknown} value
 * @returns {string | null}
 */
export function safeRelativePath(value) {
  if (typeof value !== "string" || value === "" || value.length > MAX_RETURN_TO_LENGTH) return null;
  if (!value.startsWith("/") || value.startsWith("//") || value.startsWith("/\\")) return null;
  if (/[\u0000-\u001f\u007f\\]/.test(value)) return null;
  return value;
}

/**
 * @summary    Parse one `allowedReturnOrigins` entry into a matcher, or throw a `config` error.
 * @description
 *   Accepted shapes: "https://shop.example.com", "https://*.example.com" (any subdomain, at any depth, of a
 *   base with at least two labels; the bare base is NOT included, list it separately) and
 *   "http://localhost:3000" (http only for a loopback host, never with a wildcard). No path, query,
 *   fragment or credentials.
 * @param {unknown} entry
 * @returns {{protocol: string, host: string | null, suffix: string | null, port: string}}
 */
function compileOrigin(entry) {
  if (typeof entry !== "string" || entry.trim() === "") {
    throw new AnointedSsoError("config", "allowedReturnOrigins entries must be non-empty strings");
  }
  const wildcard = /^(https?):\/\/\*\.(.+)$/i.exec(entry);
  const parseable = wildcard === null ? entry : `${wildcard[1]}://${WILDCARD_PLACEHOLDER}${wildcard[2]}`;
  let url;
  try {
    url = new URL(parseable);
  } catch {
    throw new AnointedSsoError("config", `allowedReturnOrigins entry ${entry} is not a URL origin`);
  }
  if (url.username !== "" || url.password !== "" || url.pathname !== "/" || url.search !== "" || url.hash !== "") {
    throw new AnointedSsoError("config", `allowedReturnOrigins entry ${entry} must be a bare origin`);
  }
  if (url.protocol !== "https:" && !(url.protocol === "http:" && wildcard === null && isLoopbackHost(url.hostname))) {
    throw new AnointedSsoError("config", `allowedReturnOrigins entry ${entry} must use https (http only for localhost)`);
  }
  if (wildcard === null) return { protocol: url.protocol, host: url.hostname, suffix: null, port: url.port };
  const base = url.hostname.slice(WILDCARD_PLACEHOLDER.length);
  if (!base.includes(".") || base.startsWith(".") || base.includes("*")) {
    throw new AnointedSsoError("config", `allowedReturnOrigins wildcard ${entry} must name a domain like *.example.com`);
  }
  return { protocol: url.protocol, host: null, suffix: `.${base}`, port: url.port };
}

/**
 * @summary    Build a return target validator for the given options (validated now, fail fast).
 * @param {{allowedReturnOrigins?: string[], isAllowedReturnTo?: (url: URL) => boolean}} [options]
 * @returns {(value: unknown) => string | null}  A same-site path, the normalized absolute URL, or null.
 * @throws {AnointedSsoError} `config` when an option is malformed.
 */
export function createReturnToValidator(options = {}) {
  if (typeof options !== "object" || options === null) throw new AnointedSsoError("config", "returnTo options must be an object");
  const { allowedReturnOrigins, isAllowedReturnTo } = options;
  if (allowedReturnOrigins !== undefined && !Array.isArray(allowedReturnOrigins)) {
    throw new AnointedSsoError("config", "allowedReturnOrigins must be an array of origins");
  }
  if (isAllowedReturnTo !== undefined && typeof isAllowedReturnTo !== "function") {
    throw new AnointedSsoError("config", "isAllowedReturnTo must be a function");
  }
  const matchers = allowedReturnOrigins === undefined ? [] : allowedReturnOrigins.map(compileOrigin);
  const absoluteAllowed = matchers.length > 0 || isAllowedReturnTo !== undefined;

  /**
   * @summary    Whether a parsed URL's origin is on the allowlist.
   * @param {URL} url
   * @returns {boolean}
   */
  function originListed(url) {
    return matchers.some((m) => {
      if (m.protocol !== url.protocol || m.port !== url.port) return false;
      if (m.host !== null) return m.host === url.hostname;
      return url.hostname.length > m.suffix.length && url.hostname.endsWith(m.suffix);
    });
  }

  return function validateReturnTo(value) {
    const relative = safeRelativePath(value);
    if (relative !== null || !absoluteAllowed) return relative;
    if (typeof value !== "string" || value.length > MAX_RETURN_TO_LENGTH || UNSAFE_CHARS.test(value)) return null;
    // Only an explicit http(s) scheme: rejects "//host", "javascript:", "data:" and every other scheme.
    if (!/^https?:\/\/[^/]/i.test(value)) return null;
    let url;
    try {
      url = new URL(value);
    } catch {
      return null;
    }
    if (url.username !== "" || url.password !== "") return null;
    if (url.protocol !== "https:" && !(url.protocol === "http:" && isLoopbackHost(url.hostname))) return null;
    if (originListed(url)) return url.href;
    if (isAllowedReturnTo !== undefined && isAllowedReturnTo(new URL(url.href)) === true) return url.href;
    return null;
  };
}
