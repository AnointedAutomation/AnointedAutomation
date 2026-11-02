// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-02 00:00:00 Jesus is King ✝️
using System;
using System.Collections.Concurrent;
using System.IO;
using System.IO.Compression;

namespace AnointedAutomation.APIMiddleware.BotDefense
{
    /// <summary>
    /// Builds the zip bomb served to bots probing for .env files: a one-entry zip whose entry expands to a configured
    /// number of zero bytes but compresses to a tiny payload. Generated once per size (a failed build is not cached) and cached, so no binary
    /// is stored in the repository.
    /// </summary>
    public static class ZipBomb
    {
        /// <summary>The largest expanded size accepted (16 GiB).</summary>
        public const long MaxExpandedBytes = 16L * 1024 * 1024 * 1024;

        private const int ChunkBytes = 1024 * 1024;

        private static readonly ConcurrentDictionary<long, byte[]> Cache = new ConcurrentDictionary<long, byte[]>();

        /// <summary>Gets the zip bytes for an entry named <c>.env</c> that expands to the given size.</summary>
        /// <param name="expandedBytes">The uncompressed size of the entry, greater than zero and at most <see cref="MaxExpandedBytes"/>.</param>
        /// <returns>The cached zip archive bytes. Callers must not modify them.</returns>
        public static byte[] Get(long expandedBytes)
        {
            if (expandedBytes <= 0 || expandedBytes > MaxExpandedBytes)
            {
                throw new ArgumentOutOfRangeException(nameof(expandedBytes), expandedBytes, "Expanded size must be between 1 and " + MaxExpandedBytes + " bytes.");
            }
            return Cache.GetOrAdd(expandedBytes, Build);
        }

        internal static bool IsCached(long expandedBytes)
        {
            return Cache.ContainsKey(expandedBytes);
        }

        private static byte[] Build(long expandedBytes)
        {
            byte[] zeros = new byte[ChunkBytes];
            using (MemoryStream output = new MemoryStream())
            {
                using (ZipArchive archive = new ZipArchive(output, ZipArchiveMode.Create, true))
                {
                    ZipArchiveEntry entry = archive.CreateEntry(".env", CompressionLevel.SmallestSize);
                    using (Stream stream = entry.Open())
                    {
                        long remaining = expandedBytes;
                        while (remaining > 0)
                        {
                            int count = (int)Math.Min(remaining, zeros.Length);
                            stream.Write(zeros, 0, count);
                            remaining -= count;
                        }
                    }
                }
                return output.ToArray();
            }
        }
    }
}
