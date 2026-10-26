// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Test-only helper: installs the dev dependencies (vue, @vue/server-renderer) when they are not there yet, so
// `npm test` works from a fresh clone and in the publish workflow. Not shipped in the package.

import { existsSync } from "node:fs";
import { spawnSync } from "node:child_process";
import { fileURLToPath } from "node:url";

const root = fileURLToPath(new URL("..", import.meta.url));

if (!existsSync(new URL("../node_modules/vue/package.json", import.meta.url))) {
  const npm = process.platform === "win32" ? "npm.cmd" : "npm";
  const result = spawnSync(npm, ["install", "--no-audit", "--no-fund"], { cwd: root, stdio: "inherit" });
  if (result.status !== 0) {
    throw new Error("npm install of the test dependencies failed");
  }
}
