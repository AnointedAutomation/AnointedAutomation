// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

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
