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

        /// <summary>
        /// A view of the security log entries written after the window was created, so a test sees only its own
        /// entries without clearing the shared buffer (clearing would also detach other tests' log handlers).
        /// </summary>
        public sealed class Window
        {
            private readonly LogMessage _last;

            /// <summary>Starts a window at the current end of the buffer.</summary>
            public Window()
            {
                List<LogMessage> all = SecurityTestLogs.Get();
                _last = all.Count == 0 ? null : all[all.Count - 1];
            }

            /// <summary>Gets the entries added since the window started.</summary>
            /// <returns>The entries.</returns>
            public List<LogMessage> Get()
            {
                List<LogMessage> all = SecurityTestLogs.Get();
                int index = _last == null ? -1 : all.FindIndex(l => object.ReferenceEquals(l, _last));
                return all.Skip(index + 1).ToList();
            }
        }

        /// <summary>Gets the buffered entries written by the security middlewares.</summary>
        /// <returns>The entries.</returns>
        public static List<LogMessage> Get()
        {
            return IPBlacklistMiddleware.GetLogs().Where(l => Prefixes.Any(p => l.message.StartsWith(p, StringComparison.Ordinal))).ToList();
        }
    }
}
