// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Public entry point of @anointedautomation/sso. Server only.

export {
  ANOINTED_ISSUER,
  BACKCHANNEL_LOGOUT_EVENT,
  DEFAULT_SCOPE,
  createAnointedClient,
  safeReturnTo,
} from "./client.js";
export { AnointedSsoError } from "./errors.js";
export { parseCookieHeader, serializeCookie } from "./cookies.js";
export { createWebHandlers } from "./adapters/web.js";
export { createExpressHandlers } from "./adapters/express.js";
