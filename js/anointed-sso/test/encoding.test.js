// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

import { test } from "node:test";
import assert from "node:assert/strict";
import {
  base64UrlToBytes,
  base64UrlToJson,
  bytesToBase64Url,
  jsonToBase64Url,
} from "../src/encoding.js";
import { randomBase64Url, sha256Base64Url, timingSafeEqual } from "../src/crypto.js";
import { parseCookieHeader, serializeCookie } from "../src/cookies.js";

test("base64url round-trips every byte value and every length remainder", () => {
  for (let length = 0; length < 70; length += 1) {
    const bytes = new Uint8Array(length);
    for (let i = 0; i < length; i += 1) bytes[i] = (i * 37 + length) % 256;
    const encoded = bytesToBase64Url(bytes);
    assert.match(encoded, /^[A-Za-z0-9_-]*$/);
    assert.deepEqual(base64UrlToBytes(encoded), bytes);
  }
});

test("base64url agrees with Node's encoder", () => {
  const bytes = new Uint8Array([0xfb, 0xff, 0xbf, 0x00, 0x3e, 0x3f]);
  assert.equal(bytesToBase64Url(bytes), Buffer.from(bytes).toString("base64url"));
});

test("base64url rejects non-strings, bad characters and impossible lengths", () => {
  assert.throws(() => base64UrlToBytes(42), TypeError);
  assert.throws(() => base64UrlToBytes("ab+c"), TypeError);
  assert.throws(() => base64UrlToBytes("ab/c"), TypeError);
  assert.throws(() => base64UrlToBytes("a"), TypeError);
  assert.throws(() => base64UrlToBytes("abcde"), TypeError);
});

test("JSON round-trips unicode", () => {
  const value = { name: "Jésus ✝️", n: 1, nested: [true, null] };
  assert.deepEqual(base64UrlToJson(jsonToBase64Url(value)), value);
});

test("sha256Base64Url matches the RFC 7636 appendix B PKCE example", async () => {
  assert.equal(
    await sha256Base64Url("dBjftJeZ4CVP-mB92K27uhbUJU1p1r_wW1gFWFOEjXk"),
    "E9Melhoa2OwvFrEMTJguCHaoeK1t8URWbuGJSstw-cM",
  );
});

test("randomBase64Url returns distinct values of the expected length", async () => {
  const a = await randomBase64Url(48);
  const b = await randomBase64Url(48);
  assert.equal(a.length, 64);
  assert.notEqual(a, b);
});

test("timingSafeEqual only accepts identical non-empty strings", () => {
  assert.equal(timingSafeEqual("abc", "abc"), true);
  assert.equal(timingSafeEqual("abc", "abd"), false);
  assert.equal(timingSafeEqual("abc", "abcd"), false);
  assert.equal(timingSafeEqual("", ""), false);
  assert.equal(timingSafeEqual(null, "abc"), false);
  assert.equal(timingSafeEqual("abc", undefined), false);
});

test("parseCookieHeader handles spacing, duplicates, quotes and bad encoding", () => {
  assert.deepEqual(parseCookieHeader(undefined), {});
  assert.deepEqual(parseCookieHeader(""), {});
  const parsed = parseCookieHeader(' a=1;b = two ; a=ignored; q="x%20y"; bad=%E0%A4%A; =nameless; flag');
  assert.equal(parsed.a, "1");
  assert.equal(parsed.b, "two");
  assert.equal(parsed.q, "x y");
  assert.equal(parsed.bad, "%E0%A4%A");
  assert.equal(Object.keys(parsed).length, 4);
});

test("serializeCookie writes every attribute and rejects bad names", () => {
  const header = serializeCookie("aa_sso_flow", "v a", {
    path: "/",
    maxAge: 600.9,
    httpOnly: true,
    secure: true,
    sameSite: "Lax",
    domain: "example.com",
  });
  assert.equal(header, "aa_sso_flow=v%20a; Path=/; Max-Age=600; Domain=example.com; HttpOnly; Secure; SameSite=Lax");
  const plain = serializeCookie("x", "", { path: "/a", maxAge: 0, httpOnly: false, secure: false, sameSite: "Strict" });
  assert.equal(plain, "x=; Path=/a; Max-Age=0; SameSite=Strict");
  assert.throws(() => serializeCookie("bad name", "v", { path: "/", maxAge: 1, httpOnly: true, secure: true, sameSite: "Lax" }), TypeError);
});
