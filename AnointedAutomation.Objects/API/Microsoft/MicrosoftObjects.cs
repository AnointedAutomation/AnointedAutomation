// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me
//Stewarded by Alexander Fields

using System.Collections.Generic;
using Newtonsoft.Json;

namespace AnointedAutomation.Objects.Microsoft
{
    /// <summary>
    /// The real Microsoft (Azure AD / Entra) identity for a user who signed in with Microsoft, holding
    /// the ACTUAL id_token claims the provider returns rather than a flattened shared shape. Mirrors the
    /// structure style of <see cref="AnointedAutomation.Objects.Google.GoogleObjects"/>: this is the
    /// object stored at <c>User.SSO.Microsoft</c>. The stable link key is <see cref="oid"/> (the
    /// immutable object id), falling back to <see cref="sub"/>.
    /// </summary>
    public class MicrosoftObjects
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MicrosoftObjects"/> class.
        /// </summary>
        public MicrosoftObjects()
        {
        }

        /// <summary>Immutable object id (the stable, durable link key for the user in the tenant).</summary>
        [JsonProperty("oid")]
        public string oid { get; set; }

        /// <summary>Tenant id the account belongs to.</summary>
        [JsonProperty("tid")]
        public string tid { get; set; }

        /// <summary>Standard OIDC subject id (per-app pairwise; <see cref="oid"/> is preferred as the key).</summary>
        [JsonProperty("sub")]
        public string sub { get; set; }

        /// <summary>The username Microsoft prefers to display (usually the UPN / email).</summary>
        [JsonProperty("preferred_username")]
        public string preferredUsername { get; set; }

        /// <summary>The user's full display name.</summary>
        [JsonProperty("name")]
        public string name { get; set; }

        /// <summary>The user's email address, when present in the token.</summary>
        [JsonProperty("email")]
        public string email { get; set; }

        /// <summary>The user's given (first) name.</summary>
        [JsonProperty("given_name")]
        public string givenName { get; set; }

        /// <summary>The user's family (last) name.</summary>
        [JsonProperty("family_name")]
        public string familyName { get; set; }

        /// <summary>
        /// The full validated claim set as raw key/value pairs, so a claim Microsoft adds later is captured
        /// even before this type models it explicitly. May be null.
        /// </summary>
        [JsonProperty("rawClaims")]
        public Dictionary<string, string> rawClaims { get; set; }

        /// <summary>When this identity was first linked to the user.</summary>
        [JsonProperty("linkedAt")]
        public System.DateTime? linkedAt { get; set; }
    }
}
