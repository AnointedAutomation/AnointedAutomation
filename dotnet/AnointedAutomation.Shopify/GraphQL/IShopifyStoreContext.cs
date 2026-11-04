// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//
// The store a client talks to. Tenancy, token storage and token refresh stay in the consumer: the client only asks
// for the domain, the API version and a current access token on every send.

using System.Threading;
using System.Threading.Tasks;

namespace AnointedAutomation.Shopify.GraphQL
{
    /// <summary>One Shopify store: domain, Admin API version and an access-token provider.</summary>
    public interface IShopifyStoreContext
    {
        /// <summary>The shop's myshopify domain, e.g. "example.myshopify.com". Required.</summary>
        string ShopDomain { get; }

        /// <summary>The Admin API version, e.g. "2026-04". Required.</summary>
        string ApiVersion { get; }

        /// <summary>A current Admin API access token. Called before every send, so a refreshed token is picked up.</summary>
        Task<string> GetAccessTokenAsync(CancellationToken cancellationToken);
    }
}
