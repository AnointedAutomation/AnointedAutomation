// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Ported from Apache Lucene lucene/core/src/java/org/apache/lucene/analysis/LowerCaseFilter.java and tantivy
// src/tokenizer/lower_caser.rs. Translated from Java and Rust to C#.

using System;
using System.Collections.Generic;

namespace AnointedAutomation.Search.Analysis
{
    /// <summary>Lowercases every term (invariant culture, like Lucene's Character.toLowerCase).</summary>
    public sealed class LowerCaseFilter : ITokenFilter
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
                string lower = token.Term.ToLowerInvariant();
                output.Add(string.Equals(lower, token.Term, StringComparison.Ordinal) ? token : token.WithTerm(lower));
            }
            return output;
        }
    }
}
