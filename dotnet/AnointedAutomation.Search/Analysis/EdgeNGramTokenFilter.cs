// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Ported from Apache Lucene lucene/analysis/common/src/java/org/apache/lucene/analysis/ngram/EdgeNGramTokenFilter.java
// and tantivy src/tokenizer/ngram_tokenizer.rs (prefix_only). Translated from Java and Rust to C#.

using System;
using System.Collections.Generic;

namespace AnointedAutomation.Search.Analysis
{
    /// <summary>Emits the leading n-grams of each term ("hoodie" to "hoo", "hood", "hoodi"), for prefix and
    /// as-you-type matching. Grams count UTF-16 code units, never splitting a surrogate pair.</summary>
    public sealed class EdgeNGramTokenFilter : ITokenFilter
    {
        private readonly int _minGram;
        private readonly int _maxGram;
        private readonly bool _preserveOriginal;

        public EdgeNGramTokenFilter(int minGram, int maxGram, bool preserveOriginal)
        {
            if (minGram < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(minGram), "minGram must be greater than zero.");
            }
            if (minGram > maxGram)
            {
                throw new ArgumentOutOfRangeException(nameof(maxGram), "minGram must not be greater than maxGram.");
            }
            _minGram = minGram;
            _maxGram = maxGram;
            _preserveOriginal = preserveOriginal;
        }

        public List<Token> Apply(List<Token> tokens)
        {
            if (tokens is null)
            {
                throw new ArgumentNullException(nameof(tokens));
            }
            List<Token> output = new List<Token>();
            foreach (Token token in tokens)
            {
                List<string> grams = Grams(token.Term, _minGram, _maxGram);
                foreach (string gram in grams)
                {
                    output.Add(new Token(gram, token.Position, token.PositionLength, token.StartOffset, token.EndOffset, TokenType.Gram));
                }
                // Lucene: the original is kept when it is shorter than minGram or longer than maxGram.
                bool emitted = token.Term.Length >= _minGram && token.Term.Length <= _maxGram;
                if (_preserveOriginal && !emitted)
                {
                    output.Add(token);
                }
            }
            return output;
        }

        /// <summary>The edge grams of <paramref name="term"/> of length minGram..maxGram, shortest first.</summary>
        public static List<string> Grams(string term, int minGram, int maxGram)
        {
            if (term is null)
            {
                throw new ArgumentNullException(nameof(term));
            }
            List<string> grams = new List<string>();
            int max = Math.Min(maxGram, term.Length);
            for (int n = minGram; n <= max; n++)
            {
                if (char.IsHighSurrogate(term[n - 1]) && n < term.Length)
                {
                    continue;
                }
                grams.Add(term.Substring(0, n));
            }
            return grams;
        }
    }
}
