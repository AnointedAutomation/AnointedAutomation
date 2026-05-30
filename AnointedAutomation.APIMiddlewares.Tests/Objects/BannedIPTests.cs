// Copyright © Anointed Automation, LLC., 2025. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2025-06-08 13:27:40
// Edited by Alexander Fields https://www.alexanderfields.me 2025-07-02 11:48:25
using Xunit;
using AnointedAutomation.APIMiddleware.Objects;

namespace AnointedAutomation.APIMiddlewares.Tests.Objects
{
    public class BannedIPTests
    {
        [Fact]
        public void DefaultConstructor_CreatesInstance()
        {
            // Act
            var bannedIP = new BannedIP();

            // Assert
            Assert.NotNull(bannedIP);
            Assert.Null(bannedIP._id);
            Assert.Null(bannedIP.Ipv4);
            Assert.Null(bannedIP.Ipv6);
            Assert.Null(bannedIP.Reason);
        }

        [Fact]
        public void Constructor_WithIPAddresses_SetsIPAddressesOnly()
        {
            // Arrange
            string ipv4 = "192.168.1.1";
            string ipv6 = "2001:0db8:85a3:0000:0000:8a2e:0370:7334";

            // Act
            var bannedIP = new BannedIP(ipv4, ipv6);

            // Assert
            Assert.Equal(ipv4, bannedIP.Ipv4);
            Assert.Equal(ipv6, bannedIP.Ipv6);
            Assert.Null(bannedIP._id);
            Assert.Null(bannedIP.Reason);
        }

        [Fact]
        public void Constructor_WithAllParameters_SetsAllProperties()
        {
            // Arrange
            string id = "test-id";
            string ipv4 = "192.168.1.1";
            string ipv6 = "2001:0db8:85a3:0000:0000:8a2e:0370:7334";
            string reason = "Test reason";

            // Act
            var bannedIP = new BannedIP(id, ipv4, ipv6, reason);

            // Assert
            Assert.Equal(id, bannedIP._id);
            Assert.Equal(ipv4, bannedIP.Ipv4);
            Assert.Equal(ipv6, bannedIP.Ipv6);
            Assert.Equal(reason, bannedIP.Reason);
        }

        [Fact]
        public void Properties_CanBeModified()
        {
            // Arrange
            var bannedIP = new BannedIP();
            string id = "test-id";
            string ipv4 = "192.168.1.1";
            string ipv6 = "2001:0db8:85a3:0000:0000:8a2e:0370:7334";
            string reason = "Test reason";

            // Act
            bannedIP._id = id;
            bannedIP.Ipv4 = ipv4;
            bannedIP.Ipv6 = ipv6;
            bannedIP.Reason = reason;

            // Assert
            Assert.Equal(id, bannedIP._id);
            Assert.Equal(ipv4, bannedIP.Ipv4);
            Assert.Equal(ipv6, bannedIP.Ipv6);
            Assert.Equal(reason, bannedIP.Reason);
        }

        // EDGE CASE TESTS - Added per CLAUDE_TESTING.md requirements

        [Fact]
        public void Constructor_WithNullIPAddresses_SetsNullValues()
        {
            // Act
            var bannedIP = new BannedIP(null, null);

            // Assert
            Assert.Null(bannedIP.Ipv4);
            Assert.Null(bannedIP.Ipv6);
        }

        [Fact]
        public void Constructor_WithEmptyIPAddresses_SetsEmptyStrings()
        {
            // Act
            var bannedIP = new BannedIP("", "");

            // Assert
            Assert.Equal("", bannedIP.Ipv4);
            Assert.Equal("", bannedIP.Ipv6);
        }

        [Fact]
        public void Constructor_WithAllNullParameters_SetsAllNull()
        {
            // Act
            var bannedIP = new BannedIP(null, null, null, null);

            // Assert
            Assert.Null(bannedIP._id);
            Assert.Null(bannedIP.Ipv4);
            Assert.Null(bannedIP.Ipv6);
            Assert.Null(bannedIP.Reason);
        }

        [Fact]
        public void Constructor_WithAllEmptyParameters_SetsAllEmpty()
        {
            // Act
            var bannedIP = new BannedIP("", "", "", "");

            // Assert
            Assert.Equal("", bannedIP._id);
            Assert.Equal("", bannedIP.Ipv4);
            Assert.Equal("", bannedIP.Ipv6);
            Assert.Equal("", bannedIP.Reason);
        }

        [Fact]
        public void Constructor_WithVeryLongStrings_SetsValues()
        {
            // Arrange
            string longString = new string('a', 10000);

            // Act
            var bannedIP = new BannedIP(longString, longString, longString, longString);

            // Assert
            Assert.Equal(longString, bannedIP._id);
            Assert.Equal(longString, bannedIP.Ipv4);
            Assert.Equal(longString, bannedIP.Ipv6);
            Assert.Equal(longString, bannedIP.Reason);
        }

        [Fact]
        public void Properties_CanBeSetToNull()
        {
            // Arrange
            var bannedIP = new BannedIP("id", "ipv4", "ipv6", "reason");

            // Act
            bannedIP._id = null;
            bannedIP.Ipv4 = null;
            bannedIP.Ipv6 = null;
            bannedIP.Reason = null;

            // Assert
            Assert.Null(bannedIP._id);
            Assert.Null(bannedIP.Ipv4);
            Assert.Null(bannedIP.Ipv6);
            Assert.Null(bannedIP.Reason);
        }

        [Fact]
        public void Properties_CanBeSetToEmptyString()
        {
            // Arrange
            var bannedIP = new BannedIP("id", "ipv4", "ipv6", "reason");

            // Act
            bannedIP._id = "";
            bannedIP.Ipv4 = "";
            bannedIP.Ipv6 = "";
            bannedIP.Reason = "";

            // Assert
            Assert.Equal("", bannedIP._id);
            Assert.Equal("", bannedIP.Ipv4);
            Assert.Equal("", bannedIP.Ipv6);
            Assert.Equal("", bannedIP.Reason);
        }

        [Fact]
        public void Constructor_WithSpecialCharacters_SetsValues()
        {
            // Arrange
            string specialChars = "!@#$%^&*()_+-=[]{}|;':\",./<>?";

            // Act
            var bannedIP = new BannedIP(specialChars, specialChars, specialChars, specialChars);

            // Assert
            Assert.Equal(specialChars, bannedIP._id);
            Assert.Equal(specialChars, bannedIP.Ipv4);
            Assert.Equal(specialChars, bannedIP.Ipv6);
            Assert.Equal(specialChars, bannedIP.Reason);
        }
    }
}
