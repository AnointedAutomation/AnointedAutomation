# Copyright © Anointed Automation Corporation., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
"""Django adapter end to end (minimal in-test settings)."""

from __future__ import annotations

from urllib.parse import parse_qs, urlsplit

import pytest

django = pytest.importorskip("django")

from django.conf import settings  # noqa: E402

if not settings.configured:
    settings.configure(
        DEBUG=False,
        SECRET_KEY="test",
        ROOT_URLCONF="tests.django_urls",
        ALLOWED_HOSTS=["app.example", "testserver"],
        MIDDLEWARE=["django.middleware.csrf.CsrfViewMiddleware"],
        USE_TZ=True,
    )
    django.setup()

from django.http import HttpResponseRedirect  # noqa: E402
from django.test import Client  # noqa: E402

from anointedautomation_sso.adapters.django import create_views  # noqa: E402

from . import django_urls  # noqa: E402


@pytest.fixture()
def wired(client):
    log = {"signed_in": [], "logouts": []}

    def on_signed_in(request, result):
        log["signed_in"].append(result)
        return HttpResponseRedirect(result.return_to or "/")

    django_urls.set_views(create_views(client, on_signed_in=on_signed_in, on_logout=log["logouts"].append))
    return Client(enforce_csrf_checks=True, HTTP_HOST="app.example"), log


def test_round_trip_and_logout(wired, provider):
    http, log = wired
    start = http.get("/auth/anointed/start/", {"returnTo": "/dash"})
    assert start.status_code == 302 and start["Cache-Control"] == "no-store"
    morsel = start.cookies["aa_sso_flow"]
    assert morsel["httponly"] and morsel["samesite"] == "Lax" and morsel["max-age"] == 600
    q = parse_qs(urlsplit(start["Location"]).query)
    provider.pending_nonce = q["nonce"][0]
    done = http.get("/auth/anointed/callback/", {"code": "abc", "state": q["state"][0]})
    assert done.status_code == 302 and done["Location"] == "/dash"
    assert log["signed_in"][0].user.sub == "user-1"
    assert done.cookies["aa_sso_flow"]["max-age"] == 0

    token = provider.logout_token()
    # CSRF is enforced on the client, yet the back-channel route must accept a cookie-less provider POST.
    assert http.post("/auth/anointed/backchannel-logout/", {"logout_token": token}).status_code == 200
    assert http.post("/auth/anointed/backchannel-logout/", {"logout_token": token}).status_code == 200
    assert len(log["logouts"]) == 1
    assert http.post("/auth/anointed/backchannel-logout/", {"logout_token": "bad"}).status_code == 400
    assert http.get("/auth/anointed/backchannel-logout/").status_code == 405


def test_callback_failure_400(wired):
    http, log = wired
    r = http.get("/auth/anointed/callback/", {"code": "abc", "state": "x"})
    assert r.status_code == 400 and not log["signed_in"]
