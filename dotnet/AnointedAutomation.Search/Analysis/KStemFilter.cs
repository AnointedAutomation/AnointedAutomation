// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Ported from Apache Lucene lucene/analysis/common/src/java/org/apache/lucene/analysis/en/KStemFilter.java.
// Translated from Java to C#.

using System;
using System.Collections.Generic;

namespace AnointedAutomation.Search.Analysis
{
    /// <summary>Stems each lowercase term with the Krovetz stemmer (KStem). Terms with anything but a-z,
    /// shorter than 3 or 50 and longer are left alone, as in Lucene.</summary>
    public sealed class KStemFilter : ITokenFilter
    {
        public List<Token> Apply(List<Token> tokens)
        {
            if (tokens is null)
            {
                throw new ArgumentNullException(nameof(tokens));
            }
            KStemmer stemmer = new KStemmer();
            List<Token> output = new List<Token>(tokens.Count);
            foreach (Token token in tokens)
            {
                string stem = stemmer.Stem(token.Term);
                output.Add(string.Equals(stem, token.Term, StringComparison.Ordinal) ? token : token.WithTerm(stem));
            }
            return output;
        }
    }
}
