<?php
// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

declare(strict_types=1);

namespace AnointedAutomation\Sso;

/** Per-process cache. Fine for tests and long-running workers; plain PHP-FPM should pass a shared cache. */
final class ArrayMetadataCache implements MetadataCache
{
    /** @var array<string, array{0: int, 1: array<string, mixed>}> */
    private array $items = [];

    public function get(string $key): ?array
    {
        if (!isset($this->items[$key]) || $this->items[$key][0] <= time()) {
            unset($this->items[$key]);
            return null;
        }
        return $this->items[$key][1];
    }

    public function set(string $key, array $value, int $ttlSeconds): void
    {
        $this->items[$key] = [time() + $ttlSeconds, $value];
    }
}
