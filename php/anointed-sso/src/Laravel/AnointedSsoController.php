<?php
// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

declare(strict_types=1);

namespace AnointedAutomation\Sso\Laravel;

use AnointedAutomation\Sso\AnointedSsoException;
use AnointedAutomation\Sso\BackchannelLogoutHandler;
use AnointedAutomation\Sso\Client;
use AnointedAutomation\Sso\Laravel\Contracts\LogoutHandler;
use AnointedAutomation\Sso\Laravel\Contracts\SignedInHandler;
use AnointedAutomation\Sso\LogoutReplayStore;
use Illuminate\Http\Request;
use Illuminate\Routing\Controller;
use Illuminate\Support\Facades\Log;
use RuntimeException;
use Symfony\Component\HttpFoundation\Response;

/**
 * The four routes. The flow secrets (state, nonce, PKCE verifier) live in the Laravel session, so start and
 * callback need the `web` middleware group (the config default).
 */
final class AnointedSsoController extends Controller
{
    private const SESSION_KEY = 'anointed_sso.flow';

    public function start(Request $request, Client $client): Response
    {
        $authorize = $client->createAuthorizeRequest(['returnTo' => $request->query('return_to')]);
        $request->session()->put(self::SESSION_KEY, $authorize->cookie->value);
        return redirect()->away($authorize->url)->header('Cache-Control', 'no-store');
    }

    public function callback(Request $request, Client $client): Response
    {
        $flow = $request->session()->pull(self::SESSION_KEY);
        try {
            $result = $client->handleCallback($request->query(), is_string($flow) ? $flow : null);
        } catch (AnointedSsoException $e) {
            Log::warning('Sign in with Anointed Automation failed', ['code' => $e->errorCode]);
            return response('Sign in with Anointed Automation failed. Please try again.', 400)->header('Cache-Control', 'no-store');
        }
        $request->session()->regenerate();
        return $this->handler('signed_in_handler', SignedInHandler::class)->handle($result, $request);
    }

    public function logout(Request $request, Client $client): Response
    {
        $options = [];
        $hint = $request->input('id_token_hint');
        if (is_string($hint) && $hint !== '') {
            $options['idTokenHint'] = $hint;
        }
        $after = config('anointed-sso.post_logout_redirect_uri');
        if (is_string($after) && $after !== '') {
            $options['postLogoutRedirectUri'] = $after;
        }
        return redirect()->away($client->buildLogoutUrl($options))->header('Cache-Control', 'no-store');
    }

    public function backchannelLogout(Request $request, Client $client, LogoutReplayStore $replay): Response
    {
        $handler = $this->handler('logout_handler', LogoutHandler::class);
        $status = (new BackchannelLogoutHandler($client, [$handler, 'handle'], $replay))->handle($request->getContent());
        return response('', $status)->header('Cache-Control', 'no-store');
    }

    /**
     * @template T of object
     * @param class-string<T> $contract
     * @return T
     */
    private function handler(string $key, string $contract): object
    {
        $class = config('anointed-sso.' . $key);
        if (!is_string($class) || $class === '') {
            throw new RuntimeException("anointed-sso.{$key} must name a class implementing {$contract}");
        }
        $handler = app($class);
        if (!$handler instanceof $contract) {
            throw new RuntimeException("{$class} must implement {$contract}");
        }
        return $handler;
    }
}
