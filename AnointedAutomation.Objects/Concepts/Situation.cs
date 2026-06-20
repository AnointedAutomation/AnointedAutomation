// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

namespace AnointedAutomation.Objects.Concepts
{
    /// <summary>
    /// A situation a <see cref="Love"/> must respond to, described as the set of
    /// <see cref="Circumstance"/>s that hold in it. Circumstances are first-class concepts, not
    /// strings, so any situation can be expressed, including ones never named in Scripture (for
    /// example, <c>new Circumstance("ninjaLootedTheBoss")</c>). A <see cref="Condition"/> in a love's
    /// behavior tree reads these circumstances to decide what is true.
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

        private readonly System.Collections.Generic.List<Circumstance> circumstances =
            new System.Collections.Generic.List<Circumstance>();

        /// <summary>
        /// Gets the circumstances that hold in this situation.
        /// </summary>
        public System.Collections.Generic.IReadOnlyList<Circumstance> Circumstances
        {
            get
            {
                return circumstances;
            }
        }

        /// <summary>
        /// Gets or sets a human-readable description of the situation.
        /// </summary>
        public string Description
        {
            get; set;
        }

        /// <summary>
        /// Adds a circumstance that holds in this situation, and returns the situation for fluent
        /// chaining.
        /// </summary>
        /// <param name="circumstance">The circumstance that holds.</param>
        /// <returns>This <see cref="Situation"/>, for chaining.</returns>
        public Situation With(Circumstance circumstance)
        {
            if (circumstance == null)
            {
                throw new System.ArgumentNullException(nameof(circumstance));
            }

            circumstances.Add(circumstance);
            return this;
        }

        /// <summary>
        /// Determines whether a circumstance holds in this situation. A circumstance that was never
        /// added simply does not hold, so conditions for it do not fire.
        /// </summary>
        /// <param name="circumstance">The circumstance to test for.</param>
        /// <returns><c>true</c> if the circumstance holds.</returns>
        public bool Has(Circumstance circumstance)
        {
            if (circumstance == null)
            {
                throw new System.ArgumentNullException(nameof(circumstance));
            }

            foreach (Circumstance held in circumstances)
            {
                if (held.Equals(circumstance))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
