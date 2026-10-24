// Copyright © Anointed Automation Corporation., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Public entry point of @anointedautomation/sso-nuxt. Server only: import it from `server/` files, never from
// a component or page. All OpenID Connect work is done by @anointedautomation/sso; this package only shapes it
// for Nuxt 3 and 4 (Nitro / h3) server routes.

export { createAnointedEventHandlers, redirectTo } from "./eventHandlers.js";
