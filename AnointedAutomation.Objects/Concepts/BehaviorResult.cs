// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Created by Alexander Fields https://www.alexanderfields.me on 2026-05-28 12:05:18
// Edited by Alexander Fields https://www.alexanderfields.me 2026-05-28 12:05:18
//Created by Alexander Fields

namespace AnointedAutomation.Objects.Concepts
{
    /// <summary>
    /// The outcome of evaluating a <see cref="BehaviorNode"/>: whether it succeeded, and the
    /// <see cref="LoveAction"/> it produced (if any).
    /// </summary>
    public class BehaviorResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BehaviorResult"/> class.
        /// </summary>
        public BehaviorResult()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BehaviorResult"/> class.
        /// </summary>
        /// <param name="succeeded">Whether the node succeeded.</param>
        /// <param name="action">The deed the node produced, if any.</param>
        public BehaviorResult(bool succeeded, LoveAction action)
        {
            this.succeeded = succeeded;
            Action = action;
        }

        /// <summary>
        /// Gets or sets whether the node succeeded.
        /// </summary>
        public bool succeeded
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the deed the node produced, if any.
        /// </summary>
        public LoveAction Action
        {
            get; set;
        }

        /// <summary>
        /// Creates a successful result carrying the given deed.
        /// </summary>
        /// <param name="action">The deed produced.</param>
        /// <returns>A successful <see cref="BehaviorResult"/>.</returns>
        public static BehaviorResult Succeed(LoveAction action)
        {
            return new BehaviorResult(true, action);
        }

        /// <summary>
        /// Creates a failed result.
        /// </summary>
        /// <returns>A failed <see cref="BehaviorResult"/>.</returns>
        public static BehaviorResult Fail()
        {
            return new BehaviorResult(false, null);
        }
    }
}
