// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Next.js App Router route handlers on top of @anointedautomation/sso's createWebHandlers.

import { AnointedSsoError, createWebHandlers } from "@anointedautomation/sso";

/**
 * @summary    Build a 302 redirect Response, resolving a path against the request URL.
 * @description Use it inside `onSignedIn` to send the user on and attach YOUR session cookie(s).
 *              `target` is required (no default): pass `result.returnTo` after choosing your own landing page.
 * @param {Request} request
 * @param {string} target                 Path or absolute URL. The SSO client already validated `returnTo`.
 * @param {string[]} [setCookies]         Full `Set-Cookie` header values to append.
 * @returns {Response}
 */
export function redirectTo(request, target, setCookies = []) {
  if (typeof target !== "string" || target === "") {
    throw new AnointedSsoError("config", "redirectTo needs a non-empty target");
  }
  const headers = new Headers({ location: new URL(target, request.url).toString(), "cache-control": "no-store" });
  for (const cookie of setCookies) headers.append("set-cookie", cookie);
  return new Response(null, { status: 302, headers });
}

/**
 * @summary    Validate the options object of `createAnointedRouteHandlers` (fail fast, no defaults).
 * @param {unknown} options
 * @returns {void}
 * @throws {AnointedSsoError} `config`
 */
function validateOptions(options) {
  if (typeof options !== "object" || options === null) throw new AnointedSsoError("config", "options must be an object");
  const { client, onSignedIn } = options;
  if (typeof client !== "object" || client === null || typeof client.createAuthorizeRequest !== "function" || typeof client.handleCallback !== "function") {
    throw new AnointedSsoError("config", "client must be the result of createAnointedClient()");
  }
  if (typeof onSignedIn !== "function") throw new AnointedSsoError("config", "onSignedIn is required");
}

/**
 * @summary    The start, callback and back-channel logout handlers for `app/api/auth/anointed/<route>/route.js`.
 * @param {object} options
 * @param {import("@anointedautomation/sso").AnointedClient} options.client  From `createAnointedClient`.
 * @param {(result: object, request: Request, context: {client: import("@anointedautomation/sso").AnointedClient}) => Response | Promise<Response>} options.onSignedIn
 *   Required. Find or create YOUR user by `result.user.sub`, start YOUR session, return a Response (see `redirectTo`).
 *   On a first sign-in call `context.client.takeNewUserPassword(result.user.sub)` and store a hash of it.
 * @param {(error: unknown, request: Request) => Response | Promise<Response>} [options.onError]
 *   Default: a plain 400. Branch on `error.code`; never echo `error.message` to users.
 * @param {(logout: object, request: Request) => void | Promise<void>} [options.onLogout]
 *   Needed for `backchannelLogout`. End every session for `logout.sub` / `logout.sid`; skip a repeated `jti`.
 * @param {string} [options.returnToParam]  Query parameter `start` reads. Default `returnTo`.
 * @returns {{start: (request: Request) => Promise<Response>, callback: (request: Request) => Promise<Response>, backchannelLogout: (request: Request) => Promise<Response>}}
 * @throws {AnointedSsoError} `config` on a missing or invalid option
 */
export function createAnointedRouteHandlers(options) {
  validateOptions(options);
  const { client, onSignedIn, onError, onLogout, returnToParam } = options;
  const webOptions = { onSignedIn: (result, request) => onSignedIn(result, request, { client }) };
  if (onError !== undefined) webOptions.onError = onError;
  if (onLogout !== undefined) webOptions.onLogout = onLogout;
  if (returnToParam !== undefined) webOptions.returnToParam = returnToParam;
  const handlers = createWebHandlers(client, webOptions);

  /**
   * @summary    POST handler for the registered back-channel logout URI.
   * @param {Request} request
   * @returns {Promise<Response>}
   * @throws {AnointedSsoError} `config` when `onLogout` was not supplied
   */
  async function backchannelLogout(request) {
    if (handlers.backchannelLogout === undefined) {
      throw new AnointedSsoError("config", "onLogout is required for the backchannelLogout route");
    }
    return handlers.backchannelLogout(request);
  }

  return { start: handlers.start, callback: handlers.callback, backchannelLogout };
}
