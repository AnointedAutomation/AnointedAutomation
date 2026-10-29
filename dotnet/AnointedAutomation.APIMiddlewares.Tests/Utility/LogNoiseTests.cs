// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-10-28 Jesus is King ✝️
using System;
using System.Collections.Generic;
using System.Linq;
using AnointedAutomation.APIMiddleware;
using AnointedAutomation.APIMiddleware.Utility;
using AnointedAutomation.Optimization.Logging;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace AnointedAutomation.APIMiddlewares.Tests.Utility
{
    /// <summary>
    /// 2026-10-28 logging audit: a missing client IP is a throttled Warning (never Critical), lines name the request,
    /// and the in-memory buffer is bounded.
    /// </summary>
    public class LogNoiseTests
    {
        [Fact]
        public void LogThrottle_LetsOnePerWindowThroughAndCountsTheRest()
        {
            LogThrottle throttle = new LogThrottle(TimeSpan.FromMinutes(5));
            DateTime start = new DateTime(2026, 10, 28, 12, 0, 0, DateTimeKind.Utc);

            Assert.True(throttle.TryEnter(start, out int first));
            Assert.Equal(0, first);
            Assert.False(throttle.TryEnter(start.AddMinutes(1), out _));
            Assert.False(throttle.TryEnter(start.AddMinutes(4), out _));
            Assert.True(throttle.TryEnter(start.AddMinutes(5), out int suppressed));
            Assert.Equal(2, suppressed);
        }

        [Fact]
        public void MissingClientIp_IsNeverCritical()
        {
            List<LogMessage> seen = new List<LogMessage>();
            string path = "/missing-ip-" + Guid.NewGuid().ToString("N");
            LogMessage.LogAddedEventHandler handler = (sender, e) =>
            {
                if (e.log.message != null && e.log.message.Contains(path, StringComparison.Ordinal))
                {
                    lock (seen)
                    {
                        seen.Add(e.log);
                    }
                }
            };
            LogMessage.LogAdded += handler;
            try
            {
                APIUtility.MissingIpThrottle.Reset();
                DefaultHttpContext context = new DefaultHttpContext();
                context.Request.Method = "POST";
                context.Request.Path = path;
                context.Connection.RemoteIpAddress = null;

                string first = APIUtility.GetClientPublicIPAddress(context);
                string second = APIUtility.GetClientPublicIPAddress(context);

                Assert.Equal("198.51.100.255", first);
                Assert.Equal("198.51.100.255", second);
                LogMessage only = Assert.Single(seen);
                Assert.Equal(MessageType.Warning, only.messageType);
                Assert.Contains("POST " + path, only.message);
            }
            finally
            {
                LogMessage.LogAdded -= handler;
            }
        }

        [Fact]
        public void DescribeRequest_IsMethodAndPathWithoutQuery()
        {
            DefaultHttpContext context = new DefaultHttpContext();
            context.Request.Method = "GET";
            context.Request.Path = "/api/orders";
            context.Request.QueryString = new QueryString("?token=secret");

            Assert.Equal("GET /api/orders", APIUtility.DescribeRequest(context));
            Assert.Equal("(no HttpContext)", APIUtility.DescribeRequest(null));
        }

        [Fact]
        public void Localhost_IsInformational()
        {
            List<LogMessage> seen = new List<LogMessage>();
            string path = "/loopback-" + Guid.NewGuid().ToString("N");
            LogMessage.LogAddedEventHandler handler = (sender, e) =>
            {
                if (e.log.message != null && e.log.message.Contains(path, StringComparison.Ordinal))
                {
                    lock (seen)
                    {
                        seen.Add(e.log);
                    }
                }
            };
            LogMessage.LogAdded += handler;
            try
            {
                DefaultHttpContext context = new DefaultHttpContext();
                context.Request.Path = path;
                context.Request.Headers["X-Forwarded-For"] = "::1";

                APIUtility.GetClientPublicIPAddress(context);

                Assert.Equal(MessageType.Informational, Assert.Single(seen).messageType);
            }
            finally
            {
                LogMessage.LogAdded -= handler;
            }
        }

        [Fact]
        public void MiddlewareBuffer_IsBounded()
        {
            for (int i = 0; i < IPBlacklistMiddleware.MaxBufferedLogs + 50; i++)
            {
                IPBlacklistMiddleware.AddLog(new LogMessage { message = "bounded " + i });
            }

            // Other tests may add concurrently, so allow a little slack over the cap.
            Assert.True(IPBlacklistMiddleware.GetLogs().Count <= IPBlacklistMiddleware.MaxBufferedLogs + 10);
        }
    }
}
