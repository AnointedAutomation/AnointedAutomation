<?php
// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

// Every required value comes from the environment with NO default: a missing one fails at first use.
return [
    'issuer' => \AnointedAutomation\Sso\SsoConfig::ISSUER,
    'client_id' => env('ANOINTED_OAUTH_CLIENT_ID'),
    'client_secret' => env('ANOINTED_OAUTH_CLIENT_SECRET'),
    // Must match a callback URL registered in the partner portal exactly, for example https://yourapp.example/auth/anointed/callback
    'redirect_uri' => env('ANOINTED_OAUTH_REDIRECT_URI'),

    // Space separated. "openid email" (the kit default) when null. Add "profile" or "offline_access" only if needed.
    'scope' => env('ANOINTED_OAUTH_SCOPE'),

    // Origins a ?return_to= may point at besides same-site paths, for example ['https://*.example.com'].
    'allowed_return_origins' => [],

    // Where Anointed Automation sends people after sign out (register it on the Sign in tab). Optional.
    'post_logout_redirect_uri' => env('ANOINTED_OAUTH_POST_LOGOUT_REDIRECT_URI'),

    // Your classes (see the README): SignedInHandler and LogoutHandler. No defaults.
    'signed_in_handler' => null,
    'logout_handler' => null,

    // Laravel cache store for discovery, JWKS and logout replay protection. null = the default store.
    // Use a shared store (redis, database) when you run more than one server.
    'cache_store' => env('ANOINTED_SSO_CACHE_STORE'),

    'routes' => [
        'enabled' => true,
        'prefix' => 'auth/anointed',
        'middleware' => ['web'],
        // The provider calls this server to server: keep it OUT of the web group (no session, no CSRF).
        'backchannel_middleware' => [],
    ],

    // Optional session status check (SessionStatusClient). Your partner API key needs the sessions:status permission.
    'session_status' => [
        'api_key' => env('ANOINTED_API_KEY'),
        'app_name' => env('ANOINTED_APP_NAME'),
    ],
];
