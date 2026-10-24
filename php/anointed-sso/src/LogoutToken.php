<?php
// Copyright © Anointed Automation Corporation., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

declare(strict_types=1);

namespace AnointedAutomation\Sso;

/** A verified back-channel logout token: end every session for `$sub` (or `$sid`). */
final class LogoutToken
{
    /** @param array<string, mixed> $claims */
    public function __construct(
        public readonly ?string $sub,
        public readonly ?string $sid,
        public readonly string $jti,
        public readonly ?int $exp,
        public readonly array $claims,
    ) {
    }
}
