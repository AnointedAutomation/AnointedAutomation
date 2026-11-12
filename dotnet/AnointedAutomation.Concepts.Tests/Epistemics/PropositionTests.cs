// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-07-02 Jesus is King ✝️
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Concepts.Epistemics;
using AnointedAutomation.Concepts.Reasoning;

namespace AnointedAutomation.Concepts.Tests.Epistemics
{
    public class PropositionTests
    {
        [Fact]
        public void Proposition_DefaultsStandingToUnknown()
        {
            // Unknown is the honest state: not established, untested, or unknowable from inside the universe.
            Proposition creator = new Proposition(
                "Creator",
                "A creator of the universe exists.",
                Testability.BeyondObservation);

            Assert.Equal(TruthValue.Unknown, creator.Standing);
            Assert.Equal(Testability.BeyondObservation, creator.Testability);
        }

        [Fact]
        public void Proposition_CarriesExplicitStanding()
        {
            Proposition entropy = new Proposition(
                "EntropyIncreases",
                "Entropy of a closed system does not decrease.",
                Testability.EmpiricallyTestable,
                TruthValue.True);

            Assert.Equal(TruthValue.True, entropy.Standing);
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
