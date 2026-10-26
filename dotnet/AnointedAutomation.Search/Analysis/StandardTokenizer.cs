// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Ported from Apache Lucene lucene/core/src/java/org/apache/lucene/analysis/standard/StandardTokenizer.java
// (with StandardTokenizerImpl.jflex, the UAX#29 word break grammar) and tantivy
// src/tokenizer/simple_tokenizer.rs. Translated from Java and Rust to C#: the JFlex state machine is
// replaced by a hand written scanner for the UAX#29 rules that matter for search (letters and digits form
// words; an apostrophe or period joins letters, a period or comma joins digits, an underscore joins
// anything; ideographs are one token each; long tokens split at the maximum length).

using System;
using System.Collections.Generic;
using System.Globalization;

namespace AnointedAutomation.Search.Analysis
{
    /// <summary>Unicode word tokenizer modeled on Lucene's StandardTokenizer (UAX#29 word boundaries).</summary>
    public sealed class StandardTokenizer : ITokenizer
    {
        /// <summary>Lucene's StandardAnalyzer.DEFAULT_MAX_TOKEN_LENGTH.</summary>
        public const int DefaultMaxTokenLength = 255;

        private readonly int _maxTokenLength;

        /// <param name="maxTokenLength">Longer runs are split into pieces of this length (Lucene behavior).</param>
        public StandardTokenizer(int maxTokenLength)
        {
            if (maxTokenLength < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(maxTokenLength));
            }
            _maxTokenLength = maxTokenLength;
        }

        public List<Token> Tokenize(string text)
        {
            if (text is null)
            {
                throw new ArgumentNullException(nameof(text));
            }
            List<Token> tokens = new List<Token>();
            int position = 0;
            int i = 0;
            while (i < text.Length)
            {
                char c = text[i];
                if (IsIdeographic(c))
                {
                    tokens.Add(new Token(text.Substring(i, 1), position, 1, i, i + 1, TokenType.Ideographic));
                    position++;
                    i++;
                    continue;
                }
                if (!IsWordChar(c))
                {
                    i++;
                    continue;
                }
                int start = i;
                bool hasLetter = false;
                bool hasDigit = false;
                while (i < text.Length)
                {
                    char ch = text[i];
                    if (IsIdeographic(ch))
                    {
                        break;
                    }
                    if (IsWordChar(ch))
                    {
                        if (char.IsDigit(ch)) { hasDigit = true; } else if (char.IsLetter(ch)) { hasLetter = true; }
                        i++;
                        continue;
                    }
                    if (i + 1 < text.Length && i > start && IsJoiner(text[i - 1], ch, text[i + 1]))
                    {
                        i++;
                        continue;
                    }
                    break;
                }
                TokenType type = hasLetter && hasDigit ? TokenType.Alphanumeric : (hasDigit ? TokenType.Number : TokenType.Word);
                for (int s = start; s < i; s += _maxTokenLength)
                {
                    int len = Math.Min(_maxTokenLength, i - s);
                    tokens.Add(new Token(text.Substring(s, len), position, 1, s, s + len, type));
                    position++;
                }
            }
            return tokens;
        }

        /// <summary>UAX#29 MidLetter / MidNum / MidNumLet / ExtendNumLet joins (WB6, WB7, WB11, WB12, WB13a/b).</summary>
        private static bool IsJoiner(char before, char c, char after)
        {
            if (!IsWordChar(after) || IsIdeographic(after))
            {
                return false;
            }
            bool letters = char.IsLetter(before) && char.IsLetter(after);
            bool digits = char.IsDigit(before) && char.IsDigit(after);
            switch (c)
            {
                case '_':
                    return true;
                case '\'':
                case '’':
                case ':':
                    return letters;
                case '.':
                    return letters || digits;
                case ',':
                case ';':
                    return digits;
                default:
                    return false;
            }
        }

        private static bool IsWordChar(char c)
        {
            if (char.IsLetterOrDigit(c))
            {
                return true;
            }
            UnicodeCategory cat = CharUnicodeInfo.GetUnicodeCategory(c);
            return cat == UnicodeCategory.NonSpacingMark
                || cat == UnicodeCategory.SpacingCombiningMark
                || cat == UnicodeCategory.EnclosingMark
                || cat == UnicodeCategory.LetterNumber
                || cat == UnicodeCategory.OtherNumber;
        }

        /// <summary>Han, Hiragana and Katakana: Lucene emits each as its own token.</summary>
        private static bool IsIdeographic(char c)
        {
            return (c >= '぀' && c <= 'ヿ')
                || (c >= '㐀' && c <= '䶿')
                || (c >= '一' && c <= '鿿')
                || (c >= '豈' && c <= '﫿');
        }
    }
}
