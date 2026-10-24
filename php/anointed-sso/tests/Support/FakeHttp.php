<?php
// Copyright © Anointed Automation Corporation., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

declare(strict_types=1);

namespace AnointedAutomation\Sso\Tests\Support;

use GuzzleHttp\Psr7\Response;
use Psr\Http\Client\ClientInterface;
use Psr\Http\Message\RequestInterface;
use Psr\Http\Message\ResponseInterface;

/** A PSR-18 client that answers by "METHOD url" and records every request. */
final class FakeHttp implements ClientInterface
{
    /** @var array<string, Response> */
    public array $routes = [];
    /** @var list<RequestInterface> */
    public array $requests = [];

    /** @param array<string, mixed>|string $body */
    public function route(string $method, string $url, array|string $body, int $status = 200): void
    {
        $this->routes["{$method} {$url}"] = new Response($status, ['Content-Type' => 'application/json'], is_string($body) ? $body : json_encode($body));
    }

    public function sendRequest(RequestInterface $request): ResponseInterface
    {
        $this->requests[] = $request;
        $key = $request->getMethod() . ' ' . (string) $request->getUri();
        foreach ($this->routes as $pattern => $response) {
            if ($pattern === $key || str_starts_with($key, $pattern . '?')) {
                return $response;
            }
        }
        return new Response(404, [], '{}');
    }

    public function count(string $method, string $urlPrefix): int
    {
        return count(array_filter($this->requests, static fn (RequestInterface $r): bool => $r->getMethod() === $method && str_starts_with((string) $r->getUri(), $urlPrefix)));
    }
}
