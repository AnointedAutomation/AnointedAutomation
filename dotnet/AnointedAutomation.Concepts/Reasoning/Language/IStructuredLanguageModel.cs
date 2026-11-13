// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-13 Jesus is King ✝️
// Stewarded by Alexander Fields
using System.Threading;
using System.Threading.Tasks;

namespace AnointedAutomation.Concepts.Reasoning.Language
{
    /// <summary>
    /// A language model that can constrain one reply to a caller-supplied JSON schema, so a prompt with a different
    /// reply shape (for example the free-text ask) still gets exactly one structured reply per call.
    /// </summary>
    public interface IStructuredLanguageModel : ILanguageModel
    {
        /// <summary>
        /// Sends one prompt and returns the raw reply text, constrained to <paramref name="replySchema"/>.
        /// </summary>
        /// <param name="prompt">The full prompt.</param>
        /// <param name="replySchema">The JSON schema the reply must match.</param>
        /// <param name="ct">Cancellation.</param>
        /// <returns>The raw reply text.</returns>
        Task<string> Complete(string prompt, string replySchema, CancellationToken ct);
    }
}
