<?php
// Copyright © Anointed Automation Corporation., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

declare(strict_types=1);

namespace AnointedAutomation\Sso;

/** Where to send the browser, plus the cookie to set on that same response. */
final class AuthorizeRequest
{
    public function __construct(
        public readonly string $url,
        public readonly string $state,
        public readonly string $redirectUri,
        public readonly FlowCookie $cookie,
    ) {
    }
}
