// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests
{
    public class BehaviorTreeTests
    {
        private static LoveAction Action(string deed)
        {
            return new LoveAction(true, deed, "virtue", "reference", null);
        }

        [Fact]
        public void Deed_AlwaysSucceeds_CarryingItsAction()
        {
            LoveAction action = Action("A");
            Deed deed = new Deed(action);

            BehaviorResult result = deed.Tick(new Situation());

            Assert.True(result.succeeded);
            Assert.Same(action, result.Action);
        }

        [Fact]
        public void Condition_PredicateTrue_Succeeds_WithNoAction()
        {
            Condition condition = new Condition(situation => true);

            BehaviorResult result = condition.Tick(new Situation());

            Assert.True(result.succeeded);
            Assert.Null(result.Action);
        }

        [Fact]
        public void Condition_PredicateFalse_Fails()
        {
            Condition condition = new Condition(situation => false);

            Assert.False(condition.Tick(new Situation()).succeeded);
        }

        [Fact]
        public void Condition_For_ReadsTheSituation()
        {
            Condition condition = Condition.For(new Need());

            Assert.True(condition.Tick(new Situation().With(new Need())).succeeded);
            Assert.False(condition.Tick(new Situation()).succeeded);
        }

        [Fact]
        public void Condition_And_HoldsOnlyWhenBothHold()
        {
            // sick AND imprisoned
            Condition condition = Condition.For(new Sickness()).And(new Imprisonment());

            Assert.True(condition.Tick(new Situation().With(new Sickness()).With(new Imprisonment())).succeeded);
            Assert.False(condition.Tick(new Situation().With(new Sickness())).succeeded);
            Assert.False(condition.Tick(new Situation().With(new Imprisonment())).succeeded);
            Assert.False(condition.Tick(new Situation()).succeeded);
        }

        [Fact]
        public void Condition_And_AcceptsConditionOverload()
        {
            Condition condition = Condition.For(new Circumstance("a")).And(Condition.For(new Circumstance("b")));

            Assert.True(condition.Tick(new Situation().With(new Circumstance("a")).With(new Circumstance("b"))).succeeded);
            Assert.False(condition.Tick(new Situation().With(new Circumstance("a"))).succeeded);
        }

        [Fact]
        public void Condition_Or_HoldsWhenEitherHolds()
        {
            // hungry OR thirsty
            Condition condition = Condition.For(new Hunger()).Or(new Thirst());

            Assert.True(condition.Tick(new Situation().With(new Hunger())).succeeded);
            Assert.True(condition.Tick(new Situation().With(new Thirst())).succeeded);
            Assert.True(condition.Tick(new Situation().With(new Hunger()).With(new Thirst())).succeeded);
            Assert.False(condition.Tick(new Situation()).succeeded);
        }

        [Fact]
        public void Condition_Not_NegatesTheCondition()
        {
            // NOT an enemy
            Condition condition = Condition.For(new Enmity()).Not();

            Assert.True(condition.Tick(new Situation()).succeeded);
            Assert.False(condition.Tick(new Situation().With(new Enmity())).succeeded);
        }

        [Fact]
        public void Condition_StaticNot_NegatesTheGivenCondition()
        {
            Condition condition = Condition.Not(Condition.For(new Enmity()));

            Assert.True(condition.Tick(new Situation()).succeeded);
            Assert.False(condition.Tick(new Situation().With(new Enmity())).succeeded);
        }

        [Fact]
        public void Condition_Composition_ChainsLeftToRight()
        {
            // (a AND b) OR c
            Condition condition = Condition.For(new Circumstance("a"))
                .And(new Circumstance("b")).Or(new Circumstance("c"));

            Assert.True(condition.Tick(new Situation().With(new Circumstance("a")).With(new Circumstance("b"))).succeeded);
            Assert.True(condition.Tick(new Situation().With(new Circumstance("c"))).succeeded);
            Assert.False(condition.Tick(new Situation().With(new Circumstance("a"))).succeeded);
        }

        [Fact]
        public void Condition_AndNot_Combine()
        {
            // sick AND NOT contagious
            Condition condition = Condition.For(new Sickness()).And(Condition.For(new Circumstance("contagious")).Not());

            Assert.True(condition.Tick(new Situation().With(new Sickness())).succeeded);
            Assert.False(condition.Tick(new Situation().With(new Sickness()).With(new Circumstance("contagious"))).succeeded);
        }

        [Fact]
        public void Condition_And_WithNullCondition_ThrowsArgumentNullException()
        {
            Assert.Throws<System.ArgumentNullException>(() => Condition.For(new Circumstance("a")).And((Condition)null));
        }

        [Fact]
        public void Condition_Or_WithNullCondition_ThrowsArgumentNullException()
        {
            Assert.Throws<System.ArgumentNullException>(() => Condition.For(new Circumstance("a")).Or((Condition)null));
        }

        [Fact]
        public void Condition_StaticNot_WithNull_ThrowsArgumentNullException()
        {
            Assert.Throws<System.ArgumentNullException>(() => Condition.Not(null));
        }

        [Fact]
        public void Sequence_AllChildrenSucceed_CarriesLastDeed()
        {
            LoveAction action = Action("A");
            Sequence sequence = new Sequence(new Condition(situation => true), new Deed(action));

            BehaviorResult result = sequence.Tick(new Situation());

            Assert.True(result.succeeded);
            Assert.Same(action, result.Action);
        }

        [Fact]
        public void Sequence_AnyChildFails_Fails()
        {
            Sequence sequence = new Sequence(new Condition(situation => false), new Deed(Action("A")));

            Assert.False(sequence.Tick(new Situation()).succeeded);
        }

        [Fact]
        public void Selector_ReturnsFirstChildThatSucceeds()
        {
            LoveAction first = Action("first");
            LoveAction fallback = Action("fallback");
            Selector selector = new Selector(
                new Sequence(new Condition(situation => false), new Deed(first)),
                new Deed(fallback));

            BehaviorResult result = selector.Tick(new Situation());

            Assert.True(result.succeeded);
            Assert.Same(fallback, result.Action);
        }

        [Fact]
        public void Selector_AllChildrenFail_Fails()
        {
            Selector selector = new Selector(
                new Condition(situation => false),
                new Condition(situation => false));

            Assert.False(selector.Tick(new Situation()).succeeded);
        }

        [Fact]
        public void Composition_PicksMatchingBranch_ElseFallback()
        {
            LoveAction help = Action("help");
            LoveAction fallback = Action("fallback");
            Selector tree = new Selector(
                new Sequence(Condition.For(new Need()), new Deed(help)),
                new Deed(fallback));

            Assert.Same(help, tree.Tick(new Situation().With(new Need())).Action);
            Assert.Same(fallback, tree.Tick(new Situation()).Action);
        }

        // EDGE CASE TESTS - per CLAUDE_TESTING.md requirements

        [Fact]
        public void Condition_WithNullPredicate_ThrowsArgumentNullException()
        {
            Assert.Throws<System.ArgumentNullException>(() => new Condition(null));
        }

        [Fact]
        public void Deed_WithNullAction_ThrowsArgumentNullException()
        {
            Assert.Throws<System.ArgumentNullException>(() => new Deed(null));
        }

        [Fact]
        public void Condition_For_WithNullCircumstance_ThrowsArgumentNullException()
        {
            Assert.Throws<System.ArgumentNullException>(() => Condition.For(null));
        }

        [Fact]
        public void Sequence_WithNoChildren_Succeeds_WithNoAction()
        {
            Sequence sequence = new Sequence();

            BehaviorResult result = sequence.Tick(new Situation());

            Assert.True(result.succeeded);
            Assert.Null(result.Action);
        }

        [Fact]
        public void Selector_WithNoChildren_Fails()
        {
            Selector selector = new Selector();

            Assert.False(selector.Tick(new Situation()).succeeded);
        }
    }
}
