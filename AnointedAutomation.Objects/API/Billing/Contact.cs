// Copyright © Anointed Automation, LLC., 2024. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2024-01-16 19:19:01
// Edited by Alexander Fields https://www.alexanderfields.me 2025-07-02 11:48:25
//Stewarded by Alexander Fields

using System.Runtime.Serialization;

namespace AnointedAutomation.Objects.Billing
{
    [System.Serializable]
    /// <summary>
    /// Represents contact information for a person including name and contact details.
    /// </summary>
    public class Contact
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Contact"/> class.
        /// </summary>
        public Contact()
        {
        }

        /// <summary>
        /// Construct their name
        /// </summary>
        /// <param name="firstName">!nullable</param>
        /// <param name="lastName">!nullable</param>
        /// <param name="middleName"></param>
        /// <param name="number"></param>
        /// <summary>
        /// Initializes a new instance of the <see cref="Contact"/> class with specified properties.
        /// </summary>
        /// <param name="dob">Date of birth (defaults to 1900-01-01 if not specified).</param>
        /// <param name="firstName">The person's first name (required).</param>
        /// <param name="lastName">The person's last name (required).</param>
        /// <param name="middleName">The person's middle name (optional).</param>
        /// <param name="number">The person's phone number.</param>
        public Contact(System.DateTime dob, string firstName, string lastName, string middleName, ulong number)
        {
            this.dob = dob != default ? dob : new System.DateTime(1900, 1, 1);
            this.FirstName = firstName ?? "";
            this.LastName = lastName ?? "";
            this.MiddleName = middleName;
            this.number = number;
        }

        [DataMember]
        /// <summary>
        /// Gets or sets the person's date of birth.
        /// </summary>
        public System.DateTime dob
        {
            get; set;
        }

        [DataMember]
        /// <summary>
        /// Gets or sets the person's first name.
        /// </summary>
        public string FirstName
        {
            get; set;
        }

        [DataMember]
        /// <summary>
        /// Gets or sets the person's last name.
        /// </summary>
        public string LastName
        {
            get; set;
        }

        [DataMember]
        /// <summary>
        /// Gets or sets the person's middle name.
        /// </summary>
        public string MiddleName
        {
            get; set;
        }

        /// <summary>
        /// This is their phone number if they have one
        /// </summary>
        [DataMember]
        public ulong number
        {
            get; set;
        }
    }
}
