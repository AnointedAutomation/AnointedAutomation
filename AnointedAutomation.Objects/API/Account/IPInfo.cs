// Copyright © Anointed Automation, LLC., 2024. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2024-01-16 19:19:01
// Edited by Alexander Fields https://www.alexanderfields.me 2025-07-02 11:48:25
//Stewarded by Alexander Fields

using System.Runtime.Serialization;

namespace AnointedAutomation.Objects.Account
{
    /// <summary>
    /// Stores information about IP address login history and activity.
    /// </summary>
    public class IPInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IPInfo"/> class.
        /// </summary>
        public IPInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IPInfo"/> class with specified properties.
        /// </summary>
        /// <param name="firstLogin">Date of first login from this IP.</param>
        /// <param name="ipAddress">The IP address.</param>
        /// <param name="lastLogin">Date of last login from this IP.</param>
        /// <param name="loginCount">Number of times logged in from this IP.</param>
        /// <param name="timeOnline">Total time spent online from this IP.</param>
        public IPInfo(System.DateTime firstLogin, string ipAddress, System.DateTime lastLogin, uint loginCount, System.TimeSpan timeOnline)
        {
            this.firstLogin = firstLogin;
            this.IpAddress = ipAddress;
            this.lastLogin = lastLogin;
            this.loginCount = loginCount;
            this.timeOnline = default;
        }

        [DataMember]
        /// <summary>
        /// Gets or sets the date of first login from this IP.
        /// </summary>
        public System.DateTime firstLogin
        {
            get; set;
        }

        [DataMember]
        /// <summary>
        /// Gets or sets the IP address.
        /// </summary>
        public string IpAddress
        {
            get; set;
        }

        [DataMember]
        /// <summary>
        /// Gets or sets the date of last login from this IP.
        /// </summary>
        public System.DateTime lastLogin
        {
            get; set;
        }

        /// <summary>
        /// amount of times the ip as logged in
        /// </summary>
        [DataMember]
        public ulong loginCount
        {
            get; set;
        }

        [DataMember]
        /// <summary>
        /// Gets or sets the total time spent online from this IP.
        /// </summary>
        public System.TimeSpan timeOnline
        {
            get; set;
        }
    }
}
