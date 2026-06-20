// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-20
using Microsoft.AspNetCore.Http;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AnointedAutomation.APIMiddleware
{
    /// <summary>
    /// Gates requests under a path prefix with a configurable API-key header. The header
    /// name, expected key, and protected prefix are all passed in (the consuming app reads
    /// them from its own config/env), so this middleware is reusable across services. The
    /// key comparison is constant-time so a wrong key cannot be guessed by timing.
    /// </summary>
    public class ApiKeyAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _headerName;
        private readonly byte[] _expectedKey;
        private readonly string _protectedPathPrefix;

        public ApiKeyAuthMiddleware(RequestDelegate next, string headerName, string expectedKey, string protectedPathPrefix)
        {
            _next = next;
            _headerName = headerName;
            _expectedKey = Encoding.UTF8.GetBytes(expectedKey ?? string.Empty);
            _protectedPathPrefix = protectedPathPrefix;
        }

        /// <summary>Constant-time comparison of a presented key against the expected bytes.</summary>
        public static bool IsAuthorized(string presentedKey, byte[] expectedKey)
        {
            if (string.IsNullOrEmpty(presentedKey))
            {
                return false;
            }
            byte[] presented = Encoding.UTF8.GetBytes(presentedKey);
            return CryptographicOperations.FixedTimeEquals(presented, expectedKey);
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments(_protectedPathPrefix))
            {
                string presented = context.Request.Headers[_headerName].ToString();
                if (!IsAuthorized(presented, _expectedKey))
                {
                    context.Response.StatusCode = 401;
                    return;
                }
            }
            await _next(context);
        }
    }
}
