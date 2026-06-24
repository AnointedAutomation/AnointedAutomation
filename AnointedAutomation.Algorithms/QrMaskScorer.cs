// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me

using System;

namespace AnointedAutomation.Algorithms.QrCode
{
    /// <summary>
    /// Scores a masked QR matrix using the four penalty rules of ISO/IEC 18004 §8.8.2.
    /// Lower is better; the encoder picks the mask with the lowest total penalty.
    /// </summary>
    internal static class QrMaskScorer
    {
        public static int Score(QrMatrix m)
        {
            return Rule1(m) + Rule2(m) + Rule3(m) + Rule4(m);
        }

        // Rule 1: five or more same-colour modules in a row/column.
        private static int Rule1(QrMatrix m)
        {
            int size = m.Size;
            int penalty = 0;
            for (int y = 0; y < size; y++)
            {
                penalty += RunPenalty(m, y, true);
            }
            for (int x = 0; x < size; x++)
            {
                penalty += RunPenalty(m, x, false);
            }
            return penalty;
        }

        private static int RunPenalty(QrMatrix m, int line, bool horizontal)
        {
            int size = m.Size;
            int penalty = 0;
            int runLength = 1;
            bool previous = horizontal ? m.IsDark(0, line) : m.IsDark(line, 0);
            for (int i = 1; i < size; i++)
            {
                bool current = horizontal ? m.IsDark(i, line) : m.IsDark(line, i);
                if (current == previous)
                {
                    runLength++;
                }
                else
                {
                    if (runLength >= 5)
                    {
                        penalty += 3 + (runLength - 5);
                    }
                    runLength = 1;
                    previous = current;
                }
            }
            if (runLength >= 5)
            {
                penalty += 3 + (runLength - 5);
            }
            return penalty;
        }

        // Rule 2: every 2x2 block of the same colour adds 3.
        private static int Rule2(QrMatrix m)
        {
            int size = m.Size;
            int penalty = 0;
            for (int y = 0; y < size - 1; y++)
            {
                for (int x = 0; x < size - 1; x++)
                {
                    bool c = m.IsDark(x, y);
                    if (c == m.IsDark(x + 1, y) && c == m.IsDark(x, y + 1) && c == m.IsDark(x + 1, y + 1))
                    {
                        penalty += 3;
                    }
                }
            }
            return penalty;
        }

        // Rule 3: 1:1:3:1:1 finder-like pattern with 4 light modules on a side adds 40 each.
        private static readonly bool[] PatternA = { true, false, true, true, true, false, true, false, false, false, false };
        private static readonly bool[] PatternB = { false, false, false, false, true, false, true, true, true, false, true };

        private static int Rule3(QrMatrix m)
        {
            int size = m.Size;
            int penalty = 0;
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x <= size - 11; x++)
                {
                    if (MatchesPattern(m, x, y, true))
                    {
                        penalty += 40;
                    }
                }
            }
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y <= size - 11; y++)
                {
                    if (MatchesPattern(m, x, y, false))
                    {
                        penalty += 40;
                    }
                }
            }
            return penalty;
        }

        private static bool MatchesPattern(QrMatrix m, int x, int y, bool horizontal)
        {
            bool matchA = true;
            bool matchB = true;
            for (int i = 0; i < 11; i++)
            {
                bool module = horizontal ? m.IsDark(x + i, y) : m.IsDark(x, y + i);
                if (module != PatternA[i])
                {
                    matchA = false;
                }
                if (module != PatternB[i])
                {
                    matchB = false;
                }
            }
            return matchA || matchB;
        }

        // Rule 4: deviation of the dark-module proportion from 50%.
        private static int Rule4(QrMatrix m)
        {
            int size = m.Size;
            int dark = 0;
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    if (m.IsDark(x, y))
                    {
                        dark++;
                    }
                }
            }
            int total = size * size;
            int percent = (dark * 100) / total;
            int deviation = Math.Abs(percent - 50);
            return (deviation / 5) * 10;
        }
    }
}
