// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Nuxt (Nitro / h3) server route handlers on top of @anointedautomation/sso's createWebHandlers.

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
 * @summary    The web `Request` behind an h3 event. h3 v2 events carry it as `event.req`; h3 v1 (Nuxt 3, Nuxt 4)
 *             builds it with `toWebRequest`.
 * @param {any} event
 * @returns {Promise<Request>}
 * @throws {AnointedSsoError} `config` when `event` is not an h3 event
 */
async function toRequest(event) {
  if (typeof event !== "object" || event === null) throw new AnointedSsoError("config", "handlers must be called with an h3 event");
  if (event.req instanceof Request) return event.req;
  const h3 = await import("h3");
  if (typeof h3.toWebRequest !== "function") throw new AnointedSsoError("config", "h3 with toWebRequest (>= 1.8) is required");
  return h3.toWebRequest(event);
}

/**
 * @summary    Validate the options object of `createAnointedEventHandlers` (fail fast, no defaults).
 * @param {unknown} options
 * @returns {void}
 * @throws {AnointedSsoError} `config`
 */
function validateOptions(options) {
  if (typeof options !== "object" || options === null) throw new AnointedSsoError("config", "options must be an object");
  const { client, onSignedIn, onSignOut, postLogoutRedirectUri } = options;
  if (typeof client !== "object" || client === null || typeof client.createAuthorizeRequest !== "function" || typeof client.handleCallback !== "function") {
    throw new AnointedSsoError("config", "client must be the result of createAnointedClient()");
  }
  if (typeof onSignedIn !== "function") throw new AnointedSsoError("config", "onSignedIn is required");
  if (onSignOut !== undefined && typeof onSignOut !== "function") throw new AnointedSsoError("config", "onSignOut must be a function");
  if (postLogoutRedirectUri !== undefined && (typeof postLogoutRedirectUri !== "string" || postLogoutRedirectUri === "")) {
    throw new AnointedSsoError("config", "postLogoutRedirectUri must be a non-empty string");
  }
}

/**
 * @summary    The start, callback, logout and back-channel logout handlers for `server/api/auth/anointed/<route>.js`.
 * @param {object} options
 * @param {import("@anointedautomation/sso").AnointedClient} options.client  From `createAnointedClient`.
 * @param {(result: object, request: Request, context: {client: import("@anointedautomation/sso").AnointedClient, event: any}) => Response | Promise<Response>} options.onSignedIn
 *   Required. Find or create YOUR user by `result.user.sub`, start YOUR session, return a Response (see `redirectTo`).
 *   On a first sign-in call `context.client.takeNewUserPassword(result.user.sub)` and store a hash of it.
 * @param {(error: unknown, request: Request) => Response | Promise<Response>} [options.onError]
 *   Default: a plain 400. Branch on `error.code`; never echo `error.message` to users.
 * @param {(logout: object, request: Request) => void | Promise<void>} [options.onLogout]
 *   Needed for `backchannelLogout`. End every session for `logout.sub` / `logout.sid`; skip a repeated `jti`.
 * @param {(request: Request, context: {client: import("@anointedautomation/sso").AnointedClient, event: any}) => {setCookies?: string[], idTokenHint?: string} | Promise<{setCookies?: string[], idTokenHint?: string}>} [options.onSignOut]
 *   Needed for `logout`. End YOUR session and return the `Set-Cookie` values that clear it, plus the ID token
 *   (if you kept it) as `idTokenHint`. The browser is then sent to Anointed Automation's end-session endpoint.
 * @param {string} [options.postLogoutRedirectUri]  https URL to come back to after the provider signs out. Register it first.
 * @param {string} [options.returnToParam]  Query parameter `start` reads. Default `returnTo`.
 * @returns {{start: (event: any) => Promise<Response>, callback: (event: any) => Promise<Response>, logout: (event: any) => Promise<Response>, backchannelLogout: (event: any) => Promise<Response>}}
 * @throws {AnointedSsoError} `config` on a missing or invalid option
 */
export function createAnointedEventHandlers(options) {
  validateOptions(options);
  const { client, onSignedIn, onError, onLogout, onSignOut, postLogoutRedirectUri, returnToParam } = options;
  // The event reaches onSignedIn through a per-request holder keyed by the Request object.
  const events = new WeakMap();
  const webOptions = { onSignedIn: (result, request) => onSignedIn(result, request, { client, event: events.get(request) }) };
  if (onError !== undefined) webOptions.onError = onError;
  if (onLogout !== undefined) webOptions.onLogout = onLogout;
  if (returnToParam !== undefined) webOptions.returnToParam = returnToParam;
  const handlers = createWebHandlers(client, webOptions);

  /**
   * @summary    Wrap a Request handler as an event handler.
   * @param {(request: Request) => Promise<Response>} handler
   * @returns {(event: any) => Promise<Response>}
   */
  function forEvent(handler) {
    return async (event) => {
      const request = await toRequest(event);
      events.set(request, event);
      return handler(request);
    };
  }

  /**
   * @summary    Back-channel logout POST handler for the registered `backchannel_logout_uri`.
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

  /**
   * @summary    Sign the user out of YOUR app, then send the browser to Anointed Automation's end-session endpoint.
   * @param {Request} request
   * @returns {Promise<Response>}
   * @throws {AnointedSsoError} `config` when `onSignOut` was not supplied
   */
  async function logout(request) {
    if (onSignOut === undefined) throw new AnointedSsoError("config", "onSignOut is required for the logout route");
    try {
      const outcome = await onSignOut(request, { client, event: events.get(request) });
      if (typeof outcome !== "object" || outcome === null) throw new AnointedSsoError("config", "onSignOut must return an object");
      const logoutOptions = {};
      if (outcome.idTokenHint !== undefined) logoutOptions.idTokenHint = outcome.idTokenHint;
      if (postLogoutRedirectUri !== undefined) logoutOptions.postLogoutRedirectUri = postLogoutRedirectUri;
      const target = await client.buildLogoutUrl(logoutOptions);
      return redirectTo(request, target, outcome.setCookies === undefined ? [] : outcome.setCookies);
    } catch (error) {
      if (onError === undefined) {
        return new Response("Sign out failed. Please try again.", { status: 400, headers: { "content-type": "text/plain; charset=utf-8", "cache-control": "no-store" } });
      }
      return onError(error, request);
    }
  }

  return {
    start: forEvent(handlers.start),
    callback: forEvent(handlers.callback),
    logout: forEvent(logout),
    backchannelLogout: forEvent(backchannelLogout),
  };
}
