// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-07-02
// Stewarded by Alexander Fields

using System.Collections.Generic;
using Xunit;
using AnointedAutomation.Objects.Concepts.Epistemics;

namespace AnointedAutomation.Objects.Tests.Epistemics
{
    public class TheologicalClaimTests
    {
        private static Proposition CreatedUniverse()
        {
            return new Proposition(
                "CreatedUniverse",
                "The universe was brought into being by something outside itself.",
                Testability.BeyondObservation);
        }

        [Fact]
        public void TheologicalClaim_CarriesSourceAndConfidence()
        {
            TheologicalClaim genesis = new TheologicalClaim(
                "In the beginning God created the heavens and the earth.",
                "Genesis 1:1",
                0.9,
                new List<Proposition> { CreatedUniverse() },
                new List<Proposition>());

            Assert.Equal("Genesis 1:1", genesis.Source);
            Assert.Equal(0.9, genesis.Confidence);
            Assert.True(genesis.AssertsProposition(CreatedUniverse()));
            Assert.False(genesis.DeniesProposition(CreatedUniverse()));
        }

        [Fact]
        public void TheologicalClaim_ThrowsWhenItTouchesNoPropositions()
        {
            Assert.Throws<System.ArgumentException>(() => new TheologicalClaim(
                "s", "src", 0.5, new List<Proposition>(), new List<Proposition>()));
        }

        [Fact]
        public void TheologicalClaim_ThrowsOnConfidenceOutOfRange()
        {
            Assert.Throws<System.ArgumentOutOfRangeException>(() => new TheologicalClaim(
                "s", "src", -0.1,
                new List<Proposition> { CreatedUniverse() }, new List<Proposition>()));
        }

        [Fact]
        public void TheologicalClaim_ThrowsOnNullArguments()
        {
            Assert.Throws<System.ArgumentException>(() => new TheologicalClaim(
                null, "src", 0.5, new List<Proposition> { CreatedUniverse() }, new List<Proposition>()));
            Assert.Throws<System.ArgumentException>(() => new TheologicalClaim(
                "s", null, 0.5, new List<Proposition> { CreatedUniverse() }, new List<Proposition>()));
            Assert.Throws<System.ArgumentNullException>(() => new TheologicalClaim(
                "s", "src", 0.5, null, new List<Proposition>()));
            Assert.Throws<System.ArgumentNullException>(() => new TheologicalClaim(
                "s", "src", 0.5, new List<Proposition> { CreatedUniverse() }, null));
        }
    }
}
