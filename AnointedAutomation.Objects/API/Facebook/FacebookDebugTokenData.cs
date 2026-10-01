// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//Stewarded by Alexander Fields

using System.Collections.Generic;
using Newtonsoft.Json;

namespace AnointedAutomation.Objects.Facebook
{
    /// <summary>
    /// The <c>data</c> object of the Graph <c>/debug_token</c> response (Graph API debug_token reference),
    /// as returned. Times are UNIX epoch seconds.
    /// </summary>
    public class FacebookDebugTokenData
    {
        /// <summary>The app the token was issued for.</summary>
        [JsonProperty("app_id")]
        public string AppId { get; set; }

        /// <summary>Token type, e.g. USER.</summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>The app's name.</summary>
        [JsonProperty("application")]
        public string Application { get; set; }

        /// <summary>Token expiry, epoch seconds (0 = never).</summary>
        [JsonProperty("expires_at")]
        public long? ExpiresAt { get; set; }

        /// <summary>When data access expires, epoch seconds.</summary>
        [JsonProperty("data_access_expires_at")]
        public long? DataAccessExpiresAt { get; set; }

        /// <summary>Whether the token is valid.</summary>
        [JsonProperty("is_valid")]
        public bool? IsValid { get; set; }

        /// <summary>When the token was issued, epoch seconds.</summary>
        [JsonProperty("issued_at")]
        public long? IssuedAt { get; set; }

        /// <summary>Permissions granted to the token.</summary>
        [JsonProperty("scopes")]
        public List<string> Scopes { get; set; }

        /// <summary>The app-scoped user id the token belongs to.</summary>
        [JsonProperty("user_id")]
        public string UserId { get; set; }

        /// <summary>Per-permission grants, each optionally limited to specific target ids (pages, groups).</summary>
        [JsonProperty("granular_scopes")]
        public List<FacebookGranularScope> GranularScopes { get; set; }
    }

    /// <summary>One <c>granular_scopes</c> entry of <c>/debug_token</c>: <c>{ scope, target_ids }</c>.</summary>
    public class FacebookGranularScope
    {
        /// <summary>The permission name.</summary>
        [JsonProperty("scope")]
        public string Scope { get; set; }

        /// <summary>Object ids the permission is limited to; absent means all.</summary>
        [JsonProperty("target_ids")]
        public List<string> TargetIds { get; set; }
    }
}
