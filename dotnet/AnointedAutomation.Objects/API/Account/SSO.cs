// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//Stewarded by Alexander Fields

using System.Runtime.Serialization;
using AnointedAutomation.Objects.Amazon;
using AnointedAutomation.Objects.Apple;
using AnointedAutomation.Objects.Discord;
using AnointedAutomation.Objects.Facebook;
using AnointedAutomation.Objects.GitHub;
using AnointedAutomation.Objects.Google;
using AnointedAutomation.Objects.LinkedIn;
using AnointedAutomation.Objects.Microsoft;
using AnointedAutomation.Objects.Reddit;
using AnointedAutomation.Objects.ShopifyCustomerAccounts;
using AnointedAutomation.Objects.Twitch;
using AnointedAutomation.Objects.Yahoo;

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

        /// <summary>The user's Discord identity (the OAuth2 <c>/users/@me</c> user), null if never linked.</summary>
        [DataMember]
        public DiscordLogin Discord { get; set; }

        /// <summary>The user's GitHub identity (<c>GET /user</c> + <c>GET /user/emails</c>), null if never linked.</summary>
        [DataMember]
        public GitHubLogin GitHub { get; set; }

        /// <summary>The user's LinkedIn identity (the OIDC userinfo), null if never linked.</summary>
        [DataMember]
        public LinkedInLogin LinkedIn { get; set; }

        /// <summary>The user's Twitch identity (Get Users user + OIDC claims), null if never linked.</summary>
        [DataMember]
        public TwitchLogin Twitch { get; set; }

        /// <summary>The user's Yahoo identity (the OIDC claims), null if never linked.</summary>
        [DataMember]
        public YahooLogin Yahoo { get; set; }

        /// <summary>The user's Login with Amazon identity (the customer profile), null if never linked.</summary>
        [DataMember]
        public AmazonLogin Amazon { get; set; }

        /// <summary>The user's Reddit identity (the <c>/api/v1/me</c> user, no email), null if never linked.</summary>
        [DataMember]
        public RedditLogin Reddit { get; set; }

        /// <summary>
        /// The user's Shopify Customer Account API sign-in (the OIDC id_token claims), null if never signed in that
        /// way. Separate from <see cref="Shopify"/>, which is the store's Admin customer link.
        /// </summary>
        [DataMember]
        public ShopifyCustomerAccountLogin ShopifyCustomerAccount { get; set; }

        /// <summary>
        /// The user's Shopify customer as the Admin REST <c>Customer</c>, null if not linked. Its
        /// <c>Id</c> is the source of truth for the Shopify customer id; the legacy
        /// <c>Meta["ShopifyCustomerId"]</c> key is kept as a back-compat mirror only.
        /// </summary>
        [DataMember]
        public global::AnointedAutomation.Shopify.Customer Shopify { get; set; }
    }
}
