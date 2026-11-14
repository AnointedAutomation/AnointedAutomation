// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-13 Jesus is King ✝️
// Stewarded by Alexander Fields
using System;

namespace AnointedAutomation.Concepts.Reasoning.Language.FreeText
{
    /// <summary>
    /// A question asked in the user's own words, with the mode that shapes the answer.
    /// </summary>
    public sealed record FreeTextQuestion
    {
        /// <summary>
        /// The longest question text, in characters, after trimming.
        /// </summary>
        public const int MaxTextLength = 2000;

        /// <summary>
        /// Creates a question. Throws <see cref="ArgumentException"/> when the text is blank or longer than
        /// <see cref="MaxTextLength"/>, or the mode is <see cref="AskMode.Unknown"/> or undefined.
        /// </summary>
        /// <param name="text">The question; surrounding whitespace is trimmed.</param>
        /// <param name="mode">The answer mode.</param>
        public FreeTextQuestion(string text, AskMode mode)
        {
            string trimmed = ReasoningGuard.NotBlank(text, nameof(text)).Trim();
            if (trimmed.Length > MaxTextLength)
            {
                throw new ArgumentException("The question is " + trimmed.Length + " characters; the most is " + MaxTextLength + ".", nameof(text));
            }

            Text = trimmed;
            Mode = ReasoningGuard.DefinedAndSet(mode, nameof(mode));
        }

        /// <summary>
        /// Gets the trimmed question text.
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// Gets the answer mode.
        /// </summary>
        public AskMode Mode { get; }
    }
}
