// Copyright © Anointed Automation Corporation., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Ported from Apache Lucene lucene/core/src/java/org/apache/lucene/analysis/Tokenizer.java and tantivy
// tokenizer-api/src/lib.rs (Tokenizer trait). Translated from Java and Rust to C#.

using System.Collections.Generic;

namespace AnointedAutomation.Search.Analysis
{
    /// <summary>Splits raw text into tokens.</summary>
    public interface ITokenizer
    {
        /// <summary>The tokens of <paramref name="text"/>, positions ascending from 0.</summary>
        List<Token> Tokenize(string text);
    }
}
