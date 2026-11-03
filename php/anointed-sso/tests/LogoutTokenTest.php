<?php
// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

declare(strict_types=1);

namespace AnointedAutomation\Sso\Tests;

use AnointedAutomation\Sso\AnointedSsoException;
use AnointedAutomation\Sso\BackchannelLogoutHandler;
use AnointedAutomation\Sso\InMemoryLogoutReplayStore;
use AnointedAutomation\Sso\LogoutToken;
use AnointedAutomation\Sso\Tests\Support\Provider;
use AnointedAutomation\Sso\Tests\Support\RecordingLogger;
use PHPUnit\Framework\TestCase;

final class LogoutTokenTest extends TestCase
{
    private Provider $provider;

    private RecordingLogger $log;

    protected function setUp(): void
    {
        $this->provider = new Provider();
        $this->log = new RecordingLogger();
    }

    public function testValidToken(): void
    {
        $t = $this->provider->client()->verifyLogoutToken($this->provider->logoutToken(['sid' => 's1']));
        $this->assertSame('user-1', $t->sub);
        $this->assertSame('s1', $t->sid);
        $this->assertSame('jti-1', $t->jti);
    }

    /** @return array<string, array{0: array<string, mixed>, 1?: array<string, mixed>}> */
    public static function badTokens(): array
    {
        return [
            'wrong typ' => [[], ['typ' => 'JWT']],
            'no jti' => [['jti' => null]],
            'nonce present' => [['nonce' => 'n']],
            'no events' => [['events' => null]],
            'wrong event' => [['events' => ['http://other' => new \stdClass()]]],
            'no sub or sid' => [['sub' => null]],
            'wrong aud' => [['aud' => 'x']],
            'wrong iss' => [['iss' => 'https://evil.example/']],
            'too old' => [['iat' => Provider::NOW - 3600, 'exp' => Provider::NOW + 900]],
            'future' => [['iat' => Provider::NOW + 3600, 'exp' => Provider::NOW + 7200]],
        ];
    }

    /**
     * @param array<string, mixed> $claims
     * @param array<string, mixed> $headers
     */
    #[\PHPUnit\Framework\Attributes\DataProvider('badTokens')]
    public function testBadTokensAreRejected(array $claims, array $headers = ['typ' => 'logout+jwt']): void
    {
        try {
            $this->provider->client()->verifyLogoutToken($this->provider->logoutToken($claims, $headers));
            $this->fail('accepted');
        } catch (AnointedSsoException $e) {
            $this->assertSame('logout_token_invalid', $e->errorCode, $e->getMessage());
        }
    }

    public function testForeignSignatureRejected(): void
    {
        $this->expectException(AnointedSsoException::class);
        $this->provider->client()->verifyLogoutToken((new Provider())->logoutToken());
    }

    public function testHandlerAnswersAndBlocksReplay(): void
    {
        $seen = [];
        $handler = new BackchannelLogoutHandler($this->provider->client(), function (LogoutToken $t) use (&$seen): void {
            $seen[] = $t->sub;
        }, new InMemoryLogoutReplayStore(), $this->log);
        $body = http_build_query(['logout_token' => $this->provider->logoutToken()]);

        $this->assertSame(200, $handler->handle($body));
        $this->assertSame(200, $handler->handle($body));
        $this->assertSame(['user-1'], $seen, 'a repeated jti must not be acted on twice');
    }

    public function testHandlerStatusCodes(): void
    {
        $handler = new BackchannelLogoutHandler($this->provider->client(), static function (): void {
        }, new InMemoryLogoutReplayStore(), $this->log);
        $this->assertSame(400, $handler->handle(''));
        $this->assertSame(400, $handler->handle('logout_token='));
        $this->assertSame(400, $handler->handle('logout_token=garbage'));
        $this->assertSame(400, $handler->handle(http_build_query(['logout_token' => $this->provider->logoutToken(['nonce' => 'n'])])));
        $warnings = $this->log->at('warning');
        $this->assertCount(4, $warnings, 'every rejected logout must be logged');
        $this->assertSame('logout_token_invalid', $warnings[3]['context']['code']);
        $this->assertInstanceOf(AnointedSsoException::class, $warnings[3]['context']['exception']);
    }

    public function testFailedCallbackIsRetriable(): void
    {
        $calls = 0;
        $handler = new BackchannelLogoutHandler($this->provider->client(), function () use (&$calls): void {
            if (++$calls === 1) {
                throw new \RuntimeException('db down');
            }
        }, new InMemoryLogoutReplayStore(), $this->log);
        $body = http_build_query(['logout_token' => $this->provider->logoutToken()]);
        $this->assertSame(500, $handler->handle($body));
        $errors = $this->log->at('error');
        $this->assertCount(1, $errors, 'a failed logout callback must be logged as an error');
        $this->assertSame('jti-1', $errors[0]['context']['jti']);
        $this->assertInstanceOf(\RuntimeException::class, $errors[0]['context']['exception']);
        $this->assertSame(200, $handler->handle($body));
        $this->assertSame(2, $calls);
    }

    public function testUnreadableProviderKeysAreLoggedAsError(): void
    {
        $this->provider->http->route('GET', \AnointedAutomation\Sso\SsoConfig::ISSUER . 'connect/jwks', 'down', 503);
        $handler = new BackchannelLogoutHandler($this->provider->client(), static function (): void {
        }, new InMemoryLogoutReplayStore(), $this->log);

        $this->assertSame(500, $handler->handle(http_build_query(['logout_token' => $this->provider->logoutToken()])));
        $errors = $this->log->at('error');
        $this->assertCount(1, $errors);
        $this->assertArrayHasKey('exception', $errors[0]['context']);
    }
}
