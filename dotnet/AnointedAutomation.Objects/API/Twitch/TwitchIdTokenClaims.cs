// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//Stewarded by Alexander Fields

using System;
using Newtonsoft.Json;

namespace AnointedAutomation.Objects.Twitch
{
    /// <summary>
    /// Twitch OIDC claims, as the id_token payload or <c>GET https://id.twitch.tv/oauth2/userinfo</c> carries them
    /// ("Requesting claims", <c>https://dev.twitch.tv/docs/authentication/getting-tokens-oidc/</c>). Times are UNIX
    /// epoch seconds. The non-default claims (<c>email</c>, <c>email_verified</c>, <c>picture</c>,
    /// <c>preferred_username</c>, <c>updated_at</c>) are present only when requested through the <c>claims</c>
    /// parameter; email ones also need <c>user:read:email</c>.
    /// </summary>
    public class TwitchIdTokenClaims
    {
        /// <summary>The client ID of the application that requested the user's authorization.</summary>
        [JsonProperty("aud")]
        public string Aud { get; set; }

        /// <summary>The client ID of the application that received the user's authorization.</summary>
        [JsonProperty("azp")]
        public string Azp { get; set; }

        /// <summary>Expiry, epoch seconds.</summary>
        [JsonProperty("exp")]
        public long? Exp { get; set; }

        /// <summary>Issued-at, epoch seconds.</summary>
        [JsonProperty("iat")]
        public long? Iat { get; set; }

        /// <summary>The URI of the issuing authority (<c>https://id.twitch.tv/oauth2</c>).</summary>
        [JsonProperty("iss")]
        public string Iss { get; set; }

        /// <summary>The ID of the user that authorized the app.</summary>
        [JsonProperty("sub")]
        public string Sub { get; set; }

        /// <summary>The nonce from the authorization request, when one was sent.</summary>
        [JsonProperty("nonce")]
        public string Nonce { get; set; }

        /// <summary>The user's email; null when <c>email_verified</c> is false.</summary>
        [JsonProperty("email")]
        public string Email { get; set; }

        /// <summary>Whether Twitch has verified the user's email address.</summary>
        [JsonProperty("email_verified")]
        public bool? EmailVerified { get; set; }

        /// <summary>A URL to the user's profile image, or a default image.</summary>
        [JsonProperty("picture")]
        public string Picture { get; set; }

        /// <summary>The user's display name.</summary>
        [JsonProperty("preferred_username")]
        public string PreferredUsername { get; set; }

        /// <summary>When the user last updated their profile (UTC).</summary>
        [JsonProperty("updated_at")]
        public DateTime? UpdatedAt { get; set; }
    }
}
