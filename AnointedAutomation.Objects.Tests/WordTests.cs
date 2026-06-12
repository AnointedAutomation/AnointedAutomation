// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests
{
    public class WordTests
    {
        [Fact]
        public void Speaking_CarriesADeedIntoReality_AndReturnsItsResolution()
        {
            // "Through him all things were made." (John 1:3) The Word is the medium through which an
            // agent and reality meet, not a locked gate.
            Reality reality = Reality.Revealed();
            Word word = new Word(reality);

            Resolution resolution = word.Speak(new Act("a mercy", new Compassion()), Grounding.InGod());

            Assert.Equal(1.0, resolution.Reading("Mercy"));
        }

        [Fact]
        public void Speaking_RecordsTheDeedOnReality()
        {
            // The medium does not bypass reality; what is spoken through it is written on the tablets.
            Reality reality = Reality.Revealed();
            Word word = new Word(reality);

            word.Speak(new Act("a kindness", new JustRecompense()), Grounding.InGod());

            Assert.Single(reality.Tablets.History());
        }
    }
}
