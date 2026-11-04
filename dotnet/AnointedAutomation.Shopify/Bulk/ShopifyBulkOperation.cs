// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//
// The state of one Shopify bulk operation as read back from node(id:) or currentBulkOperation.

using System;
using System.Globalization;
using System.Text.Json;

namespace AnointedAutomation.Shopify.Bulk
{
    /// <summary>One bulk operation (query or mutation).</summary>
    public sealed record ShopifyBulkOperation(string Id, string Type, string Status, string ErrorCode, long? ObjectCount,
        string Url, string PartialDataUrl)
    {
        /// <summary>Status values.</summary>
        public const string Created = "CREATED", Running = "RUNNING", Canceling = "CANCELING", Completed = "COMPLETED",
            Failed = "FAILED", Canceled = "CANCELED", Expired = "EXPIRED";

        /// <summary>True while Shopify is still working (CREATED, RUNNING, CANCELING).</summary>
        public bool IsActive => Is(Created) || Is(Running) || Is(Canceling);

        /// <summary>True once the operation ended in any way.</summary>
        public bool IsTerminal => !IsActive;

        /// <summary>True when the operation COMPLETED.</summary>
        public bool IsCompleted => Is(Completed);

        private bool Is(string status) => string.Equals(Status, status, StringComparison.Ordinal);

        /// <summary>Where the result JSONL is: url when COMPLETED, else partialDataUrl (lines that ran before a failure
        /// or cancel), else url. Null when there is no file (e.g. nothing matched).</summary>
        public string ResultUrl => IsCompleted ? Url : (!string.IsNullOrEmpty(PartialDataUrl) ? PartialDataUrl : Url);

        /// <summary>Read one BulkOperation object. Returns null for a JSON null (no operation); throws when the object
        /// has no id or status.</summary>
        public static ShopifyBulkOperation Read(JsonElement op)
        {
            if (op.ValueKind == JsonValueKind.Null || op.ValueKind == JsonValueKind.Undefined)
            {
                return null;
            }
            string id = ShopifyStagedUploads.Str(op, "id");
            string status = ShopifyStagedUploads.Str(op, "status");
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(status))
            {
                throw new InvalidOperationException("The bulk operation has no id or status.");
            }
            long? count = null;
            if (op.TryGetProperty("objectCount", out JsonElement c))
            {
                if (c.ValueKind == JsonValueKind.Number && c.TryGetInt64(out long n))
                {
                    count = n;
                }
                else if (c.ValueKind == JsonValueKind.String
                    && long.TryParse(c.GetString(), NumberStyles.None, CultureInfo.InvariantCulture, out long s))
                {
                    count = s;
                }
            }
            return new ShopifyBulkOperation(id, ShopifyStagedUploads.Str(op, "type"), status, ShopifyStagedUploads.Str(op, "errorCode"),
                count, ShopifyStagedUploads.Str(op, "url"), ShopifyStagedUploads.Str(op, "partialDataUrl"));
        }
    }
}
