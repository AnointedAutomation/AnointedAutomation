// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

namespace AnointedAutomation.Objects.Common
{
    /// <summary>
    /// Shared success/error envelope for an operation that either succeeds or fails with a
    /// human-readable reason. Collapses the many one-off {Success, Error} response DTOs
    /// (e.g. CartSyncErrorResponse, AdsErrorResponse, ShopifyReturnActionResult) into one type.
    /// Property names are kept PascalCase; a consuming API's serialization convention decides the
    /// wire casing (value-typed Success -&gt; camelCase, reference-typed Error -&gt; PascalCase).
    /// </summary>
    public class OperationResult
    {
        /// <summary>Whether the operation succeeded.</summary>
        public bool Success { get; set; }

        /// <summary>A human-readable failure reason, or null on success.</summary>
        public string Error { get; set; }

        /// <summary>A successful result.</summary>
        public static OperationResult Ok() => new OperationResult { Success = true };

        /// <summary>A failed result carrying the reason.</summary>
        public static OperationResult Fail(string error) => new OperationResult { Success = false, Error = error };
    }
}
