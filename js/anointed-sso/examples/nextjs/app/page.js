// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

import { cookies } from "next/headers";
import { SESSION_COOKIE, openSession } from "../lib/session.js";

export const dynamic = "force-dynamic";

const buttonStyle = {
  display: "inline-flex",
  alignItems: "center",
  gap: 10,
  padding: "10px 16px",
  border: "1px solid #d4d4d4",
  borderRadius: 8,
  background: "#fff",
  color: "#111",
  font: "600 15px system-ui, sans-serif",
  textDecoration: "none",
};

/**
 * @summary    Home page: the sign-in button, or who is signed in.
 * @returns {Promise<JSX.Element>}
 */
export default async function Home() {
  const jar = await cookies();
  const cookie = jar.get(SESSION_COOKIE);
  const user = await openSession(cookie === undefined ? undefined : cookie.value);

  if (user === null) {
    return (
      <main>
        <a href="/api/auth/anointed/start" style={buttonStyle}>
          <img src="https://www.anointedautomation.net/favicon.svg" alt="" width={20} height={20} />
          <span>Sign in with Anointed Automation</span>
        </a>
      </main>
    );
  }
  return (
    <main>
      <p>
        Signed in as <code>{user.sub}</code> {user.email === null ? null : `(${user.email})`}
      </p>
      <form method="post" action="/api/auth/logout">
        <button type="submit">Sign out</button>
      </form>
    </main>
  );
}
