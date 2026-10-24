# anointedautomation/sso

Jesus is King ✝️

"Sign in with Anointed Automation" for PHP 8.1+ and Laravel. A server-side OpenID Connect client: authorization
code with PKCE (S256), `state`, `nonce`, RS256 ID token verification against the provider's JWKS, logout, back-channel
logout with replay protection, and a fail-open session status check. It is the PHP counterpart of
[`@anointedautomation/sso`](https://github.com/AnointedAutomation/AnointedAutomation/tree/master/js/anointed-sso).

Read the full partner guide first (portal setup, client id and secret, callback URLs, claims, security checklist):
[Sign in with Anointed Automation](https://github.com/AnointedAutomation/AnointedAutomation/blob/master/docs/SIGN_IN_WITH_ANOINTED_AUTOMATION.md).

SERVER ONLY. The client secret and the token exchange stay on your server. Never put them in a browser or app bundle.

## Install

```bash
composer require anointedautomation/sso
```

Needs `firebase/php-jwt`, `guzzlehttp/guzzle` and a PSR-18 client (Guzzle is used unless you pass your own).

Environment (no defaults: a missing value fails fast):

```
ANOINTED_OAUTH_CLIENT_ID=aa_<your key id>
ANOINTED_OAUTH_CLIENT_SECRET=<server only>
ANOINTED_OAUTH_REDIRECT_URI=https://yourapp.example/auth/anointed/callback
```

The redirect URI must be an https URL registered exactly on the partner portal Sign in tab (http is accepted only
for `localhost` here, but the provider cannot register it).

## Quick start: plain PHP

```php
<?php
use AnointedAutomation\Sso\Client;
use AnointedAutomation\Sso\SsoConfig;

require __DIR__ . '/vendor/autoload.php';

function requireEnv(string $name): string {
    $value = getenv($name);
    if ($value === false || trim($value) === '') {
        throw new RuntimeException("Missing required environment variable {$name}");
    }
    return $value;
}

$sso = new Client(new SsoConfig(
    issuer: SsoConfig::ISSUER,
    clientId: requireEnv('ANOINTED_OAUTH_CLIENT_ID'),
    clientSecret: requireEnv('ANOINTED_OAUTH_CLIENT_SECRET'),
    redirectUri: requireEnv('ANOINTED_OAUTH_REDIRECT_URI'),
    // scope: 'openid email',  // the default; add 'profile' or 'offline_access' only if you need them
));
```

`start.php`: send the user to Anointed Automation.

```php
$auth = $sso->createAuthorizeRequest(['returnTo' => $_GET['return_to'] ?? null]);
setcookie($auth->cookie->name, $auth->cookie->value, $auth->cookie->toSetcookieOptions());
header('Cache-Control: no-store');
header('Location: ' . $auth->url);
```

`callback.php` (the URL you registered): finish sign-in.

```php
try {
    $result = $sso->handleCallback($_GET, $_COOKIE[$sso->config()->cookieName] ?? null);
} catch (\AnointedAutomation\Sso\AnointedSsoException $e) {
    error_log('sso failed: ' . $e->errorCode);          // never show the raw provider error
    http_response_code(400);
    exit('Sign in with Anointed Automation failed. Please try again.');
}
$clear = $sso->clearFlowCookie();
setcookie($clear->name, '', $clear->toSetcookieOptions());

session_start();
session_regenerate_id(true);
// Find or create YOUR user by $result->user->sub (never by email), then log them in:
$_SESSION['user_id'] = yourUserIdFor($result->user->sub);
header('Location: ' . ($result->returnTo ?? '/'));
```

`backchannel-logout.php`: Anointed Automation calls this when the user signs out or disconnects your app.

```php
use AnointedAutomation\Sso\BackchannelLogoutHandler;

$handler = new BackchannelLogoutHandler($sso, function (\AnointedAutomation\Sso\LogoutToken $logout): void {
    // End every session for $logout->sub (or $logout->sid).
}, $replayStore); // see "Shared stores" below

http_response_code($handler->handle(file_get_contents('php://input')));
```

A repeated `jti` is answered `200` and never reaches your callback; if your callback throws, the answer is `500`
and the provider's retry is processed.

Logout: end your own session, then `header('Location: ' . $sso->buildLogoutUrl(['postLogoutRedirectUri' => 'https://yourapp.example/bye']))`.

## Quick start: Laravel (10, 11, 12)

The service provider is auto-discovered.

```bash
composer require anointedautomation/sso
php artisan vendor:publish --tag=anointed-sso-config
```

1. Put the three environment variables above in `.env`.
2. Write the two handlers (the kit never invents a login for you):

```php
use AnointedAutomation\Sso\Laravel\Contracts\SignedInHandler;
use AnointedAutomation\Sso\SignInResult;
use Illuminate\Http\Request;
use Illuminate\Support\Facades\Auth;
use Symfony\Component\HttpFoundation\Response;

final class AnointedSignedIn implements SignedInHandler
{
    public function handle(SignInResult $result, Request $request): Response
    {
        $user = User::firstOrCreate(['anointed_sub' => $result->user->sub], ['email' => $result->user->email /* ... */]);
        Auth::login($user);
        return redirect($result->returnTo ?? '/');
    }
}
```

```php
use AnointedAutomation\Sso\Laravel\Contracts\LogoutHandler;
use AnointedAutomation\Sso\LogoutToken;

final class AnointedLogout implements LogoutHandler
{
    public function handle(LogoutToken $logout): void
    {
        // End every session for $logout->sub (or $logout->sid), for example delete their rows from the sessions table.
    }
}
```

3. Name them in `config/anointed-sso.php`:

```php
'signed_in_handler' => App\Auth\AnointedSignedIn::class,
'logout_handler' => App\Auth\AnointedLogout::class,
```

Routes registered under `auth/anointed` (change `routes.prefix`, or set `routes.enabled` to `false` to register your own):

| Route | Name | Purpose |
|---|---|---|
| `GET /auth/anointed/start?return_to=/path` | `anointed-sso.start` | Redirects to Anointed Automation |
| `GET /auth/anointed/callback` | `anointed-sso.callback` | Verifies the ID token and calls your `SignedInHandler` |
| `GET\|POST /auth/anointed/logout` | `anointed-sso.logout` | Redirects to the provider's end-session URL (end your own session first) |
| `POST /auth/anointed/backchannel-logout` | `anointed-sso.backchannel-logout` | Verifies the logout token, blocks replays, calls your `LogoutHandler` |

Register `https://yourapp.example/auth/anointed/callback` and `https://yourapp.example/auth/anointed/backchannel-logout` on the partner portal.
Start and callback use the `web` middleware group (the flow secrets live in the Laravel session). The back-channel
route is deliberately outside it (no session, no CSRF check). Link with `<a href="{{ route('anointed-sso.start') }}">Sign in with Anointed Automation</a>`.

Set `ANOINTED_SSO_CACHE_STORE` to a shared store (redis, database) when you run more than one server, so discovery,
the JWKS and logout replay protection are shared.

## Shared stores

`Client` caches discovery and the JWKS through `MetadataCache`, and `BackchannelLogoutHandler` records logout token ids
through `LogoutReplayStore`. The defaults (`ArrayMetadataCache`, `InMemoryLogoutReplayStore`) live for one process, so with
PHP-FPM implement the two small interfaces on APCu, Redis or a database table. Laravel gets cache-backed ones for free.

## What it checks

- PKCE S256, a random `state` and `nonce` per sign-in, held in a short-lived `HttpOnly`, `SameSite=Lax` cookie (Laravel: the session).
- Callback: constant-time `state` compare, flow expiry, RFC 9207 `iss` check, provider errors become exceptions.
- ID token: RS256 only (`none` and HMAC are refused), signature by `kid` against the JWKS (refetched once on an unknown `kid`), exact `iss`, `aud`, `azp`, `exp`, `iat`, `nonce`, `sub`.
- A verified email is required when you request `email` (override with `requireVerifiedEmail`).
- Return targets are same-site paths, or origins you list in `allowedReturnOrigins` (open redirect protection).
- Logout token: `typ` `logout+jwt`, `iss`, `aud`, age, the back-channel event, no `nonce`, a `jti`, a `sub` or `sid`.
- No fallbacks: required settings throw when missing or blank.

## Other calls

```php
$sso->refresh($refreshToken, expectedSub: $sub);   // store the NEW refresh token it returns
$sso->revoke($refreshToken);
$sso->fetchUserInfo($accessToken);
```

Session status (optional, fail-open: only an explicit "ended" answer means end the session; your partner API key needs the
`sessions:status` permission, ask Anointed Automation to add it):

```php
$status = new \AnointedAutomation\Sso\SessionStatusClient(requireEnv('ANOINTED_API_KEY'), 'My App');
if ($status->check($sub)->shouldEndSession()) { /* end the local session */ }
```

In Laravel, resolve `SessionStatusClient` from the container after setting `ANOINTED_API_KEY` and `ANOINTED_APP_NAME`.

## Tests

```bash
composer install
vendor/bin/phpunit --testsuite core
# Laravel integration (needs PHP 8.2+ and testbench):
composer require --dev orchestra/testbench:^10.0 --with-all-dependencies && vendor/bin/phpunit --testsuite laravel
```

## Releases

Packagist reads this package's versions from git tags on the monorepo root (`composer.json` at the repo root is the manifest;
this folder's `composer.json` is for the kit's own tests and must stay in step). The Swift package also releases by plain semver tag
at the same root, so there is one tag stream: a Swift tag also becomes a PHP version, and a PHP release is cut by tagging.

MIT licensed. Source: [AnointedAutomation/AnointedAutomation](https://github.com/AnointedAutomation/AnointedAutomation/tree/master/php/anointed-sso).
