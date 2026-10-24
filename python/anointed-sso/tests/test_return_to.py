# Copyright © Anointed Automation Corporation., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
"""Open-redirect safe return targets."""

from __future__ import annotations

import pytest

from anointedautomation_sso import AnointedSsoError, create_return_to_validator, safe_return_to


@pytest.mark.parametrize("value", ["/account", "/a?b=1#c", "/"])
def test_relative_paths_kept(value):
    assert safe_return_to(value) == value


@pytest.mark.parametrize("value", [None, "", "account", "//evil.example", "/\\evil", "https://evil.example/", "/a\nb", "javascript:alert(1)", "/" + "a" * 2100, 5])
def test_unsafe_default_rejected(value):
    assert safe_return_to(value) is None


def test_allowed_origins():
    v = create_return_to_validator(["https://www.example.com", "https://*.example.org", "http://localhost:3000"])
    assert v("https://www.example.com/x?y=1") == "https://www.example.com/x?y=1"
    assert v("https://a.b.example.org/") == "https://a.b.example.org/"
    assert v("http://localhost:3000/dev") == "http://localhost:3000/dev"
    assert v("/rel") == "/rel"
    assert v("https://example.org/") is None  # bare base not covered by the wildcard
    assert v("https://evil.example/") is None
    assert v("https://www.example.com:8443/") is None
    assert v("http://www.example.com/") is None
    assert v("http://localhost:4000/") is None
    assert v("https://user:pw@www.example.com/") is None
    assert v("//www.example.com/") is None
    assert v("javascript://www.example.com/%0aalert(1)") is None
    assert v("https://www.example.com/a b") is None


def test_predicate_still_https_only():
    v = create_return_to_validator(is_allowed_return_to=lambda url: True)
    assert v("https://anything.example/p") == "https://anything.example/p"
    assert v("http://anything.example/p") is None
    assert v("data:text/html,x") is None


@pytest.mark.parametrize("bad", ["http://example.com", "https://*.com", "https://x.example/path", "ftp://x.example", "", "https://user@x.example"])
def test_bad_origin_config_raises(bad):
    with pytest.raises(AnointedSsoError):
        create_return_to_validator([bad])
