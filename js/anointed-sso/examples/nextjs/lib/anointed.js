// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Server-only wiring: one client and one set of route handlers, built on first use so `next build` does not
// need the secrets. Importing this from a client component fails the build ("server-only").

import "server-only";
import { ANOINTED_ISSUER, createAnointedClient, createWebHandlers } from "@anointedautomation/sso";
import { requireEnv } from "./env.js";
import { clearSessionCookieHeader, revokeSubject, sealSession } from "./session.js";

let handlers = null;
const seenLogoutJti = new Map();

/**
 * @summary    The start / callback / back-channel logout handlers (created once, lazily).
 * @returns {import("@anointedautomation/sso").WebHandlers}
 */
export function anointedHandlers() {
  if (handlers !== null) return handlers;
  const client = createAnointedClient({
    issuer: ANOINTED_ISSUER,
    clientId: requireEnv("ANOINTED_OAUTH_CLIENT_ID"),
    clientSecret: requireEnv("ANOINTED_OAUTH_CLIENT_SECRET"),
    redirectUri: requireEnv("ANOINTED_OAUTH_REDIRECT_URI"),
  });
  handlers = createWebHandlers(client, {
    async onSignedIn(result, request) {
      // Look up or create YOUR user by result.user.sub here (never by email).
      const target = new URL(result.returnTo === null ? "/" : result.returnTo, request.url);
      return new Response(null, {
        status: 302,
        headers: { location: target.toString(), "set-cookie": await sealSession({ sub: result.user.sub, email: result.user.email }) },
      });
    },
    onError(err, request) {
      console.error("Anointed sign-in failed:", err.code, err.message);
      const target = new URL(err.code === "access_denied" ? "/?signin=cancelled" : "/?signin=failed", request.url);
      return new Response(null, { status: 302, headers: { location: target.toString(), "set-cookie": clearSessionCookieHeader() } });
    },
    onLogout(logout) {
      // Back-channel logout: the user disconnected this app or their account ended.
      if (seenLogoutJti.has(logout.jti)) return;
      seenLogoutJti.set(logout.jti, logout.exp);
      if (logout.sub !== null) revokeSubject(logout.sub);
    },
  });
  return handlers;
}
