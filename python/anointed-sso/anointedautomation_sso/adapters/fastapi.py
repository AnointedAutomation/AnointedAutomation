# Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
"""FastAPI / Starlette adapter: an ``APIRouter`` (FastAPI) or a list of Starlette ``Route`` objects.

The blocking core runs in a worker thread so the event loop is never blocked. Callbacks may be sync or async.

    app.include_router(create_router(client, on_signed_in=..., on_logout=...), prefix="/auth/anointed")
"""

from __future__ import annotations

import inspect
from typing import Any, Awaitable, Callable, List, Optional
from urllib.parse import parse_qs

import anyio.from_thread
from starlette.concurrency import run_in_threadpool
from starlette.requests import Request
from starlette.responses import PlainTextResponse, RedirectResponse, Response
from starlette.routing import Route

from ..backchannel import LOGOUT_TOKEN_FIELD, process_backchannel_logout
from ..client import AnointedClient, LogoutToken, SignInResult
from ..errors import AnointedSsoError
from ..replay import InMemoryLogoutReplayStore, LogoutReplayStore


async def _await(value: Awaitable[Any]) -> Any:
    return await value


def create_routes(
    client: AnointedClient,
    *,
    on_signed_in: Callable[[Request, SignInResult], Any],
    on_logout: Callable[[LogoutToken], Any],
    replay_store: Optional[LogoutReplayStore] = None,
    on_error: Optional[Callable[[Request, AnointedSsoError], Any]] = None,
    return_to_param: str = "returnTo",
) -> List[Route]:
    """Starlette routes ``/start``, ``/callback`` and ``/backchannel-logout`` (mount them under a prefix).

    :param on_signed_in: ``(request, result) -> Response`` (sync or async). Find or create YOUR user by
        ``result.user.sub``, start YOUR session and redirect to ``result.return_to``.
    :param on_logout: ``(logout) -> None`` (sync or async). End every local session for ``logout.sub``/``sid``.
    :param replay_store: Shared store for several processes; default is in-memory (one process).
    :param on_error: ``(request, error) -> Response``. Default: a bare 400.
    """
    if not callable(on_signed_in) or not callable(on_logout):
        raise AnointedSsoError("config", "on_signed_in and on_logout are required callables")
    store = InMemoryLogoutReplayStore() if replay_store is None else replay_store

    def apply_cookie(response: Response, value: str, options: dict) -> Response:
        response.set_cookie(
            client.cookie_name,
            value,
            max_age=int(options["max_age"]),
            path=options["path"],
            domain=options["domain"],
            secure=options["secure"],
            httponly=options["http_only"],
            samesite=options["same_site"].lower(),
        )
        response.headers["Cache-Control"] = "no-store"
        return response

    async def start(request: Request) -> Response:
        auth = await run_in_threadpool(
            lambda: client.create_authorize_request(return_to=request.query_params.get(return_to_param), request=request)
        )
        return apply_cookie(RedirectResponse(auth.url, status_code=302), auth.cookie_value, auth.cookie_options)

    async def callback(request: Request) -> Response:
        try:
            result = await run_in_threadpool(
                lambda: client.handle_callback(
                    query=dict(request.query_params), cookie_value=request.cookies.get(client.cookie_name), request=request
                )
            )
            response = on_signed_in(request, result)
            if inspect.isawaitable(response):
                response = await response
        except AnointedSsoError as err:
            if on_error is None:
                response = PlainTextResponse("Sign-in failed.", status_code=400)
            else:
                response = on_error(request, err)
                if inspect.isawaitable(response):
                    response = await response
        return apply_cookie(response, "", client.clear_flow_cookie_options())

    async def backchannel_logout(request: Request) -> Response:
        content_type = request.headers.get("content-type", "").split(";")[0].strip().lower()
        if content_type != "application/x-www-form-urlencoded":
            return Response(status_code=400, headers={"Cache-Control": "no-store"})
        form = parse_qs((await request.body()).decode("utf-8", errors="replace"), keep_blank_values=True)

        def sync_logout(logout: LogoutToken) -> None:
            outcome = on_logout(logout)
            if inspect.isawaitable(outcome):
                anyio.from_thread.run(_await, outcome)

        status = await run_in_threadpool(
            lambda: process_backchannel_logout(client, form.get(LOGOUT_TOKEN_FIELD, []), sync_logout, store)
        )
        return Response(status_code=status, headers={"Cache-Control": "no-store"})

    return [
        Route("/start", start, methods=["GET"]),
        Route("/callback", callback, methods=["GET"]),
        Route("/backchannel-logout", backchannel_logout, methods=["POST"]),
    ]


def create_router(client: AnointedClient, **kwargs: Any) -> Any:
    """A FastAPI ``APIRouter`` carrying the same routes (hidden from the OpenAPI schema)."""
    from fastapi import APIRouter

    router = APIRouter(include_in_schema=False)
    for route in create_routes(client, **kwargs):
        router.add_route(route.path, route.endpoint, methods=sorted(route.methods - {"HEAD"}), include_in_schema=False)
    return router
