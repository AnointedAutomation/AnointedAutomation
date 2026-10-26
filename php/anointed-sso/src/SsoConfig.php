<?php
// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

declare(strict_types=1);

namespace AnointedAutomation\Sso;

/** Validated, immutable settings. A missing or empty required value throws; nothing is guessed. */
final class SsoConfig
{
    /** The production issuer. */
    public const ISSUER = 'https://api.anointedautomation.net/';

    /** `profile` is left out on purpose: asking for it makes users without a username pick one first. */
    public const DEFAULT_SCOPE = 'openid email';

    /** The member a back-channel Logout Token's `events` claim must carry. */
    public const BACKCHANNEL_LOGOUT_EVENT = 'http://schemas.openid.net/event/backchannel-logout';

    /** @var list<string> */
    public readonly array $scopes;
    public readonly bool $requireVerifiedEmail;
    public readonly ReturnToValidator $returnTo;

    /**
     * @param list<string> $allowedRedirectUris Optional allowlist for a per-call redirect URI override
     * @param list<string> $allowedReturnOrigins
     * @param (callable(string): bool)|null $isAllowedReturnTo
     */
    public function __construct(
        public readonly string $issuer,
        public readonly string $clientId,
        public readonly string $clientSecret,
        public readonly string $redirectUri,
        public readonly string $scope = self::DEFAULT_SCOPE,
        public readonly string $cookieName = 'aa_sso_flow',
        public readonly string $cookiePath = '/',
        public readonly ?string $cookieDomain = null,
        public readonly int $flowTtlSeconds = 600,
        public readonly int $clockSkewSeconds = 60,
        public readonly int $metadataTtlSeconds = 3600,
        ?bool $requireVerifiedEmail = null,
        public readonly array $allowedRedirectUris = [],
        array $allowedReturnOrigins = [],
        ?callable $isAllowedReturnTo = null,
    ) {
        foreach (['issuer' => $issuer, 'clientId' => $clientId, 'clientSecret' => $clientSecret, 'redirectUri' => $redirectUri] as $name => $value) {
            if (trim($value) === '') {
                throw new AnointedSsoException('config', "{$name} is required");
            }
        }
        self::assertSecureUrl($issuer, 'issuer', 'config');
        self::assertSecureUrl($redirectUri, 'redirectUri', 'config');
        foreach ($allowedRedirectUris as $uri) {
            self::assertSecureUrl((string) $uri, 'allowedRedirectUris entry', 'config');
        }
        if ($allowedRedirectUris !== [] && !in_array($redirectUri, $allowedRedirectUris, true)) {
            throw new AnointedSsoException('config', 'redirectUri is not in allowedRedirectUris');
        }
        $scopes = array_values(array_filter(explode(' ', $scope), static fn (string $s): bool => $s !== ''));
        if (!in_array('openid', $scopes, true)) {
            throw new AnointedSsoException('config', 'scope must include openid');
        }
        $this->scopes = $scopes;
        if ($cookieName === '' || preg_match('/^[!#$%&\'*+\-.^_`|~0-9A-Za-z]+$/', $cookieName) !== 1) {
            throw new AnointedSsoException('config', 'cookieName is not a valid cookie name');
        }
        if ($cookiePath === '' || $cookiePath[0] !== '/') {
            throw new AnointedSsoException('config', 'cookiePath must start with /');
        }
        foreach (['flowTtlSeconds' => $flowTtlSeconds, 'clockSkewSeconds' => $clockSkewSeconds, 'metadataTtlSeconds' => $metadataTtlSeconds] as $name => $value) {
            if ($value <= 0) {
                throw new AnointedSsoException('config', "{$name} must be a positive number");
            }
        }
        $this->requireVerifiedEmail = $requireVerifiedEmail ?? in_array('email', $scopes, true);
        $this->returnTo = new ReturnToValidator($allowedReturnOrigins, $isAllowedReturnTo);
    }

    /**
     * Build from an array with the constructor's names in snake_case (as the Laravel config file has them).
     * Absent or null keys use the constructor default; `issuer`, `client_id`, `client_secret` and `redirect_uri`
     * have none and throw when missing.
     *
     * @param array<string, mixed> $values
     */
    public static function fromArray(array $values): self
    {
        $map = [
            'issuer' => 'issuer', 'client_id' => 'clientId', 'client_secret' => 'clientSecret', 'redirect_uri' => 'redirectUri',
            'scope' => 'scope', 'cookie_name' => 'cookieName', 'cookie_path' => 'cookiePath', 'cookie_domain' => 'cookieDomain',
            'flow_ttl_seconds' => 'flowTtlSeconds', 'clock_skew_seconds' => 'clockSkewSeconds',
            'metadata_ttl_seconds' => 'metadataTtlSeconds', 'require_verified_email' => 'requireVerifiedEmail',
            'allowed_redirect_uris' => 'allowedRedirectUris', 'allowed_return_origins' => 'allowedReturnOrigins',
            'is_allowed_return_to' => 'isAllowedReturnTo',
        ];
        $args = [];
        foreach ($map as $key => $param) {
            if (array_key_exists($key, $values) && $values[$key] !== null) {
                $args[$param] = $values[$key];
            }
        }
        foreach (['issuer' => 'issuer', 'clientId' => 'clientId', 'clientSecret' => 'clientSecret', 'redirectUri' => 'redirectUri'] as $param) {
            if (!isset($args[$param]) || !is_string($args[$param])) {
                throw new AnointedSsoException('config', "{$param} is required");
            }
        }
        return new self(...$args);
    }

    public static function assertSecureUrl(string $value, string $label, string $errorCode): void
    {
        $parts = parse_url($value);
        if ($parts === false || !isset($parts['scheme'], $parts['host'])) {
            throw new AnointedSsoException($errorCode, "{$label} is not an absolute URL");
        }
        $scheme = strtolower($parts['scheme']);
        if ($scheme !== 'https' && !($scheme === 'http' && ReturnToValidator::isLoopbackHost($parts['host']))) {
            throw new AnointedSsoException($errorCode, "{$label} must use https (http is allowed only for localhost)");
        }
        if (isset($parts['fragment']) || str_contains($value, '#')) {
            throw new AnointedSsoException($errorCode, "{$label} must not contain a fragment");
        }
    }
}
