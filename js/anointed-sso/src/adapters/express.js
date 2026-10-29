// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Express (and plain node:http) route handlers. Uses only the node:http request/response API, so it needs
// no cookie-parser or body-parser and also works with Connect-style frameworks.

import { AnointedSsoError } from "../errors.js";
import { parseCookieHeader } from "../cookies.js";
import { defaultFailureStatus, describeError, levelOf, requireOnSignedIn, resolveLogger, validateCommonOptions } from "./options.js";

const MAX_LOGOUT_BODY_BYTES = 64 * 1024;

/**
 * @summary    Append one Set-Cookie value without dropping ones already set.
 * @param {import("node:http").ServerResponse} res
 * @param {string} value
 * @returns {void}
 */
function appendSetCookie(res, value) {
  const existing = res.getHeader("set-cookie");
  if (existing === undefined) {
    res.setHeader("set-cookie", [value]);
  } else {
    res.setHeader("set-cookie", (Array.isArray(existing) ? existing : [String(existing)]).concat(value));
  }
}

/**
 * @summary    The request's query string as URLSearchParams (Express sets originalUrl; node:http only url).
 * @param {import("node:http").IncomingMessage & {originalUrl?: string}} req
 * @returns {URLSearchParams}
 */
function requestQuery(req) {
  const path = typeof req.originalUrl === "string" ? req.originalUrl : req.url;
  return new URL(String(path), "http://request.invalid").searchParams;
}

/**
 * @summary    Send a short no-store response.
 * @param {import("node:http").ServerResponse} res
 * @param {number} status
 * @param {string} [text]
 * @returns {void}
 */
function sendStatus(res, status, text) {
  res.statusCode = status;
  res.setHeader("cache-control", "no-store");
  if (text === undefined) {
    res.end();
    return;
  }
  res.setHeader("content-type", "text/plain; charset=utf-8");
  res.end(text);
}

/**
 * @summary    Read a urlencoded `logout_token` from an already parsed body or from the raw stream.
 * @param {import("node:http").IncomingMessage & {body?: unknown}} req
 * @returns {Promise<string | null>}
 */
async function readLogoutToken(req) {
  if (typeof req.body === "object" && req.body !== null && typeof req.body.logout_token === "string") {
    return req.body.logout_token;
  }
  if (typeof req.body === "string") return new URLSearchParams(req.body).get("logout_token");
  const chunks = [];
  let size = 0;
  for await (const chunk of req) {
    const bytes = typeof chunk === "string" ? new TextEncoder().encode(chunk) : chunk;
    size += bytes.length;
    if (size > MAX_LOGOUT_BODY_BYTES) return null;
    chunks.push(bytes);
  }
  const body = new Uint8Array(size);
  let offset = 0;
  for (const chunk of chunks) {
    body.set(chunk, offset);
    offset += chunk.length;
  }
  return new URLSearchParams(new TextDecoder().decode(body)).get("logout_token");
}

/**
 * @summary    Build start / callback / back-channel logout handlers for Express or node:http.
 * @param {import("../client.js").AnointedClient} client
 * @param {object} [options]
 * @param {(result: Awaited<ReturnType<import("../client.js").AnointedClient["handleCallback"]>>, req: any, res: any) => void | Promise<void>} [options.onSignedIn]
 *   Required by `callback` only. Create YOUR session from `result.user.sub` and send the response (usually a redirect to `result.returnTo`).
 * @param {(error: unknown, req: any, res: any) => void | Promise<void>} [options.onError]
 *   Default: a plain 400 (502 when the provider failed). Branch on `error.code`; never echo `error.message` to users.
 * @param {(level: "warn" | "error", message: string, details: Record<string, unknown>) => void} [options.logger]
 *   Receives every failure (before `onError` runs): "warn" for user-caused codes, "error" for the rest. Default: the console.
 * @param {(logout: Awaited<ReturnType<import("../client.js").AnointedClient["verifyLogoutToken"]>>, req: any) => void | Promise<void>} [options.onLogout]
 *   Enables `backchannelLogout`: end every session for `logout.sub` / `logout.sid`, skip a repeated `jti`.
 * @param {string} [options.returnToParam]  Query parameter `start` reads the return target from. Default `returnTo`.
 * @returns {{start: (req: any, res: any) => Promise<void>, callback: (req: any, res: any) => Promise<void>, backchannelLogout: ((req: any, res: any) => Promise<void>) | undefined}}
 */
export function createExpressHandlers(client, options = {}) {
  const returnToParam = validateCommonOptions(options);
  const logger = resolveLogger(options);

  /**
   * @summary    Record a failure, then route it to `onError` or answer a generic 400 (502 when the provider failed).
   * @param {string} step
   * @param {unknown} err
   * @param {any} req
   * @param {any} res
   * @returns {Promise<void>}
   */
  async function fail(step, err, req, res) {
    logger(levelOf(err), `anointed-sso ${step} failed`, describeError(err));
    if (options.onError === undefined) {
      sendStatus(res, defaultFailureStatus(err), "Sign in with Anointed Automation failed. Please try again.");
      return;
    }
    await options.onError(err, req, res);
  }

  /**
   * @summary    GET handler: redirect to Anointed Automation and set the flow cookie.
   * @param {any} req
   * @param {any} res
   * @returns {Promise<void>}
   */
  async function start(req, res) {
    try {
      const returnTo = requestQuery(req).get(returnToParam);
      const auth = await client.createAuthorizeRequest(returnTo === null ? { request: req } : { returnTo, request: req });
      appendSetCookie(res, auth.setCookieHeader);
      res.setHeader("location", auth.url);
      sendStatus(res, 302);
    } catch (err) {
      await fail("start", err, req, res);
    }
  }

  /**
   * @summary    GET handler on the registered redirect URI: validate, exchange, hand off to `onSignedIn`.
   * @param {any} req
   * @param {any} res
   * @returns {Promise<void>}
   */
  async function callback(req, res) {
    requireOnSignedIn(options);
    appendSetCookie(res, client.clearFlowCookieHeader());
    try {
      const cookieValue = parseCookieHeader(req.headers.cookie)[client.cookieName];
      const result = await client.handleCallback({ query: requestQuery(req), cookieValue, request: req });
      await options.onSignedIn(result, req, res);
    } catch (err) {
      await fail("callback", err, req, res);
    }
  }

  /**
   * @summary    POST handler for the registered back-channel logout URI.
   * @description 200 when processed, 400 for an invalid token, 500 when JWKS or `onLogout` failed (we retry).
   * @param {any} req
   * @param {any} res
   * @returns {Promise<void>}
   */
  async function backchannelLogout(req, res) {
    let logoutToken;
    try {
      logoutToken = await readLogoutToken(req);
    } catch {
      sendStatus(res, 400);
      return;
    }
    if (logoutToken === null || logoutToken === "") {
      sendStatus(res, 400);
      return;
    }
    let verified;
    try {
      verified = await client.verifyLogoutToken(logoutToken);
    } catch (err) {
      const status = err instanceof AnointedSsoError && err.code === "logout_token_invalid" ? 400 : 500;
      logger(levelOf(err), "anointed-sso back-channel logout verify failed", { ...describeError(err), answered: status });
      sendStatus(res, status);
      return;
    }
    try {
      await options.onLogout(verified, req);
    } catch (err) {
      logger("error", "anointed-sso onLogout handler threw", { jti: verified.jti, ...describeError(err) });
      sendStatus(res, 500);
      return;
    }
    sendStatus(res, 200);
  }

  return { start, callback, backchannelLogout: options.onLogout === undefined ? undefined : backchannelLogout };
}
