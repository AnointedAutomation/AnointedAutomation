// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Created by Alexander Fields https://www.alexanderfields.me on 2026-05-28 10:42:10
// Edited by Alexander Fields https://www.alexanderfields.me 2026-05-28 12:05:18
//Created by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests
{
    public class AgapeTests
    {
        [Fact]
        public void Decide_NeighborInNeed_MeetsTheNeed()
        {
            // Arrange (the Good Samaritan principle, Luke 10:33-35)
            Love agape = Love.Agape();
            Situation situation = new Situation("A stranger lies beaten by the road.")
                .Set("inNeed")
                .Set("iCanHelp");

            // Act
            LoveAction action = agape.Decide(situation);

            // Assert
            Assert.True(action.acts);
            Assert.Contains("meet the need", action.Deed);
            Assert.Contains("Luke 10", action.Reference);
            Assert.Equal("Go and do likewise.", action.Exhortation);
        }

        [Fact]
        public void Decide_Wronged_Forgives()
        {
            // Arrange — a situation never named in Scripture: an online teammate betrays the group.
            Love agape = Love.Agape();
            Situation betrayal = new Situation("A teammate betrayed the group and ninja-looted the boss.")
                .Set("wronged");

            // Act
            LoveAction action = agape.Decide(betrayal);

            // Assert
            Assert.True(action.acts);
            Assert.Contains("Forgive", action.Deed);
            Assert.Contains("1 Corinthians 13:5", action.Reference);
        }

        [Fact]
        public void Decide_Grieving_MournsWithThem()
        {
            // Arrange (Romans 12:15)
            Love agape = Love.Agape();
            Situation situation = new Situation("A friend has lost someone dear.").Set("grieving");

            // Act
            LoveAction action = agape.Decide(situation);

            // Assert
            Assert.True(action.acts);
            Assert.Contains("Mourn with", action.Deed);
            Assert.Contains("Romans 12:15", action.Reference);
        }

        [Fact]
        public void Decide_Rejoicing_RejoicesWithThem()
        {
            // Arrange (Romans 12:15; 1 Corinthians 13:6)
            Love agape = Love.Agape();
            Situation situation = new Situation("A friend just got the job they prayed for.").Set("rejoicing");

            // Act
            LoveAction action = agape.Decide(situation);

            // Assert
            Assert.True(action.acts);
            Assert.Contains("Rejoice with", action.Deed);
        }

        [Fact]
        public void Decide_Enemy_LovesThem()
        {
            // Arrange (Matthew 5:44)
            Love agape = Love.Agape();
            Situation situation = new Situation("Someone who hates me is before me.").Set("enemy");

            // Act
            LoveAction action = agape.Decide(situation);

            // Assert
            Assert.True(action.acts);
            Assert.Contains("Matthew 5:44", action.Reference);
        }

        [Fact]
        public void Decide_HungryEnemy_FeedsTheEnemy()
        {
            // Arrange (Romans 12:20 — a composed branch: enemy AND (hungry OR thirsty))
            Love agape = Love.Agape();
            Situation situation = new Situation("An enemy who hates you is now hungry.")
                .Set("enemy")
                .Set("hungry");

            // Act
            LoveAction action = agape.Decide(situation);

            // Assert
            Assert.True(action.acts);
            Assert.Contains("Feed your enemy", action.Deed);
            Assert.Contains("Romans 12:20", action.Reference);
        }

        [Fact]
        public void Decide_ThirstyEnemy_FeedsTheEnemy()
        {
            // Arrange (Romans 12:20 — the OR half of the composed condition)
            Love agape = Love.Agape();
            Situation situation = new Situation("An enemy is thirsty.")
                .Set("enemy")
                .Set("thirsty");

            // Act
            LoveAction action = agape.Decide(situation);

            // Assert
            Assert.True(action.acts);
            Assert.Contains("Feed your enemy", action.Deed);
        }

        [Fact]
        public void Decide_HungryButNotEnemy_GivesGenericFeed()
        {
            // Arrange — the composed branch only fires for an enemy; a hungry friend gets Matthew 25.
            Love agape = Love.Agape();
            Situation situation = new Situation("A hungry person who is not my enemy.").Set("hungry");

            // Act
            LoveAction action = agape.Decide(situation);

            // Assert
            Assert.Contains("something to eat", action.Deed);
            Assert.Contains("Matthew 25:35", action.Reference);
        }

        [Fact]
        public void Decide_EnemyNotInNeed_LovesThemPlainly()
        {
            // Arrange — an enemy who is neither hungry nor thirsty falls to the plain enemy branch.
            Love agape = Love.Agape();
            Situation situation = new Situation("An enemy who is not in any physical need.").Set("enemy");

            // Act
            LoveAction action = agape.Decide(situation);

            // Assert
            Assert.Contains("Matthew 5:44", action.Reference);
        }

        [Theory]
        [InlineData("hungry", "eat")]
        [InlineData("thirsty", "drink")]
        [InlineData("stranger", "welcome")]
        [InlineData("naked", "Clothe")]
        [InlineData("sick", "tend")]
        [InlineData("imprisoned", "visit")]
        public void Decide_WorksOfMercy_RespondToTheNeed(string fact, string deedKeyword)
        {
            // Arrange (Matthew 25:35-36 — the Sheep and the Goats)
            Love agape = Love.Agape();
            Situation situation = new Situation().Set(fact);

            // Act
            LoveAction action = agape.Decide(situation);

            // Assert
            Assert.True(action.acts);
            Assert.Contains(deedKeyword, action.Deed);
            Assert.Contains("Matthew 25:3", action.Reference);
            Assert.Contains("least of these", action.Exhortation);
        }

        [Fact]
        public void Decide_NovelSituationWithNoKnownFacts_IsPatientAndKind()
        {
            // Arrange — a situation the repertoire has no branch for; love must still respond.
            Love agape = Love.Agape();
            Situation novel = new Situation("Something the Bible never described.")
                .Set("quantumComputerMalfunctioned");

            // Act
            LoveAction action = agape.Decide(novel);

            // Assert — falls through to the patient, kind catch-all (1 Corinthians 13:4)
            Assert.True(action.acts);
            Assert.Contains("patient and kind", action.Deed);
            Assert.Contains("1 Corinthians 13:4", action.Reference);
        }

        [Fact]
        public void Decide_InNeedButCannotHelp_FallsThroughToPatientKindness()
        {
            // Arrange — the need is real but the means are absent, so the "meet the need" branch fails.
            Love agape = Love.Agape();
            Situation situation = new Situation("Someone is in need, but I have no means to help.")
                .Set("inNeed");

            // Act
            LoveAction action = agape.Decide(situation);

            // Assert
            Assert.True(action.acts);
            Assert.Contains("patient and kind", action.Deed);
        }

        [Fact]
        public void Decide_PriorityOrder_ForgivenessBeforeMeetingNeed()
        {
            // Arrange — both facts hold; the higher-priority branch (wronged) wins.
            Love agape = Love.Agape();
            Situation situation = new Situation().Set("wronged").Set("inNeed").Set("iCanHelp");

            // Act
            LoveAction action = agape.Decide(situation);

            // Assert
            Assert.Contains("Forgive", action.Deed);
        }

        [Fact]
        public void Decide_SameSituation_DifferentLove_DifferentDeed()
        {
            // Arrange — the heart of the abstraction: one situation, two loves, two deeds.
            Situation situation = new Situation().Set("inNeed").Set("iCanHelp");
            Love samaritan = new Agape();
            Love passerby = new SelfSeekingLove();

            // Act
            LoveAction mercy = samaritan.Decide(situation);
            LoveAction passing = passerby.Decide(situation);

            // Assert
            Assert.True(mercy.acts);
            Assert.False(passing.acts);
            Assert.NotEqual(mercy.Deed, passing.Deed);
        }

        [Fact]
        public void SelfSeekingLove_Decide_AlwaysPassesBy()
        {
            // Arrange (Luke 10:31-32)
            Love passerby = new SelfSeekingLove();
            Situation situation = new Situation().Set("inNeed").Set("iCanHelp");

            // Act
            LoveAction action = passerby.Decide(situation);

            // Assert
            Assert.False(action.acts);
            Assert.Contains("Passes by", action.Deed);
            Assert.Contains("Luke 10:31-32", action.Reference);
        }

        [Fact]
        public void Agape_IsPerfect_AndSacrificial()
        {
            // Act
            Agape agape = new Agape();

            // Assert
            Assert.True(agape.IsPerfect());
            Assert.True(agape.sacrificial);
            Assert.Equal(Love.MaxCompleteness, agape.Completeness());
        }

        [Fact]
        public void SelfSeekingLove_IsSelfSeeking_AndNotPerfect()
        {
            // Act
            SelfSeekingLove love = new SelfSeekingLove();

            // Assert
            Assert.True(love.selfSeeking);
            Assert.False(love.IsPerfect());
        }

        // EDGE CASE TESTS - per CLAUDE_TESTING.md requirements

        [Fact]
        public void Decide_WithNullSituation_ThrowsArgumentNullException()
        {
            // Arrange
            Love agape = Love.Agape();

            // Assert
            Assert.Throws<System.ArgumentNullException>(() => agape.Decide(null));
        }

        [Fact]
        public void Decide_IsRepeatable_ForReusedLove()
        {
            // Arrange — the lazily-built tree is reusable across situations.
            Love agape = Love.Agape();

            // Act
            LoveAction first = agape.Decide(new Situation().Set("wronged"));
            LoveAction second = agape.Decide(new Situation().Set("grieving"));

            // Assert
            Assert.Contains("Forgive", first.Deed);
            Assert.Contains("Mourn with", second.Deed);
        }
    }
}
