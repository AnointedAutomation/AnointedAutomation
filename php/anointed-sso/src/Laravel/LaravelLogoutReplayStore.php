<?php
// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

declare(strict_types=1);

namespace AnointedAutomation\Sso\Laravel;

use AnointedAutomation\Sso\LogoutReplayStore;
use Illuminate\Contracts\Cache\Repository;

/** Replay protection on a Laravel cache store (use a shared store such as redis or database in production). */
final class LaravelLogoutReplayStore implements LogoutReplayStore
{
    public function __construct(private readonly Repository $store)
    {
    }

    public function markSeen(string $jti, int $ttlSeconds): bool
    {
        return $this->store->add('anointed_sso.logout_jti.' . $jti, true, $ttlSeconds);
    }

    public function forget(string $jti): void
    {
        $this->store->forget('anointed_sso.logout_jti.' . $jti);
    }
}
