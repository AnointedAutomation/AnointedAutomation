<?php
// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

declare(strict_types=1);

namespace AnointedAutomation\Sso;

/** The outcome of a completed callback. */
final class SignInResult
{
    /** @param array<string, mixed> $claims Verified ID token claims */
    public function __construct(
        public readonly User $user,
        public readonly array $claims,
        public readonly Tokens $tokens,
        public readonly ?string $returnTo,
    ) {
    }
}
