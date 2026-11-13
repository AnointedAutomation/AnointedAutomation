// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
using System;
using System.Collections.Generic;
using AnointedAutomation.Optimization.Logging;
using Xunit;

namespace AnointedAutomation.Logging.Tests
{
    /// <summary>
    /// The pre-subscription buffer behind LogAdded (each test uses its own dispatcher, never the global one).
    /// </summary>
    public class LogDispatcherTests
    {
        private static LogMessage Row(string text, string source)
        {
            return new LogMessage { message = text, messageSource = source, messageType = MessageType.Informational };
        }

        [Fact]
        public void FirstSubscriber_ReceivesBufferedLogsInOrder_WithSourceRestamped()
        {
            LogDispatcher dispatcher = new LogDispatcher(10);
            Assert.True(dispatcher.EnableBuffering());
            dispatcher.Raise(Row("one", LogMessage.UnsetMessageSource));
            dispatcher.Raise(Row("two", "Named"));
            Assert.Equal(2, dispatcher.PendingCount);
            List<LogMessage> first = new List<LogMessage>();
            List<LogMessage> second = new List<LogMessage>();

            dispatcher.Subscribe((s, e) => first.Add(e.log));
            dispatcher.Subscribe((s, e) => second.Add(e.log));
            dispatcher.Raise(Row("three", "Named"));

            Assert.Equal(new[] { "one", "two", "three" }, first.ConvertAll(l => l.message));
            Assert.Equal(LogMessage.MessageSourceSetter, first[0].messageSource);
            Assert.Equal("Named", first[1].messageSource);
            Assert.Equal(new[] { "three" }, second.ConvertAll(l => l.message));
            Assert.Equal(0, dispatcher.PendingCount);
        }

        [Fact]
        public void Overflow_DropsOldest_AndWarnsWithTheDroppedCount()
        {
            LogDispatcher dispatcher = new LogDispatcher(2);
            Assert.True(dispatcher.EnableBuffering());
            dispatcher.Raise(Row("a", "S"));
            dispatcher.Raise(Row("b", "S"));
            dispatcher.Raise(Row("c", "S"));
            List<string> seen = new List<string>();
            using LogStoreCapture capture = LogStoreCapture.Start();

            dispatcher.Subscribe((s, e) => seen.Add(e.log.message));

            Assert.Equal(new[] { "b", "c" }, seen);
            Assert.True(capture.Contains(MessageType.Warning, "1 log message(s) raised before the first LogAdded subscriber were dropped"));
        }

        [Fact]
        public void AfterAllUnsubscribe_NothingIsBufferedAgain()
        {
            LogDispatcher dispatcher = new LogDispatcher(5);
            Assert.True(dispatcher.EnableBuffering());
            LogMessage.LogAddedEventHandler handler = (s, e) => { };
            dispatcher.Subscribe(handler);
            dispatcher.Unsubscribe(handler);

            dispatcher.Raise(Row("late", "S"));

            Assert.Equal(0, dispatcher.PendingCount);
        }

        [Fact]
        public void BufferingIsOffByDefault_AndCannotStartAfterASubscriber()
        {
            LogDispatcher dispatcher = new LogDispatcher(5);
            dispatcher.Raise(Row("unbuffered", "S"));
            Assert.Equal(0, dispatcher.PendingCount);
            List<string> seen = new List<string>();

            dispatcher.Subscribe((s, e) => seen.Add(e.log.message));

            Assert.Empty(seen);
            Assert.False(dispatcher.EnableBuffering());
        }

        [Fact]
        public void ThrowingSubscriber_DoesNotStopOthers_AndNullIsIgnored()
        {
            LogDispatcher dispatcher = new LogDispatcher(5);
            bool secondCalled = false;
            dispatcher.Subscribe(null);
            dispatcher.Subscribe((s, e) => throw new InvalidOperationException("subscriber failure"));
            dispatcher.Subscribe((s, e) => secondCalled = true);

            dispatcher.Raise(Row("x", "S"));

            Assert.True(secondCalled);
            Assert.Throws<ArgumentOutOfRangeException>(() => new LogDispatcher(0));
        }
    }
}
