// Copyright © Anointed Automation Corporation., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Ported from tantivy src/schema/text_options.rs (TextFieldIndexing: index record option, positions) and
// Apache Lucene lucene/core/src/java/org/apache/lucene/document/FieldType.java with per-field BM25Similarity
// parameters. Translated from Rust and Java to C#.

using System;

namespace AnointedAutomation.Search.Index
{
    /// <summary>How a text field is indexed and scored. Every property is required.</summary>
    public sealed class TextFieldOptions
    {
        /// <summary>Multiplier on this field's BM25 score (Lucene BoostQuery).</summary>
        public required decimal Boost { get; init; }

        /// <summary>Keep word positions (phrase queries read them).</summary>
        public required bool Positions { get; init; }

        /// <summary>BM25 term frequency saturation (Lucene/tantivy default 1.2).</summary>
        public required decimal K1 { get; init; }

        /// <summary>BM25 length normalization, 0 to 1 (Lucene/tantivy default 0.75).</summary>
        public required decimal B { get; init; }

        internal void Validate(string field)
        {
            if (Boost <= 0m)
            {
                throw new ArgumentOutOfRangeException(nameof(Boost), "Field " + field + ": Boost must be positive.");
            }
            if (K1 <= 0m)
            {
                throw new ArgumentOutOfRangeException(nameof(K1), "Field " + field + ": K1 must be positive.");
            }
            if (B < 0m || B > 1m)
            {
                throw new ArgumentOutOfRangeException(nameof(B), "Field " + field + ": B must be between 0 and 1.");
            }
        }
    }
}
