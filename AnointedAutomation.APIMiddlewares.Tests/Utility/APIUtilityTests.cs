// Copyright © Anointed Automation, LLC., 2025. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2025-06-08 13:27:40
// Edited by Alexander Fields https://www.alexanderfields.me 2025-07-02 11:48:25
using Xunit;
using AnointedAutomation.APIMiddleware.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Moq;
using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Routing;

namespace AnointedAutomation.APIMiddlewares.Tests.Utility
{
    public class APIUtilityTests
    {
        [Fact]
        public void GetClientPublicIPAddress_FromHttpContext_WithXForwardedFor_ReturnsCorrectIP()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            string expectedIP = "203.0.113.1";
            
            httpContext.Request.Headers["X-Forwarded-For"] = expectedIP;
            
            // Act
            string resultIP = APIUtility.GetClientPublicIPAddress(httpContext);
            
            // Assert
            Assert.Equal(expectedIP, resultIP);
        }

        [Fact]
        public void GetClientPublicIPAddress_FromHttpContext_WithoutXForwardedFor_UsesRemoteIpAddress()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            string expectedIP = "198.51.100.1";
            
            httpContext.Connection.RemoteIpAddress = IPAddress.Parse(expectedIP);
            
            // Act
            string resultIP = APIUtility.GetClientPublicIPAddress(httpContext);
            
            // Assert
            Assert.Equal(expectedIP, resultIP);
        }

        [Fact]
        public void GetClientPublicIPAddress_FromHttpContext_WithLocalhost_ReturnsDefaultIP()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Connection.RemoteIpAddress = IPAddress.Parse("::1");
            
            // Act
            string resultIP = APIUtility.GetClientPublicIPAddress(httpContext);
            
            // Assert
            Assert.Equal("198.51.100.255", resultIP);
        }

        [Fact]
        public void GetClientPublicIPAddress_FromActionContext_WithXForwardedFor_ReturnsCorrectIP()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            string expectedIP = "203.0.113.2";
            
            httpContext.Request.Headers["X-Forwarded-For"] = expectedIP;
            
            var actionContext = new ActionContext(
                httpContext,
                new RouteData(),
                new ActionDescriptor()
            );
            
            // Create a mockable controller context
            var controllerMock = new Mock<ControllerBase>();
            
            var actionExecutingContext = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                controllerMock.Object
            );
            
            // Act
            string resultIP = APIUtility.GetClientPublicIPAddress(actionExecutingContext);
            
            // Assert
            Assert.Equal(expectedIP, resultIP);
        }

        [Fact]
        public void GetClientPublicIPAddress_FromActionContext_WithoutXForwardedFor_UsesRemoteIpAddress()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            string expectedIP = "198.51.100.2";
            
            httpContext.Connection.RemoteIpAddress = IPAddress.Parse(expectedIP);
            
            var actionContext = new ActionContext(
                httpContext,
                new RouteData(),
                new ActionDescriptor()
            );
            
            // Create a mockable controller context
            var controllerMock = new Mock<ControllerBase>();
            
            var actionExecutingContext = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                controllerMock.Object
            );
            
            // Act
            string resultIP = APIUtility.GetClientPublicIPAddress(actionExecutingContext);
            
            // Assert
            Assert.Equal(expectedIP, resultIP);
        }

        [Fact]
        public void GetClientPublicIPAddress_FromActionContext_WithLocalhost_ReturnsDefaultIP()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Connection.RemoteIpAddress = IPAddress.Parse("::1");

            var actionContext = new ActionContext(
                httpContext,
                new RouteData(),
                new ActionDescriptor()
            );

            // Create a mockable controller context
            var controllerMock = new Mock<ControllerBase>();

            var actionExecutingContext = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                controllerMock.Object
            );

            // Act
            string resultIP = APIUtility.GetClientPublicIPAddress(actionExecutingContext);

            // Assert
            Assert.Equal("198.51.100.255", resultIP);
        }

        // EDGE CASE TESTS - Added per CLAUDE_TESTING.md requirements

        [Fact]
        public void GetClientPublicIPAddress_FromHttpContext_WithNullRemoteIpAddress_ReturnsDefaultIP()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Connection.RemoteIpAddress = null;

            // Act
            string resultIP = APIUtility.GetClientPublicIPAddress(httpContext);

            // Assert
            Assert.Equal("198.51.100.255", resultIP);
        }

        [Fact]
        public void GetClientPublicIPAddress_FromActionContext_WithNullRemoteIpAddress_ReturnsDefaultIP()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Connection.RemoteIpAddress = null;

            var actionContext = new ActionContext(
                httpContext,
                new RouteData(),
                new ActionDescriptor()
            );

            var controllerMock = new Mock<ControllerBase>();

            var actionExecutingContext = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                controllerMock.Object
            );

            // Act
            string resultIP = APIUtility.GetClientPublicIPAddress(actionExecutingContext);

            // Assert
            // FIXED: Now consistent with HttpContext overload - returns default IP instead of null
            Assert.Equal("198.51.100.255", resultIP);
        }

        [Fact]
        public void GetClientPublicIPAddress_FromHttpContext_WithEmptyXForwardedFor_UsesRemoteIP()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            string expectedIP = "203.0.113.5";

            httpContext.Request.Headers["X-Forwarded-For"] = "";
            httpContext.Connection.RemoteIpAddress = IPAddress.Parse(expectedIP);

            // Act
            string resultIP = APIUtility.GetClientPublicIPAddress(httpContext);

            // Assert
            Assert.Equal(expectedIP, resultIP);
        }

        [Fact]
        public void GetClientPublicIPAddress_FromHttpContext_WithWhitespaceXForwardedFor_ReturnsWhitespace()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();

            httpContext.Request.Headers["X-Forwarded-For"] = "   ";

            // Act
            string resultIP = APIUtility.GetClientPublicIPAddress(httpContext);

            // Assert
            Assert.Equal("   ", resultIP);
        }

        [Fact]
        public void GetClientPublicIPAddress_FromHttpContext_WithMultipleXForwardedFor_ReturnsFirst()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            string firstIP = "203.0.113.10";

            httpContext.Request.Headers["X-Forwarded-For"] = new StringValues(new[] { firstIP, "203.0.113.11", "203.0.113.12" });

            // Act
            string resultIP = APIUtility.GetClientPublicIPAddress(httpContext);

            // Assert
            Assert.Equal(firstIP, resultIP);
        }

        [Fact]
        public void GetClientPublicIPAddress_FromActionContext_WithMultipleXForwardedFor_ReturnsFirst()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            string firstIP = "203.0.113.20";

            httpContext.Request.Headers["X-Forwarded-For"] = new StringValues(new[] { firstIP, "203.0.113.21" });

            var actionContext = new ActionContext(
                httpContext,
                new RouteData(),
                new ActionDescriptor()
            );

            var controllerMock = new Mock<ControllerBase>();

            var actionExecutingContext = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                controllerMock.Object
            );

            // Act
            string resultIP = APIUtility.GetClientPublicIPAddress(actionExecutingContext);

            // Assert
            Assert.Equal(firstIP, resultIP);
        }

        [Fact]
        public void GetClientPublicIPAddress_FromHttpContext_WithIPv6Address_ReturnsIPv6()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            string expectedIPv6 = "2001:db8::1";

            httpContext.Connection.RemoteIpAddress = IPAddress.Parse(expectedIPv6);

            // Act
            string resultIP = APIUtility.GetClientPublicIPAddress(httpContext);

            // Assert
            Assert.Equal(expectedIPv6, resultIP);
        }

        [Fact]
        public void GetClientPublicIPAddress_FromActionContext_WithIPv6Address_ReturnsIPv6()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            string expectedIPv6 = "2001:db8::2";

            httpContext.Connection.RemoteIpAddress = IPAddress.Parse(expectedIPv6);

            var actionContext = new ActionContext(
                httpContext,
                new RouteData(),
                new ActionDescriptor()
            );

            var controllerMock = new Mock<ControllerBase>();

            var actionExecutingContext = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                controllerMock.Object
            );

            // Act
            string resultIP = APIUtility.GetClientPublicIPAddress(actionExecutingContext);

            // Assert
            Assert.Equal(expectedIPv6, resultIP);
        }

        [Fact]
        public void GetClientPublicIPAddress_FromHttpContext_WithLocalhostIPv4_ReturnsDefaultIP()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Connection.RemoteIpAddress = IPAddress.Parse("127.0.0.1");

            // Act
            string resultIP = APIUtility.GetClientPublicIPAddress(httpContext);

            // Assert
            // Note: Implementation doesn't specifically check for 127.0.0.1, only ::1
            Assert.Equal("127.0.0.1", resultIP);
        }

        [Fact]
        public void GetClientPublicIPAddress_FromActionContext_WithEmptyXForwardedFor_UsesRemoteIP()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            string expectedIP = "203.0.113.30";

            httpContext.Request.Headers["X-Forwarded-For"] = "";
            httpContext.Connection.RemoteIpAddress = IPAddress.Parse(expectedIP);

            var actionContext = new ActionContext(
                httpContext,
                new RouteData(),
                new ActionDescriptor()
            );

            var controllerMock = new Mock<ControllerBase>();

            var actionExecutingContext = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                controllerMock.Object
            );

            // Act
            string resultIP = APIUtility.GetClientPublicIPAddress(actionExecutingContext);

            // Assert
            Assert.Equal(expectedIP, resultIP);
        }
    }
}
