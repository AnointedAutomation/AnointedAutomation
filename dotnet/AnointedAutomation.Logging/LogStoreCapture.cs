// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
using System;
using System.Collections.Generic;
using System.Threading;

namespace AnointedAutomation.Optimization.Logging
{
    /// <summary>
    /// Test helper that records every <see cref="LogMessage"/> created in the current async flow while it is open.
    /// Isolation is per async flow (<see cref="AsyncLocal{T}"/>), so parallel xUnit/NUnit tests never see each other's
    /// logs, and code awaited or started as a task inside the capture is included. Work that does not flow the
    /// execution context (for example a timer created before the capture) is not recorded.
    /// <code>
    /// using LogStoreCapture capture = LogStoreCapture.Start();
    /// service.Refund(order);
    /// Assert.Contains(capture.Entries, e => e.messageType == MessageType.Warning);
    /// </code>
    /// Captures nest: an inner capture's entries also reach the outer one.
    /// </summary>
    public sealed class LogStoreCapture : IDisposable
    {
        private static readonly AsyncLocal<LogStoreCapture> Current = new AsyncLocal<LogStoreCapture>();

        private readonly LogStoreCapture _parent;
        private readonly List<LogMessage> _entries = new List<LogMessage>();
        private volatile bool _disposed;

        private LogStoreCapture(LogStoreCapture parent)
        {
            _parent = parent;
        }

        /// <summary>
        /// Starts recording for the current async flow.
        /// </summary>
        /// <returns>The capture; dispose it to stop recording.</returns>
        public static LogStoreCapture Start()
        {
            LogStoreCapture capture = new LogStoreCapture(Current.Value);
            Current.Value = capture;
            return capture;
        }

        /// <summary>
        /// A snapshot of the recorded messages, in creation order.
        /// </summary>
        public IReadOnlyList<LogMessage> Entries
        {
            get
            {
                lock (_entries)
                {
                    return _entries.ToArray();
                }
            }
        }

        /// <summary>
        /// The recorded messages of one level, in creation order.
        /// </summary>
        /// <param name="messageType">The level to keep.</param>
        /// <returns>The matching messages.</returns>
        public IReadOnlyList<LogMessage> OfLevel(MessageType messageType)
        {
            List<LogMessage> matches = new List<LogMessage>();
            foreach (LogMessage log in Entries)
            {
                if (log.messageType == messageType)
                {
                    matches.Add(log);
                }
            }
            return matches;
        }

        /// <summary>
        /// Whether a message of <paramref name="messageType"/> containing <paramref name="fragment"/> (ordinal) was recorded.
        /// </summary>
        /// <param name="messageType">The level.</param>
        /// <param name="fragment">Text the message must contain.</param>
        /// <returns>True when one was recorded.</returns>
        public bool Contains(MessageType messageType, string fragment)
        {
            if (fragment == null)
            {
                throw new ArgumentNullException(nameof(fragment));
            }
            foreach (LogMessage log in OfLevel(messageType))
            {
                if (log.message != null && log.message.Contains(fragment, StringComparison.Ordinal))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Stops recording and restores the enclosing capture of this flow.
        /// </summary>
        public void Dispose()
        {
            _disposed = true;
            if (ReferenceEquals(Current.Value, this))
            {
                Current.Value = _parent;
            }
        }

        /// <summary>
        /// Records a new message in every open capture of the current flow (called by <see cref="LogMessage"/>).
        /// </summary>
        /// <param name="log">The new message.</param>
        internal static void Record(LogMessage log)
        {
            for (LogStoreCapture capture = Current.Value; capture != null; capture = capture._parent)
            {
                if (capture._disposed)
                {
                    continue;
                }
                lock (capture._entries)
                {
                    capture._entries.Add(log);
                }
            }
        }
    }
}
