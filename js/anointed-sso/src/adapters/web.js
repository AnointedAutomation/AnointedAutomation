// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Web-standard (Request -> Response) route handlers. One adapter covers every fetch-style runtime:
// Next.js App Router route handlers, Base44 / Deno `Deno.serve`, Cloudflare Workers, Bun, Hono.

import { AnointedSsoError } from "../errors.js";
import { parseCookieHeader } from "../cookies.js";

/**
 * @summary    Copy a Response so headers are mutable, and append one Set-Cookie value.
 * @param {Response} response
 * @param {string} setCookie
 * @returns {Response}
 */
function withSetCookie(response, setCookie) {
  if (!(response instanceof Response)) throw new AnointedSsoError("config", "handlers must return a Response");
  const headers = new Headers(response.headers);
  headers.append("set-cookie", setCookie);
  return new Response(response.body, { status: response.status, statusText: response.statusText, headers });
}

/**
 * @summary    The response sent when sign-in fails and no `onError` was supplied. Never leaks the cause.
 * @returns {Response}
 */
function defaultErrorResponse() {
  return new Response("Sign in with Anointed Automation failed. Please try again.", {
    status: 400,
    headers: { "content-type": "text/plain; charset=utf-8", "cache-control": "no-store" },
  });
}

/**
 * @summary    Build start / callback / back-channel logout handlers for a fetch-style runtime.
 * @param {import("../client.js").AnointedClient} client
 * @param {object} options
 * @param {(result: Awaited<ReturnType<import("../client.js").AnointedClient["handleCallback"]>>, request: Request) => Response | Promise<Response>} options.onSignedIn
 *   Create YOUR session from `result.user.sub` and return the response (usually a redirect to
 *   `result.returnTo`). The flow cookie is cleared for you.
 * @param {(error: unknown, request: Request) => Response | Promise<Response>} [options.onError]
 *   Default: a plain 400. Branch on `error.code` (see AnointedSsoError); never echo `error.message` to users.
 * @param {(logout: Awaited<ReturnType<import("../client.js").AnointedClient["verifyLogoutToken"]>>, request: Request) => void | Promise<void>} [options.onLogout]
 *   Enables `backchannelLogout`: end every session for `logout.sub` / `logout.sid`, skip a repeated `jti`.
 * @returns {{start: (request: Request) => Promise<Response>, callback: (request: Request) => Promise<Response>, backchannelLogout: ((request: Request) => Promise<Response>) | undefined}}
 */
export function createWebHandlers(client, options) {
  if (typeof options !== "object" || options === null || typeof options.onSignedIn !== "function") {
    throw new AnointedSsoError("config", "onSignedIn is required");
  }
  if (options.onError !== undefined && typeof options.onError !== "function") {
    throw new AnointedSsoError("config", "onError must be a function");
  }
  if (options.onLogout !== undefined && typeof options.onLogout !== "function") {
    throw new AnointedSsoError("config", "onLogout must be a function");
  }
  const onError = options.onError === undefined ? defaultErrorResponse : options.onError;

  /**
   * @summary    GET handler: redirect to Anointed Automation and set the flow cookie.
   * @description `?returnTo=/path` is carried through when it is a same-site path.
   * @param {Request} request
   * @returns {Promise<Response>}
   */
  async function start(request) {
    try {
      const returnTo = new URL(request.url).searchParams.get("returnTo");
      const auth = await client.createAuthorizeRequest(returnTo === null ? {} : { returnTo });
      return new Response(null, {
        status: 302,
        headers: { location: auth.url, "set-cookie": auth.setCookieHeader, "cache-control": "no-store" },
      });
    } catch (err) {
      return onError(err, request);
    }
  }

  /**
   * @summary    GET handler on the registered redirect URI: validate, exchange, hand off to `onSignedIn`.
   * @param {Request} request
   * @returns {Promise<Response>}
   */
  async function callback(request) {
    const clear = client.clearFlowCookieHeader();
    try {
      const cookieValue = parseCookieHeader(request.headers.get("cookie"))[client.cookieName];
      const result = await client.handleCallback({ url: request.url, cookieValue });
      return withSetCookie(await options.onSignedIn(result, request), clear);
    } catch (err) {
      return withSetCookie(await onError(err, request), clear);
    }
  }

  /**
   * @summary    POST handler for the registered back-channel logout URI.
   * @description 200 when processed, 400 for an invalid token, 500 when JWKS or `onLogout` failed (we retry).
   * @param {Request} request
   * @returns {Promise<Response>}
   */
  async function backchannelLogout(request) {
    const noStore = { "cache-control": "no-store" };
    let logoutToken;
    try {
      logoutToken = new URLSearchParams(await request.text()).get("logout_token");
    } catch {
      return new Response(null, { status: 400, headers: noStore });
    }
    if (logoutToken === null || logoutToken === "") return new Response(null, { status: 400, headers: noStore });
    let verified;
    try {
      verified = await client.verifyLogoutToken(logoutToken);
    } catch (err) {
      const status = err instanceof AnointedSsoError && err.code === "logout_token_invalid" ? 400 : 500;
      return new Response(null, { status, headers: noStore });
    }
    try {
      await options.onLogout(verified, request);
    } catch {
      return new Response(null, { status: 500, headers: noStore });
    }
    return new Response(null, { status: 200, headers: noStore });
  }

  return { start, callback, backchannelLogout: options.onLogout === undefined ? undefined : backchannelLogout };
}
