// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-10-03 Jesus is King ✝️
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Concepts;

namespace AnointedAutomation.Concepts.Tests
{
    public class BondTests
    {
        [Fact]
        public void NewBond_IsUndecided_GateZero()
        {
            Bond bond = new Bond();

            Assert.Equal(Commitment.Unknown, bond.State);
            Assert.False(bond.IsCommitted);
            Assert.Equal(0, bond.Gate);
            Assert.Empty(bond.History);
        }

        [Fact]
        public void CommitAndWithdraw_AreAuditable()
        {
            Bond bond = new Bond().Commit("choose life").Withdraw("turned away").Commit("returned");

            Assert.Equal(1, bond.Gate);
            Assert.Equal(3, bond.History.Count);
            Assert.Equal(Commitment.Withdrawn, bond.History[1].State);
            Assert.Equal(2, bond.History[1].Sequence);
            Assert.Equal("turned away", bond.History[1].Reason);
        }

        [Fact]
        public void Commit_RequiresAReason()
        {
            Assert.Throws<System.ArgumentNullException>(() => new Bond().Commit(null));
            Assert.Throws<System.ArgumentNullException>(() => new Bond().ShareWith(""));
        }

        [Fact]
        public void Condilectio_RequiresCommitmentPartiesAndADistinctThird()
        {
            Bond bond = new Bond().ShareWith("the neighbor");
            Assert.False(bond.IsCondilectio("husband", "wife"));

            bond.Commit("vowed");
            Assert.True(bond.IsCondilectio("husband", "wife"));
            Assert.False(bond.IsCondilectio("husband", null));
            Assert.False(bond.IsCondilectio("the neighbor", "wife"));
            Assert.False(new Bond().Commit("vowed").IsCondilectio("husband", "wife"));
        }

        [Fact]
        public void Commitment_ZeroIsUnknown()
        {
            Assert.Equal(Commitment.Unknown, default(Commitment));
        }
    }
}
