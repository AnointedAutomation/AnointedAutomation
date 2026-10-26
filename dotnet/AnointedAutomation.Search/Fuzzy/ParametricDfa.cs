// Copyright © Anointed Automation Corporation., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Ported from tantivy levenshtein_automata src/parametric_dfa.rs and src/alphabet.rs by Paul Masurel (MIT).
// Translated from Rust to C#: the UTF-8 byte DFA (src/dfa.rs Utf8DFABuilder) became a char DFA, since .NET
// strings are UTF-16 and terms are compared char by char.

using System;
using System.Collections.Generic;

namespace AnointedAutomation.Search.Fuzzy
{
    /// <summary>A query independent DFA over "shapes" of NFA multistates. Built once per (max distance,
    /// transposition) pair, then specialized to a query in <see cref="BuildDfa"/>.</summary>
    internal sealed class ParametricDfa
    {
        private readonly int[] _distance;
        private readonly int[] _transitionShape;
        private readonly int[] _transitionDelta;
        private readonly int _maxDistance;
        private readonly int _transitionStride;
        private readonly int _diameter;

        private ParametricDfa(int[] distance, int[] transitionShape, int[] transitionDelta, int maxDistance, int transitionStride, int diameter)
        {
            _distance = distance;
            _transitionShape = transitionShape;
            _transitionDelta = transitionDelta;
            _maxDistance = maxDistance;
            _transitionStride = transitionStride;
            _diameter = diameter;
        }

        public int MaxDistance => _maxDistance;

        public int NumShapes => _transitionShape.Length / _transitionStride;

        private readonly struct ParametricState
        {
            public ParametricState(int shapeId, int offset)
            {
                ShapeId = shapeId;
                Offset = offset;
            }

            public int ShapeId { get; }
            public int Offset { get; }
            public bool IsDeadEnd => ShapeId == 0;
        }

        private ParametricState Apply(ParametricState state, int chi)
        {
            int index = _transitionStride * state.ShapeId + chi;
            int dest = _transitionShape[index];
            return new ParametricState(dest, dest == 0 ? 0 : state.Offset + _transitionDelta[index]);
        }

        private LevenshteinDistance Distance(ParametricState state, int queryLen)
        {
            int remaining = queryLen - state.Offset;
            if (state.IsDeadEnd || remaining < 0 || remaining >= _diameter)
            {
                return LevenshteinDistance.AtLeast(_maxDistance + 1);
            }
            int d = _distance[_diameter * state.ShapeId + remaining];
            return d > _maxDistance ? LevenshteinDistance.AtLeast(d) : LevenshteinDistance.Exact(d);
        }

        private bool IsPrefixSink(ParametricState state, int queryLen)
        {
            if (state.IsDeadEnd)
            {
                return true;
            }
            int remaining = queryLen - state.Offset;
            if (remaining < 0 || remaining >= _diameter)
            {
                return false;
            }
            int baseIndex = _diameter * state.ShapeId;
            int prefixDistance = _distance[baseIndex + remaining];
            if (prefixDistance > _maxDistance)
            {
                return false;
            }
            for (int i = 0; i < remaining; i++)
            {
                if (_distance[baseIndex + i] < prefixDistance)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>The DFA accepting every string within the maximum distance of <paramref name="query"/>
        /// (or, with <paramref name="prefix"/>, every string starting with such a string).</summary>
        public LevenshteinDfa BuildDfa(string query, bool prefix)
        {
            char[] chars = query.ToCharArray();
            int queryLen = chars.Length;
            // Alphabet: the distinct query chars with their characteristic bit vectors (alphabet.rs).
            SortedSet<char> distinct = new SortedSet<char>(chars);
            List<KeyValuePair<char, uint[]>> alphabet = new List<KeyValuePair<char, uint[]>>(distinct.Count);
            int chunks = (queryLen + 31) / 32;
            foreach (char c in distinct)
            {
                uint[] bits = new uint[chunks + 1];
                for (int i = 0; i < queryLen; i++)
                {
                    if (chars[i] == c)
                    {
                        bits[i / 32] |= 1u << (i % 32);
                    }
                }
                alphabet.Add(new KeyValuePair<char, uint[]>(c, bits));
            }

            int numOffsets = queryLen + 1;
            int[] stateIndex = new int[NumShapes * numOffsets];
            Array.Fill(stateIndex, -1);
            List<ParametricState> queue = new List<ParametricState>();
            int GetOrAllocate(ParametricState s)
            {
                int bucket = s.ShapeId * numOffsets + s.Offset;
                if (stateIndex[bucket] >= 0)
                {
                    return stateIndex[bucket];
                }
                int id = queue.Count;
                queue.Add(s);
                stateIndex[bucket] = id;
                return id;
            }

            int dead = GetOrAllocate(new ParametricState(0, 0));
            int initial = GetOrAllocate(new ParametricState(1, 0));
            uint mask = (1u << _diameter) - 1u;
            List<int> defaults = new List<int>();
            List<Dictionary<char, int>> transitions = new List<Dictionary<char, int>>();
            List<LevenshteinDistance> distances = new List<LevenshteinDistance>();
            for (int stateId = 0; stateId < queue.Count; stateId++)
            {
                ParametricState state = queue[stateId];
                distances.Add(Distance(state, queryLen));
                if (prefix && IsPrefixSink(state, queryLen))
                {
                    defaults.Add(stateId);
                    transitions.Add(null);
                    continue;
                }
                int defaultSuccessor = GetOrAllocate(Apply(state, 0));
                defaults.Add(defaultSuccessor);
                Dictionary<char, int> map = new Dictionary<char, int>(alphabet.Count);
                foreach (KeyValuePair<char, uint[]> entry in alphabet)
                {
                    int chi = (int)ShiftAndMask(entry.Value, state.Offset, mask);
                    map[entry.Key] = GetOrAllocate(Apply(state, chi));
                }
                transitions.Add(map);
            }
            if (dead != 0 || initial != 1)
            {
                throw new InvalidOperationException("Parametric DFA state allocation out of order.");
            }
            return new LevenshteinDfa(defaults.ToArray(), transitions.ToArray(), distances.ToArray(), initial);
        }

        private static uint ShiftAndMask(uint[] bits, int offset, uint mask)
        {
            int bucket = offset / 32;
            int align = offset - bucket * 32;
            if (align == 0)
            {
                return bits[bucket] & mask;
            }
            uint left = bits[bucket] >> align;
            uint right = bits[bucket + 1] << (32 - align);
            return (left | right) & mask;
        }

        /// <summary>Builds the parametric DFA by exploring every NFA multistate shape reachable from the start.</summary>
        public static ParametricDfa FromNfa(LevenshteinNfa nfa)
        {
            Dictionary<string, int> index = new Dictionary<string, int>(StringComparer.Ordinal);
            List<MultiState> shapes = new List<MultiState>();
            int GetOrAllocate(MultiState ms)
            {
                string key = ms.Key();
                int id;
                if (index.TryGetValue(key, out id))
                {
                    return id;
                }
                id = shapes.Count;
                shapes.Add(ms.Clone());
                index[key] = id;
                return id;
            }

            GetOrAllocate(new MultiState());
            GetOrAllocate(nfa.InitialStates());
            int diameter = nfa.MultistateDiameter;
            int numChi = 1 << diameter;
            List<int> shapeIds = new List<int>();
            List<int> deltas = new List<int>();
            MultiState dest = new MultiState();
            for (int stateId = 0; stateId < shapes.Count; stateId++)
            {
                for (int chi = 0; chi < numChi; chi++)
                {
                    nfa.Transition(shapes[stateId], dest, (ulong)chi);
                    int translation = dest.Normalize();
                    int destId = GetOrAllocate(dest);
                    shapeIds.Add(destId);
                    deltas.Add(translation);
                }
            }
            int[] distance = new int[diameter * shapes.Count];
            for (int stateId = 0; stateId < shapes.Count; stateId++)
            {
                for (int offset = 0; offset < diameter; offset++)
                {
                    distance[stateId * diameter + offset] = nfa.MultistateDistance(shapes[stateId], offset).Value;
                }
            }
            return new ParametricDfa(distance, shapeIds.ToArray(), deltas.ToArray(), nfa.MaxDistance, numChi, diameter);
        }
    }
}
