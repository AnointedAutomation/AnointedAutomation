// Copyright © Anointed Automation, LLC., 2024. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2024-01-16 19:19:01
// Edited by Alexander Fields https://www.alexanderfields.me 2025-07-02 11:48:25
//Stewarded by Alexander Fields

using AnointedAutomation.Enums;

namespace AnointedAutomation.Objects.Billing
{
    /// <summary>
    /// Represents payment credentials with various payment methods.
    /// </summary>
    public class PaymentCredentials
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentCredentials"/> class.
        /// </summary>
        public PaymentCredentials()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentCredentials"/> class with a specified payment type.
        /// </summary>
        /// <param name="paymentType">The type of payment.</param>
        public PaymentCredentials(PaymentType paymentType)
        {
            this.paymentType = paymentType;
        }

        /// <summary>
        /// Gets or sets the credit card payment information.
        /// </summary>
        public CreditCard CreditCard
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the payee information for other payment methods.
        /// </summary>
        public PayeeInfo PayeeInfo
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the type of payment method being used.
        /// </summary>
        public PaymentType paymentType
        {
            get; set;
        }
    }
}
