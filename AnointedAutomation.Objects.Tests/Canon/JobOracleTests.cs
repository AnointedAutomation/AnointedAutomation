// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests.Canon
{
    // Oracle 22: Job.
    public class JobOracleTests
    {
        [Fact]
        public void JobsIntegrityUnderSuffering_IsRighteous()
        {
            // "Though he slay me, yet will I trust in him." (Job 13:15; 1:21-22)
            OracleHarness.Righteous(OracleHarness.Witness("Job holds his integrity", new Trust(), new ObedienceToGod()));
        }

        [Fact]
        public void GodsSovereigntyOverCreationsOrder_IsADivineAct()
        {
            // "Where were you when I laid the earth's foundation?" (Job 38)
            OracleHarness.DivineAct(OracleHarness.Witness("God answers from the whirlwind", new CreationOrder()));
        }

        [Fact]
        public void TheComfortersFalseAccusation_IsSin()
        {
            // They condemn the innocent sufferer with false counsel (Job 42:7).
            OracleHarness.Sin(OracleHarness.Witness("the comforters accuse Job falsely", new Condemnation(), new Deceit()));
        }

        [Fact]
        public void GodVindicatesAndRestoresJob_IsADivineAct()
        {
            // God vindicates Job and restores him (Job 42).
            OracleHarness.DivineAct(OracleHarness.Witness("God vindicates Job", new Righteousness(), new Compassion()));
        }
    }
}
