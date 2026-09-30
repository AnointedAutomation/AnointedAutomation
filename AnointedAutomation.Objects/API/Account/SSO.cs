// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me
//Stewarded by Alexander Fields

using System.Runtime.Serialization;
using AnointedAutomation.Objects.Apple;
using AnointedAutomation.Objects.Facebook;
using AnointedAutomation.Objects.Google;
using AnointedAutomation.Objects.Microsoft;
using AnointedAutomation.Objects.Shopify;

namespace AnointedAutomation.Objects.Account
{
    [System.Serializable]
    /// <summary>
    /// Groups every external identity linked to a <see cref="User"/> under one object. Each slot holds
    /// the PROVIDER'S OWN REAL OBJECT with the actual data that provider returns (full claim / profile
    /// sets), NOT a shared flattened shape, so no data the provider gives is thrown away. A null slot
    /// means the user has never signed in with (or been linked to) that provider. As a reference type
    /// this object serializes PascalCase; the property that carries it is <c>User.SSO</c> and its BSON
    /// element key is <c>SSO</c>. The provider slots, also reference types, serialize PascalCase
    /// (<c>Google</c>, <c>Microsoft</c>, <c>Apple</c>, <c>Facebook</c>, <c>Shopify</c>).
    /// </summary>
    public class SSO
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SSO"/> class.
        /// </summary>
        public SSO()
        { }

        /// <summary>
        /// The user's Google identity (token info + profile), null if never linked. Reuses the existing
        /// <see cref="GoogleObjects"/> type; this is the new canonical home for it (replacing the old
        /// top-level <c>User.Google</c> field).
        /// </summary>
        [DataMember]
        public GoogleObjects Google { get; set; }

        /// <summary>The user's Microsoft (Azure AD / Entra) identity, null if never linked.</summary>
        [DataMember]
        public MicrosoftObjects Microsoft { get; set; }

        /// <summary>The user's Sign in with Apple identity, null if never linked.</summary>
        [DataMember]
        public AppleObjects Apple { get; set; }

        /// <summary>The user's Facebook identity, null if never linked.</summary>
        [DataMember]
        public FacebookObjects Facebook { get; set; }

        /// <summary>
        /// The user's Shopify customer link, null if not linked. This is the source of truth for the
        /// Shopify customer id; the legacy <c>Meta["ShopifyCustomerId"]</c> key is kept as a back-compat
        /// mirror only.
        /// </summary>
        [DataMember]
        public ShopifyObjects Shopify { get; set; }
    }
}
