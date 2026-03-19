// Copyright © Anointed Automation, LLC., 2024. All Rights Reserved. Created by Alexander Fields https://www.alexanderfields.me on 2024-10-13 01:57:25
// Edited by Alexander Fields https://www.alexanderfields.me 2025-07-02 11:48:25
//Created by Alexander Fields
using AnointedAutomation.Optimization.Logging;
using System;
using System.Collections.Generic;

namespace AnointedAutomation.APIMiddleware.Objects
{
    /// <summary>
    /// Static class for managing blacklisted IP addresses.
    /// </summary>
    public static class IPBlacklist
    {
        private static readonly Dictionary<string, string> blacklistedIPs = new Dictionary<string, string>();

        // Event declaration for when a new IP is added
        /// <summary>
        /// Event triggered when a new IP address is banned.
        /// </summary>
        public static event EventHandler<BannedIP> IPBanned;

        /// <summary>
        /// Event triggered when an IP address is unbanned.
        /// </summary>
        public static event EventHandler<string> IPUnbanned;

        // Method to add a banned IP with a reason
        /// <summary>
        /// Adds an IP address to the blacklist with a specified reason.
        /// </summary>
        /// <param name="ip">The IP address to ban.</param>
        /// <param name="reason">The reason for banning the IP.</param>
        public static void AddBannedIP(string ip, string reason)
        {
            if (string.IsNullOrWhiteSpace(ip))
            {
                IPBlacklistMiddleware.AddLog(
                    LogMessage.Warning("Attempted to add null or empty IP to blacklist. Ignoring.")
                );
                return;
            }

            if (!blacklistedIPs.ContainsKey(ip))
            {
                blacklistedIPs[ip] = reason;
                IPBanned?.Invoke(null, new BannedIP(null, ip, null, reason));
            }
        }

        // Optionally, provide a method to get the block reason for a specific IP
        /// <summary>
        /// Gets the reason why an IP address was blocked.
        /// </summary>
        /// <param name="ipAddress">The IP address to check.</param>
        /// <returns>The reason for blocking the IP, or null if the IP is not blocked.</returns>
        public static string GetBlockReason(string ipAddress)
        {
            if (string.IsNullOrWhiteSpace(ipAddress))
            {
                return null;
            }

            if (blacklistedIPs.TryGetValue(ipAddress, out string reason))
            {
                return reason;
            }
            return null; // Or "Not blocked" or any other indicator that the IP is not in the blacklist
        }

        // Check if an IP is blocked and log the reason
        /// <summary>
        /// Checks if an IP address is currently blacklisted.
        /// </summary>
        /// <param name="ipAddress">The IP address to check.</param>
        /// <returns>True if the IP is blocked, false otherwise.</returns>
        public static bool IsIPBlocked(string ipAddress)
        {
            if (string.IsNullOrWhiteSpace(ipAddress))
            {
                return false;
            }

            if (blacklistedIPs.ContainsKey(ipAddress))
            {
                string reason = blacklistedIPs[ipAddress];
                IPBlacklistMiddleware.AddLog(LogMessage.Warning($"{ipAddress} was already blocked! Reason: {reason}"));
                return true;
            }
            return false;
        }

        /// <summary>
        /// Removes an IP address from the blacklist.
        /// </summary>
        /// <param name="ip">The IP address to unban.</param>
        /// <returns>True if the IP was removed, false if it wasn't in the blacklist.</returns>
        public static bool RemoveBannedIP(string ip)
        {
            if (string.IsNullOrWhiteSpace(ip))
            {
                return false;
            }

            if (blacklistedIPs.Remove(ip))
            {
                IPUnbanned?.Invoke(null, ip);
                IPBlacklistMiddleware.AddLog(
                    LogMessage.Informational($"IP {ip} removed from blacklist")
                );
                return true;
            }

            return false;
        }

        // Method to clear all blacklisted IPs (for testing purposes)
        /// <summary>
        /// Clears all blacklisted IPs and event handlers (for testing purposes).
        /// </summary>
        public static void ClearBlacklist()
        {
            blacklistedIPs.Clear();
            // Clear all event handlers
            IPBanned = null;
            IPUnbanned = null;
        }
    }
}
