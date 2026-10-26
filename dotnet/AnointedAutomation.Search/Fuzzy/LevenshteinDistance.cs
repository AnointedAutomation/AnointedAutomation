// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Ported from tantivy levenshtein_automata src/levenshtein_nfa.rs (Distance) by Paul Masurel (MIT).
// Translated from Rust to C#.

namespace AnointedAutomation.Search.Fuzzy
{
    /// <summary>An edit distance: exact, or "at least" when it exceeds the automaton's maximum.</summary>
    public readonly struct LevenshteinDistance
    {
        public LevenshteinDistance(int value, bool isExact)
        {
            Value = value;
            IsExact = isExact;
        }

        public int Value { get; }

        /// <summary>True when <see cref="Value"/> is the exact distance (within the maximum).</summary>
        public bool IsExact { get; }

        public static LevenshteinDistance Exact(int value) => new LevenshteinDistance(value, true);

        public static LevenshteinDistance AtLeast(int value) => new LevenshteinDistance(value, false);

        public override string ToString() => (IsExact ? "Exact(" : "AtLeast(") + Value + ")";
    }
}
