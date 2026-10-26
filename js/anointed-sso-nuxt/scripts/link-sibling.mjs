// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Test-only helper (not shipped): in this monorepo the dependency @anointedautomation/sso lives next door in
// ../anointed-sso. When it is not installed from the registry yet, link the sibling into node_modules so
// `npm test` runs against the code in this repository. Does nothing when the dependency already resolves.

import { existsSync, mkdirSync, symlinkSync } from "node:fs";
import { fileURLToPath } from "node:url";

const target = new URL("../node_modules/@anointedautomation/sso", import.meta.url);
const sibling = new URL("../../anointed-sso", import.meta.url);

if (!existsSync(new URL("package.json", target.href + "/"))) {
  if (!existsSync(new URL("package.json", sibling.href + "/"))) {
    throw new Error("@anointedautomation/sso is not installed and ../anointed-sso was not found");
  }
  mkdirSync(fileURLToPath(new URL("../node_modules/@anointedautomation/", import.meta.url)), { recursive: true });
  symlinkSync(fileURLToPath(sibling), fileURLToPath(target), "dir");
}
