// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Created by Alexander Fields https://www.alexanderfields.me on 2026-05-28 12:05:18
// Edited by Alexander Fields https://www.alexanderfields.me 2026-05-28 12:05:18
//Created by Alexander Fields

namespace AnointedAutomation.Objects.Concepts
{
    /// <summary>
    /// A situation a <see cref="Love"/> must respond to, described as a blackboard of named facts.
    /// Facts are arbitrary strings, so any situation can be expressed — including ones never named in
    /// Scripture (for example, "betrayedTrust", "ninjaLootedTheBoss", "ghostedMe"). A
    /// <see cref="Condition"/> in a love's behavior tree reads these facts to decide what is true.
    /// </summary>
    public class Situation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Situation"/> class.
        /// </summary>
        public Situation()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Situation"/> class with a description.
        /// </summary>
        /// <param name="description">A human-readable description of the situation.</param>
        public Situation(string description)
        {
            Description = description;
        }

        /// <summary>
        /// Gets or sets the named facts that describe this situation.
        /// </summary>
        public System.Collections.Generic.Dictionary<string, bool> Facts { get; set; } =
            new System.Collections.Generic.Dictionary<string, bool>();

        /// <summary>
        /// Gets or sets a human-readable description of the situation.
        /// </summary>
        public string Description
        {
            get; set;
        }

        /// <summary>
        /// Records a fact about the situation and returns the situation for fluent chaining.
        /// </summary>
        /// <param name="fact">The fact name.</param>
        /// <param name="value">Whether the fact is true.</param>
        /// <returns>This <see cref="Situation"/>, for chaining.</returns>
        public Situation Set(string fact, bool value)
        {
            if (string.IsNullOrEmpty(fact))
            {
                throw new System.ArgumentNullException(nameof(fact));
            }

            Facts[fact] = value;
            return this;
        }

        /// <summary>
        /// Records that a fact is true about the situation, and returns the situation for fluent
        /// chaining.
        /// </summary>
        /// <param name="fact">The fact name.</param>
        /// <returns>This <see cref="Situation"/>, for chaining.</returns>
        public Situation Set(string fact)
        {
            return Set(fact, true);
        }

        /// <summary>
        /// Determines whether a fact is present and true. An unrecorded fact is treated as not true,
        /// so conditions for facts that were never set simply do not hold.
        /// </summary>
        /// <param name="fact">The fact name.</param>
        /// <returns><c>true</c> if the fact was recorded as true; otherwise <c>false</c>.</returns>
        public bool Is(string fact)
        {
            if (string.IsNullOrEmpty(fact))
            {
                throw new System.ArgumentNullException(nameof(fact));
            }

            bool value;
            if (Facts.TryGetValue(fact, out value))
            {
                return value;
            }

            return false;
        }

        /// <summary>
        /// Determines whether a fact has been recorded at all (regardless of its value).
        /// </summary>
        /// <param name="fact">The fact name.</param>
        /// <returns><c>true</c> if the fact is present.</returns>
        public bool Has(string fact)
        {
            if (string.IsNullOrEmpty(fact))
            {
                throw new System.ArgumentNullException(nameof(fact));
            }

            return Facts.ContainsKey(fact);
        }
    }
}
