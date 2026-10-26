// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Ported from tantivy src/reader/mod.rs (IndexReader::reload swapping the searcher pool atomically) and
// src/collector/top_score_collector.rs (TopDocs with limit, offset and order by fast field), with Apache
// Lucene lucene/core/src/java/org/apache/lucene/search/SearcherManager.java (acquire a searcher, swap on
// refresh). Query execution follows Lucene's BooleanQuery (MUST, SHOULD, MUST_NOT, FILTER), TermQuery with
// BM25Similarity, DisjunctionMaxQuery across fields (tie breaker 0), PhraseQuery with slop
// (SloppyPhraseMatcher) and FuzzyQuery (FuzzyTermsEnum boost 1 - edits / shorter length, best expansion
// kept), and tantivy's equivalents in src/query/. Translated from Java and Rust to C#.

using System;
using System.Collections.Generic;
using System.Threading;
using AnointedAutomation.Search.Analysis;
using AnointedAutomation.Search.Fuzzy;
using AnointedAutomation.Search.Index;
using AnointedAutomation.Search.Query;
using AnointedAutomation.Search.Scoring;

namespace AnointedAutomation.Search
{
    /// <summary>An in-memory full-text index over caller documents with an atomically swapped snapshot.</summary>
    public sealed class SearchIndex<TDocument>
    {
        private readonly SearchSchema<TDocument> _schema;
        private readonly TextFieldDefinition<TDocument>[] _textFields;
        private readonly FastFieldDefinition<TDocument>[] _fastFields;
        private readonly object _rebuildLock = new object();
        private IndexSnapshot<TDocument> _current;

        /// <param name="schema">Field layout; copied, later changes to it are ignored.</param>
        public SearchIndex(SearchSchema<TDocument> schema)
        {
            if (schema is null)
            {
                throw new ArgumentNullException(nameof(schema));
            }
            if (schema.TextFields.Count == 0)
            {
                throw new ArgumentException("The schema needs at least one text field.", nameof(schema));
            }
            _schema = schema;
            _textFields = new List<TextFieldDefinition<TDocument>>(schema.TextFields).ToArray();
            _fastFields = new List<FastFieldDefinition<TDocument>>(schema.FastFields).ToArray();
        }

        /// <summary>The current snapshot; throws until the first <see cref="Rebuild"/>.</summary>
        public IndexSnapshot<TDocument> Snapshot => Current();

        /// <summary>True once a snapshot has been published.</summary>
        public bool IsReady => Volatile.Read(ref _current) is not null;

        /// <summary>Indexes <paramref name="documents"/> into a new snapshot and publishes it atomically.
        /// Rebuilds are serialized; searches never block.</summary>
        public IndexSnapshot<TDocument> Rebuild(IReadOnlyList<TDocument> documents)
        {
            if (documents is null)
            {
                throw new ArgumentNullException(nameof(documents));
            }
            lock (_rebuildLock)
            {
                IndexSnapshot<TDocument> previous = Volatile.Read(ref _current);
                long generation = previous is null ? 1 : previous.Generation + 1;
                IndexSnapshot<TDocument> snapshot = IndexSnapshot<TDocument>.Build(_schema, _textFields, _fastFields, documents, generation);
                Interlocked.Exchange(ref _current, snapshot);
                return snapshot;
            }
        }

        /// <summary>Runs a request against the current snapshot.</summary>
        public SearchResult<TDocument> Search(SearchRequest<TDocument> request)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            IndexSnapshot<TDocument> snapshot = Current();
            InvertedIndex index = snapshot.Index;
            int n = snapshot.Documents.Count;
            bool[] allowed = BuildAllowed(snapshot, request);

            List<Dictionary<int, decimal>> must = new List<Dictionary<int, decimal>>();
            List<Dictionary<int, decimal>> should = new List<Dictionary<int, decimal>>();
            HashSet<int> excluded = new HashSet<int>();
            foreach (QueryClause clause in request.Clauses)
            {
                if (clause is null)
                {
                    throw new ArgumentException("A clause is null.", nameof(request));
                }
                Dictionary<int, decimal> matched = ScoreClause(index, clause, FieldMask(clause.Fields));
                if (clause.Occur == Occur.Must) { must.Add(matched); }
                else if (clause.Occur == Occur.Should) { should.Add(matched); }
                else if (clause.Occur == Occur.MustNot) { excluded.UnionWith(matched.Keys); }
            }

            // BooleanQuery: every MUST matches; SHOULD scores where it matches and, with no MUST, one is
            // required; MUST_NOT and FILTER exclude. No scoring clause at all matches every document (score 0).
            Dictionary<int, decimal> scores = new Dictionary<int, decimal>();
            if (must.Count > 0)
            {
                Dictionary<int, decimal> smallest = must[0];
                foreach (Dictionary<int, decimal> m in must)
                {
                    if (m.Count < smallest.Count) { smallest = m; }
                }
                foreach (int doc in smallest.Keys)
                {
                    decimal sum = 0m;
                    bool all = true;
                    foreach (Dictionary<int, decimal> m in must)
                    {
                        decimal s;
                        if (!m.TryGetValue(doc, out s)) { all = false; break; }
                        sum += s;
                    }
                    if (all) { scores[doc] = sum; }
                }
                foreach (Dictionary<int, decimal> m in should)
                {
                    foreach (KeyValuePair<int, decimal> pair in m)
                    {
                        decimal s;
                        if (scores.TryGetValue(pair.Key, out s)) { scores[pair.Key] = s + pair.Value; }
                    }
                }
            }
            else if (should.Count > 0)
            {
                foreach (Dictionary<int, decimal> m in should)
                {
                    foreach (KeyValuePair<int, decimal> pair in m)
                    {
                        decimal s;
                        scores.TryGetValue(pair.Key, out s);
                        scores[pair.Key] = s + pair.Value;
                    }
                }
            }
            else
            {
                for (int doc = 0; doc < n; doc++)
                {
                    scores[doc] = 0m;
                }
            }

            Comparison<KeyValuePair<int, decimal>> order = BuildOrder(snapshot, request);
            TopK<KeyValuePair<int, decimal>> top = new TopK<KeyValuePair<int, decimal>>(request.Limit + request.Offset, order);
            int total = 0;
            foreach (KeyValuePair<int, decimal> pair in scores)
            {
                if ((allowed is not null && !allowed[pair.Key]) || excluded.Contains(pair.Key))
                {
                    continue;
                }
                total++;
                top.Add(pair);
            }
            List<KeyValuePair<int, decimal>> sorted = top.ToSortedList();
            List<SearchHit<TDocument>> page = new List<SearchHit<TDocument>>();
            for (int i = request.Offset; i < sorted.Count; i++)
            {
                page.Add(new SearchHit<TDocument> { Document = snapshot.Documents[sorted[i].Key], DocId = sorted[i].Key, Score = sorted[i].Value });
            }
            return new SearchResult<TDocument> { Hits = page, TotalHits = total };
        }

        /// <summary>Documents matching one clause, with its score.</summary>
        private Dictionary<int, decimal> ScoreClause(InvertedIndex index, QueryClause clause, bool[] fields)
        {
            TermClause term = clause as TermClause;
            if (term is not null)
            {
                return AllWords(index, Words(term.Text), fields, (f, w) => TermScores(index, f, w, 1m));
            }
            FuzzyClause fuzzy = clause as FuzzyClause;
            if (fuzzy is not null)
            {
                return AllWords(index, Words(fuzzy.Term), fields, (f, w) => FuzzyScores(index, f, w, fuzzy));
            }
            PhraseClause phrase = clause as PhraseClause;
            if (phrase is not null)
            {
                return PhraseScores(index, Words(phrase.Text), phrase.Slop, fields);
            }
            throw new ArgumentException("Unsupported clause type " + clause.GetType().Name + ".", nameof(clause));
        }

        private List<string> Words(string text) => TokenGraph.MainPath(_schema.Analyzer.Analyze(text));

        /// <summary>Every word must match; a word scores its best field (DisjunctionMaxQuery, tie breaker 0).</summary>
        private static Dictionary<int, decimal> AllWords(InvertedIndex index, List<string> words, bool[] fields, Func<int, string, Dictionary<int, decimal>> score)
        {
            Dictionary<int, decimal> result = null;
            foreach (string word in words)
            {
                Dictionary<int, decimal> best = new Dictionary<int, decimal>();
                for (int f = 0; f < index.Fields.Count; f++)
                {
                    if (fields is not null && !fields[f]) { continue; }
                    foreach (KeyValuePair<int, decimal> pair in score(f, word))
                    {
                        decimal s;
                        if (!best.TryGetValue(pair.Key, out s) || pair.Value > s) { best[pair.Key] = pair.Value; }
                    }
                }
                if (result is null)
                {
                    result = best;
                    continue;
                }
                Dictionary<int, decimal> next = new Dictionary<int, decimal>();
                foreach (KeyValuePair<int, decimal> pair in result)
                {
                    decimal s;
                    if (best.TryGetValue(pair.Key, out s)) { next[pair.Key] = pair.Value + s; }
                }
                result = next;
            }
            return result ?? new Dictionary<int, decimal>();
        }

        /// <summary>BM25 of one term in one field (TermQuery), times the field boost and <paramref name="boost"/>.</summary>
        private static Dictionary<int, decimal> TermScores(InvertedIndex index, int field, string term, decimal boost)
        {
            Dictionary<int, decimal> scores = new Dictionary<int, decimal>();
            Posting[] postings = index.Postings(field, term);
            if (postings.Length == 0)
            {
                return scores;
            }
            decimal weight = boost * index.Fields[field].Boost * index.Idf(postings.Length);
            foreach (Posting p in postings)
            {
                scores[p.DocId] = weight * index.TfWeight(field, p.DocId, p.TermFreq);
            }
            return scores;
        }

        /// <summary>FuzzyQuery: every dictionary term within the edit budget, boosted 1 - edits / shorter length;
        /// a document keeps its best expansion.</summary>
        private static Dictionary<int, decimal> FuzzyScores(InvertedIndex index, int field, string word, FuzzyClause clause)
        {
            Dictionary<int, decimal> best = new Dictionary<int, decimal>();
            foreach (FuzzyMatch m in FuzzyTermEnumerator.Enumerate(index.Terms(field), word, clause.MaxEdits, clause.Transpositions, clause.PrefixLength))
            {
                int shorter = Math.Min(word.Length, m.Term.Length);
                decimal boost = shorter == 0 ? 0m : 1m - (decimal)m.Distance / shorter;
                if (boost <= 0m)
                {
                    continue;
                }
                foreach (KeyValuePair<int, decimal> pair in TermScores(index, field, m.Term, boost))
                {
                    decimal s;
                    if (!best.TryGetValue(pair.Key, out s) || pair.Value > s) { best[pair.Key] = pair.Value; }
                }
            }
            return best;
        }

        /// <summary>PhraseQuery: the words in order within <paramref name="slop"/> in one positions field; the
        /// score is the sum of the words' BM25 in that field, best field kept.</summary>
        private static Dictionary<int, decimal> PhraseScores(InvertedIndex index, List<string> words, int slop, bool[] fields)
        {
            Dictionary<int, decimal> result = new Dictionary<int, decimal>();
            if (words.Count == 0)
            {
                return result;
            }
            for (int f = 0; f < index.Fields.Count; f++)
            {
                if (!index.Fields[f].Positions || (fields is not null && !fields[f])) { continue; }
                Posting[] first = index.Postings(f, words[0]);
                foreach (Posting p in first)
                {
                    List<int[]> lists = new List<int[]>(words.Count);
                    decimal sum = 0m;
                    bool complete = true;
                    foreach (string w in words)
                    {
                        Posting? hit = index.PostingFor(f, w, p.DocId);
                        if (!hit.HasValue) { complete = false; break; }
                        lists.Add(hit.Value.Positions);
                        sum += index.Fields[f].Boost * index.Idf(index.DocFrequency(f, w)) * index.TfWeight(f, p.DocId, hit.Value.TermFreq);
                    }
                    if (!complete || !PositionIntersection.PhraseMatches(lists, slop))
                    {
                        continue;
                    }
                    decimal s;
                    if (!result.TryGetValue(p.DocId, out s) || sum > s) { result[p.DocId] = sum; }
                }
            }
            return result;
        }

        private IndexSnapshot<TDocument> Current()
        {
            IndexSnapshot<TDocument> snapshot = Volatile.Read(ref _current);
            if (snapshot is null)
            {
                throw new InvalidOperationException("The index has no snapshot yet; call Rebuild first.");
            }
            return snapshot;
        }

        private bool[] FieldMask(IReadOnlyList<string> fields)
        {
            if (fields is null)
            {
                return null;
            }
            bool[] mask = new bool[_textFields.Length];
            foreach (string name in fields)
            {
                mask[_schema.TextFieldIndex(name)] = true;
            }
            return mask;
        }

        private static bool[] BuildAllowed(IndexSnapshot<TDocument> snapshot, SearchRequest<TDocument> request)
        {
            if (request.Filters.Count == 0)
            {
                return null;
            }
            List<Func<int, bool>> tests = new List<Func<int, bool>>();
            foreach (FastFieldFilter filter in request.Filters)
            {
                if (filter is null)
                {
                    throw new ArgumentException("A filter is null.", nameof(request));
                }
                tests.Add(filter.Compile(snapshot.Column(filter.Field)));
            }
            int n = snapshot.Documents.Count;
            bool[] allowed = new bool[n];
            for (int doc = 0; doc < n; doc++)
            {
                bool ok = true;
                for (int t = 0; t < tests.Count && ok; t++)
                {
                    ok = tests[t](doc);
                }
                allowed[doc] = ok;
            }
            return allowed;
        }

        private static Comparison<KeyValuePair<int, decimal>> BuildOrder(IndexSnapshot<TDocument> snapshot, SearchRequest<TDocument> request)
        {
            List<KeyValuePair<FastFieldColumn, bool>> sort = new List<KeyValuePair<FastFieldColumn, bool>>();
            foreach (SortField f in request.Sort)
            {
                if (f is null)
                {
                    throw new ArgumentException("A sort field is null.", nameof(request));
                }
                FastFieldColumn column = snapshot.Column(f.Field);
                if (column.Kind != FastFieldKind.Integer && column.Kind != FastFieldKind.Decimal)
                {
                    throw new ArgumentException("Sort field '" + f.Field + "' must be numeric.", nameof(request));
                }
                sort.Add(new KeyValuePair<FastFieldColumn, bool>(column, f.Descending));
            }
            return (a, b) =>
            {
                foreach (KeyValuePair<FastFieldColumn, bool> pair in sort)
                {
                    FastFieldColumn column = pair.Key;
                    bool ha = column.HasValue(a.Key);
                    bool hb = column.HasValue(b.Key);
                    if (ha != hb)
                    {
                        // A document without a value sorts after one with a value.
                        return ha ? -1 : 1;
                    }
                    if (!ha)
                    {
                        continue;
                    }
                    int c = column.Numeric(a.Key).CompareTo(column.Numeric(b.Key));
                    if (c != 0)
                    {
                        return pair.Value ? -c : c;
                    }
                }
                int byScore = b.Value.CompareTo(a.Value);
                return byScore != 0 ? byScore : a.Key.CompareTo(b.Key);
            };
        }
    }
}
