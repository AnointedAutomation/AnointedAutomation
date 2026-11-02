// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-02 00:00:00 Jesus is King ✝️
using System;
using System.Collections.Generic;
using System.Linq;
using AnointedAutomation.APIMiddleware;
using AnointedAutomation.Optimization.Logging;

namespace AnointedAutomation.APIMiddlewares.Tests.Security
{
    /// <summary>
    /// The middleware log buffer is process-wide and other test classes write to it in parallel, so assertions
    /// look only at entries produced by the code under test (identified by their message prefix).
    /// </summary>
    internal static class SecurityTestLogs
    {
        private static readonly string[] Prefixes = new string[] { "IpAllowlist", "CountryBlock", "BotDefense", "FormBotGuard", "Captcha" };

        /// <summary>Gets the buffered entries written by the security middlewares.</summary>
        /// <returns>The entries.</returns>
        public static List<LogMessage> Get()
        {
            return IPBlacklistMiddleware.GetLogs().Where(l => Prefixes.Any(p => l.message.StartsWith(p, StringComparison.Ordinal))).ToList();
        }
    }
}
