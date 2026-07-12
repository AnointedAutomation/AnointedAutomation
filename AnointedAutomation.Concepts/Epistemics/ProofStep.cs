// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-07-12
// Stewarded by Alexander Fields

namespace AnointedAutomation.Concepts.Epistemics
{
    /// <summary>
    /// One line in a proof's derivation: its position in the sequence, the symbolic form of the
    /// line, and the plain-words reading or justification for it. Steps are ordered by
    /// <see cref="Number"/>, starting at 1.
    /// </summary>
    public class ProofStep
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProofStep"/> class.
        /// </summary>
        /// <param name="number">Position in the derivation, 1-based.</param>
        /// <param name="symbolicForm">The line in symbolic notation.</param>
        /// <param name="reading">The line read out in plain words, or its justification.</param>
        public ProofStep(int number, string symbolicForm, string reading)
        {
            if (number < 1)
            {
                throw new System.ArgumentOutOfRangeException(
                    nameof(number),
                    "A proof step is numbered from 1.");
            }

            if (string.IsNullOrWhiteSpace(symbolicForm))
            {
                throw new System.ArgumentException("A step requires a symbolic form.", nameof(symbolicForm));
            }

            if (string.IsNullOrWhiteSpace(reading))
            {
                throw new System.ArgumentException("A step requires a plain-words reading.", nameof(reading));
            }

            Number = number;
            SymbolicForm = symbolicForm;
            Reading = reading;
        }

        /// <summary>
        /// Position in the derivation, 1-based.
        /// </summary>
        public int Number
        {
            get;
        }

        /// <summary>
        /// The line in symbolic notation.
        /// </summary>
        public string SymbolicForm
        {
            get;
        }

        /// <summary>
        /// The line read out in plain words, or its justification.
        /// </summary>
        public string Reading
        {
            get;
        }
    }
}
