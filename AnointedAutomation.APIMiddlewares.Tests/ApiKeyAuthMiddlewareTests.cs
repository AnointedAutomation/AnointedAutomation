// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-20
using Xunit;
using AnointedAutomation.APIMiddleware;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Threading.Tasks;

namespace AnointedAutomation.APIMiddlewares.Tests
{
    public class ApiKeyAuthMiddlewareTests
    {
        private static byte[] Key(string s) => Encoding.UTF8.GetBytes(s);

        [Fact]
        public void IsAuthorized_CorrectKey_True()
        {
            Assert.True(ApiKeyAuthMiddleware.IsAuthorized("secret-123", Key("secret-123")));
        }

        [Fact]
        public void IsAuthorized_WrongKey_False()
        {
            Assert.False(ApiKeyAuthMiddleware.IsAuthorized("nope", Key("secret-123")));
        }

        [Fact]
        public void IsAuthorized_EmptyOrNull_False()
        {
            Assert.False(ApiKeyAuthMiddleware.IsAuthorized("", Key("secret-123")));
            Assert.False(ApiKeyAuthMiddleware.IsAuthorized(null, Key("secret-123")));
        }

        [Fact]
        public async Task InvokeAsync_ProtectedPath_MissingKey_Returns401_DoesNotCallNext()
        {
            bool nextCalled = false;
            RequestDelegate next = _ => { nextCalled = true; return Task.CompletedTask; };
            ApiKeyAuthMiddleware middleware = new ApiKeyAuthMiddleware(next, "x-anointed-api-key", "secret", "/api/native");
            DefaultHttpContext context = new DefaultHttpContext();
            context.Request.Path = "/api/native/shopify/archive";

            await middleware.InvokeAsync(context);

            Assert.Equal(401, context.Response.StatusCode);
            Assert.False(nextCalled);
        }

        [Fact]
        public async Task InvokeAsync_ProtectedPath_ValidKey_CallsNext()
        {
            bool nextCalled = false;
            RequestDelegate next = _ => { nextCalled = true; return Task.CompletedTask; };
            ApiKeyAuthMiddleware middleware = new ApiKeyAuthMiddleware(next, "x-anointed-api-key", "secret", "/api/native");
            DefaultHttpContext context = new DefaultHttpContext();
            context.Request.Path = "/api/native/shopify/archive";
            context.Request.Headers["x-anointed-api-key"] = "secret";

            await middleware.InvokeAsync(context);

            Assert.True(nextCalled);
            Assert.Equal(200, context.Response.StatusCode);
        }

        [Fact]
        public async Task InvokeAsync_UnprotectedPath_CallsNext_WithoutKey()
        {
            bool nextCalled = false;
            RequestDelegate next = _ => { nextCalled = true; return Task.CompletedTask; };
            ApiKeyAuthMiddleware middleware = new ApiKeyAuthMiddleware(next, "x-anointed-api-key", "secret", "/api/native");
            DefaultHttpContext context = new DefaultHttpContext();
            context.Request.Path = "/api/public/health";

            await middleware.InvokeAsync(context);

            Assert.True(nextCalled);
        }
    }
}
