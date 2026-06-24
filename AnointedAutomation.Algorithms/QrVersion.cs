// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me

using System;

namespace AnointedAutomation.Algorithms.QrCode
{
    /// <summary>
    /// Describes the Reed–Solomon block layout for one (version, error-correction level):
    /// how many blocks of each group there are and how many data codewords each holds, plus
    /// the per-block EC codeword count (ISO/IEC 18004 Table 9).
    /// </summary>
    internal sealed class QrEcBlocks
    {
        public int EcCodewordsPerBlock { get; }
        public int Group1Blocks { get; }
        public int Group1DataCodewords { get; }
        public int Group2Blocks { get; }
        public int Group2DataCodewords { get; }

        public QrEcBlocks(int ecPerBlock, int g1Blocks, int g1Data, int g2Blocks, int g2Data)
        {
            EcCodewordsPerBlock = ecPerBlock;
            Group1Blocks = g1Blocks;
            Group1DataCodewords = g1Data;
            Group2Blocks = g2Blocks;
            Group2DataCodewords = g2Data;
        }

        /// <summary>Total data codewords across both groups.</summary>
        public int TotalDataCodewords => (Group1Blocks * Group1DataCodewords) + (Group2Blocks * Group2DataCodewords);

        /// <summary>Total blocks across both groups.</summary>
        public int TotalBlocks => Group1Blocks + Group2Blocks;
    }

    /// <summary>
    /// Per-version QR metadata: module count, alignment-pattern centres, total codeword
    /// capacity, and the EC block layout for each error-correction level. Versions 1–10 are
    /// supported (sufficient for URLs and short text); the tables extend cleanly to 40.
    /// </summary>
    internal static class QrVersion
    {
        /// <summary>Highest supported version. Extend the tables below to raise this.</summary>
        public const int MaxVersion = 10;

        // Total codewords (data + EC) per version, index = version. [0] unused.
        private static readonly int[] TotalCodewordsByVersion =
        {
            0, 26, 44, 70, 100, 134, 172, 196, 242, 292, 346
        };

        // Alignment pattern centre coordinates per version. [0] unused.
        private static readonly int[][] AlignmentCentres =
        {
            null,
            new int[] { },
            new int[] { 6, 18 },
            new int[] { 6, 22 },
            new int[] { 6, 26 },
            new int[] { 6, 30 },
            new int[] { 6, 34 },
            new int[] { 6, 22, 38 },
            new int[] { 6, 24, 42 },
            new int[] { 6, 26, 46 },
            new int[] { 6, 28, 50 }
        };

        // EC block layout indexed by [version][eccLevel]. eccLevel order: L=0, M=1, Q=2, H=3.
        private static readonly QrEcBlocks[][] Blocks =
        {
            null, // version 0 unused
            new[] { B(7,1,19,0,0),  B(10,1,16,0,0), B(13,1,13,0,0), B(17,1,9,0,0) },   // v1
            new[] { B(10,1,34,0,0), B(16,1,28,0,0), B(22,1,22,0,0), B(28,1,16,0,0) },  // v2
            new[] { B(15,1,55,0,0), B(26,1,44,0,0), B(18,2,17,0,0), B(22,2,13,0,0) },  // v3
            new[] { B(20,1,80,0,0), B(18,2,32,0,0), B(26,2,24,0,0), B(16,4,9,0,0) },   // v4
            new[] { B(26,1,108,0,0),B(24,2,43,0,0), B(18,2,15,2,16),B(22,2,11,2,12) }, // v5
            new[] { B(18,2,68,0,0), B(16,4,27,0,0), B(24,4,19,0,0), B(28,4,15,0,0) },  // v6
            new[] { B(20,2,78,0,0), B(18,4,31,0,0), B(18,2,14,4,15),B(26,4,13,1,14) }, // v7
            new[] { B(24,2,97,0,0), B(22,2,38,2,39),B(22,4,18,2,19),B(26,4,14,2,15) }, // v8
            new[] { B(30,2,116,0,0),B(22,3,36,2,37),B(20,4,16,4,17),B(24,4,12,4,13) }, // v9
            new[] { B(18,2,68,2,69),B(26,4,43,1,44),B(24,6,19,2,20),B(28,6,15,2,16) }  // v10
        };

        private static QrEcBlocks B(int ec, int g1b, int g1d, int g2b, int g2d)
        {
            return new QrEcBlocks(ec, g1b, g1d, g2b, g2d);
        }

        /// <summary>Module count along one side: 17 + 4 × version.</summary>
        public static int ModulesPerSide(int version)
        {
            return 17 + (4 * version);
        }

        /// <summary>Alignment pattern centre coordinates for the version.</summary>
        public static int[] AlignmentPatternCentres(int version)
        {
            return AlignmentCentres[version];
        }

        /// <summary>EC block layout for the version and error-correction level.</summary>
        public static QrEcBlocks GetEcBlocks(int version, QrErrorCorrectionLevel ecc)
        {
            return Blocks[version][(int)ecc];
        }

        /// <summary>Total codewords (data + EC) the version holds.</summary>
        public static int TotalCodewords(int version)
        {
            return TotalCodewordsByVersion[version];
        }

        /// <summary>Bit length of the byte-mode character-count indicator for the version.</summary>
        public static int ByteModeCharCountBits(int version)
        {
            return version <= 9 ? 8 : 16;
        }

        /// <summary>
        /// Smallest version (1..<see cref="MaxVersion"/>) whose data capacity at the given
        /// EC level holds <paramref name="byteCount"/> bytes in byte mode, or -1 if none fits.
        /// </summary>
        public static int SmallestVersionFor(int byteCount, QrErrorCorrectionLevel ecc)
        {
            for (int v = 1; v <= MaxVersion; v++)
            {
                int dataCodewords = GetEcBlocks(v, ecc).TotalDataCodewords;
                int dataBits = dataCodewords * 8;
                // 4-bit mode indicator + char-count indicator + 8 bits per byte.
                int requiredBits = 4 + ByteModeCharCountBits(v) + (byteCount * 8);
                if (requiredBits <= dataBits)
                {
                    return v;
                }
            }
            return -1;
        }

        /// <summary>
        /// Validates the EC block table against the published total-codeword capacity for every
        /// supported version/level. Returns true if all entries are internally consistent.
        /// </summary>
        public static bool ValidateTables()
        {
            for (int v = 1; v <= MaxVersion; v++)
            {
                foreach (QrErrorCorrectionLevel ecc in (QrErrorCorrectionLevel[])Enum.GetValues(typeof(QrErrorCorrectionLevel)))
                {
                    QrEcBlocks b = GetEcBlocks(v, ecc);
                    int computed = b.TotalDataCodewords + (b.TotalBlocks * b.EcCodewordsPerBlock);
                    if (computed != TotalCodewords(v))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
