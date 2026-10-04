// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//Stewarded by Alexander Fields

using AnointedAutomation.Serialization.Newtonsoft;
using Newtonsoft.Json;

namespace AnointedAutomation.Objects.Apple
{
    /// <summary>
    /// The claim set of a Sign in with Apple identity token, modeled on Apple's documentation
    /// ("Authenticating users with Sign in with Apple", identity token payload). No maintained NuGet package
    /// models this payload, so it is hand-built. Times are UNIX epoch seconds as Apple sends them. The
    /// boolean claims arrive as either booleans or "true"/"false" strings and are normalized to bool.
    /// </summary>
    public class AppleIdTokenClaims
    {
        /// <summary>Issuer, always <c>https://appleid.apple.com</c>.</summary>
        [JsonProperty("iss")]
        public string Iss { get; set; }

        /// <summary>Audience: our client (services) id.</summary>
        [JsonProperty("aud")]
        public string Aud { get; set; }

        /// <summary>Expiry, epoch seconds.</summary>
        [JsonProperty("exp")]
        public long? Exp { get; set; }

        /// <summary>Issued-at, epoch seconds.</summary>
        [JsonProperty("iat")]
        public long? Iat { get; set; }

        /// <summary>Apple's stable, team-scoped user id (the durable link key).</summary>
        [JsonProperty("sub")]
        public string Sub { get; set; }

        /// <summary>The nonce passed in the authorization request, when one was sent.</summary>
        [JsonProperty("nonce")]
        public string Nonce { get; set; }

        /// <summary>Whether the platform supports nonces.</summary>
        [JsonProperty("nonce_supported")]
        [JsonConverter(typeof(StringOrBoolConverter))]
        public bool? NonceSupported { get; set; }

        /// <summary>Hash of the authorization code, when issued alongside one.</summary>
        [JsonProperty("c_hash")]
        public string CHash { get; set; }

        /// <summary>Hash of the access token, when issued alongside one.</summary>
        [JsonProperty("at_hash")]
        public string AtHash { get; set; }

        /// <summary>Time the user authenticated, epoch seconds.</summary>
        [JsonProperty("auth_time")]
        public long? AuthTime { get; set; }

        /// <summary>The user's email (may be a private relay address).</summary>
        [JsonProperty("email")]
        public string Email { get; set; }

        /// <summary>Whether Apple verified the email.</summary>
        [JsonProperty("email_verified")]
        [JsonConverter(typeof(StringOrBoolConverter))]
        public bool? EmailVerified { get; set; }

        /// <summary>Whether the email is an Apple private-relay address.</summary>
        [JsonProperty("is_private_email")]
        [JsonConverter(typeof(StringOrBoolConverter))]
        public bool? IsPrivateEmail { get; set; }

        /// <summary>Apple's real-user indicator (0 Unsupported, 1 Unknown, 2 LikelyReal).</summary>
        [JsonProperty("real_user_status")]
        public AppleRealUserStatus? RealUserStatus { get; set; }

        /// <summary>The user's id under the previous team, present only during an app transfer.</summary>
        [JsonProperty("transfer_sub")]
        public string TransferSub { get; set; }
    }

    /// <summary>
    /// Apple's <c>real_user_status</c> values. The numbering is Apple's own (0 is Unsupported, not an
    /// unknown placeholder), so the values must not be renumbered.
    /// </summary>
    public enum AppleRealUserStatus
    {
        /// <summary>The platform does not support the indicator.</summary>
        Unsupported = 0,

        /// <summary>Apple could not determine whether the user is real.</summary>
        Unknown = 1,

        /// <summary>The user is likely real.</summary>
        LikelyReal = 2
    }
}
