// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-05-28 12:05:18
// Edited by Alexander Fields https://www.alexanderfields.me 2026-05-28 12:05:18
//Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests
{
    public class SituationTests
    {
        [Fact]
        public void Set_ThenIs_ReturnsTrue()
        {
            // Arrange
            Situation situation = new Situation();

            // Act
            situation.Set("inNeed");

            // Assert
            Assert.True(situation.Is("inNeed"));
        }

        [Fact]
        public void Set_FalseValue_IsReturnsFalse()
        {
            // Arrange
            Situation situation = new Situation();

            // Act
            situation.Set("inNeed", false);

            // Assert
            Assert.False(situation.Is("inNeed"));
        }

        [Fact]
        public void Is_UnknownFact_ReturnsFalse()
        {
            // Arrange
            Situation situation = new Situation();

            // Assert — a fact never recorded simply does not hold
            Assert.False(situation.Is("neverHeardOfIt"));
        }

        [Fact]
        public void Has_RecordedFact_ReturnsTrue_EvenWhenFalse()
        {
            // Arrange
            Situation situation = new Situation().Set("inNeed", false);

            // Assert
            Assert.True(situation.Has("inNeed"));
            Assert.False(situation.Is("inNeed"));
        }

        [Fact]
        public void Has_UnknownFact_ReturnsFalse()
        {
            // Arrange
            Situation situation = new Situation();

            // Assert
            Assert.False(situation.Has("nope"));
        }

        [Fact]
        public void Set_ReturnsSameInstance_ForChaining()
        {
            // Arrange
            Situation situation = new Situation();

            // Act
            Situation chained = situation.Set("a").Set("b");

            // Assert
            Assert.Same(situation, chained);
        }

        [Fact]
        public void Constructor_WithDescription_SetsDescription()
        {
            // Act
            Situation situation = new Situation("A stranger lies beaten by the road.");

            // Assert
            Assert.Equal("A stranger lies beaten by the road.", situation.Description);
        }

        [Fact]
        public void Facts_ArePopulated_AfterSet()
        {
            // Arrange
            Situation situation = new Situation().Set("a").Set("b", false);

            // Assert
            Assert.Equal(2, situation.Facts.Count);
        }

        // EDGE CASE TESTS - per CLAUDE_TESTING.md requirements

        [Fact]
        public void Set_WithNullFact_ThrowsArgumentNullException()
        {
            // Arrange
            Situation situation = new Situation();

            // Assert
            Assert.Throws<System.ArgumentNullException>(() => situation.Set(null));
        }

        [Fact]
        public void Is_WithNullFact_ThrowsArgumentNullException()
        {
            // Arrange
            Situation situation = new Situation();

            // Assert
            Assert.Throws<System.ArgumentNullException>(() => situation.Is(null));
        }

        [Fact]
        public void Has_WithNullFact_ThrowsArgumentNullException()
        {
            // Arrange
            Situation situation = new Situation();

            // Assert
            Assert.Throws<System.ArgumentNullException>(() => situation.Has(null));
        }

        [Fact]
        public void Set_SameFactTwice_TakesLatestValue()
        {
            // Arrange
            Situation situation = new Situation().Set("inNeed", true).Set("inNeed", false);

            // Assert
            Assert.False(situation.Is("inNeed"));
            Assert.Single(situation.Facts);
        }

        [Fact]
        public void Set_WithUnicodeFactName_Works()
        {
            // Arrange
            Situation situation = new Situation().Set("σε ανάγκη 需要");

            // Assert
            Assert.True(situation.Is("σε ανάγκη 需要"));
        }
    }
}
