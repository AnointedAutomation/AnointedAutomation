// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//
// extensions.cost of a Shopify Admin GraphQL answer (requestedQueryCost and throttleStatus).

using System.Text.Json;

namespace AnointedAutomation.Shopify.GraphQL
{
    /// <summary>Parsed extensions.cost. Points are rate-limit telemetry, not money, so double is used.</summary>
    public sealed class ShopifyQueryCost
    {
        /// <summary>The query's requested cost.</summary>
        public double RequestedQueryCost { get; init; }

        /// <summary>True when throttleStatus was present with a positive restore rate.</summary>
        public bool HasThrottleStatus { get; init; }

        /// <summary>throttleStatus.currentlyAvailable.</summary>
        public double CurrentlyAvailable { get; init; }

        /// <summary>throttleStatus.maximumAvailable.</summary>
        public double MaximumAvailable { get; init; }

        /// <summary>throttleStatus.restoreRate (points per second).</summary>
        public double RestoreRate { get; init; }

        /// <summary>Read extensions.cost off a parsed root, or null when it has none.</summary>
        public static ShopifyQueryCost Read(JsonElement root)
        {
            if (root.ValueKind != JsonValueKind.Object
                || !root.TryGetProperty("extensions", out JsonElement ext) || ext.ValueKind != JsonValueKind.Object
                || !ext.TryGetProperty("cost", out JsonElement cost) || cost.ValueKind != JsonValueKind.Object)
            {
                return null;
            }
            bool hasTs = cost.TryGetProperty("throttleStatus", out JsonElement ts) && ts.ValueKind == JsonValueKind.Object;
            double restore = hasTs ? Number(ts, "restoreRate") : 0;
            return new ShopifyQueryCost
            {
                RequestedQueryCost = Number(cost, "requestedQueryCost"),
                HasThrottleStatus = hasTs && restore > 0,
                CurrentlyAvailable = hasTs ? Number(ts, "currentlyAvailable") : 0,
                MaximumAvailable = hasTs ? Number(ts, "maximumAvailable") : 0,
                RestoreRate = restore,
            };
        }

        private static double Number(JsonElement obj, string name) =>
            obj.TryGetProperty(name, out JsonElement v) && v.ValueKind == JsonValueKind.Number ? v.GetDouble() : 0;
    }
}
