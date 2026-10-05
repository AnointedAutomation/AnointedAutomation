// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Minimal Cookie / Set-Cookie helpers so the adapters need no cookie-parser dependency.

const COOKIE_NAME = /^[!#$%&'*+\-.^_`|~0-9A-Za-z]+$/;

/**
 * @summary    Parse a `Cookie` request header into a name to value map.
 * @description
 *   The first occurrence of a name wins (browsers send the most specific path first). Values that are not
 *   valid percent-encoding are kept raw rather than dropped.
 * @param {string | null | undefined} header
 * @returns {Record<string, string>}
 */
export function parseCookieHeader(header) {
  const cookies = {};
  if (typeof header !== "string" || header === "") return cookies;
  for (const part of header.split(";")) {
    const eq = part.indexOf("=");
    if (eq <= 0) continue;
    const name = part.slice(0, eq).trim();
    if (name === "" || Object.prototype.hasOwnProperty.call(cookies, name)) continue;
    let value = part.slice(eq + 1).trim();
    if (value.startsWith('"') && value.endsWith('"') && value.length >= 2) value = value.slice(1, -1);
    try {
      cookies[name] = decodeURIComponent(value);
    } catch {
      cookies[name] = value;
    }
  }
  return cookies;
}

/**
 * @summary    Build a `Set-Cookie` header value.
 * @param {string} name
 * @param {string} value
 * @param {{path: string, maxAge: number, httpOnly: boolean, secure: boolean, sameSite: "Lax" | "Strict" | "None", domain?: string}} options
 * @returns {string}
 * @throws {TypeError} When the cookie name is not a valid token.
 */
export function serializeCookie(name, value, options) {
  if (!COOKIE_NAME.test(name)) throw new TypeError(`invalid cookie name ${name}`);
  const parts = [`${name}=${encodeURIComponent(value)}`, `Path=${options.path}`, `Max-Age=${Math.floor(options.maxAge)}`];
  if (options.domain !== undefined) parts.push(`Domain=${options.domain}`);
  if (options.httpOnly) parts.push("HttpOnly");
  if (options.secure) parts.push("Secure");
  parts.push(`SameSite=${options.sameSite}`);
  return parts.join("; ");
}
