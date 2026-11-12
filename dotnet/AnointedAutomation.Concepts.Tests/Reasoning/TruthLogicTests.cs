// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields

using AnointedAutomation.Concepts.Reasoning;
using Xunit;

namespace AnointedAutomation.Concepts.Tests.Reasoning
{
    public class TruthLogicTests
    {
        private const TruthValue T = TruthValue.True;
        private const TruthValue F = TruthValue.False;
        private const TruthValue U = TruthValue.Unknown;

        [Theory]
        [InlineData(T, T, T)]
        [InlineData(T, U, U)]
        [InlineData(T, F, F)]
        [InlineData(U, T, U)]
        [InlineData(U, U, U)]
        [InlineData(U, F, F)]
        [InlineData(F, T, F)]
        [InlineData(F, U, F)]
        [InlineData(F, F, F)]
        public void And_FollowsTheKleeneTable(TruthValue left, TruthValue right, TruthValue expected)
        {
            Assert.Equal(expected, TruthLogic.And(left, right));
        }

        [Theory]
        [InlineData(T, T, T)]
        [InlineData(T, U, T)]
        [InlineData(T, F, T)]
        [InlineData(U, T, T)]
        [InlineData(U, U, U)]
        [InlineData(U, F, U)]
        [InlineData(F, T, T)]
        [InlineData(F, U, U)]
        [InlineData(F, F, F)]
        public void Or_FollowsTheKleeneTable(TruthValue left, TruthValue right, TruthValue expected)
        {
            Assert.Equal(expected, TruthLogic.Or(left, right));
        }

        [Theory]
        [InlineData(T, F)]
        [InlineData(F, T)]
        [InlineData(U, U)]
        public void Not_FollowsTheKleeneTable(TruthValue value, TruthValue expected)
        {
            Assert.Equal(expected, TruthLogic.Not(value));
        }

        [Theory]
        [InlineData(T)]
        [InlineData(F)]
        [InlineData(U)]
        public void DoubleNegation_IsIdentity(TruthValue value)
        {
            Assert.Equal(value, TruthLogic.Not(TruthLogic.Not(value)));
        }

        [Fact]
        public void Unknown_NeverBecomesFalse()
        {
            // An unset value is "not established", never False.
            Assert.Equal(U, default(TruthValue));
            Assert.NotEqual(F, U);
            Assert.Equal(U, TruthLogic.Not(U));
            Assert.Equal(U, TruthLogic.And(U, T));
            Assert.Equal(U, TruthLogic.Or(U, F));
        }

        [Fact]
        public void TruthValue_HasNoBoolConversion()
        {
            Assert.Null(typeof(TruthValue).GetMethod("op_Implicit"));
            Assert.Null(typeof(TruthValue).GetMethod("op_Explicit"));
        }

        [Fact]
        public void UndefinedValues_AreRejected()
        {
            TruthValue bogus = (TruthValue)7;

            Assert.Throws<System.ArgumentOutOfRangeException>(() => TruthLogic.Not(bogus));
            Assert.Throws<System.ArgumentOutOfRangeException>(() => TruthLogic.And(T, bogus));
            Assert.Throws<System.ArgumentOutOfRangeException>(() => TruthLogic.Or(bogus, F));
        }
    }
}
