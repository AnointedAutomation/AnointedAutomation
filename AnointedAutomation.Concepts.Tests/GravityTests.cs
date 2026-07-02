// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Concepts;

namespace AnointedAutomation.Concepts.Tests
{
    public class GravityTests
    {
        [Fact]
        public void AVirtue_HasNoGravity()
        {
            // A virtue introduces no destruction; it has no gravity of disorder.
            Assert.Equal(Gravity.None, new Compassion().Gravity);
        }

        [Fact]
        public void Murder_IsCapital()
        {
            // "Whoever sheds human blood, by humans shall their blood be shed." (Genesis 9:6)
            Assert.Equal(Gravity.Capital, new Murder().Gravity);
        }

        [Fact]
        public void Bloodshed_IsCapital()
        {
            // The shedding of innocent blood (Deuteronomy 19; Proverbs 6:17).
            Assert.Equal(Gravity.Capital, new Bloodshed().Gravity);
        }

        [Fact]
        public void ChildSacrifice_IsCapital()
        {
            // "Anyone who sacrifices a child to Molek is to be put to death." (Leviticus 20:2)
            Assert.Equal(Gravity.Capital, new ChildSacrifice().Gravity);
        }

        [Fact]
        public void Defilement_IsCapital()
        {
            // The violation of a person is equated with murder: "this case is like that of someone
            // who attacks and murders a neighbor." (Deuteronomy 22:25-27)
            Assert.Equal(Gravity.Capital, new Defilement().Gravity);
        }

        [Fact]
        public void Theft_IsSerious_NotCapital()
        {
            // Theft is grave but answered by restitution, not death (Exodus 22:1-4).
            Assert.Equal(Gravity.Serious, new Theft().Gravity);
        }

        [Fact]
        public void Rudeness_IsMinor()
        {
            // A wrong, but not heinous: "Love ... does not dishonor others." (1 Corinthians 13:5)
            Assert.Equal(Gravity.Minor, new Rudeness().Gravity);
        }

        [Fact]
        public void GravityIncreasesInWeight_FromMinorToCapital()
        {
            // The ordering is meaningful: heavier wrongs unleash more destruction.
            Assert.True(Gravity.Minor < Gravity.Serious);
            Assert.True(Gravity.Serious < Gravity.Grave);
            Assert.True(Gravity.Grave < Gravity.Capital);
        }
    }
}
