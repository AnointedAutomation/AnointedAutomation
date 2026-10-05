// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Base44 backend function `anointedStart`: link your "Sign in with Anointed Automation" button to
//   https://<your-app-domain>/functions/anointedStart?returnTo=/dashboard

import { secrets } from "base44:runtime";
import { ANOINTED_ISSUER, createAnointedClient, createWebHandlers } from "npm:@anointedautomation/sso@0.1.0";

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
 * @summary    Redirect the browser to Anointed Automation with a fresh PKCE flow cookie.
 * @param {Request} req
 * @returns {Promise<Response>}
 */
export default async function (req: Request): Promise<Response> {
  const client = createAnointedClient({
    issuer: ANOINTED_ISSUER,
    clientId: requireSecret("ANOINTED_OAUTH_CLIENT_ID"),
    clientSecret: requireSecret("ANOINTED_OAUTH_CLIENT_SECRET"),
    redirectUri: requireSecret("ANOINTED_OAUTH_REDIRECT_URI"),
  });
  const handlers = createWebHandlers(client, {
    onSignedIn: () => new Response("unreachable", { status: 500 }),
    onError: (err) => {
      console.error("Anointed sign-in start failed:", (err as { code?: string }).code);
      return Response.redirect(new URL("/?signin=failed", req.url), 302);
    },
  });
  return handlers.start(req);
}
