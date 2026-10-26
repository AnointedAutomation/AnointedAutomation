// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Ported from Apache Lucene (CharTermAttribute, PositionIncrementAttribute, PositionLengthAttribute,
// OffsetAttribute, TypeAttribute in lucene/core/src/java/org/apache/lucene/analysis/tokenattributes/) and
// tantivy tokenizer-api/src/lib.rs (Token). Translated from Java and Rust to C#: the attribute set is one
// immutable class with an absolute position instead of a position increment.

using System;

namespace AnointedAutomation.Search.Analysis
{
    /// <summary>What produced a token. Zero is reserved.</summary>
    public enum TokenType
    {
        Unknown = 0,
        Word = 1,
        Number = 2,
        Alphanumeric = 3,
        Ideographic = 4,
        Catenation = 5,
        Shingle = 6,
        Synonym = 7,
        Gram = 8,
    }

    /// <summary>One token of an analyzed text. <see cref="Position"/> is absolute (0 based) and
    /// <see cref="PositionLength"/> is how many positions the token spans, so a token graph (word
    /// delimiter catenations, multi word synonyms) is a plain list.</summary>
    public sealed class Token
    {
        public Token(string term, int position, int positionLength, int startOffset, int endOffset, TokenType type)
        {
            if (term is null)
            {
                throw new ArgumentNullException(nameof(term));
            }
            if (position < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(position));
            }
            if (positionLength < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(positionLength));
            }
            Term = term;
            Position = position;
            PositionLength = positionLength;
            StartOffset = startOffset;
            EndOffset = endOffset;
            Type = type;
        }

        public string Term { get; }
        public int Position { get; }
        public int PositionLength { get; }
        public int StartOffset { get; }
        public int EndOffset { get; }
        public TokenType Type { get; }

        /// <summary>The same token with another term.</summary>
        public Token WithTerm(string term) => new Token(term, Position, PositionLength, StartOffset, EndOffset, Type);

        /// <summary>The same token at another position.</summary>
        public Token WithPosition(int position, int positionLength) =>
            new Token(Term, position, positionLength, StartOffset, EndOffset, Type);

        public override string ToString() => Term + "@" + Position + (PositionLength > 1 ? "+" + PositionLength : string.Empty);
    }
}
