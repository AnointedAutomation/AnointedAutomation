# Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
"""Sign in with Anointed Automation for Python: a server-side OpenID Connect kit.

Framework-neutral core here; thin adapters live in ``anointedautomation_sso.adapters.{django,fastapi,flask}``.
"""

from __future__ import annotations

import os

from .backchannel import LOGOUT_TOKEN_FIELD, process_backchannel_logout
from .client import (
    ANOINTED_ISSUER,
    BACKCHANNEL_LOGOUT_EVENT,
    DEFAULT_SCOPE,
    AnointedClient,
    AnointedTokens,
    AnointedUser,
    AuthorizeRequest,
    LogoutToken,
    SignInResult,
    safe_return_to,
)
from .errors import AnointedSsoError
from .http import HttpResponse, HttpTransport
from .replay import InMemoryLogoutReplayStore, LogoutReplayStore
from .return_to import create_return_to_validator
from .session_status import SessionCheck, SessionOutcome, SessionStatusClient

__version__ = "0.1.0"


def require_env(name: str) -> str:
    """Read a required environment variable; raises ``AnointedSsoError`` (code ``config``) when unset or empty."""
    value = os.environ.get(name)
    if value is None or value.strip() == "":
        raise AnointedSsoError("config", f"Missing required environment variable {name}")
    return value


__all__ = [
    "ANOINTED_ISSUER",
    "BACKCHANNEL_LOGOUT_EVENT",
    "DEFAULT_SCOPE",
    "LOGOUT_TOKEN_FIELD",
    "AnointedClient",
    "AnointedSsoError",
    "AnointedTokens",
    "AnointedUser",
    "AuthorizeRequest",
    "HttpResponse",
    "HttpTransport",
    "InMemoryLogoutReplayStore",
    "LogoutReplayStore",
    "LogoutToken",
    "SessionCheck",
    "SessionOutcome",
    "SessionStatusClient",
    "SignInResult",
    "create_return_to_validator",
    "process_backchannel_logout",
    "require_env",
    "safe_return_to",
]
