// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// The request side of the query API. Shaped after Apache Lucene IndexSearcher.search(Query, n, Sort) with a
// BooleanQuery of clauses, and tantivy's TopDocs collector (limit, offset, order by fast field). Translated
// from Java and Rust concepts to C#.

using System;
using System.Collections.Generic;

namespace AnointedAutomation.Search.Query
{
    /// <summary>One search: a boolean combination of <see cref="Clauses"/>, optional fast field filters and
    /// sorts. <see cref="Limit"/> is required.</summary>
    public sealed class SearchRequest<TDocument>
    {
        public SearchRequest(int limit, int offset)
        {
            if (limit < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(limit), "Limit must be at least 1.");
            }
            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }
            Limit = limit;
            Offset = offset;
        }

        public int Limit { get; }

        public int Offset { get; }

        /// <summary>The clauses of the boolean query (Lucene BooleanClause list).</summary>
        public List<QueryClause> Clauses { get; } = new List<QueryClause>();

        /// <summary>Fast field filters (all must pass; Lucene FILTER clauses).</summary>
        public List<FastFieldFilter> Filters { get; } = new List<FastFieldFilter>();

        /// <summary>When non empty, results order by these fields, then score, instead of score (Lucene Sort).</summary>
        public List<SortField> Sort { get; } = new List<SortField>();
    }
}
