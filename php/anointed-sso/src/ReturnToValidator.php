<?php
// Copyright © Anointed Automation Corporation., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

declare(strict_types=1);

namespace AnointedAutomation\Sso;

/**
 * Keeps a post-sign-in return target only when it is safe: a same-site absolute path, or (when configured) an
 * https URL whose origin is allowlisted. Blocks open redirects.
 */
final class ReturnToValidator
{
    private const MAX_LENGTH = 2048;
    private const PLACEHOLDER = 'wildcard-placeholder.';

    /** @var list<array{protocol: string, host: ?string, suffix: ?string, port: string}> */
    private array $matchers = [];

    /** @var (callable(string): bool)|null */
    private $isAllowed;

    /**
     * @param list<string> $allowedReturnOrigins "https://shop.example.com", "https://*.example.com" or "http://localhost:3000"
     * @param (callable(string): bool)|null $isAllowedReturnTo Extra check for an absolute URL that is not on the list
     */
    public function __construct(array $allowedReturnOrigins = [], ?callable $isAllowedReturnTo = null)
    {
        foreach ($allowedReturnOrigins as $entry) {
            $this->matchers[] = self::compile($entry);
        }
        $this->isAllowed = $isAllowedReturnTo;
    }

    public static function isLoopbackHost(string $host): bool
    {
        return in_array(strtolower($host), ['localhost', '127.0.0.1', '[::1]'], true);
    }

    public static function safeRelativePath(mixed $value): ?string
    {
        if (!is_string($value) || $value === '' || strlen($value) > self::MAX_LENGTH) {
            return null;
        }
        if ($value[0] !== '/' || str_starts_with($value, '//') || str_starts_with($value, '/\\')) {
            return null;
        }
        if (preg_match('/[\x00-\x1f\x7f\\\\]/', $value) === 1) {
            return null;
        }
        return $value;
    }

    public function validate(mixed $value): ?string
    {
        $relative = self::safeRelativePath($value);
        $absoluteAllowed = $this->matchers !== [] || $this->isAllowed !== null;
        if ($relative !== null || !$absoluteAllowed) {
            return $relative;
        }
        if (!is_string($value) || strlen($value) > self::MAX_LENGTH || preg_match('/[\x00-\x20\x7f\\\\]/', $value) === 1) {
            return null;
        }
        if (preg_match('#^https?://[^/]#i', $value) !== 1) {
            return null;
        }
        $parts = parse_url($value);
        if ($parts === false || !isset($parts['scheme'], $parts['host']) || isset($parts['user']) || isset($parts['pass'])) {
            return null;
        }
        $scheme = strtolower($parts['scheme']);
        $host = strtolower($parts['host']);
        if ($scheme !== 'https' && !($scheme === 'http' && self::isLoopbackHost($host))) {
            return null;
        }
        $port = self::normalizePort($scheme, $parts['port'] ?? null);
        $href = $scheme . '://' . $host . ($port === '' ? '' : ':' . $port) . ($parts['path'] ?? '/')
            . (isset($parts['query']) ? '?' . $parts['query'] : '')
            . (isset($parts['fragment']) ? '#' . $parts['fragment'] : '');
        foreach ($this->matchers as $m) {
            if ($m['protocol'] !== $scheme || $m['port'] !== $port) {
                continue;
            }
            if ($m['host'] !== null ? $m['host'] === $host : (strlen($host) > strlen((string) $m['suffix']) && str_ends_with($host, (string) $m['suffix']))) {
                return $href;
            }
        }
        if ($this->isAllowed !== null && ($this->isAllowed)($href) === true) {
            return $href;
        }
        return null;
    }

    private static function normalizePort(string $scheme, ?int $port): string
    {
        if ($port === null || ($scheme === 'https' && $port === 443) || ($scheme === 'http' && $port === 80)) {
            return '';
        }
        return (string) $port;
    }

    /** @return array{protocol: string, host: ?string, suffix: ?string, port: string} */
    private static function compile(mixed $entry): array
    {
        if (!is_string($entry) || trim($entry) === '') {
            throw new AnointedSsoException('config', 'allowedReturnOrigins entries must be non-empty strings');
        }
        $wildcard = preg_match('#^(https?)://\*\.(.+)$#i', $entry, $m) === 1;
        $parseable = $wildcard ? $m[1] . '://' . self::PLACEHOLDER . $m[2] : $entry;
        $p = parse_url($parseable);
        if ($p === false || !isset($p['scheme'], $p['host'])) {
            throw new AnointedSsoException('config', "allowedReturnOrigins entry {$entry} is not a URL origin");
        }
        if (isset($p['user']) || isset($p['pass']) || isset($p['query']) || isset($p['fragment']) || (isset($p['path']) && $p['path'] !== '/')) {
            throw new AnointedSsoException('config', "allowedReturnOrigins entry {$entry} must be a bare origin");
        }
        $scheme = strtolower($p['scheme']);
        $host = strtolower($p['host']);
        if ($scheme !== 'https' && !($scheme === 'http' && !$wildcard && self::isLoopbackHost($host))) {
            throw new AnointedSsoException('config', "allowedReturnOrigins entry {$entry} must use https (http only for localhost)");
        }
        $port = self::normalizePort($scheme, $p['port'] ?? null);
        if (!$wildcard) {
            return ['protocol' => $scheme, 'host' => $host, 'suffix' => null, 'port' => $port];
        }
        $base = substr($host, strlen(self::PLACEHOLDER));
        if (!str_contains($base, '.') || str_starts_with($base, '.') || str_contains($base, '*')) {
            throw new AnointedSsoException('config', "allowedReturnOrigins wildcard {$entry} must name a domain like *.example.com");
        }
        return ['protocol' => $scheme, 'host' => null, 'suffix' => '.' . $base, 'port' => $port];
    }
}
