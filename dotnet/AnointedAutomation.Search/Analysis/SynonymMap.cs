// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Ported from Apache Lucene lucene/analysis/common/src/java/org/apache/lucene/analysis/synonym/SynonymMap.java
// (SynonymMap.Builder). Translated from Java to C#: the FST of input word sequences became a dictionary
// keyed by the space joined analyzed words, with the longest input length kept for greedy matching.

using System;
using System.Collections.Generic;

namespace AnointedAutomation.Search.Analysis
{
    /// <summary>Caller supplied synonyms. A group is a set of equivalent phrases (every phrase maps to every
    /// other, Solr "a, b, c" expand=true); a one way rule maps an input to outputs ("a =&gt; b"). Phrases are
    /// analyzed with the analyzer given to <see cref="Build"/>, so they match the indexed form.</summary>
    public sealed class SynonymMap
    {
        private readonly Dictionary<string, List<string[]>> _map;

        private SynonymMap(Dictionary<string, List<string[]>> map, int maxInputWords)
        {
            _map = map;
            MaxInputWords = maxInputWords;
        }

        /// <summary>The longest input phrase, in words.</summary>
        public int MaxInputWords { get; }

        public bool IsEmpty => _map.Count == 0;

        /// <summary>The outputs (each a word sequence) for an input word sequence; empty when none.</summary>
        public IReadOnlyList<string[]> Lookup(IReadOnlyList<string> words, int start, int count)
        {
            string key = Key(words, start, count);
            List<string[]> outputs;
            if (_map.TryGetValue(key, out outputs))
            {
                return outputs;
            }
            return Array.Empty<string[]>();
        }

        private static string Key(IReadOnlyList<string> words, int start, int count)
        {
            if (count == 1)
            {
                return words[start];
            }
            string[] parts = new string[count];
            for (int i = 0; i < count; i++)
            {
                parts[i] = words[start + i];
            }
            return string.Join(" ", parts);
        }

        /// <summary>Builds a map from equivalence groups and one way rules, analyzing every phrase.</summary>
        public static SynonymMap Build(TextAnalyzer analyzer, IEnumerable<IReadOnlyList<string>> groups, IEnumerable<KeyValuePair<string, IReadOnlyList<string>>> oneWay)
        {
            if (analyzer is null)
            {
                throw new ArgumentNullException(nameof(analyzer));
            }
            if (groups is null)
            {
                throw new ArgumentNullException(nameof(groups));
            }
            if (oneWay is null)
            {
                throw new ArgumentNullException(nameof(oneWay));
            }
            Dictionary<string, List<string[]>> map = new Dictionary<string, List<string[]>>(StringComparer.Ordinal);
            Dictionary<string, HashSet<string>> seen = new Dictionary<string, HashSet<string>>(StringComparer.Ordinal);
            int maxInput = 0;
            foreach (IReadOnlyList<string> group in groups)
            {
                List<string[]> phrases = new List<string[]>();
                foreach (string phrase in group)
                {
                    string[] words = Words(analyzer, phrase);
                    if (words.Length > 0)
                    {
                        phrases.Add(words);
                    }
                }
                foreach (string[] input in phrases)
                {
                    foreach (string[] output in phrases)
                    {
                        if (!ReferenceEquals(input, output))
                        {
                            maxInput = Math.Max(maxInput, Add(map, seen, input, output));
                        }
                    }
                }
            }
            foreach (KeyValuePair<string, IReadOnlyList<string>> rule in oneWay)
            {
                string[] input = Words(analyzer, rule.Key);
                if (input.Length == 0)
                {
                    continue;
                }
                foreach (string phrase in rule.Value)
                {
                    string[] output = Words(analyzer, phrase);
                    if (output.Length > 0)
                    {
                        maxInput = Math.Max(maxInput, Add(map, seen, input, output));
                    }
                }
            }
            return new SynonymMap(map, maxInput);
        }

        /// <summary>An empty map.</summary>
        public static SynonymMap Empty { get; } = new SynonymMap(new Dictionary<string, List<string[]>>(StringComparer.Ordinal), 0);

        private static int Add(Dictionary<string, List<string[]>> map, Dictionary<string, HashSet<string>> seen, string[] input, string[] output)
        {
            string key = string.Join(" ", input);
            string value = string.Join(" ", output);
            if (string.Equals(key, value, StringComparison.Ordinal))
            {
                return 0;
            }
            HashSet<string> values;
            if (!seen.TryGetValue(key, out values))
            {
                values = new HashSet<string>(StringComparer.Ordinal);
                seen[key] = values;
                map[key] = new List<string[]>();
            }
            if (values.Add(value))
            {
                map[key].Add(output);
            }
            return input.Length;
        }

        /// <summary>The main path words of an analyzed phrase (<see cref="TokenGraph.MainPath"/>).</summary>
        private static string[] Words(TextAnalyzer analyzer, string phrase)
        {
            return TokenGraph.MainPath(analyzer.Analyze(phrase)).ToArray();
        }
    }
}
