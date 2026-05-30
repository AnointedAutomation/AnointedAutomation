// Copyright © Anointed Automation, LLC., 2024. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2024-01-16 19:19:01
// Edited by Alexander Fields https://www.alexanderfields.me 2025-07-02 11:48:25
//Stewarded by Alexander Fields

using System.Runtime.Serialization;

namespace AnointedAutomation.Objects.Billing
{
    /// <summary>
    /// Represents payee information for payment processing.
    /// </summary>
    public class PayeeInfo
    {
        [DataMember(Name = "client_id")]
        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        public string ClientId;

        [DataMember(Name = "email_address")]
        /// <summary>
        /// Gets or sets the payee's email address.
        /// </summary>
        public string Email;

        [DataMember(Name = "merchant_id")]
        /// <summary>
        /// Gets or sets the merchant identifier.
        /// </summary>
        public string MerchantId;

        /// <summary>
        /// Initializes a new instance of the <see cref="PayeeInfo"/> class.
        /// </summary>
        public PayeeInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PayeeInfo"/> class with specified properties.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="email">The payee's email address.</param>
        /// <param name="merchantId">The merchant identifier.</param>
        public PayeeInfo(string clientId, string email, string merchantId)
        {
            ClientId = clientId;
            Email = email;
            MerchantId = merchantId;
        }
    }
}
