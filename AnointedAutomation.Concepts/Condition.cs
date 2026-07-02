// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

namespace AnointedAutomation.Concepts
{
    /// <summary>
    /// A leaf node that tests the situation. It succeeds (producing no deed) when its predicate holds,
    /// and fails otherwise. This is where situational logic enters the tree, by reading the
    /// <see cref="Circumstance"/>s that hold.
    /// </summary>
    public class Condition : BehaviorNode
    {
        private readonly System.Func<Situation, bool> predicate;

        /// <summary>
        /// Initializes a new instance of the <see cref="Condition"/> class from a predicate over the
        /// situation.
        /// </summary>
        /// <param name="predicate">The logic that decides whether this condition holds.</param>
        public Condition(System.Func<Situation, bool> predicate)
        {
            if (predicate == null)
            {
                throw new System.ArgumentNullException(nameof(predicate));
            }

            this.predicate = predicate;
        }

        /// <summary>
        /// Creates a condition that holds when a given circumstance holds in the situation.
        /// </summary>
        /// <param name="circumstance">The circumstance to test for.</param>
        /// <returns>A <see cref="Condition"/> over that circumstance.</returns>
        public static Condition For(Circumstance circumstance)
        {
            if (circumstance == null)
            {
                throw new System.ArgumentNullException(nameof(circumstance));
            }

            return new Condition(situation => situation.Has(circumstance));
        }

        /// <summary>
        /// Succeeds when the predicate holds for the situation; fails otherwise.
        /// </summary>
        /// <param name="situation">The situation to test.</param>
        /// <returns>A successful (no deed) or failed result.</returns>
        public override BehaviorResult Tick(Situation situation)
        {
            if (predicate(situation))
            {
                return BehaviorResult.Succeed(null);
            }

            return BehaviorResult.Fail();
        }

        /// <summary>
        /// Combines this condition with another using logical AND — the result holds only when both
        /// hold (for example, "sick AND imprisoned").
        /// </summary>
        /// <param name="other">The condition to AND with this one.</param>
        /// <returns>A new <see cref="Condition"/> that holds when both hold.</returns>
        public Condition And(Condition other)
        {
            if (other == null)
            {
                throw new System.ArgumentNullException(nameof(other));
            }

            return new Condition(situation => predicate(situation) && other.predicate(situation));
        }

        /// <summary>
        /// Combines this condition with a circumstance using logical AND.
        /// </summary>
        /// <param name="circumstance">The circumstance to AND with this condition.</param>
        /// <returns>A new <see cref="Condition"/> that holds when this condition holds and the circumstance holds.</returns>
        public Condition And(Circumstance circumstance)
        {
            return And(For(circumstance));
        }

        /// <summary>
        /// Combines this condition with another using logical OR — the result holds when either holds
        /// (for example, "hungry OR thirsty").
        /// </summary>
        /// <param name="other">The condition to OR with this one.</param>
        /// <returns>A new <see cref="Condition"/> that holds when either holds.</returns>
        public Condition Or(Condition other)
        {
            if (other == null)
            {
                throw new System.ArgumentNullException(nameof(other));
            }

            return new Condition(situation => predicate(situation) || other.predicate(situation));
        }

        /// <summary>
        /// Combines this condition with a circumstance using logical OR.
        /// </summary>
        /// <param name="circumstance">The circumstance to OR with this condition.</param>
        /// <returns>A new <see cref="Condition"/> that holds when this condition holds or the circumstance holds.</returns>
        public Condition Or(Circumstance circumstance)
        {
            return Or(For(circumstance));
        }

        /// <summary>
        /// Negates this condition (logical NOT) — the result holds exactly when this condition does
        /// not (for example, "NOT an enemy").
        /// </summary>
        /// <returns>A new <see cref="Condition"/> that holds when this one does not.</returns>
        public Condition Not()
        {
            return new Condition(situation => !predicate(situation));
        }

        /// <summary>
        /// Negates the given condition (logical NOT).
        /// </summary>
        /// <param name="condition">The condition to negate.</param>
        /// <returns>A new <see cref="Condition"/> that holds when the given condition does not.</returns>
        public static Condition Not(Condition condition)
        {
            if (condition == null)
            {
                throw new System.ArgumentNullException(nameof(condition));
            }

            return condition.Not();
        }
    }
}
