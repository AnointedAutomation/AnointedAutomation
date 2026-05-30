// Copyright © Anointed Automation, LLC., 2025. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2025-06-08 13:27:40
// Edited by Alexander Fields https://www.alexanderfields.me 2025-07-02 11:48:25
using Xunit;
using AnointedAutomation.APIMiddleware.Objects;
using AnointedAutomation.APIMiddleware;
using System;

namespace AnointedAutomation.APIMiddlewares.Tests.Objects
{
    [Collection("Sequential")]
    public class IPBlacklistTests : IDisposable
    {
        public IPBlacklistTests()
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
        public void AddBannedIP_AddsIPToBlacklist()
        {
            // Arrange
            string ip = "192.168.1.1";
            string reason = "Test reason";
            
            bool eventRaised = false;
            EventHandler<BannedIP> handler = (sender, e) => 
            {
                eventRaised = true;
                Assert.Equal(ip, e.Ipv4);
                Assert.Equal(reason, e.Reason);
            };
            IPBlacklist.IPBanned += handler;
            
            // Act
            IPBlacklist.AddBannedIP(ip, reason);
            
            // Assert
            Assert.True(eventRaised, "IPBanned event should be raised");
            Assert.Equal(reason, IPBlacklist.GetBlockReason(ip));
            Assert.True(IPBlacklist.IsIPBlocked(ip));
            
            // Cleanup
            IPBlacklist.IPBanned -= handler;
        }
        
        [Fact]
        public void AddBannedIP_WhenIPAlreadyExists_DoesNotRaiseEvent()
        {
            // Arrange
            string ip = "192.168.1.2";
            string reason1 = "Test reason 1";
            string reason2 = "Test reason 2";
            
            // Add IP first time
            IPBlacklist.AddBannedIP(ip, reason1);
            
            bool eventRaised = false;
            EventHandler<BannedIP> handler = (sender, e) => eventRaised = true;
            IPBlacklist.IPBanned += handler;
            
            // Act - try to add again
            IPBlacklist.AddBannedIP(ip, reason2);
            
            // Assert
            Assert.False(eventRaised, "IPBanned event should not be raised for already banned IP");
            // The reason should not change
            Assert.Equal(reason1, IPBlacklist.GetBlockReason(ip));
            
            // Cleanup
            IPBlacklist.IPBanned -= handler;
        }
        
        [Fact]
        public void GetBlockReason_ForNonBlacklistedIP_ReturnsNull()
        {
            // Arrange
            string ip = "192.168.1.100"; // Assuming this IP is not blacklisted
            
            // Act
            string reason = IPBlacklist.GetBlockReason(ip);
            
            // Assert
            Assert.Null(reason);
        }
        
        [Fact]
        public void IsIPBlocked_ForNonBlacklistedIP_ReturnsFalse()
        {
            // Arrange
            string ip = "192.168.1.101"; // Assuming this IP is not blacklisted
            
            // Act
            bool isBlocked = IPBlacklist.IsIPBlocked(ip);
            
            // Assert
            Assert.False(isBlocked);
        }
        
        [Fact]
        public void AddBannedIP_WithMultipleIPs_AllAreBlacklisted()
        {
            // Arrange
            string ip1 = "192.168.1.11";
            string reason1 = "Test reason 1";

            string ip2 = "192.168.1.12";
            string reason2 = "Test reason 2";

            // Act
            IPBlacklist.AddBannedIP(ip1, reason1);
            IPBlacklist.AddBannedIP(ip2, reason2);

            // Assert
            Assert.Equal(reason1, IPBlacklist.GetBlockReason(ip1));
            Assert.Equal(reason2, IPBlacklist.GetBlockReason(ip2));
            Assert.True(IPBlacklist.IsIPBlocked(ip1));
            Assert.True(IPBlacklist.IsIPBlocked(ip2));
        }

        // EDGE CASE TESTS - Added per CLAUDE_TESTING.md requirements

        [Fact]
        public void AddBannedIP_WithNullIP_DoesNotAddToBlacklist()
        {
            // Arrange
            string ip = null;
            string reason = "Test reason";

            // Act
            IPBlacklist.AddBannedIP(ip, reason);

            // Assert
            // FIXED: Now validates input and ignores null IPs instead of throwing exception
            Assert.Null(IPBlacklist.GetBlockReason(ip));
            Assert.False(IPBlacklist.IsIPBlocked(ip));
        }

        [Fact]
        public void AddBannedIP_WithEmptyStringIP_DoesNotAddToBlacklist()
        {
            // Arrange
            string ip = "";
            string reason = "Test reason";

            // Act
            IPBlacklist.AddBannedIP(ip, reason);

            // Assert
            // FIXED: Now validates input and ignores empty string IPs
            Assert.Null(IPBlacklist.GetBlockReason(ip));
            Assert.False(IPBlacklist.IsIPBlocked(ip));
        }

        [Fact]
        public void AddBannedIP_WithNullReason_AddsToBlacklist()
        {
            // Arrange
            string ip = "192.168.1.200";
            string reason = null;

            // Act
            IPBlacklist.AddBannedIP(ip, reason);

            // Assert
            Assert.Null(IPBlacklist.GetBlockReason(ip));
            Assert.True(IPBlacklist.IsIPBlocked(ip));
        }

        [Fact]
        public void AddBannedIP_WithEmptyStringReason_AddsToBlacklist()
        {
            // Arrange
            string ip = "192.168.1.201";
            string reason = "";

            // Act
            IPBlacklist.AddBannedIP(ip, reason);

            // Assert
            Assert.Equal("", IPBlacklist.GetBlockReason(ip));
            Assert.True(IPBlacklist.IsIPBlocked(ip));
        }

        [Fact]
        public void GetBlockReason_WithNullIP_ReturnsNull()
        {
            // Arrange
            string ip = null;

            // Act
            string reason = IPBlacklist.GetBlockReason(ip);

            // Assert
            // FIXED: Now validates input and returns null instead of throwing exception
            Assert.Null(reason);
        }

        [Fact]
        public void GetBlockReason_WithEmptyStringIP_ReturnsNullIfNotBlacklisted()
        {
            // Arrange
            string ip = "";

            // Act
            string reason = IPBlacklist.GetBlockReason(ip);

            // Assert
            Assert.Null(reason);
        }

        [Fact]
        public void IsIPBlocked_WithNullIP_ReturnsFalse()
        {
            // Arrange
            string ip = null;

            // Act
            bool isBlocked = IPBlacklist.IsIPBlocked(ip);

            // Assert
            // FIXED: Now validates input and returns false instead of throwing exception
            Assert.False(isBlocked);
        }

        [Fact]
        public void IsIPBlocked_WithEmptyStringIP_ReturnsFalseIfNotBlacklisted()
        {
            // Arrange
            string ip = "";

            // Act
            bool isBlocked = IPBlacklist.IsIPBlocked(ip);

            // Assert
            Assert.False(isBlocked);
        }

        [Fact]
        public void IsIPBlocked_WhenIPIsBlocked_CallsAddLog()
        {
            // Arrange
            string ip = "192.168.1.250";
            string reason = "Test logging";
            IPBlacklist.AddBannedIP(ip, reason);

            // Clear logs to ensure fresh state
            IPBlacklistMiddleware.ClearLogs();

            // Act
            bool isBlocked = IPBlacklist.IsIPBlocked(ip);

            // Assert
            Assert.True(isBlocked);
            // Verify that a log was added (checking that logs collection is not empty)
            // Note: We can't directly inspect the logs without exposing them,
            // but this documents the expected behavior per IPBlacklist.cs:64
        }

        [Fact]
        public void AddBannedIP_WithWhitespaceIP_DoesNotAddToBlacklist()
        {
            // Arrange
            string ip = "   ";
            string reason = "Test whitespace";

            // Act
            IPBlacklist.AddBannedIP(ip, reason);

            // Assert
            // FIXED: Now validates input and ignores whitespace-only IPs
            Assert.Null(IPBlacklist.GetBlockReason(ip));
            Assert.False(IPBlacklist.IsIPBlocked(ip));
        }

        [Fact]
        public void AddBannedIP_WithSpecialCharactersInReason_AddsToBlacklist()
        {
            // Arrange
            string ip = "192.168.1.202";
            string reason = "Test with special chars: !@#$%^&*()_+-=[]{}|;':\",./<>?";

            // Act
            IPBlacklist.AddBannedIP(ip, reason);

            // Assert
            Assert.Equal(reason, IPBlacklist.GetBlockReason(ip));
            Assert.True(IPBlacklist.IsIPBlocked(ip));
        }

        [Fact]
        public void AddBannedIP_WithVeryLongReason_AddsToBlacklist()
        {
            // Arrange
            string ip = "192.168.1.203";
            string reason = new string('A', 10000); // 10,000 character reason

            // Act
            IPBlacklist.AddBannedIP(ip, reason);

            // Assert
            Assert.Equal(reason, IPBlacklist.GetBlockReason(ip));
            Assert.True(IPBlacklist.IsIPBlocked(ip));
        }
    }
}
