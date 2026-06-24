// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me

using System;
using Xunit;
using AnointedAutomation.Algorithms.QrCode;

namespace AnointedAutomation.Algorithms.Tests
{
    /// <summary>
    /// Tests for the QR encoder: Galois-field arithmetic, Reed-Solomon generator polynomials
    /// (validated against ISO/IEC 18004 reference vectors), capacity-table consistency, format
    /// information BCH, and matrix structure.
    /// </summary>
    public class QrCodeTests
    {
        [Fact]
        public void GaloisField_Multiply_Identities()
        {
            Assert.Equal(0, GaloisField256.Multiply(0, 5));
            Assert.Equal(0, GaloisField256.Multiply(5, 0));
            Assert.Equal(5, GaloisField256.Multiply(1, 5));
            // alpha^1 * alpha^1 = alpha^2 = 4 with primitive polynomial 0x11D.
            Assert.Equal(4, GaloisField256.Multiply(2, 2));
        }

        [Fact]
        public void GaloisField_LogAntilog_RoundTrip()
        {
            for (int v = 1; v < 256; v++)
            {
                int log = GaloisField256.LogOf(v);
                Assert.Equal(v, GaloisField256.Antilog(log));
            }
        }

        [Fact]
        public void GeneratorPolynomial_Degree7_MatchesIsoVector()
        {
            // ISO/IEC 18004 generator polynomial for 7 EC codewords, as alpha exponents.
            int[] expected = { 0, 87, 229, 146, 149, 238, 102, 21 };
            AssertGeneratorExponents(7, expected);
        }

        [Fact]
        public void GeneratorPolynomial_Degree10_MatchesIsoVector()
        {
            int[] expected = { 0, 251, 67, 46, 61, 118, 70, 64, 94, 32, 45 };
            AssertGeneratorExponents(10, expected);
        }

        private static void AssertGeneratorExponents(int degree, int[] expectedExponents)
        {
            int[] poly = ReedSolomonEncoder.BuildGeneratorPolynomial(degree);
            Assert.Equal(expectedExponents.Length, poly.Length);
            for (int i = 0; i < poly.Length; i++)
            {
                Assert.Equal(expectedExponents[i], GaloisField256.LogOf(poly[i]));
            }
        }

        [Fact]
        public void VersionTables_AreInternallyConsistent()
        {
            Assert.True(QrVersion.ValidateTables(),
                "EC block table does not match the published total-codeword capacity for some version/level.");
        }

        [Fact]
        public void SmallestVersionFor_PicksExpectedVersion()
        {
            // v1-M holds 16 data codewords -> 4 + 8 + 14*8 = 124 bits <= 128. 14 bytes fits v1.
            Assert.Equal(1, QrVersion.SmallestVersionFor(14, QrErrorCorrectionLevel.M));
            // 15 bytes needs 132 bits, overflows v1 (128) -> v2.
            Assert.Equal(2, QrVersion.SmallestVersionFor(15, QrErrorCorrectionLevel.M));
        }

        [Fact]
        public void FormatInformation_MMask0_EqualsMaskConstant()
        {
            // EC level M (indicator 00) + mask 0 -> data 0 -> BCH 0 -> XOR 0x5412.
            Assert.Equal(0x5412, QrFormatInfo.Build(QrErrorCorrectionLevel.M, 0));
        }

        [Theory]
        [InlineData(QrErrorCorrectionLevel.L, 1)] // L indicator = 01
        [InlineData(QrErrorCorrectionLevel.M, 0)] // M indicator = 00
        [InlineData(QrErrorCorrectionLevel.Q, 3)] // Q indicator = 11
        [InlineData(QrErrorCorrectionLevel.H, 2)] // H indicator = 10
        public void FormatInformation_DataBits_RoundTrip(QrErrorCorrectionLevel ecc, int indicator)
        {
            for (int mask = 0; mask < 8; mask++)
            {
                int unmasked = QrFormatInfo.Build(ecc, mask) ^ 0x5412;
                int data = (unmasked >> 10) & 0x1F;
                Assert.Equal((indicator << 3) | mask, data);
            }
        }

        [Fact]
        public void Encode_ShortText_ProducesVersion1Matrix()
        {
            QrMatrix m = QrEncoder.Encode("HELLO", QrErrorCorrectionLevel.M);
            Assert.Equal(21, m.Size); // version 1 = 21x21
        }

        [Fact]
        public void Encode_PlacesFinderPattern()
        {
            QrMatrix m = QrEncoder.Encode("follow on shop", QrErrorCorrectionLevel.M);
            Assert.True(m.IsDark(0, 0));   // finder outer corner
            Assert.False(m.IsDark(1, 1));  // light ring inside finder
            Assert.True(m.IsDark(3, 3));   // dark 3x3 centre
            Assert.False(m.IsDark(7, 7) && m.IsDark(7, 0)); // separator keeps these light somewhere
        }

        [Fact]
        public void Encode_LongerText_SelectsHigherVersion()
        {
            string url = "https://shop.app/@anointedattire?utm_source=amazon_insert&utm_campaign=follow";
            QrMatrix m = QrEncoder.Encode(url, QrErrorCorrectionLevel.M);
            Assert.True(m.Size > 21, "expected a version > 1 for a long URL");
            Assert.Equal(0, (m.Size - 21) % 4); // valid version size 17 + 4v
        }

        [Fact]
        public void Encode_Null_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => QrEncoder.Encode(null));
        }

        [Fact]
        public void Encode_TooLong_Throws()
        {
            string huge = new string('x', 5000);
            Assert.Throws<ArgumentException>(() => QrEncoder.Encode(huge, QrErrorCorrectionLevel.H));
        }
    }
}
