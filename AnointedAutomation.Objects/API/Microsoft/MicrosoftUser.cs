// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me
//Stewarded by Alexander Fields

using Newtonsoft.Json;

namespace AnointedAutomation.Objects.Microsoft
{
    /// <summary>
    /// A Microsoft (Entra ID / personal Microsoft account) identity, built from the verified id_token claims.
    /// Member names follow the Microsoft Graph <c>User</c> resource so it reads like Graph, but only the
    /// members the id_token actually carries are modeled, and it takes no dependency on the Graph SDK.
    /// Stored at <c>User.SSO.Microsoft</c>.
    /// </summary>
    public class MicrosoftUser
    {
        /// <summary>The object id (<c>oid</c> claim), falling back to <c>sub</c> when <c>oid</c> is absent.</summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>Display name (<c>name</c> claim).</summary>
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        /// <summary>Email (<c>email</c> claim), absent when the email scope or claim was not granted.</summary>
        [JsonProperty("mail")]
        public string Mail { get; set; }

        /// <summary>Sign-in name (<c>preferred_username</c> claim).</summary>
        [JsonProperty("userPrincipalName")]
        public string UserPrincipalName { get; set; }

        /// <summary>Given name (<c>given_name</c> claim).</summary>
        [JsonProperty("givenName")]
        public string GivenName { get; set; }

        /// <summary>Surname (<c>family_name</c> claim).</summary>
        [JsonProperty("surname")]
        public string Surname { get; set; }

        /// <summary>The directory tenant id (<c>tid</c> claim). Personal accounts carry 9188040d-6c67-4c5b-b112-36a304b66dad.</summary>
        [JsonProperty("tenantId")]
        public string TenantId { get; set; }

        /// <summary>The identity provider that authenticated the user (<c>idp</c> claim), e.g. <c>live.com</c> for personal accounts.</summary>
        [JsonProperty("identityProvider")]
        public string IdentityProvider { get; set; }
    }
}
