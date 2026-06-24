// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me

using System;
using System.Collections.Generic;
using System.Text;

namespace AnointedAutomation.Algorithms.QrCode
{
    /// <summary>
    /// Encodes text into a QR <see cref="QrMatrix"/> using byte mode (ISO/IEC 18004).
    /// Handles version selection, Reed–Solomon error correction, block interleaving, function
    /// patterns, data placement, automatic mask selection, and format/version information.
    /// </summary>
    public static class QrEncoder
    {
        private const int ByteModeIndicator = 0b0100;
        private static readonly byte[] PadBytes = { 0xEC, 0x11 };

        /// <summary>
        /// Encodes <paramref name="text"/> (UTF-8) at the given error-correction level.
        /// </summary>
        /// <exception cref="ArgumentNullException">text is null.</exception>
        /// <exception cref="ArgumentException">text does not fit any supported version.</exception>
        public static QrMatrix Encode(string text, QrErrorCorrectionLevel ecc = QrErrorCorrectionLevel.M)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }
            byte[] payload = Encoding.UTF8.GetBytes(text);
            int version = QrVersion.SmallestVersionFor(payload.Length, ecc);
            if (version < 0)
            {
                throw new ArgumentException(
                    "Text is too long for the supported QR versions (1-" + QrVersion.MaxVersion + ").", nameof(text));
            }

            byte[] dataCodewords = BuildDataCodewords(payload, version, ecc);
            byte[] finalCodewords = StructureFinalMessage(dataCodewords, version, ecc);

            int size = QrVersion.ModulesPerSide(version);
            QrMatrix matrix = new QrMatrix(size);
            BuildFunctionPatterns(matrix, version);
            PlaceData(matrix, finalCodewords);

            int mask = ApplyBestMask(matrix, ecc);
            PlaceFormatInformation(matrix, ecc, mask);
            PlaceVersionInformation(matrix, version);
            return matrix;
        }

        // ---- Stage 1: data codewords (mode + count + bytes + terminator + padding) ----

        private static byte[] BuildDataCodewords(byte[] payload, int version, QrErrorCorrectionLevel ecc)
        {
            int dataCodewordCount = QrVersion.GetEcBlocks(version, ecc).TotalDataCodewords;
            int capacityBits = dataCodewordCount * 8;
            BitBuffer bits = new BitBuffer();
            bits.Append(ByteModeIndicator, 4);
            bits.Append(payload.Length, QrVersion.ByteModeCharCountBits(version));
            for (int i = 0; i < payload.Length; i++)
            {
                bits.Append(payload[i], 8);
            }
            // Terminator: up to 4 zero bits, but no more than remaining capacity.
            int terminator = Math.Min(4, capacityBits - bits.Length);
            bits.Append(0, terminator);
            // Pad to a byte boundary.
            if (bits.Length % 8 != 0)
            {
                bits.Append(0, 8 - (bits.Length % 8));
            }
            // Pad bytes alternate 0xEC / 0x11 until the data capacity is filled.
            byte[] codewords = bits.ToBytes();
            List<byte> result = new List<byte>(codewords);
            int pad = 0;
            while (result.Count < dataCodewordCount)
            {
                result.Add(PadBytes[pad % 2]);
                pad++;
            }
            return result.ToArray();
        }

        // ---- Stage 2: split into blocks, add EC, interleave ----

        private static byte[] StructureFinalMessage(byte[] dataCodewords, int version, QrErrorCorrectionLevel ecc)
        {
            QrEcBlocks layout = QrVersion.GetEcBlocks(version, ecc);
            int totalBlocks = layout.TotalBlocks;
            byte[][] dataBlocks = new byte[totalBlocks][];
            byte[][] ecBlocks = new byte[totalBlocks][];

            int offset = 0;
            for (int b = 0; b < totalBlocks; b++)
            {
                int dataLen = b < layout.Group1Blocks ? layout.Group1DataCodewords : layout.Group2DataCodewords;
                byte[] block = new byte[dataLen];
                Array.Copy(dataCodewords, offset, block, 0, dataLen);
                offset += dataLen;
                dataBlocks[b] = block;
                ecBlocks[b] = ReedSolomonEncoder.Encode(block, layout.EcCodewordsPerBlock);
            }

            List<byte> result = new List<byte>();
            // Interleave data codewords column-by-column across blocks.
            int maxData = Math.Max(layout.Group1DataCodewords, layout.Group2DataCodewords);
            for (int i = 0; i < maxData; i++)
            {
                for (int b = 0; b < totalBlocks; b++)
                {
                    if (i < dataBlocks[b].Length)
                    {
                        result.Add(dataBlocks[b][i]);
                    }
                }
            }
            // Interleave EC codewords (every block has the same EC count).
            for (int i = 0; i < layout.EcCodewordsPerBlock; i++)
            {
                for (int b = 0; b < totalBlocks; b++)
                {
                    result.Add(ecBlocks[b][i]);
                }
            }
            return result.ToArray();
        }

        // ---- Stage 3: function patterns ----

        private static void BuildFunctionPatterns(QrMatrix m, int version)
        {
            int size = m.Size;
            PlaceFinder(m, 0, 0);
            PlaceFinder(m, size - 7, 0);
            PlaceFinder(m, 0, size - 7);

            // Timing patterns along row 6 and column 6.
            for (int i = 8; i < size - 8; i++)
            {
                bool dark = i % 2 == 0;
                m.SetFunction(i, 6, dark);
                m.SetFunction(6, i, dark);
            }

            // Alignment patterns (skip any overlapping a finder).
            int[] centres = QrVersion.AlignmentPatternCentres(version);
            for (int a = 0; a < centres.Length; a++)
            {
                for (int b = 0; b < centres.Length; b++)
                {
                    int cx = centres[a];
                    int cy = centres[b];
                    if (IsInFinder(cx, cy, size))
                    {
                        continue;
                    }
                    PlaceAlignment(m, cx, cy);
                }
            }

            // Reserve format-info area so data placement skips it; fill with timing-safe value.
            ReserveFormatArea(m);
            // Dark module.
            m.SetFunction(8, size - 8, true);

            if (version >= 7)
            {
                ReserveVersionArea(m);
            }
        }

        private static void PlaceFinder(QrMatrix m, int x0, int y0)
        {
            for (int dy = -1; dy <= 7; dy++)
            {
                for (int dx = -1; dx <= 7; dx++)
                {
                    int x = x0 + dx;
                    int y = y0 + dy;
                    if (x < 0 || y < 0 || x >= m.Size || y >= m.Size)
                    {
                        continue;
                    }
                    bool dark =
                        (dx >= 0 && dx <= 6 && (dy == 0 || dy == 6)) ||
                        (dy >= 0 && dy <= 6 && (dx == 0 || dx == 6)) ||
                        (dx >= 2 && dx <= 4 && dy >= 2 && dy <= 4);
                    m.SetFunction(x, y, dark);
                }
            }
        }

        private static void PlaceAlignment(QrMatrix m, int cx, int cy)
        {
            for (int dy = -2; dy <= 2; dy++)
            {
                for (int dx = -2; dx <= 2; dx++)
                {
                    int ring = Math.Max(Math.Abs(dx), Math.Abs(dy));
                    bool dark = ring != 1; // dark centre + dark outer ring, light middle ring
                    m.SetFunction(cx + dx, cy + dy, dark);
                }
            }
        }

        private static bool IsInFinder(int cx, int cy, int size)
        {
            // Alignment centres land inside a finder region when near a corner.
            bool nearTopLeft = cx <= 8 && cy <= 8;
            bool nearTopRight = cx >= size - 9 && cy <= 8;
            bool nearBottomLeft = cx <= 8 && cy >= size - 9;
            return nearTopLeft || nearTopRight || nearBottomLeft;
        }

        private static void ReserveFormatArea(QrMatrix m)
        {
            int size = m.Size;
            for (int i = 0; i <= 8; i++)
            {
                if (!m.IsFunction(i, 8))
                {
                    m.SetFunction(i, 8, false);
                }
                if (!m.IsFunction(8, i))
                {
                    m.SetFunction(8, i, false);
                }
            }
            for (int i = 0; i < 8; i++)
            {
                if (!m.IsFunction(size - 1 - i, 8))
                {
                    m.SetFunction(size - 1 - i, 8, false);
                }
                if (!m.IsFunction(8, size - 1 - i))
                {
                    m.SetFunction(8, size - 1 - i, false);
                }
            }
        }

        private static void ReserveVersionArea(QrMatrix m)
        {
            int size = m.Size;
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    m.SetFunction(size - 11 + j, i, false);
                    m.SetFunction(i, size - 11 + j, false);
                }
            }
        }

        // ---- Stage 4: data placement (zigzag, upward then downward) ----

        private static void PlaceData(QrMatrix m, byte[] codewords)
        {
            int size = m.Size;
            int bitIndex = 0;
            int totalBits = codewords.Length * 8;
            bool upward = true;
            for (int right = size - 1; right >= 1; right -= 2)
            {
                if (right == 6)
                {
                    right = 5; // skip the vertical timing column
                }
                for (int vert = 0; vert < size; vert++)
                {
                    int y = upward ? size - 1 - vert : vert;
                    for (int c = 0; c < 2; c++)
                    {
                        int x = right - c;
                        if (m.IsFunction(x, y))
                        {
                            continue;
                        }
                        bool dark = false;
                        if (bitIndex < totalBits)
                        {
                            int b = codewords[bitIndex >> 3];
                            int bit = 7 - (bitIndex & 7);
                            dark = ((b >> bit) & 1) == 1;
                            bitIndex++;
                        }
                        m.Set(x, y, dark);
                    }
                }
                upward = !upward;
            }
        }

        // ---- Stage 5: masking ----

        private static int ApplyBestMask(QrMatrix m, QrErrorCorrectionLevel ecc)
        {
            int bestMask = 0;
            int bestPenalty = int.MaxValue;
            for (int mask = 0; mask < 8; mask++)
            {
                ApplyMask(m, mask);
                PlaceFormatInformation(m, ecc, mask); // format bits affect penalty scoring
                int penalty = QrMaskScorer.Score(m);
                if (penalty < bestPenalty)
                {
                    bestPenalty = penalty;
                    bestMask = mask;
                }
                ApplyMask(m, mask); // toggle back off (mask is its own inverse)
            }
            ApplyMask(m, bestMask);
            return bestMask;
        }

        private static void ApplyMask(QrMatrix m, int mask)
        {
            int size = m.Size;
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    if (m.IsFunction(x, y))
                    {
                        continue;
                    }
                    if (MaskCondition(mask, x, y))
                    {
                        m.Toggle(x, y);
                    }
                }
            }
        }

        private static bool MaskCondition(int mask, int x, int y)
        {
            switch (mask)
            {
                case 0: return (x + y) % 2 == 0;
                case 1: return y % 2 == 0;
                case 2: return x % 3 == 0;
                case 3: return (x + y) % 3 == 0;
                case 4: return ((y / 2) + (x / 3)) % 2 == 0;
                case 5: return ((x * y) % 2) + ((x * y) % 3) == 0;
                case 6: return (((x * y) % 2) + ((x * y) % 3)) % 2 == 0;
                case 7: return (((x + y) % 2) + ((x * y) % 3)) % 2 == 0;
                default: return false;
            }
        }

        // ---- Stage 6: format & version information ----

        private static void PlaceFormatInformation(QrMatrix m, QrErrorCorrectionLevel ecc, int mask)
        {
            int format = QrFormatInfo.Build(ecc, mask);
            int size = m.Size;
            // Copy 1: around the top-left finder.
            for (int i = 0; i <= 5; i++)
            {
                SetFunctionBit(m, 8, i, format, i);
            }
            SetFunctionBit(m, 8, 7, format, 6);
            SetFunctionBit(m, 8, 8, format, 7);
            SetFunctionBit(m, 7, 8, format, 8);
            for (int i = 9; i <= 14; i++)
            {
                SetFunctionBit(m, 14 - i, 8, format, i);
            }
            // Copy 2: split between top-right and bottom-left finders.
            for (int i = 0; i <= 7; i++)
            {
                SetFunctionBit(m, size - 1 - i, 8, format, i);
            }
            for (int i = 8; i <= 14; i++)
            {
                SetFunctionBit(m, 8, size - 15 + i, format, i);
            }
        }

        private static void PlaceVersionInformation(QrMatrix m, int version)
        {
            if (version < 7)
            {
                return;
            }
            int info = QrFormatInfo.BuildVersion(version);
            int size = m.Size;
            for (int i = 0; i < 18; i++)
            {
                bool bit = ((info >> i) & 1) == 1;
                int a = i / 3;
                int b = i % 3;
                m.SetFunction(size - 11 + b, a, bit);
                m.SetFunction(a, size - 11 + b, bit);
            }
        }

        private static void SetFunctionBit(QrMatrix m, int x, int y, int value, int bitIndex)
        {
            bool bit = ((value >> bitIndex) & 1) == 1;
            m.SetFunction(x, y, bit);
        }

        /// <summary>Minimal MSB-first bit accumulator.</summary>
        private sealed class BitBuffer
        {
            private readonly List<bool> _bits = new List<bool>();

            public int Length => _bits.Count;

            public void Append(int value, int bitCount)
            {
                for (int i = bitCount - 1; i >= 0; i--)
                {
                    _bits.Add(((value >> i) & 1) == 1);
                }
            }

            public byte[] ToBytes()
            {
                int byteCount = (_bits.Count + 7) / 8;
                byte[] result = new byte[byteCount];
                for (int i = 0; i < _bits.Count; i++)
                {
                    if (_bits[i])
                    {
                        result[i >> 3] |= (byte)(1 << (7 - (i & 7)));
                    }
                }
                return result;
            }
        }
    }
}
