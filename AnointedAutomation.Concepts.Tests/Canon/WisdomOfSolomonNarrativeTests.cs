// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Concepts;

namespace AnointedAutomation.Concepts.Tests.Canon
{
    // Wisdom of Solomon narrative oracle: the engine must return the verdict Scripture gives at each step.
    public class WisdomOfSolomonNarrativeTests
    {
        [Fact]
        public void LoveRighteousnessAndSeekTheLordInSinglenessOfHeart_IsRighteous()
        {
            // "Love righteousness, you rulers of the earth ... seek him in singleness of heart." (Wisdom 1:1-2)
            // The book opens calling the powerful to honest, trusting pursuit of God.
            OracleHarness.Righteous(OracleHarness.Witness("The rulers of the earth are called to love righteousness and seek the Lord in singleness of heart",
                new Righteousness(), new Trust(), new RejoicingInTruth(), new Purity(), new ObedienceToGod()));
        }

        [Fact]
        public void TheUngodlyOppressThePoorManAndCrushTheWidow_IsSin()
        {
            // "Let us oppress the righteous poor man ... let not the widow's costly garment escape us." (Wisdom 2:10)
            // The ungodly reason themselves into preying on the weak as their measure of strength.
            OracleHarness.Sin(OracleHarness.Witness("The ungodly resolve to oppress the poor man and crush the widow and the aged",
                new Oppression(), new Greed(), new Heartlessness(), new Wrongdoing()));
        }

        [Fact]
        public void TheUngodlyPlotToCondemnTheRighteousManToAShamefulDeath_IsSin()
        {
            // "Let us lie in wait for the righteous man ... let us condemn him to a shameful death." (Wisdom 2:12-20)
            // They hate the righteous one for reproving them and conspire to murder him by torture.
            OracleHarness.Sin(OracleHarness.Witness("The ungodly lie in wait to condemn the righteous man to a shameful death",
                new Murder(), new WickedScheming(), new Hatred(), new Bloodshed(), new Condemnation()));
            Assert.Equal(1.0, OracleHarness.Witness("The ungodly lie in wait to condemn the righteous man to a shameful death",
                new Murder(), new WickedScheming(), new Hatred(), new Bloodshed(), new Condemnation()).Disorder);
        }

        [Fact]
        public void TheSoulsOfTheRighteousAreInTheHandOfGod_IsADivineAct()
        {
            // "The souls of the righteous are in the hand of God, and no torment will ever touch them." (Wisdom 3:1-9)
            // God Himself guards, tests as gold, and grants peace and immortality to the faithful.
            OracleHarness.DivineAct(OracleHarness.Grounded("God holds the souls of the righteous in His hand, beyond the reach of torment",
                Grounding.InGod(),
                new Protection(), new Peace(), new Fidelity(), new RestoringLife(), new JustRecompense()));
        }

        [Fact]
        public void SolomonLovesWisdomAndPraysToReceiveHerFromGod_IsRighteous()
        {
            // "I prayed, and understanding was given me; I called upon God, and the spirit of wisdom came to me." (Wisdom 7:7; 8:21)
            // Solomon counts wisdom above thrones and wealth and humbly asks God for her.
            OracleHarness.Righteous(OracleHarness.Grounded("Solomon prays to God and receives the spirit of wisdom, prizing her above power and riches",
                Grounding.InGod(),
                new Humility(), new Worship(), new Trust(), new HungerForRighteousness(), new ObedienceToGod()));
        }

        [Fact]
        public void WisdomDeliversTheRighteousThroughHistoryFromAdamToTheExodus_IsADivineAct()
        {
            // "Wisdom protected the first-formed father ... she rescued a righteous man ... she did not abandon
            //  the righteous man when he was sold." (Wisdom 10:1-21) God's wisdom guards Adam, Noah, Abraham,
            //  Lot, Jacob, and Joseph, and brings Israel through the sea.
            OracleHarness.DivineAct(OracleHarness.Grounded("God's wisdom protects and delivers the righteous from Adam down to the Exodus",
                Grounding.InGod(),
                new Protection(), new Deliverance(), new Righteousness(), new Fidelity(), new CovenantFaithfulness()));
        }

        [Fact]
        public void GodIsMercifulToAllAndOverlooksSinsThatMenMayRepent_IsADivineAct()
        {
            // "You are merciful to all ... you overlook men's sins, that they may repent ... you spare all things,
            //  for they are yours, O Lord who loves the living." (Wisdom 11:23-26; 12:16-19)
            // God's sovereign power is shown in mercy, patience, and the call to repentance.
            OracleHarness.DivineAct(OracleHarness.Grounded("God in His mercy spares all and overlooks sins so that men may repent",
                Grounding.InGod(),
                new Compassion(), new Patience(), new Forgiveness(), new Goodness(), new Pardon()));
        }

        [Fact]
        public void MenWorshipTheCreaturesOfTheWorldInsteadOfTheirMaker_IsSin()
        {
            // "They supposed that either fire or wind ... or the luminaries of heaven were the gods that rule the world."
            //  (Wisdom 13:1-9) Vain men admire the works and miss the Artisan, exchanging the Creator for creation.
            OracleHarness.Sin(OracleHarness.Grounded("Men worship fire, wind, and the stars as gods instead of their Maker",
                Grounding.InIdol("the elements and luminaries of the world"),
                new Idolatry(), new GravenImage(), new Ingratitude(), new Deceit()));
        }

        [Fact]
        public void TheIdolMakerPraysForLifeToALifelessThingHisHandsHaveShaped_IsSin()
        {
            // "He prays for ... health he calls upon a thing that is weak; for life he prays to a thing that is dead."
            //  (Wisdom 13:10-19; 15:7-17) The craftsman shapes a dead idol and bows to the work of his own hands.
            OracleHarness.Sin(OracleHarness.Grounded("The craftsman shapes a lifeless idol and prays to the dead work of his own hands",
                Grounding.InIdol("an idol carved by the craftsman's hands"),
                new Idolatry(), new GravenImage(), new Blasphemy(), new Deceit()));
        }

        [Fact]
        public void IdolatryDescendsIntoChildSacrificeAndEveryDefilement_IsSin()
        {
            // "For ... they either kill their children in their initiations ... the worship of idols ... is the
            //  beginning and cause and end of every evil." (Wisdom 14:23-27) Idolatry breeds child-murder,
            //  defilement, and lawlessness.
            OracleHarness.Sin(OracleHarness.Grounded("Idol worshippers slaughter their children in secret rites and plunge into every defilement",
                Grounding.InIdol("idols of the heathen"),
                new ChildSacrifice(), new Idolatry(), new Defilement(), new Bloodshed(), new SexualImmorality()));
            Assert.Equal(1.0, OracleHarness.Grounded("Idol worshippers slaughter their children in secret rites and plunge into every defilement",
                Grounding.InIdol("idols of the heathen"),
                new ChildSacrifice(), new Idolatry(), new Defilement(), new Bloodshed(), new SexualImmorality()).Disorder);
        }

        [Fact]
        public void GodFeedsHisPeopleWithBreadFromHeavenSuitedToEveryTaste_IsADivineAct()
        {
            // "You gave your people food of angels ... bread ready to eat ... suited to every taste." (Wisdom 16:20-26)
            // While Egypt is plagued, God nourishes Israel with manna from heaven by His sustaining word.
            OracleHarness.DivineAct(OracleHarness.Grounded("God feeds Israel in the wilderness with bread from heaven suited to every taste",
                Grounding.InGod(),
                new FeedingTheHungry(), new Goodness(), new Kindness(), new Fidelity(), new Protection()));
        }

        [Fact]
        public void GodSavesByTheBronzeSerpentAsASymbolOfDeliverance_IsADivineAct()
        {
            // "He who turned toward it was saved, not by what he saw, but by you, the Savior of all." (Wisdom 16:5-7)
            // When the serpents bit Israel, the one who looked was healed, for God Himself is their deliverance.
            OracleHarness.DivineAct(OracleHarness.Grounded("God heals the serpent-bitten who look to the bronze sign, saving them as the Deliverer of all",
                Grounding.InGod(),
                new Healing(), new Deliverance(), new RestoringLife(), new Compassion(), new Protection()));
        }

        [Fact]
        public void GodLeadsIsraelThroughTheRedSeaWhileTheEgyptiansAreDrowned_IsADivineAct()
        {
            // "They saw a marvelous crossing ... grass sprang up in the deep ... the whole creation was fashioned anew."
            //  (Wisdom 19:1-9) God opens the sea as a saving path for His covenant people at the Exodus.
            OracleHarness.DivineAct(OracleHarness.Grounded("God brings Israel safely through the Red Sea as on dry land and delivers them from Egypt",
                Grounding.InGod(),
                new Deliverance(), new Protection(), new CovenantFaithfulness(), new CreationOrder(), new PromiseFulfilled()));
        }
    }
}
