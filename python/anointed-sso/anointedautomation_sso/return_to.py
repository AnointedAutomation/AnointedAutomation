# Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
"""Open-redirect safe return targets.

By default only same-site absolute paths ("/account") are kept. A site can additionally trust absolute
URLs on origins it names (``allowed_return_origins``, e.g. ``https://*.example.com``) or decides itself
(``is_allowed_return_to``). Even then: https only (http only for a loopback host), no credentials, no
protocol-relative ``//host``, no javascript:/data: or any other scheme.
"""

from __future__ import annotations

import re
from typing import Callable, Iterable, List, NamedTuple, Optional
from urllib.parse import urlsplit

from .errors import AnointedSsoError

MAX_RETURN_TO_LENGTH = 2048
_UNSAFE = re.compile(r"[\x00-\x20\x7f\\]")
_CONTROL = re.compile(r"[\x00-\x1f\x7f\\]")
_ABSOLUTE = re.compile(r"^https?://[^/]", re.IGNORECASE)
_WILDCARD = re.compile(r"^(https?)://\*\.(.+)$", re.IGNORECASE)


def is_loopback_host(hostname: Optional[str]) -> bool:
    return hostname in ("localhost", "127.0.0.1", "::1")


def safe_relative_path(value: object) -> Optional[str]:
    """Keep a return path only when it is a same-site absolute path."""
    if not isinstance(value, str) or value == "" or len(value) > MAX_RETURN_TO_LENGTH:
        return None
    if not value.startswith("/") or value.startswith("//") or value.startswith("/\\"):
        return None
    if _CONTROL.search(value):
        return None
    return value


class _Matcher(NamedTuple):
    scheme: str
    host: Optional[str]
    suffix: Optional[str]
    port: Optional[int]


def _compile_origin(entry: object) -> _Matcher:
    if not isinstance(entry, str) or entry.strip() == "":
        raise AnointedSsoError("config", "allowed_return_origins entries must be non-empty strings")
    wildcard = _WILDCARD.match(entry)
    parseable = entry if wildcard is None else f"{wildcard.group(1)}://wildcard-placeholder.{wildcard.group(2)}"
    try:
        parts = urlsplit(parseable)
        port = parts.port
    except ValueError as err:
        raise AnointedSsoError("config", f"allowed_return_origins entry {entry} is not a URL origin") from err
    if parts.username or parts.password or parts.path not in ("", "/") or parts.query or parts.fragment:
        raise AnointedSsoError("config", f"allowed_return_origins entry {entry} must be a bare origin")
    scheme = parts.scheme.lower()
    host = (parts.hostname or "").lower()
    if scheme != "https" and not (scheme == "http" and wildcard is None and is_loopback_host(host)):
        raise AnointedSsoError("config", f"allowed_return_origins entry {entry} must use https (http only for localhost)")
    if wildcard is None:
        return _Matcher(scheme, host, None, port)
    base = host[len("wildcard-placeholder."):]
    if "." not in base or base.startswith(".") or "*" in base:
        raise AnointedSsoError("config", f"allowed_return_origins wildcard {entry} must name a domain like *.example.com")
    return _Matcher(scheme, None, f".{base}", port)


def create_return_to_validator(
    allowed_return_origins: Optional[Iterable[str]] = None,
    is_allowed_return_to: Optional[Callable[[str], bool]] = None,
) -> Callable[[object], Optional[str]]:
    """Build a validator (configuration checked now). ``is_allowed_return_to`` receives the normalized URL."""
    if is_allowed_return_to is not None and not callable(is_allowed_return_to):
        raise AnointedSsoError("config", "is_allowed_return_to must be callable")
    matchers: List[_Matcher] = [_compile_origin(e) for e in (allowed_return_origins or [])]
    absolute_allowed = len(matchers) > 0 or is_allowed_return_to is not None

    def listed(scheme: str, host: str, port: Optional[int]) -> bool:
        for m in matchers:
            if m.scheme != scheme or m.port != port:
                continue
            if m.host is not None:
                if m.host == host:
                    return True
            elif m.suffix is not None and len(host) > len(m.suffix) and host.endswith(m.suffix):
                return True
        return False

    def validate(value: object) -> Optional[str]:
        relative = safe_relative_path(value)
        if relative is not None or not absolute_allowed:
            return relative
        if not isinstance(value, str) or len(value) > MAX_RETURN_TO_LENGTH or _UNSAFE.search(value):
            return None
        if not _ABSOLUTE.match(value):
            return None
        try:
            parts = urlsplit(value)
            port = parts.port
        except ValueError:
            return None
        host = (parts.hostname or "").lower()
        scheme = parts.scheme.lower()
        if parts.username or parts.password or host == "":
            return None
        if scheme != "https" and not (scheme == "http" and is_loopback_host(host)):
            return None
        if listed(scheme, host, port):
            return value
        if is_allowed_return_to is not None and is_allowed_return_to(value) is True:
            return value
        return None

    return validate
