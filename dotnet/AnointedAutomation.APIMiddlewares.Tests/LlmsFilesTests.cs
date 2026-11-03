// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-03 00:00:00 Jesus is King ✝️
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using Xunit;

namespace AnointedAutomation.APIMiddlewares.Tests
{
    /// <summary>
    /// Guards the repository's machine-readable package index: llms.json must parse, and every package it lists
    /// must appear with the same version in llms.md and llms.txt so the three files never drift apart.
    /// </summary>
    public class LlmsFilesTests
    {
        private static string RepoRoot()
        {
            DirectoryInfo dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (dir != null && !File.Exists(Path.Combine(dir.FullName, "llms.json")))
            {
                dir = dir.Parent;
            }
            if (dir == null)
            {
                throw new InvalidOperationException("llms.json not found above " + AppContext.BaseDirectory + ".");
            }
            return dir.FullName;
        }

        [Fact]
        public void LlmsJson_Parses()
        {
            using (JsonDocument doc = JsonDocument.Parse(File.ReadAllText(Path.Combine(RepoRoot(), "llms.json"))))
            {
                Assert.True(doc.RootElement.GetProperty("packages").GetArrayLength() > 0);
                Assert.False(string.IsNullOrWhiteSpace(doc.RootElement.GetProperty("fileHeader").GetString()));
            }
        }

        [Fact]
        public void LlmsMarkdownAndText_ListEveryJsonPackageAtTheSameVersion()
        {
            string root = RepoRoot();
            string[] md = File.ReadAllLines(Path.Combine(root, "llms.md"));
            string[] txt = File.ReadAllLines(Path.Combine(root, "llms.txt"));
            using (JsonDocument doc = JsonDocument.Parse(File.ReadAllText(Path.Combine(root, "llms.json"))))
            {
                foreach (JsonElement package in doc.RootElement.GetProperty("packages").EnumerateArray())
                {
                    string id = package.GetProperty("id").GetString();
                    string version = package.GetProperty("version").GetString();
                    string heading = "### " + id + " " + version;
                    Assert.True(md.Any(line => string.Equals(line.Trim(), heading, StringComparison.Ordinal)), "llms.md has no \"" + heading + "\" heading.");
                    string link = "- [" + id + "](";
                    Assert.True(
                        txt.Any(line => line.StartsWith(link, StringComparison.Ordinal) && line.Contains("Version " + version + ",", StringComparison.Ordinal)),
                        "llms.txt has no entry for " + id + " at version " + version + ".");
                }
            }
        }
    }
}
