// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Ported from tantivy src/query/bm25.rs (idf, cached_tf_component, Bm25Weight::tf_factor/score) and
// Apache Lucene lucene/core/src/java/org/apache/lucene/search/similarities/BM25Similarity.java (k1 1.2,
// b 0.75, per-field configurable). Translated from Rust and Java to C#, in decimal: the per document
// length component is precomputed from exact field lengths (tantivy precomputes it for 256 quantized
// lengths instead).

using System;

namespace AnointedAutomation.Search.Scoring
{
    /// <summary>Okapi BM25.</summary>
    public static class Bm25
    {
        /// <summary>Lucene and tantivy default term frequency saturation.</summary>
        public const decimal DefaultK1 = 1.2m;

        /// <summary>Lucene and tantivy default length normalization.</summary>
        public const decimal DefaultB = 0.75m;

        /// <summary>ln(1 + (N - df + 0.5) / (df + 0.5)) (tantivy bm25.rs idf).</summary>
        public static decimal Idf(long docFreq, long docCount)
        {
            if (docFreq < 0 || docCount < docFreq)
            {
                throw new ArgumentOutOfRangeException(nameof(docFreq), "Need 0 <= docFreq <= docCount.");
            }
            decimal x = (docCount - docFreq + 0.5m) / (docFreq + 0.5m);
            // .NET has no decimal logarithm: one conversion to double and straight back.
            return (decimal)Math.Log((double)(1m + x));
        }

        /// <summary>k1 x (1 - b + b x fieldLength / averageFieldLength) (tantivy cached_tf_component).</summary>
        public static decimal LengthNorm(decimal k1, decimal b, int fieldLength, decimal averageFieldLength)
        {
            if (averageFieldLength <= 0m)
            {
                throw new ArgumentOutOfRangeException(nameof(averageFieldLength));
            }
            return k1 * (1m - b + b * fieldLength / averageFieldLength);
        }

        /// <summary>tf x (k1 + 1) / (tf + norm): multiply by idf and boost for the term weight.</summary>
        public static decimal TfFactor(decimal k1, int termFreq, decimal lengthNorm)
        {
            if (termFreq <= 0)
            {
                return 0m;
            }
            return termFreq * (k1 + 1m) / (termFreq + lengthNorm);
        }

        /// <summary>Full BM25 score of one term in one field of one document.</summary>
        public static decimal Score(decimal idf, int termFreq, int fieldLength, decimal averageFieldLength, decimal k1, decimal b)
        {
            return idf * TfFactor(k1, termFreq, LengthNorm(k1, b, fieldLength, averageFieldLength));
        }
    }
}
