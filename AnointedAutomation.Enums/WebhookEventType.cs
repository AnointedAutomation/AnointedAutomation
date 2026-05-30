// Copyright © Anointed Automation, LLC., 2024. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2024-01-16 19:19:01
// Edited by Alexander Fields https://www.alexanderfields.me 2025-07-02 11:48:25
//Stewarded by Alexander Fields

namespace AnointedAutomation.Enums
{
    /// <summary>
    /// Common webhook event types across payment providers.
    /// Normalized event names for Stripe, PayPal, Braintree, and Checkout.com webhooks.
    /// </summary>
    public enum WebhookEventType : int
    {
        /// <summary>
        /// Unknown or unrecognized event type.
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// Payment intent or order created.
        /// </summary>
        PaymentCreated = 1,
        /// <summary>
        /// Payment succeeded/completed.
        /// </summary>
        PaymentSucceeded = 2,
        /// <summary>
        /// Payment failed.
        /// </summary>
        PaymentFailed = 3,
        /// <summary>
        /// Payment was canceled.
        /// </summary>
        PaymentCanceled = 4,
        /// <summary>
        /// Refund was created.
        /// </summary>
        RefundCreated = 5,
        /// <summary>
        /// Refund succeeded.
        /// </summary>
        RefundSucceeded = 6,
        /// <summary>
        /// Refund failed.
        /// </summary>
        RefundFailed = 7,
        /// <summary>
        /// Dispute/chargeback opened.
        /// </summary>
        DisputeCreated = 8,
        /// <summary>
        /// Dispute was updated.
        /// </summary>
        DisputeUpdated = 9,
        /// <summary>
        /// Dispute was closed/resolved.
        /// </summary>
        DisputeClosed = 10,
        /// <summary>
        /// Customer was created.
        /// </summary>
        CustomerCreated = 11,
        /// <summary>
        /// Customer was updated.
        /// </summary>
        CustomerUpdated = 12,
        /// <summary>
        /// Customer was deleted.
        /// </summary>
        CustomerDeleted = 13,
        /// <summary>
        /// Subscription was created.
        /// </summary>
        SubscriptionCreated = 14,
        /// <summary>
        /// Subscription was updated.
        /// </summary>
        SubscriptionUpdated = 15,
        /// <summary>
        /// Subscription was canceled.
        /// </summary>
        SubscriptionCanceled = 16,
        /// <summary>
        /// Subscription trial will end soon.
        /// </summary>
        SubscriptionTrialEnding = 17,
        /// <summary>
        /// Invoice was created.
        /// </summary>
        InvoiceCreated = 18,
        /// <summary>
        /// Invoice payment succeeded.
        /// </summary>
        InvoicePaid = 19,
        /// <summary>
        /// Invoice payment failed.
        /// </summary>
        InvoicePaymentFailed = 20,
        /// <summary>
        /// Payment method was attached to customer.
        /// </summary>
        PaymentMethodAttached = 21,
        /// <summary>
        /// Payment method was detached from customer.
        /// </summary>
        PaymentMethodDetached = 22,
        /// <summary>
        /// Payout was created.
        /// </summary>
        PayoutCreated = 23,
        /// <summary>
        /// Payout was completed/paid.
        /// </summary>
        PayoutPaid = 24,
        /// <summary>
        /// Payout failed.
        /// </summary>
        PayoutFailed = 25
    }
}
