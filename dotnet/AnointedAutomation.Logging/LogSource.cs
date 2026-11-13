// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
using System;
using System.Runtime.CompilerServices;

namespace AnointedAutomation.Optimization.Logging
{
    /// <summary>
    /// A logger bound to one category, from <see cref="LogMessage.For{T}"/> or <see cref="LogMessage.For(string)"/>.
    /// Hold it in a static field; each call creates and raises one <see cref="LogMessage"/> with the exact caller file:line.
    /// </summary>
    public sealed class LogSource
    {
        internal LogSource(string category)
        {
            Category = category;
        }

        /// <summary>
        /// The category stamped on every message from this logger.
        /// </summary>
        public string Category { get; }

        /// <summary>
        /// Creates a MessageType.Trace message in this category and raises <see cref="LogMessage.LogAdded"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="p1">An optional property such as <c>("orderId", orderId)</c>.</param>
        /// <param name="p2">An optional second property.</param>
        /// <param name="p3">An optional third property.</param>
        /// <param name="p4">An optional fourth property.</param>
        /// <param name="callerMemberName">Supplied by the compiler; do not pass.</param>
        /// <param name="callerFilePath">Supplied by the compiler; do not pass.</param>
        /// <param name="callerLineNumber">Supplied by the compiler; do not pass.</param>
        /// <returns>The message.</returns>
        public LogMessage Trace(string message, (string Key, object Value) p1 = default, (string Key, object Value) p2 = default, (string Key, object Value) p3 = default, (string Key, object Value) p4 = default, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            return LogMessage.Create(MessageType.Trace, message, null, Category, LogMessage.Pack(p1, p2, p3, p4), callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Creates a MessageType.Trace message with the full exception in this category and raises <see cref="LogMessage.LogAdded"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception; its full text (type, inner exceptions, stack) is appended. Null logs the message alone.</param>
        /// <param name="p1">An optional property such as <c>("orderId", orderId)</c>.</param>
        /// <param name="p2">An optional second property.</param>
        /// <param name="p3">An optional third property.</param>
        /// <param name="p4">An optional fourth property.</param>
        /// <param name="callerMemberName">Supplied by the compiler; do not pass.</param>
        /// <param name="callerFilePath">Supplied by the compiler; do not pass.</param>
        /// <param name="callerLineNumber">Supplied by the compiler; do not pass.</param>
        /// <returns>The message.</returns>
        public LogMessage Trace(string message, Exception exception, (string Key, object Value) p1 = default, (string Key, object Value) p2 = default, (string Key, object Value) p3 = default, (string Key, object Value) p4 = default, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            return LogMessage.Create(MessageType.Trace, message, exception, Category, LogMessage.Pack(p1, p2, p3, p4), callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Creates a MessageType.Debug message in this category and raises <see cref="LogMessage.LogAdded"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="p1">An optional property such as <c>("orderId", orderId)</c>.</param>
        /// <param name="p2">An optional second property.</param>
        /// <param name="p3">An optional third property.</param>
        /// <param name="p4">An optional fourth property.</param>
        /// <param name="callerMemberName">Supplied by the compiler; do not pass.</param>
        /// <param name="callerFilePath">Supplied by the compiler; do not pass.</param>
        /// <param name="callerLineNumber">Supplied by the compiler; do not pass.</param>
        /// <returns>The message.</returns>
        public LogMessage Debug(string message, (string Key, object Value) p1 = default, (string Key, object Value) p2 = default, (string Key, object Value) p3 = default, (string Key, object Value) p4 = default, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            return LogMessage.Create(MessageType.Debug, message, null, Category, LogMessage.Pack(p1, p2, p3, p4), callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Creates a MessageType.Debug message with the full exception in this category and raises <see cref="LogMessage.LogAdded"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception; its full text (type, inner exceptions, stack) is appended. Null logs the message alone.</param>
        /// <param name="p1">An optional property such as <c>("orderId", orderId)</c>.</param>
        /// <param name="p2">An optional second property.</param>
        /// <param name="p3">An optional third property.</param>
        /// <param name="p4">An optional fourth property.</param>
        /// <param name="callerMemberName">Supplied by the compiler; do not pass.</param>
        /// <param name="callerFilePath">Supplied by the compiler; do not pass.</param>
        /// <param name="callerLineNumber">Supplied by the compiler; do not pass.</param>
        /// <returns>The message.</returns>
        public LogMessage Debug(string message, Exception exception, (string Key, object Value) p1 = default, (string Key, object Value) p2 = default, (string Key, object Value) p3 = default, (string Key, object Value) p4 = default, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            return LogMessage.Create(MessageType.Debug, message, exception, Category, LogMessage.Pack(p1, p2, p3, p4), callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Creates a MessageType.Informational message in this category and raises <see cref="LogMessage.LogAdded"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="p1">An optional property such as <c>("orderId", orderId)</c>.</param>
        /// <param name="p2">An optional second property.</param>
        /// <param name="p3">An optional third property.</param>
        /// <param name="p4">An optional fourth property.</param>
        /// <param name="callerMemberName">Supplied by the compiler; do not pass.</param>
        /// <param name="callerFilePath">Supplied by the compiler; do not pass.</param>
        /// <param name="callerLineNumber">Supplied by the compiler; do not pass.</param>
        /// <returns>The message.</returns>
        public LogMessage Info(string message, (string Key, object Value) p1 = default, (string Key, object Value) p2 = default, (string Key, object Value) p3 = default, (string Key, object Value) p4 = default, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            return LogMessage.Create(MessageType.Informational, message, null, Category, LogMessage.Pack(p1, p2, p3, p4), callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Creates a MessageType.Informational message with the full exception in this category and raises <see cref="LogMessage.LogAdded"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception; its full text (type, inner exceptions, stack) is appended. Null logs the message alone.</param>
        /// <param name="p1">An optional property such as <c>("orderId", orderId)</c>.</param>
        /// <param name="p2">An optional second property.</param>
        /// <param name="p3">An optional third property.</param>
        /// <param name="p4">An optional fourth property.</param>
        /// <param name="callerMemberName">Supplied by the compiler; do not pass.</param>
        /// <param name="callerFilePath">Supplied by the compiler; do not pass.</param>
        /// <param name="callerLineNumber">Supplied by the compiler; do not pass.</param>
        /// <returns>The message.</returns>
        public LogMessage Info(string message, Exception exception, (string Key, object Value) p1 = default, (string Key, object Value) p2 = default, (string Key, object Value) p3 = default, (string Key, object Value) p4 = default, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            return LogMessage.Create(MessageType.Informational, message, exception, Category, LogMessage.Pack(p1, p2, p3, p4), callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Creates a MessageType.Success message in this category and raises <see cref="LogMessage.LogAdded"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="p1">An optional property such as <c>("orderId", orderId)</c>.</param>
        /// <param name="p2">An optional second property.</param>
        /// <param name="p3">An optional third property.</param>
        /// <param name="p4">An optional fourth property.</param>
        /// <param name="callerMemberName">Supplied by the compiler; do not pass.</param>
        /// <param name="callerFilePath">Supplied by the compiler; do not pass.</param>
        /// <param name="callerLineNumber">Supplied by the compiler; do not pass.</param>
        /// <returns>The message.</returns>
        public LogMessage Success(string message, (string Key, object Value) p1 = default, (string Key, object Value) p2 = default, (string Key, object Value) p3 = default, (string Key, object Value) p4 = default, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            return LogMessage.Create(MessageType.Success, message, null, Category, LogMessage.Pack(p1, p2, p3, p4), callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Creates a MessageType.Success message with the full exception in this category and raises <see cref="LogMessage.LogAdded"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception; its full text (type, inner exceptions, stack) is appended. Null logs the message alone.</param>
        /// <param name="p1">An optional property such as <c>("orderId", orderId)</c>.</param>
        /// <param name="p2">An optional second property.</param>
        /// <param name="p3">An optional third property.</param>
        /// <param name="p4">An optional fourth property.</param>
        /// <param name="callerMemberName">Supplied by the compiler; do not pass.</param>
        /// <param name="callerFilePath">Supplied by the compiler; do not pass.</param>
        /// <param name="callerLineNumber">Supplied by the compiler; do not pass.</param>
        /// <returns>The message.</returns>
        public LogMessage Success(string message, Exception exception, (string Key, object Value) p1 = default, (string Key, object Value) p2 = default, (string Key, object Value) p3 = default, (string Key, object Value) p4 = default, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            return LogMessage.Create(MessageType.Success, message, exception, Category, LogMessage.Pack(p1, p2, p3, p4), callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Creates a MessageType.Warning message in this category and raises <see cref="LogMessage.LogAdded"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="p1">An optional property such as <c>("orderId", orderId)</c>.</param>
        /// <param name="p2">An optional second property.</param>
        /// <param name="p3">An optional third property.</param>
        /// <param name="p4">An optional fourth property.</param>
        /// <param name="callerMemberName">Supplied by the compiler; do not pass.</param>
        /// <param name="callerFilePath">Supplied by the compiler; do not pass.</param>
        /// <param name="callerLineNumber">Supplied by the compiler; do not pass.</param>
        /// <returns>The message.</returns>
        public LogMessage Warning(string message, (string Key, object Value) p1 = default, (string Key, object Value) p2 = default, (string Key, object Value) p3 = default, (string Key, object Value) p4 = default, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            return LogMessage.Create(MessageType.Warning, message, null, Category, LogMessage.Pack(p1, p2, p3, p4), callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Creates a MessageType.Warning message with the full exception in this category and raises <see cref="LogMessage.LogAdded"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception; its full text (type, inner exceptions, stack) is appended. Null logs the message alone.</param>
        /// <param name="p1">An optional property such as <c>("orderId", orderId)</c>.</param>
        /// <param name="p2">An optional second property.</param>
        /// <param name="p3">An optional third property.</param>
        /// <param name="p4">An optional fourth property.</param>
        /// <param name="callerMemberName">Supplied by the compiler; do not pass.</param>
        /// <param name="callerFilePath">Supplied by the compiler; do not pass.</param>
        /// <param name="callerLineNumber">Supplied by the compiler; do not pass.</param>
        /// <returns>The message.</returns>
        public LogMessage Warning(string message, Exception exception, (string Key, object Value) p1 = default, (string Key, object Value) p2 = default, (string Key, object Value) p3 = default, (string Key, object Value) p4 = default, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            return LogMessage.Create(MessageType.Warning, message, exception, Category, LogMessage.Pack(p1, p2, p3, p4), callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Creates a MessageType.Error message in this category and raises <see cref="LogMessage.LogAdded"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="p1">An optional property such as <c>("orderId", orderId)</c>.</param>
        /// <param name="p2">An optional second property.</param>
        /// <param name="p3">An optional third property.</param>
        /// <param name="p4">An optional fourth property.</param>
        /// <param name="callerMemberName">Supplied by the compiler; do not pass.</param>
        /// <param name="callerFilePath">Supplied by the compiler; do not pass.</param>
        /// <param name="callerLineNumber">Supplied by the compiler; do not pass.</param>
        /// <returns>The message.</returns>
        public LogMessage Error(string message, (string Key, object Value) p1 = default, (string Key, object Value) p2 = default, (string Key, object Value) p3 = default, (string Key, object Value) p4 = default, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            return LogMessage.Create(MessageType.Error, message, null, Category, LogMessage.Pack(p1, p2, p3, p4), callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Creates a MessageType.Error message with the full exception in this category and raises <see cref="LogMessage.LogAdded"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception; its full text (type, inner exceptions, stack) is appended. Null logs the message alone.</param>
        /// <param name="p1">An optional property such as <c>("orderId", orderId)</c>.</param>
        /// <param name="p2">An optional second property.</param>
        /// <param name="p3">An optional third property.</param>
        /// <param name="p4">An optional fourth property.</param>
        /// <param name="callerMemberName">Supplied by the compiler; do not pass.</param>
        /// <param name="callerFilePath">Supplied by the compiler; do not pass.</param>
        /// <param name="callerLineNumber">Supplied by the compiler; do not pass.</param>
        /// <returns>The message.</returns>
        public LogMessage Error(string message, Exception exception, (string Key, object Value) p1 = default, (string Key, object Value) p2 = default, (string Key, object Value) p3 = default, (string Key, object Value) p4 = default, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            return LogMessage.Create(MessageType.Error, message, exception, Category, LogMessage.Pack(p1, p2, p3, p4), callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Creates a MessageType.Critical message in this category and raises <see cref="LogMessage.LogAdded"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="p1">An optional property such as <c>("orderId", orderId)</c>.</param>
        /// <param name="p2">An optional second property.</param>
        /// <param name="p3">An optional third property.</param>
        /// <param name="p4">An optional fourth property.</param>
        /// <param name="callerMemberName">Supplied by the compiler; do not pass.</param>
        /// <param name="callerFilePath">Supplied by the compiler; do not pass.</param>
        /// <param name="callerLineNumber">Supplied by the compiler; do not pass.</param>
        /// <returns>The message.</returns>
        public LogMessage Critical(string message, (string Key, object Value) p1 = default, (string Key, object Value) p2 = default, (string Key, object Value) p3 = default, (string Key, object Value) p4 = default, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            return LogMessage.Create(MessageType.Critical, message, null, Category, LogMessage.Pack(p1, p2, p3, p4), callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Creates a MessageType.Critical message with the full exception in this category and raises <see cref="LogMessage.LogAdded"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception; its full text (type, inner exceptions, stack) is appended. Null logs the message alone.</param>
        /// <param name="p1">An optional property such as <c>("orderId", orderId)</c>.</param>
        /// <param name="p2">An optional second property.</param>
        /// <param name="p3">An optional third property.</param>
        /// <param name="p4">An optional fourth property.</param>
        /// <param name="callerMemberName">Supplied by the compiler; do not pass.</param>
        /// <param name="callerFilePath">Supplied by the compiler; do not pass.</param>
        /// <param name="callerLineNumber">Supplied by the compiler; do not pass.</param>
        /// <returns>The message.</returns>
        public LogMessage Critical(string message, Exception exception, (string Key, object Value) p1 = default, (string Key, object Value) p2 = default, (string Key, object Value) p3 = default, (string Key, object Value) p4 = default, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            return LogMessage.Create(MessageType.Critical, message, exception, Category, LogMessage.Pack(p1, p2, p3, p4), callerMemberName, callerFilePath, callerLineNumber);
        }
    }
}
