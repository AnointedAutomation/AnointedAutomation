// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-10-03 Jesus is King ✝️
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Concepts;

namespace AnointedAutomation.Concepts.Tests
{
    public class TrinityTests
    {
        [Fact]
        public void Trinity_HasThreeDistinctPersons_NotModalism()
        {
            Trinity trinity = Trinity.Revealed();

            Assert.Equal(3, trinity.Persons.Count);
            Assert.True(trinity.ArePersonsDistinct());
            Assert.False(trinity.Father.IsSamePersonAs(trinity.Son));
            Assert.False(trinity.Son.IsSamePersonAs(trinity.HolySpirit));
            Assert.False(trinity.Father.IsSamePersonAs(trinity.HolySpirit));
            Assert.NotSame(trinity.Father, trinity.Son);
            Assert.True(trinity.Father.IsSamePersonAs(trinity.Father));
        }

        [Fact]
        public void Trinity_HasOneEssence_NotTritheism()
        {
            Trinity trinity = Trinity.Revealed();

            Assert.True(trinity.IsOneEssence());
            Assert.Same(trinity.Essence, trinity.Father.Essence);
            Assert.Same(trinity.Essence, trinity.Son.Essence);
            Assert.Same(trinity.Essence, trinity.HolySpirit.Essence);
            Assert.Same(trinity.Reality.Character, trinity.Essence);
        }

        [Fact]
        public void EveryPerson_HoldsEveryAttribute()
        {
            Trinity trinity = Trinity.Revealed();

            foreach (DivinePerson person in trinity.Persons)
            {
                Assert.Same(trinity.Essence.Facets, person.Attributes);
                Assert.Equal(4, person.Attributes.Count);
            }
        }

        [Fact]
        public void PersonsFromDifferentEssences_AreNotTheSameGod()
        {
            Trinity one = Trinity.Revealed();
            Trinity other = Trinity.Revealed();

            Assert.False(one.Father.IsSameEssenceAs(other.Father));
            Assert.False(one.Father.IsSamePersonAs(other.Father));
        }

        [Fact]
        public void Persons_MapToGroundWordAndPresence()
        {
            Trinity trinity = Trinity.Revealed();

            Assert.Equal(DivinePersonKind.Father, trinity.Persons[0].Kind);
            Assert.Equal(DivinePersonKind.Son, trinity.Persons[1].Kind);
            Assert.Equal(DivinePersonKind.HolySpirit, trinity.Persons[2].Kind);
            Assert.True(trinity.Ground.IsInGod);
            Assert.NotNull(trinity.Word);
            Assert.True(trinity.Presence.IsPresent);
        }

        [Fact]
        public void WorksAdExtra_AreUndivided()
        {
            Trinity trinity = Trinity.Revealed();
            Act act = new Act("a mercy", new Compassion());

            Resolution father = trinity.WorkThrough(DivinePersonKind.Father, act);
            Resolution son = trinity.WorkThrough(DivinePersonKind.Son, act);
            Resolution spirit = trinity.WorkThrough(DivinePersonKind.HolySpirit, act);

            Assert.Equal(father.Coherence, son.Coherence);
            Assert.Equal(son.Coherence, spirit.Coherence);
            Assert.Equal(father.Disorder, spirit.Disorder);
            Assert.Equal(3, trinity.Reality.Tablets.History().Count);
        }

        [Fact]
        public void PersonOf_Unknown_Throws()
        {
            Trinity trinity = Trinity.Revealed();

            Assert.Throws<System.ArgumentException>(() => trinity.PersonOf(DivinePersonKind.Unknown));
            Assert.Throws<System.ArgumentException>(() => trinity.WorkThrough(DivinePersonKind.Unknown, new Act("x", new Kindness())));
        }

        [Fact]
        public void DivinePersonKind_ZeroIsUnknown()
        {
            Assert.Equal(DivinePersonKind.Unknown, default(DivinePersonKind));
        }

        [Fact]
        public void Constructor_RejectsNullReality()
        {
            Assert.Throws<System.ArgumentNullException>(() => new Trinity(null));
        }
    }
}
