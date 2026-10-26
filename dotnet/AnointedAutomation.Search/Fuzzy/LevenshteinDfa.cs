// Copyright © Anointed Automation Corporation., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Ported from tantivy levenshtein_automata src/dfa.rs by Paul Masurel (MIT). Translated from Rust to C#:
// transitions are per char (UTF-16) instead of per UTF-8 byte; state 0 is the sink, as in dfa.rs.

using System.Collections.Generic;

namespace AnointedAutomation.Search.Fuzzy
{
    /// <summary>A deterministic automaton that reads a string and reports its edit distance to the query.</summary>
    public sealed class LevenshteinDfa
    {
        /// <summary>The dead state: no continuation can come back within the maximum distance.</summary>
        public const int SinkState = 0;

        private readonly int[] _defaults;
        private readonly Dictionary<char, int>[] _transitions;
        private readonly LevenshteinDistance[] _distances;

        internal LevenshteinDfa(int[] defaults, Dictionary<char, int>[] transitions, LevenshteinDistance[] distances, int initialState)
        {
            _defaults = defaults;
            _transitions = transitions;
            _distances = distances;
            InitialState = initialState;
        }

        public int InitialState { get; }

        public int StateCount => _defaults.Length;

        public int Transition(int state, char c)
        {
            Dictionary<char, int> map = _transitions[state];
            int next;
            if (map is not null && map.TryGetValue(c, out next))
            {
                return next;
            }
            return _defaults[state];
        }

        public LevenshteinDistance Distance(int state) => _distances[state];

        /// <summary>Runs the whole string from the initial state.</summary>
        public LevenshteinDistance Eval(string text)
        {
            int state = InitialState;
            foreach (char c in text)
            {
                state = Transition(state, c);
            }
            return Distance(state);
        }
    }
}
