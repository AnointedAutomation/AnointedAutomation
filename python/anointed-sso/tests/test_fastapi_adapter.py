# Copyright © Anointed Automation Corporation., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
"""FastAPI / Starlette adapter end to end."""

from __future__ import annotations

from urllib.parse import parse_qs, urlsplit

import pytest

pytest.importorskip("fastapi")
pytest.importorskip("httpx")

from fastapi import FastAPI  # noqa: E402
from fastapi.responses import RedirectResponse  # noqa: E402
from fastapi.testclient import TestClient  # noqa: E402
from starlette.applications import Starlette  # noqa: E402

from anointedautomation_sso.adapters.fastapi import create_router, create_routes  # noqa: E402


def build(client, async_callbacks):
    log = {"signed_in": [], "logouts": []}
    if async_callbacks:

        async def on_signed_in(request, result):
            log["signed_in"].append(result)
            return RedirectResponse(result.return_to or "/", status_code=302)

        async def on_logout(logout):
            log["logouts"].append(logout)

    else:

        def on_signed_in(request, result):
            log["signed_in"].append(result)
            return RedirectResponse(result.return_to or "/", status_code=302)

        def on_logout(logout):
            log["logouts"].append(logout)

    return on_signed_in, on_logout, log


def fastapi_app(client, async_callbacks):
    on_signed_in, on_logout, log = build(client, async_callbacks)
    app = FastAPI()
    app.include_router(create_router(client, on_signed_in=on_signed_in, on_logout=on_logout), prefix="/auth/anointed")
    return app, log


def starlette_app(client, async_callbacks):
    on_signed_in, on_logout, log = build(client, async_callbacks)
    from starlette.routing import Mount

    return Starlette(routes=[Mount("/auth/anointed", routes=create_routes(client, on_signed_in=on_signed_in, on_logout=on_logout))]), log


@pytest.mark.parametrize("factory", [fastapi_app, starlette_app])
@pytest.mark.parametrize("async_callbacks", [False, True])
def test_round_trip_and_logout(client, provider, factory, async_callbacks):
    app, log = factory(client, async_callbacks)
    http = TestClient(app, base_url="https://app.example", follow_redirects=False)
    start = http.get("/auth/anointed/start", params={"returnTo": "/dash"})
    assert start.status_code == 302 and start.headers["cache-control"] == "no-store"
    q = parse_qs(urlsplit(start.headers["location"]).query)
    provider.pending_nonce = q["nonce"][0]
    assert "aa_sso_flow" in http.cookies
    done = http.get("/auth/anointed/callback", params={"code": "abc", "state": q["state"][0]})
    assert done.status_code == 302 and done.headers["location"] == "/dash"
    assert log["signed_in"][0].user.email == "alex@example.com"
    assert "aa_sso_flow" not in http.cookies  # cleared

    token = provider.logout_token()
    assert http.post("/auth/anointed/backchannel-logout", data={"logout_token": token}).status_code == 200
    assert http.post("/auth/anointed/backchannel-logout", data={"logout_token": token}).status_code == 200
    assert len(log["logouts"]) == 1
    assert http.post("/auth/anointed/backchannel-logout", data={"logout_token": "bad"}).status_code == 400
    assert http.post("/auth/anointed/backchannel-logout", json={"logout_token": token}).status_code == 400
    assert http.post("/auth/anointed/backchannel-logout").status_code == 400


def test_callback_error_default_400(client):
    app, log = fastapi_app(client, False)
    http = TestClient(app, base_url="https://app.example", follow_redirects=False)
    r = http.get("/auth/anointed/callback", params={"code": "abc", "state": "x"})
    assert r.status_code == 400 and not log["signed_in"]


def test_failing_async_logout_returns_500(client, provider):
    async def boom(logout):
        raise RuntimeError("db")

    app = FastAPI()
    app.include_router(create_router(client, on_signed_in=lambda r, x: None, on_logout=boom), prefix="/a")
    http = TestClient(app, base_url="https://app.example")
    assert http.post("/a/backchannel-logout", data={"logout_token": provider.logout_token()}).status_code == 500
