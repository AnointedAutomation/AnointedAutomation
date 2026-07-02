// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-07-02
// Stewarded by Alexander Fields

using System.Collections.Generic;
using Xunit;
using AnointedAutomation.Objects.Concepts.Epistemics;

namespace AnointedAutomation.Objects.Tests.Epistemics
{
    public class FoundationalClaimTests
    {
        private static Proposition Causality()
        {
            return new Proposition(
                "EffectsHaveCauses",
                "Within the universe, effects have causes.",
                Testability.EmpiricallyTestable,
                true);
        }

        [Fact]
        public void FoundationalClaim_IsFalsifiableByDefinition()
        {
            // Even laws are held by faith: asserted, and open to being proven wrong. That is what
            // makes them scientific rather than decreed.
            FoundationalClaim causality = new FoundationalClaim(
                "Causality",
                "Within the universe, effects have causes.",
                LawDomain.IntraUniverse,
                new List<Proposition> { Causality() },
                new List<Proposition>(),
                0.99);

            Assert.True(causality.Falsifiable);
            Assert.Equal(0.99, causality.SurvivedFalsificationWeight);
            Assert.True(causality.AssertsProposition(Causality()));
            Assert.False(causality.DeniesProposition(Causality()));
        }

        [Fact]
        public void FoundationalClaim_ThrowsOnWeightOutOfRange()
        {
            Assert.Throws<System.ArgumentOutOfRangeException>(() => new FoundationalClaim(
                "Causality",
                "s",
                LawDomain.IntraUniverse,
                new List<Proposition> { Causality() },
                new List<Proposition>(),
                1.5));
        }

        [Fact]
        public void FoundationalClaim_ThrowsWhenItTouchesNoPropositions()
        {
            Assert.Throws<System.ArgumentException>(() => new FoundationalClaim(
                "Empty",
                "s",
                LawDomain.IntraUniverse,
                new List<Proposition>(),
                new List<Proposition>(),
                0.5));
        }

        [Fact]
        public void FoundationalClaim_ThrowsOnNullArguments()
        {
            Assert.Throws<System.ArgumentException>(() => new FoundationalClaim(
                null, "s", LawDomain.IntraUniverse,
                new List<Proposition> { Causality() }, new List<Proposition>(), 0.5));
            Assert.Throws<System.ArgumentNullException>(() => new FoundationalClaim(
                "Causality", "s", LawDomain.IntraUniverse,
                null, new List<Proposition>(), 0.5));
            Assert.Throws<System.ArgumentNullException>(() => new FoundationalClaim(
                "Causality", "s", LawDomain.IntraUniverse,
                new List<Proposition> { Causality() }, null, 0.5));
        }
    }
}
