// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me
//Stewarded by Alexander Fields

using System.Collections.Generic;
using Newtonsoft.Json;

namespace AnointedAutomation.Objects.Microsoft
{
    /// <summary>
    /// The validated claim set from a Microsoft (Azure AD / Entra) id_token. Fields are the standard
    /// Microsoft identity-platform v2.0 claims per Microsoft's official documentation
    /// (learn.microsoft.com "ID token claims reference"): <c>oid</c>, <c>tid</c>, <c>sub</c>, <c>iss</c>,
    /// <c>aud</c>, <c>preferred_username</c> and <c>email</c>, plus a raw-claims dictionary so any claim
    /// Microsoft adds later is captured even before this type models it explicitly.
    /// </summary>
    public class MicrosoftTokenInfo
    {
        /// <summary>Immutable object id (the stable, durable link key for the user in the tenant).</summary>
        [JsonProperty("oid")]
        public string oid { get; set; }

        /// <summary>Tenant id the account belongs to.</summary>
        [JsonProperty("tid")]
        public string tid { get; set; }

        /// <summary>Standard OIDC subject id (per-app pairwise; <see cref="oid"/> is preferred as the key).</summary>
        [JsonProperty("sub")]
        public string sub { get; set; }

        /// <summary>The token issuer (the Microsoft v2.0 tenant issuer URL).</summary>
        [JsonProperty("iss")]
        public string iss { get; set; }

        /// <summary>The audience the token was issued for (our application/client id).</summary>
        [JsonProperty("aud")]
        public string aud { get; set; }

        /// <summary>The username Microsoft prefers to display (usually the UPN / email).</summary>
        [JsonProperty("preferred_username")]
        public string preferredUsername { get; set; }

        /// <summary>The user's email address, when present in the token.</summary>
        [JsonProperty("email")]
        public string email { get; set; }

        /// <summary>
        /// The full validated claim set as raw key/value pairs, so a claim Microsoft adds later is captured
        /// even before this type models it explicitly. May be null.
        /// </summary>
        [JsonProperty("rawClaims")]
        public Dictionary<string, string> rawClaims { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MicrosoftTokenInfo"/> class.
        /// </summary>
        public MicrosoftTokenInfo()
        {
        }
    }
}
