// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//Stewarded by Alexander Fields

using Newtonsoft.Json;

namespace AnointedAutomation.Objects.Amazon
{
    /// <summary>
    /// The Login with Amazon customer profile
    /// (<c>https://developer.amazon.com/docs/login-with-amazon/obtain-customer-profile.html</c>). Which fields are
    /// present depends on the requested scope (<c>profile</c>, <c>profile:user_id</c>, <c>postal_code</c>); absent
    /// fields stay null. Amazon returns no email-verified flag.
    /// </summary>
    public class AmazonProfile
    {
        /// <summary>Unique identifier for the customer's Amazon account (e.g. <c>amznl.account.K2LI23KL2LK2</c>).</summary>
        [JsonProperty("user_id")]
        public string UserId { get; set; }

        /// <summary>The customer's name.</summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>The customer's email address.</summary>
        [JsonProperty("email")]
        public string Email { get; set; }

        /// <summary>The customer's postal code (<c>postal_code</c> scope).</summary>
        [JsonProperty("postal_code")]
        public string PostalCode { get; set; }
    }
}
