// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Type definitions for @anointedautomation/sso-nuxt.

import type { AnointedClient, CallbackResult, LogoutNotice } from "@anointedautomation/sso";

/** An h3 event (Nuxt 3 and 4 server routes). Typed loosely so no h3 import is needed here. */
export type NuxtServerEvent = object;

export interface HandlerContext {
  /** The same client you passed in, for `takeNewUserPassword`, `refresh`, `revoke`. */
  client: AnointedClient;
  /** The h3 event, for h3 helpers such as `setCookie` or `useSession`. */
  event: NuxtServerEvent;
}

export interface SignOutOutcome {
  /** Full `Set-Cookie` values that clear YOUR session. */
  setCookies?: string[];
  /** The ID token from sign-in, if you kept it. Lets the provider skip its confirmation page. */
  idTokenHint?: string;
}

export interface AnointedEventHandlerOptions {
  /** From `createAnointedClient` in @anointedautomation/sso. */
  client: AnointedClient;
  /** Create YOUR session from `result.user.sub` and return a Response (see `redirectTo`). */
  onSignedIn: (result: CallbackResult, request: Request, context: HandlerContext) => Response | Promise<Response>;
  /** Default: a plain 400. Branch on `error.code`; never echo `error.message` to users. */
  onError?: (error: unknown, request: Request) => Response | Promise<Response>;
  /** Needed for `backchannelLogout`. End every session for `logout.sub` / `logout.sid`. */
  onLogout?: (logout: LogoutNotice, request: Request) => void | Promise<void>;
  /** Needed for `logout`. End YOUR session and say which cookies to clear. */
  onSignOut?: (request: Request, context: HandlerContext) => SignOutOutcome | Promise<SignOutOutcome>;
  /** https URL to come back to after the provider signs out. Must be registered with the app. */
  postLogoutRedirectUri?: string;
  /** Query parameter `start` reads the return target from. Default "returnTo". */
  returnToParam?: string;
}

export interface AnointedEventHandlers {
  /** `export default defineEventHandler(anointed.start)` in `server/api/auth/anointed/start.get.js`. */
  start: (event: NuxtServerEvent) => Promise<Response>;
  /** `server/api/auth/anointed/callback.get.js`. */
  callback: (event: NuxtServerEvent) => Promise<Response>;
  /** `server/api/auth/anointed/logout.js`. Throws a `config` error when `onSignOut` was not supplied. */
  logout: (event: NuxtServerEvent) => Promise<Response>;
  /** `server/api/auth/anointed/backchannel-logout.post.js`. Throws a `config` error when `onLogout` was not supplied. */
  backchannelLogout: (event: NuxtServerEvent) => Promise<Response>;
}

export declare function createAnointedEventHandlers(options: AnointedEventHandlerOptions): AnointedEventHandlers;
export declare function redirectTo(request: Request, target: string, setCookies?: string[]): Response;
