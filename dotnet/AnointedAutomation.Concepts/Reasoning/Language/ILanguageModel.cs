// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields
using System.Threading;
using System.Threading.Tasks;

namespace AnointedAutomation.Concepts.Reasoning.Language
{
    /// <summary>
    /// The port the language engine talks through. The core ships no implementation and no AI SDK;
    /// an adapter package (or a test fake) supplies one.
    /// </summary>
    public interface ILanguageModel
    {
        /// <summary>
        /// The model identifier and version, recorded in each answer's provenance.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Sends one prompt and returns the model's raw reply text.
        /// </summary>
        /// <param name="prompt">The full prompt.</param>
        /// <param name="ct">Cancels the call.</param>
        /// <returns>The raw reply.</returns>
        Task<string> Complete(string prompt, CancellationToken ct);
    }
}
