<?php
// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

declare(strict_types=1);

namespace AnointedAutomation\Sso\Laravel;

use AnointedAutomation\Sso\Client;
use AnointedAutomation\Sso\LogoutReplayStore;
use AnointedAutomation\Sso\MetadataCache;
use AnointedAutomation\Sso\SessionStatusClient;
use AnointedAutomation\Sso\SsoConfig;
use Illuminate\Contracts\Cache\Factory as CacheFactory;
use Illuminate\Support\Facades\Route;
use Illuminate\Support\ServiceProvider;

/** Auto-discovered. Publish the config with `php artisan vendor:publish --tag=anointed-sso-config`. */
final class AnointedSsoServiceProvider extends ServiceProvider
{
    public function register(): void
    {
        $this->mergeConfigFrom(__DIR__ . '/../../config/anointed-sso.php', 'anointed-sso');

        $this->app->singleton(SsoConfig::class, static fn ($app): SsoConfig => SsoConfig::fromArray($app['config']->get('anointed-sso')));
        $this->app->singleton(MetadataCache::class, fn ($app): MetadataCache => new LaravelMetadataCache($this->store($app)));
        $this->app->singleton(LogoutReplayStore::class, fn ($app): LogoutReplayStore => new LaravelLogoutReplayStore($this->store($app)));
        $this->app->singleton(Client::class, static fn ($app): Client => new Client($app->make(SsoConfig::class), null, $app->make(MetadataCache::class)));
        $this->app->singleton(SessionStatusClient::class, static function ($app): SessionStatusClient {
            $c = $app['config']->get('anointed-sso.session_status');
            return new SessionStatusClient((string) ($c['api_key'] ?? ''), (string) ($c['app_name'] ?? ''), SsoConfig::ISSUER, null, $app->make(MetadataCache::class));
        });
    }

    public function boot(): void
    {
        $this->publishes([__DIR__ . '/../../config/anointed-sso.php' => $this->app->configPath('anointed-sso.php')], 'anointed-sso-config');

        if ($this->app['config']->get('anointed-sso.routes.enabled') === true) {
            $this->registerRoutes();
        }
    }

    private function registerRoutes(): void
    {
        $prefix = (string) $this->app['config']->get('anointed-sso.routes.prefix');
        $web = (array) $this->app['config']->get('anointed-sso.routes.middleware');
        $backchannel = (array) $this->app['config']->get('anointed-sso.routes.backchannel_middleware');
        $controller = AnointedSsoController::class;

        Route::prefix($prefix)->middleware($web)->group(function () use ($controller): void {
            Route::get('start', [$controller, 'start'])->name('anointed-sso.start');
            Route::get('callback', [$controller, 'callback'])->name('anointed-sso.callback');
            Route::match(['GET', 'POST'], 'logout', [$controller, 'logout'])->name('anointed-sso.logout');
        });
        // No session and no CSRF check: the provider calls this server to server with a signed logout_token.
        Route::prefix($prefix)->middleware($backchannel)->group(function () use ($controller): void {
            Route::post('backchannel-logout', [$controller, 'backchannelLogout'])->name('anointed-sso.backchannel-logout');
        });
    }

    private function store($app): \Illuminate\Contracts\Cache\Repository
    {
        $name = $app['config']->get('anointed-sso.cache_store');
        return $app->make(CacheFactory::class)->store(is_string($name) && $name !== '' ? $name : null);
    }
}
