// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-07-02
// Stewarded by Alexander Fields

namespace AnointedAutomation.Objects.Concepts.Epistemics
{
    /// <summary>
    /// One entry in the shared vocabulary through which claims interact. The engine does not parse
    /// natural language; claims declare which propositions they assert and deny, and checking is
    /// set logic over these. Standing is three-valued: true (asserted, so far unfalsified), false
    /// (falsified), null (unknown, untested, or unknowable). Null is the honest state for most of
    /// theology and is never defaulted or guessed away.
    /// </summary>
    public class Proposition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Proposition"/> class with an unknown
        /// (null) standing.
        /// </summary>
        /// <param name="name">The vocabulary name, e.g. "CreatedUniverse".</param>
        /// <param name="description">What the proposition asserts, in plain words.</param>
        /// <param name="testability">Whether it can be tested from inside the universe.</param>
        public Proposition(string name, string description, Testability testability)
            : this(name, description, testability, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Proposition"/> class with an explicit
        /// three-valued standing.
        /// </summary>
        /// <param name="name">The vocabulary name, e.g. "CreatedUniverse".</param>
        /// <param name="description">What the proposition asserts, in plain words.</param>
        /// <param name="testability">Whether it can be tested from inside the universe.</param>
        /// <param name="standing">True (so far unfalsified), false (falsified), or null (unknown).</param>
        public Proposition(string name, string description, Testability testability, bool? standing)
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
        /// The three-valued standing: true (so far unfalsified), false (falsified), or null
        /// (unknown, untested, or unknowable).
        /// </summary>
        public bool? Standing
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
