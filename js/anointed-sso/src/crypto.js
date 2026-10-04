// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// WebCrypto access. Node 19+, Deno and edge runtimes expose `globalThis.crypto`; Node 18 only exposes it
// through `node:crypto`, which is imported lazily so bundlers for other runtimes never see it.

import { bytesToBase64Url, utf8Encode } from "./encoding.js";

let webCrypto = null;

/**
 * @summary    The runtime's WebCrypto implementation.
 * @returns {Promise<Crypto>}
 */
export async function getWebCrypto() {
  if (webCrypto !== null) return webCrypto;
  if (globalThis.crypto !== undefined && globalThis.crypto.subtle !== undefined) {
    webCrypto = globalThis.crypto;
    return webCrypto;
  }
  const nodeCrypto = await import("node:crypto");
  webCrypto = nodeCrypto.webcrypto;
  return webCrypto;
}

/**
 * @summary    Cryptographically random bytes as base64url.
 * @param {number} byteLength
 * @returns {Promise<string>}
 */
export async function randomBase64Url(byteLength) {
  const cryptoImpl = await getWebCrypto();
  const bytes = new Uint8Array(byteLength);
  cryptoImpl.getRandomValues(bytes);
  return bytesToBase64Url(bytes);
}

/**
 * @summary    SHA-256 of a UTF-8 string as base64url (the PKCE S256 transform).
 * @param {string} text
 * @returns {Promise<string>}
 */
export async function sha256Base64Url(text) {
  const cryptoImpl = await getWebCrypto();
  const digest = await cryptoImpl.subtle.digest("SHA-256", utf8Encode(text));
  return bytesToBase64Url(new Uint8Array(digest));
}

/**
 * @summary    Constant-time string comparison.
 * @description
 *   False for non-strings, empty strings and length mismatches (length is not secret here: state and
 *   nonce values have a fixed length). Equal-length inputs are compared without an early exit.
 * @param {unknown} a
 * @param {unknown} b
 * @returns {boolean}
 */
export function timingSafeEqual(a, b) {
  if (typeof a !== "string" || typeof b !== "string" || a === "" || b === "" || a.length !== b.length) {
    return false;
  }
  let diff = 0;
  for (let i = 0; i < a.length; i += 1) diff |= a.charCodeAt(i) ^ b.charCodeAt(i);
  return diff === 0;
}
