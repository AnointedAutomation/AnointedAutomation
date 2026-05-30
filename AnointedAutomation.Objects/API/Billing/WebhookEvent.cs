// Copyright © Anointed Automation, LLC., 2024. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2024-01-16 19:19:01
// Edited by Alexander Fields https://www.alexanderfields.me 2025-07-02 11:48:25
//Stewarded by Alexander Fields

using System.Runtime.Serialization;
using AnointedAutomation.Enums;

namespace AnointedAutomation.Objects.Billing
{
    /// <summary>
    /// Represents a webhook event received from a payment provider.
    /// Provides a standardized structure for handling webhooks from Stripe, PayPal, Braintree, and Checkout.com.
    /// </summary>
    [System.Serializable]
    public class WebhookEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebhookEvent"/> class.
        /// </summary>
        public WebhookEvent()
        {
            this.Id = System.Guid.NewGuid().ToString();
            this.CreatedAt = System.DateTime.UtcNow;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebhookEvent"/> class with specified parameters.
        /// </summary>
        /// <param name="provider">The payment provider that sent the webhook.</param>
        /// <param name="eventType">The type of event.</param>
        /// <param name="providerEventId">The event ID from the provider.</param>
        /// <param name="providerEventType">The original event type string from the provider.</param>
        /// <param name="resourceId">The ID of the related resource (payment, customer, etc.).</param>
        /// <param name="rawPayload">The raw JSON payload from the webhook.</param>
        public WebhookEvent(PaymentProvider provider, WebhookEventType eventType, string providerEventId,
            string providerEventType, string resourceId, string rawPayload)
        {
            this.Id = System.Guid.NewGuid().ToString();
            this.Provider = provider;
            this.EventType = eventType;
            this.ProviderEventId = providerEventId;
            this.ProviderEventType = providerEventType;
            this.ResourceId = resourceId;
            this.RawPayload = rawPayload;
            this.CreatedAt = System.DateTime.UtcNow;
            this.IsProcessed = false;
        }

        /// <summary>
        /// Gets or sets the unique identifier for this webhook event.
        /// </summary>
        [DataMember]
        public string Id
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the payment provider that sent this webhook.
        /// </summary>
        [DataMember]
        public PaymentProvider Provider
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the normalized event type.
        /// </summary>
        [DataMember]
        public WebhookEventType EventType
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the original event ID from the payment provider.
        /// </summary>
        [DataMember]
        public string ProviderEventId
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the original event type string from the payment provider.
        /// For example, "payment_intent.succeeded" for Stripe or "PAYMENT.CAPTURE.COMPLETED" for PayPal.
        /// </summary>
        [DataMember]
        public string ProviderEventType
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the ID of the related resource (payment intent, customer, subscription, etc.).
        /// </summary>
        [DataMember]
        public string ResourceId
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the raw JSON payload from the webhook for custom processing.
        /// </summary>
        [DataMember]
        public string RawPayload
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the time when the webhook was created/received.
        /// </summary>
        [DataMember]
        public System.DateTime CreatedAt
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this webhook has been processed.
        /// </summary>
        [DataMember]
        public bool IsProcessed
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the time when the webhook was processed.
        /// </summary>
        [DataMember]
        public System.DateTime ProcessedAt
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets any error message if processing failed.
        /// </summary>
        [DataMember]
        public string ProcessingError
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the webhook signature for verification.
        /// </summary>
        [DataMember]
        public string Signature
        {
            get; set;
        }
    }
}
