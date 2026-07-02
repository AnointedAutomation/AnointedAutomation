// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Concepts;

namespace AnointedAutomation.Concepts.Tests.Canon
{
    // Daniel narrative oracle (Eastern-Orthodox, including the canonical Greek additions: Song of the Three,
    // Susanna, and Bel and the Dragon): the engine must return the verdict Scripture gives at each step.
    public class DanielNarrativeTests
    {
        [Fact]
        public void DanielResolvesNotToDefileHimselfWithTheKingsFood_IsRighteous()
        {
            // Daniel purposes in his heart not to defile himself with the king's meat and wine (Daniel 1:8-16).
            OracleHarness.Righteous(OracleHarness.Witness("Daniel refuses the king's defiling food to keep faithful",
                new SelfControl(), new Fidelity(), new ObedienceToGod()));
        }

        [Fact]
        public void GodRevealsNebuchadnezzarsDreamToDaniel_IsADivineAct()
        {
            // The mystery is revealed to Daniel in a night vision; God alone reveals deep and secret things (Daniel 2:19-23, 27-28).
            OracleHarness.DivineAct(OracleHarness.Grounded("God reveals the king's dream and its meaning to Daniel",
                Grounding.InGod(), new TeachingTruth(), new RejoicingInTruth(), new Goodness()));
        }

        [Fact]
        public void DanielGivesThanksAndAscribesWisdomToGod_IsRighteous()
        {
            // Daniel blesses the God of heaven, ascribing the wisdom and the revelation to Him, not himself (Daniel 2:20-23).
            OracleHarness.Righteous(OracleHarness.Witness("Daniel blesses God and gives Him the glory for the revelation",
                new Worship(), new Humility(), new Trust()));
        }

        [Fact]
        public void TheThreeHebrewsRefuseToWorshipTheGoldenImage_IsRighteous()
        {
            // Shadrach, Meshach, and Abednego will not bow to the golden image, choosing the furnace over idolatry (Daniel 3:13-18).
            OracleHarness.Righteous(OracleHarness.Witness("the three refuse to worship the golden image, ready to die",
                new CourageousFaith(), new ObedienceToGod(), new SelfSacrifice(), new Fidelity()));
        }

        [Fact]
        public void NebuchadnezzarErectsTheGoldenImageAndCommandsAllToWorshipIt_IsSin()
        {
            // The king sets up a golden image and decrees that all peoples must fall down and worship it (Daniel 3:1-7).
            OracleHarness.Sin(OracleHarness.Witness("Nebuchadnezzar commands all to worship his golden image",
                new Idolatry(), new Pride(), new Oppression()));
        }

        [Fact]
        public void GodDeliversTheThreeFromTheFieryFurnace_IsADivineAct()
        {
            // A fourth like a son of the gods walks in the fire with them, and they come out unharmed (Daniel 3:24-27).
            OracleHarness.DivineAct(OracleHarness.Grounded("God walks in the furnace and delivers the three unharmed",
                Grounding.InGod(), new Deliverance(), new Protection(), new Fidelity()));
        }

        [Fact]
        public void DanielCounselsNebuchadnezzarToBreakOffSinByRighteousness_IsRighteous()
        {
            // Daniel urges the king to break off his sins by righteousness and his iniquities by mercy to the poor (Daniel 4:27).
            OracleHarness.Righteous(OracleHarness.Witness("Daniel counsels the king to turn from sin by righteousness and mercy to the poor",
                new TeachingTruth(), new Righteousness(), new Generosity(), new Compassion()));
        }

        [Fact]
        public void BelshazzarDesecratesTheTempleVesselsAndPraisesIdols_IsSin()
        {
            // Belshazzar drinks from the holy vessels of the temple while praising gods of gold and silver (Daniel 5:1-4, 22-23).
            OracleHarness.Sin(OracleHarness.Witness("Belshazzar profanes the temple vessels and praises idols",
                new Defilement(), new Idolatry(), new Pride(), new Blasphemy()));
            // Defilement is a capital vice and drives disorder to its maximum.
        }

        [Fact]
        public void TheSatrapsConspireFalselyToCondemnDaniel_IsSin()
        {
            // The officials, finding no fault in Daniel, scheme to trap him through a law about his prayer (Daniel 6:4-9).
            OracleHarness.Sin(OracleHarness.Witness("the satraps conspire with a false decree to destroy Daniel",
                new WickedScheming(), new FalseWitness(), new Envy(), new Treachery()));
        }

        [Fact]
        public void DanielPraysToGodThreeTimesDailyDespiteTheDecree_IsRighteous()
        {
            // Knowing the decree is signed, Daniel still kneels and prays toward Jerusalem as he always had (Daniel 6:10-11).
            OracleHarness.Righteous(OracleHarness.Witness("Daniel prays to God three times daily despite the death decree",
                new Worship(), new CourageousFaith(), new Fidelity(), new ObedienceToGod()));
        }

        [Fact]
        public void GodShutsTheLionsMouthsAndDeliversDaniel_IsADivineAct()
        {
            // God sends His angel to shut the lions' mouths, and Daniel is taken up unhurt because he trusted God (Daniel 6:19-23).
            OracleHarness.DivineAct(OracleHarness.Grounded("God shuts the lions' mouths and delivers Daniel from the den",
                Grounding.InGod(), new Deliverance(), new Protection(), new Fidelity()));
        }

        [Fact]
        public void DanielExposesTheFraudOfBelsPriests_IsRighteous()
        {
            // Daniel reveals by the ashes on the floor that the priests of Bel, not the idol, ate the offerings (Daniel 14:10-22, Bel and the Dragon).
            OracleHarness.Righteous(OracleHarness.Witness("Daniel exposes the deceit of Bel's priests by the secret footprints",
                new TeachingTruth(), new RejoicingInTruth(), new Righteousness()));
        }

        [Fact]
        public void SusannaRefusesTheEldersAndKeepsHerPurity_IsRighteous()
        {
            // Susanna refuses the two elders' demand, choosing to fall into their hands rather than sin against God (Daniel 13:22-23, Susanna).
            OracleHarness.Righteous(OracleHarness.Witness("Susanna refuses the elders and keeps her purity though it may cost her life",
                new Purity(), new CourageousFaith(), new SelfControl(), new ObedienceToGod()));
        }

        [Fact]
        public void TheTwoEldersBearFalseWitnessAgainstSusanna_IsSin()
        {
            // The elders, rebuffed in their lust, lie in court to condemn the innocent Susanna to death (Daniel 13:36-41, Susanna).
            OracleHarness.Sin(OracleHarness.Witness("the elders lust after and bear false witness against Susanna",
                new Lust(), new FalseWitness(), new WickedScheming(), new Condemnation()));
        }

        [Fact]
        public void DanielVindicatesSusannaByCrossExaminingTheElders_IsRighteous()
        {
            // The young Daniel separates the elders and convicts them by their contradiction, saving innocent blood (Daniel 13:45-62, Susanna).
            OracleHarness.Righteous(OracleHarness.Witness("Daniel cross-examines the elders and vindicates the innocent Susanna",
                new DefendingTheWeak(), new Righteousness(), new JustRecompense(), new Protection()));
        }
    }
}
