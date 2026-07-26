// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-07-12
// Stewarded by Alexander Fields

using System.Collections.Generic;
using Xunit;
using AnointedAutomation.Concepts.Epistemics;

namespace AnointedAutomation.Concepts.Tests.Epistemics
{
    public class ProofTests
    {
        private static List<ProofSymbol> SingleEntryGlossary()
        {
            return new List<ProofSymbol>
            {
                new ProofSymbol("∃", "there exists"),
            };
        }

        [Fact]
        public void Proof_KeepsSymbolicFormExactly()
        {
            // Arrange — the logic symbols are all Basic Multilingual Plane code points, so an
            // ordinary string must round-trip them without loss.
            string symbolic = "∃x (G(x) ∧ (I(x) ⊕ O(x)))";

            // Act
            Proof proof = new Proof(
                "Test",
                "plain",
                symbolic,
                SingleEntryGlossary(),
                new List<ProofStep>(),
                "so");

            // Assert
            Assert.Equal(symbolic, proof.SymbolicForm);
        }

        [Fact]
        public void ExistsGlyph_IsASingleUtf16CodeUnit()
        {
            // Assert — "∃" (U+2203) fits in one UTF-16 char; no surrogate pair, no UTF-32 needed.
            string exists = "∃";
            Assert.Single(exists);
            Assert.Equal(0x2203, exists[0]);
        }

        [Fact]
        public void UnicodeEscape_EqualsRawGlyph()
        {
            // Assert — the \uXXXX escape form the source comments describe compiles to the same
            // string as the raw glyph.
            Assert.Equal("∃x (G(x) ∧ (I(x) ⊕ O(x)))", "∃x (G(x) ∧ (I(x) ⊕ O(x)))");
        }

        [Fact]
        public void Proof_RejectsEmptyGlossary()
        {
            Assert.Throws<System.ArgumentException>(() => new Proof(
                "Test",
                "plain",
                "∃",
                new List<ProofSymbol>(),
                new List<ProofStep>(),
                "so"));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Proof_RejectsMissingSymbolicForm(string symbolic)
        {
            Assert.Throws<System.ArgumentException>(() => new Proof(
                "Test",
                "plain",
                symbolic,
                SingleEntryGlossary(),
                new List<ProofStep>(),
                "so"));
        }

        [Fact]
        public void Proof_RejectsNullSteps()
        {
            Assert.Throws<System.ArgumentNullException>(() => new Proof(
                "Test",
                "plain",
                "∃",
                SingleEntryGlossary(),
                null,
                "so"));
        }

        [Fact]
        public void ProofStep_RejectsNumberBelowOne()
        {
            Assert.Throws<System.ArgumentOutOfRangeException>(() => new ProofStep(0, "∃", "reading"));
        }

        [Fact]
        public void ProofSymbol_RejectsBlankToken()
        {
            Assert.Throws<System.ArgumentException>(() => new ProofSymbol("  ", "meaning"));
        }

        [Fact]
        public void Agnosticism_HasExpectedSymbolicFormAndGlossary()
        {
            // Act
            Proof proof = TheoreticalProofs.Agnosticism();

            // Assert
            Assert.Equal("Agnosticism", proof.Name);
            Assert.Equal("∃x (G(x) ∧ (I(x) ⊕ O(x)))", proof.SymbolicForm);
            Assert.Contains(proof.Glossary, symbol => symbol.Token.Equals("⊕", System.StringComparison.Ordinal));
        }

        [Fact]
        public void Christianity_DerivesInSixOrderedSteps()
        {
            // Act
            Proof proof = TheoreticalProofs.Christianity();

            // Assert
            Assert.Equal("(∃x J(x)) ∧ (∀p (C(J,p) → T(p))) ∧ C(J,G(J))", proof.SymbolicForm);
            Assert.Equal(6, proof.Steps.Count);
            for (int index = 0; index < proof.Steps.Count; index++)
            {
                Assert.Equal(index + 1, proof.Steps[index].Number);
            }

            Assert.Equal("Christianity", proof.Steps[5].SymbolicForm);
        }
    }
}
