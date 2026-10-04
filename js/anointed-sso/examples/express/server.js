// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// "Sign in with Anointed Automation" in an Express app, using express-session for YOUR app session.
// Run: create .env with the variables listed in the package README, then `npm install && npm start`.

import express from "express";
import session from "express-session";
import { ANOINTED_ISSUER, createAnointedClient, createExpressHandlers } from "@anointedautomation/sso";

/**
 * @summary    Read a required environment variable or crash at startup (no fallbacks).
 * @param {string} name
 * @returns {string}
 */
function requireEnv(name) {
  const value = process.env[name];
  if (value === undefined || value.trim() === "") throw new Error(`Missing required environment variable ${name}`);
  return value;
}

/**
 * @summary    Escape text for HTML.
 * @param {string} text
 * @returns {string}
 */
function escapeHtml(text) {
  return text.replace(/[&<>"']/g, (c) => ({ "&": "&amp;", "<": "&lt;", ">": "&gt;", '"': "&quot;", "'": "&#39;" })[c]);
}

const client = createAnointedClient({
  issuer: ANOINTED_ISSUER,
  clientId: requireEnv("ANOINTED_OAUTH_CLIENT_ID"),
  clientSecret: requireEnv("ANOINTED_OAUTH_CLIENT_SECRET"),
  redirectUri: requireEnv("ANOINTED_OAUTH_REDIRECT_URI"),
});

// Demo-only stores. In production use your database / Redis for sessions, the sub index and seen jti values.
const store = new session.MemoryStore();
const sessionIdsBySub = new Map();
const seenLogoutJti = new Map();

const app = express();
app.set("trust proxy", 1);
app.use(
  session({
    store,
    secret: requireEnv("SESSION_SECRET"),
    resave: false,
    saveUninitialized: false,
    cookie: { httpOnly: true, sameSite: "lax", secure: "auto" },
  }),
);

const anointed = createExpressHandlers(client, {
  onSignedIn(result, req, res) {
    // Look up or create YOUR user by result.user.sub here (never by email).
    req.session.regenerate((err) => {
      if (err) {
        res.status(500).send("Could not start a session.");
        return;
      }
      req.session.user = { sub: result.user.sub, email: result.user.email };
      const ids = sessionIdsBySub.get(result.user.sub) === undefined ? new Set() : sessionIdsBySub.get(result.user.sub);
      ids.add(req.session.id);
      sessionIdsBySub.set(result.user.sub, ids);
      res.redirect(result.returnTo === null ? "/" : result.returnTo);
    });
  },
  onError(err, req, res) {
    console.error("Anointed sign-in failed:", err.code, err.message);
    res.redirect(err.code === "access_denied" ? "/?signin=cancelled" : "/?signin=failed");
  },
  async onLogout(logout) {
    // Back-channel logout: the user disconnected this app or their account ended. End their sessions.
    if (seenLogoutJti.has(logout.jti)) return;
    seenLogoutJti.set(logout.jti, logout.exp);
    const ids = logout.sub === null ? undefined : sessionIdsBySub.get(logout.sub);
    if (ids === undefined) return;
    await Promise.all([...ids].map((id) => new Promise((resolve) => store.destroy(id, () => resolve()))));
    sessionIdsBySub.delete(logout.sub);
  },
});

app.get("/auth/anointed/start", anointed.start);
app.get("/auth/anointed/callback", anointed.callback);
app.post("/auth/anointed/backchannel-logout", anointed.backchannelLogout);

app.post("/logout", (req, res) => {
  req.session.destroy(() => res.redirect("/"));
});

app.get("/", (req, res) => {
  const user = req.session.user;
  const body =
    user === undefined
      ? `<a href="/auth/anointed/start" style="display:inline-flex;align-items:center;gap:10px;padding:10px 16px;border:1px solid #d4d4d4;border-radius:8px;color:#111;text-decoration:none;font:600 15px system-ui">
           <img src="https://www.anointedautomation.net/favicon.svg" alt="" width="20" height="20">
           <span>Sign in with Anointed Automation</span></a>`
      : `<p>Signed in as <code>${escapeHtml(user.sub)}</code> ${user.email === null ? "" : `(${escapeHtml(user.email)})`}</p>
         <form method="post" action="/logout"><button>Sign out</button></form>`;
  res.type("html").send(`<!doctype html><html lang="en"><head><meta charset="utf-8"><title>Anointed SSO Express example</title></head><body>${body}</body></html>`);
});

const port = Number(requireEnv("PORT"));
app.listen(port, () => console.log(`Listening on http://localhost:${port}`));
