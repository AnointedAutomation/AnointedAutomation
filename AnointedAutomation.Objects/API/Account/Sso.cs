// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me
//Stewarded by Alexander Fields

using System.Runtime.Serialization;

namespace AnointedAutomation.Objects.Account
{
    [System.Serializable]
    /// <summary>
    /// Groups every external identity linked to a <see cref="User"/> under one object, so
    /// Google/Microsoft/Apple/Facebook/Shopify identity metadata lives in one place instead of
    /// scattered fields. Each provider slot is a nullable <see cref="SsoIdentity"/>; a null slot means
    /// the user has never signed in with that provider. As a reference type this object serializes
    /// PascalCase (<c>Sso</c>), while its provider slots, being nullable value types, serialize
    /// camelCase (google, microsoft, apple, facebook, shopify).
    /// </summary>
    public class Sso
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Sso"/> class.
        /// </summary>
        public Sso()
        { }

        /// <summary>
        /// Gets or sets the linked Google identity (null if the user never signed in with Google).
        /// </summary>
        [DataMember]
        public SsoIdentity? Google { get; set; }

        /// <summary>
        /// Gets or sets the linked Microsoft identity (null if the user never signed in with Microsoft).
        /// </summary>
        [DataMember]
        public SsoIdentity? Microsoft { get; set; }

        /// <summary>
        /// Gets or sets the linked Apple identity (null if the user never signed in with Apple).
        /// </summary>
        [DataMember]
        public SsoIdentity? Apple { get; set; }

        /// <summary>
        /// Gets or sets the linked Facebook identity (null if the user never signed in with Facebook).
        /// </summary>
        [DataMember]
        public SsoIdentity? Facebook { get; set; }

        /// <summary>
        /// Gets or sets the linked Shopify customer identity (null if the user has no Shopify link).
        /// </summary>
        [DataMember]
        public SsoIdentity? Shopify { get; set; }
    }
}
