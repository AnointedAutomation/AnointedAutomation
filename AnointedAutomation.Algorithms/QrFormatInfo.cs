// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me

namespace AnointedAutomation.Algorithms.QrCode
{
    /// <summary>
    /// Builds the 15-bit format information and 18-bit version information bit strings,
    /// including their BCH error-correction bits and the format mask (ISO/IEC 18004).
    /// </summary>
    internal static class QrFormatInfo
    {
        // Format-info 2-bit indicator differs from the enum order: L=01, M=00, Q=11, H=10.
        private static readonly int[] EccIndicator = { 1, 0, 3, 2 };
        private const int FormatGenerator = 0x537;   // x^10 + x^8 + x^5 + x^4 + x^2 + x + 1
        private const int FormatMask = 0x5412;        // applied to scramble all-zero data
        private const int VersionGenerator = 0x1F25;  // degree-12 BCH generator

        /// <summary>15-bit format information for the EC level + mask pattern.</summary>
        public static int Build(QrErrorCorrectionLevel ecc, int mask)
        {
            int data = (EccIndicator[(int)ecc] << 3) | (mask & 0x7);
            int remainder = data << 10;
            for (int i = 14; i >= 10; i--)
            {
                if (((remainder >> i) & 1) != 0)
                {
                    remainder ^= FormatGenerator << (i - 10);
                }
            }
            return ((data << 10) | (remainder & 0x3FF)) ^ FormatMask;
        }

        /// <summary>18-bit version information (only used for versions 7 and above).</summary>
        public static int BuildVersion(int version)
        {
            int remainder = version << 12;
            for (int i = 17; i >= 12; i--)
            {
                if (((remainder >> i) & 1) != 0)
                {
                    remainder ^= VersionGenerator << (i - 12);
                }
            }
            return (version << 12) | (remainder & 0xFFF);
        }
    }
}
