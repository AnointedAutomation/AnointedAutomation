// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests.Canon
{
    // 2 Ezra / Ezra Sutuel narrative oracle: the engine must return the verdict Scripture gives at each step.
    // Canon: ETH (Ethiopic-only; best-effort). This is the apocalyptic Ezra (the seven visions / dialogues
    // with the angel Uriel, the lament over Zion, the eagle vision, the man from the sea, and the
    // restoration of the lost scriptures), corresponding to 2 Esdras / 4 Ezra 3-14.
    [Trait("Canon", "EthiopicNonLoadBearing")]
    public class SecondEzraNarrativeTests
    {
        [Fact]
        public void EzraMournsOverDesolateZionAndCriesToGod_IsRighteous()
        {
            // Ezra, troubled in the thirtieth year after the fall of the city, lies upon his bed and
            // pours out his trouble before the Most High over the desolation of Zion. (2 Esd 3:1-3)
            // A grieving, humble servant turning his lament toward God.
            OracleHarness.Righteous(OracleHarness.Grounded("Ezra mourns over the ruin of Zion and pours out his trouble before the Most High",
                Grounding.InGod(),
                new Mourning(), new Humility(), new Trust(), new Worship()));
        }

        [Fact]
        public void GodGivesAdamCommandmentAndOrdersCreation_IsADivineAct()
        {
            // Ezra rehearses that God made the world, formed Adam, gave him one commandment, and set
            // the order of all things in the beginning. (2 Esd 3:4-7)
            // God's creative ordering of the world and His just commandment to man.
            OracleHarness.DivineAct(OracleHarness.Grounded("God forms Adam, sets His commandment, and orders all creation",
                Grounding.InGod(),
                new CreationOrder(), new Righteousness(), new Goodness(), new Fidelity()));
        }

        [Fact]
        public void AdamTransgressesAndTheEvilHeartSpreads_IsSin()
        {
            // "For the first Adam bearing a wicked heart transgressed, and was overcome ... so be all
            // they also that are born of him." A grain of evil seed sown in Adam's heart. (2 Esd 3:21-22; 4:30)
            // The fall: disobedience and rebellion seeding corruption through the race.
            OracleHarness.Sin(OracleHarness.Witness("Adam transgresses the commandment and the evil heart spreads to all his offspring",
                new Disobedience(), new Rebellion(), new CovenantBreaking()));
        }

        [Fact]
        public void UrielTeachesEzraThatGodsWaysAreUnsearchable_IsRighteous()
        {
            // The angel Uriel answers Ezra's complaints, setting before him weights, measures, and the
            // wind, teaching that he cannot comprehend the way of the Most High. (2 Esd 4:1-21; 5:36-40)
            // A messenger of God teaching truth and calling Ezra to humility before God's wisdom.
            OracleHarness.Righteous(OracleHarness.Grounded("the angel Uriel teaches Ezra that the ways of the Most High are unsearchable",
                Grounding.InGod(),
                new TeachingTruth(), new RejoicingInTruth(), new Humility(), new Trust()));
        }

        [Fact]
        public void GodPromisesTheEndOfTheAgeAndJudgmentToCome_IsADivineAct()
        {
            // God reveals through the angel the signs of the end, that the age hastens to its close and
            // that He will judge the world and recompense every deed. (2 Esd 5:1-13; 6:18-28; 7:33-44)
            // God's faithful promise of just judgment and the ordered consummation of the age.
            OracleHarness.DivineAct(OracleHarness.Grounded("God reveals the signs of the end and His coming righteous judgment of the world",
                Grounding.InGod(),
                new JustRecompense(), new Righteousness(), new PromiseFulfilled(), new CreationOrder(), new Fidelity()));
        }

        [Fact]
        public void GodRecountsTheSixDaysOfCreation_IsADivineAct()
        {
            // In answer to Ezra, God recounts how He spread out the firmament, gathered the waters,
            // made the lights, the living creatures, and man, in the six days. (2 Esd 6:38-54)
            // The good ordering of creation declared by its Maker.
            OracleHarness.DivineAct(OracleHarness.Grounded("God recounts His ordering of the six days of creation",
                Grounding.InGod(),
                new CreationOrder(), new Goodness(), new Righteousness()));
        }

        [Fact]
        public void EzraPleadsForMercyOnSinfulHumanity_IsRighteous()
        {
            // Ezra intercedes: "let thy mercy ... be magnified toward us ... for in this thy righteousness
            // and thy goodness shall be declared, if thou be merciful unto them that have no store of good
            // works." (2 Esd 7:62-69; 8:26-36; 8:45)
            // Compassionate intercession for a guilty people, resting on God's mercy.
            OracleHarness.Righteous(OracleHarness.Grounded("Ezra pleads for God's mercy upon sinful humanity",
                Grounding.InGod(),
                new Compassion(), new Humility(), new Trust(), new Worship()));
        }

        [Fact]
        public void GodPromisesParadiseAndRestForTheRighteous_IsADivineAct()
        {
            // God shows that for the righteous a paradise is opened, the tree of life planted, rest
            // prepared, and joy laid up. (2 Esd 7:36; 7:88-99; 8:52-54)
            // God's gracious deliverance and reward kept for those who keep His ways.
            OracleHarness.DivineAct(OracleHarness.Grounded("God promises paradise, rest, and joy prepared for the righteous",
                Grounding.InGod(),
                new Deliverance(), new JustRecompense(), new Goodness(), new PromiseFulfilled(), new Joy()));
        }

        [Fact]
        public void TheWeepingWomanIsRevealedAsZionRestored_IsADivineAct()
        {
            // Ezra meets a woman mourning her only son; as he comforts her she is transfigured into a
            // built city, revealed by the angel to be Zion, whom God will establish. (2 Esd 9:38-10:57)
            // God's healing, restoring vision: mourning Zion raised up and built anew.
            OracleHarness.DivineAct(OracleHarness.Grounded("the mourning woman is transfigured into the rebuilt city of Zion",
                Grounding.InGod(),
                new RestoringLife(), new Healing(), new Compassion(), new PromiseFulfilled(), new Joy()));
        }

        [Fact]
        public void TheEagleOfTheFourthKingdomDevoursAndOppresses_IsSin()
        {
            // The eagle rising from the sea reigns over the earth with terror, its heads and wings ruling
            // with oppression, and the Most High condemns it for tyranny. (2 Esd 11:1-12:3; 11:40-43)
            // The wicked kingdom: oppression, treachery, lawlessness, and pride against the Most High.
            OracleHarness.Sin(OracleHarness.Witness("the eagle of the fourth kingdom rules the earth with oppression and tyranny",
                new Oppression(), new Pride(), new Treachery(), new Lawlessness(), new WickedScheming()));
        }

        [Fact]
        public void TheLionRebukesTheEagleAndAnnouncesJudgment_IsADivineAct()
        {
            // A lion, the Messiah whom the Most High keeps unto the end, rebukes the eagle and announces
            // that its oppression is ended and that judgment is come. (2 Esd 11:36-12:3; 12:31-34)
            // The Anointed One delivers the righteous remnant and renders just recompense to the tyrant.
            OracleHarness.DivineAct(OracleHarness.Grounded("the Messiah-lion rebukes the eagle, ends its oppression, and delivers the righteous",
                Grounding.InGod(),
                new Deliverance(), new JustRecompense(), new Righteousness(), new DefendingTheWeak(), new Protection()));
        }

        [Fact]
        public void TheManFromTheSeaDestroysTheHostileMultitude_IsADivineAct()
        {
            // The man rising from the heart of the sea (the Son of the Most High) is assailed by a
            // multitude; he burns them with a stream of fire from his mouth, then gathers a peaceable
            // company to himself. (2 Esd 13:1-13; 13:25-50)
            // God's appointed Deliverer destroys the wicked who war against Him and protects His own.
            OracleHarness.DivineAct(OracleHarness.Grounded("the man from the sea destroys the hostile multitude and gathers a peaceable people",
                Grounding.InGod(),
                new Deliverance(), new JustRecompense(), new Protection(), new Righteousness(), new DefendingTheWeak()));
        }

        [Fact]
        public void EzraIsInspiredToRestoreTheLostScriptures_IsADivineAct()
        {
            // God gives Ezra to drink a cup like fire; for forty days, with five swift scribes, the words
            // of the Most High are spoken and ninety-four books are written, restoring the lost Law. (2 Esd 14:37-48)
            // God restores His revealed truth to His people through His enabled servant.
            OracleHarness.DivineAct(OracleHarness.Grounded("God inspires Ezra to restore the lost scriptures, the Law and the secret books",
                Grounding.InGod(),
                new TeachingTruth(), new RestoringLife(), new CovenantFaithfulness(), new PromiseFulfilled(), new Goodness()));
        }
    }
}
