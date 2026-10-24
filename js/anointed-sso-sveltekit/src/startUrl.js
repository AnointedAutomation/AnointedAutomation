// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Pure helper: the URL of your server's start route, with the optional return target attached.

/**
 * @summary    Validate the `startUrl` option. Fail fast: there is no default start route.
 * @param {unknown} startUrl
 * @returns {string}
 * @throws {Error} when `startUrl` is not a non-empty string
 */
export function requireStartUrl(startUrl) {
  if (typeof startUrl !== "string" || startUrl.trim() === "") {
    throw new Error("@anointedautomation/sso-sveltekit: startUrl is required (the path of your start route, e.g. /auth/anointed/start)");
  }
  return startUrl;
}

/**
 * @summary    Build the href of the sign-in link: `startUrl` plus `?returnTo=<target>` when a target is given.
 * @description Keeps existing query parameters and any hash. The server validates `returnTo` again, so this is a convenience, not a guard.
 * @param {string} startUrl            Path or absolute URL of the server's start route.
 * @param {string} [returnTo]          Where to land after sign-in. Omit for none.
 * @param {string} [returnToParam]     Query parameter the server reads. Must match the server's `returnToParam`. Default `returnTo`.
 * @returns {string}
 */
export function buildStartUrl(startUrl, returnTo, returnToParam = "returnTo") {
  requireStartUrl(startUrl);
  if (returnTo === undefined || returnTo === null) return startUrl;
  if (typeof returnTo !== "string") throw new Error("@anointedautomation/sso-sveltekit: returnTo must be a string");
  if (typeof returnToParam !== "string" || returnToParam.trim() === "") {
    throw new Error("@anointedautomation/sso-sveltekit: returnToParam must be a non-empty string");
  }
  const hashAt = startUrl.indexOf("#");
  const base = hashAt === -1 ? startUrl : startUrl.slice(0, hashAt);
  const hash = hashAt === -1 ? "" : startUrl.slice(hashAt);
  const separator = base.includes("?") ? "&" : "?";
  return `${base}${separator}${encodeURIComponent(returnToParam)}=${encodeURIComponent(returnTo)}${hash}`;
}
