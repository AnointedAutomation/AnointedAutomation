// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

namespace AnointedAutomation.Concepts
{
    /// <summary>
    /// A moral concept modeled as abstract code rather than a magic string. Each concrete concept
    /// (Compassion, Murder, Sacrifice, Obedience, Idolatry, ...) is a first-class entity that knows
    /// its own name, the Scripture it answers to, and how it bears on the facets of God's character:
    /// which facets it embodies (<see cref="Upholds"/>) and which it offends (<see cref="Violates"/>).
    /// This puts the moral nature of an act on the concept itself, once, instead of scattering it
    /// across string lists inside each facet.
    /// </summary>
    public abstract class MoralConcept : Concept
    {
        /// <summary>
        /// The Scripture this concept answers to.
        /// </summary>
        public abstract string Scripture
        {
            get;
        }

        /// <summary>
        /// How heinous this concept is, and so how much disorder it unleashes into reality when it is
        /// committed. A virtue has <see cref="Gravity.None"/>; each sin declares its weight, from a
        /// minor wrong to a capital one (Scripture grades sin, Matthew 23:23; John 19:11).
        /// </summary>
        public virtual Gravity Gravity
        {
            get
            {
                return Gravity.None;
            }
        }

        /// <summary>
        /// Whether this concept embodies the given facet of God's character. Indifferent by default;
        /// concrete concepts declare the facets they uphold.
        /// </summary>
        /// <param name="facet">A facet of God's character.</param>
        /// <returns><c>true</c> if this concept embodies that facet.</returns>
        public virtual bool Upholds(DivineAttribute facet)
        {
            return false;
        }

        /// <summary>
        /// Whether this concept offends the given facet of God's character. Indifferent by default;
        /// concrete concepts declare the facets they violate.
        /// </summary>
        /// <param name="facet">A facet of God's character.</param>
        /// <returns><c>true</c> if this concept offends that facet.</returns>
        public virtual bool Violates(DivineAttribute facet)
        {
            return false;
        }

        /// <summary>
        /// Whether this concept embodies the given facet, read in the light of the deed's context. By
        /// default context is ignored and the plain reading stands; a context-sensitive concept
        /// overrides this to respond to war, divine command, or the defense of the innocent.
        /// </summary>
        /// <param name="facet">A facet of God's character.</param>
        /// <param name="act">The whole deed, carrying its context.</param>
        /// <returns><c>true</c> if this concept embodies that facet here.</returns>
        public virtual bool Upholds(DivineAttribute facet, Act act)
        {
            return Upholds(facet);
        }

        /// <summary>
        /// Whether this concept offends the given facet, read in the light of the deed's context. By
        /// default context is ignored and the plain reading stands; a context-sensitive concept
        /// overrides this (for example, a deception to save the innocent is not counted as the lie
        /// God condemns, Joshua 2; Hebrews 11:31).
        /// </summary>
        /// <param name="facet">A facet of God's character.</param>
        /// <param name="act">The whole deed, carrying its context.</param>
        /// <returns><c>true</c> if this concept offends that facet here.</returns>
        public virtual bool Violates(DivineAttribute facet, Act act)
        {
            return Violates(facet);
        }

        /// <summary>
        /// How much this concept restores the order that sin has broken, from 0.0 (none) to 1.0 (full).
        /// Most concepts restore nothing; repentance, atonement, forgiveness, and grace heal the
        /// record (2 Chronicles 7:14; 1 John 1:9; Romans 5:20).
        /// </summary>
        public virtual double Restoration
        {
            get
            {
                return 0.0;
            }
        }
    }
}
