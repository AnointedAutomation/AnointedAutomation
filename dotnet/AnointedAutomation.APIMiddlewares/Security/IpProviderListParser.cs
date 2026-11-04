// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-02 00:00:00 Jesus is King ✝️
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace AnointedAutomation.APIMiddleware.Security
{
    /// <summary>
    /// Parses the range lists providers publish. Strict: any malformed entry fails the whole list so a
    /// half-read or tampered download never replaces a good list.
    /// </summary>
    public static class IpProviderListParser
    {
        /// <summary>
        /// Parses downloaded bodies into CIDR strings.
        /// </summary>
        /// <param name="format">The provider's format.</param>
        /// <param name="bodies">The downloaded bodies, one per URL.</param>
        /// <returns>The CIDR strings (never empty).</returns>
        /// <exception cref="FormatException">Thrown when the content is malformed or yields no ranges.</exception>
        public static List<string> Parse(IpProviderFormat format, IEnumerable<string> bodies)
        {
            List<string> result = new List<string>();
            foreach (string body in bodies)
            {
                if (string.IsNullOrWhiteSpace(body))
                {
                    throw new FormatException("A provider download was empty.");
                }
                if (format == IpProviderFormat.LineSeparated)
                {
                    foreach (string line in body.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        result.Add(line.Trim());
                    }
                }
                else if (format == IpProviderFormat.FastlyJson)
                {
                    using (JsonDocument document = JsonDocument.Parse(body))
                    {
                        AddArray(document.RootElement, "addresses", result);
                        AddArray(document.RootElement, "ipv6_addresses", result);
                    }
                }
                else
                {
                    using (JsonDocument document = JsonDocument.Parse(body))
                    {
                        AddAwsCloudFront(document.RootElement, "prefixes", "ip_prefix", result);
                        AddAwsCloudFront(document.RootElement, "ipv6_prefixes", "ipv6_prefix", result);
                    }
                }
            }
            if (result.Count == 0)
            {
                throw new FormatException("A provider list contained no ranges.");
            }
            foreach (string entry in result)
            {
                IpNetwork.Parse(entry);
            }
            return result;
        }

        private static void AddArray(JsonElement root, string property, List<string> result)
        {
            JsonElement array;
            if (!root.TryGetProperty(property, out array) || array.ValueKind != JsonValueKind.Array)
            {
                throw new FormatException("Provider JSON has no array '" + property + "'.");
            }
            foreach (JsonElement item in array.EnumerateArray())
            {
                result.Add(item.GetString());
            }
        }

        private static void AddAwsCloudFront(JsonElement root, string property, string cidrProperty, List<string> result)
        {
            JsonElement array;
            if (!root.TryGetProperty(property, out array) || array.ValueKind != JsonValueKind.Array)
            {
                throw new FormatException("Provider JSON has no array '" + property + "'.");
            }
            foreach (JsonElement item in array.EnumerateArray())
            {
                string service = item.GetProperty("service").GetString();
                if (string.Equals(service, "CLOUDFRONT", StringComparison.Ordinal))
                {
                    result.Add(item.GetProperty(cidrProperty).GetString());
                }
            }
        }
    }
}
