<?php
// Copyright © Anointed Automation Corporation., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

declare(strict_types=1);

namespace AnointedAutomation\Sso\Laravel\Contracts;

use AnointedAutomation\Sso\LogoutToken;

/** Your code: end every session for `$logout->sub` (or `$logout->sid`). Repeated jti values never reach it. */
interface LogoutHandler
{
    public function handle(LogoutToken $logout): void;
}
