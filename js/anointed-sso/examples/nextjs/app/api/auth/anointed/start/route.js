// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

import { anointedHandlers } from "../../../../../lib/anointed.js";

export const dynamic = "force-dynamic";
export const runtime = "nodejs";

/**
 * @summary    GET /api/auth/anointed/start[?returnTo=/path]: redirect to Anointed Automation.
 * @param {Request} request
 * @returns {Promise<Response>}
 */
export function GET(request) {
  return anointedHandlers().start(request);
}
