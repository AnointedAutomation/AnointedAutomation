<?php
// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

declare(strict_types=1);

namespace AnointedAutomation\Sso\Tests;

use AnointedAutomation\Sso\AnointedSsoException;
use AnointedAutomation\Sso\ReturnToValidator;
use PHPUnit\Framework\TestCase;

final class ReturnToValidatorTest extends TestCase
{
    public function testRelativePathsOnly(): void
    {
        $v = new ReturnToValidator();
        $this->assertSame('/a/b?c=1', $v->validate('/a/b?c=1'));
        foreach (['//evil.example', '/\\evil.example', 'https://evil.example/', 'javascript:alert(1)', '', "/a\nb", '/a\\b', null, 5] as $bad) {
            $this->assertNull($v->validate($bad), var_export($bad, true));
        }
        $this->assertNull($v->validate('/' . str_repeat('a', 2100)));
    }

    public function testAllowlistedOrigins(): void
    {
        $v = new ReturnToValidator(['https://shop.example.com', 'https://*.example.org', 'http://localhost:3000']);
        $this->assertSame('https://shop.example.com/x?y=1', $v->validate('https://shop.example.com/x?y=1'));
        $this->assertSame('https://shop.example.com/', $v->validate('https://shop.example.com:443'), 'default port is normalized');
        $this->assertSame('https://a.b.example.org/', $v->validate('https://a.b.example.org'));
        $this->assertSame('http://localhost:3000/p', $v->validate('http://localhost:3000/p'));
        $this->assertNull($v->validate('https://example.org/'), 'wildcard does not include the bare base');
        $this->assertNull($v->validate('https://evil.example.com/'));
        $this->assertNull($v->validate('https://shop.example.com.evil.test/'));
        $this->assertNull($v->validate('http://shop.example.com/'));
        $this->assertNull($v->validate('http://localhost:4000/'));
        $this->assertNull($v->validate('https://user:pw@shop.example.com/'));
        $this->assertSame('/ok', $v->validate('/ok'));
    }

    public function testCallbackPredicate(): void
    {
        $v = new ReturnToValidator([], static fn (string $url): bool => str_starts_with($url, 'https://tenant.example/'));
        $this->assertSame('https://tenant.example/a', $v->validate('https://tenant.example/a'));
        $this->assertNull($v->validate('https://other.example/a'));
    }

    public function testBadOriginEntriesFailFast(): void
    {
        foreach (['https://x.com/path', 'http://x.com', 'https://*.com', 'ftp://x.com', '', 'https://*.localhost'] as $entry) {
            try {
                new ReturnToValidator([$entry]);
                $this->fail("accepted {$entry}");
            } catch (AnointedSsoException $e) {
                $this->assertSame('config', $e->errorCode);
            }
        }
    }
}
