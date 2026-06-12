// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

namespace AnointedAutomation.Objects.Concepts
{
    /// <summary>
    /// The root of the project's concept model: an idea modeled as first-class code rather than a
    /// magic string. Two kinds descend from it, and distinguishing them in the type system is itself
    /// meaningful, because a sin is not the same kind of thing as a situation:
    /// <list type="bullet">
    /// <item><description>A <see cref="MoralConcept"/> bears on the facets of God's character: it is
    /// a virtue or a sin (Compassion, Murder), with a Scripture, a stance, and a gravity.</description></item>
    /// <item><description>A <see cref="Circumstance"/> is a state of the world that love responds to
    /// (hunger, enmity, grief). It carries no moral weight of its own; it is simply the case.</description></item>
    /// </list>
    /// </summary>
    public abstract class Concept
    {
        /// <summary>
        /// The name of this concept.
        /// </summary>
        public abstract string Name
        {
            get;
        }
    }
}
