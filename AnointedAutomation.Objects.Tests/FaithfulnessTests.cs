// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests
{
    public class FaithfulnessTests
    {
        [Fact]
        public void Faithfulness_IsNamed()
        {
            Faithfulness faith = new Faithfulness();

            Assert.Equal("Faithfulness", faith.Name);
        }

        [Fact]
        public void CovenantFaithfulness_ReadsFullyCoherent_ToOrder()
        {
            // "Great is your faithfulness." (Lamentations 3:23)
            Faithfulness faith = new Faithfulness();
            Act act = new Act("a covenant kept", new CovenantFaithfulness());

            Assert.Equal(1.0, faith.Read(act));
        }

        [Fact]
        public void Treachery_ReadsIncoherent_ToOrder()
        {
            // Breaking faith transgresses the order God decreed (Malachi 2:10; 1 Enoch 80).
            Faithfulness faith = new Faithfulness();
            Act act = new Act("a covenant betrayed", new Treachery());

            Assert.Equal(0.0, faith.Read(act));
        }
    }
}
