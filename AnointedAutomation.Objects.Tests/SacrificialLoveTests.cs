// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Created by Alexander Fields https://www.alexanderfields.me on 2026-05-28 11:08:54
// Edited by Alexander Fields https://www.alexanderfields.me 2026-05-28 12:05:18
//Created by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests
{
    public class SacrificialLoveTests
    {
        [Fact]
        public void Decide_FriendInMortalDanger_LaysDownLife()
        {
            // Arrange (John 15:13)
            Love love = new SacrificialLove();
            Situation situation = new Situation("A friend's life is at stake; saving them costs my own.")
                .Set("friendsLifeAtStake");

            // Act
            LoveAction action = love.Decide(situation);

            // Assert
            Assert.True(action.acts);
            Assert.Contains("Lay down your life", action.Deed);
            Assert.Equal("John 15:13", action.Reference);
            Assert.Equal("Love each other as I have loved you.", action.Exhortation);
        }

        [Fact]
        public void Decide_WhenNotCalledToSacrifice_FallsThroughToAgape()
        {
            // Arrange — no ultimate sacrifice is needed, but a neighbor is in need.
            Love love = new SacrificialLove();
            Situation situation = new Situation("A stranger in need by the road.")
                .Set("inNeed")
                .Set("iCanHelp");

            // Act
            LoveAction action = love.Decide(situation);

            // Assert — inherits agape's repertoire
            Assert.True(action.acts);
            Assert.Contains("meet the need", action.Deed);
        }

        [Fact]
        public void Decide_GreaterDeedThanAgape_ForSameSituation()
        {
            // Arrange — "greater love" (John 15:13): same situation, a greater deed.
            Situation situation = new Situation("A friend's life is at stake.").Set("friendsLifeAtStake");
            Love agape = new Agape();
            Love sacrificial = new SacrificialLove();

            // Act
            LoveAction agapeDeed = agape.Decide(situation);          // no branch for this -> patient & kind
            LoveAction laysDownLife = sacrificial.Decide(situation); // adds the sacrifice branch

            // Assert
            Assert.NotEqual(agapeDeed.Deed, laysDownLife.Deed);
            Assert.Contains("Lay down your life", laysDownLife.Deed);
        }

        [Fact]
        public void SacrificialLove_IsPerfect_AndSacrificial()
        {
            // Act
            SacrificialLove love = new SacrificialLove();

            // Assert (inherits the full agape character)
            Assert.True(love.IsPerfect());
            Assert.True(love.sacrificial);
            Assert.Equal(Love.MaxCompleteness, love.Completeness());
        }

        [Fact]
        public void SacrificialLove_GreaterLove_ReturnsTrue()
        {
            // Act (John 15:13)
            SacrificialLove love = new SacrificialLove();

            // Assert
            Assert.True(love.GreaterLove());
        }

        [Fact]
        public void SacrificialLove_IsBothAgapeAndLove()
        {
            // Act
            SacrificialLove love = new SacrificialLove();

            // Assert — sacrificial love is agape brought to its fullness
            Assert.IsAssignableFrom<Agape>(love);
            Assert.IsAssignableFrom<Love>(love);
        }

        [Fact]
        public void SacrificialLove_WithParties_SetsLoverAndBeloved()
        {
            // Arrange (Christ laying down His life for His friends)
            string lover = "Christ";
            string beloved = "His friends";

            // Act
            SacrificialLove love = new SacrificialLove(lover, beloved);

            // Assert
            Assert.Equal(lover, love.Lover);
            Assert.Equal(beloved, love.Beloved);
        }

        // EDGE CASE TESTS - per CLAUDE_TESTING.md requirements

        [Fact]
        public void Decide_WithNullSituation_ThrowsArgumentNullException()
        {
            // Arrange
            Love love = new SacrificialLove();

            // Assert
            Assert.Throws<System.ArgumentNullException>(() => love.Decide(null));
        }

        [Fact]
        public void SacrificialLove_WithNullParties_CreatesInstance()
        {
            // Act
            SacrificialLove love = new SacrificialLove(null, null);

            // Assert
            Assert.NotNull(love);
            Assert.Null(love.Lover);
            Assert.Null(love.Beloved);
            Assert.True(love.IsPerfect());
        }
    }
}
