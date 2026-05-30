// Copyright © Anointed Automation, LLC., 2024. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2024-01-16 19:19:01
// Edited by Alexander Fields https://www.alexanderfields.me 2025-07-02 11:48:25
//Stewarded by Alexander Fields

namespace AnointedAutomation.Enums
{
    /// <summary>
    /// Defines the types of operations that can be performed with a payment processor.
    /// Used for audit logging and tracking payment processor communications.
    /// </summary>
    public enum PaymentOperation : int
    {
        /// <summary>
        /// Unknown or unspecified operation.
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// Create a new payment charge.
        /// </summary>
        CreateCharge = 1,
        /// <summary>
        /// Create a refund for a previous charge.
        /// </summary>
        CreateRefund = 2,
        /// <summary>
        /// Create a new customer in the payment system.
        /// </summary>
        CreateCustomer = 3,
        /// <summary>
        /// Retrieve customer details.
        /// </summary>
        GetCustomer = 4,
        /// <summary>
        /// Update customer information.
        /// </summary>
        UpdateCustomer = 5,
        /// <summary>
        /// Delete a customer from the payment system.
        /// </summary>
        DeleteCustomer = 6,
        /// <summary>
        /// Create a new subscription.
        /// </summary>
        CreateSubscription = 7,
        /// <summary>
        /// Update an existing subscription.
        /// </summary>
        UpdateSubscription = 8,
        /// <summary>
        /// Cancel a subscription.
        /// </summary>
        CancelSubscription = 9,
        /// <summary>
        /// Pause a subscription.
        /// </summary>
        PauseSubscription = 10,
        /// <summary>
        /// Resume a paused subscription.
        /// </summary>
        ResumeSubscription = 11,
        /// <summary>
        /// Retrieve transaction details.
        /// </summary>
        GetTransaction = 12,
        /// <summary>
        /// Verify a webhook signature.
        /// </summary>
        VerifyWebhook = 13,
        /// <summary>
        /// Handle an incoming webhook event.
        /// </summary>
        HandleWebhookEvent = 14,
        /// <summary>
        /// Create a payment intent for future payment.
        /// </summary>
        CreatePaymentIntent = 15,
        /// <summary>
        /// Confirm a payment intent.
        /// </summary>
        ConfirmPaymentIntent = 16,
        /// <summary>
        /// Cancel a payment intent.
        /// </summary>
        CancelPaymentIntent = 17,
        /// <summary>
        /// Capture a previously authorized payment.
        /// </summary>
        CapturePayment = 18,
        /// <summary>
        /// Void a previously authorized payment.
        /// </summary>
        VoidPayment = 19,
        /// <summary>
        /// Add a payment method to a customer.
        /// </summary>
        AddPaymentMethod = 20,
        /// <summary>
        /// Remove a payment method from a customer.
        /// </summary>
        RemovePaymentMethod = 21,
        /// <summary>
        /// Set the default payment method for a customer.
        /// </summary>
        SetDefaultPaymentMethod = 22,
        /// <summary>
        /// Create an invoice.
        /// </summary>
        CreateInvoice = 23,
        /// <summary>
        /// Pay an invoice.
        /// </summary>
        PayInvoice = 24,
        /// <summary>
        /// Void an invoice.
        /// </summary>
        VoidInvoice = 25
    }
}
