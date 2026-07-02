// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Concepts;

namespace AnointedAutomation.Concepts.Tests.Canon
{
    // Oracle 02: Exodus.
    public class ExodusOracleTests
    {
        [Fact]
        public void GodDeliversIsrael_IsFaithfulDeliverance()
        {
            // "I have come down to rescue them." (Exodus 3:8)
            OracleHarness.DivineAct(OracleHarness.Witness("the exodus", new Deliverance(), new CovenantFaithfulness()));
        }

        [Fact]
        public void PharaohOppressesAndKillsTheSons_IsGraveSin()
        {
            // Enslaving the Hebrews and ordering their sons drowned (Exodus 1).
            OracleHarness.Sin(OracleHarness.Witness("Pharaoh's oppression", new Oppression(), new Murder()));
        }

        [Fact]
        public void GodGivesTheRighteousLaw_IsADivineAct()
        {
            // The Law is God's righteous order for His people (Exodus 20).
            OracleHarness.DivineAct(OracleHarness.Witness("the Law given at Sinai", new Righteousness(), new CovenantFaithfulness()));
        }

        [Fact]
        public void TheGoldenCalf_IsSin()
        {
            // "These are your gods, Israel." (Exodus 32:4) Idolatry while the covenant is still fresh.
            OracleHarness.Sin(OracleHarness.Witness("the golden calf", new Idolatry()));
        }

        [Fact]
        public void WorshipGroundedInTheIdol_DriftsTowardNonBeing()
        {
            // The same act of worship cannot hold when offered to a dead idol (1 Meqabyan 1).
            Resolution onGod = OracleHarness.Grounded("worship", Grounding.InGod(), new Worship());
            Resolution onIdol = OracleHarness.Grounded("worship", Grounding.InIdol("the golden calf"), new Worship());

            Assert.Equal(0.0, onGod.Disorder);
            Assert.True(onIdol.Disorder > 0.0);
            Assert.True(onIdol.Coherence < onGod.Coherence);
        }
    }
}
