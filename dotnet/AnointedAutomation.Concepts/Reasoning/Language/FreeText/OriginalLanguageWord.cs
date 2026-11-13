// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-13 Jesus is King ✝️
// Stewarded by Alexander Fields

namespace AnointedAutomation.Concepts.Reasoning.Language.FreeText
{
    /// <summary>
    /// One Hebrew, Aramaic or Greek word the answer reasons from.
    /// </summary>
    public sealed record OriginalLanguageWord
    {
        /// <summary>
        /// Creates a word.
        /// </summary>
        /// <param name="language">The language.</param>
        /// <param name="lemma">The word in its own script (dictionary form).</param>
        /// <param name="transliteration">The word in Latin letters.</param>
        /// <param name="gloss">Its meaning in plain English.</param>
        /// <param name="reference">A verse where it appears.</param>
        public OriginalLanguageWord(OriginalLanguage language, string lemma, string transliteration, string gloss, ScriptureReference reference)
        {
            Language = ReasoningGuard.DefinedAndSet(language, nameof(language));
            Lemma = ReasoningGuard.NotBlank(lemma, nameof(lemma));
            Transliteration = ReasoningGuard.NotBlank(transliteration, nameof(transliteration));
            Gloss = ReasoningGuard.NotBlank(gloss, nameof(gloss));
            Reference = reference;
        }

        /// <summary>
        /// Gets the language.
        /// </summary>
        public OriginalLanguage Language { get; }

        /// <summary>
        /// Gets the word in its own script.
        /// </summary>
        public string Lemma { get; }

        /// <summary>
        /// Gets the word in Latin letters.
        /// </summary>
        public string Transliteration { get; }

        /// <summary>
        /// Gets the meaning in plain English.
        /// </summary>
        public string Gloss { get; }

        /// <summary>
        /// Gets a verse where the word appears.
        /// </summary>
        public ScriptureReference Reference { get; }
    }
}
