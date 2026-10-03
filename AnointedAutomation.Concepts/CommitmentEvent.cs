// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-10-03 Jesus is King ✝️
// Stewarded by Alexander Fields

namespace AnointedAutomation.Concepts
{
    /// <summary>
    /// One recorded choice of the will on a <see cref="Bond"/>, so a love's commitment is auditable:
    /// every commit and withdrawal is kept in order, nothing is silently changed. "Let your 'Yes' be
    /// 'Yes,' and your 'No,' 'No.'" (Matthew 5:37).
    /// </summary>
    public sealed class CommitmentEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommitmentEvent"/> class.
        /// </summary>
        /// <param name="sequence">The position of this choice in the bond's history, starting at 1.</param>
        /// <param name="state">The state the will chose.</param>
        /// <param name="reason">Why, in the chooser's words or Scripture.</param>
        public CommitmentEvent(int sequence, Commitment state, string reason)
        {
            Sequence = sequence;
            State = state;
            Reason = reason;
        }

        /// <summary>Gets the position of this choice in the bond's history, starting at 1.</summary>
        public int Sequence
        {
            get;
        }

        /// <summary>Gets the state the will chose.</summary>
        public Commitment State
        {
            get;
        }

        /// <summary>Gets why the choice was made.</summary>
        public string Reason
        {
            get;
        }
    }
}
