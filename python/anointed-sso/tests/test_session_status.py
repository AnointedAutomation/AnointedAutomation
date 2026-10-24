# Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
"""Session status: fail open, caching, machine token reuse and refresh."""

from __future__ import annotations

import json

import pytest

from anointedautomation_sso import AnointedSsoError, SessionOutcome, SessionStatusClient
from anointedautomation_sso.http import HttpResponse
from anointedautomation_sso.session_status import parse_status

BASE = "https://api.anointedautomation.net/"


class Api:
    def __init__(self):
        self.clock = [1000.0]
        self.calls = []
        self.token_n = 0
        self.status_answers = []
        self.token_status = 200
        self.raise_on_status = False

    def __call__(self, method, url, headers, data, timeout):
        self.calls.append((method, url, headers, data))
        if url.endswith("api/auth/machine-token"):
            if self.token_status != 200:
                return HttpResponse(self.token_status, "")
            self.token_n += 1
            return HttpResponse(200, json.dumps({"Data": {"Token": f"mt{self.token_n}", "ExpiresAt": "1970-01-01T00:20:00Z"}}))
        if self.raise_on_status:
            raise OSError("down")
        return self.status_answers.pop(0)


def make(api, **kw):
    return SessionStatusClient(api_key="key", app_name="app", http=api, now=lambda: api.clock[0], **kw)


def ok(active, reason=None):
    return HttpResponse(200, json.dumps({"success": True, "data": {"active": active, "reason": reason}}))


def test_config_required():
    for kw in ({"api_key": "", "app_name": "a"}, {"api_key": "k", "app_name": ""}, {"api_key": "k", "app_name": "a", "base_url": "http://x/"}, {"api_key": "k", "app_name": "a", "base_url": "https://x"}):
        with pytest.raises(AnointedSsoError):
            SessionStatusClient(**kw)


def test_active_and_ended_and_cache():
    api = Api()
    api.status_answers = [ok(True), ok(False, "banned")]
    c = make(api)
    assert c.check("u1").outcome is SessionOutcome.ACTIVE
    assert c.check("u1").outcome is SessionOutcome.ACTIVE  # cached
    ended = c.check("u2", grant=True)
    assert ended.should_end_session and ended.reason == "banned"
    status_calls = [x for x in api.calls if "session-status" in x[1]]
    assert len(status_calls) == 2
    assert "userId=u2&grant=true" in status_calls[1][1]
    assert status_calls[0][2]["authorization"] == "Bearer mt1"
    assert api.token_n == 1  # machine token reused


def test_cache_expires():
    api = Api()
    api.status_answers = [ok(True), ok(False)]
    c = make(api, answer_cache_seconds=60)
    assert c.check("u").outcome is SessionOutcome.ACTIVE
    api.clock[0] += 61
    assert c.check("u").should_end_session


def test_fails_open():
    api = Api()
    c = make(api)
    api.raise_on_status = True
    assert c.check("u").outcome is SessionOutcome.UNKNOWN and not c.check("u").should_end_session
    api.raise_on_status = False
    api.status_answers = [HttpResponse(500, ""), HttpResponse(200, "not json"), HttpResponse(200, json.dumps({"success": False})), HttpResponse(200, json.dumps({"data": {"active": "yes"}}))]
    for _ in range(4):
        result = c.check("u")
        assert result.outcome is SessionOutcome.UNKNOWN and result.error and not result.should_end_session
    api.token_status = 500
    api.token_n = 0
    c2 = make(api)
    assert c2.check("u").outcome is SessionOutcome.UNKNOWN


def test_401_refreshes_machine_token_once():
    api = Api()
    api.status_answers = [HttpResponse(401, ""), ok(True)]
    c = make(api)
    assert c.check("u").outcome is SessionOutcome.ACTIVE
    assert api.token_n == 2
    status_calls = [x for x in api.calls if "session-status" in x[1]]
    assert [x[2]["authorization"] for x in status_calls] == ["Bearer mt1", "Bearer mt2"]


def test_blank_user_raises():
    with pytest.raises(ValueError):
        make(Api()).check(" ")


def test_parse_status_case_insensitive():
    assert parse_status('{"Success":true,"Data":{"Active":false,"Reason":"x"}}').should_end_session
