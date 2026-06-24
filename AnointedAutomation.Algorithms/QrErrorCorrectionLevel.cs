// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me

namespace AnointedAutomation.Algorithms.QrCode
{
    /// <summary>
    /// QR code error-correction levels (ISO/IEC 18004). Higher levels recover more of the
    /// symbol when damaged but hold less data for a given version.
    /// </summary>
    public enum QrErrorCorrectionLevel
    {
        /// <summary>Low — recovers ~7% of codewords.</summary>
        L = 0,
        /// <summary>Medium — recovers ~15% of codewords.</summary>
        M = 1,
        /// <summary>Quartile — recovers ~25% of codewords.</summary>
        Q = 2,
        /// <summary>High — recovers ~30% of codewords.</summary>
        H = 3
    }
}
