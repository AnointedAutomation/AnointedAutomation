// Copyright © Anointed Automation, LLC., 2025. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2025-06-08 13:27:40
// Edited by Alexander Fields https://www.alexanderfields.me 2025-07-02 11:48:25
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Xunit;
using AnointedAutomation.APIMiddleware;
using AnointedAutomation.APIMiddleware.Objects;
using AnointedAutomation.Optimization.Logging;
using Moq;

namespace AnointedAutomation.APIMiddlewares.Tests
{
    [Collection("Sequential")]
    public class IPBlacklistMiddlewareTests : IDisposable
    {
        public IPBlacklistMiddlewareTests()
        {
            // Clear state before each test
            IPBlacklist.ClearBlacklist();
            IPBlacklistMiddleware.ClearLogs();
        }

        public void Dispose()
        {
            // Clear state after each test
            IPBlacklist.ClearBlacklist();
            IPBlacklistMiddleware.ClearLogs();
        }
        [Fact]
        public void Constructor_CreatesInstance()
        {
            // Arrange
            RequestDelegate next = (context) => Task.CompletedTask;

            // Act
            var middleware = new IPBlacklistMiddleware(next);

            // Assert
            Assert.NotNull(middleware);
        }

        [Fact]
        public async Task InvokeAsync_WithBlockedIP_Returns403()
        {
            // Arrange
            bool nextCalled = false;
            RequestDelegate next = (context) => 
            {
                nextCalled = true;
                return Task.CompletedTask;
            };
            
            var middleware = new IPBlacklistMiddleware(next);
            
            var context = new DefaultHttpContext();
            context.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("192.168.1.20");
            
            // Add IP to blacklist
            IPBlacklist.AddBannedIP("192.168.1.20", "Test blocking for middleware test");
            
            // Clear any existing logs
            IPBlacklistMiddleware.ClearLogs();
            
            // Act
            await middleware.InvokeAsync(context);
            
            // Assert
            Assert.Equal(StatusCodes.Status403Forbidden, context.Response.StatusCode);
            Assert.False(nextCalled, "Next middleware should not be called for blocked IP");
            
            // Check if a log was added
            var logs = IPBlacklistMiddleware.GetLogs();
            Assert.NotEmpty(logs);
        }

        [Fact]
        public async Task InvokeAsync_WithNonBlockedIP_CallsNext()
        {
            // Arrange
            bool nextCalled = false;
            RequestDelegate next = (context) => 
            {
                nextCalled = true;
                return Task.CompletedTask;
            };
            
            var middleware = new IPBlacklistMiddleware(next);
            
            var context = new DefaultHttpContext();
            context.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("192.168.1.21");
            
            // IP should not be blacklisted due to test setup
            
            // Act
            await middleware.InvokeAsync(context);
            
            // Assert
            Assert.True(nextCalled, "Next middleware should be called for non-blocked IP");
            Assert.NotEqual(StatusCodes.Status403Forbidden, context.Response.StatusCode);
        }

        [Fact]
        public void AddLog_AddsLogToQueue_AndTriggersEvent()
        {
            // Arrange
            IPBlacklistMiddleware.ClearLogs();
            var logMessage = LogMessage.Warning("Test log message");
            bool eventRaised = false;
            
            EventHandler<LogMessageEventArgs> handler = (sender, e) => 
            {
                eventRaised = true;
                Assert.Equal(logMessage, e.log);
            };
            
            IPBlacklistMiddleware.LogAdded += handler;
            
            // Act
            IPBlacklistMiddleware.AddLog(logMessage);
            
            // Assert
            var logs = IPBlacklistMiddleware.GetLogs();
            Assert.Single(logs);
            Assert.Equal(logMessage, logs[0]);
            Assert.True(eventRaised, "LogAdded event should be triggered");
            
            // Cleanup
            IPBlacklistMiddleware.LogAdded -= handler;
        }

        [Fact]
        public void ClearLogs_EmptiesLogQueue_AndTriggersEvent()
        {
            // Arrange
            IPBlacklistMiddleware.AddLog(LogMessage.Warning("Test log message 1"));
            IPBlacklistMiddleware.AddLog(LogMessage.Warning("Test log message 2"));
            
            bool eventRaised = false;
            EventHandler handler = (sender, e) => eventRaised = true;
            
            IPBlacklistMiddleware.LogCleared += handler;
            
            // Act
            IPBlacklistMiddleware.ClearLogs();
            
            // Assert
            var logs = IPBlacklistMiddleware.GetLogs();
            Assert.Empty(logs);
            Assert.True(eventRaised, "LogCleared event should be triggered");
            
            // Cleanup
            IPBlacklistMiddleware.LogCleared -= handler;
        }

        [Fact]
        public void GetLogs_ReturnsAllLogs()
        {
            // Arrange
            IPBlacklistMiddleware.ClearLogs();
            var log1 = LogMessage.Warning("Test log message 1");
            var log2 = LogMessage.Warning("Test log message 2");
            var log3 = LogMessage.Warning("Test log message 3");
            
            IPBlacklistMiddleware.AddLog(log1);
            IPBlacklistMiddleware.AddLog(log2);
            IPBlacklistMiddleware.AddLog(log3);
            
            // Act
            var logs = IPBlacklistMiddleware.GetLogs();
            
            // Assert
            Assert.Equal(3, logs.Count);
            Assert.Contains(log1, logs);
            Assert.Contains(log2, logs);
            Assert.Contains(log3, logs);
        }
    }
}
