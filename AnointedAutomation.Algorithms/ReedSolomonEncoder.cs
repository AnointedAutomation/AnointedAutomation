// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me

using System;

namespace AnointedAutomation.Algorithms.QrCode
{
    /// <summary>
    /// Generates Reed–Solomon error-correction codewords for a block of data codewords,
    /// per ISO/IEC 18004. Pure polynomial division over GF(256).
    /// </summary>
    internal static class ReedSolomonEncoder
    {
        /// <summary>
        /// Builds the generator polynomial of the given degree (number of EC codewords).
        /// Coefficients are returned as GF(256) values, highest degree first.
        /// </summary>
        public static int[] BuildGeneratorPolynomial(int degree)
        {
            int[] poly = new int[] { 1 };
            for (int i = 0; i < degree; i++)
            {
                // Multiply current polynomial by (x - alpha^i) = (x + alpha^i) in GF(256).
                int[] next = new int[poly.Length + 1];
                int root = GaloisField256.Antilog(i);
                for (int j = 0; j < poly.Length; j++)
                {
                    next[j] ^= poly[j];                                  // x term
                    next[j + 1] ^= GaloisField256.Multiply(poly[j], root); // constant term
                }
                poly = next;
            }
            return poly;
        }

        /// <summary>
        /// Computes <paramref name="ecCount"/> error-correction codewords for the data block.
        /// </summary>
        public static byte[] Encode(byte[] data, int ecCount)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }
            int[] generator = BuildGeneratorPolynomial(ecCount);
            // Remainder register, one slot per EC codeword.
            int[] remainder = new int[ecCount];
            for (int i = 0; i < data.Length; i++)
            {
                int factor = (data[i] ^ remainder[0]) & 0xFF;
                // Shift remainder left by one.
                Array.Copy(remainder, 1, remainder, 0, ecCount - 1);
                remainder[ecCount - 1] = 0;
                if (factor != 0)
                {
                    for (int j = 0; j < ecCount; j++)
                    {
                        // generator[j + 1] skips the leading 1 coefficient.
                        remainder[j] ^= GaloisField256.Multiply(generator[j + 1], factor);
                    }
                }
            }
            byte[] ec = new byte[ecCount];
            for (int i = 0; i < ecCount; i++)
            {
                ec[i] = (byte)remainder[i];
            }
            return ec;
        }
    }
}
