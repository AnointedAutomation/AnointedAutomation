// Copyright © Anointed Automation, LLC., 2024. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2024-10-15 18:02:13
// Edited by Alexander Fields https://www.alexanderfields.me 2025-07-02 11:48:25
﻿using AnointedAutomation.Optimization.Memory;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

//Stewarded by Alexander Fields

namespace AnointedAutomation.APIMiddleware
{
    /// <summary>
    /// Middleware for tracking endpoint access and managing memory cleanup based on endpoint idle time.
    /// </summary>
    public class EndpointAccessMiddleware
    {
        private static readonly ConcurrentDictionary<string, DateTime> _lastAccessed = new ConcurrentDictionary<string, DateTime>();
        private readonly bool _cleanMem;
        private readonly GarbageCollection _garbageCollection;
        private readonly RequestDelegate _next;
        private readonly TimeSpan? _timeout;
        private readonly Timer _timer;

        /// <summary>
        /// Middleware for checking if all endpoints have been idle for a certain amount of time and optionally clean memory.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline</param>
        /// <param name="timeout">Interval in which you want to check when the last endpoint was hit. If null, no timeout checking will be performed.</param>
        /// <param name="cleanMem">Whether memory cleaning should be performed. Defaults to false.</param>
        public EndpointAccessMiddleware(RequestDelegate next, TimeSpan? timeout, bool cleanMem = false)
        {
            _next = next;
            _timeout = timeout;
            _cleanMem = cleanMem;
            _garbageCollection = new GarbageCollection();

            // Set up a timer to check for garbage collection only if memory cleaning is enabled
            if (_cleanMem)
            {
                _timer = new Timer(CheckForGarbageCollection, null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));
            }
        }

        /// <summary>
        /// Checks if a specific endpoint has been accessed within the last 5 minutes.
        /// </summary>
        /// <param name="path">The endpoint path to check.</param>
        /// <returns>True if the endpoint has been accessed recently, false otherwise.</returns>
        public static bool HasBeenHitRecently(string path)
        {
            if (_lastAccessed.TryGetValue(path, out DateTime lastAccessed))
            {
                return (DateTime.UtcNow - lastAccessed) < TimeSpan.FromMinutes(5);
            }
            return false; // Endpoint hasn't been hit recently
        }

        /// <summary>
        /// Invokes the middleware to track endpoint access.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task Invoke(HttpContext context)
        {
            string path = context.Request.Path.ToString();

            // Update the last accessed time
            _lastAccessed[path] = DateTime.UtcNow;

            await _next(context);
        }

        /// <summary>
        /// Checks if all endpoints have been idle for longer than the specified timeout.
        /// </summary>
        /// <returns>True if all endpoints are idle, false otherwise.</returns>
        private bool AreAllEndpointsIdle()
        {
            if (!_timeout.HasValue)
            {
                return false; // No timeout specified, so skip idle check
            }

            DateTime now = DateTime.UtcNow;

            // Check if any endpoint has been accessed within the specified timeout
            foreach (DateTime lastAccessedTime in _lastAccessed.Values)
            {
                if ((now - lastAccessedTime) < _timeout.Value)
                {
                    return false; // At least one endpoint has been hit recently
                }
            }

            return true; // No endpoints have been hit recently
        }

        /// <summary>
        /// Timer callback to check if garbage collection should be performed based on endpoint idle time.
        /// </summary>
        /// <param name="state">The state object (unused).</param>
        private void CheckForGarbageCollection(object state)
        {
            if (_timeout.HasValue && AreAllEndpointsIdle())
            {
                _garbageCollection.PerformGarbageCollection(null);
            }
        }
    }
}
