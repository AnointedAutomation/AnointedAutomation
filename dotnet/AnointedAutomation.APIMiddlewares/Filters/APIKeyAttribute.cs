// Copyright © Anointed Automation Corporation., 2024. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2024-10-13 01:57:25 Jesus is King ✝️
// Edited by Alexander Fields https://www.alexanderfields.me 2025-07-02 11:48:25
//Stewarded by Alexander Fields
using AnointedAutomation.APIMiddleware.Utility;
using AnointedAutomation.Optimization.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Text;
using System.Threading.Tasks;

namespace AnointedAutomation.APIMiddleware.Filters
{
    [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Method)]
    /// <summary>
    /// Attribute that enforces API key authentication on controllers or actions.
    /// The API key should be provided in the request headers with the name specified in the API_KEY_NAME environment variable.
    /// The correct API key value should be stored in the API_KEY environment variable.
    /// </summary>
    public class APIKeyAttribute : System.Attribute, IAsyncActionFilter
    {
        private readonly string _apiKeyHeaderName;
        private readonly byte[] _expectedKey;

        /// <summary>
        /// Reads API_KEY and API_KEY_NAME once. Both are required: a missing value fails here, at construction,
        /// with a clear message instead of a NullReferenceException (HTTP 500) on every request.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when API_KEY or API_KEY_NAME is unset or empty.</exception>
        public APIKeyAttribute()
        {
            _apiKeyHeaderName = RequireEnvironmentVariable("API_KEY_NAME");
            _expectedKey = Encoding.UTF8.GetBytes(RequireEnvironmentVariable("API_KEY"));
        }

        /// <summary>
        /// Validates the API key in the request headers before allowing the action to execute.
        /// </summary>
        /// <param name="context">The context for the executing action.</param>
        /// <param name="next">The delegate for the next action in the pipeline.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>
        /// If the API key is missing or incorrect, returns an UnauthorizedResult and logs a warning.
        /// The client's IP address is included in the warning logs for security tracking.
        /// </remarks>
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            string apiKeyHeaderName = _apiKeyHeaderName;

            Microsoft.Extensions.Primitives.StringValues potentialKey = "";
            string clientIP = APIUtility.GetClientPublicIPAddress(context);

            // Try to retrieve the API key from the request headers
            if (!context.HttpContext.Request.Headers.TryGetValue(apiKeyHeaderName, out potentialKey))
            {
                // A missing key is an ordinary client 401 (Info); a wrong key is worth a Warning.
                IPBlacklistMiddleware.AddLog(LogMessage.Info($"API Key not found in header '{apiKeyHeaderName}' from IP {clientIP} ({APIUtility.DescribeRequest(context.HttpContext)})"));
                context.Result = new UnauthorizedResult();
                return;
            }

            // Constant-time comparison so a wrong key cannot be guessed by timing.
            if (!ApiKeyAuthMiddleware.IsAuthorized(potentialKey.ToString(), _expectedKey))
            {
                // Never log the key that was sent: it may be a real secret (another environment's key, a typo of ours).
                IPBlacklistMiddleware.AddLog(LogMessage.Warning($"IP {clientIP} provided an incorrect key ({potentialKey.ToString().Length} chars) in header '{apiKeyHeaderName}' for {APIUtility.DescribeRequest(context.HttpContext)}"));
                context.Result = new UnauthorizedResult();
                return;
            }

            await next();
        }

        /// <summary>
        /// Reads a required environment variable.
        /// </summary>
        /// <param name="name">The environment variable name.</param>
        /// <returns>The non-empty value.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the variable is unset or empty.</exception>
        private static string RequireEnvironmentVariable(string name)
        {
            string value = Environment.GetEnvironmentVariable(name);
            if (string.IsNullOrEmpty(value))
            {
                throw new InvalidOperationException($"Environment variable {name} must be set to use APIKeyAttribute");
            }
            return value;
        }
    }
}
