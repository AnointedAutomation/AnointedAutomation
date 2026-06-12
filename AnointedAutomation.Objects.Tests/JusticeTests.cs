// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests
{
    public class JusticeTests
    {
        [Fact]
        public void Justice_IsNamed()
        {
            Justice justice = new Justice();

            Assert.Equal("Justice", justice.Name);
        }

        [Fact]
        public void JustAct_RendersWhatIsDue_ReadsFullyCoherent()
        {
            // "Render to all what is due them." (Romans 13:7)
            Justice justice = new Justice();
            Act act = new Act("a debt rendered", new JustRecompense());

            Assert.Equal(1.0, justice.Read(act));
        }

        [Fact]
        public void Theft_ReadsIncoherent_ToJustice()
        {
            // Taking what is not yours wrongs the neighbour (Exodus 20:15).
            Justice justice = new Justice();
            Act act = new Act("a theft", new Theft());

            Assert.Equal(0.0, justice.Read(act));
        }

        [Fact]
        public void IndifferentAct_ReadsNeutral_ToJustice()
        {
            // An act that neither renders due nor wrongs anyone sits at the neutral midpoint.
            Justice justice = new Justice();
            Act act = new Act("watching a sunset");

            Assert.Equal(0.5, justice.Read(act));
        }
    }
}
