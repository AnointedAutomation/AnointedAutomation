// Copyright © Anointed Automation Corporation., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Ported from Apache Lucene lucene/core/src/java/org/apache/lucene/search/ (BooleanClause.Occur, TermQuery,
// PhraseQuery, FuzzyQuery) and tantivy src/query/ (Occur, TermQuery, PhraseQuery, FuzzyTermQuery).
// Translated from Java and Rust to C# as a small typed query model.

using System;
using System.Collections.Generic;

namespace AnointedAutomation.Search.Query
{
    /// <summary>How a clause takes part in matching. Zero is reserved.</summary>
    public enum Occur
    {
        Unknown = 0,
        /// <summary>Required; scores.</summary>
        Must = 1,
        /// <summary>Optional; scores when it matches. With no Must clause at least one Should must match.</summary>
        Should = 2,
        /// <summary>Excludes matching documents.</summary>
        MustNot = 3,
    }

    /// <summary>A typed query clause.</summary>
    public abstract class QueryClause
    {
        protected QueryClause(Occur occur, IReadOnlyList<string> fields)
        {
            if (occur == Occur.Unknown)
            {
                throw new ArgumentException("Occur is required.", nameof(occur));
            }
            Occur = occur;
            Fields = fields;
        }

        public Occur Occur { get; }

        /// <summary>Text fields to search; null searches every text field.</summary>
        public IReadOnlyList<string> Fields { get; }
    }

    /// <summary>Every analyzed word of <see cref="Text"/> must match (a BooleanQuery of MUST TermQuery).</summary>
    public sealed class TermClause : QueryClause
    {
        public TermClause(string text, Occur occur, IReadOnlyList<string> fields)
            : base(occur, fields)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException("Text is required.", nameof(text));
            }
            Text = text;
        }

        public string Text { get; }
    }

    /// <summary>The analyzed words of <see cref="Text"/> in order, each within <see cref="Slop"/> extra
    /// positions of the previous, in one positions enabled field.</summary>
    public sealed class PhraseClause : QueryClause
    {
        public PhraseClause(string text, int slop, Occur occur, IReadOnlyList<string> fields)
            : base(occur, fields)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException("Text is required.", nameof(text));
            }
            if (slop < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(slop));
            }
            Text = text;
            Slop = slop;
        }

        public string Text { get; }
        public int Slop { get; }
    }

    /// <summary>Terms within <see cref="MaxEdits"/> of <see cref="Term"/> (Levenshtein automaton).</summary>
    public sealed class FuzzyClause : QueryClause
    {
        public FuzzyClause(string term, int maxEdits, bool transpositions, int prefixLength, Occur occur, IReadOnlyList<string> fields)
            : base(occur, fields)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                throw new ArgumentException("Term is required.", nameof(term));
            }
            if (maxEdits < 0 || maxEdits > Fuzzy.LevenshteinAutomatonBuilder.MaximumSupportedDistance)
            {
                throw new ArgumentOutOfRangeException(nameof(maxEdits));
            }
            if (prefixLength < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(prefixLength));
            }
            Term = term;
            MaxEdits = maxEdits;
            Transpositions = transpositions;
            PrefixLength = prefixLength;
        }

        public string Term { get; }
        public int MaxEdits { get; }
        public bool Transpositions { get; }
        public int PrefixLength { get; }
    }
}
