// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Concepts;

namespace AnointedAutomation.Concepts.Tests.Canon
{
    // Jubilees narrative oracle: the engine must return the verdict Scripture gives at each step.
    // Jubilees retells Genesis through early Exodus as the angel of the presence dictates the heavenly
    // tablets to Moses on Sinai, with strong emphasis on the Sabbath, the calendar, and covenant law.
    [Trait("Canon", "EthiopicNonLoadBearing")]
    public class JubileesBookNarrativeTests
    {
        [Fact]
        public void GodCreatesTheHeavensAndEarthInSixDays_IsADivineAct()
        {
            // "On the first day He created the heavens ... and on the sixth day ... He finished all His work." (Jubilees 2:1-16)
            // The ordered six-day creation establishes the order of all reality.
            OracleHarness.DivineAct(OracleHarness.Witness("God creates the heavens and the earth in six ordered days",
                new CreationOrder(), new Goodness(), new Fidelity(), new Worship()));
        }

        [Fact]
        public void GodSanctifiesTheSeventhDayAsTheSabbath_IsADivineAct()
        {
            // "He gave us the Sabbath day ... to keep Sabbath thereon from all work." (Jubilees 2:17-33)
            // God blesses and hallows the seventh day as an everlasting covenant sign.
            OracleHarness.DivineAct(OracleHarness.Witness("God blesses and sanctifies the seventh day as the Sabbath",
                new CreationOrder(), new CovenantFaithfulness(), new Worship(),
                new Goodness(), new Fidelity()));
        }

        [Fact]
        public void AdamAndEveTransgressTheCommandInEden_IsSin()
        {
            // The serpent deceives Eve and they eat the forbidden fruit, breaking the command. (Jubilees 3:17-25)
            // Disobedience and deceit shatter the order of the garden and bring death.
            OracleHarness.Sin(OracleHarness.Witness("Adam and Eve disobey God's command in the garden, deceived by the serpent",
                new Disobedience(), new Deceit(), new CovenantBreaking(), new Rebellion()));
        }

        [Fact]
        public void CainMurdersHisBrotherAbel_IsSin()
        {
            // "Cain slew Abel his brother ... and his blood cried from the ground." (Jubilees 4:1-6)
            // The first murder is capital bloodshed that defiles the earth.
            Resolution r = OracleHarness.Witness("Cain murders his brother Abel in the field",
                new Murder(), new Bloodshed(), new Hatred(), new Envy());
            OracleHarness.Sin(r);
            Assert.Equal(1.0, r.Disorder);
        }

        [Fact]
        public void TheWatchersDefileThemselvesWithTheDaughtersOfMen_IsSin()
        {
            // The angels who were Watchers sin with the daughters of men, begetting the giants. (Jubilees 5:1-11)
            // Their fornication and corruption fill the earth with violence and uncleanness.
            Resolution r = OracleHarness.Witness("the Watchers defile themselves with the daughters of men and corrupt the earth",
                new SexualImmorality(), new Defilement(), new Lust(), new Rebellion());
            OracleHarness.Sin(r);
            Assert.Equal(1.0, r.Disorder);
        }

        [Fact]
        public void NoahWalksBlamelesslyAndBuildsTheArkInObedience_IsRighteous()
        {
            // Noah found grace and did according to all God commanded, building the ark. (Jubilees 5:19; 6:1)
            // His blameless obedience preserves life through the flood.
            OracleHarness.Righteous(OracleHarness.Witness("Noah obeys God and builds the ark to preserve life",
                new ObedienceToGod(), new Righteousness(), new Fidelity(),
                new CourageousFaith(), new Protection()));
        }

        [Fact]
        public void GodEstablishesHisCovenantWithNoahAfterTheFlood_IsADivineAct()
        {
            // God sets His bow in the cloud and covenants never again to destroy the earth by flood. (Jubilees 6:1-16)
            // The everlasting covenant and the feast of weeks are ordained.
            OracleHarness.DivineAct(OracleHarness.Witness("God makes His covenant with Noah and sets the bow in the cloud",
                new CovenantFaithfulness(), new PromiseFulfilled(), new Goodness(),
                new Fidelity(), new CreationOrder()));
        }

        [Fact]
        public void GodCommandsNoahToAbstainFromBloodAndKeepRighteousness_IsADivineAct()
        {
            // Noah is charged to eat no blood, to do judgment and righteousness, and to cover blamelessness. (Jubilees 7:20-33)
            // God ordains the law against bloodshed to keep the earth from defilement.
            OracleHarness.DivineAct(OracleHarness.Witness("God commands Noah to shun blood and walk in righteousness",
                new Righteousness(), new ObedienceToGod(), new CovenantFaithfulness(),
                new CreationOrder(), new Goodness()));
        }

        [Fact]
        public void AbramBurnsTheHouseOfIdolsAndRefusesTheStars_IsRighteous()
        {
            // Abram rejects the idols of his fathers, burns the house of idols, and worships the Creator. (Jubilees 11:16-17; 12:1-14)
            // He turns from idolatry to courageous, true worship of the one God.
            OracleHarness.Righteous(OracleHarness.Witness("Abram forsakes idolatry and worships the Creator alone",
                new Worship(), new CourageousFaith(), new ObedienceToGod(),
                new Fidelity(), new RejoicingInTruth()));
        }

        [Fact]
        public void GodCovenantsWithAbrahamAndPromisesTheLand_IsADivineAct()
        {
            // God appears to Abram, makes covenant, and promises seed, blessing, and the land. (Jubilees 14:1-20; 15:1-10)
            // The covenant of circumcision is given as an everlasting ordinance.
            OracleHarness.DivineAct(OracleHarness.Witness("God covenants with Abraham and promises the land and seed",
                new CovenantFaithfulness(), new PromiseFulfilled(), new Fidelity(),
                new Goodness(), new Trustworthiness()));
        }

        [Fact]
        public void MastemaAndTheSodomitesWorkUncleannessAndViolence_IsSin()
        {
            // The men of Sodom are wholly sinful, defiling themselves and working violence. (Jubilees 16:5-6)
            // Prince Mastema's people fill the land with uncleanness and bloodshed and are destroyed.
            Resolution r = OracleHarness.Grounded("the men of Sodom defile themselves with violence and uncleanness",
                Grounding.InIdol("Mastema"),
                new Bloodshed(), new Defilement(), new SexualImmorality(), new Oppression());
            OracleHarness.Sin(r);
            Assert.Equal(1.0, r.Disorder);
        }

        [Fact]
        public void AbrahamObeysGodInTheBindingOfIsaac_IsRighteous()
        {
            // Tempted by Mastema, Abraham obeys to offer Isaac, and is found faithful. (Jubilees 17:15-18:19)
            // His obedience and faith under trial prove his covenant faithfulness.
            OracleHarness.Righteous(OracleHarness.Witness("Abraham obeys God in the binding of Isaac and is found faithful",
                new ObedienceToGod(), new CourageousFaith(), new Fidelity(),
                new CovenantFaithfulness(), new Trust()));
        }

        [Fact]
        public void GodSmitesEgyptAndDeliversIsraelAtThePassover_IsADivineAct()
        {
            // On the night of the Passover God smites Egypt and brings Israel out by His mighty hand. (Jubilees 49:1-23)
            // The Lord delivers His covenant people and ordains the Passover forever.
            OracleHarness.DivineAct(OracleHarness.Witness("God delivers Israel from Egypt and ordains the Passover",
                new Deliverance(), new CovenantFaithfulness(), new PromiseFulfilled(),
                new JustRecompense(), new Fidelity()));
        }

        [Fact]
        public void GodGivesTheSabbathLawOnSinaiThroughMoses_IsADivineAct()
        {
            // The Lord commands the keeping of the Sabbath as an everlasting law from the heavenly tablets. (Jubilees 50:1-13)
            // The Sabbath rest is sealed as the holy sign of the covenant.
            OracleHarness.DivineAct(OracleHarness.Witness("God gives the everlasting Sabbath law to Israel through Moses on Sinai",
                new CovenantFaithfulness(), new CreationOrder(), new Worship(),
                new Fidelity(), new TeachingTruth()));
        }
    }
}
