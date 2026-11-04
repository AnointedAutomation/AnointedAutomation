// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//
// Hook the client awaits before each send, so a consumer can put Shopify traffic behind its own priority queue or
// rate limiter. The default runs every send immediately.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace AnointedAutomation.Shopify.GraphQL
{
    /// <summary>Decides when one send attempt runs.</summary>
    public interface IShopifyRequestScheduler
    {
        /// <summary>Run <paramref name="send"/> (one HTTP attempt for <paramref name="operation"/>) when allowed.</summary>
        Task<T> ScheduleAsync<T>(string operation, Func<CancellationToken, Task<T>> send, CancellationToken cancellationToken);
    }

    /// <summary>Runs every send immediately.</summary>
    public sealed class ImmediateShopifyRequestScheduler : IShopifyRequestScheduler
    {
        /// <summary>Shared instance.</summary>
        public static readonly ImmediateShopifyRequestScheduler Instance = new ImmediateShopifyRequestScheduler();

        /// <inheritdoc />
        public Task<T> ScheduleAsync<T>(string operation, Func<CancellationToken, Task<T>> send, CancellationToken cancellationToken)
        {
            if (send == null)
            {
                throw new ArgumentNullException(nameof(send));
            }
            return send(cancellationToken);
        }
    }
}
