<?php
// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

declare(strict_types=1);

namespace AnointedAutomation\Sso\Tests;

use AnointedAutomation\Sso\SessionCheck;
use AnointedAutomation\Sso\SessionStatusClient;
use AnointedAutomation\Sso\SsoConfig;
use AnointedAutomation\Sso\Tests\Support\FakeHttp;
use PHPUnit\Framework\TestCase;

final class SessionStatusClientTest extends TestCase
{
    private function client(FakeHttp $http): SessionStatusClient
    {
        return new SessionStatusClient('key', 'My App', SsoConfig::ISSUER, $http, null, 300, 60, fn (): int => 1_800_000_000);
    }

    private function http(): FakeHttp
    {
        $http = new FakeHttp();
        $http->route('POST', SsoConfig::ISSUER . 'api/auth/machine-token', ['Data' => ['token' => 'mt', 'expiresAt' => gmdate('c', 1_800_000_000 + 3600)]]);
        return $http;
    }

    public function testActiveEndedAndCaching(): void
    {
        $http = $this->http();
        $http->route('GET', SsoConfig::ISSUER . 'api/session-status', ['success' => true, 'data' => ['active' => true, 'reason' => 'active']]);
        $c = $this->client($http);
        $this->assertSame(SessionCheck::ACTIVE, $c->check('u1')->outcome);
        $this->assertSame(SessionCheck::ACTIVE, $c->check('u1')->outcome);
        $this->assertSame(1, $http->count('GET', SsoConfig::ISSUER . 'api/session-status'), 'answer cached');
        $this->assertSame(1, $http->count('POST', SsoConfig::ISSUER . 'api/auth/machine-token'), 'token cached');

        $http->route('GET', SsoConfig::ISSUER . 'api/session-status', ['Data' => ['Active' => false, 'Reason' => 'account_banned']]);
        $ended = $c->check('u2');
        $this->assertTrue($ended->shouldEndSession());
        $this->assertSame('account_banned', $ended->reason);
        $this->assertStringContainsString('Bearer mt', $http->requests[array_key_last($http->requests)]->getHeaderLine('Authorization'));
    }

    public function testFailsOpen(): void
    {
        $http = $this->http();
        $c = $this->client($http); // session-status route missing -> 404
        $this->assertSame(SessionCheck::UNKNOWN, $c->check('u1')->outcome);
        $this->assertFalse($c->check('u1')->shouldEndSession());

        $http->route('GET', SsoConfig::ISSUER . 'api/session-status', 'not json');
        $this->assertFalse($this->client($http)->check('u3')->shouldEndSession());

        $http->route('GET', SsoConfig::ISSUER . 'api/session-status', ['data' => ['active' => 'no']]);
        $this->assertFalse($this->client($http)->check('u4')->shouldEndSession());

        $down = new FakeHttp(); // token exchange 404
        $this->assertSame(SessionCheck::UNKNOWN, $this->client($down)->check('u5')->outcome);
    }
}
