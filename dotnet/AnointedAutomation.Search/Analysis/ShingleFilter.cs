// Copyright © Anointed Automation Corporation., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Ported from Apache Lucene lucene/analysis/common/src/java/org/apache/lucene/analysis/shingle/ShingleFilter.java.
// Translated from Java to C#: shingles are built over consecutive positions of the token list (the first
// token at each position), joined by the separator, spanning their positions. Filler tokens for position
// holes are not ported: a hole ends the shingle, as with Lucene's empty filler token.

using System;
using System.Collections.Generic;
using System.Text;

namespace AnointedAutomation.Search.Analysis
{
    /// <summary>Emits word n-grams ("shingles") such as "pot holder" from "pot", "holder".</summary>
    public sealed class ShingleFilter : ITokenFilter
    {
        /// <summary>Lucene's ShingleFilter.DEFAULT_TOKEN_SEPARATOR.</summary>
        public const string DefaultSeparator = " ";

        private readonly int _minShingleSize;
        private readonly int _maxShingleSize;
        private readonly bool _outputUnigrams;
        private readonly string _separator;

        public ShingleFilter(int minShingleSize, int maxShingleSize, bool outputUnigrams, string separator)
        {
            if (minShingleSize < 2)
            {
                throw new ArgumentOutOfRangeException(nameof(minShingleSize), "Minimum shingle size must be >= 2.");
            }
            if (maxShingleSize < minShingleSize)
            {
                throw new ArgumentOutOfRangeException(nameof(maxShingleSize), "Maximum shingle size must be >= minimum.");
            }
            if (separator is null)
            {
                throw new ArgumentNullException(nameof(separator));
            }
            _minShingleSize = minShingleSize;
            _maxShingleSize = maxShingleSize;
            _outputUnigrams = outputUnigrams;
            _separator = separator;
        }

        public List<Token> Apply(List<Token> tokens)
        {
            if (tokens is null)
            {
                throw new ArgumentNullException(nameof(tokens));
            }
            // The first token at each position is the shingle unit (Lucene shingles the main path).
            List<Token> units = new List<Token>();
            int lastPos = -1;
            foreach (Token t in tokens)
            {
                if (t.Position != lastPos)
                {
                    units.Add(t);
                    lastPos = t.Position;
                }
            }
            List<Token> output = new List<Token>(tokens.Count * 2);
            int next = 0;
            for (int u = 0; u < units.Count; u++)
            {
                Token head = units[u];
                if (_outputUnigrams)
                {
                    while (next < tokens.Count && tokens[next].Position == head.Position)
                    {
                        output.Add(tokens[next]);
                        next++;
                    }
                }
                StringBuilder sb = new StringBuilder(head.Term);
                int expected = head.Position + 1;
                for (int size = 2; size <= _maxShingleSize && u + size - 1 < units.Count; size++)
                {
                    Token tail = units[u + size - 1];
                    if (tail.Position != expected)
                    {
                        break;
                    }
                    sb.Append(_separator).Append(tail.Term);
                    expected++;
                    if (size >= _minShingleSize)
                    {
                        output.Add(new Token(sb.ToString(), head.Position, size, head.StartOffset, tail.EndOffset, TokenType.Shingle));
                    }
                }
            }
            return output;
        }
    }
}
