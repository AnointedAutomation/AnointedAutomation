// Copyright © Anointed Automation Corporation., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Ported from Apache Lucene lucene/core/src/java/org/apache/lucene/analysis/TokenFilter.java and tantivy
// tokenizer-api/src/lib.rs (TokenFilter trait). Translated from Java and Rust to C#: a filter maps a whole
// token list instead of pulling one token at a time.

using System.Collections.Generic;

namespace AnointedAutomation.Search.Analysis
{
    /// <summary>Transforms a token list (sorted by position, then by start offset).</summary>
    public interface ITokenFilter
    {
        List<Token> Apply(List<Token> tokens);
    }
}
