// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Type definitions for @anointedautomation/sso-next.

import type { AnointedClient, CallbackResult, LogoutNotice } from "@anointedautomation/sso";

export interface OnSignedInContext {
  /** The same client you passed in, for `takeNewUserPassword`, `refresh`, `revoke`. */
  client: AnointedClient;
}

export interface AnointedRouteHandlerOptions {
  /** From `createAnointedClient` in @anointedautomation/sso. */
  client: AnointedClient;
  /** Create YOUR session from `result.user.sub` and return a Response (see `redirectTo`). */
  onSignedIn: (result: CallbackResult, request: Request, context: OnSignedInContext) => Response | Promise<Response>;
  /** Default: a plain 400. Branch on `error.code`; never echo `error.message` to users. */
  onError?: (error: unknown, request: Request) => Response | Promise<Response>;
  /** Needed for `backchannelLogout`. End every session for `logout.sub` / `logout.sid`. */
  onLogout?: (logout: LogoutNotice, request: Request) => void | Promise<void>;
  /** Query parameter `start` reads the return target from. Default "returnTo". */
  returnToParam?: string;
}

export interface AnointedRouteHandlers {
  /** `export const GET = handlers.start` in `app/api/auth/anointed/start/route.js`. */
  start: (request: Request) => Promise<Response>;
  /** `export const GET = handlers.callback` in `app/api/auth/anointed/callback/route.js`. */
  callback: (request: Request) => Promise<Response>;
  /** `export const POST = handlers.backchannelLogout`. Throws a `config` error when `onLogout` was not supplied. */
  backchannelLogout: (request: Request) => Promise<Response>;
}

export declare function createAnointedRouteHandlers(options: AnointedRouteHandlerOptions): AnointedRouteHandlers;
export declare function redirectTo(request: Request, target: string, setCookies?: string[]): Response;
