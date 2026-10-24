// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Pure helper: the URL of your server's start route, with the optional return target attached.

const PREFIX = "@anointedautomation/sso-angular";

/**
 * Validate the startUrl option. Fail fast: there is no default start route.
 * @param startUrl The value to check.
 * @returns The same value.
 */
export function requireStartUrl(startUrl: unknown): string {
  if (typeof startUrl !== "string" || startUrl.trim() === "") {
    throw new Error(`${PREFIX}: startUrl is required (the path of your server's start route, e.g. /api/auth/anointed/start)`);
  }
  return startUrl;
}

/**
 * Build the URL the browser is sent to: startUrl plus ?returnTo=<target> when a target is given.
 * Keeps existing query parameters and any hash. The server validates returnTo again, so this is a convenience, not a guard.
 * @param startUrl Path or absolute URL of the server's start route.
 * @param returnTo Where to land after sign-in. Omit for none.
 * @param returnToParam Query parameter the server reads. Must match the server's returnToParam. Default returnTo.
 * @returns The URL to navigate to.
 */
export function buildStartUrl(startUrl: string, returnTo?: string | null, returnToParam: string = "returnTo"): string {
  requireStartUrl(startUrl);
  if (returnTo === undefined || returnTo === null) return startUrl;
  if (typeof returnTo !== "string") throw new Error(`${PREFIX}: returnTo must be a string`);
  if (typeof returnToParam !== "string" || returnToParam.trim() === "") {
    throw new Error(`${PREFIX}: returnToParam must be a non-empty string`);
  }
  const hashAt = startUrl.indexOf("#");
  const base = hashAt === -1 ? startUrl : startUrl.slice(0, hashAt);
  const hash = hashAt === -1 ? "" : startUrl.slice(hashAt);
  const separator = base.includes("?") ? "&" : "?";
  return `${base}${separator}${encodeURIComponent(returnToParam)}=${encodeURIComponent(returnTo)}${hash}`;
}
