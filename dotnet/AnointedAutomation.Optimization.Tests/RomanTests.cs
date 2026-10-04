// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Copyright 2026 Anointed Automation, LLC. All Rights Reserved.

using AnointedAutomation.Optimization;
using Xunit;

namespace AnointedAutomation.Optimization.Tests
{
    public class RomanTests
    {
        // Values below 1000 use the standard additive/subtractive form.
        [Theory]
        [InlineData(1, "I")]
        [InlineData(4, "IV")]
        [InlineData(9, "IX")]
        [InlineData(26, "XXVI")]
        [InlineData(999, "CMXCIX")]
        public void IntegerToRoman_StandardBelowThousand(int number, string expected)
        {
            Assert.Equal(expected, Roman.IntegerToRoman(number));
        }

        [Fact]
        public void IntegerToRoman_LargeNumbersUseOverlineGroups()
        {
            // The library encodes thousands as an overlined group, not as repeated M.
            Assert.Equal("II̅XXVI", Roman.IntegerToRoman(2026));
        }

        [Fact]
        public void IntegerToRoman_ZeroAndNegative()
        {
            Assert.Equal("nulla", Roman.IntegerToRoman(0));
            Assert.Equal("Negative numbers are invalid", Roman.IntegerToRoman(-5));
        }

        [Theory]
        [InlineData("IV", 4)]
        [InlineData("MMXXVI", 2026)]
        [InlineData("", 0)]
        public void RomanToInteger_KnownValues(string roman, int expected)
        {
            Assert.Equal(expected, Roman.RomanToInteger(roman));
        }

        [Fact]
        public void ContainsRomanNumeralWord_DetectsWholeWords()
        {
            Assert.True(Roman.ContainsRomanNumeralWord("Chapter IV begins"));
            Assert.False(Roman.ContainsRomanNumeralWord("no numerals here"));
            Assert.False(Roman.ContainsRomanNumeralWord("   "));
        }
    }
}
