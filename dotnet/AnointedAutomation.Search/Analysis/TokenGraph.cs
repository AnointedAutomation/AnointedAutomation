// Copyright © Anointed Automation Corporation., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Ported from Apache Lucene lucene/core/src/java/org/apache/lucene/util/graph/GraphTokenStreamFiniteStrings.java
// (walking one path of a token graph). Translated from Java to C#: picks a single path for query words.

using System;
using System.Collections.Generic;

namespace AnointedAutomation.Search.Analysis
{
    /// <summary>Token graph helpers.</summary>
    public static class TokenGraph
    {
        /// <summary>One word per position along the "parts" path: a single position token that is not a
        /// catenation; else a catenation spanning the parts (whose positions are skipped); else the first
        /// token. "cover-up" gives cover, up; "t-shirt" (the "t" dropped) gives tshirt.</summary>
        public static List<string> MainPath(List<Token> tokens)
        {
            List<Token> path = MainPathTokens(tokens);
            List<string> words = new List<string>(path.Count);
            foreach (Token t in path)
            {
                words.Add(t.Term);
            }
            return words;
        }

        /// <summary>The tokens of <see cref="MainPath"/>.</summary>
        public static List<Token> MainPathTokens(List<Token> tokens)
        {
            if (tokens is null)
            {
                throw new ArgumentNullException(nameof(tokens));
            }
            SortedDictionary<int, List<Token>> byPos = new SortedDictionary<int, List<Token>>();
            foreach (Token t in tokens)
            {
                List<Token> list;
                if (!byPos.TryGetValue(t.Position, out list))
                {
                    list = new List<Token>();
                    byPos[t.Position] = list;
                }
                list.Add(t);
            }
            List<Token> words = new List<Token>();
            int skipUntil = -1;
            foreach (KeyValuePair<int, List<Token>> pair in byPos)
            {
                if (pair.Key < skipUntil)
                {
                    continue;
                }
                Token chosen = null;
                foreach (Token t in pair.Value)
                {
                    if (t.PositionLength == 1 && t.Type != TokenType.Catenation && t.Type != TokenType.Synonym)
                    {
                        chosen = t;
                        break;
                    }
                }
                if (chosen is null)
                {
                    foreach (Token t in pair.Value)
                    {
                        if (t.Type == TokenType.Catenation)
                        {
                            chosen = t;
                            break;
                        }
                    }
                    // A catenation that starts glued to the word just taken ("3d-effect": "3", then "deffect"
                    // from the dropped "d") begins inside a word split on a number or case change. Taking it
                    // would swallow the real words after it ("effect"), so the position is skipped instead,
                    // and "3d-effect" reads like "3d effect".
                    if (chosen is not null && words.Count > 0 && words[words.Count - 1].EndOffset == chosen.StartOffset)
                    {
                        continue;
                    }
                }
                if (chosen is null)
                {
                    chosen = pair.Value[0];
                }
                words.Add(chosen);
                skipUntil = chosen.Position + chosen.PositionLength;
            }
            return words;
        }
    }
}
