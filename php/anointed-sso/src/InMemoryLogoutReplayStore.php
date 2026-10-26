<?php
// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

declare(strict_types=1);

namespace AnointedAutomation\Sso;

/** Per-process replay store. Use a shared one (database, Redis, Laravel cache) when you run several workers. */
final class InMemoryLogoutReplayStore implements LogoutReplayStore
{
    /** @var array<string, int> */
    private array $seen = [];

    public function markSeen(string $jti, int $ttlSeconds): bool
    {
        $now = time();
        foreach ($this->seen as $id => $expiresAt) {
            if ($expiresAt <= $now) {
                unset($this->seen[$id]);
            }
        }
        if (isset($this->seen[$jti])) {
            return false;
        }
        $this->seen[$jti] = $now + $ttlSeconds;
        return true;
    }

    public function forget(string $jti): void
    {
        unset($this->seen[$jti]);
    }
}
