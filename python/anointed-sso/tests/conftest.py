# Copyright © Anointed Automation Corporation., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
"""Shared fixtures: a fake Anointed Automation provider that signs real RS256 tokens."""

from __future__ import annotations

import json
from typing import Any, Callable, Dict, List, Optional, Tuple
from urllib.parse import parse_qs, urlsplit

import jwt
import pytest
from cryptography.hazmat.primitives import serialization
from cryptography.hazmat.primitives.asymmetric import rsa
from jwt.algorithms import RSAAlgorithm

from anointedautomation_sso import ANOINTED_ISSUER, AnointedClient
from anointedautomation_sso.http import HttpResponse

CLIENT_ID = "aa_testkey"
CLIENT_SECRET = "s3cret"
REDIRECT_URI = "https://app.example/auth/anointed/callback"
NOW = 1_800_000_000


def make_key(kid: str) -> Tuple[Any, Dict[str, Any]]:
    private = rsa.generate_private_key(public_exponent=65537, key_size=2048)
    jwk = json.loads(RSAAlgorithm.to_jwk(private.public_key()))
    jwk.update({"kid": kid, "use": "sig", "alg": "RS256"})
    return private, jwk


class Provider:
    """Answers the transport calls like the real issuer, and records them."""

    def __init__(self) -> None:
        self.key, self.jwk = make_key("k1")
        self.jwks: Dict[str, Any] = {"keys": [self.jwk]}
        self.calls: List[Tuple[str, str, Optional[bytes]]] = []
        self.token_response: Optional[Dict[str, Any]] = None
        self.token_status = 200
        self.token_error: Optional[str] = None
        self.discovery_override: Dict[str, Any] = {}
        self.handlers: Dict[str, Callable[[str, Dict[str, str], Optional[bytes]], HttpResponse]] = {}
        self.pending_nonce: Optional[str] = None

    def discovery(self) -> Dict[str, Any]:
        base = ANOINTED_ISSUER
        doc = {
            "issuer": base,
            "authorization_endpoint": base + "connect/authorize",
            "token_endpoint": base + "connect/token",
            "jwks_uri": base + ".well-known/jwks.json",
            "userinfo_endpoint": base + "connect/userinfo",
            "revocation_endpoint": base + "connect/revocation",
            "end_session_endpoint": base + "connect/endsession",
            "code_challenge_methods_supported": ["S256"],
        }
        doc.update(self.discovery_override)
        return doc

    def sign(self, claims: Dict[str, Any], *, typ: Optional[str] = None, key: Any = None, kid: str = "k1", alg: str = "RS256") -> str:
        headers: Dict[str, Any] = {"kid": kid}
        if typ is not None:
            headers["typ"] = typ
        return jwt.encode(claims, key or self.key, algorithm=alg, headers=headers)

    def id_token(self, **overrides: Any) -> str:
        claims: Dict[str, Any] = {
            "iss": ANOINTED_ISSUER,
            "aud": CLIENT_ID,
            "sub": "user-1",
            "exp": NOW + 600,
            "iat": NOW,
            "nonce": self.pending_nonce,
            "email": "Alex@Example.com ",
            "email_verified": True,
            "sid": "grant-1",
        }
        claims.update(overrides)
        claims = {k: v for k, v in claims.items() if v is not None}
        return self.sign(claims)

    def logout_token(self, **overrides: Any) -> str:
        claims: Dict[str, Any] = {
            "iss": ANOINTED_ISSUER,
            "aud": CLIENT_ID,
            "iat": NOW,
            "jti": "jti-1",
            "sub": "user-1",
            "events": {"http://schemas.openid.net/event/backchannel-logout": {}},
        }
        claims.update(overrides)
        claims = {k: v for k, v in claims.items() if v is not None}
        return self.sign(claims, typ="logout+jwt")

    def __call__(self, method: str, url: str, headers: Dict[str, str], data: Optional[bytes], timeout: float) -> HttpResponse:
        self.calls.append((method, url, data))
        for prefix, handler in self.handlers.items():
            if url.startswith(prefix):
                return handler(method, headers, data)
        if url.endswith("/.well-known/openid-configuration"):
            return HttpResponse(200, json.dumps(self.discovery()))
        if url.endswith("/.well-known/jwks.json"):
            return HttpResponse(200, json.dumps(self.jwks))
        if url.endswith("/connect/token"):
            if self.token_status != 200:
                return HttpResponse(self.token_status, json.dumps({"error": self.token_error or "invalid_grant"}))
            body = self.token_response
            if body is None:
                body = {"access_token": "at", "token_type": "Bearer", "expires_in": 3600, "id_token": self.id_token(), "refresh_token": "rt"}
            return HttpResponse(200, json.dumps(body))
        if url.endswith("/connect/revocation"):
            return HttpResponse(200, "")
        return HttpResponse(404, "")

    def form_of_last_token_call(self) -> Dict[str, str]:
        data = [c for c in self.calls if c[1].endswith("/connect/token")][-1][2]
        assert data is not None
        return {k: v[0] for k, v in parse_qs(data.decode()).items()}


@pytest.fixture()
def provider() -> Provider:
    return Provider()


@pytest.fixture()
def client(provider: Provider) -> AnointedClient:
    return AnointedClient(
        issuer=ANOINTED_ISSUER,
        client_id=CLIENT_ID,
        client_secret=CLIENT_SECRET,
        redirect_uri=REDIRECT_URI,
        http=provider,
        now=lambda: NOW,
    )


def start_flow(client: AnointedClient, provider: Provider, **kwargs: Any) -> Tuple[Any, str]:
    """Run the start step; returns (AuthorizeRequest, callback query string with a code)."""
    auth = client.create_authorize_request(**kwargs)
    query = parse_qs(urlsplit(auth.url).query)
    provider.pending_nonce = query["nonce"][0]
    return auth, query["state"][0]
