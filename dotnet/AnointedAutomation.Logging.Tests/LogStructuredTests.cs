// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AnointedAutomation.Optimization.Logging;
using Microsoft.Extensions.Logging;
using Xunit;

namespace AnointedAutomation.Logging.Tests
{
    /// <summary>
    /// Debug/Trace levels, structured properties, scopes, categories, LogCapture and the ILogger adapter.
    /// </summary>
    public class LogStructuredTests
    {
        private sealed class OrderService
        {
        }

        private sealed class ThrowingToString
        {
            public override string ToString() => throw new InvalidOperationException("bad ToString");
        }

        [Fact]
        public void Property_WhoseToStringThrows_DoesNotBreakLogging()
        {
            LogMessage log = LogMessage.Warning("still logged", ("bad", new ThrowingToString()));

            Assert.Contains("ToString threw InvalidOperationException", log.properties);
        }

        [Fact]
        public void DebugAndTrace_FactoriesSetLevelAndFileCategory()
        {
            using LogCapture capture = LogCapture.Start();

            LogMessage debug = LogMessage.Debug("cache miss");
            LogMessage trace = LogMessage.Trace(7, "step");

            Assert.Equal(MessageType.Debug, debug.messageType);
            Assert.Equal(MessageType.Trace, trace.messageType);
            Assert.Equal(7, trace.id);
            Assert.Equal("LogStructuredTests", debug.category);
            Assert.StartsWith("LogStructuredTests.DebugAndTrace_FactoriesSetLevelAndFileCategory:", debug.localOperationName);
            Assert.Equal(2, capture.Entries.Count);
        }

        [Fact]
        public void Factory_WithoutProperties_LeavesPropertiesNull()
        {
            LogMessage log = LogMessage.Info("plain");

            Assert.Null(log.properties);
            Assert.Null(log.structuredProperties);
        }

        [Fact]
        public void Warning_WithExceptionAndProperties_StoresAll()
        {
            InvalidOperationException ex = new InvalidOperationException("boom");

            LogMessage log = LogMessage.Warning("Refund failed", ex, ("orderId", 42L), ("store", "aaa"));

            Assert.Equal(MessageType.Warning, log.messageType);
            Assert.Equal(typeof(InvalidOperationException).FullName, log.exceptionType);
            Assert.Contains("boom", log.message);
            Assert.Equal(42L, log.structuredProperties["orderId"]);
            Assert.Equal("aaa", log.structuredProperties["store"]);
            Assert.Equal("{\"orderId\":42,\"store\":\"aaa\"}", log.properties);
        }

        [Fact]
        public void Properties_SerializeEveryValueKindWithoutThrowing()
        {
            DateTime when = new DateTime(2026, 11, 12, 1, 2, 3, DateTimeKind.Utc);

            LogMessage log = LogMessage.Error("kinds", ("nan", double.NaN), ("when", when), ("level", MessageType.Debug), ("none", null));

            Assert.Equal("{\"nan\":\"NaN\",\"when\":\"2026-11-12T01:02:03.0000000Z\",\"level\":\"Debug\",\"none\":null}", log.properties);
        }

        [Fact]
        public void Scope_MergesOuterFirstAndCallSiteWins()
        {
            using (LogMessage.Scope(("orderId", 1), ("store", "outer")))
            using (LogMessage.Scope(("store", "inner")))
            {
                LogMessage log = LogMessage.Info("scoped", ("orderId", 2));

                Assert.Equal(2, log.structuredProperties["orderId"]);
                Assert.Equal("inner", log.structuredProperties["store"]);
            }

            Assert.Null(LogMessage.Info("after").structuredProperties);
        }

        [Fact]
        public async Task Scope_FlowsIntoTasksAndIsIsolatedBetweenFlows()
        {
            Task<LogMessage> a = Task.Run(() =>
            {
                using (LogMessage.Scope(("flow", "a")))
                {
                    Thread.Sleep(20);
                    return LogMessage.Info("a");
                }
            });
            Task<LogMessage> b = Task.Run(() =>
            {
                using (LogMessage.Scope(("flow", "b")))
                {
                    Thread.Sleep(20);
                    return LogMessage.Info("b");
                }
            });
            LogMessage inner;
            using (LogMessage.Scope(("flow", "parent")))
            {
                inner = await Task.Run(() => LogMessage.Info("child"));
            }

            Assert.Equal("a", (await a).structuredProperties["flow"]);
            Assert.Equal("b", (await b).structuredProperties["flow"]);
            Assert.Equal("parent", inner.structuredProperties["flow"]);
        }

        [Fact]
        public void For_StampsTypeCategory_AndRejectsEmptyName()
        {
            LogSource log = LogMessage.For<OrderService>();

            LogMessage message = log.Warning("late", new TimeoutException("t"), ("orderId", 5));

            Assert.Equal(typeof(OrderService).FullName, message.category);
            Assert.Equal(MessageType.Warning, message.messageType);
            Assert.Equal(typeof(TimeoutException).FullName, message.exceptionType);
            Assert.Equal(5, message.structuredProperties["orderId"]);
            Assert.StartsWith("LogStructuredTests.", message.localOperationName);
            Assert.Equal("Custom", LogMessage.For("Custom").Debug("x").category);
            Assert.Throws<ArgumentException>(() => LogMessage.For(""));
        }

        [Fact]
        public async Task LogCapture_IsIsolatedPerAsyncFlow()
        {
            string markerA = "cap-a-" + Guid.NewGuid().ToString("N");
            string markerB = "cap-b-" + Guid.NewGuid().ToString("N");
            using Barrier barrier = new Barrier(2);

            Task<IReadOnlyList<LogMessage>> a = Task.Run(() => CaptureWhileOtherLogs(markerA, barrier));
            Task<IReadOnlyList<LogMessage>> b = Task.Run(() => CaptureWhileOtherLogs(markerB, barrier));

            IReadOnlyList<LogMessage> entriesA = await a;
            IReadOnlyList<LogMessage> entriesB = await b;
            Assert.Single(entriesA);
            Assert.Equal(markerA, entriesA[0].message);
            Assert.Single(entriesB);
            Assert.Equal(markerB, entriesB[0].message);
        }

        private static IReadOnlyList<LogMessage> CaptureWhileOtherLogs(string marker, Barrier barrier)
        {
            using LogCapture capture = LogCapture.Start();
            barrier.SignalAndWait();
            LogMessage.Warning(marker);
            barrier.SignalAndWait();
            return capture.Entries;
        }

        [Fact]
        public void LogCapture_NestsAndStopsOnDispose()
        {
            using LogCapture outer = LogCapture.Start();
            LogCapture inner = LogCapture.Start();
            LogMessage.Error("both");
            inner.Dispose();
            LogMessage.Error("outer only");

            Assert.Single(inner.Entries);
            Assert.Equal(2, outer.Entries.Count);
            Assert.True(outer.Contains(MessageType.Error, "outer only"));
            Assert.False(inner.Contains(MessageType.Error, "outer only"));
            Assert.Equal(2, outer.OfLevel(MessageType.Error).Count);
            Assert.Empty(outer.OfLevel(MessageType.Warning));
        }

        [Fact]
        public void LogAdded_HandlersRunSynchronouslyOnTheLoggingThread()
        {
            string marker = "sync-" + Guid.NewGuid().ToString("N");
            int handlerThread = -1;
            LogMessage.LogAddedEventHandler handler = (sender, e) =>
            {
                if (string.Equals(e.log.message, marker, StringComparison.Ordinal))
                {
                    handlerThread = Environment.CurrentManagedThreadId;
                }
            };
            LogMessage.LogAdded += handler;
            try
            {
                LogMessage.Info(marker);

                Assert.Equal(Environment.CurrentManagedThreadId, handlerThread);
            }
            finally
            {
                LogMessage.LogAdded -= handler;
            }
        }

        [Fact]
        public void LogMessageLogger_MapsLevelsStateEventIdAndException()
        {
            using LogCapture capture = LogCapture.Start();
            LogMessageLogger logger = new LogMessageLogger("Sdk.Client", LogLevel.Information);

            logger.LogWarning(new EventId(12), new InvalidOperationException("x"), "Retry {Attempt} for {Url}", 3, "https://a");
            logger.LogDebug("dropped");

            LogMessage log = Assert.Single(capture.Entries);
            Assert.Equal(MessageType.Warning, log.messageType);
            Assert.Equal("Sdk.Client", log.category);
            Assert.Equal("Sdk.Client", log.localOperationName);
            Assert.StartsWith("Retry 3 for https://a", log.message);
            Assert.Equal(typeof(InvalidOperationException).FullName, log.exceptionType);
            Assert.Equal(3, log.structuredProperties["Attempt"]);
            Assert.Equal("Retry {Attempt} for {Url}", log.structuredProperties["template"]);
            Assert.Equal(12, log.structuredProperties["eventId"]);
        }

        [Fact]
        public void LogMessageLogger_IsEnabledRespectsFloorAndNone_AndScopesAddProperties()
        {
            LogMessageLogger logger = new LogMessageLogger("c", LogLevel.Warning);
            Assert.False(logger.IsEnabled(LogLevel.Information));
            Assert.True(logger.IsEnabled(LogLevel.Critical));
            Assert.False(logger.IsEnabled(LogLevel.None));
            Assert.False(new LogMessageLogger("c", LogLevel.None).IsEnabled(LogLevel.Critical));
            Assert.Throws<ArgumentException>(() => new LogMessageLogger("", LogLevel.Warning));
            Assert.Equal(MessageType.Trace, LogMessageLogger.ToMessageType(LogLevel.Trace));
            Assert.Equal(MessageType.Informational, LogMessageLogger.ToMessageType(LogLevel.Information));
            Assert.Throws<ArgumentOutOfRangeException>(() => LogMessageLogger.ToMessageType(LogLevel.None));

            using LogCapture capture = LogCapture.Start();
            using (logger.BeginScope(new Dictionary<string, object> { ["jobId"] = "j1" }))
            using (logger.BeginScope("plain text"))
            {
                logger.LogError("failed");
            }

            LogMessage log = Assert.Single(capture.Entries);
            Assert.Equal("j1", log.structuredProperties["jobId"]);
            Assert.Equal("plain text", log.structuredProperties["scope"]);
        }

        [Fact]
        public void LogMessageLoggerProvider_CreatesCategoryLoggers()
        {
            using LogMessageLoggerProvider provider = new LogMessageLoggerProvider(LogLevel.Debug);
            using LogCapture capture = LogCapture.Start();

            provider.CreateLogger("Third.Party").LogDebug("hello");

            Assert.Equal("Third.Party", Assert.Single(capture.Entries).category);
        }
    }
}
