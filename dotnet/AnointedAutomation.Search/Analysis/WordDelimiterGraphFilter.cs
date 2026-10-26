// Copyright © Anointed Automation Corporation., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Ported from Apache Lucene lucene/analysis/common/src/java/org/apache/lucene/analysis/miscellaneous/WordDelimiterGraphFilter.java.
// Translated from Java to C#: the flag int became WordDelimiterOptions, the attribute stream became a
// token list with absolute positions; bufferWordParts, the concatenation rules and the part sort (start
// offset ascending, end offset descending, stable) are unchanged. Protected words and custom char type
// tables are not ported.

using System;
using System.Collections.Generic;
using System.Text;

namespace AnointedAutomation.Search.Analysis
{
    /// <summary>Which word delimiter outputs to produce (Lucene's WordDelimiterGraphFilter flags).</summary>
    public sealed class WordDelimiterOptions
    {
        public bool GenerateWordParts { get; init; }
        public bool GenerateNumberParts { get; init; }
        public bool CatenateWords { get; init; }
        public bool CatenateNumbers { get; init; }
        public bool CatenateAll { get; init; }
        public bool PreserveOriginal { get; init; }
        public bool SplitOnCaseChange { get; init; }
        public bool SplitOnNumerics { get; init; }
        public bool StemEnglishPossessive { get; init; }
    }

    /// <summary>Splits words into subwords and emits catenations as a token graph (position lengths).</summary>
    public sealed class WordDelimiterGraphFilter : ITokenFilter
    {
        private readonly WordDelimiterOptions _options;

        public WordDelimiterGraphFilter(WordDelimiterOptions options)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            _options = options;
        }

        private sealed class Part
        {
            public string Term;
            public int StartPos;
            public int EndPos;
            public int StartPart;
            public int EndPart;
            public int Order;
        }

        private sealed class Concatenation
        {
            public readonly StringBuilder Buffer = new StringBuilder();
            public int StartPart;
            public int EndPart;
            public int StartPos;
            public int Type;
            public int SubwordCount;

            public bool IsEmpty => Buffer.Length == 0;

            public void Append(char[] text, int offset, int length)
            {
                Buffer.Append(text, offset, length);
                SubwordCount++;
            }

            public void Clear()
            {
                Buffer.Clear();
                StartPart = EndPart = Type = SubwordCount = 0;
            }
        }

        /// <summary>Per call state (the filter itself stays immutable and thread safe).</summary>
        private sealed class State
        {
            public readonly List<Part> Parts = new List<Part>();
            public readonly Concatenation Concat = new Concatenation();
            public readonly Concatenation ConcatAll = new Concatenation();
            public WordDelimiterIterator Iterator;
            public char[] Text;
            public int WordPos;
            public int LastConcatCount;
        }

        public List<Token> Apply(List<Token> tokens)
        {
            if (tokens is null)
            {
                throw new ArgumentNullException(nameof(tokens));
            }
            List<Token> output = new List<Token>(tokens.Count * 2);
            State state = new State
            {
                Iterator = new WordDelimiterIterator(_options.SplitOnCaseChange, _options.SplitOnNumerics, _options.StemEnglishPossessive),
            };
            int lastOld = -1;
            int lastNew = -1;
            int lastSpan = 1;
            foreach (Token token in tokens)
            {
                int basePos;
                if (token.Position == lastOld)
                {
                    basePos = lastNew;
                }
                else
                {
                    basePos = lastOld < 0 ? token.Position : lastNew + lastSpan + (token.Position - lastOld - 1);
                    lastOld = token.Position;
                    lastNew = basePos;
                    lastSpan = 1;
                }
                int span = Split(token, basePos, state, output);
                if (span > lastSpan)
                {
                    lastSpan = span;
                }
            }
            return output;
        }

        /// <summary>Emits the token's parts at <paramref name="basePos"/>; returns the positions consumed (0 when dropped).</summary>
        private int Split(Token token, int basePos, State st, List<Token> output)
        {
            char[] text = token.Term.ToCharArray();
            int termLength = text.Length;
            WordDelimiterIterator iterator = st.Iterator;
            iterator.SetText(text, termLength);
            iterator.Next();
            if (iterator.Current == 0 && iterator.End == termLength)
            {
                output.Add(token.WithPosition(basePos, token.PositionLength));
                return token.PositionLength;
            }
            if (iterator.End == WordDelimiterIterator.Done)
            {
                if (!_options.PreserveOriginal)
                {
                    return 1;
                }
                output.Add(token.WithPosition(basePos, token.PositionLength));
                return token.PositionLength;
            }

            st.Text = text;
            st.Parts.Clear();
            st.Concat.Clear();
            st.ConcatAll.Clear();
            st.WordPos = 0;
            st.LastConcatCount = 0;
            Part original = null;
            if (_options.PreserveOriginal)
            {
                original = Buffer(st, null, 0, 1, 0, termLength);
            }
            if (iterator.IsSingleWord())
            {
                Buffer(st, null, st.WordPos, st.WordPos + 1, iterator.Current, iterator.End);
                st.WordPos++;
                iterator.Next();
            }
            else
            {
                while (iterator.End != WordDelimiterIterator.Done)
                {
                    int wordType = iterator.Type();
                    if (!st.Concat.IsEmpty && (st.Concat.Type & wordType) == 0)
                    {
                        FlushConcatenation(st, st.Concat);
                    }
                    if (ShouldConcatenate(wordType))
                    {
                        Concatenate(st, st.Concat);
                    }
                    if (_options.CatenateAll)
                    {
                        Concatenate(st, st.ConcatAll);
                    }
                    if (ShouldGenerateParts(wordType))
                    {
                        Buffer(st, null, st.WordPos, st.WordPos + 1, iterator.Current, iterator.End);
                        st.WordPos++;
                    }
                    iterator.Next();
                }
                if (!st.Concat.IsEmpty)
                {
                    FlushConcatenation(st, st.Concat);
                }
                if (!st.ConcatAll.IsEmpty)
                {
                    if (st.ConcatAll.SubwordCount > st.LastConcatCount)
                    {
                        if (st.WordPos == st.ConcatAll.StartPos)
                        {
                            st.WordPos++;
                        }
                        Write(st, st.ConcatAll);
                    }
                    st.ConcatAll.Clear();
                }
            }
            if (original is not null)
            {
                if (st.WordPos == 0)
                {
                    st.WordPos++;
                }
                original.EndPos = st.WordPos;
            }

            int first = original is not null ? 1 : 0;
            List<Part> sorted = st.Parts.GetRange(first, st.Parts.Count - first);
            sorted.Sort(ComparePart);
            int span = Math.Max(1, st.WordPos);
            if (original is not null)
            {
                Emit(token, original, basePos, output);
            }
            foreach (Part part in sorted)
            {
                Emit(token, part, basePos, output);
            }
            return span;
        }

        private static void Emit(Token token, Part part, int basePos, List<Token> output)
        {
            string term = part.Term is not null ? part.Term : token.Term.Substring(part.StartPart, part.EndPart - part.StartPart);
            bool adjust = token.EndOffset - token.StartOffset == token.Term.Length;
            int start = adjust ? token.StartOffset + part.StartPart : token.StartOffset;
            int end = adjust ? token.StartOffset + part.EndPart : token.EndOffset;
            TokenType type = part.Term is not null ? TokenType.Catenation : token.Type;
            output.Add(new Token(term, basePos + part.StartPos, Math.Max(1, part.EndPos - part.StartPos), start, end, type));
        }

        /// <summary>Smaller start offset first, then longer end offset; stable.</summary>
        private static int ComparePart(Part a, Part b)
        {
            int cmp = a.StartPart.CompareTo(b.StartPart);
            if (cmp != 0)
            {
                return cmp;
            }
            cmp = b.EndPart.CompareTo(a.EndPart);
            if (cmp != 0)
            {
                return cmp;
            }
            return a.Order.CompareTo(b.Order);
        }

        private static Part Buffer(State st, string term, int startPos, int endPos, int startPart, int endPart)
        {
            Part part = new Part
            {
                Term = term,
                StartPos = startPos,
                EndPos = endPos,
                StartPart = startPart,
                EndPart = endPart,
                Order = st.Parts.Count,
            };
            st.Parts.Add(part);
            return part;
        }

        private void FlushConcatenation(State st, Concatenation concat)
        {
            if (st.WordPos == concat.StartPos)
            {
                st.WordPos++;
            }
            st.LastConcatCount = concat.SubwordCount;
            if (concat.SubwordCount != 1 || !ShouldGenerateParts(concat.Type))
            {
                Write(st, concat);
            }
            concat.Clear();
        }

        private static void Write(State st, Concatenation concat)
        {
            Buffer(st, concat.Buffer.ToString(), concat.StartPos, st.WordPos, concat.StartPart, concat.EndPart);
        }

        private bool ShouldConcatenate(int wordType)
        {
            return (_options.CatenateWords && WordDelimiterIterator.IsAlpha(wordType))
                || (_options.CatenateNumbers && WordDelimiterIterator.IsDigit(wordType));
        }

        private bool ShouldGenerateParts(int wordType)
        {
            return (_options.GenerateWordParts && WordDelimiterIterator.IsAlpha(wordType))
                || (_options.GenerateNumberParts && WordDelimiterIterator.IsDigit(wordType));
        }

        private static void Concatenate(State st, Concatenation concatenation)
        {
            WordDelimiterIterator iterator = st.Iterator;
            if (concatenation.IsEmpty)
            {
                concatenation.Type = iterator.Type();
                concatenation.StartPart = iterator.Current;
                concatenation.StartPos = st.WordPos;
            }
            concatenation.Append(st.Text, iterator.Current, iterator.End - iterator.Current);
            concatenation.EndPart = iterator.End;
        }
    }
}
