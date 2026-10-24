# Copyright © Anointed Automation Corporation., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
"""Every failure the kit raises.

``code`` is stable and safe to branch on; the message is for logs only and must never be shown to an
end user verbatim.

Codes: config, discovery, flow_missing, flow_expired, state_mismatch, access_denied, provider_error,
issuer_mismatch, missing_code, token_exchange, id_token_invalid, email_not_verified,
logout_token_invalid, redirect_uri_rejected, http.
"""

from __future__ import annotations

from typing import Optional


class AnointedSsoError(Exception):
    """A coded Sign in with Anointed Automation failure."""

    def __init__(
        self,
        code: str,
        message: str,
        *,
        status: Optional[int] = None,
        oauth_error: Optional[str] = None,
    ) -> None:
        super().__init__(message)
        self.code = code
        self.status = status
        self.oauth_error = oauth_error
