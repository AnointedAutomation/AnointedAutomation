// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Ported from tantivy src/query/range_query/ (fast field range filtering) and Apache Lucene
// lucene/core/src/java/org/apache/lucene/document/LongPoint.java newRangeQuery / TermInSetQuery semantics.
// Translated from Rust and Java to C#.

using System;
using System.Collections.Generic;
using AnointedAutomation.Search.Index;

namespace AnointedAutomation.Search.Query
{
    /// <summary>A filter over one fast field. Documents without a value fail range and AnyKeyword filters and pass NoKeyword.</summary>
    public sealed class FastFieldFilter
    {
        private FastFieldFilter(string field, decimal? min, decimal? max, IReadOnlyList<string> keywords, bool exclude)
        {
            if (string.IsNullOrWhiteSpace(field))
            {
                throw new ArgumentException("Field is required.", nameof(field));
            }
            Field = field;
            Min = min;
            Max = max;
            Keywords = keywords;
            Exclude = exclude;
        }

        public string Field { get; }

        /// <summary>Inclusive lower bound; null for an open range.</summary>
        public decimal? Min { get; }

        /// <summary>Inclusive upper bound; null for an open range.</summary>
        public decimal? Max { get; }

        /// <summary>For keyword filters: the accepted keywords (any of).</summary>
        public IReadOnlyList<string> Keywords { get; }

        /// <summary>For keyword filters: reject documents having any of the keywords instead.</summary>
        public bool Exclude { get; }

        /// <summary>min &lt;= value &lt;= max on an integer or decimal field; at least one bound required.</summary>
        public static FastFieldFilter Range(string field, decimal? min, decimal? max)
        {
            if (!min.HasValue && !max.HasValue)
            {
                throw new ArgumentException("A range needs at least one bound.");
            }
            if (min.HasValue && max.HasValue && min.Value > max.Value)
            {
                throw new ArgumentException("Range minimum exceeds maximum.");
            }
            return new FastFieldFilter(field, min, max, null, false);
        }

        public static FastFieldFilter Equal(string field, decimal value) => new FastFieldFilter(field, value, value, null, false);

        /// <summary>Documents having any of <paramref name="keywords"/> in a keyword field.</summary>
        public static FastFieldFilter AnyKeyword(string field, IReadOnlyList<string> keywords)
        {
            if (keywords is null || keywords.Count == 0)
            {
                throw new ArgumentException("At least one keyword is required.", nameof(keywords));
            }
            return new FastFieldFilter(field, null, null, keywords, false);
        }

        /// <summary>Documents having none of <paramref name="keywords"/> in a keyword field.</summary>
        public static FastFieldFilter NoKeyword(string field, IReadOnlyList<string> keywords)
        {
            if (keywords is null || keywords.Count == 0)
            {
                throw new ArgumentException("At least one keyword is required.", nameof(keywords));
            }
            return new FastFieldFilter(field, null, null, keywords, true);
        }

        internal Func<int, bool> Compile(FastFieldColumn column)
        {
            if (Keywords is not null)
            {
                if (column.Kind != FastFieldKind.Keyword)
                {
                    throw new InvalidOperationException("Keyword filter on non keyword field '" + Field + "'.");
                }
                List<int> ords = new List<int>();
                foreach (string k in Keywords)
                {
                    int ord = column.Ordinal(k);
                    if (ord >= 0)
                    {
                        ords.Add(ord);
                    }
                }
                int[] wanted = ords.ToArray();
                bool exclude = Exclude;
                return doc =>
                {
                    bool any = false;
                    for (int i = 0; i < wanted.Length && !any; i++)
                    {
                        any = column.HasOrdinal(doc, wanted[i]);
                    }
                    return exclude ? !any : any;
                };
            }
            if (column.Kind != FastFieldKind.Integer && column.Kind != FastFieldKind.Decimal)
            {
                throw new InvalidOperationException("Range filter on non numeric field '" + Field + "'.");
            }
            decimal? min = Min;
            decimal? max = Max;
            return doc =>
            {
                if (!column.HasValue(doc))
                {
                    return false;
                }
                decimal v = column.Numeric(doc);
                return (!min.HasValue || v >= min.Value) && (!max.HasValue || v <= max.Value);
            };
        }
    }

    /// <summary>Sort by a fast field. Documents without a value sort last either way.</summary>
    public sealed class SortField
    {
        public SortField(string field, bool descending)
        {
            if (string.IsNullOrWhiteSpace(field))
            {
                throw new ArgumentException("Field is required.", nameof(field));
            }
            Field = field;
            Descending = descending;
        }

        public string Field { get; }
        public bool Descending { get; }
    }
}
