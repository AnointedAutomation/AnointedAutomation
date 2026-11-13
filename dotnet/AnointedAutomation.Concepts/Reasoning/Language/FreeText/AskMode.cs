// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-13 Jesus is King ✝️
// Stewarded by Alexander Fields

namespace AnointedAutomation.Concepts.Reasoning.Language.FreeText
{
    /// <summary>
    /// How a free-text question is answered.
    /// </summary>
    public enum AskMode
    {
        /// <summary>
        /// Not chosen. Never valid on a question.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// A direct answer to a question, from Scripture.
        /// </summary>
        BibleAnswer = 1,

        /// <summary>
        /// A study of a passage, word or theme: context, original-language words and cross references.
        /// </summary>
        BibleStudy = 2,

        /// <summary>
        /// A dream weighed against Scripture (symbols as Scripture uses them; never presented as new revelation).
        /// </summary>
        DreamAnalysis = 3,
    }
}
