// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-07-02
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Concepts.Epistemics;

namespace AnointedAutomation.Concepts.Tests.Epistemics
{
    public class PropositionTests
    {
        [Fact]
        public void Proposition_DefaultsStandingToNull()
        {
            // null is the honest state: unknown, untested, or unknowable from inside the universe.
            Proposition creator = new Proposition(
                "Creator",
                "A creator of the universe exists.",
                Testability.BeyondObservation);

            Assert.Null(creator.Standing);
            Assert.Equal(Testability.BeyondObservation, creator.Testability);
        }

        [Fact]
        public void Proposition_CarriesExplicitStanding()
        {
            Proposition entropy = new Proposition(
                "EntropyIncreases",
                "Entropy of a closed system does not decrease.",
                Testability.EmpiricallyTestable,
                true);

            Assert.True(entropy.Standing);
        }

        [Fact]
        public void Proposition_EqualityIsByName()
        {
            Proposition a = new Proposition("Creator", "one wording", Testability.BeyondObservation);
            Proposition b = new Proposition("Creator", "another wording", Testability.BeyondObservation);

            Assert.Equal(a, b);
            Assert.Equal(a.GetHashCode(), b.GetHashCode());
        }

        [Fact]
        public void Proposition_ThrowsOnNullOrEmptyName()
        {
            Assert.Throws<System.ArgumentException>(
                () => new Proposition(null, "d", Testability.EmpiricallyTestable));
            Assert.Throws<System.ArgumentException>(
                () => new Proposition("", "d", Testability.EmpiricallyTestable));
        }

        [Fact]
        public void Proposition_ThrowsOnNullDescription()
        {
            Assert.Throws<System.ArgumentNullException>(
                () => new Proposition("Creator", null, Testability.BeyondObservation));
        }
    }
}
