# Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
"""Sync llms.json, llms.md and llms.txt to the versions in the package manifests.

Run from the repo root (the version-increment workflow runs it after its bumps). Each package's real version is read
from its manifest (csproj <Version>, package.json, pyproject.toml, or a VERSION file), and every llms.* entry that
names an older version is rewritten. Fails loudly when a manifest or an llms.* entry cannot be found.
"""
import json
import re
import sys
import tomllib
from pathlib import Path

ROOT = Path.cwd()
TREE = "https://github.com/AnointedAutomation/AnointedAutomation/tree/master/"


def fail(message):
    print("::error::" + message)
    sys.exit(1)


def manifest_version(rel):
    """rel is the package path under the repo, as in the llms.json source URL."""
    path = ROOT / rel
    if rel.startswith("dotnet/"):
        projects = list(path.glob("*.csproj"))
        if len(projects) != 1:
            fail(f"expected one csproj in {rel}, found {len(projects)}")
        match = re.search(r"<Version>([^<]+)</Version>", projects[0].read_text(encoding="utf-8"))
        if match is None:
            fail(f"no <Version> in {projects[0]}")
        return match.group(1).strip()
    if rel.startswith("js/"):
        return json.loads((path / "package.json").read_text(encoding="utf-8"))["version"]
    if rel.startswith("python/"):
        return tomllib.loads((path / "pyproject.toml").read_text(encoding="utf-8"))["project"]["version"]
    if rel.startswith("php/"):
        # PHP shares the repo's git tags with the Swift package
        return (ROOT / "swift/AnointedSSO/VERSION").read_text(encoding="utf-8").strip()
    if rel.startswith("swift/") or rel.startswith("android/"):
        return (path / "VERSION").read_text(encoding="utf-8").strip()
    fail(f"no version rule for {rel}")


def main():
    json_path = ROOT / "llms.json"
    json_text = json_path.read_text(encoding="utf-8")
    data = json.loads(json_text)
    md = (ROOT / "llms.md").read_text(encoding="utf-8")
    txt_lines = (ROOT / "llms.txt").read_text(encoding="utf-8").split("\n")

    # (md heading name, source path, old version, new version)
    changes = []
    for key in ("packages", "jsPackages", "swiftPackages", "pythonPackages", "phpPackages"):
        for package in data[key]:
            rel = package["source"].removeprefix(TREE)
            new = manifest_version(rel)
            old = package["version"]
            if old != new:
                # Edit the text in place so the file keeps its hand formatting
                marker = json_text.find('"id": ' + json.dumps(package["id"], ensure_ascii=False) + ",")
                start = json_text.rfind("\n    {", 0, marker)
                end = json_text.find("\n    }", marker)
                if marker < 0 or start < 0 or end < 0:
                    fail(f"llms.json entry for {package['id']} not found")
                entry = json_text[start:end]
                updated = entry.replace('"version": "' + old + '"', '"version": "' + new + '"').replace("git tag " + old, "git tag " + new)
                if updated == entry:
                    fail(f"llms.json entry for {package['id']} has no version {old}")
                json_text = json_text[:start] + updated + json_text[end:]
                changes.append((package["id"], rel, old, new))

    # Android is listed only in llms.md and llms.txt
    android = re.search(r"^### AnointedSSO \(Android\) (\S+)$", md, re.MULTILINE)
    if android is None:
        fail("llms.md has no '### AnointedSSO (Android) <version>' heading")
    android_new = manifest_version("android/AnointedSSO")
    if android.group(1) != android_new:
        changes.append(("AnointedSSO (Android)", "android/AnointedSSO", android.group(1), android_new))

    for heading, rel, old, new in changes:
        # llms.md: the heading and every mention of the old version inside that package's section
        pattern = re.compile(r"^### " + re.escape(heading) + " " + re.escape(old) + r"$.*?(?=^### |\Z)", re.MULTILINE | re.DOTALL)
        section = pattern.search(md)
        if section is None:
            fail(f"llms.md has no '### {heading} {old}' section")
        md = md[: section.start()] + section.group(0).replace(old, new) + md[section.end():]

        # llms.txt: the line that links this package's source
        link = re.compile(re.escape(TREE + rel) + r"(?![\w./-])")
        rows = [i for i, line in enumerate(txt_lines) if link.search(line)]
        if len(rows) != 1:
            fail(f"llms.txt has {len(rows)} lines linking {rel}, expected 1")
        line = txt_lines[rows[0]]
        if rel.startswith("dotnet/"):
            if "Version " + old + "," not in line:
                fail(f"llms.txt line for {rel} has no 'Version {old},'")
            line = line.replace("Version " + old + ",", "Version " + new + ",")
        else:
            line = line.replace(old, new)
        txt_lines[rows[0]] = line
        print(f"{heading}: {old} -> {new}")

    json.loads(json_text)
    json_path.write_text(json_text, encoding="utf-8")
    (ROOT / "llms.md").write_text(md, encoding="utf-8")
    (ROOT / "llms.txt").write_text("\n".join(txt_lines), encoding="utf-8")
    print(f"{len(changes)} llms.* version(s) synced")


main()
