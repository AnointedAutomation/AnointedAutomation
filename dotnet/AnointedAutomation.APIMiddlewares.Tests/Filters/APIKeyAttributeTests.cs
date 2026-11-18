// Copyright © Anointed Automation Corporation., 2025. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2025-06-08 13:27:40 Jesus is King ✝️
// Edited by Alexander Fields https://www.alexanderfields.me 2025-07-02 11:48:25
using Xunit;
using AnointedAutomation.APIMiddleware.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;

namespace AnointedAutomation.APIMiddlewares.Tests.Filters
{
    /// <summary>
    /// Tests for the APIKeyAttribute filter.
    /// </summary>
    public class APIKeyAttributeTests : IDisposable
    {
        private readonly string originalApiKey;
        private readonly string originalApiKeyName;
        private const string TestApiKey = "test-api-key-12345";
        private const string TestApiKeyName = "X-API-Key-Test";

        /// <summary>
        /// Initializes a new instance of the <see cref="APIKeyAttributeTests"/> class.
        /// Sets up test environment variables for API key authentication.
        /// </summary>
        public APIKeyAttributeTests()
        {
            // Store original environment variable values
            originalApiKey = Environment.GetEnvironmentVariable("API_KEY");
            originalApiKeyName = Environment.GetEnvironmentVariable("API_KEY_NAME");
            
            // Set test values
            Environment.SetEnvironmentVariable("API_KEY", TestApiKey);
            Environment.SetEnvironmentVariable("API_KEY_NAME", TestApiKeyName);
        }

        /// <summary>
        /// Restores original environment variables after tests.
        /// </summary>
        public void Dispose()
        {
            // Restore original environment variable values
            if (originalApiKey != null)
                Environment.SetEnvironmentVariable("API_KEY", originalApiKey);
            else
                Environment.SetEnvironmentVariable("API_KEY", null);
                
            if (originalApiKeyName != null)
                Environment.SetEnvironmentVariable("API_KEY_NAME", originalApiKeyName);
            else
                Environment.SetEnvironmentVariable("API_KEY_NAME", null);
        }

        /// <summary>
        /// A missing API_KEY fails at construction with a clear message, not a NullReferenceException per request.
        /// </summary>
        [Fact]
        public void Constructor_WithMissingApiKey_Throws()
        {
            Environment.SetEnvironmentVariable("API_KEY", null);

            InvalidOperationException ex = Assert.Throws<InvalidOperationException>(() => new APIKeyAttribute());

            Assert.Contains("API_KEY", ex.Message);
        }

        /// <summary>
        /// A missing API_KEY_NAME fails at construction instead of throwing on a null header name.
        /// </summary>
        [Fact]
        public void Constructor_WithMissingApiKeyName_Throws()
        {
            Environment.SetEnvironmentVariable("API_KEY_NAME", null);

            InvalidOperationException ex = Assert.Throws<InvalidOperationException>(() => new APIKeyAttribute());

            Assert.Contains("API_KEY_NAME", ex.Message);
        }

        /// <summary>
        /// A key of the right length that differs only in the last character is still rejected.
        /// </summary>
        [Fact]
        public async Task OnActionExecutionAsync_WithNearMissKey_ReturnsUnauthorized()
        {
            APIKeyAttribute attribute = new APIKeyAttribute();
            DefaultHttpContext httpContext = new DefaultHttpContext();
            httpContext.Request.Headers[TestApiKeyName] = TestApiKey.Substring(0, TestApiKey.Length - 1) + "X";
            httpContext.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("192.168.1.3");
            ActionContext actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
            List<IFilterMetadata> filters = new List<IFilterMetadata>();
            Mock<ControllerBase> controllerMock = new Mock<ControllerBase>();
            ActionExecutingContext context = new ActionExecutingContext(actionContext, filters, new Dictionary<string, object>(), controllerMock.Object);
            bool nextCalled = false;
            ActionExecutionDelegate next = () =>
            {
                nextCalled = true;
                return Task.FromResult(new ActionExecutedContext(actionContext, filters, controllerMock.Object));
            };

            await attribute.OnActionExecutionAsync(context, next);

            Assert.False(nextCalled);
            Assert.IsType<UnauthorizedResult>(context.Result);
        }

        [Fact]
        /// <summary>
        /// Verifies that OnActionExecutionAsync allows the request to proceed when a valid API key is provided.
        /// </summary>
        /// <returns>A task representing the asynchronous test operation.</returns>
        public async Task OnActionExecutionAsync_WithValidKey_CallsNext()
        {
            // Arrange
            var attribute = new APIKeyAttribute();
            
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers[TestApiKeyName] = TestApiKey;
            httpContext.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("192.168.1.1");
            
            var actionContext = new ActionContext(
                httpContext,
                new RouteData(),
                new ActionDescriptor()
            );
            
            var filters = new List<IFilterMetadata>();
            var actionArguments = new Dictionary<string, object>();
            var controllerMock = new Mock<ControllerBase>();
            
            var context = new ActionExecutingContext(
                actionContext,
                filters,
                actionArguments,
                controllerMock.Object
            );
            
            bool nextCalled = false;
            ActionExecutionDelegate next = () => 
            {
                nextCalled = true;
                return Task.FromResult(new ActionExecutedContext(actionContext, filters, controllerMock.Object));
            };
            
            // Act
            await attribute.OnActionExecutionAsync(context, next);
            
            // Assert
            Assert.True(nextCalled, "Next delegate should be called with valid API key");
            Assert.Null(context.Result);
        }

        [Fact]
        /// <summary>
        /// Verifies that OnActionExecutionAsync returns Unauthorized when no API key is provided.
        /// </summary>
        /// <returns>A task representing the asynchronous test operation.</returns>
        public async Task OnActionExecutionAsync_WithMissingKey_ReturnsUnauthorized()
        {
            // Arrange
            var attribute = new APIKeyAttribute();
            
            var httpContext = new DefaultHttpContext();
            // Do not add the API key header
            httpContext.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("192.168.1.1");
            
            var actionContext = new ActionContext(
                httpContext,
                new RouteData(),
                new ActionDescriptor()
            );
            
            var filters = new List<IFilterMetadata>();
            var actionArguments = new Dictionary<string, object>();
            var controllerMock = new Mock<ControllerBase>();
            
            var context = new ActionExecutingContext(
                actionContext,
                filters,
                actionArguments,
                controllerMock.Object
            );
            
            bool nextCalled = false;
            ActionExecutionDelegate next = () => 
            {
                nextCalled = true;
                return Task.FromResult(new ActionExecutedContext(actionContext, filters, controllerMock.Object));
            };
            
            // Act
            await attribute.OnActionExecutionAsync(context, next);
            
            // Assert
            Assert.False(nextCalled, "Next delegate should not be called with missing API key");
            Assert.IsType<UnauthorizedResult>(context.Result);
        }

        [Fact]
        /// <summary>
        /// Verifies that OnActionExecutionAsync returns Unauthorized when an invalid API key is provided.
        /// </summary>
        /// <returns>A task representing the asynchronous test operation.</returns>
        public async Task OnActionExecutionAsync_WithInvalidKey_ReturnsUnauthorized()
        {
            // Arrange
            var attribute = new APIKeyAttribute();
            
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers[TestApiKeyName] = "invalid-key";
            httpContext.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("192.168.1.1");
            
            var actionContext = new ActionContext(
                httpContext,
                new RouteData(),
                new ActionDescriptor()
            );
            
            var filters = new List<IFilterMetadata>();
            var actionArguments = new Dictionary<string, object>();
            var controllerMock = new Mock<ControllerBase>();
            
            var context = new ActionExecutingContext(
                actionContext,
                filters,
                actionArguments,
                controllerMock.Object
            );
            
            bool nextCalled = false;
            ActionExecutionDelegate next = () => 
            {
                nextCalled = true;
                return Task.FromResult(new ActionExecutedContext(actionContext, filters, controllerMock.Object));
            };
            
            // Act
            await attribute.OnActionExecutionAsync(context, next);
            
            // Assert
            Assert.False(nextCalled, "Next delegate should not be called with invalid API key");
            Assert.IsType<UnauthorizedResult>(context.Result);
        }

        [Fact]
        /// <summary>
        /// The rejected key may be a real secret, so the warning names the header and length, never the value.
        /// </summary>
        /// <returns>A task representing the asynchronous test operation.</returns>
        public async Task OnActionExecutionAsync_WithInvalidKey_NeverLogsTheKeyValue()
        {
            string sentKey = "sent-key-" + Guid.NewGuid().ToString("N");
            List<AnointedAutomation.Optimization.Logging.LogMessage> seen = new List<AnointedAutomation.Optimization.Logging.LogMessage>();
            AnointedAutomation.Optimization.Logging.LogMessage.LogAddedEventHandler handler = (sender, e) =>
            {
                if (e.log.message != null && e.log.message.Contains("incorrect key", StringComparison.Ordinal))
                {
                    lock (seen)
                    {
                        seen.Add(e.log);
                    }
                }
            };
            AnointedAutomation.Optimization.Logging.LogMessage.LogAdded += handler;
            try
            {
                APIKeyAttribute attribute = new APIKeyAttribute();
                DefaultHttpContext httpContext = new DefaultHttpContext();
                httpContext.Request.Headers[TestApiKeyName] = sentKey;
                httpContext.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("192.168.1.2");
                ActionContext actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
                List<IFilterMetadata> filters = new List<IFilterMetadata>();
                Mock<ControllerBase> controllerMock = new Mock<ControllerBase>();
                ActionExecutingContext context = new ActionExecutingContext(actionContext, filters, new Dictionary<string, object>(), controllerMock.Object);
                ActionExecutionDelegate next = () => Task.FromResult(new ActionExecutedContext(actionContext, filters, controllerMock.Object));

                await attribute.OnActionExecutionAsync(context, next);

                Assert.NotEmpty(seen);
                Assert.All(seen, log => Assert.DoesNotContain(sentKey, log.message));
            }
            finally
            {
                AnointedAutomation.Optimization.Logging.LogMessage.LogAdded -= handler;
            }
        }
    }
}
