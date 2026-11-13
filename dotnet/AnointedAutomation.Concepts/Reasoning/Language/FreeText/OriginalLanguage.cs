// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-13 Jesus is King ✝️
// Stewarded by Alexander Fields

namespace AnointedAutomation.Concepts.Reasoning.Language.FreeText
{
    /// <summary>
    /// The original language of a cited word.
    /// </summary>
    public enum OriginalLanguage
    {
        /// <summary>
        /// Not set. Never valid on a word.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Biblical Hebrew (Old Testament).
        /// </summary>
        Hebrew = 1,

        /// <summary>
        /// Biblical Aramaic (parts of Daniel and Ezra).
        /// </summary>
        Aramaic = 2,

        /// <summary>
        /// Koine Greek (New Testament).
        /// </summary>
        Greek = 3,
    }
}
