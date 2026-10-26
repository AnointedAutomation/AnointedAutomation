// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11 Jesus is King ✝️
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Concepts;

namespace AnointedAutomation.Concepts.Tests.Canon
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
