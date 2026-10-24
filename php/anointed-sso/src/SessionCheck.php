<?php
// Copyright © Anointed Automation Corporation., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

declare(strict_types=1);

namespace AnointedAutomation\Sso;

/** What Anointed Automation said about a session. Only `ended` means end it. */
final class SessionCheck
{
    public const ACTIVE = 'active';
    public const ENDED = 'ended';
    public const UNKNOWN = 'unknown';

    private function __construct(
        public readonly string $outcome,
        public readonly ?string $reason,
        public readonly ?string $error,
    ) {
    }

    public static function active(?string $reason): self
    {
        return new self(self::ACTIVE, $reason, null);
    }

    public static function ended(?string $reason): self
    {
        return new self(self::ENDED, $reason, null);
    }

    public static function unknown(string $error): self
    {
        return new self(self::UNKNOWN, null, $error);
    }

    public function shouldEndSession(): bool
    {
        return $this->outcome === self::ENDED;
    }
}
