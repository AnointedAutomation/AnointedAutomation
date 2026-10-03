// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-10-03 Jesus is King ✝️
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Concepts;

namespace AnointedAutomation.Concepts.Tests
{
    public class LoveTriadTests
    {
        private static Situation Hungry()
        {
            return new Situation().With(new Hunger());
        }

        [Fact]
        public void ConstructedLove_IsCommitted_BackwardCompatible()
        {
            Love love = Love.Agape();

            Assert.True(love.Bond.IsCommitted);
            Assert.Single(love.Bond.History);
            Assert.True(love.Decide(Hungry()).acts);
        }

        [Fact]
        public void WithdrawnLove_CannotAct_EvenWhenItFeelsKind()
        {
            Love love = Love.Agape("Samaritan", "traveler").Withdraw("passed by");

            LoveAction action = love.Decide(Hungry());

            Assert.True(love.IsPerfect());
            Assert.False(action.acts);
            Assert.Equal("Deuteronomy 30:19; Joshua 24:15", action.Reference);
        }

        [Fact]
        public void RecommittedLove_ActsAgain()
        {
            Love love = Love.Agape("Samaritan", "traveler").Withdraw("passed by").Commit("turned back");

            Assert.True(love.Decide(Hungry()).acts);
        }

        [Fact]
        public void Exists_RequiresLoverBelovedAndCommittedBond()
        {
            Assert.True(Love.Agape("Samaritan", "traveler").Exists());
            Assert.False(Love.Agape(null, "traveler").Exists());
            Assert.False(Love.Agape("Samaritan", null).Exists());
            Assert.False(Love.Agape("Samaritan", "").Exists());
            Assert.False(Love.Agape("Samaritan", "traveler").Withdraw("no").Exists());
        }

        [Fact]
        public void Complete_IsPerfectCommittedAndShared()
        {
            Love love = Love.Agape("husband", "wife");
            Assert.False(love.IsComplete());

            love.Bond.ShareWith("child");
            Assert.True(love.IsComplete());

            love.Withdraw("no");
            Assert.False(love.IsComplete());
        }

        [Fact]
        public void SelfSeekingLove_SharedIsStillNotComplete()
        {
            Love love = new SelfSeekingLove();
            love.Lover = "priest";
            love.Beloved = "self";
            love.Bond.ShareWith("crowd");

            Assert.False(love.IsComplete());
        }
    }
}
