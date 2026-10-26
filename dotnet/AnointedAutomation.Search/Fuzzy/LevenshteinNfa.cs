// Copyright © Anointed Automation Corporation., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Ported from tantivy levenshtein_automata src/levenshtein_nfa.rs and src/index.rs by Paul Masurel (MIT),
// itself based on "Fast String Correction with Levenshtein-Automata" (Schulz and Mihov, 2002), the same
// paper behind Apache Lucene's LevenshteinAutomata.java. Translated from Rust to C#.

using System;
using System.Collections.Generic;
using System.Text;

namespace AnointedAutomation.Search.Fuzzy
{
    /// <summary>One NFA state: how far into the query, how many edits so far, and whether a transposition is open.</summary>
    internal readonly struct NfaState : IComparable<NfaState>
    {
        public NfaState(int offset, int distance, bool inTranspose)
        {
            Offset = offset;
            Distance = distance;
            InTranspose = inTranspose;
        }

        public int Offset { get; }
        public int Distance { get; }
        public bool InTranspose { get; }

        /// <summary>True when this state makes <paramref name="other"/> redundant.</summary>
        public bool Imply(NfaState other)
        {
            bool transposeImply = InTranspose || !other.InTranspose;
            int deltaOffset = Math.Abs(Offset - other.Offset);
            if (transposeImply)
            {
                return other.Distance >= Distance + deltaOffset;
            }
            return other.Distance > Distance + deltaOffset;
        }

        public int CompareTo(NfaState other)
        {
            int c = Offset.CompareTo(other.Offset);
            if (c != 0) { return c; }
            c = Distance.CompareTo(other.Distance);
            if (c != 0) { return c; }
            return InTranspose.CompareTo(other.InTranspose);
        }
    }

    /// <summary>A set of NFA states with redundant states pruned.</summary>
    internal sealed class MultiState
    {
        public readonly List<NfaState> States = new List<NfaState>();

        public void Clear() => States.Clear();

        /// <summary>Shifts offsets so the smallest is 0; returns the shift.</summary>
        public int Normalize()
        {
            int min = 0;
            if (States.Count > 0)
            {
                min = int.MaxValue;
                foreach (NfaState s in States)
                {
                    if (s.Offset < min) { min = s.Offset; }
                }
            }
            for (int i = 0; i < States.Count; i++)
            {
                NfaState s = States[i];
                States[i] = new NfaState(s.Offset - min, s.Distance, s.InTranspose);
            }
            States.Sort();
            return min;
        }

        public void AddState(NfaState newState)
        {
            foreach (NfaState s in States)
            {
                if (s.Imply(newState))
                {
                    return;
                }
            }
            int i = 0;
            while (i < States.Count)
            {
                if (newState.Imply(States[i]))
                {
                    // swap_remove
                    States[i] = States[States.Count - 1];
                    States.RemoveAt(States.Count - 1);
                }
                else
                {
                    i++;
                }
            }
            States.Add(newState);
        }

        public MultiState Clone()
        {
            MultiState copy = new MultiState();
            copy.States.AddRange(States);
            return copy;
        }

        /// <summary>Identity key for the shape index.</summary>
        public string Key()
        {
            StringBuilder sb = new StringBuilder(States.Count * 6);
            foreach (NfaState s in States)
            {
                sb.Append(s.Offset).Append(',').Append(s.Distance).Append(s.InTranspose ? 't' : 'f').Append(';');
            }
            return sb.ToString();
        }
    }

    /// <summary>The (Damerau) Levenshtein NFA, simulated over characteristic vectors.</summary>
    internal sealed class LevenshteinNfa
    {
        public LevenshteinNfa(int maxDistance, bool transposition)
        {
            MaxDistance = maxDistance;
            Damerau = transposition;
        }

        public int MaxDistance { get; }

        public bool Damerau { get; }

        public int MultistateDiameter => 2 * MaxDistance + 1;

        public MultiState InitialStates()
        {
            MultiState ms = new MultiState();
            ms.AddState(new NfaState(0, 0, false));
            return ms;
        }

        public LevenshteinDistance MultistateDistance(MultiState multistate, int queryLen)
        {
            int best = int.MaxValue;
            foreach (NfaState s in multistate.States)
            {
                int d = s.Distance + Math.Abs(queryLen - s.Offset);
                if (d <= MaxDistance && d < best)
                {
                    best = d;
                }
            }
            return best == int.MaxValue ? LevenshteinDistance.AtLeast(MaxDistance + 1) : LevenshteinDistance.Exact(best);
        }

        private static bool ExtractBit(ulong bitset, int pos) => ((bitset >> pos) & 1UL) == 1UL;

        private void SimpleTransition(NfaState state, ulong symbol, MultiState multistate)
        {
            if (state.Distance < MaxDistance)
            {
                // insertion
                multistate.AddState(new NfaState(state.Offset, state.Distance + 1, false));
                // substitution
                multistate.AddState(new NfaState(state.Offset + 1, state.Distance + 1, false));
                // deletions
                for (int d = 1; d < MaxDistance + 1 - state.Distance; d++)
                {
                    if (ExtractBit(symbol, d))
                    {
                        multistate.AddState(new NfaState(state.Offset + 1 + d, state.Distance + d, false));
                    }
                }
                // transposition
                if (Damerau && ExtractBit(symbol, 1))
                {
                    multistate.AddState(new NfaState(state.Offset, state.Distance + 1, true));
                }
            }
            if (ExtractBit(symbol, 0))
            {
                multistate.AddState(new NfaState(state.Offset + 1, state.Distance, false));
            }
            if (state.InTranspose && ExtractBit(symbol, 0))
            {
                multistate.AddState(new NfaState(state.Offset + 2, state.Distance, false));
            }
        }

        public void Transition(MultiState current, MultiState dest, ulong shiftedChiVector)
        {
            dest.Clear();
            ulong mask = (1UL << MultistateDiameter) - 1UL;
            foreach (NfaState state in current.States)
            {
                ulong shifted = (shiftedChiVector >> state.Offset) & mask;
                SimpleTransition(state, shifted, dest);
            }
            dest.States.Sort();
        }

        /// <summary>Distance by direct NFA simulation (reference implementation for tests).</summary>
        public LevenshteinDistance ComputeDistance(string query, string other)
        {
            MultiState current = InitialStates();
            MultiState next = new MultiState();
            foreach (char c in other)
            {
                ulong chi = 0;
                for (int i = 0; i < query.Length && i < 64; i++)
                {
                    if (query[i] == c) { chi |= 1UL << i; }
                }
                Transition(current, next, chi);
                MultiState swap = current;
                current = next;
                next = swap;
            }
            return MultistateDistance(current, query.Length);
        }
    }
}
