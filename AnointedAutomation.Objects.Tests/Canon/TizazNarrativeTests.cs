// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests.Canon
{
    // Tizaz (Te'ezaz, "The Commandment / Ordinance") narrative oracle. Tizaz is a book of the
    // Ethiopic broader New Testament (the Sinodos / apostolic church-order corpus). Its calling is
    // the ordinance of holiness: love of God and neighbor, right worship, mercy to the penitent,
    // charity to the needy, care for the vulnerable, and the rebuke of the sins that break the
    // covenant. The engine must return the verdict Scripture gives at each commandment.
    public class TizazNarrativeTests
    {
        // The first and great commandment: love the Lord your God, ordained in Tizaz.
        [Fact]
        public void TheCommandToLoveAndWorshipGod_IsRighteous()
        {
            OracleHarness.Righteous(OracleHarness.Witness("the commandment to love and worship God", new Worship(), new ObedienceToGod()));
        }

        // The second commandment, like it: love your neighbor as yourself (Tizaz).
        [Fact]
        public void TheCommandToLoveTheNeighbor_IsRighteous()
        {
            OracleHarness.Righteous(OracleHarness.Witness("love your neighbor as yourself", new Compassion(), new Kindness()));
        }

        // The ordinance to keep faith with God's covenant and obey His statutes (Tizaz).
        [Fact]
        public void KeepingTheCovenantOrdinance_IsRighteous()
        {
            OracleHarness.Righteous(OracleHarness.Witness("keeping the covenant ordinance", new CovenantFaithfulness(), new ObedienceToGod()));
        }

        // The command to receive and restore the penitent in mercy (Tizaz on repentance).
        [Fact]
        public void ReceivingThePenitentInMercy_IsRighteous()
        {
            OracleHarness.Righteous(OracleHarness.Witness("receiving the penitent in mercy", new Forgiveness(), new Repentance()));
        }

        // The command of charity: give alms and feed the hungry (Tizaz on almsgiving).
        [Fact]
        public void CharityAndAlmsgivingToTheNeedy_IsRighteous()
        {
            OracleHarness.Righteous(OracleHarness.Witness("alms to the needy and bread to the hungry", new Generosity(), new FeedingTheHungry()));
        }

        // The command to care for the orphan, the widow, and the stranger (Tizaz).
        [Fact]
        public void CaringForOrphanWidowAndStranger_IsRighteous()
        {
            OracleHarness.Righteous(OracleHarness.Witness("care for orphan, widow, and stranger", new HonoringTheVulnerable(), new Hospitality()));
        }

        // The command to clothe the naked and visit the sick and the prisoner (Tizaz on mercy).
        [Fact]
        public void TendingTheSickClothingTheNaked_IsRighteous()
        {
            OracleHarness.Righteous(OracleHarness.Witness("clothe the naked and tend the sick", new ClothingTheNaked(), new CaringForTheSick()));
        }

        // The command to walk in holiness and purity of heart before God (Tizaz on sanctification).
        [Fact]
        public void WalkingInHolinessAndPurity_IsRighteous()
        {
            OracleHarness.Righteous(OracleHarness.Witness("walk in holiness and purity of heart", new Purity(), new SelfControl()));
        }

        // The ordinance to render every man his due in justice and truth (Tizaz).
        [Fact]
        public void RenderingJusticeAndTruth_IsRighteous()
        {
            OracleHarness.Righteous(OracleHarness.Witness("render justice and speak the truth", new JustRecompense(), new TeachingTruth()));
        }

        // Tizaz forbids idolatry: turning from the Lord to serve other gods breaks the first ordinance.
        [Fact]
        public void TurningToIdols_IsSin()
        {
            OracleHarness.Sin(OracleHarness.Grounded("turning to serve idols", Grounding.InIdol("an idol"), new Idolatry(), new Disobedience()));
        }

        // Tizaz forbids bloodshed and murder against the neighbor, capital defilement of the covenant.
        [Fact]
        public void TheBloodshedOfMurder_IsSin()
        {
            Resolution r = OracleHarness.Witness("the shedding of innocent blood", new Murder(), new Bloodshed());
            OracleHarness.Sin(r);
            Assert.Equal(1.0, r.Disorder);
        }

        // Tizaz forbids theft and oppression, withholding from the laborer and the poor.
        [Fact]
        public void TheftAndOppressionOfThePoor_IsSin()
        {
            OracleHarness.Sin(OracleHarness.Witness("theft and oppression of the poor", new Theft(), new Oppression()));
        }

        // Tizaz forbids adultery and uncleanness, defiling the body and the covenant.
        [Fact]
        public void AdulteryAndUncleanness_IsSin()
        {
            OracleHarness.Sin(OracleHarness.Witness("adultery and uncleanness", new Adultery(), new Impurity()));
        }

        // Tizaz forbids false witness and slander against the brother (the lying tongue).
        [Fact]
        public void FalseWitnessAndSlander_IsSin()
        {
            OracleHarness.Sin(OracleHarness.Witness("false witness and slander against the brother", new FalseWitness(), new Slander()));
        }
    }
}
