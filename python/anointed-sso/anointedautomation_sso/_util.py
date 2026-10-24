# Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
"""Small shared helpers: base64url, constant-time compare, random tokens, cookies."""

from __future__ import annotations

import base64
import hashlib
import hmac
import json
import re
import secrets
from typing import Any, Dict, Optional

_COOKIE_NAME = re.compile(r"^[!#$%&'*+\-.^_`|~0-9A-Za-z]+$")


def b64url_encode(raw: bytes) -> str:
    return base64.urlsafe_b64encode(raw).rstrip(b"=").decode("ascii")


def b64url_decode(text: str) -> bytes:
    return base64.urlsafe_b64decode(text + "=" * (-len(text) % 4))


def json_to_b64url(value: Any) -> str:
    return b64url_encode(json.dumps(value, separators=(",", ":")).encode("utf-8"))


def b64url_to_json(text: str) -> Any:
    return json.loads(b64url_decode(text).decode("utf-8"))


def random_b64url(num_bytes: int) -> str:
    return b64url_encode(secrets.token_bytes(num_bytes))


def sha256_b64url(text: str) -> str:
    return b64url_encode(hashlib.sha256(text.encode("ascii")).digest())


def timing_safe_equal(a: Optional[str], b: Optional[str]) -> bool:
    """Constant-time string comparison; False when either side is not a string."""
    if not isinstance(a, str) or not isinstance(b, str):
        return False
    return hmac.compare_digest(a.encode("utf-8"), b.encode("utf-8"))


def is_object(value: Any) -> bool:
    return isinstance(value, dict)


def string_or_none(value: Any) -> Optional[str]:
    return value if isinstance(value, str) and value != "" else None


def serialize_cookie(name: str, value: str, options: Dict[str, Any]) -> str:
    """Build a ``Set-Cookie`` header value. Raises ``ValueError`` on an invalid cookie name."""
    from urllib.parse import quote

    if not _COOKIE_NAME.match(name):
        raise ValueError(f"invalid cookie name {name}")
    parts = [f"{name}={quote(value, safe='')}", f"Path={options['path']}", f"Max-Age={int(options['max_age'])}"]
    if options.get("domain") is not None:
        parts.append(f"Domain={options['domain']}")
    if options.get("http_only"):
        parts.append("HttpOnly")
    if options.get("secure"):
        parts.append("Secure")
    parts.append(f"SameSite={options['same_site']}")
    return "; ".join(parts)
