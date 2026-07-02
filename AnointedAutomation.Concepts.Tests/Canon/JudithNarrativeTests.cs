// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Concepts;

namespace AnointedAutomation.Concepts.Tests.Canon
{
    // Judith narrative oracle: the engine must return the verdict Scripture gives at each step.
    public class JudithNarrativeTests
    {
        [Fact]
        public void HolofernesRavagesTheNationsAndDemandsNebuchadnezzarBeWorshipped_IsSin()
        {
            // Holofernes marches out to "destroy all the gods of the land," that every nation
            // might call upon Nebuchadnezzar alone as god. (Judith 2:1-13; 3:8; 6:2)
            // Idolatrous, blasphemous conquest soaked in bloodshed and oppression.
            OracleHarness.Sin(OracleHarness.Witness("Holofernes ravages the nations and demands Nebuchadnezzar be worshipped as god",
                new Idolatry(), new Blasphemy(), new Bloodshed(), new Oppression(), new Pride()));
        }

        [Fact]
        public void IsraelHumblesItselfInFastingAndSackclothBeforeTheLord_IsRighteous()
        {
            // "Every man of Israel cried out to God with great fervor ... they put on sackcloth ...
            // and the Lord heard their prayers." (Judith 4:9-13)
            // A humbled, mourning, repentant people seeking the Lord.
            OracleHarness.Righteous(OracleHarness.Witness("Israel cries out to God in fasting, sackcloth, and humility",
                new Humility(), new Mourning(), new Repentance(), new Trust(), new Worship()));
        }

        [Fact]
        public void AchiorTestifiesTheTruthThatGodGuardsTheRighteous_IsRighteous()
        {
            // Achior the Ammonite tells Holofernes the truth: Israel cannot be conquered unless they
            // sin against their God, for their God protects them. (Judith 5:5-21)
            // Honest testimony to the truth at great personal risk.
            OracleHarness.Righteous(OracleHarness.Witness("Achior testifies the truth about the God of Israel",
                new TeachingTruth(), new RejoicingInTruth(), new CourageousFaith(), new Trustworthiness()));
        }

        [Fact]
        public void HolofernesScornsGodAndCastsOutAchior_IsSin()
        {
            // Holofernes rages: "Who is God except Nebuchadnezzar?" and hands Achior over to be slain. (Judith 6:1-9)
            // Blasphemous pride, oppression of the truth-teller, and threatened bloodshed.
            OracleHarness.Sin(OracleHarness.Witness("Holofernes scorns the God of Israel and casts out Achior to die",
                new Blasphemy(), new Pride(), new Oppression(), new Boasting()));
        }

        [Fact]
        public void JudithRebukesTheEldersForPuttingGodToTheTest_IsRighteous()
        {
            // "Who are you to put God to the test ... You cannot plumb the depths of the human heart." (Judith 8:11-27)
            // Judith corrects Uzziah's ultimatum, calling Israel to trust and endure under God.
            OracleHarness.Righteous(OracleHarness.Witness("Judith rebukes the elders' five-day ultimatum and calls them to trust God",
                new TeachingTruth(), new Trust(), new Endurance(), new Humility(), new CourageousFaith()));
        }

        [Fact]
        public void JudithPraysForStrengthAndDeliverance_IsRighteous()
        {
            // Judith falls down in sackcloth and ashes and prays for the Lord to crush the proud
            // by the hand of a widow. (Judith 9:1-14)
            // Humble, worshipful petition resting wholly on God's covenant power.
            OracleHarness.Righteous(OracleHarness.Grounded("Judith prays in sackcloth for God to deliver His people",
                Grounding.InGod(),
                new Worship(), new Humility(), new Trust(), new Repentance(), new CovenantFaithfulness()));
        }

        [Fact]
        public void JudithGoesIntoTheEnemyCampTrustingGod_IsRighteous()
        {
            // Judith and her maid go out through the gates toward the Assyrian camp, the elders
            // praying, "May the Lord ... grant you success." (Judith 10:6-10)
            // Self-offering courage, going alone into mortal danger to defend her people.
            OracleHarness.Righteous(OracleHarness.Witness("Judith ventures into the camp of Holofernes to save Israel",
                new CourageousFaith(), new SelfSacrifice(), new Trust(), new DefendingTheWeak()));
        }

        [Fact]
        public void HolofernesLustsAndDrinksHimselfSenselessAtTheBanquet_IsSin()
        {
            // Holofernes burns to seduce Judith and "drank a great quantity of wine, much more than
            // he had ever drunk in any one day." (Judith 12:16-20)
            // Lust, drunkenness, and self-seeking debauchery.
            OracleHarness.Sin(OracleHarness.Witness("Holofernes lusts after Judith and drinks himself into a stupor",
                new Lust(), new Drunkenness(), new SexualImmorality(), new SelfSeeking()));
        }

        [Fact]
        public void GodDeliversIsraelByJudithsHandAgainstTheOppressor_IsADivineAct()
        {
            // "The Lord has struck him down by the hand of a woman." (Judith 13:14-15; 13:18)
            // The proud oppressor is brought low and Israel is delivered. Scripture's verdict is
            // God's saving deliverance of the weak, worked through His handmaid.
            OracleHarness.DivineAct(OracleHarness.Grounded("God delivers Israel from Holofernes by Judith's hand",
                Grounding.InGod(),
                new Deliverance(), new DefendingTheWeak(), new Protection(),
                new HonoringTheVulnerable(), new JustRecompense(), new Fidelity()));
        }

        [Fact]
        public void UzziahAndThePeopleBlessGodAndPraiseJudith_IsRighteous()
        {
            // "Blessed are you, daughter, by the Most High God above all women on earth." (Judith 13:18-20; 15:9-10)
            // Israel rejoices in the truth of God's salvation and honors His faithful servant.
            OracleHarness.Righteous(OracleHarness.Witness("Uzziah and the people bless God and praise Judith for the deliverance",
                new Worship(), new RejoicingInTruth(), new Joy(), new Goodness()));
        }

        [Fact]
        public void AchiorBelievesInGodAndJoinsTheHouseOfIsrael_IsRighteous()
        {
            // Seeing all the Lord had done, "Achior believed firmly in God, and was circumcised,
            // and joined the house of Israel." (Judith 14:5-10)
            // A Gentile turns in faith and worship to the true God.
            OracleHarness.Righteous(OracleHarness.Witness("Achior believes in God and is joined to Israel",
                new CourageousFaith(), new Trust(), new Worship(), new ObedienceToGod()));
        }

        [Fact]
        public void JudithSingsHerHymnOfThanksgivingToGodTheDeliverer_IsRighteous()
        {
            // "Begin a song to my God ... the Lord is a God who crushes wars." (Judith 16:1-17)
            // A hymn of worship, joy, and grateful praise to the God who saves the lowly.
            OracleHarness.Righteous(OracleHarness.Witness("Judith leads Israel in a hymn of thanksgiving to God",
                new Worship(), new Joy(), new RejoicingInTruth(), new Humility(), new PromiseFulfilled()));
        }
    }
}
