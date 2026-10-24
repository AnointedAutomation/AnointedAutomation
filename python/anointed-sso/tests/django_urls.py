# Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
"""URL conf for the Django adapter tests; the real ``views.urls()`` are mounted per test."""

from django.urls import clear_url_caches, include, path

urlpatterns = []


def set_views(views):
    urlpatterns[:] = [path("auth/anointed/", include(views.urls()))]
    clear_url_caches()
