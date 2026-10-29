// Copyright © Anointed Automation Corporation., 2024. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2024-10-13 01:57:25 Jesus is King ✝️
// Edited by Alexander Fields https://www.alexanderfields.me 2025-07-02 11:48:25
// Edited 2026-10-28: missing client IP is a throttled Warning (was Critical per request); log lines name method + path.
//Stewarded by Alexander Fields

using AnointedAutomation.Optimization.Logging;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

using System;
using System.Linq;

namespace AnointedAutomation.APIMiddleware.Utility
{
    /// <summary>
    /// Utility class for handling API-related operations, particularly IP address extraction.
    /// </summary>
    public class APIUtility
    {
        /// <summary>The address returned when no usable client IP exists (TEST-NET-2, never routable).</summary>
        internal const string UnknownClientIp = "198.51.100.255";

        /// <summary>One "no client IP" warning per 5 minutes, with the count of the ones held back.</summary>
        internal static readonly LogThrottle MissingIpThrottle = new LogThrottle(TimeSpan.FromMinutes(5));

        /// <summary>
        /// "METHOD /path" for log lines (no query string, which can carry tokens). Never throws.
        /// </summary>
        /// <param name="context">The request, or null.</param>
        /// <returns>The description.</returns>
        internal static string DescribeRequest(HttpContext context)
        {
            if (context == null || context.Request == null)
            {
                return "(no HttpContext)";
            }
            return $"{context.Request.Method} {context.Request.Path}";
        }

        /// <summary>
        /// Records a request that carried no client IP at all. Not an outage (health checks, Unix-socket fronting,
        /// in-memory hosts), so it is a Warning, at most once per window, with the number held back.
        /// </summary>
        /// <param name="context">The request.</param>
        private static void LogMissingClientIp(HttpContext context)
        {
            if (!MissingIpThrottle.TryEnter(DateTime.UtcNow, out int suppressed))
            {
                return;
            }
            IPBlacklistMiddleware.AddLog(
                LogMessage.Warning(
                    $"APIUtility.GetClientPublicIPAddress: no client IP for {DescribeRequest(context)} (no X-Forwarded-For, null RemoteIpAddress); using {UnknownClientIp}. "
                    + $"{suppressed} more in the previous {MissingIpThrottle.Window.TotalMinutes:0} min were not logged."
                )
            );
        }

        /// <summary>
        /// Gets the client's public IP address from an HttpContext.
        /// </summary>
        /// <param name="context">The HTTP context containing request information.</param>
        /// <returns>The client's IP address, or a default IP if unable to determine.</returns>
        /// <remarks>
        /// This method checks the X-Forwarded-For header first, then falls back to the remote IP address.
        /// If the IP is ::1 (localhost) or if an error occurs, returns 198.51.100.255.
        /// </remarks>
        public static string GetClientPublicIPAddress(HttpContext context)
        {
            try
            {
                string ipAddress = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();

                if (string.IsNullOrEmpty(ipAddress))
                {
                    // Null-conditional: RemoteIpAddress is null for IP-less requests (DefaultHttpContext /
                    // TestServer / some health checks). Mirrors the ActionExecutingContext overload below.
                    ipAddress = context.Connection.RemoteIpAddress?.ToString();
                }

                if (string.IsNullOrEmpty(ipAddress))
                {
                    // NULL/empty = no socket peer at all (no X-Forwarded-For and a null RemoteIpAddress).
                    LogMissingClientIp(context);
                    return UnknownClientIp;
                }

                if (ipAddress == "::1")
                {
                    // Localhost loopback = a real but local connection (dev / same-host health checks); benign.
                    IPBlacklistMiddleware.AddLog(
                        LogMessage.Info($"Localhost (::1) client IP for {DescribeRequest(context)}; using {UnknownClientIp}")
                    );
                    return UnknownClientIp;
                }

                return ipAddress;
            }
            catch (System.Exception ex)
            {
                IPBlacklistMiddleware.AddLog(
                    LogMessage.Error($"APIUtility.GetClientPublicIPAddress: reading the client IP for {DescribeRequest(context)} failed; using {UnknownClientIp}", ex)
                );
                return UnknownClientIp;
            }
        }

        /// <summary>
        /// Gets the client's public IP address from an ActionExecutingContext.
        /// </summary>
        /// <param name="context">The action executing context containing request information.</param>
        /// <returns>The client's IP address, or a default IP if unable to determine.</returns>
        /// <remarks>
        /// This method checks the X-Forwarded-For header first, then falls back to the remote IP address.
        /// If the IP is ::1 (localhost) or if invalid, returns 198.51.100.255.
        /// </remarks>
        public static string GetClientPublicIPAddress(ActionExecutingContext context)
        {
            string ipAddress = string.Empty;

            // Check if the X-Forwarded-For header has any value
            if (context.HttpContext.Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                ipAddress = context.HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            }

            // If X-Forwarded-For is empty, use the remote IP address
            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = context.HttpContext.Connection.RemoteIpAddress?.ToString();
            }

            // NULL/empty = no socket peer at all.
            if (string.IsNullOrEmpty(ipAddress))
            {
                LogMissingClientIp(context.HttpContext);
                return UnknownClientIp;
            }

            // Localhost loopback = a real but local connection (dev / same-host health checks); benign.
            if (ipAddress.Equals("::1"))
            {
                IPBlacklistMiddleware.AddLog(
                    LogMessage.Info($"Localhost (::1) client IP for {DescribeRequest(context.HttpContext)}; using {UnknownClientIp}")
                );
                return UnknownClientIp;
            }

            return ipAddress;
        }
    }
}
