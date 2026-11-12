// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace AnointedAutomation.Optimization.Logging
{
    /// <summary>
    /// Structured properties, scopes and categories for <see cref="LogMessage"/>.
    /// </summary>
    public partial class LogMessage
    {
        private static readonly AsyncLocal<ScopeFrame> CurrentScope = new AsyncLocal<ScopeFrame>();

        /// <summary>
        /// Opens a property scope for the current async flow: every message created inside it (including in awaited
        /// calls and tasks started inside it) carries these properties. Dispose it to close the scope; nest freely.
        /// <code>using IDisposable scope = LogMessage.Scope(("orderId", orderId), ("store", storeSlug));</code>
        /// </summary>
        /// <param name="properties">Key/value pairs; a null or empty key is ignored.</param>
        /// <returns>The scope; dispose it to restore the enclosing one.</returns>
        public static IDisposable Scope(params (string Key, object Value)[] properties)
        {
            if (properties == null)
            {
                throw new ArgumentNullException(nameof(properties));
            }
            ScopeFrame frame = new ScopeFrame(CurrentScope.Value, (ValueTuple<string, object>[])properties.Clone());
            CurrentScope.Value = frame;
            return frame;
        }

        /// <summary>
        /// Returns a logger whose messages carry <c>typeof(T).FullName</c> as their <see cref="category"/> instead of
        /// the caller file name (useful for partial classes, nested types and shared helpers).
        /// <code>private static readonly LogSource Log = LogMessage.For&lt;OrderService&gt;();</code>
        /// </summary>
        /// <typeparam name="T">The type whose full name becomes the category.</typeparam>
        /// <returns>A category-bound logger.</returns>
        public static LogSource For<T>()
        {
            return new LogSource(typeof(T).FullName);
        }

        /// <summary>
        /// Returns a logger whose messages carry <paramref name="category"/> as their <see cref="category"/>.
        /// </summary>
        /// <param name="category">The category; must not be null or empty.</param>
        /// <returns>A category-bound logger.</returns>
        public static LogSource For(string category)
        {
            if (string.IsNullOrEmpty(category))
            {
                throw new ArgumentException("A log category must not be null or empty.", nameof(category));
            }
            return new LogSource(category);
        }

        /// <summary>
        /// Creates and raises a factory message with an explicit category (used by <see cref="LogSource"/>).
        /// </summary>
        internal static LogMessage Create(MessageType messageType, string message, Exception exception, string category, (string Key, object Value)[] callProperties, string callerMemberName, string callerFilePath, int callerLineNumber)
        {
            return new LogMessage(0, messageType, message, exception, category, callProperties, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Creates and raises a message from a bridge (for example <see cref="LogMessageLogger"/>) that has no compiler
        /// caller information: the operation name is supplied instead of walked from the stack.
        /// </summary>
        internal static LogMessage Emit(MessageType messageType, string message, Exception exception, string category, string operationName, (string Key, object Value)[] callProperties)
        {
            LogMessage log = new LogMessage
            {
                timeStamp = DateTime.UtcNow,
                messageSource = MessageSourceSetter,
                localOperationName = operationName,
                messageType = messageType,
                category = category,
            };
            if (exception == null)
            {
                log.message = message;
            }
            else
            {
                log.message = message + Environment.NewLine + exception.ToString();
                log.exceptionType = exception.GetType().FullName;
            }
            ApplyProperties(log, callProperties);
            RaiseLogAdded(log);
            return log;
        }

        /// <summary>
        /// Packs the inline call-site properties of a factory, skipping unused (null-key) slots.
        /// </summary>
        internal static (string Key, object Value)[] Pack((string Key, object Value) p1, (string Key, object Value) p2, (string Key, object Value) p3, (string Key, object Value) p4)
        {
            List<(string Key, object Value)> list = new List<(string Key, object Value)>(4);
            if (!string.IsNullOrEmpty(p1.Key)) list.Add(p1);
            if (!string.IsNullOrEmpty(p2.Key)) list.Add(p2);
            if (!string.IsNullOrEmpty(p3.Key)) list.Add(p3);
            if (!string.IsNullOrEmpty(p4.Key)) list.Add(p4);
            return list.ToArray();
        }

        /// <summary>
        /// Merges the active scopes (outermost first) and the call-site properties (last, so they win) into
        /// <see cref="structuredProperties"/> and <see cref="properties"/>. Leaves both null when there are none.
        /// </summary>
        internal static void ApplyProperties(LogMessage log, (string Key, object Value)[] callProperties)
        {
            ScopeFrame scope = CurrentScope.Value;
            bool hasCall = callProperties != null && callProperties.Length > 0;
            if (scope == null && !hasCall)
            {
                return;
            }
            Stack<ScopeFrame> frames = new Stack<ScopeFrame>();
            for (ScopeFrame frame = scope; frame != null; frame = frame.Parent)
            {
                frames.Push(frame);
            }
            Dictionary<string, object> merged = new Dictionary<string, object>(StringComparer.Ordinal);
            foreach (ScopeFrame frame in frames)
            {
                Add(merged, frame.Properties);
            }
            if (hasCall)
            {
                Add(merged, callProperties);
            }
            if (merged.Count == 0)
            {
                return;
            }
            log.structuredProperties = merged;
            log.properties = ToJson(merged);
        }

        private static void Add(Dictionary<string, object> merged, (string Key, object Value)[] items)
        {
            foreach ((string key, object value) in items)
            {
                if (!string.IsNullOrEmpty(key))
                {
                    merged[key] = value;
                }
            }
        }

        /// <summary>
        /// Serializes properties as a flat JSON object. Strings, booleans and numbers keep their JSON type; dates use the
        /// round-trip ("O") format; enums use their name; anything else is written as its invariant-culture text, so a
        /// property can never make logging throw.
        /// </summary>
        internal static string ToJson(IReadOnlyDictionary<string, object> values)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (Utf8JsonWriter writer = new Utf8JsonWriter(stream))
                {
                    writer.WriteStartObject();
                    foreach (KeyValuePair<string, object> pair in values)
                    {
                        writer.WritePropertyName(pair.Key);
                        WriteValue(writer, pair.Value);
                    }
                    writer.WriteEndObject();
                }
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        private static void WriteValue(Utf8JsonWriter writer, object value)
        {
            switch (value)
            {
                case null:
                    writer.WriteNullValue();
                    break;
                case string text:
                    writer.WriteStringValue(text);
                    break;
                case bool flag:
                    writer.WriteBooleanValue(flag);
                    break;
                case Enum enumValue:
                    writer.WriteStringValue(enumValue.ToString());
                    break;
                case int number:
                    writer.WriteNumberValue(number);
                    break;
                case long number:
                    writer.WriteNumberValue(number);
                    break;
                case short number:
                    writer.WriteNumberValue(number);
                    break;
                case byte number:
                    writer.WriteNumberValue(number);
                    break;
                case uint number:
                    writer.WriteNumberValue(number);
                    break;
                case ulong number:
                    writer.WriteNumberValue(number);
                    break;
                case decimal number:
                    writer.WriteNumberValue(number);
                    break;
                case double number when double.IsFinite(number):
                    writer.WriteNumberValue(number);
                    break;
                case float number when float.IsFinite(number):
                    writer.WriteNumberValue(number);
                    break;
                case DateTime date:
                    writer.WriteStringValue(date.ToString("O", CultureInfo.InvariantCulture));
                    break;
                case DateTimeOffset date:
                    writer.WriteStringValue(date.ToString("O", CultureInfo.InvariantCulture));
                    break;
                case IFormattable formattable:
                    writer.WriteStringValue(formattable.ToString(null, CultureInfo.InvariantCulture));
                    break;
                default:
                    writer.WriteStringValue(value.ToString());
                    break;
            }
        }

        /// <summary>
        /// One level of <see cref="Scope"/>: an immutable link to its parent, so concurrent flows never share state.
        /// </summary>
        private sealed class ScopeFrame : IDisposable
        {
            internal ScopeFrame(ScopeFrame parent, (string Key, object Value)[] properties)
            {
                Parent = parent;
                Properties = properties;
            }

            internal ScopeFrame Parent { get; }

            internal (string Key, object Value)[] Properties { get; }

            public void Dispose()
            {
                // Restore the enclosing scope only if this one is still current (a double dispose is a no-op).
                if (ReferenceEquals(CurrentScope.Value, this))
                {
                    CurrentScope.Value = Parent;
                }
            }
        }
    }
}
