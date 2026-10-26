// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Ported from tantivy src/termdict/ (TermDictionary: sorted terms, range by prefix) and Apache Lucene
// lucene/core/src/java/org/apache/lucene/index/TermsEnum.java (seekCeil). Translated from Rust and Java to
// C#: a sorted string array with binary search replaces the FST/sstable, ample for in-memory indexes.

using System;
using System.Collections.Generic;

namespace AnointedAutomation.Search.Index
{
    /// <summary>An immutable, ordinally sorted set of terms.</summary>
    public sealed class TermDictionary
    {
        public TermDictionary(IEnumerable<string> terms)
        {
            if (terms is null)
            {
                throw new ArgumentNullException(nameof(terms));
            }
            SortedSet<string> set = new SortedSet<string>(terms, StringComparer.Ordinal);
            string[] array = new string[set.Count];
            set.CopyTo(array);
            Terms = array;
        }

        public IReadOnlyList<string> Terms { get; }

        public int Count => Terms.Count;

        /// <summary>Index of the first term &gt;= <paramref name="term"/> (Lucene seekCeil).</summary>
        public int SeekCeil(string term)
        {
            int lo = 0;
            int hi = Terms.Count;
            while (lo < hi)
            {
                int mid = (lo + hi) >> 1;
                if (string.CompareOrdinal(Terms[mid], term) < 0) { lo = mid + 1; } else { hi = mid; }
            }
            return lo;
        }

        /// <summary>The half open index range [lo, hi) of terms starting with <paramref name="prefix"/>.</summary>
        public void PrefixRange(string prefix, out int lo, out int hi)
        {
            if (prefix is null)
            {
                throw new ArgumentNullException(nameof(prefix));
            }
            lo = SeekCeil(prefix);
            hi = lo;
            while (hi < Terms.Count && Terms[hi].StartsWith(prefix, StringComparison.Ordinal))
            {
                hi++;
            }
        }

        public bool Contains(string term)
        {
            int i = SeekCeil(term);
            return i < Terms.Count && string.Equals(Terms[i], term, StringComparison.Ordinal);
        }
    }
}
