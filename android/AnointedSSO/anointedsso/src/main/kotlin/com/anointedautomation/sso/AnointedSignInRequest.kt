// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

package com.anointedautomation.sso

import java.net.URI
import java.net.URLDecoder
import java.net.URLEncoder
import java.security.MessageDigest
import java.security.SecureRandom

/** Errors raised by the kit. Every code is final; none is retried for you. */
class AnointedSignInException(val code: String, message: String) : Exception(message) {
    companion object {
        /** A configuration value is unusable (the message says which). */
        const val CONFIG = "config"
        /** The person closed the Custom Tab before finishing. */
        const val CANCELLED = "cancelled"
        /** The callback carried a different state than the request sent, so it is not ours. */
        const val STATE_MISMATCH = "state_mismatch"
        /** The callback carried no state at all. */
        const val MISSING_STATE = "missing_state"
        /** Your server sent the person back with an error query parameter (the message is its code). */
        const val PROVIDER_ERROR = "provider_error"
        /** A callback arrived but no sign-in was started by this app. */
        const val NO_PENDING_REQUEST = "no_pending_request"
        /** The intent is not the callback URL this kit was configured with. */
        const val INVALID_CALLBACK = "invalid_callback"
        /** No browser could open the Custom Tab. */
        const val PRESENTATION = "presentation"
    }
}

/** How your server hands control back to the app. */
sealed class AnointedCallback {
    /**
     * An Android App Link on your own domain (verified through assetlinks.json). Preferred: nothing else on
     * the device can claim it.
     */
    data class Https(val host: String, val path: String) : AnointedCallback()

    /**
     * A custom URL scheme. The server kit only returns to https URLs, so [via] is an https route on your
     * server that forwards every query parameter to "<scheme>://anointed-sign-in" (see the README).
     */
    data class Scheme(val scheme: String, val via: String) : AnointedCallback()
}

/** One sign-in attempt: the URL to open and the state that must come back. Pure JVM, so it is testable anywhere. */
class AnointedSignInRequest(
    start: String,
    val callback: AnointedCallback,
    val state: String = randomState(),
) {
    /** The start route with the returnTo query parameter added. */
    val startUrl: String

    /** The URL the app itself is finally handed (without the state parameter). */
    val callbackUrl: String

    /** The URL your server finishes sign-in on, with the state; this is the returnTo sent to the start route. */
    val returnTo: String

    init {
        val startUri = parseUri(start, "start URL must be an absolute https URL")
        if (startUri.scheme != "https" || startUri.host.isNullOrEmpty()) {
            throw config("start URL must be an absolute https URL")
        }
        if (state.isEmpty()) throw config("state must not be empty")

        val returnBase: String
        when (callback) {
            is AnointedCallback.Https -> {
                if (callback.host.isEmpty() || callback.host.any { it == '/' || it == ':' || it.isWhitespace() }) {
                    throw config("callback host is not a valid host name")
                }
                val path = if (callback.path.startsWith("/")) callback.path else "/" + callback.path
                callbackUrl = "https://" + callback.host + path
                returnBase = callbackUrl
            }
            is AnointedCallback.Scheme -> {
                val s = callback.scheme
                val valid = s.isNotEmpty() && s[0].isLetter() &&
                    s.all { it.isLetterOrDigit() || it == '+' || it == '-' || it == '.' }
                if (!valid || s.lowercase() == "http" || s.lowercase() == "https") {
                    throw config("callback scheme '" + s + "' is not a valid custom scheme")
                }
                val viaUri = parseUri(callback.via, "the scheme callback's via URL must be an absolute https URL")
                if (viaUri.scheme != "https" || viaUri.host.isNullOrEmpty()) {
                    throw config("the scheme callback's via URL must be an absolute https URL")
                }
                callbackUrl = s + "://anointed-sign-in"
                returnBase = stripQueryAndFragment(callback.via)
            }
        }

        returnTo = returnBase + "?" + STATE_ITEM + "=" + encode(state)

        val rawQuery = startUri.rawQuery
        val kept = if (rawQuery.isNullOrEmpty()) emptyList() else
            rawQuery.split("&").filter { it.isNotEmpty() && decode(it.substringBefore("=")) != "returnTo" }
        val base = stripQueryAndFragment(start)
        startUrl = base + "?" + (kept + ("returnTo=" + encode(returnTo))).joinToString("&")
    }

    /**
     * Checks a callback URL and returns its query parameters (minus the state). Throws when the URL is not
     * this request's callback, the state is missing or does not match, or your server reported an error.
     */
    fun parseCallback(url: String): Map<String, String> {
        val uri = parseUri(url, "callback is not a valid URL")
        if (!matchesCallback(uri)) {
            throw AnointedSignInException(AnointedSignInException.INVALID_CALLBACK, "the URL is not this sign-in's callback")
        }
        val values = LinkedHashMap<String, String>()
        val rawQuery = uri.rawQuery
        if (!rawQuery.isNullOrEmpty()) {
            for (pair in rawQuery.split("&")) {
                if (pair.isEmpty()) continue
                val name = decode(pair.substringBefore("="))
                val value = if (pair.contains("=")) decode(pair.substringAfter("=")) else ""
                values[name] = value
            }
        }
        val returned = values[STATE_ITEM]
            ?: throw AnointedSignInException(AnointedSignInException.MISSING_STATE, "the callback carried no " + STATE_ITEM)
        if (!constantTimeEquals(returned, state)) {
            throw AnointedSignInException(AnointedSignInException.STATE_MISMATCH, "the callback state does not match this sign-in")
        }
        val error = values["error"]
        if (error != null) throw AnointedSignInException(AnointedSignInException.PROVIDER_ERROR, error)
        values.remove(STATE_ITEM)
        return values
    }

    private fun matchesCallback(uri: URI): Boolean = when (callback) {
        is AnointedCallback.Https -> {
            val path = if (callback.path.startsWith("/")) callback.path else "/" + callback.path
            uri.scheme == "https" && uri.host.equals(callback.host, ignoreCase = true) && uri.path == path
        }
        is AnointedCallback.Scheme ->
            uri.scheme.equals(callback.scheme, ignoreCase = true) && uri.host.equals("anointed-sign-in", ignoreCase = true)
    }

    companion object {
        /** Query parameter carrying the app's state through your server and back. */
        const val STATE_ITEM = "aa_state"

        /** 32 random bytes, base64url without padding (43 characters). */
        fun randomState(): String {
            val bytes = ByteArray(32)
            SecureRandom().nextBytes(bytes)
            return base64Url(bytes)
        }

        private const val ALPHABET = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_"

        /** base64url without padding (java.util.Base64 needs API 26, the kit supports API 24). */
        private fun base64Url(bytes: ByteArray): String {
            val out = StringBuilder()
            var i = 0
            while (i < bytes.size) {
                val b0 = bytes[i].toInt() and 0xff
                val b1 = if (i + 1 < bytes.size) bytes[i + 1].toInt() and 0xff else 0
                val b2 = if (i + 2 < bytes.size) bytes[i + 2].toInt() and 0xff else 0
                out.append(ALPHABET[b0 shr 2])
                out.append(ALPHABET[((b0 and 3) shl 4) or (b1 shr 4)])
                if (i + 1 < bytes.size) out.append(ALPHABET[((b1 and 15) shl 2) or (b2 shr 6)])
                if (i + 2 < bytes.size) out.append(ALPHABET[b2 and 63])
                i += 3
            }
            return out.toString()
        }

        /** Constant-time string comparison. */
        fun constantTimeEquals(a: String, b: String): Boolean =
            MessageDigest.isEqual(a.toByteArray(Charsets.UTF_8), b.toByteArray(Charsets.UTF_8))

        private fun config(message: String) = AnointedSignInException(AnointedSignInException.CONFIG, message)

        private fun parseUri(value: String, message: String): URI =
            try {
                URI(value)
            } catch (e: java.net.URISyntaxException) {
                throw config(message)
            }

        private fun stripQueryAndFragment(url: String): String = url.substringBefore('#').substringBefore('?')

        private fun encode(value: String): String = URLEncoder.encode(value, "UTF-8").replace("+", "%20")

        private fun decode(value: String): String = URLDecoder.decode(value, "UTF-8")
    }
}
