// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields
namespace AnointedAutomation.Concepts.Reasoning.Language
{
    /// <summary>
    /// A prompt ready to send: its text and a stable id derived from a hash of that text.
    /// </summary>
    public sealed record ComposedPrompt
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ComposedPrompt"/> record.
        /// </summary>
        /// <param name="id">The prompt id ("prompt-" plus the first 16 hex digits of the SHA-256 of the text).</param>
        /// <param name="text">The prompt text.</param>
        public ComposedPrompt(string id, string text)
        {
            Id = ReasoningGuard.NotBlank(id, nameof(id));
            Text = ReasoningGuard.NotBlank(text, nameof(text));
        }

        /// <summary>
        /// The stable prompt id; equal text always gives the same id.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// The full prompt text.
        /// </summary>
        public string Text { get; }
    }
}
