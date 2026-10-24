<?php
// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

declare(strict_types=1);

namespace AnointedAutomation\Sso\Tests;

use AnointedAutomation\Sso\AnointedSsoException;
use AnointedAutomation\Sso\SsoConfig;
use PHPUnit\Framework\TestCase;

final class ConfigTest extends TestCase
{
    /** @return array<string, mixed> */
    private function values(): array
    {
        return ['issuer' => SsoConfig::ISSUER, 'client_id' => 'aa_x', 'client_secret' => 's', 'redirect_uri' => 'https://app.example/cb'];
    }

    public function testFromArrayUsesDefaultsForNullKeys(): void
    {
        $c = SsoConfig::fromArray($this->values() + ['scope' => null, 'cookie_name' => null]);
        $this->assertSame(['openid', 'email'], $c->scopes);
        $this->assertTrue($c->requireVerifiedEmail);
        $this->assertSame('aa_sso_flow', $c->cookieName);
    }

    public function testRequiredValuesHaveNoFallback(): void
    {
        foreach (['issuer', 'client_id', 'client_secret', 'redirect_uri'] as $key) {
            $values = $this->values();
            $values[$key] = null;
            try {
                SsoConfig::fromArray($values);
                $this->fail("{$key} was optional");
            } catch (AnointedSsoException $e) {
                $this->assertSame('config', $e->errorCode);
            }
            $values[$key] = '  ';
            try {
                SsoConfig::fromArray($values);
                $this->fail("{$key} accepted blank");
            } catch (AnointedSsoException $e) {
                $this->assertSame('config', $e->errorCode);
            }
        }
    }

    public function testRejectsInsecureAndMalformedSettings(): void
    {
        foreach ([
            ['redirect_uri' => 'http://app.example/cb'],
            ['redirect_uri' => 'https://app.example/cb#frag'],
            ['scope' => 'email'],
            ['cookie_name' => 'bad name'],
            ['cookie_path' => 'x'],
            ['flow_ttl_seconds' => 0],
        ] as $override) {
            try {
                SsoConfig::fromArray($override + $this->values());
                $this->fail('accepted ' . json_encode($override));
            } catch (AnointedSsoException $e) {
                $this->assertSame('config', $e->errorCode);
            }
        }
        $this->assertSame('http://localhost:8000/cb', SsoConfig::fromArray(['redirect_uri' => 'http://localhost:8000/cb'] + $this->values())->redirectUri);
    }

    public function testProfileOnlyScopeDoesNotRequireEmail(): void
    {
        $c = SsoConfig::fromArray(['scope' => 'openid profile'] + $this->values());
        $this->assertFalse($c->requireVerifiedEmail);
    }
}
