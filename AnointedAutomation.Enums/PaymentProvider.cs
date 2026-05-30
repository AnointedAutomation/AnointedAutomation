// Copyright © Anointed Automation, LLC., 2024. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2024-01-16 19:19:01
// Edited by Alexander Fields https://www.alexanderfields.me 2025-07-02 11:48:25
//Stewarded by Alexander Fields

namespace AnointedAutomation.Enums
{
    /// <summary>
    /// Defines supported payment gateway providers.
    /// </summary>
    public enum PaymentProvider : int
    {
        /// <summary>
        /// No provider specified.
        /// </summary>
        None = 0,
        /// <summary>
        /// Stripe payment gateway.
        /// </summary>
        Stripe = 1,
        /// <summary>
        /// PayPal payment gateway.
        /// </summary>
        PayPal = 2,
        /// <summary>
        /// Braintree payment gateway (owned by PayPal).
        /// </summary>
        Braintree = 3,
        /// <summary>
        /// Checkout.com payment gateway.
        /// </summary>
        Checkout = 4,
        /// <summary>
        /// Square payment gateway.
        /// </summary>
        Square = 5,
        /// <summary>
        /// Adyen payment gateway.
        /// </summary>
        Adyen = 6,
        /// <summary>
        /// Authorize.Net payment gateway.
        /// </summary>
        AuthorizeNet = 7
    }
}
