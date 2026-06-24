// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me

using System;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Text;
using AnointedAutomation.Algorithms.QrCode;

namespace AnointedAutomation.Imaging.QrCode
{
    /// <summary>
    /// Renders a <see cref="QrMatrix"/> to SVG or PNG. The PNG writer is built on the BCL's
    /// <see cref="ZLibStream"/> and a small CRC-32, so there is no third-party image dependency.
    /// </summary>
    public static class QrRenderer
    {
        /// <summary>
        /// Renders the matrix as an SVG string. <paramref name="moduleSize"/> is the pixel size
        /// of each module; <paramref name="quietZone"/> is the light border in modules.
        /// </summary>
        public static string ToSvg(QrMatrix matrix, int moduleSize = 8, int quietZone = 4)
        {
            ValidateArgs(matrix, moduleSize, quietZone);
            int dim = (matrix.Size + (2 * quietZone)) * moduleSize;
            StringBuilder sb = new StringBuilder();
            sb.Append("<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"").Append(dim)
              .Append("\" height=\"").Append(dim).Append("\" viewBox=\"0 0 ").Append(dim)
              .Append(' ').Append(dim).Append("\" shape-rendering=\"crispEdges\">");
            sb.Append("<rect width=\"").Append(dim).Append("\" height=\"").Append(dim).Append("\" fill=\"#ffffff\"/>");
            for (int y = 0; y < matrix.Size; y++)
            {
                for (int x = 0; x < matrix.Size; x++)
                {
                    if (!matrix.IsDark(x, y))
                    {
                        continue;
                    }
                    int px = (x + quietZone) * moduleSize;
                    int py = (y + quietZone) * moduleSize;
                    sb.Append("<rect x=\"").Append(px.ToString(CultureInfo.InvariantCulture))
                      .Append("\" y=\"").Append(py.ToString(CultureInfo.InvariantCulture))
                      .Append("\" width=\"").Append(moduleSize).Append("\" height=\"").Append(moduleSize)
                      .Append("\" fill=\"#000000\"/>");
                }
            }
            sb.Append("</svg>");
            return sb.ToString();
        }

        /// <summary>
        /// Renders the matrix as an 8-bit grayscale PNG byte array.
        /// </summary>
        public static byte[] ToPng(QrMatrix matrix, int moduleSize = 8, int quietZone = 4)
        {
            ValidateArgs(matrix, moduleSize, quietZone);
            int dim = (matrix.Size + (2 * quietZone)) * moduleSize;

            // Build raw scanlines: each row prefixed with filter byte 0 (None).
            byte[] raw = new byte[(dim + 1) * dim];
            int p = 0;
            for (int y = 0; y < dim; y++)
            {
                raw[p++] = 0; // filter: None
                int my = (y / moduleSize) - quietZone;
                for (int x = 0; x < dim; x++)
                {
                    int mx = (x / moduleSize) - quietZone;
                    bool dark = mx >= 0 && my >= 0 && mx < matrix.Size && my < matrix.Size && matrix.IsDark(mx, my);
                    raw[p++] = dark ? (byte)0x00 : (byte)0xFF;
                }
            }

            using (MemoryStream output = new MemoryStream())
            {
                output.Write(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 }, 0, 8); // PNG signature

                byte[] ihdr = new byte[13];
                WriteBigEndian(ihdr, 0, dim);
                WriteBigEndian(ihdr, 4, dim);
                ihdr[8] = 8; // bit depth
                ihdr[9] = 0; // colour type: grayscale
                ihdr[10] = 0; ihdr[11] = 0; ihdr[12] = 0;
                WriteChunk(output, "IHDR", ihdr);

                byte[] compressed;
                using (MemoryStream comp = new MemoryStream())
                {
                    using (ZLibStream zlib = new ZLibStream(comp, CompressionLevel.Optimal, true))
                    {
                        zlib.Write(raw, 0, raw.Length);
                    }
                    compressed = comp.ToArray();
                }
                WriteChunk(output, "IDAT", compressed);
                WriteChunk(output, "IEND", Array.Empty<byte>());
                return output.ToArray();
            }
        }

        private static void ValidateArgs(QrMatrix matrix, int moduleSize, int quietZone)
        {
            if (matrix == null)
            {
                throw new ArgumentNullException(nameof(matrix));
            }
            if (moduleSize < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(moduleSize));
            }
            if (quietZone < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(quietZone));
            }
        }

        private static void WriteBigEndian(byte[] buffer, int offset, int value)
        {
            buffer[offset] = (byte)((value >> 24) & 0xFF);
            buffer[offset + 1] = (byte)((value >> 16) & 0xFF);
            buffer[offset + 2] = (byte)((value >> 8) & 0xFF);
            buffer[offset + 3] = (byte)(value & 0xFF);
        }

        private static void WriteChunk(Stream output, string type, byte[] data)
        {
            byte[] lengthBytes = new byte[4];
            WriteBigEndian(lengthBytes, 0, data.Length);
            output.Write(lengthBytes, 0, 4);

            byte[] typeBytes = Encoding.ASCII.GetBytes(type);
            output.Write(typeBytes, 0, 4);
            output.Write(data, 0, data.Length);

            uint crc = Crc32.Compute(typeBytes, data);
            byte[] crcBytes = new byte[4];
            WriteBigEndian(crcBytes, 0, (int)crc);
            output.Write(crcBytes, 0, 4);
        }

        /// <summary>CRC-32 (PNG/zlib polynomial) over a chunk's type bytes followed by its data.</summary>
        private static class Crc32
        {
            private static readonly uint[] Table = BuildTable();

            private static uint[] BuildTable()
            {
                uint[] table = new uint[256];
                for (uint n = 0; n < 256; n++)
                {
                    uint c = n;
                    for (int k = 0; k < 8; k++)
                    {
                        c = (c & 1) != 0 ? 0xEDB88320 ^ (c >> 1) : c >> 1;
                    }
                    table[n] = c;
                }
                return table;
            }

            public static uint Compute(byte[] type, byte[] data)
            {
                uint crc = 0xFFFFFFFF;
                crc = Update(crc, type);
                crc = Update(crc, data);
                return crc ^ 0xFFFFFFFF;
            }

            private static uint Update(uint crc, byte[] bytes)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    crc = Table[(crc ^ bytes[i]) & 0xFF] ^ (crc >> 8);
                }
                return crc;
            }
        }
    }
}
