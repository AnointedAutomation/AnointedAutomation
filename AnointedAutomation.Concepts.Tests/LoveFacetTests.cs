// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Concepts;

namespace AnointedAutomation.Concepts.Tests
{
    public class LoveFacetTests
    {
        [Fact]
        public void LoveFacet_IsNamed()
        {
            LoveFacet love = new LoveFacet();

            Assert.Equal("Love", love.Name);
        }

        [Fact]
        public void LoveFacet_IsBackedByAgape()
        {
            // God's love is agape, perfect and sourced in Him (1 John 4:8; 1 Corinthians 13:4-8).
            LoveFacet love = new LoveFacet();

            Assert.True(love.Agape.IsPerfect());
        }

        [Fact]
        public void Compassion_ReadsFullyCoherent_ToLove()
        {
            // "Love ... always protects." (1 Corinthians 13:7)
            LoveFacet love = new LoveFacet();
            Act act = new Act("binding the wounds of a stranger", new Compassion());

            Assert.Equal(1.0, love.Read(act));
        }

        [Fact]
        public void SelfSacrifice_ReadsFullyCoherent_ToLove()
        {
            // "Greater love has no one than this: to lay down one's life for one's friends." (John 15:13)
            LoveFacet love = new LoveFacet();
            Act act = new Act("the cross", new SelfSacrifice());

            Assert.Equal(1.0, love.Read(act));
        }

        [Fact]
        public void SelfSeeking_ReadsIncoherent_ToLove()
        {
            // Agape "is not self-seeking." (1 Corinthians 13:5)
            LoveFacet love = new LoveFacet();
            Act act = new Act("passing by on the other side", new SelfSeeking());

            Assert.Equal(0.0, love.Read(act));
        }
    }
}
