<?php
// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

declare(strict_types=1);

namespace AnointedAutomation\Sso;

use RuntimeException;
use Throwable;

/**
 * Every failure the kit raises. `$errorCode` is one of: config, discovery, http, token_exchange, state_mismatch,
 * flow_missing, flow_expired, issuer_mismatch, access_denied, provider_error, missing_code, id_token_invalid,
 * email_not_verified, logout_token_invalid, redirect_uri_rejected.
 */
final class AnointedSsoException extends RuntimeException
{
    public function __construct(
        public readonly string $errorCode,
        string $message,
        public readonly ?int $status = null,
        public readonly ?string $oauthError = null,
        ?Throwable $previous = null,
    ) {
        parent::__construct($message, 0, $previous);
    }
}
