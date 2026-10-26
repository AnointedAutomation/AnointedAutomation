// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Ported from Apache Lucene lucene/core/src/java/org/apache/lucene/search/FuzzyTermsEnum.java (prefix
// length, max edits, transpositions) and tantivy src/query/fuzzy_query.rs with src/query/automaton_weight.rs
// (running a Levenshtein DFA over the term dictionary). Translated from Java and Rust to C#: the FST walk
// became a walk over a sorted term array that reuses DFA states across the shared prefix of neighbors.

using System;
using System.Collections.Generic;
using AnointedAutomation.Search.Index;

namespace AnointedAutomation.Search.Fuzzy
{
    /// <summary>One dictionary term within the edit budget of a fuzzy query.</summary>
    public readonly struct FuzzyMatch
    {
        public FuzzyMatch(string term, int distance)
        {
            Term = term;
            Distance = distance;
        }

        public string Term { get; }
        public int Distance { get; }
    }

    /// <summary>Enumerates the terms of a <see cref="TermDictionary"/> within an edit distance of a query.</summary>
    public static class FuzzyTermEnumerator
    {
        /// <param name="dictionary">Sorted term dictionary.</param>
        /// <param name="query">The (analyzed) query term.</param>
        /// <param name="maxEdits">0 to 2.</param>
        /// <param name="transpositions">Count an adjacent swap as one edit.</param>
        /// <param name="prefixLength">Leading characters that must match exactly (Lucene FuzzyQuery prefixLength).</param>
        public static List<FuzzyMatch> Enumerate(TermDictionary dictionary, string query, int maxEdits, bool transpositions, int prefixLength)
        {
            if (dictionary is null)
            {
                throw new ArgumentNullException(nameof(dictionary));
            }
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            if (prefixLength < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(prefixLength));
            }
            List<FuzzyMatch> matches = new List<FuzzyMatch>();
            int fixedLen = Math.Min(prefixLength, query.Length);
            string prefix = query.Substring(0, fixedLen);
            string rest = query.Substring(fixedLen);
            LevenshteinDfa dfa = new LevenshteinAutomatonBuilder(maxEdits, transpositions).BuildDfa(rest);
            int lo;
            int hi;
            dictionary.PrefixRange(prefix, out lo, out hi);
            // states[i] = DFA state after reading i chars of the current term's suffix.
            List<int> states = new List<int> { dfa.InitialState };
            string previous = string.Empty;
            for (int t = lo; t < hi; t++)
            {
                string term = dictionary.Terms[t];
                string suffix = term.Substring(fixedLen);
                int shared = CommonPrefix(previous, suffix);
                if (shared + 1 < states.Count)
                {
                    states.RemoveRange(shared + 1, states.Count - shared - 1);
                }
                int state = states[states.Count - 1];
                int i = states.Count - 1;
                for (; i < suffix.Length && state != LevenshteinDfa.SinkState; i++)
                {
                    state = dfa.Transition(state, suffix[i]);
                    states.Add(state);
                }
                previous = suffix.Substring(0, states.Count - 1);
                if (i < suffix.Length || state == LevenshteinDfa.SinkState)
                {
                    continue;
                }
                LevenshteinDistance d = dfa.Distance(state);
                if (d.IsExact)
                {
                    matches.Add(new FuzzyMatch(term, d.Value));
                }
            }
            return matches;
        }

        private static int CommonPrefix(string a, string b)
        {
            int n = Math.Min(a.Length, b.Length);
            int i = 0;
            while (i < n && a[i] == b[i])
            {
                i++;
            }
            return i;
        }
    }
}
