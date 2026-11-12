// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields

namespace AnointedAutomation.Concepts.Reasoning
{
    /// <summary>
    /// Strong Kleene connectives over <see cref="TruthValue"/>. False dominates And, True dominates
    /// Or, and Not leaves Unknown as Unknown, so Unknown is never silently turned into False.
    /// Undefined enum values are rejected rather than guessed.
    /// </summary>
    public static class TruthLogic
    {
        /// <summary>
        /// Kleene negation: True becomes False, False becomes True, Unknown stays Unknown.
        /// </summary>
        /// <param name="value">The value to negate.</param>
        /// <returns>The negated value.</returns>
        public static TruthValue Not(TruthValue value)
        {
            switch (Require(value, nameof(value)))
            {
                case TruthValue.True:
                    return TruthValue.False;
                case TruthValue.False:
                    return TruthValue.True;
                default:
                    return TruthValue.Unknown;
            }
        }

        /// <summary>
        /// Kleene conjunction: False if either is False, True if both are True, otherwise Unknown.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The conjunction.</returns>
        public static TruthValue And(TruthValue left, TruthValue right)
        {
            Require(left, nameof(left));
            Require(right, nameof(right));
            if (left == TruthValue.False || right == TruthValue.False)
            {
                return TruthValue.False;
            }

            if (left == TruthValue.True && right == TruthValue.True)
            {
                return TruthValue.True;
            }

            return TruthValue.Unknown;
        }

        /// <summary>
        /// Kleene disjunction: True if either is True, False if both are False, otherwise Unknown.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The disjunction.</returns>
        public static TruthValue Or(TruthValue left, TruthValue right)
        {
            return Not(And(Not(left), Not(right)));
        }

        private static TruthValue Require(TruthValue value, string parameterName)
        {
            if (!System.Enum.IsDefined(typeof(TruthValue), value))
            {
                throw new System.ArgumentOutOfRangeException(parameterName, value, "Undefined truth value.");
            }

            return value;
        }
    }
}
