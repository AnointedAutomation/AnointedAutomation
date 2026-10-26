<?php
// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

declare(strict_types=1);

namespace AnointedAutomation\Sso;

/** Small cache for discovery, JWKS and session status answers. Share it across requests in production. */
interface MetadataCache
{
    /** @return array<string, mixed>|null */
    public function get(string $key): ?array;

    /** @param array<string, mixed> $value */
    public function set(string $key, array $value, int $ttlSeconds): void;
}
