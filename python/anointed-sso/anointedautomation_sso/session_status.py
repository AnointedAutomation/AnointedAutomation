# Copyright © Anointed Automation Corporation., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
"""Session status: ask Anointed Automation whether a user's account or grant is still active.

Needs a partner API key with the ``sessions:status`` permission (ask Anointed Automation to add it). It
FAILS OPEN: any outage or odd answer is ``Unknown`` and ``should_end_session`` is False, so an outage never
signs your users out.
"""

from __future__ import annotations

import json
import logging
import threading
import time
from dataclasses import dataclass
from datetime import datetime, timezone
from enum import Enum
from typing import Any, Callable, Dict, Optional, Tuple
from urllib.parse import quote, urljoin

from .client import ANOINTED_ISSUER
from .errors import AnointedSsoError
from .http import HttpResponse, HttpTransport, urllib_transport

_log = logging.getLogger("anointedautomation_sso")

MACHINE_TOKEN_PATH = "api/auth/machine-token"
SESSION_STATUS_PATH = "api/session-status"


class SessionOutcome(Enum):
    UNKNOWN = "unknown"
    ACTIVE = "active"
    ENDED = "ended"


@dataclass(frozen=True)
class SessionCheck:
    outcome: SessionOutcome
    reason: Optional[str] = None
    error: Optional[str] = None

    @property
    def should_end_session(self) -> bool:
        return self.outcome is SessionOutcome.ENDED


class _BadAnswer(Exception):
    pass


def _find(obj: Dict[str, Any], name: str) -> Tuple[bool, Any]:
    for key, value in obj.items():
        if isinstance(key, str) and key.lower() == name:
            return True, value
    return False, None


def parse_status(body: str) -> SessionCheck:
    """Parse a session-status body (``{"data": {"active": bool, "reason": str}}``, any key casing)."""
    root = json.loads(body)
    if not isinstance(root, dict):
        raise _BadAnswer("session-status body is not a JSON object")
    found, success = _find(root, "success")
    if found and success is False:
        return SessionCheck(SessionOutcome.UNKNOWN, error="session-status answered success: false")
    found, data = _find(root, "data")
    if not found or not isinstance(data, dict):
        raise _BadAnswer("session-status body has no Data object")
    found, active = _find(data, "active")
    if not found or not isinstance(active, bool):
        raise _BadAnswer("session-status Data.active is not a boolean")
    _, reason = _find(data, "reason")
    reason = reason if isinstance(reason, str) else None
    return SessionCheck(SessionOutcome.ACTIVE if active else SessionOutcome.ENDED, reason=reason)


def _parse_machine_token(body: str) -> Tuple[str, float]:
    root = json.loads(body)
    found, data = _find(root, "data") if isinstance(root, dict) else (False, None)
    if not found or not isinstance(data, dict):
        raise _BadAnswer("machine-token body has no Data object")
    found, token = _find(data, "token")
    if not found or not isinstance(token, str) or token == "":
        raise _BadAnswer("machine-token body has no token")
    found, expires = _find(data, "expiresat")
    if not found or not isinstance(expires, str):
        raise _BadAnswer("machine-token body has no readable expiresAt")
    try:
        parsed = datetime.fromisoformat(expires.replace("Z", "+00:00"))
    except ValueError as err:
        raise _BadAnswer("machine-token body has no readable expiresAt") from err
    if parsed.tzinfo is None:
        parsed = parsed.replace(tzinfo=timezone.utc)
    return token, parsed.timestamp()


class SessionStatusClient:
    """Fail-open session status checks with a cached machine token and cached answers."""

    def __init__(
        self,
        *,
        api_key: str,
        app_name: str,
        base_url: str = ANOINTED_ISSUER,
        timeout_seconds: float = 3,
        answer_cache_seconds: float = 300,
        token_refresh_margin_seconds: float = 60,
        http: Optional[HttpTransport] = None,
        now: Optional[Callable[[], float]] = None,
    ) -> None:
        if not isinstance(api_key, str) or api_key.strip() == "":
            raise AnointedSsoError("config", "api_key is required. Read it from your server's secret store.")
        if not isinstance(app_name, str) or app_name.strip() == "":
            raise AnointedSsoError("config", "app_name is required")
        if not isinstance(base_url, str) or not base_url.startswith("https://"):
            raise AnointedSsoError("config", "base_url must be an absolute https URL")
        if not base_url.endswith("/"):
            raise AnointedSsoError("config", "base_url must end with a slash")
        if timeout_seconds <= 0:
            raise AnointedSsoError("config", "timeout_seconds must be positive")
        if answer_cache_seconds < 0 or token_refresh_margin_seconds < 0:
            raise AnointedSsoError("config", "answer_cache_seconds and token_refresh_margin_seconds must not be negative")
        self._api_key = api_key
        self._app_name = app_name
        self._base_url = base_url
        self._timeout = timeout_seconds
        self._cache_seconds = answer_cache_seconds
        self._margin = token_refresh_margin_seconds
        self._http: HttpTransport = urllib_transport if http is None else http
        self._now = time.time if now is None else now
        self._lock = threading.Lock()
        self._answers: Dict[str, Tuple[SessionCheck, float]] = {}
        self._token: Optional[str] = None
        self._token_refresh_at = 0.0

    def check(self, user_id: str, grant: bool = False) -> SessionCheck:
        """Whether to keep ``user_id`` (the ``sub`` claim) signed in. ``grant=True`` also checks the app grant."""
        if not isinstance(user_id, str) or user_id.strip() == "":
            raise ValueError("a user id (the sub claim) is required")
        key = ("1|" if grant else "0|") + user_id
        now = self._now()
        with self._lock:
            cached = self._answers.get(key)
            if cached is not None and now < cached[1]:
                return cached[0]
        try:
            answer = self._query(user_id, grant)
        except (OSError, ValueError, _BadAnswer, AnointedSsoError) as err:
            _log.warning("Anointed session status check failed; keeping the session (fail open): %s", err)
            return SessionCheck(SessionOutcome.UNKNOWN, error=f"{type(err).__name__}: {err}")
        if answer.outcome is SessionOutcome.UNKNOWN:
            _log.warning("Anointed session status gave no answer (%s); keeping the session (fail open).", answer.error)
            return answer
        with self._lock:
            now = self._now()
            for k in [k for k, (_, until) in self._answers.items() if until <= now]:
                del self._answers[k]
            self._answers[key] = (answer, now + self._cache_seconds)
        return answer

    def _query(self, user_id: str, grant: bool) -> SessionCheck:
        token = self._machine_token(None)
        response = self._send_status(user_id, grant, token)
        if response.status == 401:
            token = self._machine_token(token)
            response = self._send_status(user_id, grant, token)
        if response.status != 200:
            return SessionCheck(SessionOutcome.UNKNOWN, error=f"session-status answered HTTP {response.status}")
        return parse_status(response.body)

    def _send_status(self, user_id: str, grant: bool, token: str) -> HttpResponse:
        url = urljoin(self._base_url, f"{SESSION_STATUS_PATH}?userId={quote(user_id, safe='')}&grant={'true' if grant else 'false'}")
        return self._http("GET", url, {"authorization": f"Bearer {token}", "accept": "application/json"}, None, self._timeout)

    def _machine_token(self, rejected: Optional[str]) -> str:
        with self._lock:
            usable = self._token is not None and self._now() < self._token_refresh_at
            refused = rejected is not None and self._token == rejected
            if usable and not refused:
                assert self._token is not None
                return self._token
            payload = json.dumps({"apiKey": self._api_key, "name": self._app_name}).encode("utf-8")
            response = self._http(
                "POST",
                urljoin(self._base_url, MACHINE_TOKEN_PATH),
                {"content-type": "application/json", "accept": "application/json"},
                payload,
                self._timeout,
            )
            if response.status != 200:
                raise _BadAnswer(f"machine-token exchange answered HTTP {response.status}")
            token, expires_at = _parse_machine_token(response.body)
            self._token = token
            self._token_refresh_at = expires_at - self._margin
            return token
