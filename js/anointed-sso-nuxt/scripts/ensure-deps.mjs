// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Test-only helper: installs the dev dependencies (h3) when they are not there yet, so
// `npm test` works from a fresh clone and in the publish workflow. Not shipped in the package.

import { existsSync } from "node:fs";
import { spawnSync } from "node:child_process";
import { fileURLToPath } from "node:url";

const root = fileURLToPath(new URL("..", import.meta.url));

if (!existsSync(new URL("../node_modules/h3/package.json", import.meta.url))) {
  const npm = process.platform === "win32" ? "npm.cmd" : "npm";
  const result = spawnSync(npm, ["install", "--no-audit", "--no-fund"], { cwd: root, stdio: "inherit" });
  if (result.status !== 0) {
    throw new Error("npm install of the test dependencies failed");
  }
}
