// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Deno smoke test (the Base44 backend runtime): the full start -> authorize -> callback flow through the Web
// adapter against a tiny provider served by Deno.serve, using only WebCrypto. Run:
//   deno test --allow-net test/deno/smoke.ts

import { createAnointedClient, createWebHandlers } from "../../src/index.js";

const CLIENT_ID = "aa_deno_smoke";
const CLIENT_SECRET = "test-only-secret-not-real";
const REDIRECT_URI = "http://localhost:8787/auth/anointed/callback";

/**
 * @summary    Unpadded base64url of bytes.
 * @param {Uint8Array} bytes
 * @returns {string}
 */
function b64url(bytes: Uint8Array): string {
  let binary = "";
  for (const b of bytes) binary += String.fromCharCode(b);
  return btoa(binary).replace(/\+/g, "-").replace(/\//g, "_").replace(/=+$/, "");
}

/**
 * @summary    Throw when a condition does not hold.
 * @param {boolean} condition
 * @param {string} message
 * @returns {void}
 */
function check(condition: boolean, message: string): void {
  if (!condition) throw new Error(message);
}

Deno.test("full sign-in flow on Deno through the Web adapter", async () => {
  const keyPair = await crypto.subtle.generateKey(
    { name: "RSASSA-PKCS1-v1_5", modulusLength: 2048, publicExponent: new Uint8Array([1, 0, 1]), hash: "SHA-256" },
    true,
    ["sign", "verify"],
  );
  const publicJwk = await crypto.subtle.exportKey("jwk", keyPair.publicKey);
  const codes = new Map<string, { nonce: string; challenge: string }>();
  let issuer = "";

  const signJwt = async (claims: Record<string, unknown>): Promise<string> => {
    const enc = new TextEncoder();
    const head = b64url(enc.encode(JSON.stringify({ alg: "RS256", kid: "d1", typ: "JWT" })));
    const body = b64url(enc.encode(JSON.stringify(claims)));
    const sig = await crypto.subtle.sign("RSASSA-PKCS1-v1_5", keyPair.privateKey, enc.encode(`${head}.${body}`));
    return `${head}.${body}.${b64url(new Uint8Array(sig))}`;
  };

  const server = Deno.serve({ port: 0, hostname: "127.0.0.1", onListen() {} }, async (req) => {
    const url = new URL(req.url);
    if (url.pathname === "/.well-known/openid-configuration") {
      return Response.json({
        issuer,
        authorization_endpoint: `${issuer}connect/authorize`,
        token_endpoint: `${issuer}connect/token`,
        jwks_uri: `${issuer}.well-known/jwks`,
        code_challenge_methods_supported: ["S256"],
      });
    }
    if (url.pathname === "/.well-known/jwks") return Response.json({ keys: [{ ...publicJwk, kid: "d1", use: "sig", alg: "RS256" }] });
    if (url.pathname === "/connect/token") {
      const form = new URLSearchParams(await req.text());
      const record = codes.get(String(form.get("code")));
      const digest = await crypto.subtle.digest("SHA-256", new TextEncoder().encode(String(form.get("code_verifier"))));
      if (record === undefined || b64url(new Uint8Array(digest)) !== record.challenge || form.get("client_secret") !== CLIENT_SECRET) {
        return Response.json({ error: "invalid_grant" }, { status: 400 });
      }
      const now = Math.floor(Date.now() / 1000);
      const idToken = await signJwt({
        iss: issuer,
        aud: CLIENT_ID,
        sub: "deno-user",
        iat: now,
        exp: now + 300,
        nonce: record.nonce,
        email: "deno@example.com",
        email_verified: true,
      });
      return Response.json({ access_token: "a", token_type: "Bearer", expires_in: 900, id_token: idToken });
    }
    return new Response("not found", { status: 404 });
  });
  issuer = `http://127.0.0.1:${server.addr.port}/`;

  try {
    const client = createAnointedClient({ issuer, clientId: CLIENT_ID, clientSecret: CLIENT_SECRET, redirectUri: REDIRECT_URI });
    const handlers = createWebHandlers(client, {
      onSignedIn: (result: { user: { sub: string; email: string } }) => Response.json(result.user),
    });

    const startRes = await handlers.start(new Request("http://localhost:8787/auth/anointed/start"));
    check(startRes.status === 302, `start returned ${startRes.status}`);
    const authorize = new URL(String(startRes.headers.get("location")));
    const flowCookie = startRes.headers.getSetCookie()[0].split(";")[0];

    // Play the provider's authorize step: remember nonce + challenge, redirect back with a code.
    const code = crypto.randomUUID();
    codes.set(code, {
      nonce: String(authorize.searchParams.get("nonce")),
      challenge: String(authorize.searchParams.get("code_challenge")),
    });
    const callback = new URL(REDIRECT_URI);
    callback.searchParams.set("code", code);
    callback.searchParams.set("state", String(authorize.searchParams.get("state")));
    callback.searchParams.set("iss", issuer);

    const res = await handlers.callback(new Request(callback, { headers: { cookie: flowCookie } }));
    check(res.status === 200, `callback returned ${res.status}: ${await res.clone().text()}`);
    const user = await res.json();
    check(user.sub === "deno-user", `unexpected sub ${user.sub}`);
    check(user.email === "deno@example.com", `unexpected email ${user.email}`);
    check(res.headers.getSetCookie().some((c) => c.startsWith("aa_sso_flow=;")), "flow cookie was not cleared");
  } finally {
    await server.shutdown();
  }
});
