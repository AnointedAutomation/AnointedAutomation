// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Ported from Apache Lucene lucene/analysis/common/src/java/org/apache/lucene/analysis/util/OpenStringBuilder.java.
// Translated from Java to C#: only the members KStemmer uses.

using System;

namespace AnointedAutomation.Search.Analysis
{
    /// <summary>A growable char buffer with direct index access, the working word of <see cref="KStemmer"/>.</summary>
    internal sealed class KStemBuffer
    {
        private char[] _buf = new char[32];
        private int _len;

        public int Length => _len;

        public char CharAt(int index) => _buf[index];

        public void SetCharAt(int index, char ch) => _buf[index] = ch;

        public void SetLength(int length) => _len = length;

        public void Reset() => _len = 0;

        public void Reserve(int num)
        {
            if (_len + num > _buf.Length)
            {
                Array.Resize(ref _buf, Math.Max(_buf.Length * 2, _len + num));
            }
        }

        /// <summary>Appends without a capacity check beyond a resize (KStemmer reserves len + 10 up front).</summary>
        public void UnsafeWrite(char ch)
        {
            Reserve(1);
            _buf[_len++] = ch;
        }

        public void Append(string s)
        {
            Reserve(s.Length);
            for (int i = 0; i < s.Length; i++)
            {
                _buf[_len++] = s[i];
            }
        }

        public ReadOnlySpan<char> AsSpan() => new ReadOnlySpan<char>(_buf, 0, _len);

        public override string ToString() => new string(_buf, 0, _len);
    }
}
