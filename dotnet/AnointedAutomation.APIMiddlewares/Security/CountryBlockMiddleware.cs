// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-02 00:00:00 Jesus is King ✝️
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AnointedAutomation.APIMiddleware.Utility;
using AnointedAutomation.Optimization.Logging;
using Microsoft.AspNetCore.Http;

namespace AnointedAutomation.APIMiddleware.Security
{
    /// <summary>
    /// Blocks (403) requests by country of origin: <see cref="CountryBlockMode.Block"/> blocks listed countries,
    /// <see cref="CountryBlockMode.AllowOnly"/> allows only listed ones. Off unless
    /// <see cref="CountryBlockOptions.Enabled"/>. The unknown-country behavior is explicit configuration.
    /// </summary>
    public class CountryBlockMiddleware
    {
        private static readonly IpNetworkList NonRoutable = IpNetworkList.Parse(new string[]
        {
            "127.0.0.0/8", "10.0.0.0/8", "172.16.0.0/12", "192.168.0.0/16", "169.254.0.0/16", "100.64.0.0/10",
            "::1/128", "fc00::/7", "fe80::/10"
        });

        private readonly RequestDelegate _next;
        private readonly CountryBlockOptions _options;
        private readonly ICountryLookup _lookup;
        private readonly HashSet<string> _countries;
        private readonly PathExemptions _exemptions;
        private readonly IpNetworkList _trustedProxies;

        /// <summary>
        /// Initializes a new instance of the <see cref="CountryBlockMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next middleware.</param>
        /// <param name="options">The options (validated here).</param>
        /// <param name="lookup">The IP to country source.</param>
        public CountryBlockMiddleware(RequestDelegate next, CountryBlockOptions options, ICountryLookup lookup)
        {
            _next = next;
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _lookup = lookup ?? throw new ArgumentNullException(nameof(lookup));
            _options.Validate();
            _countries = _options.BuildCountrySet();
            _exemptions = new PathExemptions(_options.ExemptPaths);
            _trustedProxies = _options.ClientIp.BuildTrustedProxies();
        }

        /// <summary>
        /// Checks the request.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            if (!_options.Enabled || _exemptions.IsExempt(context.Request.Path))
            {
                await _next(context);
                return;
            }
            IPAddress client = ClientIpResolver.Resolve(context, _trustedProxies, _options.ClientIp.HonorCfConnectingIp);
            if (client != null && _options.AllowPrivateAddresses && NonRoutable.Contains(client))
            {
                await _next(context);
                return;
            }
            string country = ResolveCountry(context, client);
            string reason = Decide(country);
            if (reason != null)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                IPBlacklistMiddleware.AddLog(LogMessage.Warning(
                    "CountryBlock blocked " + (client == null ? "(no client address)" : client.ToString())
                    + " country=" + (country == null ? "unknown" : country) + " " + APIUtility.DescribeRequest(context) + ": " + reason + "."));
                return;
            }
            await _next(context);
        }

        private string ResolveCountry(HttpContext context, IPAddress client)
        {
            if (_options.UseCfIpCountryHeader && ClientIpResolver.IsPeerTrusted(context, _trustedProxies))
            {
                string header = context.Request.Headers["CF-IPCountry"].ToString().Trim();
                // Cloudflare sends XX when it does not know and T1 for Tor; neither is a country.
                if (Regex.IsMatch(header, "^[A-Za-z]{2}$") && !header.Equals("XX", StringComparison.OrdinalIgnoreCase) && !header.Equals("T1", StringComparison.OrdinalIgnoreCase))
                {
                    return header.ToUpperInvariant();
                }
            }
            return client == null ? null : _lookup.Lookup(client);
        }

        private string Decide(string country)
        {
            if (country == null)
            {
                return _options.UnknownCountry.Value == UnknownCountryAction.Block ? "country unknown (UnknownCountry=Block)" : null;
            }
            bool listed = _countries.Contains(country.ToUpperInvariant());
            if (_options.Mode.Value == CountryBlockMode.Block)
            {
                return listed ? "country is on the block list" : null;
            }
            return listed ? null : "country is not on the allow-only list";
        }
    }
}
