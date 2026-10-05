// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// base64url and UTF-8 helpers built on web standards only (btoa/atob, TextEncoder), so the same code runs
// on Node 18+, Deno and edge runtimes without Buffer.

const encoder = new TextEncoder();
const decoder = new TextDecoder("utf-8", { fatal: true });
const BASE64URL = /^[A-Za-z0-9_-]*$/;

/**
 * @summary    Encode a string as UTF-8 bytes.
 * @param {string} text
 * @returns {Uint8Array}
 */
export function utf8Encode(text) {
  return encoder.encode(text);
}

/**
 * @summary    Decode UTF-8 bytes to a string (throws on invalid UTF-8).
 * @param {Uint8Array} bytes
 * @returns {string}
 */
export function utf8Decode(bytes) {
  return decoder.decode(bytes);
}

/**
 * @summary    Encode bytes as unpadded base64url.
 * @param {Uint8Array} bytes
 * @returns {string}
 */
export function bytesToBase64Url(bytes) {
  let binary = "";
  for (let i = 0; i < bytes.length; i += 1) binary += String.fromCharCode(bytes[i]);
  return btoa(binary).replace(/\+/g, "-").replace(/\//g, "_").replace(/=+$/, "");
}

/**
 * @summary    Decode unpadded (or padded) base64url to bytes.
 * @param {string} text
 * @returns {Uint8Array}
 * @throws {TypeError} When the input is not a string of base64url characters of a valid length.
 */
export function base64UrlToBytes(text) {
  if (typeof text !== "string") throw new TypeError("base64url input must be a string");
  const trimmed = text.replace(/=+$/, "");
  if (!BASE64URL.test(trimmed) || trimmed.length % 4 === 1) throw new TypeError("invalid base64url");
  const padded = trimmed.replace(/-/g, "+").replace(/_/g, "/") + "=".repeat((4 - (trimmed.length % 4)) % 4);
  const binary = atob(padded);
  const bytes = new Uint8Array(binary.length);
  for (let i = 0; i < binary.length; i += 1) bytes[i] = binary.charCodeAt(i);
  return bytes;
}

/**
 * @summary    JSON-serialize a value and encode it as base64url.
 * @param {unknown} value
 * @returns {string}
 */
export function jsonToBase64Url(value) {
  return bytesToBase64Url(utf8Encode(JSON.stringify(value)));
}

/**
 * @summary    Decode a base64url segment and parse it as JSON.
 * @param {string} text
 * @returns {any}
 * @throws {TypeError|SyntaxError} When the segment is not base64url-encoded JSON.
 */
export function base64UrlToJson(text) {
  return JSON.parse(utf8Decode(base64UrlToBytes(text)));
}
