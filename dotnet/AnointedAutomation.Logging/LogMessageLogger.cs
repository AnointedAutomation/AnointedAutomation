// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace AnointedAutomation.Optimization.Logging
{
    /// <summary>
    /// An <see cref="ILogger"/> that writes through <see cref="LogMessage"/>, for third-party APIs that require an
    /// ILogger. Our own code calls <see cref="LogMessage"/> directly. Structured state pairs become properties (the
    /// "{OriginalFormat}" pair is stored as "template"), a non-zero EventId becomes "eventId", and BeginScope opens a
    /// <see cref="LogMessage.Scope"/>.
    /// </summary>
    public sealed class LogMessageLogger : ILogger
    {
        private readonly string _category;
        private readonly LogLevel _minimumLevel;

        /// <summary>
        /// Creates the adapter.
        /// </summary>
        /// <param name="category">Category stamped on every message (usually the SDK's logger name).</param>
        /// <param name="minimumLevel">Lowest level written; <see cref="LogLevel.None"/> writes nothing.</param>
        public LogMessageLogger(string category, LogLevel minimumLevel)
        {
            if (string.IsNullOrEmpty(category))
            {
                throw new ArgumentException("A logger category must not be null or empty.", nameof(category));
            }
            _category = category;
            _minimumLevel = minimumLevel;
        }

        /// <inheritdoc />
        public IDisposable BeginScope<TState>(TState state) where TState : notnull
        {
            List<(string Key, object Value)> pairs = new List<(string Key, object Value)>();
            if (state is IEnumerable<KeyValuePair<string, object>> values)
            {
                foreach (KeyValuePair<string, object> pair in values)
                {
                    if (!string.Equals(pair.Key, "{OriginalFormat}", StringComparison.Ordinal))
                    {
                        pairs.Add((pair.Key, pair.Value));
                    }
                }
            }
            else
            {
                pairs.Add(("scope", state.ToString()));
            }
            return LogMessage.Scope(pairs.ToArray());
        }

        /// <inheritdoc />
        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None && _minimumLevel != LogLevel.None && logLevel >= _minimumLevel;
        }

        /// <inheritdoc />
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }
            if (!IsEnabled(logLevel))
            {
                return;
            }
            List<(string Key, object Value)> pairs = new List<(string Key, object Value)>();
            if (state is IEnumerable<KeyValuePair<string, object>> values)
            {
                foreach (KeyValuePair<string, object> pair in values)
                {
                    string key = string.Equals(pair.Key, "{OriginalFormat}", StringComparison.Ordinal) ? "template" : pair.Key;
                    pairs.Add((key, pair.Value));
                }
            }
            if (eventId.Id != 0)
            {
                pairs.Add(("eventId", eventId.Id));
            }
            LogMessage.Emit(ToMessageType(logLevel), formatter(state, exception), exception, _category, _category, pairs.ToArray());
        }

        /// <summary>
        /// Maps an ILogger level to a <see cref="MessageType"/>.
        /// </summary>
        /// <param name="logLevel">The ILogger level (not None).</param>
        /// <returns>The matching message type.</returns>
        public static MessageType ToMessageType(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                    return MessageType.Trace;
                case LogLevel.Debug:
                    return MessageType.Debug;
                case LogLevel.Information:
                    return MessageType.Informational;
                case LogLevel.Warning:
                    return MessageType.Warning;
                case LogLevel.Error:
                    return MessageType.Error;
                case LogLevel.Critical:
                    return MessageType.Critical;
                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, "LogLevel.None has no message type.");
            }
        }
    }

    /// <summary>
    /// <see cref="ILoggerProvider"/> creating <see cref="LogMessageLogger"/>s, for SDKs that take an ILoggerFactory:
    /// <c>new LoggerFactory(new[] { new LogMessageLoggerProvider(LogLevel.Warning) })</c>.
    /// </summary>
    public sealed class LogMessageLoggerProvider : ILoggerProvider
    {
        private readonly LogLevel _minimumLevel;

        /// <summary>
        /// Creates the provider.
        /// </summary>
        /// <param name="minimumLevel">Lowest level its loggers write.</param>
        public LogMessageLoggerProvider(LogLevel minimumLevel)
        {
            _minimumLevel = minimumLevel;
        }

        /// <inheritdoc />
        public ILogger CreateLogger(string categoryName)
        {
            return new LogMessageLogger(categoryName, _minimumLevel);
        }

        /// <inheritdoc />
        public void Dispose()
        {
        }
    }
}
