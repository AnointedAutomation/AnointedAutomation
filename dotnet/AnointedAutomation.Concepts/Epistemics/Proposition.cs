// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-07-02 Jesus is King ✝️
// Stewarded by Alexander Fields

using AnointedAutomation.Concepts.Reasoning;

namespace AnointedAutomation.Concepts.Epistemics
{
    /// <summary>
    /// One entry in the shared vocabulary through which claims interact. The engine does not parse
    /// natural language; claims declare which propositions they assert and deny, and checking is
    /// set logic over these. Standing is a three-valued <see cref="TruthValue"/>: True (asserted, so
    /// far unfalsified), False (falsified), Unknown (untested or unknowable). Unknown is the honest
    /// state for most of theology and is never defaulted or guessed away to False.
    /// </summary>
    public class Proposition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Proposition"/> class with an
        /// <see cref="TruthValue.Unknown"/> standing.
        /// </summary>
        /// <param name="name">The vocabulary name, e.g. "CreatedUniverse".</param>
        /// <param name="description">What the proposition asserts, in plain words.</param>
        /// <param name="testability">Whether it can be tested from inside the universe.</param>
        public Proposition(string name, string description, Testability testability)
            : this(name, description, testability, TruthValue.Unknown)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Proposition"/> class with an explicit
        /// three-valued standing.
        /// </summary>
        /// <param name="name">The vocabulary name, e.g. "CreatedUniverse".</param>
        /// <param name="description">What the proposition asserts, in plain words.</param>
        /// <param name="testability">Whether it can be tested from inside the universe.</param>
        /// <param name="standing">True (so far unfalsified), False (falsified), or Unknown.</param>
        public Proposition(string name, string description, Testability testability, TruthValue standing)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new System.ArgumentException("A proposition requires a name.", nameof(name));
            }

            if (description == null)
            {
                throw new System.ArgumentNullException(nameof(description));
            }

            Name = name;
            Description = description;
            Testability = testability;
            Standing = standing;
        }

        /// <summary>
        /// The vocabulary name. Propositions are equal when their names match ordinally, so two
        /// claims touch the same proposition by using the same name.
        /// </summary>
        public string Name
        {
            get;
        }

        /// <summary>
        /// What the proposition asserts, in plain words.
        /// </summary>
        public string Description
        {
            get;
        }

        /// <summary>
        /// Whether the proposition can be tested from inside the universe.
        /// </summary>
        public Testability Testability
        {
            get;
        }

        /// <summary>
        /// The three-valued standing: True (so far unfalsified), False (falsified), or Unknown
        /// (untested or unknowable). Unknown is "not established", never False.
        /// </summary>
        public TruthValue Standing
        {
            get;
        }

        /// <summary>
        /// Two propositions are the same vocabulary entry when their names match ordinally.
        /// </summary>
        /// <param name="obj">The object to compare with.</param>
        /// <returns>True when <paramref name="obj"/> is a proposition with the same name.</returns>
        public override bool Equals(object obj)
        {
            Proposition other = obj as Proposition;
            if (other == null)
            {
                return false;
            }

            return Name.Equals(other.Name, System.StringComparison.Ordinal);
        }

        /// <summary>
        /// Hash code derived from the name, matching <see cref="Equals(object)"/>.
        /// </summary>
        /// <returns>The ordinal hash of the name.</returns>
        public override int GetHashCode()
        {
            return System.StringComparer.Ordinal.GetHashCode(Name);
        }
    }
}
