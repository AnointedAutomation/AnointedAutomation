<?php
// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

declare(strict_types=1);

namespace AnointedAutomation\Sso\Laravel\Contracts;

use AnointedAutomation\Sso\SignInResult;
use Illuminate\Http\Request;
use Symfony\Component\HttpFoundation\Response;

/** Your code: find or create YOUR user by `$result->user->sub`, start YOUR session, return a redirect. */
interface SignedInHandler
{
    public function handle(SignInResult $result, Request $request): Response;
}
