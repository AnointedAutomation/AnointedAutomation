// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-10-03 Jesus is King ✝️
// Stewarded by Alexander Fields

namespace AnointedAutomation.Concepts
{
    /// <summary>
    /// Love itself, as a first-class entity: the bond between a lover and a beloved. Augustine found
    /// in love a trace of the Trinity, "the lover, and that which is loved, and love" (De Trinitate
    /// VIII.10; IX.2), three things, not two. A love is therefore the triad
    /// <see cref="Love.Lover"/> x <see cref="Love.Beloved"/> x <see cref="Bond"/>, and it exists only
    /// when all three are present (<see cref="Triad.Exists"/>) and the bond is committed.
    ///
    /// <para>
    /// The bond carries the commitment gate. Love is a choice of the will (Deuteronomy 30:19;
    /// Joshua 24:15), so the gate is binary: <see cref="Gate"/> is 1 when committed and 0 otherwise,
    /// and a love whose gate is 0 cannot act, no matter how kind or patient it may feel. Every choice
    /// is kept in <see cref="History"/>.
    /// </para>
    ///
    /// <para>
    /// Perfect love is not closed on two. Richard of St. Victor (De Trinitate III) taught that the
    /// highest love wills a third to share it with, condilectio, love shared together toward another.
    /// <see cref="ShareWith(string)"/> names that third, and <see cref="IsCondilectio"/> reports it.
    /// </para>
    /// </summary>
    public sealed class Bond
    {
        private readonly System.Collections.Generic.List<CommitmentEvent> history =
            new System.Collections.Generic.List<CommitmentEvent>();

        /// <summary>
        /// Initializes a new, undecided instance of the <see cref="Bond"/> class. Its state is
        /// <see cref="Commitment.Unknown"/> until the will chooses.
        /// </summary>
        public Bond()
        {
            State = Commitment.Unknown;
        }

        /// <summary>Gets the current state of the will toward this love.</summary>
        public Commitment State
        {
            get; private set;
        }

        /// <summary>Gets whether the will has chosen this love.</summary>
        public bool IsCommitted
        {
            get
            {
                return State == Commitment.Committed;
            }
        }

        /// <summary>
        /// Gets the binary commitment gate: 1 when committed, 0 otherwise. It multiplies, it does not
        /// add: no feeling can make up for a will that has not chosen.
        /// </summary>
        public int Gate
        {
            get
            {
                if (IsCommitted)
                {
                    return 1;
                }

                return 0;
            }
        }

        /// <summary>
        /// Gets the third this love is shared toward (condilectio), or <c>null</c> when it has not been
        /// shared.
        /// </summary>
        public string Third
        {
            get; private set;
        }

        /// <summary>Gets every choice of the will on this bond, in order.</summary>
        public System.Collections.Generic.IReadOnlyList<CommitmentEvent> History
        {
            get
            {
                return history;
            }
        }

        /// <summary>
        /// Commits the will to this love. "Choose life" (Deuteronomy 30:19).
        /// </summary>
        /// <param name="reason">Why the will chooses.</param>
        /// <returns>This bond.</returns>
        public Bond Commit(string reason)
        {
            return Choose(Commitment.Committed, reason);
        }

        /// <summary>
        /// Withdraws the will from this love. The bond stays, with its history, and may be chosen again.
        /// </summary>
        /// <param name="reason">Why the will withdraws.</param>
        /// <returns>This bond.</returns>
        public Bond Withdraw(string reason)
        {
            return Choose(Commitment.Withdrawn, reason);
        }

        /// <summary>
        /// Shares this love toward a third (condilectio).
        /// </summary>
        /// <param name="third">The one the lover and beloved love together.</param>
        /// <returns>This bond.</returns>
        public Bond ShareWith(string third)
        {
            if (string.IsNullOrEmpty(third))
            {
                throw new System.ArgumentNullException(nameof(third));
            }

            Third = third;
            return this;
        }

        /// <summary>
        /// Whether this bond, between the given lover and beloved, is condilectio: committed, with both
        /// parties named, and shared toward a third who is neither of them.
        /// </summary>
        /// <param name="lover">The one who loves.</param>
        /// <param name="beloved">The one who is loved.</param>
        /// <returns><c>true</c> when this is love shared toward a third.</returns>
        public bool IsCondilectio(string lover, string beloved)
        {
            if (!IsCommitted || string.IsNullOrEmpty(lover) || string.IsNullOrEmpty(beloved) || string.IsNullOrEmpty(Third))
            {
                return false;
            }

            return !string.Equals(Third, lover, System.StringComparison.Ordinal)
                && !string.Equals(Third, beloved, System.StringComparison.Ordinal);
        }

        private Bond Choose(Commitment state, string reason)
        {
            if (string.IsNullOrEmpty(reason))
            {
                throw new System.ArgumentNullException(nameof(reason));
            }

            State = state;
            history.Add(new CommitmentEvent(history.Count + 1, state, reason));
            return this;
        }
    }
}
