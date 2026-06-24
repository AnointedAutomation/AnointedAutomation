// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me

using System;
using System.Text;
using Xunit;
using AnointedAutomation.Algorithms.QrCode;
using AnointedAutomation.Imaging.QrCode;

namespace AnointedAutomation.Imaging.Tests
{
    /// <summary>
    /// Tests for QR SVG / PNG rendering.
    /// </summary>
    public class QrRendererTests
    {
        private static QrMatrix Sample()
        {
            return QrEncoder.Encode("https://shop.app/@anointedattire", QrErrorCorrectionLevel.M);
        }

        [Fact]
        public void ToSvg_ProducesWellFormedSvg()
        {
            string svg = QrRenderer.ToSvg(Sample(), moduleSize: 8, quietZone: 4);
            Assert.StartsWith("<svg", svg);
            Assert.EndsWith("</svg>", svg);
            Assert.Contains("fill=\"#000000\"", svg); // at least one dark module
        }

        [Fact]
        public void ToSvg_DimensionsIncludeQuietZone()
        {
            QrMatrix m = Sample();
            int expected = (m.Size + 8) * 8; // quietZone 4 each side, module 8
            string svg = QrRenderer.ToSvg(m, moduleSize: 8, quietZone: 4);
            Assert.Contains("width=\"" + expected + "\"", svg);
        }

        [Fact]
        public void ToPng_HasValidSignatureAndHeader()
        {
            byte[] png = QrRenderer.ToPng(Sample(), moduleSize: 4, quietZone: 4);
            byte[] signature = { 137, 80, 78, 71, 13, 10, 26, 10 };
            for (int i = 0; i < signature.Length; i++)
            {
                Assert.Equal(signature[i], png[i]);
            }
            // First chunk after the signature must be IHDR (length 4 bytes at 8, type at 12).
            string firstChunkType = Encoding.ASCII.GetString(png, 12, 4);
            Assert.Equal("IHDR", firstChunkType);
            // PNG must end with an IEND chunk.
            string lastChunkType = Encoding.ASCII.GetString(png, png.Length - 8, 4);
            Assert.Equal("IEND", lastChunkType);
        }

        [Fact]
        public void ToPng_EncodesExpectedPixelDimensions()
        {
            QrMatrix m = Sample();
            int dim = (m.Size + 8) * 4; // module 4, quiet 4
            byte[] png = QrRenderer.ToPng(m, moduleSize: 4, quietZone: 4);
            // IHDR width is the 4 big-endian bytes at offset 16.
            int width = (png[16] << 24) | (png[17] << 16) | (png[18] << 8) | png[19];
            Assert.Equal(dim, width);
        }

        [Fact]
        public void ToPng_NullMatrix_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => QrRenderer.ToPng(null));
        }

        [Fact]
        public void ToSvg_InvalidModuleSize_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => QrRenderer.ToSvg(Sample(), moduleSize: 0));
        }
    }
}
