// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Base44 backend function `anointedMe`: your React frontend calls
//   fetch("/functions/anointedMe", { credentials: "include" })
// and gets `{ user: { sub, email } }` or `{ user: null }` from YOUR signed session cookie.

import { secrets } from "base44:runtime";
import { parseCookieHeader } from "npm:@anointedautomation/sso@0.2.0";

const SESSION_COOKIE = "app_session";

/**
 * @summary    Decode unpadded base64url to bytes.
 * @param {string} text
 * @returns {Uint8Array}
 */
function fromB64url(text: string): Uint8Array {
  const padded = text.replace(/-/g, "+").replace(/_/g, "/") + "=".repeat((4 - (text.length % 4)) % 4);
  return Uint8Array.from(atob(padded), (c) => c.charCodeAt(0));
}

/**
 * @summary    Return the signed-in user, or null.
 * @param {Request} req
 * @returns {Promise<Response>}
 */
export default async function (req: Request): Promise<Response> {
  const secret = secrets.get("SESSION_SECRET");
  if (typeof secret !== "string" || secret.trim() === "") throw new Error("Missing required secret SESSION_SECRET");
  const noStore = { "cache-control": "no-store" };
  const value = parseCookieHeader(req.headers.get("cookie"))[SESSION_COOKIE];
  if (value === undefined) return Response.json({ user: null }, { headers: noStore });
  const [payload, signature] = value.split(".");
  if (payload === undefined || signature === undefined) return Response.json({ user: null }, { headers: noStore });
  try {
    const enc = new TextEncoder();
    const key = await crypto.subtle.importKey("raw", enc.encode(secret), { name: "HMAC", hash: "SHA-256" }, false, ["verify"]);
    const valid = await crypto.subtle.verify("HMAC", key, fromB64url(signature), enc.encode(payload));
    if (!valid) return Response.json({ user: null }, { headers: noStore });
    const data = JSON.parse(new TextDecoder().decode(fromB64url(payload)));
    if (data.exp < Math.floor(Date.now() / 1000)) return Response.json({ user: null }, { headers: noStore });
    return Response.json({ user: { sub: data.sub, email: data.email } }, { headers: noStore });
  } catch {
    return Response.json({ user: null }, { headers: noStore });
  }
}
