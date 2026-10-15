// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//Stewarded by Alexander Fields

using Newtonsoft.Json;

namespace AnointedAutomation.Objects.ShopifyCustomerAccounts
{
    /// <summary>
    /// Shopify Customer Account API OIDC id_token claims (<c>https://shopify.dev/docs/api/customer</c>,
    /// <c>https://shopify.dev/docs/storefronts/headless/building-with-the-customer-account-api</c>). The shop's
    /// discovery document (<c>https://shopify.com/authentication/{shopId}/.well-known/openid-configuration</c>) lists
    /// <c>claims_supported</c>: iss, sub, aud, exp, iat, nonce, sid, email, email_verified. <c>given_name</c> /
    /// <c>family_name</c> are not in that list and stay null unless Shopify sends them (the Customer Account API
    /// <c>customer { firstName lastName }</c> query is the documented source). Times are UNIX epoch seconds.
    /// </summary>
    public class ShopifyCustomerAccountClaims
    {
        /// <summary>Issuer (<c>https://shopify.com/authentication/{shopId}</c>).</summary>
        [JsonProperty("iss")]
        public string Iss { get; set; }

        /// <summary>The customer's stable id for this shop.</summary>
        [JsonProperty("sub")]
        public string Sub { get; set; }

        /// <summary>Audience: our Customer Account API client id.</summary>
        [JsonProperty("aud")]
        public string Aud { get; set; }

        /// <summary>Expiry, epoch seconds.</summary>
        [JsonProperty("exp")]
        public long? Exp { get; set; }

        /// <summary>Issued-at, epoch seconds.</summary>
        [JsonProperty("iat")]
        public long? Iat { get; set; }

        /// <summary>The nonce from the authorization request.</summary>
        [JsonProperty("nonce")]
        public string Nonce { get; set; }

        /// <summary>The Shopify session id (used for logout).</summary>
        [JsonProperty("sid")]
        public string Sid { get; set; }

        /// <summary>The customer's email (<c>email</c> scope).</summary>
        [JsonProperty("email")]
        public string Email { get; set; }

        /// <summary>Whether Shopify has verified the email.</summary>
        [JsonProperty("email_verified")]
        public bool? EmailVerified { get; set; }

        /// <summary>The customer's first name, when sent.</summary>
        [JsonProperty("given_name")]
        public string GivenName { get; set; }

        /// <summary>The customer's last name, when sent.</summary>
        [JsonProperty("family_name")]
        public string FamilyName { get; set; }
    }
}
