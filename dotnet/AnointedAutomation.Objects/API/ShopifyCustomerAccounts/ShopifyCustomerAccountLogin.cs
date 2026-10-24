// Copyright © Anointed Automation Corporation., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//Stewarded by Alexander Fields

namespace AnointedAutomation.Objects.ShopifyCustomerAccounts
{
    /// <summary>
    /// What "Sign in with Shopify" through the Customer Account API gives us: the verified OIDC id_token claims.
    /// Stored at <c>User.SSO.ShopifyCustomerAccount</c>, separate from <c>User.SSO.Shopify</c> (the store's Admin
    /// customer link). Never holds an access or refresh token.
    /// </summary>
    public class ShopifyCustomerAccountLogin
    {
        /// <summary>The verified id_token claims.</summary>
        public ShopifyCustomerAccountClaims IdToken { get; set; }
    }
}
