// Copyright © Anointed Automation Corporation., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Ported from tantivy src/query/phrase_query/phrase_scorer.rs (intersection_exists,
// intersection_count_with_slop) and Apache Lucene lucene/core/src/java/org/apache/lucene/search/SloppyPhraseMatcher.java
// (slop as allowed position distance). Translated from Rust and Java to C#.

using System;
using System.Collections.Generic;

namespace AnointedAutomation.Search.Scoring
{
    /// <summary>Phrase and proximity checks over sorted position lists.</summary>
    public static class PositionIntersection
    {
        /// <summary>True when the sorted arrays share a value (tantivy intersection_exists).</summary>
        public static bool IntersectionExists(int[] left, int[] right)
        {
            int l = 0;
            int r = 0;
            while (l < left.Length && r < right.Length)
            {
                int c = left[l].CompareTo(right[r]);
                if (c < 0) { l++; }
                else if (c == 0) { return true; }
                else { r++; }
            }
            return false;
        }

        /// <summary>True when the terms occur in order, term i at position p + i (within <paramref name="slop"/>
        /// extra positions per gap). positions[i] are the sorted positions of the i-th phrase term.</summary>
        public static bool PhraseMatches(IReadOnlyList<int[]> positions, int slop)
        {
            if (positions is null)
            {
                throw new ArgumentNullException(nameof(positions));
            }
            if (slop < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(slop));
            }
            if (positions.Count == 0)
            {
                return false;
            }
            foreach (int start in positions[0])
            {
                if (Extend(positions, 1, start, slop))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>Backtracking extension: some position of term i lies 1 to slop + 1 after prev.</summary>
        private static bool Extend(IReadOnlyList<int[]> positions, int i, int prev, int slop)
        {
            if (i == positions.Count)
            {
                return true;
            }
            int[] list = positions[i];
            int lo = FirstIndexAtLeast(list, prev + 1);
            for (int k = lo; k < list.Length && list[k] - prev - 1 <= slop; k++)
            {
                if (Extend(positions, i + 1, list[k], slop))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>True when some position of <paramref name="second"/> follows one of <paramref name="first"/>
        /// by 1 to <paramref name="maxGap"/> positions.</summary>
        public static bool Follows(int[] first, int[] second, int maxGap)
        {
            for (int x = 0; x < first.Length; x++)
            {
                int next = FirstAtLeast(second, first[x] + 1);
                if (next >= 0 && next - first[x] <= maxGap)
                {
                    return true;
                }
            }
            return false;
        }

        private static int FirstAtLeast(int[] sorted, int value)
        {
            int index = FirstIndexAtLeast(sorted, value);
            return index < sorted.Length ? sorted[index] : -1;
        }

        private static int FirstIndexAtLeast(int[] sorted, int value)
        {
            int lo = 0;
            int hi = sorted.Length;
            while (lo < hi)
            {
                int mid = (lo + hi) >> 1;
                if (sorted[mid] < value) { lo = mid + 1; } else { hi = mid; }
            }
            return lo;
        }
    }
}
