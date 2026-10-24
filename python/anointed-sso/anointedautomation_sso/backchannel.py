# Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
"""Framework-neutral back-channel logout handling (validation, replay protection, status code)."""

from __future__ import annotations

import logging
from typing import Callable, Sequence

from .client import AnointedClient, LogoutToken
from .errors import AnointedSsoError
from .replay import LogoutReplayStore

_log = logging.getLogger("anointedautomation_sso")

#: The form field the provider posts the Logout Token in.
LOGOUT_TOKEN_FIELD = "logout_token"


def process_backchannel_logout(
    client: AnointedClient,
    logout_token_values: Sequence[str],
    on_logout: Callable[[LogoutToken], None],
    replay_store: LogoutReplayStore,
) -> int:
    """Handle one back-channel logout POST and return the HTTP status to answer with.

    - 400 when ``logout_token`` is missing, repeated, or fails validation.
    - 500 when the provider keys cannot be read or ``on_logout`` raised (the provider retries; the ``jti``
      is forgotten first so the retry is processed).
    - 200 for a ``jti`` already processed (``on_logout`` is not called again) and after a successful run.

    Always send ``Cache-Control: no-store``. ``on_logout`` should end every local session for
    ``logout.sub`` (or ``logout.sid``).
    """
    if len(logout_token_values) != 1 or not isinstance(logout_token_values[0], str) or logout_token_values[0].strip() == "":
        _log.warning("back-channel logout rejected: logout_token is missing or repeated")
        return 400
    try:
        logout = client.verify_logout_token(logout_token_values[0])
    except AnointedSsoError as err:
        if err.code == "logout_token_invalid":
            _log.warning("back-channel logout rejected: %s", err)
            return 400
        _log.error("back-channel logout could not be validated (provider keys unavailable): %s", err)
        return 500
    remember_until = (logout.exp if logout.exp is not None else client.now()) + client.clock_skew_seconds
    if not replay_store.try_add(logout.jti, remember_until):
        _log.info("back-channel logout %s was already processed", logout.jti)
        return 200
    try:
        on_logout(logout)
    except Exception:  # noqa: BLE001 - any handler failure must be retried by the provider
        replay_store.remove(logout.jti)
        _log.exception("back-channel logout %s handler failed; answering 500 so it is retried", logout.jti)
        return 500
    return 200
