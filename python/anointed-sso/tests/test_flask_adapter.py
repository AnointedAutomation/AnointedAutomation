# Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
"""Flask adapter end to end."""

from __future__ import annotations

from urllib.parse import parse_qs, urlsplit

import pytest

flask = pytest.importorskip("flask")

from anointedautomation_sso.adapters.flask import create_blueprint  # noqa: E402

from .conftest import REDIRECT_URI, start_flow  # noqa: E402


@pytest.fixture()
def app_and_log(client):
    log = {"signed_in": [], "logouts": []}

    def on_signed_in(result):
        log["signed_in"].append(result)
        return flask.redirect(result.return_to or "/")

    app = flask.Flask(__name__)
    app.register_blueprint(create_blueprint(client, on_signed_in=on_signed_in, on_logout=log["logouts"].append), url_prefix="/auth/anointed")
    return app, log


def test_sign_in_round_trip(app_and_log, client, provider):
    app, log = app_and_log
    http = app.test_client()
    response = http.get("/auth/anointed/start?returnTo=/dash", base_url="https://app.example")
    assert response.status_code == 302 and response.headers["Cache-Control"] == "no-store"
    q = parse_qs(urlsplit(response.headers["Location"]).query)
    provider.pending_nonce = q["nonce"][0]
    cookie = http.get_cookie("aa_sso_flow", domain="app.example")
    assert cookie is not None and cookie.http_only if hasattr(cookie, "http_only") else True
    done = http.get(f"/auth/anointed/callback?code=abc&state={q['state'][0]}", base_url="https://app.example")
    assert done.status_code == 302 and done.headers["Location"] == "/dash"
    assert log["signed_in"][0].user.sub == "user-1"
    assert "Max-Age=0" in "".join(done.headers.getlist("Set-Cookie")) or "Expires" in "".join(done.headers.getlist("Set-Cookie"))


def test_callback_failure_is_400_and_clears_cookie(app_and_log):
    app, log = app_and_log
    response = app.test_client().get("/auth/anointed/callback?code=abc&state=nope", base_url="https://app.example")
    assert response.status_code == 400 and not log["signed_in"]
    assert "aa_sso_flow=;" in response.headers["Set-Cookie"]


def test_backchannel_logout(app_and_log, provider):
    app, log = app_and_log
    http = app.test_client()
    token = provider.logout_token()
    r = http.post("/auth/anointed/backchannel-logout", data={"logout_token": token})
    assert r.status_code == 200 and r.headers["Cache-Control"] == "no-store"
    assert http.post("/auth/anointed/backchannel-logout", data={"logout_token": token}).status_code == 200
    assert len(log["logouts"]) == 1
    assert http.post("/auth/anointed/backchannel-logout", data={"logout_token": "bad"}).status_code == 400
    assert http.post("/auth/anointed/backchannel-logout", data={}).status_code == 400
