// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

package com.anointedautomation.sso

import java.net.URI
import java.net.URLDecoder
import org.junit.Assert.assertEquals
import org.junit.Assert.assertFalse
import org.junit.Assert.assertNotEquals
import org.junit.Assert.assertTrue
import org.junit.Assert.fail
import org.junit.Test

class AnointedSignInRequestTest {
    private val start = "https://api.example.com/auth/anointed/start"
    private val via = "https://api.example.com/auth/anointed/app-done"

    private fun query(url: String): Map<String, String> =
        URI(url).rawQuery.split("&").associate {
            URLDecoder.decode(it.substringBefore("="), "UTF-8") to URLDecoder.decode(it.substringAfter("="), "UTF-8")
        }

    private fun code(block: () -> Unit): String {
        try {
            block()
        } catch (e: AnointedSignInException) {
            return e.code
        }
        fail("expected AnointedSignInException")
        return ""
    }

    @Test fun httpsCallbackBuildsReturnToWithState() {
        val r = AnointedSignInRequest(start, AnointedCallback.Https("app.example.com", "/anointed/done"), "abc")
        assertEquals("https://app.example.com/anointed/done", r.callbackUrl)
        assertEquals("https://app.example.com/anointed/done?aa_state=abc", r.returnTo)
        assertEquals("https://app.example.com/anointed/done?aa_state=abc", query(r.startUrl)["returnTo"])
    }

    @Test fun httpsPathWithoutLeadingSlashIsFixed() {
        val r = AnointedSignInRequest(start, AnointedCallback.Https("app.example.com", "done"), "abc")
        assertEquals("https://app.example.com/done", r.callbackUrl)
    }

    @Test fun schemeCallbackGoesThroughVia() {
        val r = AnointedSignInRequest(start, AnointedCallback.Scheme("myapp", via), "abc")
        assertEquals("myapp://anointed-sign-in", r.callbackUrl)
        assertEquals("$via?aa_state=abc", r.returnTo)
    }

    @Test fun refusesNonHttpsVia() {
        val c = code { AnointedSignInRequest(start, AnointedCallback.Scheme("myapp", "http://api.example.com/x"), "abc") }
        assertEquals(AnointedSignInException.CONFIG, c)
    }

    @Test fun replacesAnExistingReturnTo() {
        val s = "https://api.example.com/start?returnTo=/evil&x=1"
        val r = AnointedSignInRequest(s, AnointedCallback.Scheme("myapp", via), "abc")
        val pairs = URI(r.startUrl).rawQuery.split("&")
        assertEquals(1, pairs.count { it.startsWith("returnTo=") })
        assertEquals("1", query(r.startUrl)["x"])
    }

    @Test fun refusesHttpStart() {
        val c = code { AnointedSignInRequest("http://api.example.com/start", AnointedCallback.Scheme("myapp", via), "abc") }
        assertEquals(AnointedSignInException.CONFIG, c)
    }

    @Test fun refusesBadScheme() {
        for (bad in listOf("https", "http", "1bad", "")) {
            val c = code { AnointedSignInRequest(start, AnointedCallback.Scheme(bad, via), "abc") }
            assertEquals(AnointedSignInException.CONFIG, c)
        }
    }

    @Test fun refusesEmptyState() {
        val c = code { AnointedSignInRequest(start, AnointedCallback.Scheme("myapp", via), "") }
        assertEquals(AnointedSignInException.CONFIG, c)
    }

    @Test fun parseCallbackReturnsParamsWithoutState() {
        val r = AnointedSignInRequest(start, AnointedCallback.Scheme("myapp", via), "abc")
        assertEquals(mapOf("code" to "xyz"), r.parseCallback("myapp://anointed-sign-in?aa_state=abc&code=xyz"))
    }

    @Test fun parseCallbackWorksForHttps() {
        val r = AnointedSignInRequest(start, AnointedCallback.Https("app.example.com", "/anointed/done"), "abc")
        assertEquals(mapOf("code" to "a b"), r.parseCallback("https://app.example.com/anointed/done?aa_state=abc&code=a%20b"))
    }

    @Test fun parseCallbackRejectsWrongState() {
        val r = AnointedSignInRequest(start, AnointedCallback.Scheme("myapp", via), "abc")
        assertEquals(AnointedSignInException.STATE_MISMATCH, code { r.parseCallback("myapp://anointed-sign-in?aa_state=nope&code=xyz") })
    }

    @Test fun parseCallbackRejectsMissingState() {
        val r = AnointedSignInRequest(start, AnointedCallback.Scheme("myapp", via), "abc")
        assertEquals(AnointedSignInException.MISSING_STATE, code { r.parseCallback("myapp://anointed-sign-in?code=xyz") })
    }

    @Test fun parseCallbackSurfacesProviderError() {
        val r = AnointedSignInRequest(start, AnointedCallback.Scheme("myapp", via), "abc")
        try {
            r.parseCallback("myapp://anointed-sign-in?aa_state=abc&error=access_denied")
            fail("expected exception")
        } catch (e: AnointedSignInException) {
            assertEquals(AnointedSignInException.PROVIDER_ERROR, e.code)
            assertEquals("access_denied", e.message)
        }
    }

    @Test fun errorWithWrongStateIsAStateMismatch() {
        val r = AnointedSignInRequest(start, AnointedCallback.Scheme("myapp", via), "abc")
        assertEquals(AnointedSignInException.STATE_MISMATCH, code { r.parseCallback("myapp://anointed-sign-in?aa_state=bad&error=x") })
    }

    @Test fun parseCallbackRejectsForeignUrl() {
        val r = AnointedSignInRequest(start, AnointedCallback.Https("app.example.com", "/anointed/done"), "abc")
        assertEquals(AnointedSignInException.INVALID_CALLBACK, code { r.parseCallback("https://evil.example.com/anointed/done?aa_state=abc") })
        assertEquals(AnointedSignInException.INVALID_CALLBACK, code { r.parseCallback("https://app.example.com/other?aa_state=abc") })
    }

    @Test fun constantTimeEquals() {
        assertTrue(AnointedSignInRequest.constantTimeEquals("abc", "abc"))
        assertFalse(AnointedSignInRequest.constantTimeEquals("abc", "abd"))
        assertFalse(AnointedSignInRequest.constantTimeEquals("abc", "abcd"))
    }

    @Test fun randomStateIsUrlSafeAndUnique() {
        val a = AnointedSignInRequest.randomState()
        val b = AnointedSignInRequest.randomState()
        assertNotEquals(a, b)
        assertEquals(43, a.length)
        assertFalse(a.contains("+") || a.contains("/") || a.contains("="))
    }
}
