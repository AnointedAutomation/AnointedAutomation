// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
using System;
using System.Runtime.CompilerServices;

namespace AnointedAutomation.Optimization.Logging
{
    /// <summary>
    /// Factories with structured properties, and the Debug and Trace levels.
    /// </summary>
    public partial class LogMessage
    {
        /// <summary>
        /// Creates a MessageType.Celebrate message carrying the full exception and raises <see cref="LogAdded"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception; its full text (type, inner exceptions, stack) is appended. Null logs the message alone.</param>
        /// <param name="callerMemberName">Supplied by the compiler; do not pass.</param>
        /// <param name="callerFilePath">Supplied by the compiler; do not pass.</param>
        /// <param name="callerLineNumber">Supplied by the compiler; do not pass.</param>
        /// <returns>The message.</returns>
        public static LogMessage Celebrate(string message, Exception exception, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            return new LogMessage(0, MessageType.Celebrate, message, exception, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Creates a MessageType.Celebrate message with structured properties and raises <see cref="LogAdded"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="p1">A property such as <c>("orderId", orderId)</c>.</param>
        /// <param name="p2">An optional second property.</param>
        /// <param name="p3">An optional third property.</param>
        /// <param name="p4">An optional fourth property.</param>
        /// <param name="callerMemberName">Supplied by the compiler; do not pass.</param>
        /// <param name="callerFilePath">Supplied by the compiler; do not pass.</param>
        /// <param name="callerLineNumber">Supplied by the compiler; do not pass.</param>
        /// <returns>The message.</returns>
        public static LogMessage Celebrate(string message, (string Key, object Value) p1, (string Key, object Value) p2 = default, (string Key, object Value) p3 = default, (string Key, object Value) p4 = default, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            return new LogMessage(0, MessageType.Celebrate, message, null, null, Pack(p1, p2, p3, p4), callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Creates a MessageType.Celebrate message with the full exception and structured properties, and raises <see cref="LogAdded"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception; its full text (type, inner exceptions, stack) is appended. Null logs the message alone.</param>
        /// <param name="p1">A property such as <c>("orderId", orderId)</c>.</param>
        /// <param name="p2">An optional second property.</param>
        /// <param name="p3">An optional third property.</param>
        /// <param name="p4">An optional fourth property.</param>
        /// <param name="callerMemberName">Supplied by the compiler; do not pass.</param>
        /// <param name="callerFilePath">Supplied by the compiler; do not pass.</param>
        /// <param name="callerLineNumber">Supplied by the compiler; do not pass.</param>
        /// <returns>The message.</returns>
        public static LogMessage Celebrate(string message, Exception exception, (string Key, object Value) p1, (string Key, object Value) p2 = default, (string Key, object Value) p3 = default, (string Key, object Value) p4 = default, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            return new LogMessage(0, MessageType.Celebrate, message, exception, null, Pack(p1, p2, p3, p4), callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Creates a MessageType.Critical message with structured properties and raises <see cref="LogAdded"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="p1">A property such as <c>("orderId", orderId)</c>.</param>
        /// <param name="p2">An optional second property.</param>
        /// <param name="p3">An optional third property.</param>
        /// <param name="p4">An optional fourth property.</param>
        /// <param name="callerMemberName">Supplied by the compiler; do not pass.</param>
        /// <param name="callerFilePath">Supplied by the compiler; do not pass.</param>
        /// <param name="callerLineNumber">Supplied by the compiler; do not pass.</param>
        /// <returns>The message.</returns>
        public static LogMessage Critical(string message, (string Key, object Value) p1, (string Key, object Value) p2 = default, (string Key, object Value) p3 = default, (string Key, object Value) p4 = default, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            return new LogMessage(0, MessageType.Critical, message, null, null, Pack(p1, p2, p3, p4), callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Creates a MessageType.Critical message with the full exception and structured properties, and raises <see cref="LogAdded"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception; its full text (type, inner exceptions, stack) is appended. Null logs the message alone.</param>
        /// <param name="p1">A property such as <c>("orderId", orderId)</c>.</param>
        /// <param name="p2">An optional second property.</param>
        /// <param name="p3">An optional third property.</param>
        /// <param name="p4">An optional fourth property.</param>
        /// <param name="callerMemberName">Supplied by the compiler; do not pass.</param>
        /// <param name="callerFilePath">Supplied by the compiler; do not pass.</param>
        /// <param name="callerLineNumber">Supplied by the compiler; do not pass.</param>
        /// <returns>The message.</returns>
        public static LogMessage Critical(string message, Exception exception, (string Key, object Value) p1, (string Key, object Value) p2 = default, (string Key, object Value) p3 = default, (string Key, object Value) p4 = default, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            return new LogMessage(0, MessageType.Critical, message, exception, null, Pack(p1, p2, p3, p4), callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Creates a MessageType.Error message with structured properties and raises <see cref="LogAdded"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="p1">A property such as <c>("orderId", orderId)</c>.</param>
        /// <param name="p2">An optional second property.</param>
        /// <param name="p3">An optional third property.</param>
        /// <param name="p4">An optional fourth property.</param>
        /// <param name="callerMemberName">Supplied by the compiler; do not pass.</param>
        /// <param name="callerFilePath">Supplied by the compiler; do not pass.</param>
        /// <param name="callerLineNumber">Supplied by the compiler; do not pass.</param>
        /// <returns>The message.</returns>
        public static LogMessage Error(string message, (string Key, object Value) p1, (string Key, object Value) p2 = default, (string Key, object Value) p3 = default, (string Key, object Value) p4 = default, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            return new LogMessage(0, MessageType.Error, message, null, null, Pack(p1, p2, p3, p4), callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Creates a MessageType.Error message with the full exception and structured properties, and raises <see cref="LogAdded"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception; its full text (type, inner exceptions, stack) is appended. Null logs the message alone.</param>
        /// <param name="p1">A property such as <c>("orderId", orderId)</c>.</param>
        /// <param name="p2">An optional second property.</param>
        /// <param name="p3">An optional third property.</param>
        /// <param name="p4">An optional fourth property.</param>
        /// <param name="callerMemberName">Supplied by the compiler; do not pass.</param>
        /// <param name="callerFilePath">Supplied by the compiler; do not pass.</param>
        /// <param name="callerLineNumber">Supplied by the compiler; do not pass.</param>
        /// <returns>The message.</returns>
        public static LogMessage Error(string message, Exception exception, (string Key, object Value) p1, (string Key, object Value) p2 = default, (string Key, object Value) p3 = default, (string Key, object Value) p4 = default, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            return new LogMessage(0, MessageType.Error, message, exception, null, Pack(p1, p2, p3, p4), callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Creates a MessageType.Informational message carrying the full exception and raises <see cref="LogAdded"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception; its full text (type, inner exceptions, stack) is appended. Null logs the message alone.</param>
        /// <param name="callerMemberName">Supplied by the compiler; do not pass.</param>
        /// <param name="callerFilePath">Supplied by the compiler; do not pass.</param>
        /// <param name="callerLineNumber">Supplied by the compiler; do not pass.</param>
        /// <returns>The message.</returns>
        public static LogMessage Info(string message, Exception exception, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            return new LogMessage(0, MessageType.Informational, message, exception, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Creates a MessageType.Informational message with structured properties and raises <see cref="LogAdded"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="p1">A property such as <c>("orderId", orderId)</c>.</param>
        /// <param name="p2">An optional second property.</param>
        /// <param name="p3">An optional third property.</param>
        /// <param name="p4">An optional fourth property.</param>
        /// <param name="callerMemberName">Supplied by the compiler; do not pass.</param>
        /// <param name="callerFilePath">Supplied by the compiler; do not pass.</param>
        /// <param name="callerLineNumber">Supplied by the compiler; do not pass.</param>
        /// <returns>The message.</returns>
        public static LogMessage Info(string message, (string Key, object Value) p1, (string Key, object Value) p2 = default, (string Key, object Value) p3 = default, (string Key, object Value) p4 = default, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            return new LogMessage(0, MessageType.Informational, message, null, null, Pack(p1, p2, p3, p4), callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Creates a MessageType.Informational message with the full exception and structured properties, and raises <see cref="LogAdded"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception; its full text (type, inner exceptions, stack) is appended. Null logs the message alone.</param>
        /// <param name="p1">A property such as <c>("orderId", orderId)</c>.</param>
        /// <param name="p2">An optional second property.</param>
        /// <param name="p3">An optional third property.</param>
        /// <param name="p4">An optional fourth property.</param>
        /// <param name="callerMemberName">Supplied by the compiler; do not pass.</param>
        /// <param name="callerFilePath">Supplied by the compiler; do not pass.</param>
        /// <param name="callerLineNumber">Supplied by the compiler; do not pass.</param>
        /// <returns>The message.</returns>
        public static LogMessage Info(string message, Exception exception, (string Key, object Value) p1, (string Key, object Value) p2 = default, (string Key, object Value) p3 = default, (string Key, object Value) p4 = default, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            return new LogMessage(0, MessageType.Informational, message, exception, null, Pack(p1, p2, p3, p4), callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Creates a MessageType.Informational message carrying the full exception and raises <see cref="LogAdded"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception; its full text (type, inner exceptions, stack) is appended. Null logs the message alone.</param>
        /// <param name="callerMemberName">Supplied by the compiler; do not pass.</param>
        /// <param name="callerFilePath">Supplied by the compiler; do not pass.</param>
        /// <param name="callerLineNumber">Supplied by the compiler; do not pass.</param>
        /// <returns>The message.</returns>
        public static LogMessage Informational(string message, Exception exception, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            return new LogMessage(0, MessageType.Informational, message, exception, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Creates a MessageType.Informational message with structured properties and raises <see cref="LogAdded"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="p1">A property such as <c>("orderId", orderId)</c>.</param>
        /// <param name="p2">An optional second property.</param>
        /// <param name="p3">An optional third property.</param>
        /// <param name="p4">An optional fourth property.</param>
        /// <param name="callerMemberName">Supplied by the compiler; do not pass.</param>
        /// <param name="callerFilePath">Supplied by the compiler; do not pass.</param>
        /// <param name="callerLineNumber">Supplied by the compiler; do not pass.</param>
        /// <returns>The message.</returns>
        public static LogMessage Informational(string message, (string Key, object Value) p1, (string Key, object Value) p2 = default, (string Key, object Value) p3 = default, (string Key, object Value) p4 = default, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            return new LogMessage(0, MessageType.Informational, message, null, null, Pack(p1, p2, p3, p4), callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Creates a MessageType.Informational message with the full exception and structured properties, and raises <see cref="LogAdded"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception; its full text (type, inner exceptions, stack) is appended. Null logs the message alone.</param>
        /// <param name="p1">A property such as <c>("orderId", orderId)</c>.</param>
        /// <param name="p2">An optional second property.</param>
        /// <param name="p3">An optional third property.</param>
        /// <param name="p4">An optional fourth property.</param>
        /// <param name="callerMemberName">Supplied by the compiler; do not pass.</param>
        /// <param name="callerFilePath">Supplied by the compiler; do not pass.</param>
        /// <param name="callerLineNumber">Supplied by the compiler; do not pass.</param>
        /// <returns>The message.</returns>
        public static LogMessage Informational(string message, Exception exception, (string Key, object Value) p1, (string Key, object Value) p2 = default, (string Key, object Value) p3 = default, (string Key, object Value) p4 = default, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            return new LogMessage(0, MessageType.Informational, message, exception, null, Pack(p1, p2, p3, p4), callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Creates a MessageType.Message message carrying the full exception and raises <see cref="LogAdded"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception; its full text (type, inner exceptions, stack) is appended. Null logs the message alone.</param>
        /// <param name="callerMemberName">Supplied by the compiler; do not pass.</param>
        /// <param name="callerFilePath">Supplied by the compiler; do not pass.</param>
        /// <param name="callerLineNumber">Supplied by the compiler; do not pass.</param>
        /// <returns>The message.</returns>
        public static LogMessage Message(string message, Exception exception, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            return new LogMessage(0, MessageType.Message, message, exception, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Creates a MessageType.Message message with structured properties and raises <see cref="LogAdded"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="p1">A property such as <c>("orderId", orderId)</c>.</param>
        /// <param name="p2">An optional second property.</param>
        /// <param name="p3">An optional third property.</param>
        /// <param name="p4">An optional fourth property.</param>
        /// <param name="callerMemberName">Supplied by the compiler; do not pass.</param>
        /// <param name="callerFilePath">Supplied by the compiler; do not pass.</param>
        /// <param name="callerLineNumber">Supplied by the compiler; do not pass.</param>
        /// <returns>The message.</returns>
        public static LogMessage Message(string message, (string Key, object Value) p1, (string Key, object Value) p2 = default, (string Key, object Value) p3 = default, (string Key, object Value) p4 = default, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            return new LogMessage(0, MessageType.Message, message, null, null, Pack(p1, p2, p3, p4), callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Creates a MessageType.Message message with the full exception and structured properties, and raises <see cref="LogAdded"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception; its full text (type, inner exceptions, stack) is appended. Null logs the message alone.</param>
        /// <param name="p1">A property such as <c>("orderId", orderId)</c>.</param>
        /// <param name="p2">An optional second property.</param>
        /// <param name="p3">An optional third property.</param>
        /// <param name="p4">An optional fourth property.</param>
        /// <param name="callerMemberName">Supplied by the compiler; do not pass.</param>
        /// <param name="callerFilePath">Supplied by the compiler; do not pass.</param>
        /// <param name="callerLineNumber">Supplied by the compiler; do not pass.</param>
        /// <returns>The message.</returns>
        public static LogMessage Message(string message, Exception exception, (string Key, object Value) p1, (string Key, object Value) p2 = default, (string Key, object Value) p3 = default, (string Key, object Value) p4 = default, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            return new LogMessage(0, MessageType.Message, message, exception, null, Pack(p1, p2, p3, p4), callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Creates a MessageType.Success message carrying the full exception and raises <see cref="LogAdded"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception; its full text (type, inner exceptions, stack) is appended. Null logs the message alone.</param>
        /// <param name="callerMemberName">Supplied by the compiler; do not pass.</param>
        /// <param name="callerFilePath">Supplied by the compiler; do not pass.</param>
        /// <param name="callerLineNumber">Supplied by the compiler; do not pass.</param>
        /// <returns>The message.</returns>
        public static LogMessage Success(string message, Exception exception, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            return new LogMessage(0, MessageType.Success, message, exception, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Creates a MessageType.Success message with structured properties and raises <see cref="LogAdded"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="p1">A property such as <c>("orderId", orderId)</c>.</param>
        /// <param name="p2">An optional second property.</param>
        /// <param name="p3">An optional third property.</param>
        /// <param name="p4">An optional fourth property.</param>
        /// <param name="callerMemberName">Supplied by the compiler; do not pass.</param>
        /// <param name="callerFilePath">Supplied by the compiler; do not pass.</param>
        /// <param name="callerLineNumber">Supplied by the compiler; do not pass.</param>
        /// <returns>The message.</returns>
        public static LogMessage Success(string message, (string Key, object Value) p1, (string Key, object Value) p2 = default, (string Key, object Value) p3 = default, (string Key, object Value) p4 = default, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            return new LogMessage(0, MessageType.Success, message, null, null, Pack(p1, p2, p3, p4), callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Creates a MessageType.Success message with the full exception and structured properties, and raises <see cref="LogAdded"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception; its full text (type, inner exceptions, stack) is appended. Null logs the message alone.</param>
        /// <param name="p1">A property such as <c>("orderId", orderId)</c>.</param>
        /// <param name="p2">An optional second property.</param>
        /// <param name="p3">An optional third property.</param>
        /// <param name="p4">An optional fourth property.</param>
        /// <param name="callerMemberName">Supplied by the compiler; do not pass.</param>
        /// <param name="callerFilePath">Supplied by the compiler; do not pass.</param>
        /// <param name="callerLineNumber">Supplied by the compiler; do not pass.</param>
        /// <returns>The message.</returns>
        public static LogMessage Success(string message, Exception exception, (string Key, object Value) p1, (string Key, object Value) p2 = default, (string Key, object Value) p3 = default, (string Key, object Value) p4 = default, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            return new LogMessage(0, MessageType.Success, message, exception, null, Pack(p1, p2, p3, p4), callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Creates a MessageType.Warning message with structured properties and raises <see cref="LogAdded"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="p1">A property such as <c>("orderId", orderId)</c>.</param>
        /// <param name="p2">An optional second property.</param>
        /// <param name="p3">An optional third property.</param>
        /// <param name="p4">An optional fourth property.</param>
        /// <param name="callerMemberName">Supplied by the compiler; do not pass.</param>
        /// <param name="callerFilePath">Supplied by the compiler; do not pass.</param>
        /// <param name="callerLineNumber">Supplied by the compiler; do not pass.</param>
        /// <returns>The message.</returns>
        public static LogMessage Warning(string message, (string Key, object Value) p1, (string Key, object Value) p2 = default, (string Key, object Value) p3 = default, (string Key, object Value) p4 = default, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            return new LogMessage(0, MessageType.Warning, message, null, null, Pack(p1, p2, p3, p4), callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Creates a MessageType.Warning message with the full exception and structured properties, and raises <see cref="LogAdded"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception; its full text (type, inner exceptions, stack) is appended. Null logs the message alone.</param>
        /// <param name="p1">A property such as <c>("orderId", orderId)</c>.</param>
        /// <param name="p2">An optional second property.</param>
        /// <param name="p3">An optional third property.</param>
        /// <param name="p4">An optional fourth property.</param>
        /// <param name="callerMemberName">Supplied by the compiler; do not pass.</param>
        /// <param name="callerFilePath">Supplied by the compiler; do not pass.</param>
        /// <param name="callerLineNumber">Supplied by the compiler; do not pass.</param>
        /// <returns>The message.</returns>
        public static LogMessage Warning(string message, Exception exception, (string Key, object Value) p1, (string Key, object Value) p2 = default, (string Key, object Value) p3 = default, (string Key, object Value) p4 = default, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            return new LogMessage(0, MessageType.Warning, message, exception, null, Pack(p1, p2, p3, p4), callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Creates a MessageType.Debug message and raises <see cref="LogAdded"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="callerMemberName">Supplied by the compiler; do not pass.</param>
        /// <param name="callerFilePath">Supplied by the compiler; do not pass.</param>
        /// <param name="callerLineNumber">Supplied by the compiler; do not pass.</param>
        /// <returns>The message.</returns>
        public static LogMessage Debug(string message, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            return new LogMessage(0, MessageType.Debug, message, null, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Creates a MessageType.Debug message with an id and raises <see cref="LogAdded"/>.
        /// </summary>
        /// <param name="id">ID of the log message.</param>
        /// <param name="message">The message.</param>
        /// <param name="callerMemberName">Supplied by the compiler; do not pass.</param>
        /// <param name="callerFilePath">Supplied by the compiler; do not pass.</param>
        /// <param name="callerLineNumber">Supplied by the compiler; do not pass.</param>
        /// <returns>The message.</returns>
        public static LogMessage Debug(long id, string message, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            return new LogMessage(id, MessageType.Debug, message, null, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Creates a MessageType.Debug message carrying the full exception and raises <see cref="LogAdded"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception; its full text (type, inner exceptions, stack) is appended. Null logs the message alone.</param>
        /// <param name="callerMemberName">Supplied by the compiler; do not pass.</param>
        /// <param name="callerFilePath">Supplied by the compiler; do not pass.</param>
        /// <param name="callerLineNumber">Supplied by the compiler; do not pass.</param>
        /// <returns>The message.</returns>
        public static LogMessage Debug(string message, Exception exception, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            return new LogMessage(0, MessageType.Debug, message, exception, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Creates a MessageType.Debug message with structured properties and raises <see cref="LogAdded"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="p1">A property such as <c>("orderId", orderId)</c>.</param>
        /// <param name="p2">An optional second property.</param>
        /// <param name="p3">An optional third property.</param>
        /// <param name="p4">An optional fourth property.</param>
        /// <param name="callerMemberName">Supplied by the compiler; do not pass.</param>
        /// <param name="callerFilePath">Supplied by the compiler; do not pass.</param>
        /// <param name="callerLineNumber">Supplied by the compiler; do not pass.</param>
        /// <returns>The message.</returns>
        public static LogMessage Debug(string message, (string Key, object Value) p1, (string Key, object Value) p2 = default, (string Key, object Value) p3 = default, (string Key, object Value) p4 = default, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            return new LogMessage(0, MessageType.Debug, message, null, null, Pack(p1, p2, p3, p4), callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Creates a MessageType.Debug message with the full exception and structured properties, and raises <see cref="LogAdded"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception; its full text (type, inner exceptions, stack) is appended. Null logs the message alone.</param>
        /// <param name="p1">A property such as <c>("orderId", orderId)</c>.</param>
        /// <param name="p2">An optional second property.</param>
        /// <param name="p3">An optional third property.</param>
        /// <param name="p4">An optional fourth property.</param>
        /// <param name="callerMemberName">Supplied by the compiler; do not pass.</param>
        /// <param name="callerFilePath">Supplied by the compiler; do not pass.</param>
        /// <param name="callerLineNumber">Supplied by the compiler; do not pass.</param>
        /// <returns>The message.</returns>
        public static LogMessage Debug(string message, Exception exception, (string Key, object Value) p1, (string Key, object Value) p2 = default, (string Key, object Value) p3 = default, (string Key, object Value) p4 = default, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            return new LogMessage(0, MessageType.Debug, message, exception, null, Pack(p1, p2, p3, p4), callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Creates a MessageType.Trace message and raises <see cref="LogAdded"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="callerMemberName">Supplied by the compiler; do not pass.</param>
        /// <param name="callerFilePath">Supplied by the compiler; do not pass.</param>
        /// <param name="callerLineNumber">Supplied by the compiler; do not pass.</param>
        /// <returns>The message.</returns>
        public static LogMessage Trace(string message, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            return new LogMessage(0, MessageType.Trace, message, null, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Creates a MessageType.Trace message with an id and raises <see cref="LogAdded"/>.
        /// </summary>
        /// <param name="id">ID of the log message.</param>
        /// <param name="message">The message.</param>
        /// <param name="callerMemberName">Supplied by the compiler; do not pass.</param>
        /// <param name="callerFilePath">Supplied by the compiler; do not pass.</param>
        /// <param name="callerLineNumber">Supplied by the compiler; do not pass.</param>
        /// <returns>The message.</returns>
        public static LogMessage Trace(long id, string message, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            return new LogMessage(id, MessageType.Trace, message, null, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Creates a MessageType.Trace message carrying the full exception and raises <see cref="LogAdded"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception; its full text (type, inner exceptions, stack) is appended. Null logs the message alone.</param>
        /// <param name="callerMemberName">Supplied by the compiler; do not pass.</param>
        /// <param name="callerFilePath">Supplied by the compiler; do not pass.</param>
        /// <param name="callerLineNumber">Supplied by the compiler; do not pass.</param>
        /// <returns>The message.</returns>
        public static LogMessage Trace(string message, Exception exception, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            return new LogMessage(0, MessageType.Trace, message, exception, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Creates a MessageType.Trace message with structured properties and raises <see cref="LogAdded"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="p1">A property such as <c>("orderId", orderId)</c>.</param>
        /// <param name="p2">An optional second property.</param>
        /// <param name="p3">An optional third property.</param>
        /// <param name="p4">An optional fourth property.</param>
        /// <param name="callerMemberName">Supplied by the compiler; do not pass.</param>
        /// <param name="callerFilePath">Supplied by the compiler; do not pass.</param>
        /// <param name="callerLineNumber">Supplied by the compiler; do not pass.</param>
        /// <returns>The message.</returns>
        public static LogMessage Trace(string message, (string Key, object Value) p1, (string Key, object Value) p2 = default, (string Key, object Value) p3 = default, (string Key, object Value) p4 = default, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            return new LogMessage(0, MessageType.Trace, message, null, null, Pack(p1, p2, p3, p4), callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Creates a MessageType.Trace message with the full exception and structured properties, and raises <see cref="LogAdded"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception; its full text (type, inner exceptions, stack) is appended. Null logs the message alone.</param>
        /// <param name="p1">A property such as <c>("orderId", orderId)</c>.</param>
        /// <param name="p2">An optional second property.</param>
        /// <param name="p3">An optional third property.</param>
        /// <param name="p4">An optional fourth property.</param>
        /// <param name="callerMemberName">Supplied by the compiler; do not pass.</param>
        /// <param name="callerFilePath">Supplied by the compiler; do not pass.</param>
        /// <param name="callerLineNumber">Supplied by the compiler; do not pass.</param>
        /// <returns>The message.</returns>
        public static LogMessage Trace(string message, Exception exception, (string Key, object Value) p1, (string Key, object Value) p2 = default, (string Key, object Value) p3 = default, (string Key, object Value) p4 = default, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            return new LogMessage(0, MessageType.Trace, message, exception, null, Pack(p1, p2, p3, p4), callerMemberName, callerFilePath, callerLineNumber);
        }
    }
}
