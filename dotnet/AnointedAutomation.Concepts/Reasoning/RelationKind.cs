// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields

namespace AnointedAutomation.Concepts.Reasoning
{
    /// <summary>
    /// The relation an <see cref="Assertion"/> claims between its subject and object. Value 0 is
    /// <see cref="None"/> per the repo enum convention, so an unset relation is detectable; it is not
    /// a real relation and <see cref="Assertion"/> rejects it.
    /// </summary>
    public enum RelationKind
    {
        /// <summary>
        /// No relation chosen. Never valid inside an <see cref="Assertion"/>.
        /// </summary>
        None = 0,

        /// <summary>
        /// The subject violates the object (e.g. an act violates a divine attribute).
        /// </summary>
        Violates = 1,

        /// <summary>
        /// The subject upholds the object.
        /// </summary>
        Upholds = 2,

        /// <summary>
        /// The subject is an instance of the object (e.g. a situation is an instance of a sin).
        /// </summary>
        IsInstanceOf = 3,

        /// <summary>
        /// The subject falls in the object's category.
        /// </summary>
        InCategory = 4,

        /// <summary>
        /// The subject carries the object's gravity.
        /// </summary>
        HasGravity = 5,

        /// <summary>
        /// The subject is grounded in the object (e.g. a sin grounded in a vice).
        /// </summary>
        GroundedIn = 6,

        /// <summary>
        /// The subject commands the object.
        /// </summary>
        Commands = 7,

        /// <summary>
        /// The subject forbids the object.
        /// </summary>
        Forbids = 8
    }
}
