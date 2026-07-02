// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Concepts;

namespace AnointedAutomation.Concepts.Tests
{
    public class DivineCharacterTests
    {
        [Fact]
        public void TheCross_SatisfiesJusticeAndMercyTogether_FullyCoherent_NoDisorder()
        {
            // The keystone. The cross is full Justice (the debt is paid, Romans 3:25) and full Mercy
            // (the guilty go free, Romans 5:8) in one act. A character that SELECTED one attribute
            // could never express this. Because every facet is always live and they harmonize, both
            // read fully high and reality is not destabilized.
            DivineCharacter character = new DivineCharacter(new Justice(), new Mercy());
            Act theCross = new Act("the cross", new Atonement(), new Pardon());

            Resolution resolution = character.Harmonize(theCross);

            Assert.Equal(1.0, resolution.Coherence);
            Assert.Equal(0.0, resolution.Disorder);
            Assert.Equal(1.0, resolution.Reading("Justice"));
            Assert.Equal(1.0, resolution.Reading("Mercy"));
        }

        [Fact]
        public void Injustice_ReadsLowAndIntroducesDisorder()
        {
            // An act that wrongs a neighbour offends Justice gravely; Mercy is indifferent to it.
            // Theft is serious but not capital, so it injects serious, not total, disorder.
            DivineCharacter character = new DivineCharacter(new Justice(), new Mercy());
            Act theft = new Act("a theft", new Theft());

            Resolution resolution = character.Harmonize(theft);

            Assert.Equal(0.25, resolution.Coherence);
            Assert.Equal(0.5, resolution.Disorder);
        }

        [Fact]
        public void Murder_UnleashesMoreDisorderThanTheft()
        {
            // Scripture grades sin: murder is capital (Genesis 9:6), theft is answered by restitution
            // (Exodus 22:1). The gravest wrong must destabilize reality more, not the same.
            DivineCharacter character = new DivineCharacter(new Justice(), new LoveFacet());

            Resolution murder = character.Harmonize(new Act("Cain kills Abel", new Murder()));
            Resolution theft = character.Harmonize(new Act("a theft", new Theft()));

            Assert.Equal(1.0, murder.Disorder);
            Assert.True(murder.Disorder > theft.Disorder);
        }

        [Fact]
        public void Rudeness_BarelyDisordersReality()
        {
            // A rude word is a real wrong, but minor: it troubles reality only slightly, nothing like
            // the destruction of a capital crime.
            DivineCharacter character = new DivineCharacter(new LoveFacet());

            Resolution resolution = character.Harmonize(new Act("a harsh word", new Rudeness()));

            Assert.Equal(0.25, resolution.Disorder);
        }

        [Fact]
        public void IndifferentAct_IsNeitherCoherentNorDisordering()
        {
            // An act that touches no facet sits neutral and does not destabilize reality.
            DivineCharacter character = new DivineCharacter(new Justice(), new Mercy());
            Act sunset = new Act("watching a sunset");

            Resolution resolution = character.Harmonize(sunset);

            Assert.Equal(0.5, resolution.Coherence);
            Assert.Equal(0.0, resolution.Disorder);
        }
    }
}
