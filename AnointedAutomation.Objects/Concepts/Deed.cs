// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-05-28 12:05:18
// Edited by Alexander Fields https://www.alexanderfields.me 2026-05-28 12:05:18
//Stewarded by Alexander Fields

namespace AnointedAutomation.Objects.Concepts
{
    /// <summary>
    /// A leaf node that produces a <see cref="LoveAction"/>. It always succeeds, carrying its deed —
    /// so it serves both as a response within a <see cref="Sequence"/> and as a fallback at the end
    /// of a <see cref="Selector"/>.
    /// </summary>
    public class Deed : BehaviorNode
    {
        private readonly LoveAction action;

        /// <summary>
        /// Initializes a new instance of the <see cref="Deed"/> class.
        /// </summary>
        /// <param name="action">The deed this node produces.</param>
        public Deed(LoveAction action)
        {
            if (action == null)
            {
                throw new System.ArgumentNullException(nameof(action));
            }

            this.action = action;
        }

        /// <summary>
        /// Always succeeds, carrying this node's deed.
        /// </summary>
        /// <param name="situation">The situation (unused; a deed is unconditional once reached).</param>
        /// <returns>A successful result carrying the deed.</returns>
        public override BehaviorResult Tick(Situation situation)
        {
            return BehaviorResult.Succeed(action);
        }
    }
}
