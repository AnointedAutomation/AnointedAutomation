// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests
{
    public class ResolutionTests
    {
        [Fact]
        public void Coherence_IsClampedIntoUnitRange()
        {
            // Reality cannot be more than fully coherent, nor less than fully incoherent.
            Resolution tooHigh = new Resolution(1.5, 0.0);
            Resolution tooLow = new Resolution(-0.5, 0.0);

            Assert.Equal(1.0, tooHigh.Coherence);
            Assert.Equal(0.0, tooLow.Coherence);
        }

        [Fact]
        public void Disorder_IsClampedIntoUnitRange()
        {
            // An act cannot introduce more than total disorder, nor negative disorder.
            Resolution tooHigh = new Resolution(0.0, 1.5);
            Resolution tooLow = new Resolution(1.0, -0.5);

            Assert.Equal(1.0, tooHigh.Disorder);
            Assert.Equal(0.0, tooLow.Disorder);
        }

        [Fact]
        public void Resolution_ExposesTheValuesItWitnessed()
        {
            // A faithful act: highly coherent, introducing no disorder.
            Resolution faithful = new Resolution(0.9, 0.0);

            Assert.Equal(0.9, faithful.Coherence);
            Assert.Equal(0.0, faithful.Disorder);
        }
    }
}
