# Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
"""Run: python3 -m unittest discover -s .github/workflows -p 'test_sync_llms_versions.py'"""
import importlib.util
import unittest
from pathlib import Path

_spec = importlib.util.spec_from_file_location("sync_llms_versions", Path(__file__).with_name("sync-llms-versions.py"))
sync = importlib.util.module_from_spec(_spec)
_spec.loader.exec_module(sync)


class RewriteMdSectionTests(unittest.TestCase):
    def test_release_notes_and_longer_numbers_are_kept(self):
        section = "### Pkg 1.0.1\n- 1.0.1 adds the thing. Needs Other 11.0.1.\n- Install: (git tag 1.0.1).\n"
        result = sync.rewrite_md_section(section, "Pkg", "1.0.1", "1.0.2")
        self.assertEqual("### Pkg 1.0.2\n- 1.0.1 adds the thing. Needs Other 11.0.1.\n- Install: (git tag 1.0.2).\n", result)

    def test_install_forms_are_bounded(self):
        text, count = sync.replace_version_forms(
            ':anointedsso:0.1.1") git tag 0.1.10 Packagist, 0.1.1;', "0.1.1", "0.1.3")
        self.assertEqual(':anointedsso:0.1.3") git tag 0.1.10 Packagist, 0.1.3;', text)
        self.assertEqual(2, count)


if __name__ == "__main__":
    unittest.main()
