<?php
// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

declare(strict_types=1);

namespace AnointedAutomation\Sso;

use Closure;
use GuzzleHttp\Psr7\Request;
use Psr\Http\Client\ClientInterface;
use Throwable;

/**
 * Asks Anointed Automation whether a user's session may continue. FAIL-OPEN: only an explicit
 * `active: false` answer ends a session; every failure is `SessionCheck::UNKNOWN`. Your partner API key
 * needs the `sessions:status` permission (ask Anointed Automation to add it).
 */
final class SessionStatusClient
{
    private readonly ClientInterface $http;
    private readonly MetadataCache $cache;
    private readonly Closure $clock;

    public function __construct(
        private readonly string $apiKey,
        private readonly string $appName,
        private readonly string $baseUrl = SsoConfig::ISSUER,
        ?ClientInterface $http = null,
        ?MetadataCache $cache = null,
        private readonly int $answerCacheSeconds = 300,
        private readonly int $tokenRefreshMarginSeconds = 60,
        ?Closure $clock = null,
    ) {
        if (trim($apiKey) === '') {
            throw new AnointedSsoException('config', 'apiKey is required');
        }
        if (trim($appName) === '') {
            throw new AnointedSsoException('config', 'appName is required');
        }
        SsoConfig::assertSecureUrl($baseUrl, 'baseUrl', 'config');
        $this->http = $http ?? new \GuzzleHttp\Client(['timeout' => 3, 'http_errors' => false]);
        $this->cache = $cache ?? new ArrayMetadataCache();
        $this->clock = $clock ?? static fn (): int => time();
    }

    /**
     * @param string $userId The Anointed Automation user id (the `sub` claim)
     * @param bool $grant True for a session that came from Sign in with Anointed Automation (it also ends when the user disconnected your app)
     */
    public function check(string $userId, bool $grant = true): SessionCheck
    {
        $cacheKey = 'anointed_sso.session.' . md5($userId . '|' . ($grant ? '1' : '0'));
        $cached = $this->cache->get($cacheKey);
        if ($cached !== null && isset($cached['active']) && is_bool($cached['active'])) {
            return $cached['active'] ? SessionCheck::active($cached['reason'] ?? null) : SessionCheck::ended($cached['reason'] ?? null);
        }
        try {
            $answer = $this->query($userId, $grant);
        } catch (Throwable $e) {
            return SessionCheck::unknown($e->getMessage());
        }
        if ($answer->outcome !== SessionCheck::UNKNOWN && $this->answerCacheSeconds > 0) {
            $this->cache->set($cacheKey, ['active' => $answer->outcome === SessionCheck::ACTIVE, 'reason' => $answer->reason], $this->answerCacheSeconds);
        }
        return $answer;
    }

    private function query(string $userId, bool $grant): SessionCheck
    {
        $token = $this->machineToken(null);
        $response = $this->sendStatus($userId, $grant, $token);
        if ($response->getStatusCode() === 401) {
            $response = $this->sendStatus($userId, $grant, $this->machineToken($token));
        }
        if ($response->getStatusCode() !== 200) {
            return SessionCheck::unknown('session-status answered HTTP ' . $response->getStatusCode());
        }
        $root = json_decode((string) $response->getBody(), true);
        if (!is_array($root)) {
            return SessionCheck::unknown('session-status body is not JSON');
        }
        if (self::find($root, 'success') === false) {
            return SessionCheck::unknown('session-status reported failure');
        }
        $data = self::find($root, 'data');
        $active = is_array($data) ? self::find($data, 'active') : null;
        if (!is_bool($active)) {
            return SessionCheck::unknown('session-status has no boolean active');
        }
        $reason = is_array($data) ? self::find($data, 'reason') : null;
        $reason = is_string($reason) ? $reason : null;
        return $active ? SessionCheck::active($reason) : SessionCheck::ended($reason);
    }

    private function sendStatus(string $userId, bool $grant, string $token): \Psr\Http\Message\ResponseInterface
    {
        $url = rtrim($this->baseUrl, '/') . '/api/session-status?' . http_build_query(['userId' => $userId, 'grant' => $grant ? 'true' : 'false'], '', '&', PHP_QUERY_RFC3986);
        return $this->http->sendRequest(new Request('GET', $url, ['Authorization' => 'Bearer ' . $token, 'Accept' => 'application/json']));
    }

    private function machineToken(?string $rejected): string
    {
        $key = 'anointed_sso.machine_token.' . md5($this->apiKey);
        $cached = $rejected === null ? $this->cache->get($key) : null;
        if ($cached !== null && isset($cached['token']) && is_string($cached['token'])) {
            return $cached['token'];
        }
        $response = $this->http->sendRequest(new Request(
            'POST',
            rtrim($this->baseUrl, '/') . '/api/auth/machine-token',
            ['Content-Type' => 'application/json', 'Accept' => 'application/json'],
            json_encode(['apiKey' => $this->apiKey, 'name' => $this->appName], JSON_THROW_ON_ERROR),
        ));
        if ($response->getStatusCode() !== 200) {
            throw new AnointedSsoException('http', 'machine-token exchange answered HTTP ' . $response->getStatusCode());
        }
        $root = json_decode((string) $response->getBody(), true);
        $data = is_array($root) ? self::find($root, 'data') : null;
        $token = is_array($data) ? self::find($data, 'token') : null;
        $expiresAt = is_array($data) ? self::find($data, 'expiresAt') : null;
        if (!is_string($token) || $token === '' || !is_string($expiresAt) || ($expires = strtotime($expiresAt)) === false) {
            throw new AnointedSsoException('http', 'machine-token response has no token and expiresAt');
        }
        $ttl = $expires - ($this->clock)() - $this->tokenRefreshMarginSeconds;
        if ($ttl > 0) {
            $this->cache->set($key, ['token' => $token], $ttl);
        }
        return $token;
    }

    /** Case-insensitive property lookup (the API may answer in PascalCase). */
    private static function find(array $object, string $name): mixed
    {
        foreach ($object as $key => $value) {
            if (is_string($key) && strcasecmp($key, $name) === 0) {
                return $value;
            }
        }
        return null;
    }
}
