// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields

namespace AnointedAutomation.Concepts.Reasoning.Hybrid
{
    /// <summary>
    /// The review state of a ledger entry.
    /// </summary>
    public enum InterpretationStatus
    {
        /// <summary>Not determined; never a valid stored status.</summary>
        Unknown = 0,

        /// <summary>Recorded from the language engine and not reviewed.</summary>
        Unverified = 1,

        /// <summary>Approved by the owner; it becomes a deterministic fact only once added to the source catalog in a reviewed PR.</summary>
        VerifiedPendingCatalog = 2,

        /// <summary>Rejected by a reviewer.</summary>
        Rejected = 3
    }
}
