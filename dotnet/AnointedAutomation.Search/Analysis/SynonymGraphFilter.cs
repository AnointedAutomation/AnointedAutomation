// Copyright © Anointed Automation Corporation., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Ported from Apache Lucene lucene/analysis/common/src/java/org/apache/lucene/analysis/synonym/SynonymGraphFilter.java.
// Translated from Java to C#: greedy longest match of input word sequences over the main path; every
// output phrase is emitted at the match start with type Synonym. A single word output spans the whole
// matched input (positionLength = input words); a multi word output lays its words on consecutive
// positions from the match start. Lucene's side paths for multi word outputs are not reproduced.

using System;
using System.Collections.Generic;
using System.Linq;

namespace AnointedAutomation.Search.Analysis
{
    /// <summary>Injects synonyms from a <see cref="SynonymMap"/>, keeping the original tokens.</summary>
    public sealed class SynonymGraphFilter : ITokenFilter
    {
        private readonly SynonymMap _map;

        public SynonymGraphFilter(SynonymMap map)
        {
            if (map is null)
            {
                throw new ArgumentNullException(nameof(map));
            }
            _map = map;
        }

        public List<Token> Apply(List<Token> tokens)
        {
            if (tokens is null)
            {
                throw new ArgumentNullException(nameof(tokens));
            }
            if (_map.IsEmpty || tokens.Count == 0)
            {
                return tokens;
            }
            List<Token> units = new List<Token>();
            List<string> words = new List<string>();
            int last = -1;
            foreach (Token t in tokens)
            {
                if (t.Position != last)
                {
                    units.Add(t);
                    words.Add(t.Term);
                    last = t.Position;
                }
            }
            Dictionary<int, List<Token>> injected = new Dictionary<int, List<Token>>();
            int u = 0;
            while (u < units.Count)
            {
                int matched = 0;
                IReadOnlyList<string[]> outputs = Array.Empty<string[]>();
                for (int len = Math.Min(_map.MaxInputWords, units.Count - u); len >= 1; len--)
                {
                    if (!Contiguous(units, u, len))
                    {
                        continue;
                    }
                    IReadOnlyList<string[]> found = _map.Lookup(words, u, len);
                    if (found.Count > 0)
                    {
                        matched = len;
                        outputs = found;
                        break;
                    }
                }
                if (matched == 0)
                {
                    u++;
                    continue;
                }
                Token first = units[u];
                Token lastUnit = units[u + matched - 1];
                List<Token> list = new List<Token>();
                foreach (string[] output in outputs)
                {
                    if (output.Length == 1)
                    {
                        list.Add(new Token(output[0], first.Position, matched, first.StartOffset, lastUnit.EndOffset, TokenType.Synonym));
                        continue;
                    }
                    for (int w = 0; w < output.Length; w++)
                    {
                        list.Add(new Token(output[w], first.Position + w, 1, first.StartOffset, lastUnit.EndOffset, TokenType.Synonym));
                    }
                }
                injected[first.Position] = list;
                u += matched;
            }
            if (injected.Count == 0)
            {
                return tokens;
            }
            List<Token> merged = new List<Token>(tokens.Count + injected.Count * 2);
            for (int i = 0; i < tokens.Count; i++)
            {
                merged.Add(tokens[i]);
                bool lastAtPosition = i + 1 == tokens.Count || tokens[i + 1].Position != tokens[i].Position;
                List<Token> add;
                if (lastAtPosition && injected.TryGetValue(tokens[i].Position, out add))
                {
                    merged.AddRange(add);
                }
            }
            return merged.OrderBy(x => x.Position).ToList();
        }

        private static bool Contiguous(List<Token> units, int start, int len)
        {
            for (int i = 1; i < len; i++)
            {
                if (units[start + i].Position != units[start + i - 1].Position + 1)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
