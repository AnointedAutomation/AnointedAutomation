// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

/**
 * Every failure the kit raises. `code` is stable and safe to branch on; `message` is for logs only and
 * must never be shown to an end user verbatim.
 *
 * Codes:
 * - `config`               the client was configured wrong (thrown at construction or first use)
 * - `discovery`            the discovery document could not be read or named a different issuer
 * - `flow_missing`         the callback arrived without the short-lived flow cookie
 * - `flow_expired`         the flow cookie is older than `flowTtlSeconds`
 * - `state_mismatch`       the `state` query value does not match the flow cookie
 * - `access_denied`        the user declined on the consent screen
 * - `provider_error`       the provider returned another `error=` on the callback
 * - `issuer_mismatch`      the callback `iss` parameter names a different issuer (RFC 9207)
 * - `missing_code`         the callback has no `code`
 * - `token_exchange`       the token endpoint refused the request (see `oauthError`)
 * - `id_token_invalid`     the ID token failed signature or claim validation
 * - `email_not_verified`   a verified email was required but not present
 * - `logout_token_invalid` a back-channel Logout Token failed validation
 * - `http`                 any other non-2xx response from the provider
 */
export class AnointedSsoError extends Error {
  /**
   * @summary    Create a coded error.
   * @param {string} code     One of the codes listed on the class.
   * @param {string} message  Log-only detail.
   * @param {{status?: number, oauthError?: string, cause?: unknown}} [details]
   */
  constructor(code, message, details = {}) {
    super(message, details.cause === undefined ? undefined : { cause: details.cause });
    this.name = "AnointedSsoError";
    this.code = code;
    if (details.status !== undefined) this.status = details.status;
    if (details.oauthError !== undefined) this.oauthError = details.oauthError;
  }
}
