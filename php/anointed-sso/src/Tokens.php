<?php
// Copyright © Anointed Automation Corporation., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

declare(strict_types=1);

namespace AnointedAutomation\Sso;

/** Tokens from the token endpoint. Treat them as secrets. */
final class Tokens
{
    public function __construct(
        public readonly ?string $accessToken,
        public readonly ?string $idToken,
        public readonly ?string $refreshToken,
        public readonly ?string $tokenType,
        public readonly ?string $scope,
        public readonly ?int $expiresAt,
    ) {
    }
}
