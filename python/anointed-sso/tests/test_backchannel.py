# Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
"""Back-channel logout token validation and replay protection."""

from __future__ import annotations

import pytest

from anointedautomation_sso import ANOINTED_ISSUER, AnointedSsoError, InMemoryLogoutReplayStore, process_backchannel_logout

from .conftest import CLIENT_ID, NOW, make_key

EVENT = "http://schemas.openid.net/event/backchannel-logout"


def test_valid_token(client, provider):
    logout = client.verify_logout_token(provider.logout_token(sid="grant-1"))
    assert logout.sub == "user-1" and logout.sid == "grant-1" and logout.jti == "jti-1" and logout.exp is None


def test_sid_only_is_valid(client, provider):
    assert client.verify_logout_token(provider.logout_token(sub=None, sid="grant-9")).sid == "grant-9"


@pytest.mark.parametrize(
    "overrides",
    [
        {"iss": "https://evil.example/"},
        {"aud": "other"},
        {"iat": NOW - 1000},
        {"iat": NOW + 1000},
        {"iat": None},
        {"exp": NOW - 1000},
        {"events": {}},
        {"events": None},
        {"nonce": "n"},
        {"jti": None},
        {"jti": ""},
        {"sub": None},
    ],
)
def test_invalid_tokens(client, provider, overrides):
    with pytest.raises(AnointedSsoError) as err:
        client.verify_logout_token(provider.logout_token(**overrides))
    assert err.value.code == "logout_token_invalid"


def test_wrong_typ_and_bad_signature(client, provider):
    claims = {"iss": ANOINTED_ISSUER, "aud": CLIENT_ID, "iat": NOW, "jti": "j", "sub": "u", "events": {EVENT: {}}}
    for token in (provider.sign(claims), provider.sign(claims, typ="logout+jwt", key=make_key("k1")[0])):
        with pytest.raises(AnointedSsoError) as err:
            client.verify_logout_token(token)
        assert err.value.code == "logout_token_invalid"


def test_jwks_outage_is_not_a_400(client, provider):
    def boom(*a):
        raise OSError("down")

    client._http = boom
    with pytest.raises(AnointedSsoError) as err:
        client.verify_logout_token(provider.logout_token())
    assert err.value.code in ("discovery", "http")


def test_process_statuses_and_replay(client, provider):
    store = InMemoryLogoutReplayStore(now=lambda: NOW)
    seen = []
    token = provider.logout_token()
    assert process_backchannel_logout(client, [], seen.append, store) == 400
    assert process_backchannel_logout(client, ["a", "b"], seen.append, store) == 400
    assert process_backchannel_logout(client, [" "], seen.append, store) == 400
    assert process_backchannel_logout(client, ["garbage"], seen.append, store) == 400
    assert process_backchannel_logout(client, [token], seen.append, store) == 200
    assert process_backchannel_logout(client, [token], seen.append, store) == 200  # replay: no second action
    assert [s.jti for s in seen] == ["jti-1"]


def test_handler_failure_returns_500_and_allows_retry(client, provider):
    store = InMemoryLogoutReplayStore(now=lambda: NOW)
    token = provider.logout_token()
    calls = []

    def flaky(logout):
        calls.append(logout.jti)
        if len(calls) == 1:
            raise RuntimeError("db down")

    assert process_backchannel_logout(client, [token], flaky, store) == 500
    assert process_backchannel_logout(client, [token], flaky, store) == 200
    assert calls == ["jti-1", "jti-1"]


def test_jwks_failure_returns_500(client, provider):
    def boom(*a):
        raise OSError("down")

    client._http = boom
    assert process_backchannel_logout(client, [provider.logout_token()], lambda l: None, InMemoryLogoutReplayStore()) == 500


def test_replay_store_expiry():
    clock = [100.0]
    store = InMemoryLogoutReplayStore(now=lambda: clock[0])
    assert store.try_add("a", 200) is True
    assert store.try_add("a", 200) is False
    clock[0] = 201
    assert store.try_add("a", 300) is True
    store.remove("a")
    assert store.try_add("a", 300) is True
    with pytest.raises(ValueError):
        store.try_add("", 1)
