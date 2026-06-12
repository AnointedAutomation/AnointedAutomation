// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests.Canon
{
    // Oracle 20 & 21: 1 Maccabees, and the Ethiopic Meqabyan (2 & 3 Maccabees of the Ethiopian canon).
    public class MaccabeesMeqabyanOracleTests
    {
        [Fact]
        public void RefusingForcedIdolatry_IsRighteous()
        {
            // The faithful will not abandon the covenant under the tyrant's decree (1 Maccabees 1-2).
            OracleHarness.Righteous(OracleHarness.Witness("refusing the idol under persecution",
                new CourageousFaith(), new ObedienceToGod()));
        }

        [Fact]
        public void TheTyrantsForcedIdolatryAndSlaughter_IsGraveSin()
        {
            // Forcing idolatry and killing those who refuse (1 Maccabees 1).
            Resolution r = OracleHarness.Witness("the tyrant forces idolatry", new Murder(), new Oppression());

            OracleHarness.Sin(r);
            Assert.Equal(1.0, r.Disorder);
        }

        [Fact]
        public void MartyrdomRatherThanIdolatry_IsRighteous()
        {
            // The Meqabyan martyrs choose death over the dead idols, trusting the God who raises
            // (resurrection hope).
            OracleHarness.Righteous(OracleHarness.Witness("martyrdom rather than idolatry",
                new CourageousFaith(), new Trust()));
        }

        [Fact]
        public void TheIdolsCannotGiveLife_WorshipOfThemDriftsToNonBeing()
        {
            // "They do not truly live, they cannot give life." (1 Meqabyan 1)
            Resolution onGod = OracleHarness.Grounded("worship", Grounding.InGod(), new Worship());
            Resolution onIdol = OracleHarness.Grounded("worship", Grounding.InIdol("the idol"), new Worship());

            Assert.True(onIdol.Coherence < onGod.Coherence);
            Assert.True(onIdol.Disorder > 0.0);
        }
    }
}
