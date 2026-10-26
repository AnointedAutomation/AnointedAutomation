// Copyright © Anointed Automation Corporation., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Ported from Apache Lucene lucene/analysis/common/src/java/org/apache/lucene/analysis/miscellaneous/WordDelimiterIterator.java.
// Translated from Java to C#: Character.getType became CharUnicodeInfo.GetUnicodeCategory; the logic is unchanged.

using System.Globalization;

namespace AnointedAutomation.Search.Analysis
{
    /// <summary>Iterates the subwords of a token by the word delimiter rules (case changes, letter to digit
    /// changes, delimiter characters, English possessives).</summary>
    internal sealed class WordDelimiterIterator
    {
        internal const int Lower = 0x01;
        internal const int Upper = 0x02;
        internal const int Digit = 0x04;
        internal const int SubwordDelim = 0x08;
        internal const int Alpha = 0x03;
        internal const int AlphaNum = 0x07;
        internal const int Done = -1;

        private static readonly byte[] DefaultTable = BuildDefaultTable();

        private char[] _text;
        private int _length;
        private int _startBounds;
        private int _endBounds;
        internal int Current;
        internal int End;
        private bool _hasFinalPossessive;
        private readonly bool _splitOnCaseChange;
        private readonly bool _splitOnNumerics;
        private readonly bool _stemEnglishPossessive;
        private bool _skipPossessive;

        internal WordDelimiterIterator(bool splitOnCaseChange, bool splitOnNumerics, bool stemEnglishPossessive)
        {
            _splitOnCaseChange = splitOnCaseChange;
            _splitOnNumerics = splitOnNumerics;
            _stemEnglishPossessive = stemEnglishPossessive;
        }

        private static byte[] BuildDefaultTable()
        {
            byte[] tab = new byte[256];
            for (int i = 0; i < 256; i++)
            {
                char c = (char)i;
                byte code = 0;
                if (char.IsLower(c))
                {
                    code |= Lower;
                }
                else if (char.IsUpper(c))
                {
                    code |= Upper;
                }
                else if (char.IsDigit(c))
                {
                    code |= Digit;
                }
                if (code == 0)
                {
                    code = SubwordDelim;
                }
                tab[i] = code;
            }
            return tab;
        }

        internal int Next()
        {
            Current = End;
            if (Current == Done)
            {
                return Done;
            }
            if (_skipPossessive)
            {
                Current += 2;
                _skipPossessive = false;
            }
            int lastType = 0;
            while (Current < _endBounds && IsSubwordDelim(lastType = CharType(_text[Current])))
            {
                Current++;
            }
            if (Current >= _endBounds)
            {
                return End = Done;
            }
            for (End = Current + 1; End < _endBounds; End++)
            {
                int type = CharType(_text[End]);
                if (IsBreak(lastType, type))
                {
                    break;
                }
                lastType = type;
            }
            if (End < _endBounds - 1 && EndsWithPossessive(End + 2))
            {
                _skipPossessive = true;
            }
            return End;
        }

        internal int Type()
        {
            if (End == Done)
            {
                return 0;
            }
            int type = CharType(_text[Current]);
            switch (type)
            {
                case Lower:
                case Upper:
                    return Alpha;
                default:
                    return type;
            }
        }

        internal void SetText(char[] text, int length)
        {
            _text = text;
            _length = _endBounds = length;
            Current = _startBounds = End = 0;
            _skipPossessive = _hasFinalPossessive = false;
            SetBounds();
        }

        private bool IsBreak(int lastType, int type)
        {
            if ((type & lastType) != 0)
            {
                return false;
            }
            if (!_splitOnCaseChange && IsAlpha(lastType) && IsAlpha(type))
            {
                return false;
            }
            else if (IsUpper(lastType) && IsAlpha(type))
            {
                return false;
            }
            else if (!_splitOnNumerics && ((IsAlpha(lastType) && IsDigit(type)) || (IsDigit(lastType) && IsAlpha(type))))
            {
                return false;
            }
            return true;
        }

        internal bool IsSingleWord()
        {
            if (_hasFinalPossessive)
            {
                return Current == _startBounds && End == _endBounds - 2;
            }
            return Current == _startBounds && End == _endBounds;
        }

        private void SetBounds()
        {
            while (_startBounds < _length && IsSubwordDelim(CharType(_text[_startBounds])))
            {
                _startBounds++;
            }
            while (_endBounds > _startBounds && IsSubwordDelim(CharType(_text[_endBounds - 1])))
            {
                _endBounds--;
            }
            if (EndsWithPossessive(_endBounds))
            {
                _hasFinalPossessive = true;
            }
            Current = _startBounds;
        }

        private bool EndsWithPossessive(int pos)
        {
            return _stemEnglishPossessive
                && pos > 2
                && _text[pos - 2] == '\''
                && (_text[pos - 1] == 's' || _text[pos - 1] == 'S')
                && IsAlpha(CharType(_text[pos - 3]))
                && (pos == _endBounds || IsSubwordDelim(CharType(_text[pos])));
        }

        private static int CharType(char ch)
        {
            if (ch < DefaultTable.Length)
            {
                return DefaultTable[ch];
            }
            return GetType(ch);
        }

        internal static int GetType(char ch)
        {
            switch (CharUnicodeInfo.GetUnicodeCategory(ch))
            {
                case UnicodeCategory.UppercaseLetter:
                    return Upper;
                case UnicodeCategory.LowercaseLetter:
                    return Lower;
                case UnicodeCategory.TitlecaseLetter:
                case UnicodeCategory.ModifierLetter:
                case UnicodeCategory.OtherLetter:
                case UnicodeCategory.NonSpacingMark:
                case UnicodeCategory.EnclosingMark:
                case UnicodeCategory.SpacingCombiningMark:
                    return Alpha;
                case UnicodeCategory.DecimalDigitNumber:
                case UnicodeCategory.LetterNumber:
                case UnicodeCategory.OtherNumber:
                    return Digit;
                case UnicodeCategory.Surrogate:
                    return Alpha | Digit;
                default:
                    return SubwordDelim;
            }
        }

        internal static bool IsAlpha(int type) => (type & Alpha) != 0;

        internal static bool IsDigit(int type) => (type & Digit) != 0;

        internal static bool IsSubwordDelim(int type) => (type & SubwordDelim) != 0;

        internal static bool IsUpper(int type) => (type & Upper) != 0;
    }
}
