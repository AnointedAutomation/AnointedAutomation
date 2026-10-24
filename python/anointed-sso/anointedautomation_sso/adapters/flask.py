# Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
"""Flask adapter: a blueprint with the start, callback and back-channel logout routes.

    anointed = create_blueprint(client, on_signed_in=..., on_logout=...)
    app.register_blueprint(anointed, url_prefix="/auth/anointed")
"""

from __future__ import annotations

from typing import Any, Callable, Optional

from flask import Blueprint, make_response, redirect, request

from ..backchannel import LOGOUT_TOKEN_FIELD, process_backchannel_logout
from ..client import AnointedClient, LogoutToken, SignInResult
from ..errors import AnointedSsoError
from ..replay import InMemoryLogoutReplayStore, LogoutReplayStore


def create_blueprint(
    client: AnointedClient,
    *,
    on_signed_in: Callable[[SignInResult], Any],
    on_logout: Callable[[LogoutToken], None],
    replay_store: Optional[LogoutReplayStore] = None,
    on_error: Optional[Callable[[AnointedSsoError], Any]] = None,
    return_to_param: str = "returnTo",
    name: str = "anointed",
) -> Blueprint:
    """Build the blueprint.

    :param on_signed_in: Find or create YOUR user by ``result.user.sub``, start YOUR session and return a
        response (usually a redirect to ``result.return_to``). The flow cookie is cleared for you.
    :param on_logout: End every local session for ``logout.sub`` (or ``logout.sid``).
    :param replay_store: Where seen ``jti`` values live. Pass a shared store when you run several
        processes; the in-memory store only covers one.
    :param on_error: Turn a failed sign-in into a response. Default: a bare 400.
    """
    if not callable(on_signed_in) or not callable(on_logout):
        raise AnointedSsoError("config", "on_signed_in and on_logout are required callables")
    store = InMemoryLogoutReplayStore() if replay_store is None else replay_store
    bp = Blueprint(name, __name__)

    def apply_cookie(response: Any, value: str, options: dict) -> Any:
        response.set_cookie(
            client.cookie_name,
            value,
            max_age=int(options["max_age"]),
            path=options["path"],
            domain=options["domain"],
            secure=options["secure"],
            httponly=options["http_only"],
            samesite=options["same_site"],
        )
        return response

    @bp.get("/start")
    def start() -> Any:
        auth = client.create_authorize_request(return_to=request.args.get(return_to_param), request=request)
        response = redirect(auth.url, code=302)
        response.headers["Cache-Control"] = "no-store"
        return apply_cookie(response, auth.cookie_value, auth.cookie_options)

    @bp.get("/callback")
    def callback() -> Any:
        try:
            result = client.handle_callback(query=request.args.to_dict(), cookie_value=request.cookies.get(client.cookie_name), request=request)
            response = make_response(on_signed_in(result))
        except AnointedSsoError as err:
            response = make_response(on_error(err) if on_error is not None else ("Sign-in failed.", 400))
        response.headers["Cache-Control"] = "no-store"
        return apply_cookie(response, "", client.clear_flow_cookie_options())

    @bp.post("/backchannel-logout")
    def backchannel_logout() -> Any:
        status = process_backchannel_logout(client, request.form.getlist(LOGOUT_TOKEN_FIELD), on_logout, store)
        response = make_response("", status)
        response.headers["Cache-Control"] = "no-store"
        return response

    return bp
