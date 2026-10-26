// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

import { anointedHandlers } from "../../../../../lib/anointed.js";

export const dynamic = "force-dynamic";
export const runtime = "nodejs";

/**
 * @summary    POST /api/auth/anointed/backchannel-logout: register this URL as your backchannel_logout_uri.
 * @param {Request} request
 * @returns {Promise<Response>}
 */
export function POST(request) {
  return anointedHandlers().backchannelLogout(request);
}
