// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

import { clearSessionCookieHeader } from "../../../../lib/session.js";

export const dynamic = "force-dynamic";
export const runtime = "nodejs";

/**
 * @summary    POST /api/auth/logout: end YOUR app session.
 * @param {Request} request
 * @returns {Response}
 */
export function POST(request) {
  return new Response(null, {
    status: 303,
    headers: { location: new URL("/", request.url).toString(), "set-cookie": clearSessionCookieHeader() },
  });
}
