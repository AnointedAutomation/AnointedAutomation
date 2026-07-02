// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-07-02
// Stewarded by Alexander Fields

using System.Collections.Generic;
using Xunit;
using AnointedAutomation.Concepts.Epistemics;

namespace AnointedAutomation.Concepts.Tests.Epistemics
{
    public class TensionTests
    {
        [Fact]
        public void Tension_LinksBothClaimsAndTheSharedProposition()
        {
            Proposition created = new Proposition(
                "CreatedUniverse",
                "The universe was brought into being by something outside itself.",
                Testability.BeyondObservation);
            TheologicalClaim theist = new TheologicalClaim(
                "The universe was created.", "Genesis 1:1", 0.9,
                new List<Proposition> { created }, new List<Proposition>());
            TheologicalClaim materialist = new TheologicalClaim(
                "The universe is eternal and uncaused.", "materialist cosmology", 0.6,
                new List<Proposition>(), new List<Proposition> { created });

            Tension tension = new Tension(theist, materialist, created);

            // Neither claim is deleted or rejected; the contradiction is data.
            Assert.Same(theist, tension.First);
            Assert.Same(materialist, tension.Second);
            Assert.Equal("CreatedUniverse", tension.Proposition.Name);
        }

        [Fact]
        public void Tension_ThrowsOnNullArguments()
        {
            Proposition created = new Proposition(
                "CreatedUniverse", "d", Testability.BeyondObservation);
            TheologicalClaim claim = new TheologicalClaim(
                "s", "src", 0.5, new List<Proposition> { created }, new List<Proposition>());

            Assert.Throws<System.ArgumentNullException>(() => new Tension(null, claim, created));
            Assert.Throws<System.ArgumentNullException>(() => new Tension(claim, null, created));
            Assert.Throws<System.ArgumentNullException>(() => new Tension(claim, claim, null));
        }
    }
}
