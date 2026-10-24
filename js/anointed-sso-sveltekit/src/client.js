// Copyright © Anointed Automation Corporation., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Browser-safe entry point of @anointedautomation/sso-sveltekit: the button and the pure URL helper. No secrets,
// no server imports. Import server code from "@anointedautomation/sso-sveltekit" in $lib/server only.

export { default as AnointedSignInButton } from "./AnointedSignInButton.svelte";
export { buildStartUrl } from "./startUrl.js";
export const DEFAULT_LABEL = "Sign in with Anointed Automation";
