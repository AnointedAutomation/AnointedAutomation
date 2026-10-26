// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Ported from Apache Lucene lucene/analysis/common/src/java/org/apache/lucene/analysis/en/EnglishMinimalStemmer.java
// and EnglishMinimalStemFilter.java (Harman's S-stemmer). Translated from Java to C#.

using System;
using System.Collections.Generic;

namespace AnointedAutomation.Search.Analysis
{
    /// <summary>Plural stripping: "ies" to "y" (not after a or e), "es" kept after a vowel other than i, a final
    /// "s" dropped unless it follows u or s.</summary>
    public static class EnglishMinimalStemmer
    {
        public static string Stem(string word)
        {
            if (word is null)
            {
                throw new ArgumentNullException(nameof(word));
            }
            int len = word.Length;
            if (len < 3 || word[len - 1] != 's')
            {
                return word;
            }
            switch (word[len - 2])
            {
                case 'u':
                case 's':
                    return word;
                case 'e':
                    if (len > 3 && word[len - 3] == 'i' && word[len - 4] != 'a' && word[len - 4] != 'e')
                    {
                        return string.Concat(word.AsSpan(0, len - 3), "y");
                    }
                    if (word[len - 3] == 'i' || word[len - 3] == 'a' || word[len - 3] == 'o' || word[len - 3] == 'e')
                    {
                        return word;
                    }
                    return word.Substring(0, len - 1);
                default:
                    return word.Substring(0, len - 1);
            }
        }
    }

    /// <summary>Applies <see cref="EnglishMinimalStemmer"/> to every token.</summary>
    public sealed class EnglishMinimalStemFilter : ITokenFilter
    {
        public List<Token> Apply(List<Token> tokens)
        {
            if (tokens is null)
            {
                throw new ArgumentNullException(nameof(tokens));
            }
            List<Token> output = new List<Token>(tokens.Count);
            foreach (Token token in tokens)
            {
                string stem = EnglishMinimalStemmer.Stem(token.Term);
                output.Add(string.Equals(stem, token.Term, StringComparison.Ordinal) ? token : token.WithTerm(stem));
            }
            return output;
        }
    }
}
