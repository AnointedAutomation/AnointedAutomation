<?php
// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

declare(strict_types=1);

namespace AnointedAutomation\Sso;

/**
 * The short-lived httpOnly cookie that carries state, nonce, the PKCE verifier and an optional return target
 * between the start and callback requests. No server session store is needed.
 */
final class FlowCookie
{
    public function __construct(
        public readonly string $name,
        public readonly string $value,
        public readonly string $path,
        public readonly ?string $domain,
        public readonly bool $secure,
        public readonly int $maxAge,
    ) {
    }

    /**
     * Options for PHP's `setcookie($name, $value, $options)`.
     *
     * @return array{expires: int, path: string, domain?: string, secure: bool, httponly: bool, samesite: string}
     */
    public function toSetcookieOptions(): array
    {
        $options = [
            'expires' => $this->maxAge === 0 ? 1 : time() + $this->maxAge,
            'path' => $this->path,
            'secure' => $this->secure,
            'httponly' => true,
            'samesite' => 'Lax',
        ];
        if ($this->domain !== null) {
            $options['domain'] = $this->domain;
        }
        return $options;
    }

    /** A complete `Set-Cookie` header value. */
    public function toHeader(): string
    {
        $header = $this->name . '=' . $this->value . '; Max-Age=' . $this->maxAge . '; Path=' . $this->path;
        if ($this->domain !== null) {
            $header .= '; Domain=' . $this->domain;
        }
        if ($this->secure) {
            $header .= '; Secure';
        }
        return $header . '; HttpOnly; SameSite=Lax';
    }
}
