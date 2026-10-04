// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// YOUR app's session: a small HMAC-signed cookie (not part of the SSO kit; use any session library you like).

import "server-only";
import { requireEnv } from "./env.js";

export const SESSION_COOKIE = "app_session";
const MAX_AGE_SECONDS = 60 * 60 * 24 * 7;
// Demo only: back-channel logout marks a subject revoked in memory. Use your database in production.
const revokedAt = new Map();

/**
 * @summary    HMAC-SHA256 key from SESSION_SECRET.
 * @returns {Promise<CryptoKey>}
 */
function hmacKey() {
  return crypto.subtle.importKey("raw", new TextEncoder().encode(requireEnv("SESSION_SECRET")), { name: "HMAC", hash: "SHA-256" }, false, [
    "sign",
    "verify",
  ]);
}

/**
 * @summary    Whether the session cookie should carry Secure (https deployments).
 * @returns {boolean}
 */
function secureCookies() {
  return requireEnv("ANOINTED_OAUTH_REDIRECT_URI").startsWith("https://");
}

/**
 * @summary    Signed `Set-Cookie` value for a signed-in user.
 * @param {{sub: string, email: string | null}} user
 * @returns {Promise<string>}
 */
export async function sealSession(user) {
  const now = Math.floor(Date.now() / 1000);
  const payload = Buffer.from(JSON.stringify({ ...user, iat: now, exp: now + MAX_AGE_SECONDS })).toString("base64url");
  const signature = Buffer.from(await crypto.subtle.sign("HMAC", await hmacKey(), new TextEncoder().encode(payload))).toString("base64url");
  const secure = secureCookies() ? "; Secure" : "";
  return `${SESSION_COOKIE}=${payload}.${signature}; Path=/; Max-Age=${MAX_AGE_SECONDS}; HttpOnly; SameSite=Lax${secure}`;
}

/**
 * @summary    The signed-in user from the cookie value, or null (bad signature, expired or revoked).
 * @param {string | undefined} value
 * @returns {Promise<{sub: string, email: string | null} | null>}
 */
export async function openSession(value) {
  if (typeof value !== "string") return null;
  const [payload, signature] = value.split(".");
  if (payload === undefined || signature === undefined) return null;
  const valid = await crypto.subtle.verify("HMAC", await hmacKey(), Buffer.from(signature, "base64url"), new TextEncoder().encode(payload));
  if (!valid) return null;
  const data = JSON.parse(Buffer.from(payload, "base64url").toString("utf8"));
  if (data.exp < Math.floor(Date.now() / 1000)) return null;
  const revoked = revokedAt.get(data.sub);
  if (revoked !== undefined && data.iat <= revoked) return null;
  return { sub: data.sub, email: data.email };
}

/**
 * @summary    End every session issued so far for a subject (back-channel logout).
 * @param {string} sub
 * @returns {void}
 */
export function revokeSubject(sub) {
  revokedAt.set(sub, Math.floor(Date.now() / 1000));
}

/**
 * @summary    `Set-Cookie` value that deletes the session cookie.
 * @returns {string}
 */
export function clearSessionCookieHeader() {
  return `${SESSION_COOKIE}=; Path=/; Max-Age=0; HttpOnly; SameSite=Lax`;
}
