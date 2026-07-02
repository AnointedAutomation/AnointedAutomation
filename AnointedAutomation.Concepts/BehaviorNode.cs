// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-05-28 12:05:18
// Edited by Alexander Fields https://www.alexanderfields.me 2026-05-28 12:05:18
//Stewarded by Alexander Fields

namespace AnointedAutomation.Concepts
{
    /// <summary>
    /// A node in a behavior tree — the decision structure a <see cref="Love"/> walks to choose what
    /// to do in a <see cref="Situation"/>. Nodes compose: <see cref="Selector"/> and
    /// <see cref="Sequence"/> combine child nodes, <see cref="Condition"/> tests the situation, and
    /// <see cref="Deed"/> produces a <see cref="LoveAction"/>.
    /// </summary>
    public abstract class BehaviorNode
    {
        /// <summary>
        /// Evaluates this node against a situation.
        /// </summary>
        /// <param name="situation">The situation to evaluate.</param>
        /// <returns>The result — whether the node succeeded, and any deed it produced.</returns>
        public abstract BehaviorResult Tick(Situation situation);
    }
}
