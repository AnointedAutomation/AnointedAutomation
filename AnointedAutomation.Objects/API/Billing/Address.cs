// Copyright © Anointed Automation, LLC., 2024. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2024-01-16 19:19:01
// Edited by Alexander Fields https://www.alexanderfields.me 2025-07-02 11:48:25
//Stewarded by Alexander Fields

using System.Runtime.Serialization;

namespace AnointedAutomation.Objects.Billing
{
    [System.Serializable]
    /// <summary>
    /// Represents a physical address with contact information.
    /// </summary>
    public class Address
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Address"/> class.
        /// </summary>
        public Address()
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="city">!nullable</param>
        /// <param name="country">!nullable</param>
        /// <param name="name"></param>
        /// <param name="contact">!nullable</param>
        /// <param name="state">!nullable</param>
        /// <param name="street">!nullable</param>
        /// <param name="zip"></param>
        /// <summary>
        /// Initializes a new instance of the <see cref="Address"/> class with specified properties.
        /// </summary>
        /// <param name="city">The city name (required).</param>
        /// <param name="country">The country name (required).</param>
        /// <param name="name">Optional business or organization name.</param>
        /// <param name="contact">Contact information (required).</param>
        /// <param name="state">The state or province (required).</param>
        /// <param name="street">The street address (required).</param>
        /// <param name="zip">The postal code.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when required parameters are null.</exception>
        public Address(string city, string country, string name, Contact contact, string state, string street, int zip)
        {
            this.City = city ?? throw new System.ArgumentNullException(nameof(city));
            this.Country = country ?? throw new System.ArgumentNullException(nameof(country));
            this.Name = name;
            this.Contact = contact ?? throw new System.ArgumentNullException(nameof(contact));
            this.State = state ?? throw new System.ArgumentNullException(nameof(state));
            this.Street = street ?? throw new System.ArgumentNullException(nameof(street));
            this.zip = zip;
        }

        /// <summary>
        /// Do they put on for their...
        /// </summary>
        [DataMember]
        public string City
        {
            get; set;
        }

        [DataMember]
        /// <summary>
        /// Gets or sets the contact information associated with this address.
        /// </summary>
        public Contact Contact
        {
            get; set;
        }

        /// <summary>
        /// Their Country/State (not to be confused with individual states in U.S.)
        /// </summary>
        [DataMember]
        public string Country
        {
            get; set;
        }

        /// <summary>
        /// This isn't required if sending to a person
        /// </summary>
        [DataMember]
        public string Name
        {
            get; set;
        }

        /// <summary>
        /// State/Province
        /// </summary>
        [DataMember]
        public string State
        {
            get; set;
        }

        /// <summary>
        /// street address
        /// </summary>
        [DataMember]
        public string Street
        {
            get; set;
        }

        /// <summary>
        /// zip code
        /// </summary>
        [DataMember]
        public int zip
        {
            get;
        }
    }
}
