// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Ported from tantivy src/postings/ (postings_writer.rs, recorder.rs: per term doc, term frequency and
// position recording; segment_postings.rs) and src/fieldnorm/ (per document field lengths), and Apache
// Lucene lucene/core/src/java/org/apache/lucene/index/ (FreqProxTermsWriterPerField, per field docFreq
// statistics, Terms). Translated from Rust and Java to C#: one immutable in-memory segment of plain arrays,
// no codecs.

using System;
using System.Collections.Generic;
using AnointedAutomation.Search.Analysis;
using AnointedAutomation.Search.Scoring;

namespace AnointedAutomation.Search.Index
{
    /// <summary>One document's occurrences of one term in one field.</summary>
    public readonly struct Posting
    {
        public Posting(int docId, int termFreq, int[] positions)
        {
            DocId = docId;
            TermFreq = termFreq;
            Positions = positions;
        }

        public int DocId { get; }
        public int TermFreq { get; }

        /// <summary>Sorted positions; empty for fields that do not keep them.</summary>
        public int[] Positions { get; }
    }

    /// <summary>An immutable in-memory inverted index over a fixed list of text fields. Terms are per field
    /// (Lucene's field + term), with per field document frequencies, lengths and a sorted term dictionary.</summary>
    public sealed class InvertedIndex
    {
        private static readonly Posting[] NoPostings = Array.Empty<Posting>();

        private sealed class FieldIndex
        {
            public Dictionary<string, Posting[]> Postings;
            public TermDictionary Terms;
            public int[] Lengths;
            public decimal AverageLength;
            public decimal[] Norm;
        }

        private readonly FieldIndex[] _fields;

        internal InvertedIndex(IReadOnlyList<TextFieldOptions> fields, IReadOnlyList<string> names, IReadOnlyList<List<string>[]> documents, TextAnalyzer analyzer)
        {
            Fields = fields;
            FieldNames = names;
            DocCount = documents.Count;
            _fields = new FieldIndex[fields.Count];
            for (int f = 0; f < fields.Count; f++)
            {
                Dictionary<string, List<Posting>> building = new Dictionary<string, List<Posting>>(StringComparer.Ordinal);
                int[] lengths = new int[documents.Count];
                for (int d = 0; d < documents.Count; d++)
                {
                    Dictionary<string, List<int>> occurrences = new Dictionary<string, List<int>>(StringComparer.Ordinal);
                    int basePosition = 0;
                    int length = 0;
                    foreach (string value in documents[d][f])
                    {
                        List<Token> tokens = analyzer.Analyze(value);
                        if (tokens.Count == 0)
                        {
                            continue;
                        }
                        int end = 0;
                        foreach (Token token in tokens)
                        {
                            int position = basePosition + token.Position;
                            end = Math.Max(end, token.Position + token.PositionLength);
                            List<int> list;
                            if (!occurrences.TryGetValue(token.Term, out list))
                            {
                                list = new List<int>();
                                occurrences[token.Term] = list;
                            }
                            // Several tokens at one position (a token graph) are one occurrence.
                            if (!list.Contains(position))
                            {
                                list.Add(position);
                            }
                        }
                        length += end;
                        basePosition += end + SearchSchema<object>.MultiValuePositionGap;
                    }
                    lengths[d] = length;
                    foreach (KeyValuePair<string, List<int>> pair in occurrences)
                    {
                        pair.Value.Sort();
                        List<Posting> postings;
                        if (!building.TryGetValue(pair.Key, out postings))
                        {
                            postings = new List<Posting>();
                            building[pair.Key] = postings;
                        }
                        postings.Add(new Posting(d, pair.Value.Count, fields[f].Positions ? pair.Value.ToArray() : Array.Empty<int>()));
                    }
                }
                Dictionary<string, Posting[]> frozen = new Dictionary<string, Posting[]>(building.Count, StringComparer.Ordinal);
                foreach (KeyValuePair<string, List<Posting>> pair in building)
                {
                    frozen[pair.Key] = pair.Value.ToArray();
                }
                long total = 0;
                foreach (int len in lengths)
                {
                    total += len;
                }
                decimal average = documents.Count == 0 || total == 0 ? 1m : (decimal)total / documents.Count;
                decimal[] norm = new decimal[documents.Count];
                for (int d = 0; d < documents.Count; d++)
                {
                    norm[d] = Bm25.LengthNorm(fields[f].K1, fields[f].B, lengths[d], average);
                }
                _fields[f] = new FieldIndex
                {
                    Postings = frozen,
                    Terms = new TermDictionary(frozen.Keys),
                    Lengths = lengths,
                    AverageLength = average,
                    Norm = norm,
                };
            }
        }

        public IReadOnlyList<TextFieldOptions> Fields { get; }

        public IReadOnlyList<string> FieldNames { get; }

        public int DocCount { get; }

        /// <summary>The sorted terms of a field (fuzzy and prefix queries enumerate it).</summary>
        public TermDictionary Terms(int field) => _fields[field].Terms;

        /// <summary>Documents containing the term in the field (Lucene TermsEnum.docFreq).</summary>
        public int DocFrequency(int field, string term) => Postings(field, term).Length;

        /// <summary>The postings of a term in one field, sorted by doc id (empty when absent).</summary>
        public Posting[] Postings(int field, string term)
        {
            Posting[] list;
            return term is not null && _fields[field].Postings.TryGetValue(term, out list) ? list : NoPostings;
        }

        /// <summary>One document's posting (binary search), or null.</summary>
        public Posting? PostingFor(int field, string term, int docId)
        {
            Posting[] list = Postings(field, term);
            int lo = 0;
            int hi = list.Length - 1;
            while (lo <= hi)
            {
                int mid = (lo + hi) >> 1;
                int id = list[mid].DocId;
                if (id == docId) { return list[mid]; }
                if (id < docId) { lo = mid + 1; } else { hi = mid - 1; }
            }
            return null;
        }

        public int FieldLength(int field, int docId) => _fields[field].Lengths[docId];

        public decimal AverageFieldLength(int field) => _fields[field].AverageLength;

        /// <summary>BM25 tf factor of a document from the precomputed length norm.</summary>
        public decimal TfWeight(int field, int docId, int tf) => Bm25.TfFactor(Fields[field].K1, tf, _fields[field].Norm[docId]);

        /// <summary>BM25 idf of a document frequency over this index.</summary>
        public decimal Idf(int df) => Bm25.Idf(df, DocCount);
    }
}
