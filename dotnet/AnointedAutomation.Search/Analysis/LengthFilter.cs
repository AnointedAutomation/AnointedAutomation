// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Ported from Apache Lucene lucene/analysis/common/src/java/org/apache/lucene/analysis/miscellaneous/LengthFilter.java
// and tantivy src/tokenizer/remove_long.rs. Translated from Java and Rust to C#.

using System;
using System.Collections.Generic;

namespace AnointedAutomation.Search.Analysis
{
    /// <summary>Keeps terms whose length is in [min, max], leaving a position hole for the rest.</summary>
    public sealed class LengthFilter : ITokenFilter
    {
        private readonly int _min;
        private readonly int _max;

        public LengthFilter(int min, int max)
        {
            if (min < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(min));
            }
            if (max < min)
            {
                throw new ArgumentOutOfRangeException(nameof(max));
            }
            _min = min;
            _max = max;
        }

        public List<Token> Apply(List<Token> tokens)
        {
            if (tokens is null)
            {
                throw new ArgumentNullException(nameof(tokens));
            }
            List<Token> output = new List<Token>(tokens.Count);
            foreach (Token token in tokens)
            {
                if (token.Term.Length >= _min && token.Term.Length <= _max)
                {
                    output.Add(token);
                }
            }
            return output;
        }
    }
}
