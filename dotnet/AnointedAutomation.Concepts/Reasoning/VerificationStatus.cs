// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields

namespace AnointedAutomation.Concepts.Reasoning
{
    /// <summary>
    /// Whether an <see cref="Answer"/> has been checked against canon. Value 0 is
    /// <see cref="Unknown"/> per the repo enum convention so an unset status is detectable;
    /// <see cref="Provenance"/> rejects it.
    /// <para>
    /// Owner rule: only the owner can approve promoting an AI interpretation into a permanent fact.
    /// <see cref="Verified"/> on a <see cref="AnswerSource.Language"/> answer means it was checked
    /// against canon for this answer; it does not make the interpretation a permanent fact. No code
    /// path may persist a language-engine interpretation as canon without the owner's approval.
    /// </para>
    /// </summary>
    public enum VerificationStatus
    {
        /// <summary>
        /// Not determined. Never valid inside a <see cref="Provenance"/>.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Checked against canon and consistent with it.
        /// </summary>
        Verified = 1,

        /// <summary>
        /// Not (yet) checked against canon.
        /// </summary>
        Unverified = 2,

        /// <summary>
        /// Checked against canon and found to contradict it.
        /// </summary>
        Rejected = 3
    }
}
