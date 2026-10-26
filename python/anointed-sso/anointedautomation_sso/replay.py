# Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
"""Back-channel logout replay protection: remember each ``jti`` until its token could no longer be accepted."""

from __future__ import annotations

import threading
import time
from typing import Callable, Dict, Optional


class LogoutReplayStore:
    """Interface. Implement it over Redis or a database when you run more than one process."""

    def try_add(self, jti: str, remember_until: float) -> bool:
        """Remember ``jti`` until the epoch second ``remember_until``. False when it was already remembered."""
        raise NotImplementedError

    def remove(self, jti: str) -> None:
        """Forget ``jti`` (used when the logout handler failed so the provider's retry is processed)."""
        raise NotImplementedError


class InMemoryLogoutReplayStore(LogoutReplayStore):
    """A thread-safe in-process store. Fine for a single process; use a shared store otherwise."""

    def __init__(self, now: Optional[Callable[[], float]] = None) -> None:
        self._now = time.time if now is None else now
        self._seen: Dict[str, float] = {}
        self._lock = threading.Lock()

    def try_add(self, jti: str, remember_until: float) -> bool:
        if not isinstance(jti, str) or jti == "":
            raise ValueError("a jti is required")
        now = self._now()
        with self._lock:
            for key in [k for k, until in self._seen.items() if until <= now]:
                del self._seen[key]
            if jti in self._seen:
                return False
            self._seen[jti] = remember_until
            return True

    def remove(self, jti: str) -> None:
        if not isinstance(jti, str) or jti == "":
            raise ValueError("a jti is required")
        with self._lock:
            self._seen.pop(jti, None)
