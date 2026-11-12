// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields

using System.Threading;
using System.Threading.Tasks;

namespace AnointedAutomation.Concepts.Reasoning
{
    /// <summary>
    /// One API over every reasoning engine (deterministic canon, language model, or a hybrid of
    /// both): ask a <see cref="Query"/>, get an <see cref="Answer"/> that carries its truth value,
    /// justification and provenance.
    /// </summary>
    public interface IReasoner
    {
        /// <summary>
        /// Answers a query. An engine that cannot establish the assertion answers
        /// <see cref="TruthValue.Unknown"/>; it never answers False for lack of proof.
        /// </summary>
        /// <param name="query">The query to answer.</param>
        /// <param name="cancellationToken">Cancels the reasoning.</param>
        /// <returns>The answer.</returns>
        Task<Answer> Ask(Query query, CancellationToken cancellationToken);
    }
}
