// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
using System;
using System.Collections.Generic;
using System.Threading;

namespace AnointedAutomation.Optimization.Logging
{
    /// <summary>
    /// Holds the <see cref="LogMessage.LogAdded"/> subscribers and the opt-in bounded startup buffer. Once
    /// <see cref="EnableBuffering"/> is called and until the first subscriber ever attaches, raised messages are queued (oldest dropped past the capacity); that subscriber receives the queue
    /// in order, then one Warning with the dropped count when any were dropped. After that, nothing is buffered again,
    /// so a later subscriber never receives stale entries.
    /// </summary>
    internal sealed class LogDispatcher
    {
        private readonly object _sync = new object();
        private readonly Queue<LogMessage> _pending = new Queue<LogMessage>();
        private readonly int _capacity;
        private LogMessage.LogAddedEventHandler _handlers;
        private bool _everSubscribed;
        private bool _buffering;
        private long _dropped;

        /// <summary>
        /// Creates a dispatcher whose startup buffer holds at most <paramref name="capacity"/> messages.
        /// </summary>
        /// <param name="capacity">Buffer capacity; must be positive.</param>
        internal LogDispatcher(int capacity)
        {
            if (capacity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity), capacity, "The pre-subscription buffer capacity must be positive.");
            }
            _capacity = capacity;
        }

        /// <summary>
        /// Messages currently waiting for the first subscriber.
        /// </summary>
        internal int PendingCount
        {
            get
            {
                lock (_sync)
                {
                    return _pending.Count;
                }
            }
        }

        /// <summary>
        /// Turns on the startup buffer. Has no effect once a subscriber has attached (nothing would ever flush it).
        /// </summary>
        /// <returns>True when buffering is now on; false when a subscriber already attached.</returns>
        internal bool EnableBuffering()
        {
            lock (_sync)
            {
                if (_everSubscribed)
                {
                    return false;
                }
                _buffering = true;
                return true;
            }
        }

        /// <summary>
        /// Adds a subscriber; the first one ever also receives the buffered startup messages.
        /// </summary>
        /// <param name="handler">The subscriber; null is ignored, as with a plain event.</param>
        internal void Subscribe(LogMessage.LogAddedEventHandler handler)
        {
            if (handler == null)
            {
                return;
            }
            LogMessage[] pending;
            long dropped;
            lock (_sync)
            {
                _handlers += handler;
                if (_everSubscribed)
                {
                    return;
                }
                _everSubscribed = true;
                pending = _pending.ToArray();
                _pending.Clear();
                dropped = _dropped;
                _dropped = 0;
            }
            foreach (LogMessage log in pending)
            {
                // Startup logs were stamped before the host named itself; stamp them with the name it set since.
                if (string.Equals(log.messageSource, LogMessage.UnsetMessageSource, StringComparison.Ordinal))
                {
                    log.messageSource = LogMessage.MessageSourceSetter;
                }
                Invoke(handler, log);
            }
            if (dropped > 0)
            {
                LogMessage.Warning($"[AnointedAutomation.Logging] {dropped} log message(s) raised before the first LogAdded subscriber were dropped: the startup buffer holds {_capacity}. Subscribe to LogMessage.LogAdded earlier in startup.");
            }
        }

        /// <summary>
        /// Removes a subscriber.
        /// </summary>
        /// <param name="handler">The subscriber to remove.</param>
        internal void Unsubscribe(LogMessage.LogAddedEventHandler handler)
        {
            lock (_sync)
            {
                _handlers -= handler;
            }
        }

        /// <summary>
        /// Delivers a message to every subscriber in isolation, or buffers it while no subscriber has ever attached.
        /// </summary>
        /// <param name="log">The new message.</param>
        internal void Raise(LogMessage log)
        {
            LogMessage.LogAddedEventHandler handlers = Volatile.Read(ref _handlers);
            if (handlers == null)
            {
                lock (_sync)
                {
                    if (_buffering && !_everSubscribed)
                    {
                        if (_pending.Count >= _capacity)
                        {
                            _pending.Dequeue();
                            _dropped++;
                        }
                        _pending.Enqueue(log);
                        return;
                    }
                    handlers = _handlers;
                }
                if (handlers == null)
                {
                    return;
                }
            }
            foreach (Delegate handler in handlers.GetInvocationList())
            {
                Invoke((LogMessage.LogAddedEventHandler)handler, log);
            }
        }

        /// <summary>
        /// Runs one subscriber; a subscriber that throws is reported on <see cref="Console.Error"/> and never
        /// propagates into the code that merely logged.
        /// </summary>
        private static void Invoke(LogMessage.LogAddedEventHandler handler, LogMessage log)
        {
            try
            {
                handler(log, new LogMessageEventArgs(log));
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[AnointedAutomation.Logging] LogAdded subscriber {handler.Method.DeclaringType?.FullName}.{handler.Method.Name} threw while handling a {log.messageType} log from {log.localOperationName}: {ex}");
            }
        }
    }
}
