// Copyright © Anointed Automation Corporation., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// The result side of the query API, after tantivy src/collector/top_score_collector.rs (scored doc
// addresses) and Apache Lucene TopDocs/ScoreDoc. Translated from Rust and Java concepts to C#.

using System.Collections.Generic;

namespace AnointedAutomation.Search.Query
{
    /// <summary>One ranked document (Lucene ScoreDoc).</summary>
    public sealed class SearchHit<TDocument>
    {
        public TDocument Document { get; init; }

        /// <summary>The document's position in the indexed list.</summary>
        public int DocId { get; init; }

        public decimal Score { get; init; }
    }

    /// <summary>A page of ranked documents (Lucene TopDocs).</summary>
    public sealed class SearchResult<TDocument>
    {
        public List<SearchHit<TDocument>> Hits { get; init; }

        /// <summary>All matching documents before limit and offset.</summary>
        public int TotalHits { get; init; }
    }
}
