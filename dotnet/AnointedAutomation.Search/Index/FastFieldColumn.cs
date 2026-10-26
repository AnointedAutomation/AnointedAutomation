// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Ported from tantivy columnar/src/column/ (Column: one value per doc, optional index) and
// columnar/src/column_values/ with the dictionary encoded string column (columnar/src/column/dictionary_encoded.rs).
// Translated from Rust to C#: plain arrays indexed by doc id instead of bitpacked codecs.

using System;
using System.Collections.Generic;

namespace AnointedAutomation.Search.Index
{
    /// <summary>A column of per document values for filtering and sorting.</summary>
    public sealed class FastFieldColumn
    {
        private readonly long[] _integers;
        private readonly decimal[] _decimals;
        private readonly bool[] _has;
        private readonly int[][] _ordinals;
        private readonly Dictionary<string, int> _dictionary;

        private FastFieldColumn(string name, FastFieldKind kind, long[] integers, decimal[] decimals, bool[] has, int[][] ordinals, Dictionary<string, int> dictionary)
        {
            Name = name;
            Kind = kind;
            _integers = integers;
            _decimals = decimals;
            _has = has;
            _ordinals = ordinals;
            _dictionary = dictionary;
        }

        public string Name { get; }

        public FastFieldKind Kind { get; }

        /// <summary>True when the document has a value (any keyword, for keyword columns).</summary>
        public bool HasValue(int docId) => Kind == FastFieldKind.Keyword ? _ordinals[docId].Length > 0 : _has[docId];

        public long Integer(int docId)
        {
            RequireKind(FastFieldKind.Integer);
            if (!_has[docId])
            {
                throw new InvalidOperationException("Document " + docId + " has no value in '" + Name + "'.");
            }
            return _integers[docId];
        }

        public decimal Decimal(int docId)
        {
            RequireKind(FastFieldKind.Decimal);
            if (!_has[docId])
            {
                throw new InvalidOperationException("Document " + docId + " has no value in '" + Name + "'.");
            }
            return _decimals[docId];
        }

        /// <summary>The numeric value as decimal (integer or decimal columns), for range filters and sorts.</summary>
        public decimal Numeric(int docId)
        {
            if (Kind == FastFieldKind.Integer)
            {
                return Integer(docId);
            }
            return Decimal(docId);
        }

        /// <summary>The keyword ordinal, or -1 when the keyword never occurs in the column.</summary>
        public int Ordinal(string keyword)
        {
            RequireKind(FastFieldKind.Keyword);
            int ord;
            return keyword is not null && _dictionary.TryGetValue(keyword, out ord) ? ord : -1;
        }

        public bool HasOrdinal(int docId, int ordinal)
        {
            RequireKind(FastFieldKind.Keyword);
            int[] ords = _ordinals[docId];
            return Array.BinarySearch(ords, ordinal) >= 0;
        }

        private void RequireKind(FastFieldKind kind)
        {
            if (Kind != kind)
            {
                throw new InvalidOperationException("Field '" + Name + "' is " + Kind + ", not " + kind + ".");
            }
        }

        internal static FastFieldColumn Build<TDocument>(FastFieldDefinition<TDocument> def, IReadOnlyList<TDocument> docs)
        {
            int n = docs.Count;
            switch (def.Kind)
            {
                case FastFieldKind.Integer:
                {
                    long[] values = new long[n];
                    bool[] has = new bool[n];
                    for (int d = 0; d < n; d++)
                    {
                        long? v = def.Integer(docs[d]);
                        if (v.HasValue)
                        {
                            values[d] = v.Value;
                            has[d] = true;
                        }
                    }
                    return new FastFieldColumn(def.Name, def.Kind, values, null, has, null, null);
                }
                case FastFieldKind.Decimal:
                {
                    decimal[] values = new decimal[n];
                    bool[] has = new bool[n];
                    for (int d = 0; d < n; d++)
                    {
                        decimal? v = def.Decimal(docs[d]);
                        if (v.HasValue)
                        {
                            values[d] = v.Value;
                            has[d] = true;
                        }
                    }
                    return new FastFieldColumn(def.Name, def.Kind, null, values, has, null, null);
                }
                case FastFieldKind.Keyword:
                {
                    Dictionary<string, int> dictionary = new Dictionary<string, int>(StringComparer.Ordinal);
                    int[][] ordinals = new int[n][];
                    List<int> buffer = new List<int>();
                    for (int d = 0; d < n; d++)
                    {
                        buffer.Clear();
                        IEnumerable<string> values = def.Keywords(docs[d]);
                        if (values is not null)
                        {
                            foreach (string v in values)
                            {
                                if (v is null)
                                {
                                    continue;
                                }
                                int ord;
                                if (!dictionary.TryGetValue(v, out ord))
                                {
                                    ord = dictionary.Count;
                                    dictionary[v] = ord;
                                }
                                if (!buffer.Contains(ord))
                                {
                                    buffer.Add(ord);
                                }
                            }
                        }
                        buffer.Sort();
                        ordinals[d] = buffer.ToArray();
                    }
                    return new FastFieldColumn(def.Name, def.Kind, null, null, null, ordinals, dictionary);
                }
                default:
                    throw new InvalidOperationException("Unsupported fast field kind " + def.Kind + ".");
            }
        }
    }
}
