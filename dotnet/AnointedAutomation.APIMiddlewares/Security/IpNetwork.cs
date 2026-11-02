// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-02 00:00:00 Jesus is King ✝️
using System;
using System.Net;
using System.Net.Sockets;

namespace AnointedAutomation.APIMiddleware.Security
{
    /// <summary>
    /// One IP network: a single address or a CIDR range, IPv4 or IPv6.
    /// </summary>
    public sealed class IpNetwork
    {
        private readonly byte[] _network;

        private IpNetwork(byte[] network, int prefixLength, string text)
        {
            _network = network;
            PrefixLength = prefixLength;
            Text = text;
        }

        /// <summary>Gets the prefix length in bits (32 or 128 for a single address).</summary>
        public int PrefixLength { get; }

        /// <summary>Gets the rule text as configured, used in log lines as the matched rule.</summary>
        public string Text { get; }

        /// <summary>
        /// Parses "203.0.113.7", "203.0.113.0/24", "2001:db8::1" or "2001:db8::/32".
        /// </summary>
        /// <param name="text">The address or CIDR text.</param>
        /// <param name="network">The parsed network, or null when parsing fails.</param>
        /// <returns>True when the text is a valid address or CIDR range.</returns>
        public static bool TryParse(string text, out IpNetwork network)
        {
            network = null;
            if (string.IsNullOrWhiteSpace(text))
            {
                return false;
            }
            string trimmed = text.Trim();
            string addressPart = trimmed;
            int? prefix = null;
            int slash = trimmed.IndexOf('/');
            if (slash >= 0)
            {
                addressPart = trimmed.Substring(0, slash);
                int parsedPrefix;
                if (!int.TryParse(trimmed.Substring(slash + 1), System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, out parsedPrefix))
                {
                    return false;
                }
                prefix = parsedPrefix;
            }
            IPAddress address;
            if (!IPAddress.TryParse(addressPart, out address))
            {
                return false;
            }
            if (address.IsIPv4MappedToIPv6)
            {
                address = address.MapToIPv4();
                if (prefix.HasValue)
                {
                    if (prefix.Value < 96)
                    {
                        return false;
                    }
                    prefix = prefix.Value - 96;
                }
            }
            byte[] bytes = address.GetAddressBytes();
            int maxBits = bytes.Length * 8;
            int bits = prefix.HasValue ? prefix.Value : maxBits;
            if (bits < 0 || bits > maxBits)
            {
                return false;
            }
            network = new IpNetwork(Mask(bytes, bits), bits, trimmed);
            return true;
        }

        /// <summary>
        /// Parses an address or CIDR and throws when it is invalid (configuration errors must fail at startup).
        /// </summary>
        /// <param name="text">The address or CIDR text.</param>
        /// <returns>The parsed network.</returns>
        /// <exception cref="FormatException">Thrown when the text is not an address or CIDR range.</exception>
        public static IpNetwork Parse(string text)
        {
            IpNetwork network;
            if (!TryParse(text, out network))
            {
                throw new FormatException("Not a valid IP address or CIDR range: '" + text + "'.");
            }
            return network;
        }

        /// <summary>
        /// Whether the address is inside this network. An IPv4-mapped IPv6 address is compared as IPv4.
        /// </summary>
        /// <param name="address">The address to test.</param>
        /// <returns>True when the address is inside the network.</returns>
        public bool Contains(IPAddress address)
        {
            if (address == null)
            {
                return false;
            }
            IPAddress candidate = address.IsIPv4MappedToIPv6 ? address.MapToIPv4() : address;
            byte[] bytes = candidate.GetAddressBytes();
            if (bytes.Length != _network.Length)
            {
                return false;
            }
            byte[] masked = Mask(bytes, PrefixLength);
            for (int i = 0; i < masked.Length; i++)
            {
                if (masked[i] != _network[i])
                {
                    return false;
                }
            }
            return true;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return Text;
        }

        private static byte[] Mask(byte[] bytes, int bits)
        {
            byte[] result = new byte[bytes.Length];
            for (int i = 0; i < bytes.Length; i++)
            {
                int bitsInByte = Math.Max(0, Math.Min(8, bits - (i * 8)));
                int maskByte = bitsInByte == 0 ? 0 : (0xFF << (8 - bitsInByte)) & 0xFF;
                result[i] = (byte)(bytes[i] & maskByte);
            }
            return result;
        }
    }
}
