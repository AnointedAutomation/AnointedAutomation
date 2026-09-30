// Copyright 2024 Anointed Automation, LLC All rights reserved.
// Originally created by Alexander Fields. Ported and modernized for the AnointedAutomation monorepo.

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AnointedAutomation.Optimization
{
    /// <summary>Roman numeral conversion helpers.</summary>
    public static class Roman
    {
        private static readonly (int Value, string Symbol)[] RomanNumerals = new[]
        {
            (1000, "M"), (900, "CM"), (500, "D"), (400, "CD"),
            (100, "C"), (90, "XC"), (50, "L"), (40, "XL"),
            (10, "X"), (9, "IX"), (5, "V"), (4, "IV"),
            (1, "I"),
        };

        /// <summary>
        /// Checks whether the input contains at least one whole word that is a valid Roman numeral.
        /// </summary>
        public static bool ContainsRomanNumeralWord(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return false;
            }

            const string romanChars = "IVXLCDM";

            IEnumerable<string> words = Regex.Split(input.ToUpperInvariant(), @"[^A-Z]+")
                .Where(w => !string.IsNullOrEmpty(w));

            foreach (string word in words)
            {
                if (word.All(c => romanChars.Contains(c)) && RomanToInteger(word) > 0)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>Converts an integer to a Roman numeral string, supporting large numbers via overlines.</summary>
        public static string IntegerToRoman(int num)
        {
            if (num == 0)
            {
                return "nulla";
            }

            if (num < 0)
            {
                return "Negative numbers are invalid";
            }

            StringBuilder result = new StringBuilder();
            int multiplier = 0;

            while (num > 0)
            {
                int part = num % 1000;
                if (part > 0)
                {
                    result.Insert(0, ConvertToRoman(part) + GetOverline(multiplier));
                }
                num /= 1000;
                multiplier++;
            }

            return result.ToString();
        }

        /// <summary>Converts a Roman numeral string to an integer.</summary>
        public static int RomanToInteger(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return 0;
            }

            Dictionary<char, int> romanValues = new Dictionary<char, int>
            {
                { 'I', 1 }, { 'V', 5 }, { 'X', 10 }, { 'L', 50 },
                { 'C', 100 }, { 'D', 500 }, { 'M', 1000 },
                { '̅', 1000 },
            };

            int total = 0;
            int prevValue = 0;

            for (int i = s.Length - 1; i >= 0; i--)
            {
                if (romanValues.TryGetValue(s[i], out int currentValue))
                {
                    if (currentValue < prevValue)
                    {
                        total -= currentValue;
                    }
                    else
                    {
                        total += currentValue;
                    }

                    prevValue = currentValue;
                }
            }

            return total;
        }

        private static string ConvertToRoman(int num)
        {
            StringBuilder result = new StringBuilder();
            foreach ((int value, string symbol) in RomanNumerals)
            {
                while (num >= value)
                {
                    result.Append(symbol);
                    num -= value;
                }
            }
            return result.ToString();
        }

        private static string GetOverline(int multiplier)
        {
            if (multiplier == 0)
            {
                return string.Empty;
            }

            StringBuilder overline = new StringBuilder();
            for (int i = 0; i < multiplier; i++)
            {
                overline.Append('̅');
            }
            return overline.ToString();
        }
    }
}
