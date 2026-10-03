// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-10-03 Jesus is King ✝️
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Concepts;

namespace AnointedAutomation.Concepts.Tests
{
    public class TriadTests
    {
        [Fact]
        public void Exists_RequiresAllThree()
        {
            object member = new object();
            Assert.True(Triad.Exists(member, member, member));
            Assert.False(Triad.Exists(null, member, member));
            Assert.False(Triad.Exists(member, null, member));
            Assert.False(Triad.Exists(member, member, null));
        }

        [Theory]
        [InlineData(0.0, 1.0, 1.0)]
        [InlineData(1.0, 0.0, 1.0)]
        [InlineData(1.0, 1.0, 0.0)]
        public void Product_AnyFactorZero_CollapsesToZero(double a, double b, double c)
        {
            Assert.Equal(0.0, Triad.Product(a, b, c));
        }

        [Fact]
        public void Product_MultipliesAndClamps()
        {
            Assert.Equal(1.0, Triad.Product(1.0, 1.0, 1.0));
            Assert.Equal(0.25, Triad.Product(0.5, 0.5, 1.0));
            Assert.Equal(1.0, Triad.Product(2.0, 1.0, 1.0));
            Assert.Equal(0.0, Triad.Product(-1.0, 1.0, 1.0));
        }
    }
}
