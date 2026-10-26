// Copyright © Anointed Automation Corporation., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Ported from Apache Lucene lucene/analysis/common/src/java/org/apache/lucene/analysis/miscellaneous/RemoveDuplicatesTokenFilter.java.
// Translated from Java to C#.

using System;
using System.Collections.Generic;

namespace AnointedAutomation.Search.Analysis
{
    /// <summary>Drops a token whose term already occurs at the same position (for example a word
    /// delimiter catenation equal to the preserved original after lowercasing).</summary>
    public sealed class DuplicateTermFilter : ITokenFilter
    {
        public List<Token> Apply(List<Token> tokens)
        {
            if (tokens is null)
            {
                throw new ArgumentNullException(nameof(tokens));
            }
            List<Token> output = new List<Token>(tokens.Count);
            HashSet<string> seen = new HashSet<string>(StringComparer.Ordinal);
            int position = -1;
            foreach (Token token in tokens)
            {
                if (token.Position != position)
                {
                    position = token.Position;
                    seen.Clear();
                }
                if (seen.Add(token.Term))
                {
                    output.Add(token);
                }
            }
            return output;
        }
    }
}
