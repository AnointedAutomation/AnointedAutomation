# Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
"""A tiny injectable HTTP transport so the core has no HTTP client dependency and tests can mock it."""

from __future__ import annotations

import urllib.error
import urllib.request
from dataclasses import dataclass
from typing import Callable, Dict, Optional


@dataclass(frozen=True)
class HttpResponse:
    """A completed HTTP exchange."""

    status: int
    body: str

    @property
    def ok(self) -> bool:
        return 200 <= self.status < 300


#: ``transport(method, url, headers, data, timeout_seconds) -> HttpResponse``. Raise ``OSError`` on a
#: network failure. A non-2xx status is returned, not raised.
HttpTransport = Callable[[str, str, Dict[str, str], Optional[bytes], float], HttpResponse]


def urllib_transport(
    method: str, url: str, headers: Dict[str, str], data: Optional[bytes], timeout: float
) -> HttpResponse:
    """The default transport, built on the standard library."""
    if not url.lower().startswith(("https://", "http://")):
        raise OSError("only http(s) URLs are supported")
    request = urllib.request.Request(url, data=data, headers=headers, method=method)  # noqa: S310
    try:
        with urllib.request.urlopen(request, timeout=timeout) as response:  # noqa: S310
            return HttpResponse(response.status, response.read().decode("utf-8"))
    except urllib.error.HTTPError as err:
        return HttpResponse(err.code, err.read().decode("utf-8", errors="replace"))
