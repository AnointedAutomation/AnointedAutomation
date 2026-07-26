// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-07-12
// Stewarded by Alexander Fields

namespace AnointedAutomation.Concepts.Epistemics
{
    /// <summary>
    /// The canonical theoretical proofs, built as first-class <see cref="Proof"/> objects. The
    /// symbolic strings carry the logical operators ∃, ∀, ∧, ⊕ and →. Each of these lives in the
    /// Basic Multilingual Plane, so an ordinary UTF-16 <see cref="string"/> holds it as a single
    /// code unit, with no surrogate handling and no UTF-32. The one requirement is that this source
    /// file stay UTF-8; if that is ever in doubt, replace a glyph with its \uXXXX escape
    /// (∃ = \u2203, ∀ = \u2200, ∧ = \u2227, ⊕ = \u2295, → = \u2192), which compiles to the same string.
    /// </summary>
    public static class TheoreticalProofs
    {
        /// <summary>
        /// The Agnosticism proof: there exists at least one person considering God's existence who
        /// either believes God exists or believes God does not exist, exactly one of the two. It
        /// establishes that the question of God is unavoidably live before asking who that God is.
        /// </summary>
        /// <returns>The Agnosticism proof.</returns>
        public static Proof Agnosticism()
        {
            // ∃x (G(x) ∧ (I(x) ⊕ O(x)))
            string symbolicForm = "∃x (G(x) ∧ (I(x) ⊕ O(x)))";

            System.Collections.Generic.List<ProofSymbol> glossary =
                new System.Collections.Generic.List<ProofSymbol>
                {
                    new ProofSymbol("x", "a person"),
                    new ProofSymbol("G(x)", "x is considering the question of God's existence"),
                    new ProofSymbol("I(x)", "x believes God exists"),
                    new ProofSymbol("O(x)", "x believes God does not exist"),
                    new ProofSymbol("∃", "there exists"),
                    new ProofSymbol("∧", "and"),
                    new ProofSymbol("⊕", "exclusive or (exactly one is true)"),
                };

            System.Collections.Generic.List<ProofStep> steps =
                new System.Collections.Generic.List<ProofStep>
                {
                    // ∃x (G(x) ∧ (I(x) ⊕ O(x)))
                    new ProofStep(
                        1,
                        "∃x (G(x) ∧ (I(x) ⊕ O(x)))",
                        "Someone weighing the question either believes God exists or believes God does not, exactly one of the two."),
                };

            return new Proof(
                "Agnosticism",
                "There exists at least one person considering the question of God who either believes God exists or believes God does not exist, exactly one of the two.",
                symbolicForm,
                glossary,
                steps,
                "The question of God cannot be set aside: there must be at least one such person, so the remaining question is who that God is.");
        }

        /// <summary>
        /// The Christianity proof: Jesus exists, every proposition Jesus claims is true, and Jesus
        /// claims to be God; therefore Jesus is God, and therefore Christianity.
        /// </summary>
        /// <returns>The Christianity proof.</returns>
        public static Proof Christianity()
        {
            // (∃x J(x)) ∧ (∀p (C(J,p) → T(p))) ∧ C(J,G(J))
            string symbolicForm =
                "(∃x J(x)) ∧ (∀p (C(J,p) → T(p))) ∧ C(J,G(J))";

            System.Collections.Generic.List<ProofSymbol> glossary =
                new System.Collections.Generic.List<ProofSymbol>
                {
                    new ProofSymbol("x", "a person"),
                    new ProofSymbol("J(x)", "x is Jesus"),
                    new ProofSymbol("E(x)", "x existed"),
                    new ProofSymbol("p", "a proposition"),
                    new ProofSymbol("C(J,p)", "Jesus claims p"),
                    new ProofSymbol("T(p)", "p is true"),
                    new ProofSymbol("G(J)", "Jesus is God"),
                    new ProofSymbol("∃", "there exists"),
                    new ProofSymbol("∀", "for all"),
                    new ProofSymbol("∧", "and"),
                    new ProofSymbol("→", "implies"),
                };

            System.Collections.Generic.List<ProofStep> steps =
                new System.Collections.Generic.List<ProofStep>
                {
                    // ∃x (J(x) ∧ E(x))
                    new ProofStep(1, "∃x (J(x) ∧ E(x))", "Jesus existed."),
                    // ∀p (C(J,p) → T(p))
                    new ProofStep(2, "∀p (C(J,p) → T(p))", "Every claim Jesus made was true."),
                    // C(J, G(J))
                    new ProofStep(3, "C(J, G(J))", "Jesus claimed to be God."),
                    // T(G(J))
                    new ProofStep(4, "T(G(J))", "Therefore Jesus is God."),
                    // ∀x (G(x) → Christianity)
                    new ProofStep(5, "∀x (G(x) → Christianity)", "If Jesus is God, Christianity is true."),
                    new ProofStep(6, "Christianity", "Therefore Christianity."),
                };

            return new Proof(
                "Christianity",
                "There exists a person who is Jesus, every proposition Jesus claims is true, and Jesus claims that He is God.",
                symbolicForm,
                glossary,
                steps,
                "Jesus is God, and therefore Christianity is true.");
        }
    }
}
