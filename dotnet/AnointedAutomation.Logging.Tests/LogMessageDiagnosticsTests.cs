// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-10-28 Jesus is King ✝️
using System;
using AnointedAutomation.Optimization.Logging;
using Xunit;

namespace AnointedAutomation.Logging.Tests
{
    /// <summary>
    /// Covers the 2026-10-28 diagnostics changes: UTC timestamps, caller file:line, exception overloads and
    /// isolated LogAdded subscribers.
    /// </summary>
    public class LogMessageDiagnosticsTests
    {
        [Fact]
        public void Factory_StampsUtcTimestamp()
        {
            LogMessage log = LogMessage.Warning("utc check");

            Assert.Equal(DateTimeKind.Utc, log.timeStamp.Kind);
        }

        [Fact]
        public void Factory_RecordsCallerFileMemberAndLine()
        {
            LogMessage log = LogMessage.Error("caller check");

            Assert.Equal(nameof(Factory_RecordsCallerFileMemberAndLine), log.callerMemberName);
            Assert.EndsWith("LogMessageDiagnosticsTests.cs", log.callerFilePath);
            Assert.True(log.callerLineNumber > 0);
            Assert.Equal($"LogMessageDiagnosticsTests.{nameof(Factory_RecordsCallerFileMemberAndLine)}:{log.callerLineNumber}", log.localOperationName);
        }

        [Fact]
        public void FormatOperationName_HandlesWindowsPathsAndMissingInfo()
        {
            Assert.Equal("Foo.Bar:12", LogMessage.FormatOperationName("Bar", @"C:\src\App\Foo.cs", 12));
            Assert.Equal("Foo.Bar", LogMessage.FormatOperationName("Bar", "/src/App/Foo.cs", 0));
            Assert.Null(LogMessage.FormatOperationName("", "/src/App/Foo.cs", 3));
            Assert.Null(LogMessage.FormatOperationName("Bar", "", 3));
        }

        [Fact]
        public void ExceptionOverload_KeepsTypeMessageAndStack()
        {
            InvalidOperationException thrown;
            try
            {
                throw new InvalidOperationException("outer", new ArgumentException("inner detail"));
            }
            catch (InvalidOperationException ex)
            {
                thrown = ex;
            }

            LogMessage log = LogMessage.Error("Order 42 capture failed", thrown);

            Assert.StartsWith("Order 42 capture failed", log.message);
            Assert.Contains("System.InvalidOperationException: outer", log.message);
            Assert.Contains("inner detail", log.message);
            Assert.Contains(nameof(ExceptionOverload_KeepsTypeMessageAndStack), log.message);
            Assert.Equal(typeof(InvalidOperationException).FullName, log.exceptionType);
            Assert.Equal(MessageType.Error, log.messageType);
        }

        [Fact]
        public void ExceptionOverload_NullException_LogsMessageOnly()
        {
            LogMessage log = LogMessage.Critical("no exception", (Exception)null);

            Assert.Equal("no exception", log.message);
            Assert.Null(log.exceptionType);
        }

        [Fact]
        public void LongId_IsKept()
        {
            long id = 5_000_000_000L;

            LogMessage log = LogMessage.Warning(id, "big id");

            Assert.Equal(id, log.id);
        }

        [Fact]
        public void ThrowingSubscriber_DoesNotReachCallerOrOtherSubscribers()
        {
            string marker = "isolation-" + Guid.NewGuid().ToString("N");
            bool secondCalled = false;
            LogMessage.LogAddedEventHandler throwing = (sender, e) =>
            {
                if (string.Equals(e.log.message, marker, StringComparison.Ordinal))
                {
                    throw new InvalidOperationException("subscriber failure");
                }
            };
            LogMessage.LogAddedEventHandler second = (sender, e) =>
            {
                if (string.Equals(e.log.message, marker, StringComparison.Ordinal))
                {
                    secondCalled = true;
                }
            };
            LogMessage.LogAdded += throwing;
            LogMessage.LogAdded += second;
            try
            {
                LogMessage log = LogMessage.Error(marker);

                Assert.NotNull(log);
                Assert.True(secondCalled, "a later subscriber must still run after an earlier one throws");
            }
            finally
            {
                LogMessage.LogAdded -= throwing;
                LogMessage.LogAdded -= second;
            }
        }

        [Fact]
        public void DefaultConstructor_DoesNotRaiseLogAdded()
        {
            bool raised = false;
            LogMessage.LogAddedEventHandler handler = (sender, e) =>
            {
                if (string.Equals(e.log.message, "initializer-only", StringComparison.Ordinal))
                {
                    raised = true;
                }
            };
            LogMessage.LogAdded += handler;
            try
            {
                LogMessage log = new LogMessage { message = "initializer-only" };

                Assert.False(raised);
                Assert.Equal("initializer-only", log.message);
            }
            finally
            {
                LogMessage.LogAdded -= handler;
            }
        }
    }
}
