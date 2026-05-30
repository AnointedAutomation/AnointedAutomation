// Copyright © Anointed Automation, LLC., 2024. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2024-10-13 01:57:25
// Edited by Alexander Fields https://www.alexanderfields.me 2025-07-02 11:48:25
//Stewarded by Alexander Fields
using AnointedAutomation.APIMiddleware.Objects;
using AnointedAutomation.APIMiddleware.Utility;
using AnointedAutomation.Optimization.Logging;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnointedAutomation.APIMiddleware
{
    /// <summary>
    /// Middleware for blocking blacklisted IP addresses from accessing the API.
    /// </summary>
    public class IPBlacklistMiddleware
    {
        private static readonly ConcurrentQueue<LogMessage> middlewareLogs = new ConcurrentQueue<LogMessage>();
        private readonly RequestDelegate _next;

        /// <summary>
        /// Initializes a new instance of the <see cref="IPBlacklistMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline.</param>
        public IPBlacklistMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Event triggered when a log message is added.
        /// </summary>
        public static event EventHandler<LogMessageEventArgs> LogAdded;

        /// <summary>
        /// Event triggered when logs are cleared.
        /// </summary>
        public static event EventHandler LogCleared;

        /// <summary>
        /// Adds a log message to the middleware logs.
        /// </summary>
        /// <param name="logMessage">The log message to add.</param>
        public static void AddLog(LogMessage logMessage)
        {
            middlewareLogs.Enqueue(logMessage);
            LogAdded?.Invoke(null, new LogMessageEventArgs(logMessage));
        }

        /// <summary>
        /// Clears all middleware logs and event handlers.
        /// </summary>
        public static void ClearLogs()
        {
            LogCleared?.Invoke(null, EventArgs.Empty);
            middlewareLogs.Clear();
            // Clear all event handlers for test isolation
            LogAdded = null;
            LogCleared = null;
        }

        /// <summary>
        /// Gets all middleware logs.
        /// </summary>
        /// <returns>A list of all log messages.</returns>
        public static IList<LogMessage> GetLogs()
        {
            return middlewareLogs.ToList();
        }

        /// <summary>
        /// Invokes the middleware to check if the client IP is blacklisted.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            string clientIP = APIUtility.GetClientPublicIPAddress(context);

            if (IPBlacklist.IsIPBlocked(clientIP))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                AddLog(LogMessage.Message($"{clientIP} is blocked.")); // Example log addition
                return;
            }

            await _next(context);
        }
    }
}
