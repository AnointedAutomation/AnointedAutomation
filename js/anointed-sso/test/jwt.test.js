// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

import { test } from "node:test";
import assert from "node:assert/strict";
import crypto from "node:crypto";
import { decodeJwt, selectJwk, validateIdTokenClaims, verifyJwsRs256 } from "../src/jwt.js";

const ISSUER = "https://api.anointedautomation.net/";
const CLIENT = "aa_6ac0d092b22e3a5d3e042836";
const NOW = 1_800_000_000;
const SKEW = 60;

const pair = crypto.generateKeyPairSync("rsa", { modulusLength: 2048 });
const other = crypto.generateKeyPairSync("rsa", { modulusLength: 2048 });
const JWKS = { keys: [{ ...pair.publicKey.export({ format: "jwk" }), kid: "k1", use: "sig", alg: "RS256" }] };

/**
 * @summary    Sign a test token.
 * @param {object} claims
 * @param {object} [header]
 * @param {crypto.KeyObject} [key]
 * @returns {string}
 */
function sign(claims, header = { alg: "RS256", kid: "k1", typ: "JWT" }, key = pair.privateKey) {
  const h = Buffer.from(JSON.stringify(header)).toString("base64url");
  const p = Buffer.from(JSON.stringify(claims)).toString("base64url");
  const s = crypto.sign("RSA-SHA256", Buffer.from(`${h}.${p}`), key).toString("base64url");
  return `${h}.${p}.${s}`;
}

const good = { iss: ISSUER, aud: CLIENT, sub: "user-1", exp: NOW + 300, iat: NOW, nonce: "n-1" };
const opts = { issuer: ISSUER, clientId: CLIENT, nonce: "n-1", now: NOW, clockSkewSeconds: SKEW };

test("verifyJwsRs256 accepts a correctly signed token", async () => {
  const { payload, header } = await verifyJwsRs256(sign(good), JWKS);
  assert.equal(payload.sub, "user-1");
  assert.equal(header.kid, "k1");
});

test("verifyJwsRs256 rejects a token signed by another key", async () => {
  await assert.rejects(verifyJwsRs256(sign(good, undefined, other.privateKey), JWKS), /signature is invalid/);
});

test("verifyJwsRs256 rejects a payload changed after signing", async () => {
  const [h, , s] = sign(good).split(".");
  const forged = Buffer.from(JSON.stringify({ ...good, sub: "admin" })).toString("base64url");
  await assert.rejects(verifyJwsRs256(`${h}.${forged}.${s}`, JWKS), /signature is invalid/);
});

test("verifyJwsRs256 refuses alg none, HS256 and a missing alg", async () => {
  for (const alg of ["none", "HS256", "RS512", undefined]) {
    await assert.rejects(verifyJwsRs256(sign(good, { alg, kid: "k1" }), JWKS), /is not allowed/);
  }
});

test("verifyJwsRs256 flags an unknown kid so the caller can refetch the JWKS", async () => {
  await assert.rejects(verifyJwsRs256(sign(good, { alg: "RS256", kid: "k2" }), JWKS), (err) => err.unknownKid === true);
});

test("verifyJwsRs256 rejects malformed tokens", async () => {
  for (const token of [undefined, "", "a.b", "a.b.c.d", "..", "e30.e30.", "!!!.e30.sig", "W10.e30.sig"]) {
    await assert.rejects(verifyJwsRs256(token, JWKS));
  }
});

test("decodeJwt returns header, payload and signing input", () => {
  const token = sign(good);
  const decoded = decodeJwt(token);
  assert.equal(decoded.payload.iss, ISSUER);
  assert.equal(decoded.signingInput, token.split(".").slice(0, 2).join("."));
});

test("selectJwk matches kid exactly and filters out non-signing or non-RSA keys", () => {
  const rsa = { kty: "RSA", kid: "a", n: "x", e: "AQAB" };
  const enc = { kty: "RSA", kid: "b", use: "enc", n: "x", e: "AQAB" };
  const ec = { kty: "EC", kid: "c" };
  const ps = { kty: "RSA", kid: "d", alg: "PS256", n: "x", e: "AQAB" };
  const jwks = { keys: [rsa, enc, ec, ps] };
  assert.equal(selectJwk(jwks, { kid: "a" }), rsa);
  assert.equal(selectJwk(jwks, { kid: "b" }), null);
  assert.equal(selectJwk(jwks, { kid: "c" }), null);
  assert.equal(selectJwk(jwks, { kid: "d" }), null);
  assert.equal(selectJwk(jwks, {}), rsa);
  assert.equal(selectJwk({ keys: [rsa, { ...rsa, kid: "z" }] }, {}), null);
  assert.equal(selectJwk(null, { kid: "a" }), null);
  assert.equal(selectJwk({ keys: "nope" }, { kid: "a" }), null);
});

test("validateIdTokenClaims accepts good claims, an aud array with matching azp, and skew at the boundary", () => {
  validateIdTokenClaims(good, opts);
  validateIdTokenClaims({ ...good, aud: [CLIENT, "other"], azp: CLIENT }, opts);
  validateIdTokenClaims({ ...good, exp: NOW - SKEW }, opts);
  validateIdTokenClaims({ ...good, iat: NOW + SKEW }, opts);
});

test("validateIdTokenClaims rejects each broken rule", () => {
  const cases = [
    [{ iss: "https://evil.example/" }, /iss/],
    [{ iss: "https://api.anointedautomation.net" }, /iss/],
    [{ aud: "aa_someone_else" }, /aud/],
    [{ aud: [CLIENT, "other"] }, /azp/],
    [{ aud: [CLIENT, "other"], azp: "other" }, /azp/],
    [{ azp: "other" }, /azp/],
    [{ exp: undefined }, /exp is missing/],
    [{ exp: String(NOW + 300) }, /exp is missing/],
    [{ exp: NOW - SKEW - 1 }, /expired/],
    [{ iat: undefined }, /iat is missing/],
    [{ iat: NOW + SKEW + 1 }, /future/],
    [{ nonce: "n-2" }, /nonce/],
    [{ nonce: undefined }, /nonce/],
    [{ sub: "" }, /sub/],
    [{ sub: 42 }, /sub/],
  ];
  for (const [patch, pattern] of cases) {
    assert.throws(() => validateIdTokenClaims({ ...good, ...patch }, opts), pattern, JSON.stringify(patch));
  }
});

test("validateIdTokenClaims skips the nonce check only when nonce is null", () => {
  validateIdTokenClaims({ ...good, nonce: undefined }, { ...opts, nonce: null });
  assert.throws(() => validateIdTokenClaims({ ...good, nonce: undefined }, { ...opts, nonce: "" }), /nonce/);
});
