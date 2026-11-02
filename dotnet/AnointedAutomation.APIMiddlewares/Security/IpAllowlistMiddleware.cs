// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-02 00:00:00 Jesus is King ✝️
using System.Net;
using System.Threading.Tasks;
using AnointedAutomation.APIMiddleware.Utility;
using AnointedAutomation.Optimization.Logging;
using Microsoft.AspNetCore.Http;

namespace AnointedAutomation.APIMiddleware.Security
{
    /// <summary>
    /// Blocks (403) every request whose client IP is not on the allowlist. Off unless
    /// <see cref="IpAllowlistOptions.Enabled"/> is true. An unresolvable client address is blocked, never allowed.
    /// </summary>
    public class IpAllowlistMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IpAllowlistState _state;

        /// <summary>
        /// Initializes a new instance of the <see cref="IpAllowlistMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next middleware.</param>
        /// <param name="state">The live allowlist.</param>
        public IpAllowlistMiddleware(RequestDelegate next, IpAllowlistState state)
        {
            _next = next;
            _state = state;
        }

        /// <summary>
        /// Checks the request.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            if (!_state.Options.Enabled || _state.Exemptions.IsExempt(context.Request.Path))
            {
                await _next(context);
                return;
            }
            IPAddress client = ClientIpResolver.Resolve(context, _state.TrustedProxies, _state.Options.ClientIp.HonorCfConnectingIp);
            if (client == null)
            {
                Block(context, "(no client address)", "no client address");
                return;
            }
            string rule = _state.FindAllowingRule(client);
            if (rule == null)
            {
                Block(context, client.ToString(), "no allowlist rule matched");
                return;
            }
            await _next(context);
        }

        private static void Block(HttpContext context, string ip, string matchedRule)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            IPBlacklistMiddleware.AddLog(LogMessage.Warning(
                "IpAllowlist blocked " + ip + " " + APIUtility.DescribeRequest(context) + ": " + matchedRule + "."));
        }
    }
}
