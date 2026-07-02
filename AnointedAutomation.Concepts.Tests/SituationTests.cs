// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Concepts;

namespace AnointedAutomation.Concepts.Tests
{
    public class SituationTests
    {
        [Fact]
        public void With_ThenHas_ReturnsTrue()
        {
            Situation situation = new Situation();

            situation.With(new Need());

            Assert.True(situation.Has(new Need()));
        }

        [Fact]
        public void Has_UnknownCircumstance_ReturnsFalse()
        {
            // A circumstance never added simply does not hold.
            Situation situation = new Situation();

            Assert.False(situation.Has(new Circumstance("neverHeardOfIt")));
        }

        [Fact]
        public void With_ReturnsSameInstance_ForChaining()
        {
            Situation situation = new Situation();

            Situation chained = situation.With(new Circumstance("a")).With(new Circumstance("b"));

            Assert.Same(situation, chained);
        }

        [Fact]
        public void Constructor_WithDescription_SetsDescription()
        {
            Situation situation = new Situation("A stranger lies beaten by the road.");

            Assert.Equal("A stranger lies beaten by the road.", situation.Description);
        }

        [Fact]
        public void Circumstances_ArePopulated_AfterWith()
        {
            Situation situation = new Situation().With(new Circumstance("a")).With(new Circumstance("b"));

            Assert.Equal(2, situation.Circumstances.Count);
        }

        [Fact]
        public void ANamedCircumstance_MatchesANovelOneOfTheSameName()
        {
            // A typed circumstance and a directly-named one are the same state when they share a name.
            Situation situation = new Situation().With(new Hunger());

            Assert.True(situation.Has(new Circumstance("hungry")));
        }

        [Fact]
        public void ANovelCircumstance_NeverNamedInScripture_Works()
        {
            // The world is open-ended; any circumstance can be expressed.
            Situation situation = new Situation().With(new Circumstance("quantumComputerMalfunctioned"));

            Assert.True(situation.Has(new Circumstance("quantumComputerMalfunctioned")));
        }

        // EDGE CASE TESTS - per CLAUDE_TESTING.md requirements

        [Fact]
        public void With_WithNullCircumstance_ThrowsArgumentNullException()
        {
            Situation situation = new Situation();

            Assert.Throws<System.ArgumentNullException>(() => situation.With(null));
        }

        [Fact]
        public void Has_WithNullCircumstance_ThrowsArgumentNullException()
        {
            Situation situation = new Situation();

            Assert.Throws<System.ArgumentNullException>(() => situation.Has(null));
        }

        [Fact]
        public void Circumstance_WithNullName_ThrowsArgumentNullException()
        {
            Assert.Throws<System.ArgumentNullException>(() => new Circumstance(null));
        }

        [Fact]
        public void With_UnicodeCircumstanceName_Works()
        {
            Situation situation = new Situation().With(new Circumstance("σε ανάγκη 需要"));

            Assert.True(situation.Has(new Circumstance("σε ανάγκη 需要")));
        }
    }
}
