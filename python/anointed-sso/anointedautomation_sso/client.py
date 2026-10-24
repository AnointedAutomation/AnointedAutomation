# Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
"""Sign in with Anointed Automation: a server-side OpenID Connect client.

Authorization code + PKCE S256, state and nonce, RS256 ID token verification via JWKS, refresh, revoke,
userinfo, logout URL and back-channel logout token validation. SERVER ONLY: it holds the client secret.

The flow secrets (state, nonce, PKCE verifier and an optional validated return target) ride ONE
short-lived httpOnly cookie between the start and callback requests, so no server session store is needed.
"""

from __future__ import annotations

import json
import threading
import time
from dataclasses import dataclass, field
from typing import Any, Callable, Dict, List, Mapping, Optional, Tuple
from urllib.parse import parse_qsl, urlencode, urljoin, urlsplit, urlunsplit

from . import jwt_verify
from ._util import (
    b64url_to_json,
    is_object,
    json_to_b64url,
    random_b64url,
    serialize_cookie,
    sha256_b64url,
    string_or_none,
    timing_safe_equal,
)
from .errors import AnointedSsoError
from .http import HttpResponse, HttpTransport, urllib_transport
from .return_to import create_return_to_validator, is_loopback_host, safe_relative_path

#: The production issuer. Pass it explicitly as ``issuer``.
ANOINTED_ISSUER = "https://api.anointedautomation.net/"

#: Default scope. ``profile`` is left out on purpose: it makes the consent screen ask users without a
#: username to pick one first. Add it only when the app needs a name.
DEFAULT_SCOPE = "openid email"

#: The member a back-channel Logout Token's ``events`` claim must carry.
BACKCHANNEL_LOGOUT_EVENT = "http://schemas.openid.net/event/backchannel-logout"

_RESERVED_AUTHORIZE_PARAMS = frozenset(
    ["client_id", "redirect_uri", "response_type", "scope", "state", "nonce", "code_challenge", "code_challenge_method"]
)


def safe_return_to(value: object) -> Optional[str]:
    """Keep a return target only when it is a same-site path (blocks open redirects)."""
    return safe_relative_path(value)


@dataclass(frozen=True)
class AnointedUser:
    """The app-facing user from validated ID token claims."""

    sub: str
    email: Optional[str]
    email_verified: bool
    name: Optional[str]
    preferred_username: Optional[str]
    given_name: Optional[str]
    family_name: Optional[str]
    picture: Optional[str]
    sid: Optional[str]


@dataclass(frozen=True)
class AnointedTokens:
    access_token: Optional[str]
    id_token: Optional[str]
    refresh_token: Optional[str]
    token_type: Optional[str]
    scope: Optional[str]
    expires_at: Optional[float]


@dataclass(frozen=True)
class AuthorizeRequest:
    """Where to send the browser, and the flow cookie to set on that response."""

    url: str
    state: str
    redirect_uri: str
    cookie_name: str
    cookie_value: str
    cookie_options: Dict[str, Any]
    set_cookie_header: str


@dataclass(frozen=True)
class SignInResult:
    user: AnointedUser
    claims: Dict[str, Any]
    tokens: AnointedTokens
    return_to: Optional[str]


@dataclass(frozen=True)
class LogoutToken:
    """A validated back-channel Logout Token."""

    sub: Optional[str]
    sid: Optional[str]
    jti: str
    exp: Optional[float]
    claims: Dict[str, Any] = field(repr=False)


def _require_string(value: Any, name: str) -> str:
    if not isinstance(value, str) or value.strip() == "":
        raise AnointedSsoError("config", f"{name} is required")
    return value


def _parse_secure_url(value: Any, label: str, code: str) -> Tuple[str, Any]:
    try:
        parts = urlsplit(str(value))
        _ = parts.port
    except ValueError as err:
        raise AnointedSsoError(code, f"{label} is not an absolute URL") from err
    if parts.scheme == "" or parts.netloc == "":
        raise AnointedSsoError(code, f"{label} is not an absolute URL")
    if parts.scheme != "https" and not (parts.scheme == "http" and is_loopback_host(parts.hostname)):
        raise AnointedSsoError(code, f"{label} must use https (http is allowed only for localhost)")
    if parts.fragment != "":
        raise AnointedSsoError(code, f"{label} must not contain a fragment")
    return str(value), parts


def _positive(value: Any, name: str) -> float:
    if isinstance(value, bool) or not isinstance(value, (int, float)) or value <= 0:
        raise AnointedSsoError("config", f"{name} must be a positive number")
    return value


class AnointedClient:
    """A "Sign in with Anointed Automation" client.

    Configuration is validated immediately (fail fast). Discovery and JWKS are fetched lazily and cached
    for ``metadata_ttl_seconds``; the JWKS is refetched once when a token names an unknown ``kid``.

    :param issuer: ``ANOINTED_ISSUER`` (pass it explicitly).
    :param client_id: ``aa_<your key id>``.
    :param redirect_uri: The exact registered callback URL, or a callable taking the framework request and
        returning it (one client can then serve several hosts). Every value must be https.
    :param client_secret: Server only. Not needed to start a sign-in; required by the code exchange,
        refresh, revoke and ``take_new_user_password``.
    :param allowed_redirect_uris: When set, every redirect URI must be exactly one of these.
    :param allowed_return_origins: Origins an absolute ``return_to`` may point at.
    :param is_allowed_return_to: Custom approval for an absolute ``return_to`` (still https only).
    :param scope: Space separated; must include ``openid``. Default ``openid email``.
    :param require_verified_email: Default True when the scope includes ``email``.
    :param http: Injectable transport (tests, proxies). Default: standard library.
    :param now: Clock in seconds since the epoch (tests).
    """

    def __init__(
        self,
        *,
        issuer: str,
        client_id: str,
        redirect_uri: Any,
        client_secret: Optional[str] = None,
        allowed_redirect_uris: Optional[List[str]] = None,
        allowed_return_origins: Optional[List[str]] = None,
        is_allowed_return_to: Optional[Callable[[str], bool]] = None,
        scope: str = DEFAULT_SCOPE,
        require_verified_email: Optional[bool] = None,
        cookie_name: str = "aa_sso_flow",
        cookie_path: str = "/",
        cookie_domain: Optional[str] = None,
        flow_ttl_seconds: float = 600,
        clock_skew_seconds: float = 60,
        metadata_ttl_seconds: float = 3600,
        http_timeout_seconds: float = 10,
        http: Optional[HttpTransport] = None,
        now: Optional[Callable[[], float]] = None,
    ) -> None:
        self.issuer = _require_string(issuer, "issuer")
        _parse_secure_url(self.issuer, "issuer", "config")
        self.client_id = _require_string(client_id, "client_id")
        if client_secret is not None and (not isinstance(client_secret, str) or client_secret.strip() == ""):
            raise AnointedSsoError("config", "client_secret must be a non-empty string when given")
        self._client_secret = client_secret

        self._allowed_redirect_uris: Optional[frozenset] = None
        if allowed_redirect_uris is not None:
            if not isinstance(allowed_redirect_uris, (list, tuple)) or len(allowed_redirect_uris) == 0:
                raise AnointedSsoError("config", "allowed_redirect_uris must be a non-empty list")
            for entry in allowed_redirect_uris:
                self._check_redirect_uri(entry, None, "config")
            self._allowed_redirect_uris = frozenset(allowed_redirect_uris)

        self._redirect_resolver: Optional[Callable[[Any], str]] = None
        self._static_redirect: Optional[Tuple[str, Any]] = None
        if callable(redirect_uri):
            self._redirect_resolver = redirect_uri
        else:
            self._static_redirect = self._check_redirect_uri(
                _require_string(redirect_uri, "redirect_uri"), self._allowed_redirect_uris, "config"
            )
        self.redirect_uri: Optional[str] = None if self._static_redirect is None else self._static_redirect[0]

        self.validate_return_to = create_return_to_validator(allowed_return_origins, is_allowed_return_to)

        if not isinstance(scope, str):
            raise AnointedSsoError("config", "scope must be a string")
        scopes = [s for s in scope.split(" ") if s != ""]
        if "openid" not in scopes:
            raise AnointedSsoError("config", "scope must include openid")
        self.scope = " ".join(scopes)
        self._scopes = scopes

        if not isinstance(cookie_name, str) or cookie_name == "":
            raise AnointedSsoError("config", "cookie_name must be a string")
        if not isinstance(cookie_path, str) or not cookie_path.startswith("/"):
            raise AnointedSsoError("config", "cookie_path must start with /")
        if cookie_domain is not None and (not isinstance(cookie_domain, str) or cookie_domain == ""):
            raise AnointedSsoError("config", "cookie_domain must be a non-empty string")
        self.cookie_name = cookie_name
        self._flow_ttl = _positive(flow_ttl_seconds, "flow_ttl_seconds")
        self._skew = _positive(clock_skew_seconds, "clock_skew_seconds")
        self._metadata_ttl = _positive(metadata_ttl_seconds, "metadata_ttl_seconds")
        self._http_timeout = _positive(http_timeout_seconds, "http_timeout_seconds")
        if require_verified_email is not None and not isinstance(require_verified_email, bool):
            raise AnointedSsoError("config", "require_verified_email must be a boolean")
        self._require_verified_email = "email" in scopes if require_verified_email is None else require_verified_email
        self._http: HttpTransport = urllib_transport if http is None else http
        if not callable(self._http):
            raise AnointedSsoError("config", "http must be callable")
        self._clock: Callable[[], float] = time.time if now is None else now
        if not callable(self._clock):
            raise AnointedSsoError("config", "now must be callable")

        self._cookie_options: Dict[str, Any] = {
            "path": cookie_path,
            "max_age": self._flow_ttl,
            "http_only": True,
            "secure": True if self._static_redirect is None else self._static_redirect[1].scheme == "https",
            "same_site": "Lax",
            "domain": cookie_domain,
        }
        try:
            serialize_cookie(cookie_name, "", self._cookie_options)
        except ValueError as err:
            raise AnointedSsoError("config", f"cookie_name {cookie_name} is not a valid cookie name") from err

        self._lock = threading.Lock()
        self._discovery: Optional[Tuple[Dict[str, Any], float]] = None
        self._jwks: Optional[Tuple[Dict[str, Any], float]] = None

    # ---- internals -------------------------------------------------------------------------------

    @staticmethod
    def _check_redirect_uri(value: Any, allowed: Optional[frozenset], code: str) -> Tuple[str, Any]:
        if not isinstance(value, str) or value.strip() == "":
            raise AnointedSsoError(code, "redirect_uri must be a non-empty string")
        uri, parts = _parse_secure_url(value, "redirect_uri", code)
        if allowed is not None and uri not in allowed:
            raise AnointedSsoError(code, f"redirect_uri {uri} is not in allowed_redirect_uris")
        return uri, parts

    def _request_json(
        self,
        method: str,
        url: str,
        fail_code: str,
        *,
        headers: Optional[Dict[str, str]] = None,
        data: Optional[bytes] = None,
    ) -> Any:
        try:
            response: HttpResponse = self._http(method, url, headers or {"accept": "application/json"}, data, self._http_timeout)
        except OSError as err:
            raise AnointedSsoError(fail_code, f"{url} request failed: {err}") from err
        body: Any = None
        if response.body != "":
            try:
                body = json.loads(response.body)
            except ValueError:
                body = None
        if not response.ok:
            oauth_error = body.get("error") if is_object(body) and isinstance(body.get("error"), str) else None
            detail = "" if oauth_error is None else f" {oauth_error}"
            raise AnointedSsoError(fail_code, f"{url} -> {response.status}{detail}", status=response.status, oauth_error=oauth_error)
        return body

    def _post_form(self, url: str, fields: Dict[str, str], fail_code: str) -> Any:
        if self._client_secret is None:
            raise AnointedSsoError("config", "client_secret is required to call the token endpoint (confidential client)")
        payload = dict(fields)
        payload["client_id"] = self.client_id
        payload["client_secret"] = self._client_secret
        return self._request_json(
            "POST",
            url,
            fail_code,
            headers={"content-type": "application/x-www-form-urlencoded", "accept": "application/json"},
            data=urlencode(payload).encode("utf-8"),
        )

    def discover(self, *, force: bool = False) -> Dict[str, Any]:
        """The issuer's discovery document (cached)."""
        with self._lock:
            if not force and self._discovery is not None and self._discovery[1] > self._clock():
                return self._discovery[0]
            url = f"{self.issuer.rstrip('/')}/.well-known/openid-configuration"
            doc = self._request_json("GET", url, "discovery")
            if not is_object(doc):
                raise AnointedSsoError("discovery", "discovery document is not a JSON object")
            if doc.get("issuer") != self.issuer:
                raise AnointedSsoError("discovery", f"discovery issuer {doc.get('issuer')} does not match {self.issuer}")
            for key in ("authorization_endpoint", "token_endpoint", "jwks_uri"):
                if not isinstance(doc.get(key), str):
                    raise AnointedSsoError("discovery", f"discovery document has no {key}")
                _parse_secure_url(doc[key], key, "discovery")
            methods = doc.get("code_challenge_methods_supported")
            if isinstance(methods, list) and "S256" not in methods:
                raise AnointedSsoError("discovery", "provider does not advertise PKCE S256")
            self._discovery = (doc, self._clock() + self._metadata_ttl)
            return doc

    def get_jwks(self, *, force: bool = False) -> Dict[str, Any]:
        """The issuer's JWKS (cached)."""
        doc = self.discover()
        with self._lock:
            if not force and self._jwks is not None and self._jwks[1] > self._clock():
                return self._jwks[0]
            jwks = self._request_json("GET", doc["jwks_uri"], "http")
            if not is_object(jwks) or not isinstance(jwks.get("keys"), list):
                raise AnointedSsoError("http", "JWKS has no keys array")
            self._jwks = (jwks, self._clock() + self._metadata_ttl)
            return jwks

    def _verify_with_jwks(self, token: str) -> Tuple[Dict[str, Any], Dict[str, Any]]:
        try:
            return jwt_verify.verify_rs256(token, self.get_jwks())
        except jwt_verify.UnknownKidError:
            return jwt_verify.verify_rs256(token, self.get_jwks(force=True))

    def _validate_id_token(self, id_token: str, nonce: Optional[str]) -> Dict[str, Any]:
        try:
            _, payload = self._verify_with_jwks(id_token)
            jwt_verify.validate_id_token_claims(
                payload,
                issuer=self.issuer,
                client_id=self.client_id,
                nonce=nonce,
                now=self._clock(),
                clock_skew_seconds=self._skew,
            )
            return payload
        except AnointedSsoError:
            raise
        except (ValueError, jwt_verify.UnknownKidError) as err:
            raise AnointedSsoError("id_token_invalid", f"id_token rejected: {err}") from err

    def _user_from_claims(self, claims: Dict[str, Any]) -> AnointedUser:
        verified = claims.get("email_verified") is True
        raw_email = claims.get("email")
        email = raw_email.strip().lower() if verified and isinstance(raw_email, str) and raw_email.strip() != "" else None
        if self._require_verified_email and email is None:
            raise AnointedSsoError("email_not_verified", "the ID token carries no verified email")
        return AnointedUser(
            sub=claims["sub"],
            email=email,
            email_verified=email is not None,
            name=string_or_none(claims.get("name")),
            preferred_username=string_or_none(claims.get("preferred_username")),
            given_name=string_or_none(claims.get("given_name")),
            family_name=string_or_none(claims.get("family_name")),
            picture=string_or_none(claims.get("picture")),
            sid=string_or_none(claims.get("sid")),
        )

    def _normalize_tokens(self, raw: Dict[str, Any]) -> AnointedTokens:
        expires_in = raw.get("expires_in")
        numeric = isinstance(expires_in, (int, float)) and not isinstance(expires_in, bool)
        return AnointedTokens(
            access_token=string_or_none(raw.get("access_token")),
            id_token=string_or_none(raw.get("id_token")),
            refresh_token=string_or_none(raw.get("refresh_token")),
            token_type=string_or_none(raw.get("token_type")),
            scope=string_or_none(raw.get("scope")),
            expires_at=self._clock() + expires_in if numeric else None,
        )

    def _resolve_redirect(self, redirect_uri: Optional[str], request: Any) -> Tuple[str, Any]:
        if redirect_uri is not None:
            return self._check_redirect_uri(redirect_uri, self._allowed_redirect_uris, "redirect_uri_rejected")
        if self._redirect_resolver is not None:
            try:
                value = self._redirect_resolver(request)
            except Exception as err:  # noqa: BLE001 - any resolver failure is a rejection
                raise AnointedSsoError("redirect_uri_rejected", "the redirect_uri function raised") from err
            return self._check_redirect_uri(value, self._allowed_redirect_uris, "redirect_uri_rejected")
        assert self._static_redirect is not None
        return self._static_redirect

    def resolve_redirect_uri(self, request: Any = None) -> str:
        """The redirect URI the client would use for a request (validated)."""
        return self._resolve_redirect(None, request)[0]

    def decode_flow_cookie(self, value: Any) -> Optional[Dict[str, Any]]:
        """Decode the flow cookie value; None when absent or malformed."""
        if not isinstance(value, str) or value == "":
            return None
        try:
            body = b64url_to_json(value)
        except (ValueError, UnicodeDecodeError):
            return None
        if not is_object(body):
            return None
        s, n, v, t = body.get("s"), body.get("n"), body.get("v"), body.get("t")
        if not (isinstance(s, str) and isinstance(n, str) and isinstance(v, str)):
            return None
        if isinstance(t, bool) or not isinstance(t, (int, float)) or s == "" or n == "" or v == "":
            return None
        return {"state": s, "nonce": n, "verifier": v, "created_at": t, "return_to": self.validate_return_to(body.get("r"))}

    # ---- public flow -----------------------------------------------------------------------------

    def create_authorize_request(
        self,
        *,
        return_to: Optional[str] = None,
        prompt: Optional[str] = None,
        login_hint: Optional[str] = None,
        extra_params: Optional[Mapping[str, str]] = None,
        redirect_uri: Optional[str] = None,
        request: Any = None,
    ) -> AuthorizeRequest:
        """Start a sign-in: the authorize URL plus the flow cookie to set on the redirect response."""
        uri, parts = self._resolve_redirect(redirect_uri, request)
        doc = self.discover()
        state = random_b64url(32)
        nonce = random_b64url(32)
        verifier = random_b64url(48)
        params: List[Tuple[str, str]] = [
            ("client_id", self.client_id),
            ("redirect_uri", uri),
            ("response_type", "code"),
            ("scope", self.scope),
            ("state", state),
            ("nonce", nonce),
            ("code_challenge", sha256_b64url(verifier)),
            ("code_challenge_method", "S256"),
        ]
        if prompt is not None:
            params.append(("prompt", str(prompt)))
        if login_hint is not None:
            params.append(("login_hint", str(login_hint)))
        if extra_params is not None:
            for key, value in extra_params.items():
                if key in _RESERVED_AUTHORIZE_PARAMS:
                    raise AnointedSsoError("config", f"extra_params may not set {key}")
                params.append((key, str(value)))
        base = urlsplit(doc["authorization_endpoint"])
        query = "&".join(filter(None, [base.query, urlencode(params)]))
        url = urlunsplit((base.scheme, base.netloc, base.path, query, ""))

        flow: Dict[str, Any] = {"s": state, "n": nonce, "v": verifier, "t": int(self._clock())}
        safe = self.validate_return_to(return_to)
        if safe is not None:
            flow["r"] = safe
        value = json_to_b64url(flow)
        options = dict(self._cookie_options)
        options["secure"] = parts.scheme == "https"
        return AuthorizeRequest(
            url=url,
            state=state,
            redirect_uri=uri,
            cookie_name=self.cookie_name,
            cookie_value=value,
            cookie_options=options,
            set_cookie_header=serialize_cookie(self.cookie_name, value, options),
        )

    def handle_callback(
        self,
        *,
        query: Any = None,
        url: Optional[str] = None,
        cookie_value: Optional[str] = None,
        redirect_uri: Optional[str] = None,
        request: Any = None,
    ) -> SignInResult:
        """Finish a sign-in on the callback request.

        In order: the flow cookie must exist; ``state`` must match it (constant time) before anything else;
        the flow must not be older than ``flow_ttl_seconds``; ``iss``, when sent, must be our issuer
        (RFC 9207); a provider ``error`` is raised; the code is exchanged with the secret and PKCE
        verifier; the ID token is verified (RS256 via JWKS, iss, aud/azp, exp/iat, nonce, sub) and a
        verified email enforced when required. Always clear the flow cookie afterwards.

        :param query: The callback query as a mapping (``str -> str``) or ``(k, v)`` pairs, instead of ``url``.
        """
        uri, _ = self._resolve_redirect(redirect_uri, request)
        if isinstance(url, str):
            params: Dict[str, str] = dict(parse_qsl(urlsplit(urljoin(uri, url)).query, keep_blank_values=True))
        elif query is not None:
            items = query.items() if hasattr(query, "items") else query
            params = {k: v for k, v in items if isinstance(k, str) and isinstance(v, str)}
        else:
            raise AnointedSsoError("config", "handle_callback needs url or query")

        flow = self.decode_flow_cookie(cookie_value)
        if flow is None:
            raise AnointedSsoError("flow_missing", "the sign-in flow cookie is missing or malformed")
        if not timing_safe_equal(params.get("state"), flow["state"]):
            raise AnointedSsoError("state_mismatch", "state does not match the flow cookie")
        if self._clock() - flow["created_at"] > self._flow_ttl:
            raise AnointedSsoError("flow_expired", "the sign-in flow expired")
        iss = params.get("iss")
        if iss is not None and iss != self.issuer:
            raise AnointedSsoError("issuer_mismatch", f"callback iss {iss} does not match {self.issuer}")
        error = params.get("error")
        if error is not None:
            description = params.get("error_description")
            message = error if description is None else f"{error}: {description}"
            raise AnointedSsoError("access_denied" if error == "access_denied" else "provider_error", message, oauth_error=error)
        code = params.get("code")
        if code is None or code == "":
            raise AnointedSsoError("missing_code", "the callback has no code")

        doc = self.discover()
        raw = self._post_form(
            doc["token_endpoint"],
            {"grant_type": "authorization_code", "code": code, "redirect_uri": uri, "code_verifier": flow["verifier"]},
            "token_exchange",
        )
        if not is_object(raw) or not isinstance(raw.get("id_token"), str):
            raise AnointedSsoError("token_exchange", "the token response has no id_token")
        claims = self._validate_id_token(raw["id_token"], flow["nonce"])
        return SignInResult(self._user_from_claims(claims), claims, self._normalize_tokens(raw), flow["return_to"])

    def refresh(self, refresh_token: str, *, expected_sub: Optional[str] = None) -> Tuple[AnointedTokens, Optional[Dict[str, Any]]]:
        """Refresh tokens (needs ``offline_access``). Each refresh returns a NEW refresh token.

        ``invalid_grant`` (code ``token_exchange``, ``oauth_error="invalid_grant"``) means the user
        disconnected the app, was suspended, or the app was disabled: treat it as signed out.
        """
        if not isinstance(refresh_token, str) or refresh_token == "":
            raise AnointedSsoError("config", "refresh_token is required")
        doc = self.discover()
        raw = self._post_form(doc["token_endpoint"], {"grant_type": "refresh_token", "refresh_token": refresh_token}, "token_exchange")
        if not is_object(raw):
            raise AnointedSsoError("token_exchange", "the token response is not a JSON object")
        claims = None
        if isinstance(raw.get("id_token"), str):
            claims = self._validate_id_token(raw["id_token"], None)
            if expected_sub is not None and claims["sub"] != expected_sub:
                raise AnointedSsoError("id_token_invalid", "refreshed id_token names a different sub")
        return self._normalize_tokens(raw), claims

    def revoke(self, token: str, *, token_type_hint: str = "refresh_token") -> None:
        """Revoke a token (RFC 7009). Call it with the refresh token when the user signs out."""
        if not isinstance(token, str) or token == "":
            raise AnointedSsoError("config", "token is required")
        doc = self.discover()
        if not isinstance(doc.get("revocation_endpoint"), str):
            raise AnointedSsoError("discovery", "provider has no revocation_endpoint")
        self._post_form(doc["revocation_endpoint"], {"token": token, "token_type_hint": token_type_hint}, "http")

    def fetch_user_info(self, access_token: str) -> Dict[str, Any]:
        """Call UserInfo with an access token."""
        if not isinstance(access_token, str) or access_token == "":
            raise AnointedSsoError("config", "access_token is required")
        doc = self.discover()
        if not isinstance(doc.get("userinfo_endpoint"), str):
            raise AnointedSsoError("discovery", "provider has no userinfo_endpoint")
        info = self._request_json(
            "GET", doc["userinfo_endpoint"], "http", headers={"authorization": f"Bearer {access_token}", "accept": "application/json"}
        )
        if not is_object(info) or not isinstance(info.get("sub"), str):
            raise AnointedSsoError("http", "userinfo response has no sub")
        return info

    def take_new_user_password(self, sub: str) -> Optional[str]:
        """The password generated for a NEW user's account on your site (server only).

        Call it right after ``handle_callback`` when you create the local account, hash it with your own
        hasher and store the hash. It can be taken exactly once and expires about 15 minutes after consent.
        Returns None for a returning user (or when already taken or expired). Needs ``client_secret``.
        """
        if not isinstance(sub, str) or sub == "":
            raise AnointedSsoError("config", "sub is required")
        if self._client_secret is None:
            raise AnointedSsoError("config", "client_secret is required to take a new user's password (server only)")
        url = f"{self.issuer.rstrip('/')}/api/oauth/partner-password"
        body = json.dumps({"clientId": self.client_id, "clientSecret": self._client_secret, "subject": sub}).encode("utf-8")
        try:
            response = self._http("POST", url, {"content-type": "application/json", "accept": "application/json"}, body, self._http_timeout)
        except OSError as err:
            raise AnointedSsoError("http", f"{url} request failed: {err}") from err
        if response.status == 404:
            return None
        if not response.ok:
            raise AnointedSsoError("http", f"{url} -> {response.status}", status=response.status)
        try:
            envelope = json.loads(response.body)
        except ValueError as err:
            raise AnointedSsoError("http", f"{url} returned invalid JSON") from err
        data = envelope.get("Data") if is_object(envelope) else None
        password = data.get("Password") if is_object(data) else None
        if not isinstance(password, str) or password == "":
            raise AnointedSsoError("http", f"{url} response has no password")
        return password

    def build_logout_url(
        self,
        *,
        id_token_hint: Optional[str] = None,
        post_logout_redirect_uri: Optional[str] = None,
        state: Optional[str] = None,
    ) -> str:
        """The provider logout (end session) URL to send the browser to."""
        doc = self.discover()
        if not isinstance(doc.get("end_session_endpoint"), str):
            raise AnointedSsoError("discovery", "provider has no end_session_endpoint")
        params: List[Tuple[str, str]] = [("client_id", self.client_id)]
        if id_token_hint is not None:
            params.append(("id_token_hint", id_token_hint))
        if post_logout_redirect_uri is not None:
            _parse_secure_url(post_logout_redirect_uri, "post_logout_redirect_uri", "config")
            params.append(("post_logout_redirect_uri", post_logout_redirect_uri))
        if state is not None:
            params.append(("state", state))
        base = urlsplit(doc["end_session_endpoint"])
        query = "&".join(filter(None, [base.query, urlencode(params)]))
        return urlunsplit((base.scheme, base.netloc, base.path, query, ""))

    def verify_logout_token(self, logout_token: str, *, max_age_seconds: float = 300) -> LogoutToken:
        """Validate a back-channel Logout Token (OpenID Connect Back-Channel Logout 1.0).

        Checks RS256 signature via JWKS, header ``typ: logout+jwt``, iss, aud/azp, iat recent and not in
        the future, exp (when present), the back-channel logout event, no ``nonce``, a ``jti``, and ``sub``
        and/or ``sid``. Replay protection is separate: see ``handle_backchannel_logout``.

        :raises AnointedSsoError: ``logout_token_invalid`` (answer 400) or ``http`` when the JWKS cannot be
            read (answer 5xx).
        """
        try:
            header, payload = self._verify_with_jwks(logout_token)
            if header.get("typ") != "logout+jwt":
                raise ValueError("typ is not logout+jwt")
            if payload.get("iss") != self.issuer:
                raise ValueError(f"iss {payload.get('iss')} does not match {self.issuer}")
            jwt_verify.assert_audience(payload, self.client_id)
            now = self._clock()
            iat = payload.get("iat")
            if isinstance(iat, bool) or not isinstance(iat, (int, float)):
                raise ValueError("iat is missing")
            if iat - self._skew > now:
                raise ValueError("token was issued in the future")
            if now - iat > max_age_seconds + self._skew:
                raise ValueError("token is too old")
            exp = payload.get("exp")
            if "exp" in payload and (isinstance(exp, bool) or not isinstance(exp, (int, float)) or now > exp + self._skew):
                raise ValueError("token is expired")
            events = payload.get("events")
            if not is_object(events) or not is_object(events.get(BACKCHANNEL_LOGOUT_EVENT)):
                raise ValueError("events does not carry the back-channel logout event")
            if "nonce" in payload:
                raise ValueError("a logout token must not carry a nonce")
            jti = payload.get("jti")
            if not isinstance(jti, str) or jti == "":
                raise ValueError("jti is missing")
            sub = string_or_none(payload.get("sub"))
            sid = string_or_none(payload.get("sid"))
            if sub is None and sid is None:
                raise ValueError("neither sub nor sid is present")
            return LogoutToken(sub, sid, jti, exp if "exp" in payload else None, payload)
        except AnointedSsoError:
            raise
        except (ValueError, jwt_verify.UnknownKidError) as err:
            raise AnointedSsoError("logout_token_invalid", f"logout token rejected: {err}") from err

    @property
    def clock_skew_seconds(self) -> float:
        return self._skew

    def now(self) -> float:
        return self._clock()

    def clear_flow_cookie_options(self) -> Dict[str, Any]:
        """Cookie options (Max-Age 0) that clear the flow cookie."""
        options = dict(self._cookie_options)
        options["max_age"] = 0
        return options

    def clear_flow_cookie_header(self) -> str:
        """``Set-Cookie`` header value that clears the flow cookie."""
        options = dict(self._cookie_options)
        options["max_age"] = 0
        return serialize_cookie(self.cookie_name, "", options)
