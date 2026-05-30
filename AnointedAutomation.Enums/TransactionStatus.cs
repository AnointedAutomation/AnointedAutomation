// Copyright © Anointed Automation, LLC., 2024. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2024-01-16 19:19:01
// Edited by Alexander Fields https://www.alexanderfields.me 2025-07-02 11:48:25
//Stewarded by Alexander Fields

namespace AnointedAutomation.Enums
{
    /// <summary>
    /// Represents the status of a payment transaction across various payment providers.
    /// Common statuses used by Stripe, PayPal, Braintree, and Checkout.com.
    /// </summary>
    public enum TransactionStatus : int
    {
        /// <summary>
        /// No status specified or unknown.
        /// </summary>
        None = 0,
        /// <summary>
        /// Transaction is pending and awaiting processing.
        /// </summary>
        Pending = 1,
        /// <summary>
        /// Transaction is currently being processed.
        /// </summary>
        Processing = 2,
        /// <summary>
        /// Transaction requires additional action (e.g., 3D Secure authentication).
        /// </summary>
        RequiresAction = 3,
        /// <summary>
        /// Transaction requires payment method to be attached.
        /// </summary>
        RequiresPaymentMethod = 4,
        /// <summary>
        /// Transaction requires confirmation before processing.
        /// </summary>
        RequiresConfirmation = 5,
        /// <summary>
        /// Transaction requires capture after authorization.
        /// </summary>
        RequiresCapture = 6,
        /// <summary>
        /// Transaction completed successfully.
        /// </summary>
        Succeeded = 7,
        /// <summary>
        /// Transaction failed to process.
        /// </summary>
        Failed = 8,
        /// <summary>
        /// Transaction was canceled before completion.
        /// </summary>
        Canceled = 9,
        /// <summary>
        /// Transaction amount was refunded fully.
        /// </summary>
        Refunded = 10,
        /// <summary>
        /// Transaction amount was partially refunded.
        /// </summary>
        PartiallyRefunded = 11,
        /// <summary>
        /// Transaction is disputed by the customer.
        /// </summary>
        Disputed = 12,
        /// <summary>
        /// Transaction expired before completion.
        /// </summary>
        Expired = 13,
        /// <summary>
        /// Transaction is authorized but not yet captured.
        /// </summary>
        Authorized = 14,
        /// <summary>
        /// Transaction has been voided.
        /// </summary>
        Voided = 15
    }
}
