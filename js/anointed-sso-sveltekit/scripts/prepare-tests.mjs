// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Test-only helper (not shipped): installs the dev dependency (svelte) when it is missing, then links the sibling
// ../anointed-sso when @anointedautomation/sso is not installed from the registry yet.

import { existsSync } from "node:fs";
import { spawnSync } from "node:child_process";
import { fileURLToPath } from "node:url";

const root = fileURLToPath(new URL("..", import.meta.url));

if (!existsSync(new URL("../node_modules/svelte/package.json", import.meta.url))) {
  const npm = process.platform === "win32" ? "npm.cmd" : "npm";
  const result = spawnSync(npm, ["install", "--no-audit", "--no-fund", "--no-save"], { cwd: root, stdio: "inherit" });
  if (result.status !== 0) {
    throw new Error("npm install of the test dependencies failed");
  }
}
await import("./link-sibling.mjs");
