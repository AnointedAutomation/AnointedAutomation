// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Ported from tantivy levenshtein_automata src/lib.rs (LevenshteinAutomatonBuilder) by Paul Masurel (MIT),
// with the caching role of Apache Lucene lucene/core/src/java/org/apache/lucene/util/automaton/LevenshteinAutomata.java
// (one parametric description per distance and transposition setting). Translated from Rust and Java to C#.

using System;
using System.Collections.Concurrent;

namespace AnointedAutomation.Search.Fuzzy
{
    /// <summary>Builds Levenshtein DFAs for queries. The parametric DFA is computed once per
    /// (maxDistance, transposition) and cached process wide.</summary>
    public sealed class LevenshteinAutomatonBuilder
    {
        /// <summary>Lucene's LevenshteinAutomata.MAXIMUM_SUPPORTED_DISTANCE.</summary>
        public const int MaximumSupportedDistance = 2;

        private static readonly ConcurrentDictionary<int, ParametricDfa> Cache = new ConcurrentDictionary<int, ParametricDfa>();

        private readonly ParametricDfa _parametric;

        /// <param name="maxDistance">0 to 2 edits.</param>
        /// <param name="transposition">An adjacent swap costs one edit (Damerau), not two.</param>
        public LevenshteinAutomatonBuilder(int maxDistance, bool transposition)
        {
            if (maxDistance < 0 || maxDistance > MaximumSupportedDistance)
            {
                throw new ArgumentOutOfRangeException(nameof(maxDistance), "Supported distances are 0 to " + MaximumSupportedDistance + ".");
            }
            MaxDistance = maxDistance;
            Transposition = transposition;
            int key = maxDistance * 2 + (transposition ? 1 : 0);
            _parametric = Cache.GetOrAdd(key, _ => ParametricDfa.FromNfa(new LevenshteinNfa(maxDistance, transposition)));
        }

        public int MaxDistance { get; }

        public bool Transposition { get; }

        /// <summary>DFA matching strings within MaxDistance of <paramref name="query"/>.</summary>
        public LevenshteinDfa BuildDfa(string query)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return _parametric.BuildDfa(query, false);
        }

        /// <summary>DFA matching strings that START with something within MaxDistance of <paramref name="query"/>.</summary>
        public LevenshteinDfa BuildPrefixDfa(string query)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return _parametric.BuildDfa(query, true);
        }
    }
}
