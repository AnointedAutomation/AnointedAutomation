// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
using System;
using System.Collections.Generic;
using AnointedAutomation.Optimization.Logging;
using Xunit;

namespace AnointedAutomation.Logging.Tests
{
    /// <summary>
    /// The shared LogThrottle limits and summaries.
    /// </summary>
    public class LogThrottleTests
    {
        private DateTime _now = new DateTime(2026, 11, 12, 0, 0, 0, DateTimeKind.Utc);

        private LogThrottle NewThrottle() => new LogThrottle(TimeSpan.FromMinutes(10), () => _now);

        [Fact]
        public void IdenticalText_PassesFiveTimesPerWindow_ThenSummarizesWhenWindowEnds()
        {
            LogThrottle throttle = NewThrottle();
            int passed = 0;
            for (int i = 0; i < 8; i++)
            {
                if (throttle.TryPass("Cat", MessageType.Warning, null, "same text"))
                {
                    passed++;
                }
            }

            Assert.Equal(LogThrottle.MaxIdenticalPerWindow, passed);
            Assert.Empty(throttle.TakeSummaries(force: false));

            _now = _now.AddMinutes(11);
            IReadOnlyList<LogThrottleSummary> summaries = throttle.TakeSummaries(force: false);
            LogThrottleSummary summary = Assert.Single(summaries);
            Assert.Equal(3, summary.Suppressed);
            Assert.Equal("Cat", summary.Category);
            Assert.Equal(MessageType.Warning, summary.Level);
            Assert.Contains("same text", summary.Text);
            Assert.True(throttle.TryPass("Cat", MessageType.Warning, null, "same text"));
        }

        [Fact]
        public void Template_CapsWarningsAndErrorsSeparately()
        {
            LogThrottle throttle = NewThrottle();
            int warnings = 0;
            int errors = 0;
            for (int i = 0; i < 300; i++)
            {
                if (throttle.TryPass("Cat", MessageType.Warning, "order {Id}", "order " + i)) warnings++;
                if (throttle.TryPass("Cat", MessageType.Error, "order {Id}", "order " + i)) errors++;
            }

            Assert.Equal(LogThrottle.MaxWarningsPerTemplatePerWindow, warnings);
            Assert.Equal(LogThrottle.MaxErrorsPerTemplatePerWindow, errors);
        }

        [Fact]
        public void Critical_IsNeverThrottled()
        {
            LogThrottle throttle = NewThrottle();
            for (int i = 0; i < 50; i++)
            {
                Assert.True(throttle.TryPass("Cat", MessageType.Critical, null, "same"));
            }
        }

        [Fact]
        public void ForcedSummaries_ReportOpenWindowsOnce_AndRolledWindowsQueueTheirSummary()
        {
            LogThrottle throttle = NewThrottle();
            for (int i = 0; i < 7; i++)
            {
                throttle.TryPass("Cat", MessageType.Error, null, "x");
            }

            Assert.Equal(2, Assert.Single(throttle.TakeSummaries(force: true)).Suppressed);
            Assert.Empty(throttle.TakeSummaries(force: true));

            throttle.TryPass("Cat", MessageType.Error, null, "x");
            _now = _now.AddMinutes(10);
            Assert.True(throttle.TryPass("Cat", MessageType.Error, null, "x"));
            Assert.Equal(1, Assert.Single(throttle.TakeSummaries(force: false)).Suppressed);
        }

        [Fact]
        public void Constructor_RejectsBadArguments()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new LogThrottle(TimeSpan.Zero, () => DateTime.UtcNow));
            Assert.Throws<ArgumentNullException>(() => new LogThrottle(TimeSpan.FromMinutes(1), null));
            Assert.Throws<ArgumentNullException>(() => new LogThrottle().TryPass("c", MessageType.Warning, null, null));
        }
    }
}
