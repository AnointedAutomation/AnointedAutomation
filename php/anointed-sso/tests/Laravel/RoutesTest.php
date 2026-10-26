<?php
// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

declare(strict_types=1);

namespace AnointedAutomation\Sso\Tests\Laravel;

use AnointedAutomation\Sso\Client;
use AnointedAutomation\Sso\Laravel\AnointedSsoServiceProvider;
use AnointedAutomation\Sso\Laravel\Contracts\LogoutHandler;
use AnointedAutomation\Sso\Laravel\Contracts\SignedInHandler;
use AnointedAutomation\Sso\LogoutToken;
use AnointedAutomation\Sso\SignInResult;
use AnointedAutomation\Sso\Tests\Support\Provider;
use Illuminate\Http\Request;
use Orchestra\Testbench\TestCase;
use Symfony\Component\HttpFoundation\Response;

final class RoutesTest extends TestCase
{
    private Provider $provider;

    protected function getPackageProviders($app): array
    {
        return [AnointedSsoServiceProvider::class];
    }

    protected function defineEnvironment($app): void
    {
        $app['config']->set('app.key', 'base64:' . base64_encode(str_repeat('k', 32)));
        $app['config']->set('cache.default', 'array');
        $app['config']->set('session.driver', 'array');
        $app['config']->set('anointed-sso.client_id', Provider::CLIENT_ID);
        $app['config']->set('anointed-sso.client_secret', 'secret');
        $app['config']->set('anointed-sso.redirect_uri', Provider::REDIRECT);
        $app['config']->set('anointed-sso.signed_in_handler', TestSignedIn::class);
        $app['config']->set('anointed-sso.logout_handler', TestLogout::class);
        $app['config']->set('anointed-sso.post_logout_redirect_uri', 'https://app.example/bye');
    }

    protected function setUp(): void
    {
        parent::setUp();
        $this->provider = new Provider();
        TestLogout::$seen = [];
        $this->app->singleton(Client::class, fn (): Client => $this->provider->client());
    }

    public function testStartRedirectsToAuthorizeEndpointAndStoresFlow(): void
    {
        $response = $this->get('/auth/anointed/start?return_to=/dash');
        $response->assertRedirectContains('connect/authorize?');
        $this->assertStringContainsString('code_challenge_method=S256', (string) $response->headers->get('Location'));
        $this->assertNotNull(session('anointed_sso.flow'));
    }

    public function testCallbackSignsInThroughYourHandler(): void
    {
        $auth = $this->provider->client()->createAuthorizeRequest(['returnTo' => '/dash']);
        parse_str((string) parse_url($auth->url, PHP_URL_QUERY), $q);
        $this->provider->tokenResponse($this->provider->idToken($q['nonce']));

        $response = $this->withSession(['anointed_sso.flow' => $auth->cookie->value])
            ->get('/auth/anointed/callback?code=abc&state=' . $auth->state);
        $response->assertRedirect('/welcome/user-1');

        $this->withSession(['anointed_sso.flow' => $auth->cookie->value])
            ->get('/auth/anointed/callback?code=abc&state=wrong')->assertStatus(400);
        $this->get('/auth/anointed/callback?code=abc&state=' . $auth->state)->assertStatus(400);
    }

    public function testLogoutRedirectsToEndSession(): void
    {
        $this->get('/auth/anointed/logout')->assertRedirectContains('connect/logout?');
    }

    public function testBackchannelLogoutRunsOnceAndRejectsBadTokens(): void
    {
        $send = fn (string $body) => $this->call('POST', '/auth/anointed/backchannel-logout', [], [], [], ['CONTENT_TYPE' => 'application/x-www-form-urlencoded'], $body);
        $good = http_build_query(['logout_token' => $this->provider->logoutToken(['jti' => 'abc'])]);
        $send($good)->assertOk();
        $send($good)->assertOk();
        $this->assertSame(['user-1'], TestLogout::$seen);
        $send('logout_token=junk')->assertStatus(400);
        $send('')->assertStatus(400);
    }
}

final class TestSignedIn implements SignedInHandler
{
    public function handle(SignInResult $result, Request $request): Response
    {
        return redirect('/welcome/' . $result->user->sub);
    }
}

final class TestLogout implements LogoutHandler
{
    /** @var list<?string> */
    public static array $seen = [];

    public function handle(LogoutToken $logout): void
    {
        self::$seen[] = $logout->sub;
    }
}
