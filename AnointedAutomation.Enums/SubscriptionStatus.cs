// Copyright © Anointed Automation, LLC., 2024. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2024-01-16 19:19:01
// Edited by Alexander Fields https://www.alexanderfields.me 2025-07-02 11:48:25
//Stewarded by Alexander Fields

namespace AnointedAutomation.Enums
{
    /// <summary>
    /// Represents the status of a subscription.
    /// </summary>
    public enum SubscriptionStatus : int
    {
        /// <summary>
        /// No status specified or unknown.
        /// </summary>
        None = 0,
        /// <summary>
        /// Subscription is active and in good standing.
        /// </summary>
        Active = 1,
        /// <summary>
        /// Subscription is in a trial period.
        /// </summary>
        Trialing = 2,
        /// <summary>
        /// Payment is past due but subscription is still accessible.
        /// </summary>
        PastDue = 3,
        /// <summary>
        /// Subscription has been cancelled (may still be active until period end).
        /// </summary>
        Cancelled = 4,
        /// <summary>
        /// Subscription has been suspended due to payment issues or policy violations.
        /// </summary>
        Suspended = 5,
        /// <summary>
        /// Subscription is temporarily paused at customer request.
        /// </summary>
        Paused = 6,
        /// <summary>
        /// Subscription has expired and is no longer active.
        /// </summary>
        Expired = 7,
        /// <summary>
        /// Subscription is pending activation (e.g., awaiting payment confirmation).
        /// </summary>
        Pending = 8,
        /// <summary>
        /// Subscription was not renewed and has ended.
        /// </summary>
        NotRenewed = 9
    }
}
