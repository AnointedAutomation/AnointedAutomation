// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

import "server-only";

/**
 * @summary    Read a required environment variable or throw (no fallbacks).
 * @param {string} name
 * @returns {string}
 */
export function requireEnv(name) {
  const value = process.env[name];
  if (value === undefined || value.trim() === "") throw new Error(`Missing required environment variable ${name}`);
  return value;
}
