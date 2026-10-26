<?php
// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

declare(strict_types=1);

namespace AnointedAutomation\Sso\Tests\Support;

use AnointedAutomation\Sso\Client;
use AnointedAutomation\Sso\SsoConfig;
use Firebase\JWT\JWT;

/** A fake Anointed Automation: an RSA key, discovery, JWKS, token endpoint and a token signer. */
final class Provider
{
    public const NOW = 1_800_000_000;
    public const CLIENT_ID = 'aa_test';
    public const REDIRECT = 'https://app.example/auth/anointed/callback';
    public const KID = 'key-1';

    public FakeHttp $http;
    public string $privateKey;
    /** @var array<string, string> */
    public array $jwk;

    public function __construct(string $kid = self::KID)
    {
        $res = openssl_pkey_new(['private_key_bits' => 2048, 'private_key_type' => OPENSSL_KEYTYPE_RSA]);
        openssl_pkey_export($res, $pem);
        $this->privateKey = $pem;
        $d = openssl_pkey_get_details($res);
        $b64 = static fn (string $s): string => rtrim(strtr(base64_encode($s), '+/', '-_'), '=');
        $this->jwk = ['kty' => 'RSA', 'use' => 'sig', 'alg' => 'RS256', 'kid' => $kid, 'n' => $b64($d['rsa']['n']), 'e' => $b64($d['rsa']['e'])];
        $this->http = new FakeHttp();
        $this->http->route('GET', SsoConfig::ISSUER . '.well-known/openid-configuration', [
            'issuer' => SsoConfig::ISSUER,
            'authorization_endpoint' => SsoConfig::ISSUER . 'connect/authorize',
            'token_endpoint' => SsoConfig::ISSUER . 'connect/token',
            'userinfo_endpoint' => SsoConfig::ISSUER . 'connect/userinfo',
            'revocation_endpoint' => SsoConfig::ISSUER . 'connect/revoke',
            'end_session_endpoint' => SsoConfig::ISSUER . 'connect/logout',
            'jwks_uri' => SsoConfig::ISSUER . 'connect/jwks',
            'code_challenge_methods_supported' => ['S256'],
        ]);
        $this->http->route('GET', SsoConfig::ISSUER . 'connect/jwks', ['keys' => [$this->jwk]]);
    }

    public function config(string $scope = SsoConfig::DEFAULT_SCOPE): SsoConfig
    {
        return new SsoConfig(SsoConfig::ISSUER, self::CLIENT_ID, 'secret', self::REDIRECT, $scope);
    }

    public function client(?SsoConfig $config = null): Client
    {
        return new Client($config ?? $this->config(), $this->http, null, fn (): int => self::NOW);
    }

    /**
     * @param array<string, mixed> $claims merged over valid defaults; a null value removes the claim
     * @param array<string, mixed> $headers
     */
    public function sign(array $claims, array $headers = [], ?string $key = null, string $alg = 'RS256'): string
    {
        $payload = array_merge([
            'iss' => SsoConfig::ISSUER, 'aud' => self::CLIENT_ID, 'sub' => 'user-1',
            'iat' => self::NOW, 'exp' => self::NOW + 900,
        ], $claims);
        $payload = array_filter($payload, static fn ($v): bool => $v !== null);
        return JWT::encode($payload, $key ?? $this->privateKey, $alg, $headers['kid'] ?? self::KID, array_diff_key($headers, ['kid' => 1]));
    }

    /** @param array<string, mixed> $claims */
    public function idToken(string $nonce, array $claims = []): string
    {
        return $this->sign(array_merge(['nonce' => $nonce, 'email' => 'Me@Example.com', 'email_verified' => true, 'sid' => 'sid-1'], $claims));
    }

    /** @param array<string, mixed> $claims */
    public function logoutToken(array $claims = [], array $headers = ['typ' => 'logout+jwt']): string
    {
        return $this->sign(array_merge([
            'jti' => 'jti-1',
            'events' => ['http://schemas.openid.net/event/backchannel-logout' => new \stdClass()],
        ], $claims), $headers);
    }

    public function tokenResponse(string $idToken): void
    {
        $this->http->route('POST', SsoConfig::ISSUER . 'connect/token', [
            'access_token' => 'at', 'id_token' => $idToken, 'refresh_token' => 'rt', 'token_type' => 'Bearer', 'expires_in' => 900,
        ]);
    }
}
