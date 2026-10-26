<?php
// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

declare(strict_types=1);

namespace AnointedAutomation\Sso;

use Closure;
use Firebase\JWT\JWK;
use Firebase\JWT\JWT;
use GuzzleHttp\Psr7\Request;
use Psr\Http\Client\ClientExceptionInterface;
use Psr\Http\Client\ClientInterface;
use Throwable;

/**
 * "Sign in with Anointed Automation": a server-side OpenID Connect client (authorization code + PKCE S256,
 * state and nonce). SERVER ONLY: it holds the client secret and does the token exchange.
 */
final class Client
{
    private readonly ClientInterface $http;
    private readonly MetadataCache $cache;
    private readonly Closure $clock;

    /**
     * @param ClientInterface|null $http Any PSR-18 client; defaults to Guzzle
     * @param MetadataCache|null $cache Discovery and JWKS cache; defaults to a per-process array
     * @param (Closure(): int)|null $clock Seconds since the epoch (tests)
     */
    public function __construct(
        private readonly SsoConfig $config,
        ?ClientInterface $http = null,
        ?MetadataCache $cache = null,
        ?Closure $clock = null,
    ) {
        $this->http = $http ?? new \GuzzleHttp\Client(['timeout' => 10, 'http_errors' => false]);
        $this->cache = $cache ?? new ArrayMetadataCache();
        $this->clock = $clock ?? static fn (): int => time();
    }

    public function config(): SsoConfig
    {
        return $this->config;
    }

    /** @return array<string, mixed> */
    public function discover(bool $force = false): array
    {
        $key = 'anointed_sso.discovery.' . md5($this->config->issuer);
        if (!$force && ($hit = $this->cache->get($key)) !== null) {
            return $hit;
        }
        $doc = $this->requestJson('GET', rtrim($this->config->issuer, '/') . '/.well-known/openid-configuration', ['Accept' => 'application/json'], null, 'discovery');
        if (!is_array($doc)) {
            throw new AnointedSsoException('discovery', 'discovery document is not a JSON object');
        }
        if (($doc['issuer'] ?? null) !== $this->config->issuer) {
            throw new AnointedSsoException('discovery', 'discovery issuer does not match ' . $this->config->issuer);
        }
        foreach (['authorization_endpoint', 'token_endpoint', 'jwks_uri'] as $name) {
            if (!isset($doc[$name]) || !is_string($doc[$name])) {
                throw new AnointedSsoException('discovery', "discovery document has no {$name}");
            }
            SsoConfig::assertSecureUrl($doc[$name], $name, 'discovery');
        }
        if (isset($doc['code_challenge_methods_supported']) && is_array($doc['code_challenge_methods_supported'])
            && !in_array('S256', $doc['code_challenge_methods_supported'], true)) {
            throw new AnointedSsoException('discovery', 'provider does not advertise PKCE S256');
        }
        $this->cache->set($key, $doc, $this->config->metadataTtlSeconds);
        return $doc;
    }

    /** @return array<string, mixed> */
    public function getJwks(bool $force = false): array
    {
        $key = 'anointed_sso.jwks.' . md5($this->config->issuer);
        if (!$force && ($hit = $this->cache->get($key)) !== null) {
            return $hit;
        }
        $jwks = $this->requestJson('GET', (string) $this->discover()['jwks_uri'], ['Accept' => 'application/json'], null, 'http');
        if (!is_array($jwks) || !isset($jwks['keys']) || !is_array($jwks['keys'])) {
            throw new AnointedSsoException('http', 'JWKS has no keys array');
        }
        $this->cache->set($key, $jwks, $this->config->metadataTtlSeconds);
        return $jwks;
    }

    /**
     * Step 1: build the authorize URL and the flow cookie. Send the cookie and redirect to `$request->url`.
     *
     * @param array{returnTo?: mixed, prompt?: string, loginHint?: string, redirectUri?: string, extraParams?: array<string, scalar>} $options
     */
    public function createAuthorizeRequest(array $options = []): AuthorizeRequest
    {
        $redirectUri = $this->resolveRedirectUri($options['redirectUri'] ?? null);
        $doc = $this->discover();
        $state = Base64Url::random(32);
        $nonce = Base64Url::random(32);
        $verifier = Base64Url::random(48);

        $params = [
            'client_id' => $this->config->clientId,
            'redirect_uri' => $redirectUri,
            'response_type' => 'code',
            'scope' => implode(' ', $this->config->scopes),
            'state' => $state,
            'nonce' => $nonce,
            'code_challenge' => Base64Url::sha256($verifier),
            'code_challenge_method' => 'S256',
        ];
        if (isset($options['prompt'])) {
            $params['prompt'] = $options['prompt'];
        }
        if (isset($options['loginHint'])) {
            $params['login_hint'] = $options['loginHint'];
        }
        foreach ($options['extraParams'] ?? [] as $name => $value) {
            if (isset($params[$name])) {
                throw new AnointedSsoException('config', "extraParams may not set {$name}");
            }
            $params[$name] = (string) $value;
        }

        $flow = ['s' => $state, 'n' => $nonce, 'v' => $verifier, 't' => ($this->clock)()];
        $returnTo = $this->config->returnTo->validate($options['returnTo'] ?? null);
        if ($returnTo !== null) {
            $flow['r'] = $returnTo;
        }
        $cookie = new FlowCookie(
            $this->config->cookieName,
            Base64Url::encodeJson($flow),
            $this->config->cookiePath,
            $this->config->cookieDomain,
            str_starts_with($redirectUri, 'https:'),
            $this->config->flowTtlSeconds,
        );
        $endpoint = (string) $doc['authorization_endpoint'];
        $url = $endpoint . (str_contains($endpoint, '?') ? '&' : '?') . http_build_query($params, '', '&', PHP_QUERY_RFC3986);
        return new AuthorizeRequest($url, $state, $redirectUri, $cookie);
    }

    /** A cookie that deletes the flow cookie. */
    public function clearFlowCookie(): FlowCookie
    {
        return new FlowCookie($this->config->cookieName, '', $this->config->cookiePath, $this->config->cookieDomain, true, 0);
    }

    /**
     * Step 2: finish sign-in. Checks the flow cookie, state, issuer and error, exchanges the code, verifies the
     * ID token (signature, iss, aud, exp, iat, nonce) and returns the verified user.
     *
     * @param array<string, mixed> $query The callback's query parameters (`$_GET`)
     * @param string|null $cookieValue The flow cookie's value
     */
    public function handleCallback(array $query, ?string $cookieValue, ?string $redirectUri = null): SignInResult
    {
        $redirect = $this->resolveRedirectUri($redirectUri);
        $flow = $this->decodeFlowCookie($cookieValue);
        if ($flow === null) {
            throw new AnointedSsoException('flow_missing', 'the sign-in flow cookie is missing or malformed');
        }
        $param = static fn (string $name): ?string => isset($query[$name]) && is_string($query[$name]) ? $query[$name] : null;
        if (!Base64Url::equals($param('state'), $flow['state'])) {
            throw new AnointedSsoException('state_mismatch', 'state does not match the flow cookie');
        }
        if (($this->clock)() - $flow['createdAt'] > $this->config->flowTtlSeconds) {
            throw new AnointedSsoException('flow_expired', 'the sign-in flow expired');
        }
        $iss = $param('iss');
        if ($iss !== null && $iss !== $this->config->issuer) {
            throw new AnointedSsoException('issuer_mismatch', "callback iss {$iss} does not match " . $this->config->issuer);
        }
        $error = $param('error');
        if ($error !== null) {
            $description = $param('error_description');
            throw new AnointedSsoException($error === 'access_denied' ? 'access_denied' : 'provider_error', $description === null ? $error : "{$error}: {$description}", null, $error);
        }
        $code = $param('code');
        if ($code === null || $code === '') {
            throw new AnointedSsoException('missing_code', 'the callback has no code');
        }

        $raw = $this->postForm((string) $this->discover()['token_endpoint'], [
            'grant_type' => 'authorization_code',
            'code' => $code,
            'redirect_uri' => $redirect,
            'code_verifier' => $flow['verifier'],
        ], 'token_exchange');
        if (!is_array($raw) || !isset($raw['id_token']) || !is_string($raw['id_token'])) {
            throw new AnointedSsoException('token_exchange', 'the token response has no id_token');
        }
        $claims = $this->validateIdToken($raw['id_token'], $flow['nonce']);
        return new SignInResult($this->userFromClaims($claims), $claims, $this->tokens($raw), $flow['returnTo']);
    }

    /**
     * @return array{state: string, nonce: string, verifier: string, createdAt: int, returnTo: ?string}|null
     */
    public function decodeFlowCookie(?string $value): ?array
    {
        if ($value === null || $value === '') {
            return null;
        }
        try {
            $body = Base64Url::decodeJson($value);
        } catch (Throwable) {
            return null;
        }
        if (!isset($body['s'], $body['n'], $body['v'], $body['t']) || !is_string($body['s']) || !is_string($body['n'])
            || !is_string($body['v']) || !is_int($body['t']) || $body['s'] === '' || $body['n'] === '' || $body['v'] === '') {
            return null;
        }
        return [
            'state' => $body['s'], 'nonce' => $body['n'], 'verifier' => $body['v'], 'createdAt' => $body['t'],
            'returnTo' => $this->config->returnTo->validate($body['r'] ?? null),
        ];
    }

    /**
     * Trade a refresh token for new tokens. Store the new refresh token it returns.
     *
     * @return array{tokens: Tokens, claims: ?array<string, mixed>}
     */
    public function refresh(string $refreshToken, ?string $expectedSub = null): array
    {
        if ($refreshToken === '') {
            throw new AnointedSsoException('config', 'refreshToken is required');
        }
        $raw = $this->postForm((string) $this->discover()['token_endpoint'], ['grant_type' => 'refresh_token', 'refresh_token' => $refreshToken], 'token_exchange');
        if (!is_array($raw)) {
            throw new AnointedSsoException('token_exchange', 'the token response is not a JSON object');
        }
        $claims = null;
        if (isset($raw['id_token']) && is_string($raw['id_token'])) {
            $claims = $this->validateIdToken($raw['id_token'], null);
            if ($expectedSub !== null && $claims['sub'] !== $expectedSub) {
                throw new AnointedSsoException('id_token_invalid', 'refreshed id_token names a different sub');
            }
        }
        return ['tokens' => $this->tokens($raw), 'claims' => $claims];
    }

    public function revoke(string $token, string $tokenTypeHint = 'refresh_token'): void
    {
        $doc = $this->discover();
        if (!isset($doc['revocation_endpoint']) || !is_string($doc['revocation_endpoint'])) {
            throw new AnointedSsoException('discovery', 'provider has no revocation_endpoint');
        }
        $this->postForm($doc['revocation_endpoint'], ['token' => $token, 'token_type_hint' => $tokenTypeHint], 'http');
    }

    /** @return array<string, mixed> */
    public function fetchUserInfo(string $accessToken): array
    {
        $doc = $this->discover();
        if (!isset($doc['userinfo_endpoint']) || !is_string($doc['userinfo_endpoint'])) {
            throw new AnointedSsoException('discovery', 'provider has no userinfo_endpoint');
        }
        $info = $this->requestJson('GET', $doc['userinfo_endpoint'], ['Authorization' => 'Bearer ' . $accessToken, 'Accept' => 'application/json'], null, 'http');
        if (!is_array($info) || !isset($info['sub']) || !is_string($info['sub'])) {
            throw new AnointedSsoException('http', 'userinfo response has no sub');
        }
        return $info;
    }

    /**
     * The URL that ends the Anointed Automation session. End your own session first, then redirect here.
     *
     * @param array{idTokenHint?: string, postLogoutRedirectUri?: string, state?: string} $options
     */
    public function buildLogoutUrl(array $options = []): string
    {
        $doc = $this->discover();
        if (!isset($doc['end_session_endpoint']) || !is_string($doc['end_session_endpoint'])) {
            throw new AnointedSsoException('discovery', 'provider has no end_session_endpoint');
        }
        $params = ['client_id' => $this->config->clientId];
        if (isset($options['idTokenHint'])) {
            $params['id_token_hint'] = $options['idTokenHint'];
        }
        if (isset($options['postLogoutRedirectUri'])) {
            SsoConfig::assertSecureUrl($options['postLogoutRedirectUri'], 'postLogoutRedirectUri', 'config');
            $params['post_logout_redirect_uri'] = $options['postLogoutRedirectUri'];
        }
        if (isset($options['state'])) {
            $params['state'] = $options['state'];
        }
        $endpoint = $doc['end_session_endpoint'];
        return $endpoint . (str_contains($endpoint, '?') ? '&' : '?') . http_build_query($params, '', '&', PHP_QUERY_RFC3986);
    }

    /**
     * Verify a back-channel logout token: RS256 signature, `typ` logout+jwt, iss, aud, iat age, the
     * back-channel-logout event, no nonce, a jti, and a sub or sid.
     */
    public function verifyLogoutToken(string $logoutToken, int $maxAgeSeconds = 300): LogoutToken
    {
        try {
            [$header, $p] = $this->verifyWithJwks($logoutToken);
            if (($header['typ'] ?? null) !== 'logout+jwt') {
                throw new \UnexpectedValueException('typ is not logout+jwt');
            }
            if (($p['iss'] ?? null) !== $this->config->issuer) {
                throw new \UnexpectedValueException('iss does not match');
            }
            $this->assertAudience($p);
            $now = ($this->clock)();
            $skew = $this->config->clockSkewSeconds;
            if (!isset($p['iat']) || !is_int($p['iat'])) {
                throw new \UnexpectedValueException('iat is missing');
            }
            if ($p['iat'] - $skew > $now) {
                throw new \UnexpectedValueException('token was issued in the future');
            }
            if ($now - $p['iat'] > $maxAgeSeconds + $skew) {
                throw new \UnexpectedValueException('token is too old');
            }
            if (isset($p['exp']) && (!is_int($p['exp']) || $now > $p['exp'] + $skew)) {
                throw new \UnexpectedValueException('token is expired');
            }
            $events = $p['events'] ?? null;
            if (!is_array($events) || !isset($events[SsoConfig::BACKCHANNEL_LOGOUT_EVENT]) || !is_array($events[SsoConfig::BACKCHANNEL_LOGOUT_EVENT])) {
                throw new \UnexpectedValueException('events does not carry the back-channel logout event');
            }
            if (array_key_exists('nonce', $p)) {
                throw new \UnexpectedValueException('a logout token must not carry a nonce');
            }
            if (!isset($p['jti']) || !is_string($p['jti']) || $p['jti'] === '') {
                throw new \UnexpectedValueException('jti is missing');
            }
            $sub = self::stringOrNull($p['sub'] ?? null);
            $sid = self::stringOrNull($p['sid'] ?? null);
            if ($sub === null && $sid === null) {
                throw new \UnexpectedValueException('neither sub nor sid is present');
            }
            return new LogoutToken($sub, $sid, $p['jti'], isset($p['exp']) ? $p['exp'] : null, $p);
        } catch (AnointedSsoException $e) {
            throw $e;
        } catch (Throwable $e) {
            throw new AnointedSsoException('logout_token_invalid', 'logout token rejected: ' . $e->getMessage(), null, null, $e);
        }
    }

    // ---------------------------------------------------------------- internals

    private function resolveRedirectUri(?string $override): string
    {
        if ($override === null) {
            return $this->config->redirectUri;
        }
        SsoConfig::assertSecureUrl($override, 'redirectUri', 'redirect_uri_rejected');
        if ($override !== $this->config->redirectUri && !in_array($override, $this->config->allowedRedirectUris, true)) {
            throw new AnointedSsoException('redirect_uri_rejected', 'redirectUri is not in allowedRedirectUris');
        }
        return $override;
    }

    /**
     * @return array<string, mixed>
     */
    private function validateIdToken(string $idToken, ?string $nonce): array
    {
        try {
            [, $c] = $this->verifyWithJwks($idToken);
            if (($c['iss'] ?? null) !== $this->config->issuer) {
                throw new \UnexpectedValueException('iss does not match');
            }
            $this->assertAudience($c);
            $now = ($this->clock)();
            $skew = $this->config->clockSkewSeconds;
            if (!isset($c['exp']) || !is_int($c['exp'])) {
                throw new \UnexpectedValueException('exp is missing');
            }
            if ($now > $c['exp'] + $skew) {
                throw new \UnexpectedValueException('token is expired');
            }
            if (!isset($c['iat']) || !is_int($c['iat'])) {
                throw new \UnexpectedValueException('iat is missing');
            }
            if ($c['iat'] - $skew > $now) {
                throw new \UnexpectedValueException('token was issued in the future');
            }
            if ($nonce !== null && !Base64Url::equals($c['nonce'] ?? null, $nonce)) {
                throw new \UnexpectedValueException('nonce does not match');
            }
            if (!isset($c['sub']) || !is_string($c['sub']) || $c['sub'] === '') {
                throw new \UnexpectedValueException('sub is missing');
            }
            return $c;
        } catch (AnointedSsoException $e) {
            throw $e;
        } catch (Throwable $e) {
            throw new AnointedSsoException('id_token_invalid', 'id_token rejected: ' . $e->getMessage(), null, null, $e);
        }
    }

    /** @param array<string, mixed> $claims */
    private function assertAudience(array $claims): void
    {
        $aud = $claims['aud'] ?? null;
        $audiences = is_array($aud) ? $aud : [$aud];
        if (!in_array($this->config->clientId, $audiences, true)) {
            throw new \UnexpectedValueException('aud does not include the client_id');
        }
        if (count($audiences) > 1 && ($claims['azp'] ?? null) !== $this->config->clientId) {
            throw new \UnexpectedValueException('azp does not name the client_id');
        }
        if (array_key_exists('azp', $claims) && $claims['azp'] !== $this->config->clientId) {
            throw new \UnexpectedValueException('azp does not name the client_id');
        }
    }

    /**
     * Verify an RS256 JWS against the cached JWKS, refreshing the JWKS once when the kid is unknown.
     *
     * @return array{0: array<string, mixed>, 1: array<string, mixed>} header and payload
     */
    private function verifyWithJwks(string $token): array
    {
        $segments = explode('.', $token);
        if (count($segments) !== 3) {
            throw new \UnexpectedValueException('token is not a compact JWS');
        }
        try {
            $header = Base64Url::decodeJson($segments[0]);
        } catch (Throwable) {
            throw new \UnexpectedValueException('token header is malformed');
        }
        if (($header['alg'] ?? null) !== 'RS256') {
            throw new \UnexpectedValueException('alg is not RS256');
        }
        $jwks = $this->getJwks();
        if (!$this->hasKey($jwks, $header)) {
            $jwks = $this->getJwks(true);
            if (!$this->hasKey($jwks, $header)) {
                throw new \UnexpectedValueException('kid is not in the JWKS');
            }
        }
        $keys = JWK::parseKeySet(['keys' => $this->rsaKeys($jwks)], 'RS256');
        $keyArg = isset($header['kid']) ? $keys : reset($keys);

        $previousTimestamp = JWT::$timestamp;
        $previousLeeway = JWT::$leeway;
        JWT::$timestamp = ($this->clock)();
        JWT::$leeway = $this->config->clockSkewSeconds;
        try {
            $decoded = JWT::decode($token, $keyArg);
        } finally {
            JWT::$timestamp = $previousTimestamp;
            JWT::$leeway = $previousLeeway;
        }
        $payload = json_decode(json_encode($decoded, JSON_THROW_ON_ERROR), true, 32, JSON_THROW_ON_ERROR);
        return [$header, is_array($payload) ? $payload : []];
    }

    /**
     * @param array<string, mixed> $jwks
     * @return list<array<string, mixed>>
     */
    private function rsaKeys(array $jwks): array
    {
        $keys = [];
        foreach ($jwks['keys'] as $jwk) {
            if (is_array($jwk) && ($jwk['kty'] ?? null) === 'RSA' && isset($jwk['n'], $jwk['e'])
                && (($jwk['use'] ?? 'sig') === 'sig') && (!isset($jwk['alg']) || $jwk['alg'] === 'RS256')) {
                $keys[] = $jwk;
            }
        }
        return $keys;
    }

    /**
     * @param array<string, mixed> $jwks
     * @param array<string, mixed> $header
     */
    private function hasKey(array $jwks, array $header): bool
    {
        $keys = $this->rsaKeys($jwks);
        if (isset($header['kid'])) {
            foreach ($keys as $jwk) {
                if (($jwk['kid'] ?? null) === $header['kid']) {
                    return true;
                }
            }
            return false;
        }
        return count($keys) === 1;
    }

    /** @param array<string, mixed> $claims */
    private function userFromClaims(array $claims): User
    {
        $email = null;
        if (($claims['email_verified'] ?? null) === true && isset($claims['email']) && is_string($claims['email']) && trim($claims['email']) !== '') {
            $email = strtolower(trim($claims['email']));
        }
        if ($this->config->requireVerifiedEmail && $email === null) {
            throw new AnointedSsoException('email_not_verified', 'the ID token carries no verified email');
        }
        return new User(
            (string) $claims['sub'],
            $email,
            $email !== null,
            self::stringOrNull($claims['name'] ?? null),
            self::stringOrNull($claims['preferred_username'] ?? null),
            self::stringOrNull($claims['given_name'] ?? null),
            self::stringOrNull($claims['family_name'] ?? null),
            self::stringOrNull($claims['picture'] ?? null),
            self::stringOrNull($claims['sid'] ?? null),
        );
    }

    /** @param array<string, mixed> $raw */
    private function tokens(array $raw): Tokens
    {
        return new Tokens(
            self::stringOrNull($raw['access_token'] ?? null),
            self::stringOrNull($raw['id_token'] ?? null),
            self::stringOrNull($raw['refresh_token'] ?? null),
            self::stringOrNull($raw['token_type'] ?? null),
            self::stringOrNull($raw['scope'] ?? null),
            isset($raw['expires_in']) && is_int($raw['expires_in']) ? ($this->clock)() + $raw['expires_in'] : null,
        );
    }

    private static function stringOrNull(mixed $value): ?string
    {
        return is_string($value) && $value !== '' ? $value : null;
    }

    /**
     * @param array<string, string> $fields
     * @return array<string, mixed>|null
     */
    private function postForm(string $url, array $fields, string $failCode): ?array
    {
        $fields['client_id'] = $this->config->clientId;
        $fields['client_secret'] = $this->config->clientSecret;
        return $this->requestJson('POST', $url, ['Content-Type' => 'application/x-www-form-urlencoded', 'Accept' => 'application/json'], http_build_query($fields, '', '&', PHP_QUERY_RFC3986), $failCode);
    }

    /**
     * @param array<string, string> $headers
     * @return array<string, mixed>|null
     */
    private function requestJson(string $method, string $url, array $headers, ?string $body, string $failCode): ?array
    {
        try {
            $response = $this->http->sendRequest(new Request($method, $url, $headers, $body));
        } catch (ClientExceptionInterface $e) {
            throw new AnointedSsoException($failCode, "{$url} request failed: " . $e->getMessage(), null, null, $e);
        }
        $text = (string) $response->getBody();
        $data = null;
        if ($text !== '') {
            $decoded = json_decode($text, true);
            $data = is_array($decoded) ? $decoded : null;
        }
        $status = $response->getStatusCode();
        if ($status < 200 || $status >= 300) {
            $oauthError = is_array($data) && isset($data['error']) && is_string($data['error']) ? $data['error'] : null;
            throw new AnointedSsoException($failCode, "{$url} -> {$status}" . ($oauthError === null ? '' : " {$oauthError}"), $status, $oauthError);
        }
        return $data;
    }
}
