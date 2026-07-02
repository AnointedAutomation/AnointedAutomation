// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-05-28 12:05:18
// Edited by Alexander Fields https://www.alexanderfields.me 2026-05-28 12:05:18
//Stewarded by Alexander Fields

namespace AnointedAutomation.Concepts
{
    /// <summary>
    /// A composite node (logical AND): it ticks its children in order and fails as soon as one fails.
    /// If all children succeed, the sequence succeeds and carries the last deed any child produced —
    /// so a sequence of conditions followed by a <see cref="Deed"/> yields that deed only when every
    /// condition holds.
    /// </summary>
    public class Sequence : BehaviorNode
    {
        private readonly System.Collections.Generic.List<BehaviorNode> children;

        /// <summary>
        /// Initializes a new instance of the <see cref="Sequence"/> class.
        /// </summary>
        /// <param name="children">The child nodes that must all succeed, in order.</param>
        public Sequence(params BehaviorNode[] children)
        {
            this.children = new System.Collections.Generic.List<BehaviorNode>(children);
        }

        /// <summary>
        /// Ticks each child in order; fails on the first failure; otherwise succeeds carrying the last
        /// deed produced.
        /// </summary>
        /// <param name="situation">The situation to evaluate.</param>
        /// <returns>The sequence result.</returns>
        public override BehaviorResult Tick(Situation situation)
        {
            LoveAction action = null;
            foreach (BehaviorNode child in children)
            {
                BehaviorResult result = child.Tick(situation);
                if (!result.succeeded)
                {
                    return BehaviorResult.Fail();
                }

                if (result.Action != null)
                {
                    action = result.Action;
                }
            }

            return BehaviorResult.Succeed(action);
        }
    }
}
