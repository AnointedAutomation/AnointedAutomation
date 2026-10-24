<?php
// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

declare(strict_types=1);

namespace AnointedAutomation\Sso\Tests;

use AnointedAutomation\Sso\AnointedSsoException;
use AnointedAutomation\Sso\SsoConfig;
use AnointedAutomation\Sso\Tests\Support\Provider;
use PHPUnit\Framework\TestCase;

final class ClientTest extends TestCase
{
    private Provider $provider;

    protected function setUp(): void
    {
        $this->provider = new Provider();
    }

    /** @return array{0: array<string, string>, 1: string, 2: string} callback query, cookie, nonce */
    private function startFlow(): array
    {
        $auth = $this->provider->client()->createAuthorizeRequest(['returnTo' => '/dashboard']);
        parse_str((string) parse_url($auth->url, PHP_URL_QUERY), $q);
        return [['state' => $auth->state, 'code' => 'the-code'], $auth->cookie->value, $q['nonce']];
    }

    private function assertFails(string $code, callable $fn): void
    {
        try {
            $fn();
            $this->fail("expected {$code}");
        } catch (AnointedSsoException $e) {
            $this->assertSame($code, $e->errorCode, $e->getMessage());
        }
    }

    public function testAuthorizeUrlCarriesPkceStateNonceAndScope(): void
    {
        $auth = $this->provider->client()->createAuthorizeRequest();
        parse_str((string) parse_url($auth->url, PHP_URL_QUERY), $q);
        $this->assertStringStartsWith(SsoConfig::ISSUER . 'connect/authorize?', $auth->url);
        $this->assertSame('code', $q['response_type']);
        $this->assertSame(Provider::CLIENT_ID, $q['client_id']);
        $this->assertSame(Provider::REDIRECT, $q['redirect_uri']);
        $this->assertSame('openid email', $q['scope']);
        $this->assertSame('S256', $q['code_challenge_method']);
        $this->assertSame($auth->state, $q['state']);
        $this->assertGreaterThanOrEqual(43, strlen($q['nonce']));

        $flow = $this->provider->client()->decodeFlowCookie($auth->cookie->value);
        $this->assertSame(rtrim(strtr(base64_encode(hash('sha256', $flow['verifier'], true)), '+/', '-_'), '='), $q['code_challenge']);
        $this->assertStringContainsString('HttpOnly', $auth->cookie->toHeader());
        $this->assertStringContainsString('SameSite=Lax', $auth->cookie->toHeader());
        $this->assertStringContainsString('Secure', $auth->cookie->toHeader());
    }

    public function testCallbackReturnsVerifiedUser(): void
    {
        [$query, $cookie, $nonce] = $this->startFlow();
        $this->provider->tokenResponse($this->provider->idToken($nonce));
        $result = $this->provider->client()->handleCallback($query, $cookie);

        $this->assertSame('user-1', $result->user->sub);
        $this->assertSame('me@example.com', $result->user->email);
        $this->assertTrue($result->user->emailVerified);
        $this->assertSame('sid-1', $result->user->sid);
        $this->assertSame('/dashboard', $result->returnTo);
        $this->assertSame('rt', $result->tokens->refreshToken);
        $this->assertSame(Provider::NOW + 900, $result->tokens->expiresAt);

        $post = array_values(array_filter($this->provider->http->requests, static fn ($r): bool => $r->getMethod() === 'POST'))[0];
        parse_str((string) $post->getBody(), $form);
        $this->assertSame('authorization_code', $form['grant_type']);
        $this->assertSame('secret', $form['client_secret']);
        $this->assertSame('the-code', $form['code']);
        $this->assertSame(Provider::REDIRECT, $form['redirect_uri']);
        $this->assertNotSame('', $form['code_verifier']);
    }

    public function testStateMismatchAndMissingCookie(): void
    {
        [$query, $cookie] = $this->startFlow();
        $c = $this->provider->client();
        $this->assertFails('state_mismatch', fn () => $c->handleCallback(['state' => 'other', 'code' => 'x'], $cookie));
        $this->assertFails('flow_missing', fn () => $c->handleCallback($query, null));
        $this->assertFails('flow_missing', fn () => $c->handleCallback($query, 'not-a-cookie'));
    }

    public function testProviderErrorsAndIssuerMismatch(): void
    {
        [$query, $cookie] = $this->startFlow();
        $c = $this->provider->client();
        $this->assertFails('access_denied', fn () => $c->handleCallback(['state' => $query['state'], 'error' => 'access_denied'], $cookie));
        $this->assertFails('provider_error', fn () => $c->handleCallback(['state' => $query['state'], 'error' => 'server_error'], $cookie));
        $this->assertFails('issuer_mismatch', fn () => $c->handleCallback($query + ['iss' => 'https://evil.example/'], $cookie));
        $this->assertFails('missing_code', fn () => $c->handleCallback(['state' => $query['state']], $cookie));
    }

    public function testFlowExpires(): void
    {
        [$query, $cookie] = $this->startFlow();
        $late = new \AnointedAutomation\Sso\Client($this->provider->config(), $this->provider->http, null, fn (): int => Provider::NOW + 601);
        $this->assertFails('flow_expired', fn () => $late->handleCallback($query, $cookie));
    }

    public function testBadIdTokensAreRejected(): void
    {
        [$query, $cookie, $nonce] = $this->startFlow();
        $other = new Provider();
        $cases = [
            'wrong nonce' => $this->provider->idToken('different'),
            'wrong aud' => $this->provider->idToken($nonce, ['aud' => 'someone-else']),
            'wrong iss' => $this->provider->idToken($nonce, ['iss' => 'https://evil.example/']),
            'expired' => $this->provider->idToken($nonce, ['exp' => Provider::NOW - 3600]),
            'future iat' => $this->provider->idToken($nonce, ['iat' => Provider::NOW + 3600]),
            'no sub' => $this->provider->idToken($nonce, ['sub' => null]),
            'azp mismatch' => $this->provider->idToken($nonce, ['azp' => 'someone-else']),
            'foreign signature' => $other->idToken($nonce),
            'unknown kid' => $this->provider->sign(['nonce' => $nonce], ['kid' => 'nope']),
            'alg none' => rtrim(strtr(base64_encode('{"alg":"none"}'), '+/', '-_'), '=') . '.' . rtrim(strtr(base64_encode('{"sub":"x"}'), '+/', '-_'), '=') . '.',
            'garbage' => 'a.b.c',
        ];
        foreach ($cases as $label => $idToken) {
            $this->provider->tokenResponse($idToken);
            try {
                $this->provider->client()->handleCallback($query, $cookie);
                $this->fail("accepted: {$label}");
            } catch (AnointedSsoException $e) {
                $this->assertSame('id_token_invalid', $e->errorCode, $label . ': ' . $e->getMessage());
            }
        }
    }

    public function testHs256ConfusionIsRejected(): void
    {
        [$query, $cookie, $nonce] = $this->startFlow();
        $forged = $this->provider->sign(['nonce' => $nonce], [], $this->provider->jwk['n'], 'HS256');
        $this->provider->tokenResponse($forged);
        $this->assertFails('id_token_invalid', fn () => $this->provider->client()->handleCallback($query, $cookie));
    }

    public function testRequiresVerifiedEmailWhenEmailScope(): void
    {
        [$query, $cookie, $nonce] = $this->startFlow();
        $this->provider->tokenResponse($this->provider->idToken($nonce, ['email_verified' => false]));
        $this->assertFails('email_not_verified', fn () => $this->provider->client()->handleCallback($query, $cookie));
    }

    public function testUnknownKidTriggersOneJwksRefresh(): void
    {
        $c = $this->provider->client();
        $c->getJwks();
        $rotated = new Provider('key-2');
        // The provider now serves a rotated key; the cached JWKS is stale.
        $this->provider->http->route('GET', SsoConfig::ISSUER . 'connect/jwks', ['keys' => [$rotated->jwk]]);
        $auth = $c->createAuthorizeRequest();
        parse_str((string) parse_url($auth->url, PHP_URL_QUERY), $q);
        $this->provider->tokenResponse($rotated->sign(['nonce' => $q['nonce'], 'email' => 'a@b.co', 'email_verified' => true], ['kid' => 'key-2']));
        $result = $c->handleCallback(['state' => $auth->state, 'code' => 'c'], $auth->cookie->value);
        $this->assertSame('user-1', $result->user->sub);
        $this->assertSame(2, $this->provider->http->count('GET', SsoConfig::ISSUER . 'connect/jwks'));
    }

    public function testTokenEndpointFailure(): void
    {
        [$query, $cookie] = $this->startFlow();
        $this->provider->http->route('POST', SsoConfig::ISSUER . 'connect/token', ['error' => 'invalid_grant'], 400);
        try {
            $this->provider->client()->handleCallback($query, $cookie);
            $this->fail('expected failure');
        } catch (AnointedSsoException $e) {
            $this->assertSame('token_exchange', $e->errorCode);
            $this->assertSame(400, $e->status);
            $this->assertSame('invalid_grant', $e->oauthError);
        }
    }

    public function testRedirectUriOverrideMustBeAllowlisted(): void
    {
        $c = $this->provider->client();
        $this->assertFails('redirect_uri_rejected', fn () => $c->createAuthorizeRequest(['redirectUri' => 'https://evil.example/cb']));
        $this->assertFails('redirect_uri_rejected', fn () => $c->createAuthorizeRequest(['redirectUri' => 'http://app.example/cb']));
        $allowed = new SsoConfig(SsoConfig::ISSUER, 'aa_test', 's', Provider::REDIRECT, allowedRedirectUris: [Provider::REDIRECT, 'https://app.example/other']);
        $auth = $this->provider->client($allowed)->createAuthorizeRequest(['redirectUri' => 'https://app.example/other']);
        $this->assertSame('https://app.example/other', $auth->redirectUri);
    }

    public function testExtraParamsCannotOverrideReserved(): void
    {
        $this->assertFails('config', fn () => $this->provider->client()->createAuthorizeRequest(['extraParams' => ['state' => 'x']]));
    }

    public function testLogoutUrl(): void
    {
        $url = $this->provider->client()->buildLogoutUrl(['idTokenHint' => 'tok', 'postLogoutRedirectUri' => 'https://app.example/bye']);
        $this->assertStringStartsWith(SsoConfig::ISSUER . 'connect/logout?', $url);
        parse_str((string) parse_url($url, PHP_URL_QUERY), $q);
        $this->assertSame(['client_id' => 'aa_test', 'id_token_hint' => 'tok', 'post_logout_redirect_uri' => 'https://app.example/bye'], $q);
        $this->assertFails('config', fn () => $this->provider->client()->buildLogoutUrl(['postLogoutRedirectUri' => 'http://evil.example/']));
    }

    public function testDiscoveryIssuerMismatchFails(): void
    {
        $this->provider->http->route('GET', SsoConfig::ISSUER . '.well-known/openid-configuration', ['issuer' => 'https://evil.example/']);
        $this->assertFails('discovery', fn () => $this->provider->client()->discover());
    }

    public function testRefreshAndUserInfo(): void
    {
        $this->provider->http->route('POST', SsoConfig::ISSUER . 'connect/token', ['access_token' => 'at2', 'refresh_token' => 'rt2', 'expires_in' => 900, 'id_token' => $this->provider->sign([])]);
        $r = $this->provider->client()->refresh('rt', 'user-1');
        $this->assertSame('rt2', $r['tokens']->refreshToken);
        $this->assertSame('user-1', $r['claims']['sub']);
        $this->assertFails('id_token_invalid', fn () => $this->provider->client()->refresh('rt', 'someone-else'));

        $this->provider->http->route('GET', SsoConfig::ISSUER . 'connect/userinfo', ['sub' => 'user-1']);
        $this->assertSame('user-1', $this->provider->client()->fetchUserInfo('at')['sub']);
    }
}
