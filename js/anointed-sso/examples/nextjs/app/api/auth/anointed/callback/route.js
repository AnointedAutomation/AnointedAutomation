// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

import { anointedHandlers } from "../../../../../lib/anointed.js";

export const dynamic = "force-dynamic";
export const runtime = "nodejs";

/**
 * @summary    GET /api/auth/anointed/callback: the registered redirect URI.
 * @param {Request} request
 * @returns {Promise<Response>}
 */
export function GET(request) {
  return anointedHandlers().callback(request);
}
