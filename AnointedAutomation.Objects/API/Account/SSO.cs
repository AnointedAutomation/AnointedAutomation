// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me
//Stewarded by Alexander Fields

using System.Runtime.Serialization;
using AnointedAutomation.Objects.Apple;
using AnointedAutomation.Objects.Facebook;
using AnointedAutomation.Objects.Google;
using AnointedAutomation.Objects.Microsoft;

namespace AnointedAutomation.Objects.Account
{
    [System.Serializable]
    /// <summary>
    /// Groups every external identity linked to a <see cref="User"/> under one object. Each slot is modeled
    /// on what THAT provider actually returns, never forced into one shared shape: <c>AnointedAutomation.Shopify.Customer</c> (the
    /// Shopify Admin REST customer) for Shopify, and hand-built types modeled on each provider's own
    /// reference (Microsoft Graph <c>User</c> naming, Meta's Business SDK <c>User</c>, Apple's docs) for
    /// Microsoft, Facebook and Apple, with the existing <see cref="GoogleObjects"/> for Google. A null slot means the user has never signed in with (or been linked to) that provider.
    /// The property that carries it is <c>User.SSO</c> and its BSON element key is <c>SSO</c>.
    /// </summary>
    public class SSO
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SSO"/> class.
        /// </summary>
        public SSO()
        { }

        /// <summary>The user's Google identity (token info + profile), null if never linked.</summary>
        [DataMember]
        public GoogleObjects Google { get; set; }

        /// <summary>
        /// The user's Microsoft (Entra ID / personal account) identity built from the verified id_token
        /// claims, null if never linked.
        /// </summary>
        [DataMember]
        public MicrosoftUser Microsoft { get; set; }

        /// <summary>The user's Sign in with Apple identity, null if never linked.</summary>
        [DataMember]
        public AppleSignIn Apple { get; set; }

        /// <summary>The user's Facebook Login identity, null if never linked.</summary>
        [DataMember]
        public FacebookLogin Facebook { get; set; }

        /// <summary>
        /// The user's Shopify customer as the Admin REST <c>Customer</c>, null if not linked. Its
        /// <c>Id</c> is the source of truth for the Shopify customer id; the legacy
        /// <c>Meta["ShopifyCustomerId"]</c> key is kept as a back-compat mirror only.
        /// </summary>
        [DataMember]
        public global::AnointedAutomation.Shopify.Customer Shopify { get; set; }
    }
}
