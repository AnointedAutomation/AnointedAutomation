// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests
{
    public class MoralConceptTests
    {
        [Fact]
        public void MoralConcept_IsAbstract()
        {
            // A moral concept is abstract code, not a magic string; each concrete concept supplies
            // its own name, Scripture, and stance toward God's character.
            Assert.True(typeof(MoralConcept).IsAbstract);
        }

        [Fact]
        public void Compassion_UpholdsLoveAndMercy_AndCarriesScripture()
        {
            // "Be merciful, even as your Father is merciful." (Luke 6:36)
            Compassion compassion = new Compassion();

            Assert.True(compassion.Upholds(new LoveFacet()));
            Assert.True(compassion.Upholds(new Mercy()));
            Assert.False(compassion.Violates(new Justice()));
            Assert.False(string.IsNullOrEmpty(compassion.Scripture));
        }

        [Fact]
        public void Murder_ViolatesJusticeAndLove()
        {
            // "You shall not murder." (Exodus 20:13)
            Murder murder = new Murder();

            Assert.True(murder.Violates(new Justice()));
            Assert.True(murder.Violates(new LoveFacet()));
            Assert.False(murder.Upholds(new Mercy()));
        }

        [Fact]
        public void Act_CarriesItsConcepts_AndADescription()
        {
            MoralConcept compassion = new Compassion();
            Act act = new Act("the Samaritan binds the wounds", compassion);

            Assert.Equal("the Samaritan binds the wounds", act.Description);
            Assert.Single(act.Concepts);
            Assert.Same(compassion, act.Concepts[0]);
        }
    }
}
