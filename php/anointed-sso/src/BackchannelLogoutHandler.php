<?php
// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

declare(strict_types=1);

namespace AnointedAutomation\Sso;

use Closure;
use Throwable;

/**
 * Framework-neutral back-channel logout endpoint logic. Pass the raw POST body; it returns the HTTP status to
 * answer with. A repeated `jti` is answered 200 without calling your callback again (replay protection).
 */
final class BackchannelLogoutHandler
{
    /** @var Closure(LogoutToken): void */
    private readonly Closure $onLogout;

    /** @param callable(LogoutToken): void $onLogout End every session for `$logout->sub` (or `$logout->sid`). */
    public function __construct(
        private readonly Client $client,
        callable $onLogout,
        private readonly LogoutReplayStore $replayStore,
    ) {
        $this->onLogout = Closure::fromCallable($onLogout);
    }

    /** @param string $body The raw `application/x-www-form-urlencoded` request body */
    public function handle(string $body): int
    {
        parse_str($body, $form);
        $logoutToken = $form['logout_token'] ?? null;
        if (!is_string($logoutToken) || $logoutToken === '') {
            return 400;
        }
        try {
            $verified = $this->client->verifyLogoutToken($logoutToken);
        } catch (AnointedSsoException $e) {
            return $e->errorCode === 'logout_token_invalid' ? 400 : 500;
        } catch (Throwable) {
            return 500;
        }
        $skew = $this->client->config()->clockSkewSeconds;
        if (!$this->replayStore->markSeen($verified->jti, 300 + 2 * $skew)) {
            return 200;
        }
        try {
            ($this->onLogout)($verified);
        } catch (Throwable) {
            $this->replayStore->forget($verified->jti);
            return 500;
        }
        return 200;
    }
}
