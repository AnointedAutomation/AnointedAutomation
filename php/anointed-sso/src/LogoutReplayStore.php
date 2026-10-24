<?php
// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

declare(strict_types=1);

namespace AnointedAutomation\Sso;

/** Remembers logout token ids (`jti`) so a replayed token is answered but not acted on twice. */
interface LogoutReplayStore
{
    /** Atomically record the jti. Returns true the first time, false when it was already recorded. */
    public function markSeen(string $jti, int $ttlSeconds): bool;

    /** Forget the jti (called when handling failed, so the provider's retry is processed). */
    public function forget(string $jti): void;
}
