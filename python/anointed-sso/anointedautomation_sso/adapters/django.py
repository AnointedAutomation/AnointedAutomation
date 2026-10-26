# Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
"""Django adapter: views and URL patterns for the start, callback and back-channel logout routes.

    anointed = create_views(client, on_signed_in=..., on_logout=...)
    urlpatterns = [path("auth/anointed/", include(anointed.urls()))]
"""

from __future__ import annotations

from typing import Any, Callable, List, Optional

from django.http import HttpRequest, HttpResponse, HttpResponseRedirect
from django.urls import path
from django.views.decorators.csrf import csrf_exempt
from django.views.decorators.http import require_GET, require_POST

from ..backchannel import LOGOUT_TOKEN_FIELD, process_backchannel_logout
from ..client import AnointedClient, LogoutToken, SignInResult
from ..errors import AnointedSsoError
from ..replay import InMemoryLogoutReplayStore, LogoutReplayStore


class AnointedViews:
    """The three views plus ``urls()``."""

    def __init__(
        self,
        client: AnointedClient,
        on_signed_in: Callable[[HttpRequest, SignInResult], HttpResponse],
        on_logout: Callable[[LogoutToken], None],
        replay_store: Optional[LogoutReplayStore],
        on_error: Optional[Callable[[HttpRequest, AnointedSsoError], HttpResponse]],
        return_to_param: str,
    ) -> None:
        if not callable(on_signed_in) or not callable(on_logout):
            raise AnointedSsoError("config", "on_signed_in and on_logout are required callables")
        self._client = client
        self._on_signed_in = on_signed_in
        self._on_logout = on_logout
        self._store = InMemoryLogoutReplayStore() if replay_store is None else replay_store
        self._on_error = on_error
        self._return_to_param = return_to_param
        self.start = require_GET(self._start)
        self.callback = require_GET(self._callback)
        self.backchannel_logout = csrf_exempt(require_POST(self._backchannel_logout))

    def _apply_cookie(self, response: HttpResponse, value: str, options: dict) -> HttpResponse:
        response.set_cookie(
            self._client.cookie_name,
            value,
            max_age=int(options["max_age"]),
            path=options["path"],
            domain=options["domain"],
            secure=options["secure"],
            httponly=options["http_only"],
            samesite=options["same_site"],
        )
        response["Cache-Control"] = "no-store"
        return response

    def _start(self, request: HttpRequest) -> HttpResponse:
        auth = self._client.create_authorize_request(return_to=request.GET.get(self._return_to_param), request=request)
        return self._apply_cookie(HttpResponseRedirect(auth.url), auth.cookie_value, auth.cookie_options)

    def _callback(self, request: HttpRequest) -> HttpResponse:
        try:
            result = self._client.handle_callback(
                query={k: v for k, v in request.GET.items()},
                cookie_value=request.COOKIES.get(self._client.cookie_name),
                request=request,
            )
            response = self._on_signed_in(request, result)
        except AnointedSsoError as err:
            response = self._on_error(request, err) if self._on_error is not None else HttpResponse("Sign-in failed.", status=400)
        return self._apply_cookie(response, "", self._client.clear_flow_cookie_options())

    def _backchannel_logout(self, request: HttpRequest) -> HttpResponse:
        status = process_backchannel_logout(self._client, request.POST.getlist(LOGOUT_TOKEN_FIELD), self._on_logout, self._store)
        response = HttpResponse(status=status)
        response["Cache-Control"] = "no-store"
        return response

    def urls(self) -> List[Any]:
        """URL patterns: ``start/``, ``callback/`` and ``backchannel-logout/``."""
        return [
            path("start/", self.start, name="anointed-start"),
            path("callback/", self.callback, name="anointed-callback"),
            path("backchannel-logout/", self.backchannel_logout, name="anointed-backchannel-logout"),
        ]


def create_views(
    client: AnointedClient,
    *,
    on_signed_in: Callable[[HttpRequest, SignInResult], HttpResponse],
    on_logout: Callable[[LogoutToken], None],
    replay_store: Optional[LogoutReplayStore] = None,
    on_error: Optional[Callable[[HttpRequest, AnointedSsoError], HttpResponse]] = None,
    return_to_param: str = "returnTo",
) -> AnointedViews:
    """Build the views.

    :param on_signed_in: ``(request, result) -> response``. Find or create YOUR user by ``result.user.sub``,
        log them in (``django.contrib.auth.login``) and redirect to ``result.return_to``. The flow cookie
        is cleared for you.
    :param on_logout: End every local session for ``logout.sub`` (or ``logout.sid``).
    :param replay_store: Shared store for several processes; default is in-memory (one process).
    :param on_error: ``(request, error) -> response``. Default: a bare 400.
    """
    return AnointedViews(client, on_signed_in, on_logout, replay_store, on_error, return_to_param)
