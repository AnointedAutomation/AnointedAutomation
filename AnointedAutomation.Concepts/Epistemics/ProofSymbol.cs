// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-07-12
// Stewarded by Alexander Fields

namespace AnointedAutomation.Concepts.Epistemics
{
    /// <summary>
    /// One entry in a proof's glossary: a token that appears in the symbolic form paired with
    /// what it means in plain words. Tokens may be logical operators (for example the token
    /// "∃", there exists) or predicates and variables (for example "G(x)", x is God).
    /// </summary>
    public class ProofSymbol
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProofSymbol"/> class.
        /// </summary>
        /// <param name="token">The token as it appears in the symbolic form, e.g. "∃" or "G(x)".</param>
        /// <param name="meaning">What the token means, in plain words.</param>
        public ProofSymbol(string token, string meaning)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new System.ArgumentException("A glossary entry requires a token.", nameof(token));
            }

            if (string.IsNullOrWhiteSpace(meaning))
            {
                throw new System.ArgumentException("A glossary entry requires a meaning.", nameof(meaning));
            }

            Token = token;
            Meaning = meaning;
        }

        /// <summary>
        /// The token as it appears in the symbolic form, e.g. "∃" (there exists) or "G(x)".
        /// </summary>
        public string Token
        {
            get;
        }

        /// <summary>
        /// What the token means, in plain words.
        /// </summary>
        public string Meaning
        {
            get;
        }
    }
}
