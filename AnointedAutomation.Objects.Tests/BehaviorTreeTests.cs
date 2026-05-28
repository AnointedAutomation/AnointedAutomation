// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Created by Alexander Fields https://www.alexanderfields.me on 2026-05-28 12:05:18
// Edited by Alexander Fields https://www.alexanderfields.me 2026-05-28 12:05:18
//Created by Alexander Fields

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
            // Arrange
            LoveAction action = Action("A");
            Deed deed = new Deed(action);

            // Act
            BehaviorResult result = deed.Tick(new Situation());

            // Assert
            Assert.True(result.succeeded);
            Assert.Same(action, result.Action);
        }

        [Fact]
        public void Condition_PredicateTrue_Succeeds_WithNoAction()
        {
            // Arrange
            Condition condition = new Condition(situation => true);

            // Act
            BehaviorResult result = condition.Tick(new Situation());

            // Assert
            Assert.True(result.succeeded);
            Assert.Null(result.Action);
        }

        [Fact]
        public void Condition_PredicateFalse_Fails()
        {
            // Arrange
            Condition condition = new Condition(situation => false);

            // Assert
            Assert.False(condition.Tick(new Situation()).succeeded);
        }

        [Fact]
        public void Condition_Fact_ReadsTheSituation()
        {
            // Arrange
            Condition condition = Condition.Fact("inNeed");

            // Assert
            Assert.True(condition.Tick(new Situation().Set("inNeed")).succeeded);
            Assert.False(condition.Tick(new Situation()).succeeded);
        }

        [Fact]
        public void Condition_And_HoldsOnlyWhenBothHold()
        {
            // Arrange (sick AND imprisoned)
            Condition condition = Condition.Fact("sick").And("imprisoned");

            // Assert
            Assert.True(condition.Tick(new Situation().Set("sick").Set("imprisoned")).succeeded);
            Assert.False(condition.Tick(new Situation().Set("sick")).succeeded);
            Assert.False(condition.Tick(new Situation().Set("imprisoned")).succeeded);
            Assert.False(condition.Tick(new Situation()).succeeded);
        }

        [Fact]
        public void Condition_And_AcceptsConditionOverload()
        {
            // Arrange
            Condition condition = Condition.Fact("a").And(Condition.Fact("b"));

            // Assert
            Assert.True(condition.Tick(new Situation().Set("a").Set("b")).succeeded);
            Assert.False(condition.Tick(new Situation().Set("a")).succeeded);
        }

        [Fact]
        public void Condition_Or_HoldsWhenEitherHolds()
        {
            // Arrange (hungry OR thirsty)
            Condition condition = Condition.Fact("hungry").Or("thirsty");

            // Assert
            Assert.True(condition.Tick(new Situation().Set("hungry")).succeeded);
            Assert.True(condition.Tick(new Situation().Set("thirsty")).succeeded);
            Assert.True(condition.Tick(new Situation().Set("hungry").Set("thirsty")).succeeded);
            Assert.False(condition.Tick(new Situation()).succeeded);
        }

        [Fact]
        public void Condition_Not_NegatesTheCondition()
        {
            // Arrange (NOT an enemy)
            Condition condition = Condition.Fact("enemy").Not();

            // Assert
            Assert.True(condition.Tick(new Situation()).succeeded);
            Assert.False(condition.Tick(new Situation().Set("enemy")).succeeded);
        }

        [Fact]
        public void Condition_StaticNot_NegatesTheGivenCondition()
        {
            // Arrange
            Condition condition = Condition.Not(Condition.Fact("enemy"));

            // Assert
            Assert.True(condition.Tick(new Situation()).succeeded);
            Assert.False(condition.Tick(new Situation().Set("enemy")).succeeded);
        }

        [Fact]
        public void Condition_Composition_ChainsLeftToRight()
        {
            // Arrange — (a AND b) OR c
            Condition condition = Condition.Fact("a").And("b").Or("c");

            // Assert
            Assert.True(condition.Tick(new Situation().Set("a").Set("b")).succeeded);
            Assert.True(condition.Tick(new Situation().Set("c")).succeeded);
            Assert.False(condition.Tick(new Situation().Set("a")).succeeded);
        }

        [Fact]
        public void Condition_AndNot_Combine()
        {
            // Arrange — sick AND NOT contagious
            Condition condition = Condition.Fact("sick").And(Condition.Fact("contagious").Not());

            // Assert
            Assert.True(condition.Tick(new Situation().Set("sick")).succeeded);
            Assert.False(condition.Tick(new Situation().Set("sick").Set("contagious")).succeeded);
        }

        [Fact]
        public void Condition_And_WithNullCondition_ThrowsArgumentNullException()
        {
            Assert.Throws<System.ArgumentNullException>(() => Condition.Fact("a").And((Condition)null));
        }

        [Fact]
        public void Condition_Or_WithNullCondition_ThrowsArgumentNullException()
        {
            Assert.Throws<System.ArgumentNullException>(() => Condition.Fact("a").Or((Condition)null));
        }

        [Fact]
        public void Condition_StaticNot_WithNull_ThrowsArgumentNullException()
        {
            Assert.Throws<System.ArgumentNullException>(() => Condition.Not(null));
        }

        [Fact]
        public void Sequence_AllChildrenSucceed_CarriesLastDeed()
        {
            // Arrange
            LoveAction action = Action("A");
            Sequence sequence = new Sequence(new Condition(situation => true), new Deed(action));

            // Act
            BehaviorResult result = sequence.Tick(new Situation());

            // Assert
            Assert.True(result.succeeded);
            Assert.Same(action, result.Action);
        }

        [Fact]
        public void Sequence_AnyChildFails_Fails()
        {
            // Arrange
            Sequence sequence = new Sequence(new Condition(situation => false), new Deed(Action("A")));

            // Assert
            Assert.False(sequence.Tick(new Situation()).succeeded);
        }

        [Fact]
        public void Selector_ReturnsFirstChildThatSucceeds()
        {
            // Arrange
            LoveAction first = Action("first");
            LoveAction fallback = Action("fallback");
            Selector selector = new Selector(
                new Sequence(new Condition(situation => false), new Deed(first)),
                new Deed(fallback));

            // Act
            BehaviorResult result = selector.Tick(new Situation());

            // Assert
            Assert.True(result.succeeded);
            Assert.Same(fallback, result.Action);
        }

        [Fact]
        public void Selector_AllChildrenFail_Fails()
        {
            // Arrange
            Selector selector = new Selector(
                new Condition(situation => false),
                new Condition(situation => false));

            // Assert
            Assert.False(selector.Tick(new Situation()).succeeded);
        }

        [Fact]
        public void Composition_PicksMatchingBranch_ElseFallback()
        {
            // Arrange
            LoveAction help = Action("help");
            LoveAction fallback = Action("fallback");
            Selector tree = new Selector(
                new Sequence(Condition.Fact("inNeed"), new Deed(help)),
                new Deed(fallback));

            // Assert
            Assert.Same(help, tree.Tick(new Situation().Set("inNeed")).Action);
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
        public void Condition_Fact_WithNullName_ThrowsArgumentNullException()
        {
            Assert.Throws<System.ArgumentNullException>(() => Condition.Fact(null));
        }

        [Fact]
        public void Sequence_WithNoChildren_Succeeds_WithNoAction()
        {
            // Arrange — a vacuous sequence (no children) succeeds, carrying no deed.
            Sequence sequence = new Sequence();

            // Act
            BehaviorResult result = sequence.Tick(new Situation());

            // Assert
            Assert.True(result.succeeded);
            Assert.Null(result.Action);
        }

        [Fact]
        public void Selector_WithNoChildren_Fails()
        {
            // Arrange — a selector with nothing to try fails.
            Selector selector = new Selector();

            // Assert
            Assert.False(selector.Tick(new Situation()).succeeded);
        }
    }
}
