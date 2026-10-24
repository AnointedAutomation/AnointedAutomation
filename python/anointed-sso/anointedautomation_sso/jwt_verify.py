# Copyright © Anointed Automation Corporation., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
"""RS256 JWS verification against a JWKS (PyJWT) plus OpenID Connect claim checks."""

from __future__ import annotations

import json
from typing import Any, Dict, Optional, Tuple

import jwt
from jwt.algorithms import RSAAlgorithm

from ._util import timing_safe_equal


class UnknownKidError(Exception):
    """The token names a key that is not in the JWKS (the caller may refetch once)."""


def select_jwk(jwks: Any, header: Dict[str, Any]) -> Optional[Dict[str, Any]]:
    """Pick the RSA signing key for a header: exact ``kid`` match, or the single candidate when no ``kid``."""
    keys = jwks.get("keys") if isinstance(jwks, dict) else None
    if not isinstance(keys, list):
        keys = []
    candidates = [
        k
        for k in keys
        if isinstance(k, dict)
        and k.get("kty") == "RSA"
        and k.get("use") in (None, "sig")
        and k.get("alg") in (None, "RS256")
    ]
    kid = header.get("kid")
    if isinstance(kid, str):
        for key in candidates:
            if key.get("kid") == kid:
                return key
        return None
    return candidates[0] if len(candidates) == 1 else None


def verify_rs256(token: Any, jwks: Any) -> Tuple[Dict[str, Any], Dict[str, Any]]:
    """Verify the signature only. Returns ``(header, payload)``.

    Raises ``UnknownKidError`` for an unknown key and ``ValueError`` for anything else wrong.
    """
    if not isinstance(token, str):
        raise ValueError("token is not a string")
    try:
        header = jwt.get_unverified_header(token)
    except jwt.PyJWTError as err:
        raise ValueError(f"token is not a compact JWS: {err}") from err
    if header.get("alg") != "RS256":
        raise ValueError(f"alg {header.get('alg')} is not allowed")
    jwk = select_jwk(jwks, header)
    if jwk is None:
        raise UnknownKidError(f"kid {header.get('kid')} is not in the JWKS")
    try:
        key = RSAAlgorithm.from_jwk(json.dumps({"kty": "RSA", "n": jwk["n"], "e": jwk["e"]}))
        payload = jwt.decode(
            token,
            key,
            algorithms=["RS256"],
            options={
                "verify_signature": True,
                "verify_exp": False,
                "verify_nbf": False,
                "verify_iat": False,
                "verify_aud": False,
                "verify_iss": False,
                "verify_sub": False,
                "verify_jti": False,
                "require": [],
            },
        )
    except (jwt.PyJWTError, KeyError, ValueError, TypeError) as err:
        raise ValueError(f"signature is invalid or token malformed: {err}") from err
    if not isinstance(payload, dict):
        raise ValueError("payload is not an object")
    return header, payload


def assert_audience(claims: Dict[str, Any], client_id: str) -> None:
    aud = claims.get("aud")
    audiences = aud if isinstance(aud, list) else [aud]
    if client_id not in audiences:
        raise ValueError("aud does not include the client_id")
    if len(audiences) > 1 and claims.get("azp") != client_id:
        raise ValueError("azp does not name the client_id")
    if "azp" in claims and claims["azp"] != client_id:
        raise ValueError("azp does not name the client_id")


def _is_number(value: Any) -> bool:
    return isinstance(value, (int, float)) and not isinstance(value, bool)


def validate_id_token_claims(
    claims: Dict[str, Any],
    *,
    issuer: str,
    client_id: str,
    nonce: Optional[str],
    now: float,
    clock_skew_seconds: float,
) -> None:
    """Validate ID token claims (signature already checked). ``nonce=None`` skips the nonce check."""
    if claims.get("iss") != issuer:
        raise ValueError(f"iss {claims.get('iss')} does not match {issuer}")
    assert_audience(claims, client_id)
    if not _is_number(claims.get("exp")):
        raise ValueError("exp is missing")
    if now > claims["exp"] + clock_skew_seconds:
        raise ValueError("token is expired")
    if not _is_number(claims.get("iat")):
        raise ValueError("iat is missing")
    if claims["iat"] - clock_skew_seconds > now:
        raise ValueError("token was issued in the future")
    if nonce is not None and not timing_safe_equal(claims.get("nonce"), nonce):
        raise ValueError("nonce does not match")
    sub = claims.get("sub")
    if not isinstance(sub, str) or sub == "":
        raise ValueError("sub is missing")
