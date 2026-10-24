<?php
// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

declare(strict_types=1);

namespace AnointedAutomation\Sso\Laravel;

use AnointedAutomation\Sso\MetadataCache;
use Illuminate\Contracts\Cache\Repository;

/** Stores discovery, JWKS and session status answers in a Laravel cache store. */
final class LaravelMetadataCache implements MetadataCache
{
    public function __construct(private readonly Repository $store)
    {
    }

    public function get(string $key): ?array
    {
        $value = $this->store->get($key);
        return is_array($value) ? $value : null;
    }

    public function set(string $key, array $value, int $ttlSeconds): void
    {
        $this->store->put($key, $value, $ttlSeconds);
    }
}
