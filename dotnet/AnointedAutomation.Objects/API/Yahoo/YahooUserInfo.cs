// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//Stewarded by Alexander Fields

using Newtonsoft.Json;

namespace AnointedAutomation.Objects.Yahoo
{
    /// <summary>
    /// Yahoo OpenID Connect claims (<c>https://developer.yahoo.com/oauth2/guide/openid_connect/</c>). The claim set is
    /// Yahoo's discovery document <c>claims_supported</c> (<c>https://api.login.yahoo.com/.well-known/openid-configuration</c>:
    /// aud, email, email_verified, birthdate, exp, family_name, given_name, iat, iss, locale, name, sub, auth_time)
    /// plus the id_token's <c>nonce</c> / <c>at_hash</c> from the getting started sample and the standard OIDC
    /// <c>picture</c> / <c>nickname</c>. Times are UNIX epoch seconds. Profile claims need the <c>profile</c> scope,
    /// email ones the <c>email</c> scope.
    /// </summary>
    public class YahooUserInfo
    {
        /// <summary>The user's stable Yahoo identifier (the GUID, same as <c>xoauth_yahoo_guid</c>).</summary>
        [JsonProperty("sub")]
        public string Sub { get; set; }

        /// <summary>Issuer (<c>https://api.login.yahoo.com</c>).</summary>
        [JsonProperty("iss")]
        public string Iss { get; set; }

        /// <summary>Audience: our client id.</summary>
        [JsonProperty("aud")]
        public string Aud { get; set; }

        /// <summary>Expiry, epoch seconds.</summary>
        [JsonProperty("exp")]
        public long? Exp { get; set; }

        /// <summary>Issued-at, epoch seconds.</summary>
        [JsonProperty("iat")]
        public long? Iat { get; set; }

        /// <summary>Time the user authenticated, epoch seconds.</summary>
        [JsonProperty("auth_time")]
        public long? AuthTime { get; set; }

        /// <summary>The nonce from the authentication request.</summary>
        [JsonProperty("nonce")]
        public string Nonce { get; set; }

        /// <summary>Hash of the access token, when issued alongside one.</summary>
        [JsonProperty("at_hash")]
        public string AtHash { get; set; }

        /// <summary>Full name.</summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>First name.</summary>
        [JsonProperty("given_name")]
        public string GivenName { get; set; }

        /// <summary>Last name.</summary>
        [JsonProperty("family_name")]
        public string FamilyName { get; set; }

        /// <summary>Nickname.</summary>
        [JsonProperty("nickname")]
        public string Nickname { get; set; }

        /// <summary>The user's email address.</summary>
        [JsonProperty("email")]
        public string Email { get; set; }

        /// <summary>Whether Yahoo has verified the email address.</summary>
        [JsonProperty("email_verified")]
        public bool? EmailVerified { get; set; }

        /// <summary>Birth date as Yahoo sends it (OIDC <c>YYYY-MM-DD</c> or <c>YYYY</c>).</summary>
        [JsonProperty("birthdate")]
        public string Birthdate { get; set; }

        /// <summary>Locale (e.g. <c>en-US</c>).</summary>
        [JsonProperty("locale")]
        public string Locale { get; set; }

        /// <summary>Profile picture URL.</summary>
        [JsonProperty("picture")]
        public string Picture { get; set; }
    }
}
