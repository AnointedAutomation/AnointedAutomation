// Copyright © Anointed Automation, LLC., 2024. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2024-10-13 01:57:25
// Edited by Alexander Fields https://www.alexanderfields.me 2025-07-02 11:48:25
//Stewarded by Alexander Fields

using AnointedAutomation.Optimization.Logging;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

using System.Linq;

namespace AnointedAutomation.APIMiddleware.Utility
{
    /// <summary>
    /// Utility class for handling API-related operations, particularly IP address extraction.
    /// </summary>
    public class APIUtility
    {
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
                    // Anomalous in production (Unix-socket fronting, a synthetic DefaultHttpContext, or an
                    // in-memory TestServer) -> error-level so it is not silently lost.
                    IPBlacklistMiddleware.AddLog(
                        LogMessage.Critical(
                            $"No client IP available from {context.ToString()} (no X-Forwarded-For, null RemoteIpAddress); using fallback 198.51.100.255"
                        )
                    );
                    return "198.51.100.255";
                }

                if (ipAddress == "::1")
                {
                    // Localhost loopback = a real but local connection (dev / same-host health checks); benign.
                    IPBlacklistMiddleware.AddLog(
                        LogMessage.Warning(
                            $"Localhost (::1) client IP from {context.ToString()}; using fallback 198.51.100.255"
                        )
                    );
                    return "198.51.100.255";
                }

                return ipAddress;
            }
            catch (System.Exception ex)
            {
                IPBlacklistMiddleware.AddLog(
                    LogMessage.Critical(
                        $" There was a problem getting the IP Address from {context.ToString()}. It was assigned it an arbitrary 198.51.100.255 {ex.ToString()}"
                    )
                );
                return "198.51.100.255";
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

            // NULL/empty = no socket peer at all -> error-level (anomalous in production).
            if (string.IsNullOrEmpty(ipAddress))
            {
                IPBlacklistMiddleware.AddLog(
                    LogMessage.Critical(
                        $"No client IP available from {context.ToString()} (no X-Forwarded-For, null RemoteIpAddress); using fallback 198.51.100.255"
                    )
                );
                return "198.51.100.255";
            }

            // Localhost loopback = a real but local connection (dev / same-host health checks); benign.
            if (ipAddress.Equals("::1"))
            {
                IPBlacklistMiddleware.AddLog(
                    LogMessage.Warning(
                        $"Localhost (::1) client IP from {context.ToString()}; using fallback 198.51.100.255"
                    )
                );
                return "198.51.100.255";
            }

            return ipAddress;
        }
    }
}
