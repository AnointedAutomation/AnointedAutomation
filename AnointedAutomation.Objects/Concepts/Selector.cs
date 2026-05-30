// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-05-28 12:05:18
// Edited by Alexander Fields https://www.alexanderfields.me 2026-05-28 12:05:18
//Stewarded by Alexander Fields

namespace AnointedAutomation.Objects.Concepts
{
    /// <summary>
    /// A composite node (logical OR / priority fallback): it ticks its children in order and returns
    /// the first one that succeeds, carrying that child's deed. If none succeed, the selector fails.
    /// A love's repertoire is a selector — its highest-priority applicable response wins, with a
    /// final <see cref="Deed"/> acting as the catch-all for situations no branch matched.
    /// </summary>
    public class Selector : BehaviorNode
    {
        private readonly System.Collections.Generic.List<BehaviorNode> children;

        /// <summary>
        /// Initializes a new instance of the <see cref="Selector"/> class.
        /// </summary>
        /// <param name="children">The child nodes, in priority order.</param>
        public Selector(params BehaviorNode[] children)
        {
            this.children = new System.Collections.Generic.List<BehaviorNode>(children);
        }

        /// <summary>
        /// Returns the result of the first child to succeed; fails if none do.
        /// </summary>
        /// <param name="situation">The situation to evaluate.</param>
        /// <returns>The first successful child's result, or failure.</returns>
        public override BehaviorResult Tick(Situation situation)
        {
            foreach (BehaviorNode child in children)
            {
                BehaviorResult result = child.Tick(situation);
                if (result.succeeded)
                {
                    return result;
                }
            }

            return BehaviorResult.Fail();
        }
    }
}
