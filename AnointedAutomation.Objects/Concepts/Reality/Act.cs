// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

namespace AnointedAutomation.Objects.Concepts
{
    /// <summary>
    /// A deed presented to reality, composed of the moral concepts it embodies and offends. This
    /// replaces the old string blackboard: an act is no longer a bag of fact-names but a set of
    /// first-class <see cref="MoralConcept"/> entities, each carrying its own Scripture and stance
    /// toward God's character. Any situation can still be expressed, including ones never named in
    /// Scripture, by composing the concepts it involves.
    /// </summary>
    public class Act
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Act"/> class from the moral concepts it
        /// embodies.
        /// </summary>
        /// <param name="description">A human-readable description of the deed.</param>
        /// <param name="concepts">The moral concepts this deed involves.</param>
        public Act(string description, params MoralConcept[] concepts)
        {
            if (concepts == null)
            {
                throw new System.ArgumentNullException(nameof(concepts));
            }

            Description = description;
            Concepts = concepts;
        }

        private Circumstance[] context = new Circumstance[0];

        /// <summary>
        /// A human-readable description of the deed.
        /// </summary>
        public string Description
        {
            get;
        }

        /// <summary>
        /// The moral concepts this deed embodies and offends.
        /// </summary>
        public System.Collections.Generic.IReadOnlyList<MoralConcept> Concepts
        {
            get;
        }

        /// <summary>
        /// The context in which the deed is done (war, divine command, defense of the innocent). A
        /// concept can be read differently in light of context: Rahab's deception to save the spies
        /// from murderers is not the lie God condemns (Joshua 2; Hebrews 11:31). Most acts carry no
        /// context; the engine then reads concepts plainly.
        /// </summary>
        public System.Collections.Generic.IReadOnlyList<Circumstance> Context
        {
            get
            {
                return context;
            }
        }

        /// <summary>
        /// Sets the context of this deed and returns it for fluent chaining.
        /// </summary>
        /// <param name="circumstances">The circumstances that frame the deed.</param>
        /// <returns>This <see cref="Act"/>.</returns>
        public Act InContext(params Circumstance[] circumstances)
        {
            if (circumstances == null)
            {
                throw new System.ArgumentNullException(nameof(circumstances));
            }

            context = circumstances;
            return this;
        }

        /// <summary>
        /// Whether a given context frames this deed.
        /// </summary>
        /// <param name="circumstance">The context to test for.</param>
        /// <returns><c>true</c> if the deed is done in that context.</returns>
        public bool InContextOf(Circumstance circumstance)
        {
            if (circumstance == null)
            {
                throw new System.ArgumentNullException(nameof(circumstance));
            }

            foreach (Circumstance held in context)
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
