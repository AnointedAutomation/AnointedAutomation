// Copyright © Anointed Automation Corporation., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Several cases are taken from tantivy levenshtein_automata src/tests.rs (test_damerau, test_simple,
// test_dead_state, test_prefix, test_prefix_dfa_1_lev, test_issue_3), translated from Rust to C#.

using System;
using System.Collections.Generic;
using System.Linq;
using AnointedAutomation.Search.Fuzzy;
using AnointedAutomation.Search.Index;
using Xunit;

namespace AnointedAutomation.Search.Tests
{
    public class FuzzyTests
    {
        private static int Levenshtein(string a, string b)
        {
            int[,] d = new int[a.Length + 1, b.Length + 1];
            for (int i = 0; i <= a.Length; i++) { d[i, 0] = i; }
            for (int j = 0; j <= b.Length; j++) { d[0, j] = j; }
            for (int i = 1; i <= a.Length; i++)
            {
                for (int j = 1; j <= b.Length; j++)
                {
                    int cost = a[i - 1] == b[j - 1] ? 0 : 1;
                    d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + cost);
                }
            }
            return d[a.Length, b.Length];
        }

        [Fact]
        public void Damerau_TantivyCases()
        {
            LevenshteinAutomatonBuilder builder = new LevenshteinAutomatonBuilder(2, true);
            Assert.Equal(LevenshteinDistance.Exact(0), builder.BuildDfa("abc").Eval("abc"));
            Assert.Equal(LevenshteinDistance.Exact(1), builder.BuildDfa("abc").Eval("abcd"));
            Assert.Equal(LevenshteinDistance.Exact(1), builder.BuildDfa("abcdef").Eval("abddef"));
            Assert.Equal(LevenshteinDistance.Exact(1), builder.BuildDfa("abcdef").Eval("abdcef"));
        }

        [Fact]
        public void TranspositionCostsTwoWithoutDamerau()
        {
            Assert.Equal(LevenshteinDistance.Exact(2), new LevenshteinAutomatonBuilder(2, false).BuildDfa("abcdef").Eval("abdcef"));
            Assert.False(new LevenshteinAutomatonBuilder(1, false).BuildDfa("abcdef").Eval("abdcef").IsExact);
        }

        [Fact]
        public void Simple_TantivyCases()
        {
            LevenshteinDfa dfa = new LevenshteinAutomatonBuilder(2, false).BuildDfa("abcdef");
            Assert.Equal(LevenshteinDistance.Exact(0), dfa.Eval("abcdef"));
            Assert.Equal(LevenshteinDistance.Exact(1), dfa.Eval("abcdf"));
            Assert.Equal(LevenshteinDistance.Exact(1), dfa.Eval("abcdgf"));
            Assert.Equal(LevenshteinDistance.Exact(1), dfa.Eval("abccdef"));
        }

        [Fact]
        public void DeadState_TantivyCase()
        {
            LevenshteinDfa dfa = new LevenshteinAutomatonBuilder(2, false).BuildDfa("abcdefghijklmnop");
            int state = dfa.InitialState;
            state = dfa.Transition(state, 'X');
            Assert.NotEqual(LevenshteinDfa.SinkState, state);
            state = dfa.Transition(state, 'X');
            Assert.NotEqual(LevenshteinDfa.SinkState, state);
            state = dfa.Transition(state, 'X');
            Assert.Equal(LevenshteinDfa.SinkState, state);
        }

        [Fact]
        public void Issue3_TantivyCase()
        {
            foreach (bool damerau in new[] { false, true })
            {
                for (int d = 0; d < 3; d++)
                {
                    Assert.Equal(LevenshteinDistance.Exact(0), new LevenshteinAutomatonBuilder(d, damerau).BuildDfa("oreiller").Eval("oreiller"));
                }
            }
        }

        [Fact]
        public void Prefix_TantivyCases()
        {
            LevenshteinDfa exact = new LevenshteinAutomatonBuilder(0, false).BuildPrefixDfa("abc");
            Assert.Equal(LevenshteinDistance.Exact(0), exact.Eval("abc"));
            Assert.False(exact.Eval("a").IsExact);
            Assert.False(exact.Eval("ab").IsExact);
            for (int d = 3; d < 10; d++)
            {
                Assert.Equal(LevenshteinDistance.Exact(0), exact.Eval("abcdefghij".Substring(0, d)));
            }
            LevenshteinAutomatonBuilder one = new LevenshteinAutomatonBuilder(1, false);
            Assert.Equal(LevenshteinDistance.Exact(1), one.BuildPrefixDfa("a").Eval("b"));
            Assert.Equal(LevenshteinDistance.Exact(0), one.BuildPrefixDfa("a").Eval("abc"));
            Assert.Equal(LevenshteinDistance.Exact(1), one.BuildPrefixDfa("masup").Eval("marsupial"));
            Assert.Equal(LevenshteinDistance.Exact(1), one.BuildPrefixDfa("mas").Eval("mars"));
        }

        [Fact]
        public void MatchesBruteForceLevenshtein()
        {
            char[] alphabet = { 'a', 'b', 'c' };
            List<string> words = new List<string> { string.Empty };
            for (int len = 1; len <= 4; len++)
            {
                List<string> next = new List<string>();
                foreach (string w in words.Where(x => x.Length == len - 1))
                {
                    foreach (char c in alphabet) { next.Add(w + c); }
                }
                words.AddRange(next);
            }
            Random random = new Random(42);
            for (int max = 0; max <= 2; max++)
            {
                LevenshteinAutomatonBuilder builder = new LevenshteinAutomatonBuilder(max, false);
                for (int trial = 0; trial < 60; trial++)
                {
                    string query = words[random.Next(words.Count)];
                    LevenshteinDfa dfa = builder.BuildDfa(query);
                    foreach (string candidate in words)
                    {
                        int expected = Levenshtein(query, candidate);
                        LevenshteinDistance actual = dfa.Eval(candidate);
                        if (expected <= max)
                        {
                            Assert.Equal(LevenshteinDistance.Exact(expected), actual);
                        }
                        else
                        {
                            Assert.False(actual.IsExact, query + " vs " + candidate);
                        }
                    }
                }
            }
        }

        [Fact]
        public void Enumerator_FindsTermsWithinBudgetHonoringPrefix()
        {
            TermDictionary dictionary = new TermDictionary(new[] { "underwear", "underwire", "undershirt", "hoodie", "hoodies", "wonder", "kunderwear" });
            List<FuzzyMatch> matches = FuzzyTermEnumerator.Enumerate(dictionary, "underware", 2, true, 1);
            string[] terms = matches.Select(m => m.Term).OrderBy(x => x, StringComparer.Ordinal).ToArray();
            Assert.Equal(new[] { "underwear", "underwire" }, terms);
            Assert.Equal(2, matches.Single(m => string.Equals(m.Term, "underwear", StringComparison.Ordinal)).Distance);
            Assert.Equal(1, matches.Single(m => string.Equals(m.Term, "underwire", StringComparison.Ordinal)).Distance);

            List<FuzzyMatch> transposed = FuzzyTermEnumerator.Enumerate(dictionary, "hoodei", 1, true, 1);
            Assert.Equal(new[] { "hoodie" }, transposed.Select(m => m.Term).ToArray());

            // Without the fixed prefix, a first letter change is allowed.
            List<FuzzyMatch> free = FuzzyTermEnumerator.Enumerate(dictionary, "xoodie", 1, true, 0);
            Assert.Equal(new[] { "hoodie" }, free.Select(m => m.Term).ToArray());
            Assert.Empty(FuzzyTermEnumerator.Enumerate(dictionary, "xoodie", 1, true, 1));
        }

        [Fact]
        public void TermDictionary_PrefixRange()
        {
            TermDictionary dictionary = new TermDictionary(new[] { "b", "ab", "abc", "abd", "ac", "ab" });
            Assert.Equal(5, dictionary.Count);
            int lo;
            int hi;
            dictionary.PrefixRange("ab", out lo, out hi);
            Assert.Equal(new[] { "ab", "abc", "abd" }, Enumerable.Range(lo, hi - lo).Select(i => dictionary.Terms[i]).ToArray());
            Assert.True(dictionary.Contains("ac"));
            Assert.False(dictionary.Contains("a"));
        }


        [Fact]
        public void Builder_RejectsUnsupportedDistance()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new LevenshteinAutomatonBuilder(3, true));
        }
    }
}
