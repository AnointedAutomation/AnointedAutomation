<?php
// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

declare(strict_types=1);

namespace AnointedAutomation\Sso;

/** The verified identity. Key your users on `$sub`, never on the email. */
final class User
{
    public function __construct(
        public readonly string $sub,
        public readonly ?string $email,
        public readonly bool $emailVerified,
        public readonly ?string $name,
        public readonly ?string $preferredUsername,
        public readonly ?string $givenName,
        public readonly ?string $familyName,
        public readonly ?string $picture,
        public readonly ?string $sid,
    ) {
    }
}
