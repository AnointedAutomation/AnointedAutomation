// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AnointedAutomation.SSO
{
    /// <summary>
    /// Validates an OpenID Connect Back-Channel Logout 1.0 Logout Token: RS256 only, signature against the
    /// provider's JWKS (refetched once on an unknown <c>kid</c>), <c>typ</c> logout+jwt, exact <c>iss</c>,
    /// <c>aud</c> containing the client_id, <c>iat</c> within the max age, <c>exp</c> not passed, the
    /// back-channel event present, a <c>jti</c>, <c>sub</c> and/or <c>sid</c>, and NO <c>nonce</c>.
    /// Usable without HTTP; replay tracking is the caller's job (the endpoint does it).
    /// </summary>
    public class AnointedLogoutTokenValidator
    {
        private readonly AnointedLogoutValidationOptions _options;
        private readonly IConfigurationManager<OpenIdConnectConfiguration> _configurationManager;
        private readonly TimeProvider _timeProvider;
        private readonly JsonWebTokenHandler _handler = new JsonWebTokenHandler();

        /// <summary>Creates a validator.</summary>
        /// <param name="options">What the token must satisfy; validated here.</param>
        /// <param name="configurationManager">Source of the provider's signing keys.</param>
        /// <param name="timeProvider">The clock.</param>
        public AnointedLogoutTokenValidator(AnointedLogoutValidationOptions options, IConfigurationManager<OpenIdConnectConfiguration> configurationManager, TimeProvider timeProvider)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            if (configurationManager == null)
            {
                throw new ArgumentNullException(nameof(configurationManager));
            }
            if (timeProvider == null)
            {
                throw new ArgumentNullException(nameof(timeProvider));
            }
            options.Validate();
            _options = options;
            _configurationManager = configurationManager;
            _timeProvider = timeProvider;
        }

        /// <summary>The rules this validator applies.</summary>
        public AnointedLogoutValidationOptions Options
        {
            get { return _options; }
        }

        /// <summary>
        /// Validates a Logout Token. An invalid token returns a failed result; a failure to read the provider's
        /// keys throws, since the token was not proven invalid (answer 500 so the provider retries).
        /// </summary>
        /// <param name="logoutToken">The raw <c>logout_token</c> value.</param>
        /// <param name="cancellationToken">Cancels the key lookup.</param>
        /// <returns>The validated logout or the first rule that failed.</returns>
        public async Task<AnointedLogoutValidationResult> ValidateAsync(string logoutToken, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(logoutToken))
            {
                return AnointedLogoutValidationResult.Failure("logout_token is missing");
            }

            JsonWebToken jwt = ReadToken(logoutToken);
            if (jwt == null)
            {
                return AnointedLogoutValidationResult.Failure("logout_token is not a compact JWS");
            }
            if (!string.Equals(jwt.Alg, AnointedAutomationDefaults.SigningAlgorithm, StringComparison.Ordinal))
            {
                return AnointedLogoutValidationResult.Failure("alg " + jwt.Alg + " is not allowed");
            }
            if (!string.Equals(jwt.Typ, AnointedAutomationDefaults.LogoutTokenType, StringComparison.Ordinal))
            {
                return AnointedLogoutValidationResult.Failure("typ is not logout+jwt");
            }

            OpenIdConnectConfiguration configuration = await _configurationManager.GetConfigurationAsync(cancellationToken).ConfigureAwait(false);
            if (!string.IsNullOrEmpty(jwt.Kid) && !HasKey(configuration, jwt.Kid))
            {
                _configurationManager.RequestRefresh();
                configuration = await _configurationManager.GetConfigurationAsync(cancellationToken).ConfigureAwait(false);
            }

            TokenValidationParameters parameters = new TokenValidationParameters
            {
                ValidIssuer = _options.Issuer,
                ValidateIssuer = true,
                ValidAudience = _options.ClientId,
                ValidateAudience = true,
                IssuerSigningKeys = configuration.SigningKeys,
                ValidAlgorithms = new List<string> { AnointedAutomationDefaults.SigningAlgorithm },
                ValidTypes = new List<string> { AnointedAutomationDefaults.LogoutTokenType },
                RequireSignedTokens = true,
                ValidateLifetime = false,
                RequireExpirationTime = false,
            };
            TokenValidationResult validation = await _handler.ValidateTokenAsync(logoutToken, parameters).ConfigureAwait(false);
            if (!validation.IsValid)
            {
                return AnointedLogoutValidationResult.Failure("token rejected: " + validation.Exception.Message);
            }

            return ValidateClaims(jwt);
        }

        /// <summary>Parses a compact JWS, or returns null when it is malformed.</summary>
        /// <param name="logoutToken">The raw token.</param>
        /// <returns>The parsed token or null.</returns>
        private JsonWebToken ReadToken(string logoutToken)
        {
            if (!_handler.CanReadToken(logoutToken))
            {
                return null;
            }
            try
            {
                return _handler.ReadJsonWebToken(logoutToken);
            }
            catch (ArgumentException)
            {
                // SecurityTokenMalformedException derives from ArgumentException.
                return null;
            }
        }

        /// <summary>Whether the configuration holds a signing key with this id.</summary>
        /// <param name="configuration">The provider configuration.</param>
        /// <param name="kid">The key id from the token header.</param>
        /// <returns>True when a key matches exactly.</returns>
        private static bool HasKey(OpenIdConnectConfiguration configuration, string kid)
        {
            foreach (SecurityKey key in configuration.SigningKeys)
            {
                if (string.Equals(key.KeyId, kid, StringComparison.Ordinal))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>Applies the Logout Token claim rules to an already signature-checked token.</summary>
        /// <param name="jwt">The token.</param>
        /// <returns>The validated logout or the first rule that failed.</returns>
        private AnointedLogoutValidationResult ValidateClaims(JsonWebToken jwt)
        {
            string payloadJson = Base64UrlEncoder.Decode(jwt.EncodedPayload);
            using (JsonDocument document = JsonDocument.Parse(payloadJson))
            {
                JsonElement payload = document.RootElement;
                long now = _timeProvider.GetUtcNow().ToUnixTimeSeconds();
                long skew = (long)_options.ClockSkew.TotalSeconds;

                if (!TryGetLong(payload, "iat", out long iat))
                {
                    return AnointedLogoutValidationResult.Failure("iat is missing");
                }
                if (iat - skew > now)
                {
                    return AnointedLogoutValidationResult.Failure("token was issued in the future");
                }
                if (now - iat > (long)_options.MaxTokenAge.TotalSeconds + skew)
                {
                    return AnointedLogoutValidationResult.Failure("token is too old");
                }
                if (!TryGetLong(payload, "exp", out long exp))
                {
                    return AnointedLogoutValidationResult.Failure("exp is missing");
                }
                if (now > exp + skew)
                {
                    return AnointedLogoutValidationResult.Failure("token is expired");
                }
                if (!payload.TryGetProperty("events", out JsonElement events)
                    || events.ValueKind != JsonValueKind.Object
                    || !events.TryGetProperty(AnointedAutomationDefaults.BackchannelLogoutEvent, out JsonElement logoutEvent)
                    || logoutEvent.ValueKind != JsonValueKind.Object)
                {
                    return AnointedLogoutValidationResult.Failure("events does not carry the back-channel logout event");
                }
                if (payload.TryGetProperty("nonce", out JsonElement _))
                {
                    return AnointedLogoutValidationResult.Failure("a logout token must not carry a nonce");
                }
                string jti = GetString(payload, "jti");
                if (jti == null)
                {
                    return AnointedLogoutValidationResult.Failure("jti is missing");
                }
                string sub = GetString(payload, "sub");
                string sid = GetString(payload, "sid");
                if (sub == null && sid == null)
                {
                    return AnointedLogoutValidationResult.Failure("neither sub nor sid is present");
                }
                return AnointedLogoutValidationResult.Success(new AnointedLogout
                {
                    Sub = sub,
                    Sid = sid,
                    Jti = jti,
                    ExpiresAt = DateTimeOffset.FromUnixTimeSeconds(exp),
                });
            }
        }

        /// <summary>Reads a numeric claim as whole seconds.</summary>
        /// <param name="payload">The token payload.</param>
        /// <param name="name">The exact claim name.</param>
        /// <param name="value">The value when present.</param>
        /// <returns>True when the claim is a number.</returns>
        private static bool TryGetLong(JsonElement payload, string name, out long value)
        {
            value = 0;
            if (!payload.TryGetProperty(name, out JsonElement element) || element.ValueKind != JsonValueKind.Number)
            {
                return false;
            }
            if (element.TryGetInt64(out value))
            {
                return true;
            }
            if (element.TryGetDouble(out double floating))
            {
                value = (long)floating;
                return true;
            }
            return false;
        }

        /// <summary>Reads a non-empty string claim.</summary>
        /// <param name="payload">The token payload.</param>
        /// <param name="name">The exact claim name.</param>
        /// <returns>The value, or null when absent, empty or not a string.</returns>
        private static string GetString(JsonElement payload, string name)
        {
            if (!payload.TryGetProperty(name, out JsonElement element) || element.ValueKind != JsonValueKind.String)
            {
                return null;
            }
            string value = element.GetString();
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }
            return value;
        }
    }
}
