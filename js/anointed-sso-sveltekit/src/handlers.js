// Copyright © Anointed Automation Corporation., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// SvelteKit handlers on top of @anointedautomation/sso's createWebHandlers (SvelteKit speaks Web Request/Response).

import { AnointedSsoError, createWebHandlers } from "@anointedautomation/sso";

/**
 * @summary    Build a 302 redirect Response, resolving a path against the request URL.
 * @description Use it inside `onSignedIn` / `onSignOut` to send the user on and set YOUR cookies.
 *              `target` is required (no default). Prefer this over SvelteKit's `redirect()` helper here because it can carry Set-Cookie.
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
 * @summary    Validate the options object of `createAnointedHandlers` (fail fast, no defaults).
 * @param {unknown} options
 * @returns {void}
 * @throws {AnointedSsoError} `config`
 */
function validateOptions(options) {
  if (typeof options !== "object" || options === null) throw new AnointedSsoError("config", "options must be an object");
  const { client, onSignedIn, onSignOut } = options;
  if (typeof client !== "object" || client === null || typeof client.createAuthorizeRequest !== "function" || typeof client.handleCallback !== "function") {
    throw new AnointedSsoError("config", "client must be the result of createAnointedClient()");
  }
  if (typeof onSignedIn !== "function") throw new AnointedSsoError("config", "onSignedIn is required");
  if (onSignOut !== undefined && typeof onSignOut !== "function") throw new AnointedSsoError("config", "onSignOut must be a function");
}

/**
 * @summary    The start, callback, logout and back-channel logout handlers, each `(request: Request) => Promise<Response>`.
 * @param {object} options
 * @param {import("@anointedautomation/sso").AnointedClient} options.client  From `createAnointedClient`.
 * @param {(result: object, request: Request, context: {client: import("@anointedautomation/sso").AnointedClient}) => Response | Promise<Response>} options.onSignedIn
 *   Required. Find or create YOUR user by `result.user.sub`, start YOUR session, return a Response (see `redirectTo`).
 *   On a first sign-in call `context.client.takeNewUserPassword(result.user.sub)` and store a hash of it.
 * @param {(request: Request, context: {client: import("@anointedautomation/sso").AnointedClient}) => Response | Promise<Response>} [options.onSignOut]
 *   Needed for `logout`. End YOUR session (clear your cookie) and return a Response. Use a POST form for it.
 * @param {(error: unknown, request: Request) => Response | Promise<Response>} [options.onError]
 *   Default: a plain 400. Branch on `error.code`; never echo `error.message` to users.
 * @param {(logout: object, request: Request) => void | Promise<void>} [options.onLogout]
 *   Needed for `backchannelLogout`. End every session for `logout.sub` / `logout.sid`; skip a repeated `jti`.
 * @param {string} [options.returnToParam]  Query parameter `start` reads. Default `returnTo`.
 * @returns {{start: Function, callback: Function, logout: Function, backchannelLogout: Function}}
 * @throws {AnointedSsoError} `config` on a missing or invalid option
 */
export function createAnointedHandlers(options) {
  validateOptions(options);
  const { client, onSignedIn, onSignOut, onError, onLogout, returnToParam } = options;
  const webOptions = { onSignedIn: (result, request) => onSignedIn(result, request, { client }) };
  if (onError !== undefined) webOptions.onError = onError;
  if (onLogout !== undefined) webOptions.onLogout = onLogout;
  if (returnToParam !== undefined) webOptions.returnToParam = returnToParam;
  const handlers = createWebHandlers(client, webOptions);

  /**
   * @summary    Sign the user out of YOUR app (runs `onSignOut`).
   * @param {Request} request
   * @returns {Promise<Response>}
   * @throws {AnointedSsoError} `config` when `onSignOut` was not supplied
   */
  async function logout(request) {
    if (onSignOut === undefined) throw new AnointedSsoError("config", "onSignOut is required for the logout route");
    return onSignOut(request, { client });
  }

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

  return { start: handlers.start, callback: handlers.callback, logout, backchannelLogout };
}

/**
 * @summary    A SvelteKit `handle` hook that serves the four routes under `basePath`, so no +server.js files are needed.
 * @description Routes: GET `<basePath>/start`, GET `<basePath>/callback`, POST `<basePath>/logout`,
 *              POST `<basePath>/backchannel-logout`. Everything else goes to `resolve(event)`. Compose with `sequence()`.
 *              The back-channel POST comes from Anointed Automation, not a browser form, so SvelteKit's CSRF origin
 *              check must allow it (see the README).
 * @param {object} options
 * @param {ReturnType<typeof createAnointedHandlers>} options.handlers  From `createAnointedHandlers`.
 * @param {string} options.basePath  Required, absolute path such as `/auth/anointed`, no trailing slash.
 * @returns {(input: {event: {request: Request, url: URL}, resolve: (event: unknown) => Response | Promise<Response>}) => Promise<Response>}
 * @throws {AnointedSsoError} `config` on a missing or invalid option
 */
export function createAnointedHandle(options) {
  if (typeof options !== "object" || options === null) throw new AnointedSsoError("config", "options must be an object");
  const { handlers, basePath } = options;
  if (typeof handlers !== "object" || handlers === null || typeof handlers.start !== "function" || typeof handlers.backchannelLogout !== "function") {
    throw new AnointedSsoError("config", "handlers must be the result of createAnointedHandlers()");
  }
  if (typeof basePath !== "string" || !basePath.startsWith("/") || basePath.endsWith("/")) {
    throw new AnointedSsoError("config", "basePath must start with / and not end with / (for example /auth/anointed)");
  }
  const routes = new Map([
    [`GET ${basePath}/start`, handlers.start],
    [`GET ${basePath}/callback`, handlers.callback],
    [`POST ${basePath}/logout`, handlers.logout],
    [`POST ${basePath}/backchannel-logout`, handlers.backchannelLogout],
  ]);

  return async function handle({ event, resolve }) {
    const route = routes.get(`${event.request.method} ${event.url.pathname}`);
    if (route === undefined) return resolve(event);
    return route(event.request);
  };
}
