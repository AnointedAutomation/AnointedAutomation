// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-07-12
// Stewarded by Alexander Fields

namespace AnointedAutomation.Concepts.Epistemics
{
    /// <summary>
    /// A theoretical proof stated in first-order logic: a name, its plain-language claim, its
    /// symbolic form, a glossary that decodes every token, an optional ordered derivation, and a
    /// conclusion. The symbolic form and glossary carry logical operators such as ∃, ∀, ∧, ⊕ and →.
    /// All of these live in the Basic Multilingual Plane, so an ordinary UTF-16 <see cref="string"/>
    /// holds them with no surrogate handling and no UTF-32; the only requirement is that the source
    /// file be saved as UTF-8.
    /// </summary>
    public class Proof
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Proof"/> class.
        /// </summary>
        /// <param name="name">Short name of the proof, e.g. "Agnosticism".</param>
        /// <param name="plainStatement">The claim in plain words.</param>
        /// <param name="symbolicForm">The claim in first-order logic notation.</param>
        /// <param name="glossary">Decodes every token in the symbolic form.</param>
        /// <param name="steps">The ordered derivation; may be empty for a bare assertion.</param>
        /// <param name="conclusion">What the proof concludes, in plain words.</param>
        public Proof(
            string name,
            string plainStatement,
            string symbolicForm,
            System.Collections.Generic.IEnumerable<ProofSymbol> glossary,
            System.Collections.Generic.IEnumerable<ProofStep> steps,
            string conclusion)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new System.ArgumentException("A proof requires a name.", nameof(name));
            }

            if (string.IsNullOrWhiteSpace(plainStatement))
            {
                throw new System.ArgumentException("A proof requires a plain statement.", nameof(plainStatement));
            }

            if (string.IsNullOrWhiteSpace(symbolicForm))
            {
                throw new System.ArgumentException("A proof requires a symbolic form.", nameof(symbolicForm));
            }

            if (string.IsNullOrWhiteSpace(conclusion))
            {
                throw new System.ArgumentException("A proof requires a conclusion.", nameof(conclusion));
            }

            if (glossary == null)
            {
                throw new System.ArgumentNullException(nameof(glossary));
            }

            if (steps == null)
            {
                throw new System.ArgumentNullException(nameof(steps));
            }

            System.Collections.Generic.List<ProofSymbol> glossaryList =
                new System.Collections.Generic.List<ProofSymbol>(glossary);
            System.Collections.Generic.List<ProofStep> stepList =
                new System.Collections.Generic.List<ProofStep>(steps);

            if (glossaryList.Count == 0)
            {
                throw new System.ArgumentException(
                    "A proof must decode at least one token in its glossary.",
                    nameof(glossary));
            }

            Name = name;
            PlainStatement = plainStatement;
            SymbolicForm = symbolicForm;
            Glossary = glossaryList;
            Steps = stepList;
            Conclusion = conclusion;
        }

        /// <summary>
        /// Short name of the proof, e.g. "Agnosticism".
        /// </summary>
        public string Name
        {
            get;
        }

        /// <summary>
        /// The claim in plain words.
        /// </summary>
        public string PlainStatement
        {
            get;
        }

        /// <summary>
        /// The claim in first-order logic notation.
        /// </summary>
        public string SymbolicForm
        {
            get;
        }

        /// <summary>
        /// Decodes every token in the symbolic form.
        /// </summary>
        public System.Collections.Generic.IReadOnlyList<ProofSymbol> Glossary
        {
            get;
        }

        /// <summary>
        /// The ordered derivation; empty for a bare assertion.
        /// </summary>
        public System.Collections.Generic.IReadOnlyList<ProofStep> Steps
        {
            get;
        }

        /// <summary>
        /// What the proof concludes, in plain words.
        /// </summary>
        public string Conclusion
        {
            get;
        }
    }
}
