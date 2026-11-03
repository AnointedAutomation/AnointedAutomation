<?php
// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

declare(strict_types=1);

namespace AnointedAutomation\Sso;

use Closure;
use Psr\Log\LoggerInterface;
use Throwable;

/**
 * Framework-neutral back-channel logout endpoint logic. Pass the raw POST body; it returns the HTTP status to
 * answer with. A repeated `jti` is answered 200 without calling your callback again (replay protection).
 * Every rejection or failure is written to the PSR-3 logger: warning for an invalid token, error (with the
 * exception) when the provider keys could not be read or your callback threw and the session was NOT ended.
 */
final class BackchannelLogoutHandler
{
    /** @var Closure(LogoutToken): void */
    private readonly Closure $onLogout;

    /**
     * @param callable(LogoutToken): void $onLogout End every session for `$logout->sub` (or `$logout->sid`).
     * @param LoggerInterface $logger Receives every rejected or failed logout (PSR-3, for example Laravel's or Monolog).
     */
    public function __construct(
        private readonly Client $client,
        callable $onLogout,
        private readonly LogoutReplayStore $replayStore,
        private readonly LoggerInterface $logger,
    ) {
        $this->onLogout = Closure::fromCallable($onLogout);
    }

    /** @param string $body The raw `application/x-www-form-urlencoded` request body */
    public function handle(string $body): int
    {
        parse_str($body, $form);
        $logoutToken = $form['logout_token'] ?? null;
        if (!is_string($logoutToken) || $logoutToken === '') {
            $this->logger->warning('Anointed SSO back-channel logout rejected: logout_token is missing or not a single value');
            return 400;
        }
        try {
            $verified = $this->client->verifyLogoutToken($logoutToken);
        } catch (AnointedSsoException $e) {
            if ($e->errorCode === 'logout_token_invalid') {
                $this->logger->warning('Anointed SSO back-channel logout rejected: invalid logout token', ['code' => $e->errorCode, 'exception' => $e]);
                return 400;
            }
            $this->logger->error('Anointed SSO back-channel logout could not be validated (provider keys unavailable)', ['code' => $e->errorCode, 'exception' => $e]);
            return 500;
        } catch (Throwable $e) {
            $this->logger->error('Anointed SSO back-channel logout validation failed unexpectedly', ['exception' => $e]);
            return 500;
        }
        $skew = $this->client->config()->clockSkewSeconds;
        if (!$this->replayStore->markSeen($verified->jti, 300 + 2 * $skew)) {
            return 200;
        }
        try {
            ($this->onLogout)($verified);
        } catch (Throwable $e) {
            $this->logger->error('Anointed SSO back-channel logout handler failed; the session was NOT ended and the logout can be retried', ['jti' => $verified->jti, 'sub' => $verified->sub, 'exception' => $e]);
            $this->replayStore->forget($verified->jti);
            return 500;
        }
        return 200;
    }
}
