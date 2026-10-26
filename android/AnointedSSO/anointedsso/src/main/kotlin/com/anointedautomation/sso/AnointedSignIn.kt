// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

package com.anointedautomation.sso

import android.app.Activity
import android.content.Context
import android.content.Intent
import android.net.Uri
import androidx.browser.customtabs.CustomTabsIntent

/** Remembers the request between opening the Custom Tab and receiving the callback. */
interface AnointedPendingStore {
    fun save(request: AnointedSignInRequest)
    fun load(): AnointedSignInRequest?
    fun clear()
}

/** Default store: in memory. Keep the [AnointedSignIn] in a singleton or application scope. */
class AnointedMemoryStore : AnointedPendingStore {
    @Volatile private var pending: AnointedSignInRequest? = null
    override fun save(request: AnointedSignInRequest) { pending = request }
    override fun load(): AnointedSignInRequest? = pending
    override fun clear() { pending = null }
}

/**
 * Optional store that survives the app process being killed while the Custom Tab is open. Pass it as
 * `AnointedSignIn(start, callback, AnointedPreferencesStore(context))`.
 */
class AnointedPreferencesStore(context: Context) : AnointedPendingStore {
    private val prefs = context.applicationContext.getSharedPreferences("anointed_sso", Context.MODE_PRIVATE)

    override fun save(request: AnointedSignInRequest) {
        val cb = request.callback
        val editor = prefs.edit().putString("state", request.state).putString("start", request.startUrl)
        when (cb) {
            is AnointedCallback.Https -> editor.putString("kind", "https").putString("a", cb.host).putString("b", cb.path)
            is AnointedCallback.Scheme -> editor.putString("kind", "scheme").putString("a", cb.scheme).putString("b", cb.via)
        }
        editor.apply()
    }

    override fun load(): AnointedSignInRequest? {
        val state = prefs.getString("state", null) ?: return null
        val start = prefs.getString("start", null) ?: return null
        val a = prefs.getString("a", null) ?: return null
        val b = prefs.getString("b", null) ?: return null
        val callback = when (prefs.getString("kind", null)) {
            "https" -> AnointedCallback.Https(a, b)
            "scheme" -> AnointedCallback.Scheme(a, b)
            else -> return null
        }
        return AnointedSignInRequest(start, callback, state)
    }

    override fun clear() { prefs.edit().clear().apply() }
}

/**
 * "Sign in with Anointed Automation" for Android. The app never holds the client secret: [signIn] opens
 * your server's start route in a Custom Tab, and [handleCallback] checks the state that came back.
 */
class AnointedSignIn(
    private val startUrl: Uri,
    private val callback: AnointedCallback,
    private val store: AnointedPendingStore = AnointedMemoryStore(),
) {
    /** Opens the start route in a Custom Tab with a fresh aa_state. Throws [AnointedSignInException]. */
    fun signIn(activity: Activity) {
        val request = AnointedSignInRequest(startUrl.toString(), callback)
        store.save(request)
        try {
            CustomTabsIntent.Builder().build().launchUrl(activity, Uri.parse(request.startUrl))
        } catch (e: android.content.ActivityNotFoundException) {
            store.clear()
            throw AnointedSignInException(AnointedSignInException.PRESENTATION, "no browser can open the sign-in page")
        }
    }

    /**
     * Call from the activity that owns your callback intent-filter (onCreate and onNewIntent). Returns the
     * query parameters your server added (for example "code"), without the state. The pending request is
     * consumed whether or not it succeeds, so a callback can never be replayed.
     */
    fun handleCallback(intent: Intent): Map<String, String> {
        val data = intent.data ?: throw AnointedSignInException(AnointedSignInException.INVALID_CALLBACK, "the intent carries no URL")
        val request = store.load()
            ?: throw AnointedSignInException(AnointedSignInException.NO_PENDING_REQUEST, "no sign-in was started")
        store.clear()
        return request.parseCallback(data.toString())
    }

    /** True when this intent looks like a callback worth passing to [handleCallback]. */
    fun isCallback(intent: Intent): Boolean {
        val data = intent.data ?: return false
        return when (callback) {
            is AnointedCallback.Https -> data.scheme == "https" && data.host.equals(callback.host, ignoreCase = true)
            is AnointedCallback.Scheme -> data.scheme.equals(callback.scheme, ignoreCase = true)
        }
    }
}
