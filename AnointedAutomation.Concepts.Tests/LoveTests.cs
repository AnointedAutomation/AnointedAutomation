// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-05-28 09:52:39
// Edited by Alexander Fields https://www.alexanderfields.me 2026-05-28 10:42:10
//Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Concepts;

namespace AnointedAutomation.Concepts.Tests
{
    public class LoveTests
    {
        // A minimal concrete love used to exercise the abstract base class directly. Its character
        // starts unformed (every virtue absent), so it is ideal for testing the shared 1 Cor 13
        // helpers in isolation from any particular kind of love.
        private sealed class TestLove : Love
        {
            public TestLove()
            {
            }

            public TestLove(string lover, string beloved) : base(lover, beloved)
            {
            }

            protected override BehaviorNode BuildBehavior()
            {
                return new Deed(new LoveAction(false, "(test)", "(test)", "(test)", null));
            }
        }

        [Fact]
        public void Love_IsAbstract()
        {
            // Assert — Love cannot be instantiated directly; how it acts is left to concrete loves.
            Assert.True(typeof(Love).IsAbstract);
        }

        [Fact]
        public void BaseLove_CreatesInstance()
        {
            // Act
            Love love = new TestLove();

            // Assert
            Assert.NotNull(love);
        }

        [Fact]
        public void UnformedLove_IsNotPerfect()
        {
            // Act
            Love love = new TestLove();

            // Assert
            Assert.False(love.IsPerfect());
        }

        [Fact]
        public void Constructor_WithLoverAndBeloved_SetsParties()
        {
            // Arrange
            string lover = "God";
            string beloved = "the world";

            // Act
            Love love = new TestLove(lover, beloved);

            // Assert
            Assert.Equal(lover, love.Lover);
            Assert.Equal(beloved, love.Beloved);
        }

        [Fact]
        public void Source_DefaultsToGod()
        {
            // Act (1 John 4:8 — "God is love.")
            Love love = new TestLove();

            // Assert
            Assert.Equal("God", love.Source);
        }

        [Fact]
        public void Reference_IsFirstCorinthians13()
        {
            // Act
            Love love = new TestLove();

            // Assert
            Assert.Equal("1 Corinthians 13:4-8", love.Reference);
        }

        [Fact]
        public void Scripture_ContainsPatientAndKind()
        {
            // Act
            Love love = new TestLove();

            // Assert
            Assert.Contains("patient", love.Scripture);
            Assert.Contains("kind", love.Scripture);
        }

        [Fact]
        public void GreatestCommandment_ContainsLoveGodAndNeighbor()
        {
            // Act (Matthew 22:37-39)
            Love love = new TestLove();

            // Assert
            Assert.Equal(2, love.GreatestCommandment.Length);
            Assert.Contains("Love the Lord your God", love.GreatestCommandment[0]);
            Assert.Contains("Love your neighbor as yourself", love.GreatestCommandment[1]);
        }

        [Fact]
        public void Agape_SetsAllVirtues_ToBiblicalStandard()
        {
            // Act (1 Corinthians 13:4-8)
            Love love = Love.Agape();

            // Assert — virtues present
            Assert.True(love.patient);
            Assert.True(love.kind);
            Assert.True(love.rejoicesWithTruth);
            Assert.True(love.protects);
            Assert.True(love.trusts);
            Assert.True(love.hopeful);
            Assert.True(love.perseveres);
            Assert.True(love.neverFails);
            Assert.True(love.sacrificial);

            // Assert — vices absent
            Assert.False(love.envious);
            Assert.False(love.boastful);
            Assert.False(love.proud);
            Assert.False(love.rude);
            Assert.False(love.selfSeeking);
            Assert.False(love.easilyAngered);
            Assert.False(love.keepsRecordOfWrongs);
            Assert.False(love.delightsInEvil);
        }

        [Fact]
        public void Agape_Factory_ReturnsAgapeInstance()
        {
            // Act
            Love love = Love.Agape();

            // Assert
            Assert.IsType<Agape>(love);
            Assert.True(love.IsPerfect());
        }

        [Fact]
        public void Agape_WithParties_SetsLoverAndBeloved()
        {
            // Arrange
            string lover = "Christ";
            string beloved = "the Church";

            // Act
            Love love = Love.Agape(lover, beloved);

            // Assert
            Assert.Equal(lover, love.Lover);
            Assert.Equal(beloved, love.Beloved);
            Assert.True(love.IsPerfect());
        }

        [Fact]
        public void NeverFails_IsTrue_ForAgape()
        {
            // Act (1 Corinthians 13:8 — "Love never fails.")
            Love love = Love.Agape();

            // Assert
            Assert.True(love.neverFails);
        }

        [Fact]
        public void Agape_Bears_Believes_Hopes_Endures_AllReturnTrue()
        {
            // Act (1 Corinthians 13:7)
            Love love = Love.Agape();

            // Assert
            Assert.True(love.Bears());
            Assert.True(love.Believes());
            Assert.True(love.Hopes());
            Assert.True(love.Endures());
        }

        [Fact]
        public void GreaterLove_ReturnsTrue_WhenSacrificial()
        {
            // Act (John 15:13 — "Greater love has no one than this...")
            Love love = Love.Agape();

            // Assert
            Assert.True(love.GreaterLove());
        }

        [Fact]
        public void GreaterLove_ReturnsFalse_WhenNotSacrificial()
        {
            // Arrange
            Love love = new TestLove();

            // Assert (unformed love is not yet sacrificial)
            Assert.False(love.GreaterLove());
        }

        [Fact]
        public void Abides_ReturnsTrue_ForAgape()
        {
            // Act (1 Corinthians 13:13 — "the greatest of these is love.")
            Love love = Love.Agape();

            // Assert
            Assert.True(love.Abides());
        }

        [Fact]
        public void Completeness_IsMax_ForAgape()
        {
            // Act
            Love love = Love.Agape();

            // Assert
            Assert.Equal(Love.MaxCompleteness, love.Completeness());
            Assert.Equal(17, love.Completeness());
        }

        [Fact]
        public void Completeness_IsZero_WhenFullOfViceAndDevoidOfVirtue()
        {
            // Arrange (1 Corinthians 13:2 — "if I have not love, I am nothing.")
            Love love = new TestLove();

            // Act — every vice present, every virtue absent
            love.envious = true;
            love.boastful = true;
            love.proud = true;
            love.rude = true;
            love.selfSeeking = true;
            love.easilyAngered = true;
            love.keepsRecordOfWrongs = true;
            love.delightsInEvil = true;

            // Assert
            Assert.Equal(0, love.Completeness());
        }

        [Fact]
        public void Completeness_UnformedLove_IsLessThanPerfect()
        {
            // Act
            Love love = new TestLove();

            // Assert — the eight vices it avoids by default, none of the nine virtues yet
            Assert.Equal(8, love.Completeness());
            Assert.True(love.Completeness() < Love.MaxCompleteness);
        }

        [Fact]
        public void IsPerfect_ReturnsFalse_WhenOneVirtueMissing()
        {
            // Arrange
            Love love = Love.Agape();

            // Act
            love.patient = false;

            // Assert
            Assert.False(love.IsPerfect());
        }

        [Fact]
        public void IsPerfect_ReturnsFalse_WhenOneVicePresent()
        {
            // Arrange
            Love love = Love.Agape();

            // Act
            love.proud = true;

            // Assert
            Assert.False(love.IsPerfect());
        }

        [Fact]
        public void Describe_ContainsScripture()
        {
            // Act
            Love love = Love.Agape();
            string description = love.Describe();

            // Assert
            Assert.Contains("patient", description);
            Assert.Contains("kind", description);
            Assert.Contains("1 Corinthians 13:4-8", description);
        }

        [Fact]
        public void Describe_IncludesParties_WhenProvided()
        {
            // Act
            Love love = Love.Agape("Christ", "the Church");
            string description = love.Describe();

            // Assert
            Assert.Contains("Christ", description);
            Assert.Contains("the Church", description);
        }

        [Fact]
        public void Describe_OmitsParties_WhenNotProvided()
        {
            // Act
            Love love = Love.Agape();
            string description = love.Describe();

            // Assert
            Assert.DoesNotContain(" loves ", description);
        }

        // EDGE CASE TESTS - per CLAUDE_TESTING.md requirements

        [Fact]
        public void Agape_WithNullParties_CreatesInstance()
        {
            // Act
            Love love = Love.Agape(null, null);

            // Assert
            Assert.NotNull(love);
            Assert.Null(love.Lover);
            Assert.Null(love.Beloved);
            Assert.True(love.IsPerfect());
        }

        [Fact]
        public void Agape_WithEmptyParties_CreatesInstance()
        {
            // Act
            Love love = Love.Agape("", "");

            // Assert
            Assert.NotNull(love);
            Assert.Equal("", love.Lover);
            Assert.Equal("", love.Beloved);
            Assert.True(love.IsPerfect());
        }

        [Fact]
        public void Describe_WithNullParties_DoesNotThrow()
        {
            // Arrange
            Love love = Love.Agape(null, null);

            // Act
            string description = love.Describe();

            // Assert
            Assert.NotNull(description);
            Assert.Contains("Love", description);
        }

        [Fact]
        public void Constructor_WithUnicodeNames_SetsParties()
        {
            // Arrange
            string lover = "Αγάπη 神 ❤";
            string beloved = "κόσμος 世界";

            // Act
            Love love = new Agape(lover, beloved);

            // Assert
            Assert.Equal(lover, love.Lover);
            Assert.Equal(beloved, love.Beloved);
        }

        [Fact]
        public void Constructor_WithVeryLongNames_CreatesInstance()
        {
            // Arrange
            string lover = new string('A', 10000);
            string beloved = new string('B', 10000);

            // Act
            Love love = new Agape(lover, beloved);

            // Assert
            Assert.NotNull(love);
            Assert.Equal(lover, love.Lover);
            Assert.Equal(beloved, love.Beloved);
        }

        [Fact]
        public void Constructor_WithNullParties_CreatesInstance()
        {
            // Act
            Love love = new Agape(null, null);

            // Assert
            Assert.NotNull(love);
            Assert.Null(love.Lover);
            Assert.Null(love.Beloved);
        }
    }
}
