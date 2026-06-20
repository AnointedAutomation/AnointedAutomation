// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests.Canon
{
    // Oracle 07: Judges.
    public class JudgesOracleTests
    {
        [Fact]
        public void IsraelDoesEvilAndServesIdols_IsSin()
        {
            // "The Israelites did evil ... and served the Baals." (Judges 2:11) Rebellion brings disorder.
            OracleHarness.Sin(OracleHarness.Witness("Israel serves the Baals", new Idolatry(), new Rebellion()));
        }

        [Fact]
        public void GodRaisesADelivererInMercy_IsADivineAct()
        {
            // When they cried out, the LORD raised up deliverers (Judges 2:16, 18).
            OracleHarness.DivineAct(OracleHarness.Witness("God raises a deliverer", new Deliverance(), new Compassion()));
        }
    }
}
