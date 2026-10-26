// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Public entry point of @anointedautomation/sso-sveltekit. Server only: import it from hooks.server.js, +server.js
// or $lib/server, never from client code. All OpenID Connect work is done by @anointedautomation/sso; this
// package only shapes it for SvelteKit. The button lives in "@anointedautomation/sso-sveltekit/client".

export { createAnointedHandlers, createAnointedHandle, redirectTo } from "./handlers.js";
