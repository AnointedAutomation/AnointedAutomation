// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Type definitions for @anointedautomation/sso-sveltekit (server entry).

import type { AnointedClient, CallbackResult, LogoutNotice } from "@anointedautomation/sso";

export interface AnointedHandlerContext {
  /** The same client you passed in, for `takeNewUserPassword`, `refresh`, `revoke`. */
  client: AnointedClient;
}

export interface AnointedHandlerOptions {
  /** From `createAnointedClient` in @anointedautomation/sso. */
  client: AnointedClient;
  /** Create YOUR session from `result.user.sub` and return a Response (see `redirectTo`). */
  onSignedIn: (result: CallbackResult, request: Request, context: AnointedHandlerContext) => Response | Promise<Response>;
  /** Needed for `logout`. End YOUR session and return a Response. */
  onSignOut?: (request: Request, context: AnointedHandlerContext) => Response | Promise<Response>;
  /** Default: a plain 400. Branch on `error.code`; never echo `error.message` to users. */
  onError?: (error: unknown, request: Request) => Response | Promise<Response>;
  /** Needed for `backchannelLogout`. End every session for `logout.sub` / `logout.sid`. */
  onLogout?: (logout: LogoutNotice, request: Request) => void | Promise<void>;
  /** Query parameter `start` reads the return target from. Default "returnTo". */
  returnToParam?: string;
}

export interface AnointedHandlers {
  start: (request: Request) => Promise<Response>;
  callback: (request: Request) => Promise<Response>;
  /** Throws a `config` error when `onSignOut` was not supplied. */
  logout: (request: Request) => Promise<Response>;
  /** Throws a `config` error when `onLogout` was not supplied. */
  backchannelLogout: (request: Request) => Promise<Response>;
}

export interface AnointedHandleOptions {
  handlers: AnointedHandlers;
  /** Absolute path, no trailing slash, for example "/auth/anointed". */
  basePath: string;
}

/** Structural type of SvelteKit's `Handle` so this package does not import @sveltejs/kit types. */
export type AnointedHandle = (input: {
  event: { request: Request; url: URL };
  resolve: (event: any) => Response | Promise<Response>;
}) => Promise<Response>;

export declare function createAnointedHandlers(options: AnointedHandlerOptions): AnointedHandlers;
export declare function createAnointedHandle(options: AnointedHandleOptions): AnointedHandle;
export declare function redirectTo(request: Request, target: string, setCookies?: string[]): Response;
