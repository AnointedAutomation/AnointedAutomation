// Copyright © Anointed Automation Corporation., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Ported from Apache Lucene lucene/core/src/java/org/apache/lucene/analysis/Analyzer.java and
// CustomAnalyzer, and tantivy src/tokenizer/tokenizer.rs (TextAnalyzer and its builder). Translated from Java
// and Rust to C#.

using System;
using System.Collections.Generic;

namespace AnointedAutomation.Search.Analysis
{
    /// <summary>A tokenizer followed by a chain of filters. Immutable and thread safe when its parts are.</summary>
    public sealed class TextAnalyzer
    {
        private readonly ITokenizer _tokenizer;
        private readonly ITokenFilter[] _filters;

        public TextAnalyzer(ITokenizer tokenizer, IReadOnlyList<ITokenFilter> filters)
        {
            if (tokenizer is null)
            {
                throw new ArgumentNullException(nameof(tokenizer));
            }
            if (filters is null)
            {
                throw new ArgumentNullException(nameof(filters));
            }
            _tokenizer = tokenizer;
            _filters = new ITokenFilter[filters.Count];
            for (int i = 0; i < filters.Count; i++)
            {
                if (filters[i] is null)
                {
                    throw new ArgumentException("Filter " + i + " is null.", nameof(filters));
                }
                _filters[i] = filters[i];
            }
        }

        /// <summary>The analyzed tokens of <paramref name="text"/>; empty for null or blank text.</summary>
        public List<Token> Analyze(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return new List<Token>();
            }
            List<Token> tokens = _tokenizer.Tokenize(text);
            for (int i = 0; i < _filters.Length; i++)
            {
                tokens = _filters[i].Apply(tokens);
            }
            return tokens;
        }
    }
}
