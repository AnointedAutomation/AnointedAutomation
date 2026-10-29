// Copyright © Anointed Automation Corporation., 2024. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2024-10-13 01:57:25 Jesus is King ✝️
// Edited by Alexander Fields https://www.alexanderfields.me 2025-07-02 11:48:25
//Stewarded by Alexander Fields
using AnointedAutomation.APIMiddleware.Utility;
using AnointedAutomation.Optimization.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
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
            string apiKeyHeaderName = GetKeyName();

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

            string correctApiKey = GetKey();

            // Check if the provided key matches the correct key
            if (!correctApiKey.Equals(potentialKey))
            {
                // Never log the key that was sent: it may be a real secret (another environment's key, a typo of ours).
                IPBlacklistMiddleware.AddLog(LogMessage.Warning($"IP {clientIP} provided an incorrect key ({potentialKey.ToString().Length} chars) in header '{apiKeyHeaderName}' for {APIUtility.DescribeRequest(context.HttpContext)}"));
                context.Result = new UnauthorizedResult();
                return;
            }

            await next();
        }

        /// <summary>
        /// Gets the correct API key from environment variables.
        /// </summary>
        /// <returns>The API key value from the API_KEY environment variable.</returns>
        internal string GetKey()
        {
            string apikey = System.Environment.GetEnvironmentVariable("API_KEY");
            return apikey;
        }

        /// <summary>
        /// Gets the name of the header that should contain the API key.
        /// </summary>
        /// <returns>The header name from the API_KEY_NAME environment variable.</returns>
        internal string GetKeyName()
        {
            string keyName = System.Environment.GetEnvironmentVariable("API_KEY_NAME");
            return keyName;
        }
    }
}
