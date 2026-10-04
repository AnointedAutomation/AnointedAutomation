// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Type definitions for @anointedautomation/sso. Server only.

/** `https://api.anointedautomation.net/` */
export declare const ANOINTED_ISSUER: string;
/** `openid email`. Add `profile` only if you need the user's name or username. */
export declare const DEFAULT_SCOPE: string;
/** `http://schemas.openid.net/event/backchannel-logout` */
export declare const BACKCHANNEL_LOGOUT_EVENT: string;

export type AnointedSsoErrorCode =
  | "config"
  | "discovery"
  | "flow_missing"
  | "flow_expired"
  | "state_mismatch"
  | "access_denied"
  | "provider_error"
  | "issuer_mismatch"
  | "missing_code"
  | "token_exchange"
  | "id_token_invalid"
  | "email_not_verified"
  | "logout_token_invalid"
  | "http";

export declare class AnointedSsoError extends Error {
  constructor(code: AnointedSsoErrorCode, message: string, details?: { status?: number; oauthError?: string; cause?: unknown });
  readonly name: "AnointedSsoError";
  /** Stable code to branch on. */
  readonly code: AnointedSsoErrorCode;
  /** HTTP status from the provider, when there was one. */
  readonly status?: number;
  /** OAuth `error` value, e.g. `invalid_grant`. */
  readonly oauthError?: string;
}

export interface AnointedClientConfig {
  /** `https://api.anointedautomation.net/` (use ANOINTED_ISSUER). */
  issuer: string;
  /** `aa_<your key id>`. */
  clientId: string;
  /** Server only. Never in a browser or mobile bundle. */
  clientSecret: string;
  /** The exact registered callback URL. */
  redirectUri: string;
  /** Space separated, must include `openid`. Default `openid email`. */
  scope?: string;
  /** Default: true when the scope includes `email`. */
  requireVerifiedEmail?: boolean;
  /** Default `aa_sso_flow`. */
  cookieName?: string;
  /** Default `/`. */
  cookiePath?: string;
  /** Default: host-only cookie. */
  cookieDomain?: string;
  /** Default 600. */
  flowTtlSeconds?: number;
  /** Default 60. */
  clockSkewSeconds?: number;
  /** Discovery and JWKS cache lifetime. Default 3600. */
  metadataTtlSeconds?: number;
  /** Default `globalThis.fetch`. */
  fetch?: typeof fetch;
  /** Clock in seconds since the epoch (tests). */
  now?: () => number;
}

export interface AnointedUser {
  /** Stable Anointed Automation user id. Key your users on this, never on email. */
  sub: string;
  /** Verified email only; null when absent or unverified. */
  email: string | null;
  emailVerified: boolean;
  /** Needs the `profile` scope. */
  name: string | null;
  /** Needs the `profile` scope. */
  preferredUsername: string | null;
  /** Needs the `profile` scope. */
  givenName: string | null;
  /** Needs the `profile` scope. */
  familyName: string | null;
  /** Needs the `profile` scope. */
  picture: string | null;
  /** The grant id that back-channel logout names. */
  sid: string | null;
}

export interface AnointedTokens {
  accessToken: string | null;
  idToken: string | null;
  /** Present with the `offline_access` scope. */
  refreshToken: string | null;
  tokenType: string | null;
  scope: string | null;
  /** Seconds since the epoch. */
  expiresAt: number | null;
}

export interface CookieOptions {
  path: string;
  maxAge: number;
  httpOnly: boolean;
  secure: boolean;
  sameSite: "Lax" | "Strict" | "None";
  domain?: string;
}

export interface AuthorizeRequest {
  /** Redirect the browser here. */
  url: string;
  state: string;
  /** Set this cookie on the redirect response. */
  cookie: { name: string; value: string; options: CookieOptions };
  /** The same cookie as a ready `Set-Cookie` header value. */
  setCookieHeader: string;
}

export interface CallbackResult {
  user: AnointedUser;
  claims: Record<string, unknown>;
  tokens: AnointedTokens;
  /** The validated same-site path passed to `createAuthorizeRequest`, or null. */
  returnTo: string | null;
}

export interface LogoutNotice {
  sub: string | null;
  sid: string | null;
  /** Remember it until `exp` and ignore repeats. */
  jti: string;
  exp: number | null;
  claims: Record<string, unknown>;
}

export interface FlowCookie {
  state: string;
  nonce: string;
  verifier: string;
  createdAt: number;
  returnTo: string | null;
}

export interface AnointedClient {
  readonly issuer: string;
  readonly clientId: string;
  readonly redirectUri: string;
  readonly scope: string;
  readonly cookieName: string;
  discover(opts?: { force?: boolean }): Promise<Record<string, unknown>>;
  getJwks(opts?: { force?: boolean }): Promise<{ keys: Array<Record<string, unknown>> }>;
  createAuthorizeRequest(opts?: {
    returnTo?: string;
    prompt?: string;
    loginHint?: string;
    extraParams?: Record<string, string>;
  }): Promise<AuthorizeRequest>;
  handleCallback(input: {
    url?: string;
    query?: URLSearchParams | Record<string, string>;
    cookieValue: string | undefined;
  }): Promise<CallbackResult>;
  refresh(refreshToken: string, opts?: { expectedSub?: string }): Promise<{ tokens: AnointedTokens; claims: Record<string, unknown> | null }>;
  revoke(token: string, opts?: { tokenTypeHint?: "refresh_token" | "access_token" }): Promise<void>;
  fetchUserInfo(accessToken: string): Promise<Record<string, unknown> & { sub: string }>;
  buildLogoutUrl(opts?: { idTokenHint?: string; postLogoutRedirectUri?: string; state?: string }): Promise<string>;
  verifyLogoutToken(logoutToken: string, opts?: { maxAgeSeconds?: number }): Promise<LogoutNotice>;
  decodeFlowCookie(value: unknown): FlowCookie | null;
  clearFlowCookie(): { name: string; value: string; options: CookieOptions };
  clearFlowCookieHeader(): string;
}

export declare function createAnointedClient(config: AnointedClientConfig): AnointedClient;
export declare function safeReturnTo(value: unknown): string | null;
export declare function parseCookieHeader(header: string | null | undefined): Record<string, string>;
export declare function serializeCookie(name: string, value: string, options: CookieOptions): string;

export interface WebHandlerOptions {
  /** Create YOUR session from `result.user.sub` and return the response. The flow cookie is cleared for you. */
  onSignedIn: (result: CallbackResult, request: Request) => Response | Promise<Response>;
  /** Default: a generic 400. Branch on `error.code`; never show `error.message` to users. */
  onError?: (error: unknown, request: Request) => Response | Promise<Response>;
  /** Enables `backchannelLogout`. */
  onLogout?: (logout: LogoutNotice, request: Request) => void | Promise<void>;
}

export interface WebHandlers {
  start(request: Request): Promise<Response>;
  callback(request: Request): Promise<Response>;
  backchannelLogout: ((request: Request) => Promise<Response>) | undefined;
}

/** Request -> Response handlers for Next.js App Router, Base44 / Deno, Cloudflare Workers, Bun, Hono. */
export declare function createWebHandlers(client: AnointedClient, options: WebHandlerOptions): WebHandlers;

/** Minimal shapes of the node:http / Express request and response the Express adapter uses. */
export interface NodeRequestLike {
  url?: string;
  originalUrl?: string;
  headers: { cookie?: string } & Record<string, unknown>;
  body?: unknown;
  [Symbol.asyncIterator](): AsyncIterator<Uint8Array | string>;
}

export interface NodeResponseLike {
  statusCode: number;
  getHeader(name: string): unknown;
  setHeader(name: string, value: string | string[]): unknown;
  end(chunk?: string): unknown;
}

export interface ExpressHandlerOptions<Req extends NodeRequestLike = NodeRequestLike, Res extends NodeResponseLike = NodeResponseLike> {
  /** Create YOUR session from `result.user.sub` and send the response. */
  onSignedIn: (result: CallbackResult, req: Req, res: Res) => void | Promise<void>;
  /** Default: a generic 400. Branch on `error.code`; never show `error.message` to users. */
  onError?: (error: unknown, req: Req, res: Res) => void | Promise<void>;
  /** Enables `backchannelLogout`. */
  onLogout?: (logout: LogoutNotice, req: Req) => void | Promise<void>;
}

export interface ExpressHandlers<Req extends NodeRequestLike = NodeRequestLike, Res extends NodeResponseLike = NodeResponseLike> {
  start(req: Req, res: Res): Promise<void>;
  callback(req: Req, res: Res): Promise<void>;
  backchannelLogout: ((req: Req, res: Res) => Promise<void>) | undefined;
}

/** Handlers for Express or plain node:http. No cookie-parser or body-parser needed. */
export declare function createExpressHandlers<Req extends NodeRequestLike = NodeRequestLike, Res extends NodeResponseLike = NodeResponseLike>(
  client: AnointedClient,
  options: ExpressHandlerOptions<Req, Res>,
): ExpressHandlers<Req, Res>;
