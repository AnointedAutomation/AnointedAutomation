<?php
// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

declare(strict_types=1);

namespace AnointedAutomation\Sso;

/** Base64url helpers (RFC 4648 section 5, no padding) and constant-time compare. */
final class Base64Url
{
    public static function encode(string $bytes): string
    {
        return rtrim(strtr(base64_encode($bytes), '+/', '-_'), '=');
    }

    public static function decode(string $value): string
    {
        if ($value === '' || preg_match('/^[A-Za-z0-9_-]+$/', $value) !== 1) {
            throw new \InvalidArgumentException('not base64url');
        }
        $decoded = base64_decode(strtr($value, '-_', '+/') . str_repeat('=', (4 - strlen($value) % 4) % 4), true);
        if ($decoded === false) {
            throw new \InvalidArgumentException('not base64url');
        }
        return $decoded;
    }

    /** @param array<string, mixed> $data */
    public static function encodeJson(array $data): string
    {
        return self::encode(json_encode($data, JSON_THROW_ON_ERROR | JSON_UNESCAPED_SLASHES));
    }

    /** @return array<string, mixed> */
    public static function decodeJson(string $value): array
    {
        $data = json_decode(self::decode($value), true, 16, JSON_THROW_ON_ERROR);
        if (!is_array($data)) {
            throw new \InvalidArgumentException('not a JSON object');
        }
        return $data;
    }

    public static function random(int $bytes): string
    {
        return self::encode(random_bytes($bytes));
    }

    public static function sha256(string $value): string
    {
        return self::encode(hash('sha256', $value, true));
    }

    public static function equals(mixed $a, mixed $b): bool
    {
        return is_string($a) && is_string($b) && hash_equals($b, $a);
    }
}
