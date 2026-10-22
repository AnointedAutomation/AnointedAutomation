// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Public entry point of @anointedautomation/sso-next. Server only: import it from route handlers, never from
// a client component. All OpenID Connect work is done by @anointedautomation/sso; this package only shapes it
// for the Next.js App Router.

export { createAnointedRouteHandlers, redirectTo } from "./routeHandlers.js";
