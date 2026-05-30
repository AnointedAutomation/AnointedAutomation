// Copyright © Anointed Automation, LLC., 2024. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2024-01-16 19:19:01
// Edited by Alexander Fields https://www.alexanderfields.me 2025-07-02 11:48:25
//Stewarded by Alexander Fields

using System.Runtime.Serialization;

namespace AnointedAutomation.Objects.Billing
{
    /// <summary>
    /// Represents a status change entry in an audit trail.
    /// Used to track the history of status changes for purchases, subscriptions, etc.
    /// </summary>
    [System.Serializable]
    public class StatusHistoryEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StatusHistoryEntry"/> class.
        /// </summary>
        public StatusHistoryEntry()
        {
            this.Id = System.Guid.NewGuid().ToString();
            this.Timestamp = System.DateTime.UtcNow;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StatusHistoryEntry"/> class with specified parameters.
        /// </summary>
        /// <param name="previousStatus">The previous status value.</param>
        /// <param name="newStatus">The new status value.</param>
        /// <param name="changedBy">The user or system that made the change.</param>
        /// <param name="reason">The reason for the status change.</param>
        public StatusHistoryEntry(string previousStatus, string newStatus, string changedBy, string reason)
        {
            this.Id = System.Guid.NewGuid().ToString();
            this.PreviousStatus = previousStatus;
            this.NewStatus = newStatus;
            this.ChangedBy = changedBy;
            this.Reason = reason;
            this.Timestamp = System.DateTime.UtcNow;
        }

        /// <summary>
        /// Gets or sets the unique identifier for this history entry.
        /// </summary>
        [DataMember]
        public string Id
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the previous status before the change.
        /// </summary>
        [DataMember]
        public string PreviousStatus
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the new status after the change.
        /// </summary>
        [DataMember]
        public string NewStatus
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the identifier of who or what made the change.
        /// Can be a user ID, "system", "webhook", etc.
        /// </summary>
        [DataMember]
        public string ChangedBy
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the reason for the status change.
        /// </summary>
        [DataMember]
        public string Reason
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the timestamp when the change occurred.
        /// </summary>
        [DataMember]
        public System.DateTime Timestamp
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets additional metadata about the change as JSON string.
        /// </summary>
        [DataMember]
        public string Metadata
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the IP address from which the change was made.
        /// </summary>
        [DataMember]
        public string IpAddress
        {
            get; set;
        }
    }
}
