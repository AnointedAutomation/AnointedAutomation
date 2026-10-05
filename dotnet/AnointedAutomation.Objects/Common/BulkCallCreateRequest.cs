// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

using System.Collections.Generic;

namespace AnointedAutomation.Objects.Common
{
    /// <summary>
    /// Shared create-job body for a durable queued bulk operation: a list of items plus an optional
    /// human comment shown in status/list views. Collapses the identical per-provider create requests
    /// (ZendropBulkCallCreateRequest, PrintfulBulkCallCreateRequest, ShopifyBulkUpdateCreateRequest)
    /// into one generic; <typeparamref name="TItem"/> is the provider-specific item type.
    /// </summary>
    /// <typeparam name="TItem">The provider-specific work item type.</typeparam>
    public class BulkCallCreateRequest<TItem>
    {
        /// <summary>The work items to enqueue.</summary>
        public List<TItem> Items { get; set; } = new List<TItem>();

        /// <summary>An optional human comment shown in the job's status and list views.</summary>
        public string Comment { get; set; }
    }
}
