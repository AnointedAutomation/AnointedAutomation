// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Base44 backend function `anointedCallback`. Register its URL as your redirect URI:
//   https://<your-app-domain>/functions/anointedCallback
// Validates the sign-in, upserts an `AnointedUser` entity keyed on `sub`, and sets YOUR signed session cookie.

import { secrets } from "base44:runtime";
import { createClientFromRequest } from "npm:@base44/sdk";
import { ANOINTED_ISSUER, createAnointedClient, createWebHandlers } from "npm:@anointedautomation/sso@0.2.0";

const SESSION_COOKIE = "app_session";
const MAX_AGE_SECONDS = 60 * 60 * 24 * 7;

/**
 * @summary    Read a required secret or throw (no fallbacks). Must be called inside the handler.
 * @param {string} name
 * @returns {string}
 */
function requireSecret(name: string): string {
  const value = secrets.get(name);
  if (typeof value !== "string" || value.trim() === "") throw new Error(`Missing required secret ${name}`);
  return value;
}

/**
 * @summary    Unpadded base64url of bytes.
 * @param {Uint8Array} bytes
 * @returns {string}
 */
function b64url(bytes: Uint8Array): string {
  let binary = "";
  for (const b of bytes) binary += String.fromCharCode(b);
  return btoa(binary).replace(/\+/g, "-").replace(/\//g, "_").replace(/=+$/, "");
}

/**
 * @summary    An HMAC-signed session cookie for YOUR app (verified by the `anointedMe` function).
 * @param {{sub: string, email: string | null}} user
 * @param {string} secret
 * @returns {Promise<string>}
 */
async function sealSession(user: { sub: string; email: string | null }, secret: string): Promise<string> {
  const now = Math.floor(Date.now() / 1000);
  const enc = new TextEncoder();
  const payload = b64url(enc.encode(JSON.stringify({ ...user, iat: now, exp: now + MAX_AGE_SECONDS })));
  const key = await crypto.subtle.importKey("raw", enc.encode(secret), { name: "HMAC", hash: "SHA-256" }, false, ["sign"]);
  const signature = b64url(new Uint8Array(await crypto.subtle.sign("HMAC", key, enc.encode(payload))));
  return `${SESSION_COOKIE}=${payload}.${signature}; Path=/; Max-Age=${MAX_AGE_SECONDS}; HttpOnly; Secure; SameSite=Lax`;
}

/**
 * @summary    Finish the sign-in and start YOUR session.
 * @param {Request} req
 * @returns {Promise<Response>}
 */
export default async function (req: Request): Promise<Response> {
  const sessionSecret = requireSecret("SESSION_SECRET");
  const client = createAnointedClient({
    issuer: ANOINTED_ISSUER,
    clientId: requireSecret("ANOINTED_OAUTH_CLIENT_ID"),
    clientSecret: requireSecret("ANOINTED_OAUTH_CLIENT_SECRET"),
    redirectUri: requireSecret("ANOINTED_OAUTH_REDIRECT_URI"),
  });
  const handlers = createWebHandlers(client, {
    async onSignedIn(result) {
      // Direct HTTP calls carry no Base44 user, so entity writes use the service role.
      const base44 = createClientFromRequest(req);
      const existing = await base44.asServiceRole.entities.AnointedUser.filter({ sub: result.user.sub });
      if (existing.length === 0) {
        await base44.asServiceRole.entities.AnointedUser.create({ sub: result.user.sub, email: result.user.email });
      }
      const target = new URL(result.returnTo === null ? "/" : result.returnTo, req.url);
      return new Response(null, {
        status: 302,
        headers: { location: target.toString(), "set-cookie": await sealSession({ sub: result.user.sub, email: result.user.email }, sessionSecret) },
      });
    },
    onError(err) {
      const code = (err as { code?: string }).code;
      console.error("Anointed sign-in failed:", code);
      return Response.redirect(new URL(code === "access_denied" ? "/?signin=cancelled" : "/?signin=failed", req.url), 302);
    },
  });
  return handlers.callback(req);
}
