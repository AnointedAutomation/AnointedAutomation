// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Concepts;

namespace AnointedAutomation.Concepts.Tests
{
    public class MercyTests
    {
        [Fact]
        public void Mercy_IsNamed()
        {
            Mercy mercy = new Mercy();

            Assert.Equal("Mercy", mercy.Name);
        }

        [Fact]
        public void Pardon_ReadsFullyCoherent_ToMercy()
        {
            // "While we were still sinners, Christ died for us." (Romans 5:8); the guilty spared.
            Mercy mercy = new Mercy();
            Act act = new Act("the guilty pardoned", new Pardon());

            Assert.Equal(1.0, mercy.Read(act));
        }

        [Fact]
        public void Compassion_ReadsFullyCoherent_ToMercy()
        {
            // "Be merciful, even as your Father is merciful." (Luke 6:36)
            Mercy mercy = new Mercy();
            Act act = new Act("tending the wounded", new Compassion());

            Assert.Equal(1.0, mercy.Read(act));
        }

        [Fact]
        public void Condemnation_ReadsIncoherent_ToMercy()
        {
            // "A bruised reed he will not break." (Matthew 12:20)
            Mercy mercy = new Mercy();
            Act act = new Act("crushing the contrite", new Condemnation());

            Assert.Equal(0.0, mercy.Read(act));
        }
    }
}
