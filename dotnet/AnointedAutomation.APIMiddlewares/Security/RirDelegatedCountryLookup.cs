// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-02 00:00:00 Jesus is King ✝️
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace AnointedAutomation.APIMiddleware.Security
{
    /// <summary>
    /// IP to country from the Regional Internet Registries' public "delegated extended" statistics files
    /// (ARIN, RIPE NCC, APNIC, LACNIC, AFRINIC). No key, no account, and the files are published openly by the
    /// registries, so nothing is committed to this repository; the host downloads them (see
    /// <see cref="CountryDataRefresher"/>). The country is the registry's allocation country, which is coarser than
    /// commercial geolocation: use it to block whole countries, not for precise location.
    /// </summary>
    public sealed class RirDelegatedCountryLookup : ICountryLookup
    {
        private readonly uint[] _v4Start;
        private readonly uint[] _v4End;
        private readonly ushort[] _v4Country;
        private readonly UInt128[] _v6Start;
        private readonly UInt128[] _v6End;
        private readonly ushort[] _v6Country;

        private RirDelegatedCountryLookup(List<V4> v4, List<V6> v6)
        {
            v4.Sort((a, b) => a.Start.CompareTo(b.Start));
            v6.Sort((a, b) => a.Start.CompareTo(b.Start));
            _v4Start = new uint[v4.Count];
            _v4End = new uint[v4.Count];
            _v4Country = new ushort[v4.Count];
            for (int i = 0; i < v4.Count; i++)
            {
                _v4Start[i] = v4[i].Start;
                _v4End[i] = v4[i].End;
                _v4Country[i] = v4[i].Country;
            }
            _v6Start = new UInt128[v6.Count];
            _v6End = new UInt128[v6.Count];
            _v6Country = new ushort[v6.Count];
            for (int i = 0; i < v6.Count; i++)
            {
                _v6Start[i] = v6[i].Start;
                _v6End[i] = v6[i].End;
                _v6Country[i] = v6[i].Country;
            }
        }

        /// <summary>Gets the number of IPv4 ranges loaded.</summary>
        public int Ipv4RangeCount
        {
            get { return _v4Start.Length; }
        }

        /// <summary>Gets the number of IPv6 ranges loaded.</summary>
        public int Ipv6RangeCount
        {
            get { return _v6Start.Length; }
        }

        /// <summary>
        /// Counts the usable ranges in one file without building a lookup (used to reject an empty download).
        /// </summary>
        /// <param name="statsText">One delegated stats file.</param>
        /// <returns>The number of allocated or assigned IPv4 and IPv6 ranges.</returns>
        public static int CountRanges(string statsText)
        {
            List<V4> v4 = new List<V4>();
            List<V6> v6 = new List<V6>();
            ParseInto(statsText, v4, v6);
            return v4.Count + v6.Count;
        }

        /// <summary>
        /// Accumulates delegated stats files (each parsed once) and builds the lookup.
        /// </summary>
        public sealed class Builder
        {
            private readonly List<V4> _v4 = new List<V4>();
            private readonly List<V6> _v6 = new List<V6>();

            /// <summary>
            /// Parses one file.
            /// </summary>
            /// <param name="statsText">One delegated stats file.</param>
            /// <returns>The number of allocated or assigned ranges it contributed (0 means it is not a usable stats file).</returns>
            public int Add(string statsText)
            {
                int before = _v4.Count + _v6.Count;
                ParseInto(statsText, _v4, _v6);
                return _v4.Count + _v6.Count - before;
            }

            /// <summary>
            /// Builds the lookup from everything added so far.
            /// </summary>
            /// <returns>The lookup.</returns>
            public RirDelegatedCountryLookup Build()
            {
                return new RirDelegatedCountryLookup(_v4, _v6);
            }
        }

        /// <summary>
        /// Builds a lookup from delegated stats files.
        /// </summary>
        /// <param name="statsTexts">The file contents (one per registry).</param>
        /// <returns>The lookup.</returns>
        public static RirDelegatedCountryLookup FromStats(IEnumerable<string> statsTexts)
        {
            List<V4> v4 = new List<V4>();
            List<V6> v6 = new List<V6>();
            foreach (string text in statsTexts)
            {
                ParseInto(text, v4, v6);
            }
            return new RirDelegatedCountryLookup(v4, v6);
        }

        /// <inheritdoc />
        public string Lookup(IPAddress address)
        {
            if (address == null)
            {
                return null;
            }
            IPAddress candidate = address.IsIPv4MappedToIPv6 ? address.MapToIPv4() : address;
            if (candidate.AddressFamily == AddressFamily.InterNetwork)
            {
                byte[] b = candidate.GetAddressBytes();
                uint value = ((uint)b[0] << 24) | ((uint)b[1] << 16) | ((uint)b[2] << 8) | b[3];
                int index = LastAtOrBefore(_v4Start, value);
                if (index >= 0 && value <= _v4End[index])
                {
                    return Decode(_v4Country[index]);
                }
                return null;
            }
            if (candidate.AddressFamily == AddressFamily.InterNetworkV6)
            {
                UInt128 value = ToUInt128(candidate.GetAddressBytes());
                int index = LastAtOrBefore(_v6Start, value);
                if (index >= 0 && value <= _v6End[index])
                {
                    return Decode(_v6Country[index]);
                }
            }
            return null;
        }

        private static int LastAtOrBefore<T>(T[] starts, T value) where T : IComparable<T>
        {
            int low = 0;
            int high = starts.Length - 1;
            int found = -1;
            while (low <= high)
            {
                int mid = low + ((high - low) / 2);
                if (starts[mid].CompareTo(value) <= 0)
                {
                    found = mid;
                    low = mid + 1;
                }
                else
                {
                    high = mid - 1;
                }
            }
            return found;
        }

        private static string Decode(ushort country)
        {
            return new string(new char[] { (char)(country >> 8), (char)(country & 0xFF) });
        }

        private static UInt128 ToUInt128(byte[] bytes)
        {
            UInt128 value = UInt128.Zero;
            for (int i = 0; i < bytes.Length; i++)
            {
                value = (value << 8) | bytes[i];
            }
            return value;
        }

        private static void ParseInto(string text, List<V4> v4, List<V6> v6)
        {
            using (StringReader reader = new StringReader(text))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Length == 0 || line[0] == '#')
                    {
                        continue;
                    }
                    string[] f = line.Split('|');
                    if (f.Length < 7)
                    {
                        continue;
                    }
                    string country = f[1];
                    string type = f[2];
                    string status = f[6];
                    bool usable = string.Equals(status, "allocated", StringComparison.Ordinal) || string.Equals(status, "assigned", StringComparison.Ordinal);
                    if (!usable || country.Length != 2 || !IsLetter(country[0]) || !IsLetter(country[1]) || country.Equals("ZZ", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                    ushort code = (ushort)((char.ToUpperInvariant(country[0]) << 8) | char.ToUpperInvariant(country[1]));
                    if (string.Equals(type, "ipv4", StringComparison.Ordinal))
                    {
                        IPAddress start;
                        uint count;
                        if (IPAddress.TryParse(f[3], out start) && start.AddressFamily == AddressFamily.InterNetwork
                            && uint.TryParse(f[4], NumberStyles.None, CultureInfo.InvariantCulture, out count) && count > 0)
                        {
                            byte[] b = start.GetAddressBytes();
                            uint first = ((uint)b[0] << 24) | ((uint)b[1] << 16) | ((uint)b[2] << 8) | b[3];
                            ulong last = (ulong)first + count - 1;
                            if (last <= uint.MaxValue)
                            {
                                v4.Add(new V4 { Start = first, End = (uint)last, Country = code });
                            }
                        }
                    }
                    else if (string.Equals(type, "ipv6", StringComparison.Ordinal))
                    {
                        IPAddress start;
                        int prefix;
                        if (IPAddress.TryParse(f[3], out start) && start.AddressFamily == AddressFamily.InterNetworkV6
                            && int.TryParse(f[4], NumberStyles.None, CultureInfo.InvariantCulture, out prefix) && prefix >= 0 && prefix <= 128)
                        {
                            UInt128 first = ToUInt128(start.GetAddressBytes());
                            UInt128 size = prefix == 0 ? UInt128.MaxValue : (UInt128.One << (128 - prefix)) - 1;
                            v6.Add(new V6 { Start = first, End = first + size, Country = code });
                        }
                    }
                }
            }
        }

        private static bool IsLetter(char c)
        {
            return (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z');
        }

        private struct V4
        {
            public uint Start;
            public uint End;
            public ushort Country;
        }

        private struct V6
        {
            public UInt128 Start;
            public UInt128 End;
            public ushort Country;
        }
    }
}
