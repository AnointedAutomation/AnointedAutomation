// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Compact JWS (RS256 only) verification against a JWKS, plus OpenID Connect ID token claim checks.
// Pure apart from WebCrypto: the caller supplies the JWKS and the clock, so every rule is unit tested.

import { base64UrlToBytes, base64UrlToJson, utf8Encode } from "./encoding.js";
import { getWebCrypto, timingSafeEqual } from "./crypto.js";

/**
 * @summary    Whether a value is a non-null, non-array object.
 * @param {unknown} value
 * @returns {boolean}
 */
function isPlainObject(value) {
  return typeof value === "object" && value !== null && !Array.isArray(value);
}

/**
 * @summary    Split and decode a compact JWS without verifying it.
 * @param {unknown} token
 * @returns {{header: Record<string, any>, payload: Record<string, any>, signingInput: string, signature: string}}
 * @throws {Error} When the token is not three non-empty base64url JSON segments.
 */
export function decodeJwt(token) {
  if (typeof token !== "string") throw new Error("token is not a string");
  const parts = token.split(".");
  if (parts.length !== 3 || parts.some((part) => part === "")) throw new Error("token is not a compact JWS");
  let header;
  let payload;
  try {
    header = base64UrlToJson(parts[0]);
    payload = base64UrlToJson(parts[1]);
  } catch {
    throw new Error("token segments are not base64url JSON");
  }
  if (!isPlainObject(header) || !isPlainObject(payload)) throw new Error("token header or payload is not an object");
  return { header, payload, signingInput: `${parts[0]}.${parts[1]}`, signature: parts[2] };
}

/**
 * @summary    Pick the RSA signing key for a JWS header from a JWKS.
 * @description
 *   Only RSA keys meant for signatures (no `use`, or `use: "sig"`) and RS256 (no `alg`, or `alg: "RS256"`)
 *   are candidates. A header `kid` must match exactly; a header without `kid` is accepted only when the
 *   JWKS holds exactly one candidate.
 * @param {unknown} jwks
 * @param {Record<string, any>} header
 * @returns {Record<string, any> | null}
 */
export function selectJwk(jwks, header) {
  const keys = isPlainObject(jwks) && Array.isArray(jwks.keys) ? jwks.keys : [];
  const candidates = keys.filter(
    (key) =>
      isPlainObject(key) &&
      key.kty === "RSA" &&
      (key.use === undefined || key.use === "sig") &&
      (key.alg === undefined || key.alg === "RS256"),
  );
  if (typeof header.kid === "string") {
    const match = candidates.find((key) => key.kid === header.kid);
    return match === undefined ? null : match;
  }
  return candidates.length === 1 ? candidates[0] : null;
}

/**
 * @summary    Verify an RS256 compact JWS against a JWKS.
 * @param {string} token
 * @param {unknown} jwks
 * @returns {Promise<{header: Record<string, any>, payload: Record<string, any>}>}
 * @throws {Error} On a malformed token, a non-RS256 alg, an unknown key (`err.unknownKid === true`) or a bad signature.
 */
export async function verifyJwsRs256(token, jwks) {
  const decoded = decodeJwt(token);
  if (decoded.header.alg !== "RS256") throw new Error(`alg ${String(decoded.header.alg)} is not allowed`);
  const jwk = selectJwk(jwks, decoded.header);
  if (jwk === null) {
    const err = new Error(`kid ${String(decoded.header.kid)} is not in the JWKS`);
    err.unknownKid = true;
    throw err;
  }
  let signature;
  try {
    signature = base64UrlToBytes(decoded.signature);
  } catch {
    throw new Error("signature is not base64url");
  }
  const cryptoImpl = await getWebCrypto();
  const key = await cryptoImpl.subtle.importKey(
    "jwk",
    { kty: "RSA", n: jwk.n, e: jwk.e, alg: "RS256", ext: true },
    { name: "RSASSA-PKCS1-v1_5", hash: "SHA-256" },
    false,
    ["verify"],
  );
  const valid = await cryptoImpl.subtle.verify("RSASSA-PKCS1-v1_5", key, signature, utf8Encode(decoded.signingInput));
  if (!valid) throw new Error("signature is invalid");
  return { header: decoded.header, payload: decoded.payload };
}

/**
 * @summary    Check `aud` contains the client and `azp`, when needed or present, names it.
 * @param {Record<string, any>} claims
 * @param {string} clientId
 * @returns {void}
 * @throws {Error}
 */
export function assertAudience(claims, clientId) {
  const audiences = Array.isArray(claims.aud) ? claims.aud : [claims.aud];
  if (!audiences.includes(clientId)) throw new Error("aud does not include the client_id");
  if (audiences.length > 1 && claims.azp !== clientId) throw new Error("azp does not name the client_id");
  if (claims.azp !== undefined && claims.azp !== clientId) throw new Error("azp does not name the client_id");
}

/**
 * @summary    Validate OpenID Connect ID token claims (signature already checked).
 * @param {Record<string, any>} claims
 * @param {object} opts
 * @param {string} opts.issuer           Exact expected `iss`.
 * @param {string} opts.clientId         Must be in `aud`.
 * @param {string | null} opts.nonce     Expected nonce; `null` skips the check (refresh responses carry no nonce).
 * @param {number} opts.now              Seconds since the epoch.
 * @param {number} opts.clockSkewSeconds Allowed skew for `exp` and `iat`.
 * @returns {void}
 * @throws {Error} Naming the first rule that failed.
 */
export function validateIdTokenClaims(claims, { issuer, clientId, nonce, now, clockSkewSeconds }) {
  if (claims.iss !== issuer) throw new Error(`iss ${String(claims.iss)} does not match ${issuer}`);
  assertAudience(claims, clientId);
  if (typeof claims.exp !== "number") throw new Error("exp is missing");
  if (now > claims.exp + clockSkewSeconds) throw new Error("token is expired");
  if (typeof claims.iat !== "number") throw new Error("iat is missing");
  if (claims.iat - clockSkewSeconds > now) throw new Error("token was issued in the future");
  if (nonce !== null && !timingSafeEqual(claims.nonce, nonce)) throw new Error("nonce does not match");
  if (typeof claims.sub !== "string" || claims.sub === "") throw new Error("sub is missing");
}
