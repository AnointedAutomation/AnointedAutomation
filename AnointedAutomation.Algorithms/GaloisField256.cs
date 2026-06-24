// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me

namespace AnointedAutomation.Algorithms.QrCode
{
    /// <summary>
    /// Arithmetic over GF(256) with the QR primitive polynomial 0x11D (x^8 + x^4 + x^3 + x^2 + 1).
    /// Provides log / antilog (exponent) tables used by Reed–Solomon error correction.
    /// </summary>
    internal static class GaloisField256
    {
        private static readonly int[] Exp = new int[512];
        private static readonly int[] Log = new int[256];

        static GaloisField256()
        {
            int x = 1;
            for (int i = 0; i < 255; i++)
            {
                Exp[i] = x;
                Log[x] = i;
                x <<= 1;
                if ((x & 0x100) != 0)
                {
                    x ^= 0x11D;
                }
            }
            // Duplicate the exponent table so callers can index up to 510 without wrapping.
            for (int i = 255; i < 512; i++)
            {
                Exp[i] = Exp[i - 255];
            }
        }

        /// <summary>Antilog: returns alpha^exponent in GF(256).</summary>
        public static int Antilog(int exponent)
        {
            return Exp[exponent % 255];
        }

        /// <summary>Discrete log base alpha. Undefined for 0.</summary>
        public static int LogOf(int value)
        {
            return Log[value];
        }

        /// <summary>Multiplies two GF(256) elements.</summary>
        public static int Multiply(int a, int b)
        {
            if (a == 0 || b == 0)
            {
                return 0;
            }
            return Exp[Log[a] + Log[b]];
        }
    }
}
