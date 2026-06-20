// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests.Canon
{
    // Oracle 15 & 16: Ezra & Nehemiah, and 2 Ezra / Ezra Sutuel.
    public class EzraNehemiahOracleTests
    {
        [Fact]
        public void GodBringsBackTheExiles_IsFaithfulDeliverance()
        {
            // The LORD moves Cyrus and restores His people to rebuild (Ezra 1).
            OracleHarness.DivineAct(OracleHarness.Witness("the return from exile",
                new CovenantFaithfulness(), new Deliverance()));
        }

        [Fact]
        public void TheCovenantRenewalAndConfession_IsRighteous()
        {
            // The people confess and rededicate themselves to the LORD (Nehemiah 9).
            OracleHarness.Righteous(OracleHarness.Witness("the covenant renewed", new Repentance(), new Worship()));
        }

        [Fact]
        public void NehemiahEndsTheOppressionOfThePoor_IsRighteous()
        {
            // He rebukes the nobles for exacting usury and restores what was taken (Nehemiah 5).
            OracleHarness.Righteous(OracleHarness.Witness("Nehemiah relieves the poor",
                new JustRecompense(), new Generosity()));
        }

        [Fact]
        public void GodsJudgmentIsBothJustAndMerciful_IsADivineAct()
        {
            // The apocalyptic Ezra wrestles with, and rests in, God's justice joined to His mercy
            // (2 Ezra / 4 Ezra).
            OracleHarness.DivineAct(OracleHarness.Witness("God's righteous, merciful judgment",
                new Righteousness(), new Pardon()));
        }
    }
}
