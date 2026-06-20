// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests.Canon
{
    // 3 John narrative oracle: the engine must return the verdict Scripture gives at each step.
    public class ThirdJohnNarrativeTests
    {
        [Fact]
        public void ElderLovesGaiusInTruth_IsRighteous()
        {
            // "The elder unto the wellbeloved Gaius, whom I love in the truth." (3 John 1)
            OracleHarness.Righteous(OracleHarness.Witness("the elder loves Gaius in the truth", new RejoicingInTruth(), new Fidelity()));
        }

        [Fact]
        public void ElderPraysForGaiusHealthAndProsperity_IsRighteous()
        {
            // "Beloved, I wish above all things that thou mayest prosper and be in health, even as thy soul prospereth." (3 John 2)
            OracleHarness.Righteous(OracleHarness.Witness("the elder prays that Gaius may prosper and be in health", new Kindness(), new Compassion()));
        }

        [Fact]
        public void ElderRejoicesThatGaiusWalksInTruth_IsRighteous()
        {
            // "I rejoiced greatly ... even as thou walkest in the truth." (3 John 3-4)
            OracleHarness.Righteous(OracleHarness.Witness("the elder rejoices greatly that Gaius walks in the truth", new Joy(), new RejoicingInTruth(), new ObedienceToGod()));
        }

        [Fact]
        public void GaiusFaithfullyHostsTravelingBrethrenAndStrangers_IsRighteous()
        {
            // "thou doest faithfully whatsoever thou doest to the brethren, and to strangers." (3 John 5)
            OracleHarness.Righteous(OracleHarness.Witness("Gaius faithfully serves the brethren and the strangers who come to him", new Hospitality(), new Fidelity(), new Kindness()));
        }

        [Fact]
        public void GaiusBringsTheBrethrenForwardOnTheirJourneyAfterAGodlyManner_IsRighteous()
        {
            // "whom if thou bring forward on their journey after a godly sort, thou shalt do well." (3 John 6)
            OracleHarness.Righteous(OracleHarness.Witness("Gaius sends the missionary brethren onward in a manner worthy of God", new Generosity(), new Hospitality(), new HonoringTheVulnerable()));
        }

        [Fact]
        public void MissionariesWentForthForHisNameTakingNothingOfTheGentiles_IsRighteous()
        {
            // "Because that for his name's sake they went forth, taking nothing of the Gentiles." (3 John 7)
            OracleHarness.Righteous(OracleHarness.Witness("the brethren go forth for the Name, taking nothing from the Gentiles", new SelfSacrifice(), new CourageousFaith(), new Fidelity()));
        }

        [Fact]
        public void ReceivingSuchMissionariesAsFellowhelpersToTheTruth_IsRighteous()
        {
            // "We therefore ought to receive such, that we might be fellowhelpers to the truth." (3 John 8)
            OracleHarness.Righteous(OracleHarness.Witness("believers ought to receive such men, becoming fellow workers for the truth", new Hospitality(), new RejoicingInTruth(), new Generosity()));
        }

        [Fact]
        public void DiotrephesLovesPreeminenceAndRefusesTheElder_IsSin()
        {
            // "Diotrephes, who loveth to have the preeminence among them, receiveth us not." (3 John 9)
            OracleHarness.Sin(OracleHarness.Witness("Diotrephes loves to be first and refuses to receive the elder", new Pride(), new SelfishAmbition(), new Rebellion()));
        }

        [Fact]
        public void DiotrephesPratesAgainstTheElderWithMaliciousWords_IsSin()
        {
            // "prating against us with malicious words." (3 John 10)
            OracleHarness.Sin(OracleHarness.Witness("Diotrephes spreads malicious words and slander against the elder", new Slander(), new Malice(), new Gossip()));
        }

        [Fact]
        public void DiotrephesRefusesTheBrethrenAndCastsOutThoseWhoWould_IsSin()
        {
            // "neither doth he himself receive the brethren, and forbiddeth them that would, and casteth them out of the church." (3 John 10)
            OracleHarness.Sin(OracleHarness.Witness("Diotrephes refuses the brethren, forbids those who would help, and casts them out of the church", new Oppression(), new Indifference(), new Heartlessness()));
        }

        [Fact]
        public void ElderExhortsToFollowGoodNotEvilForHeThatDoethGoodIsOfGod_IsRighteous()
        {
            // "Beloved, follow not that which is evil, but that which is good. He that doeth good is of God." (3 John 11)
            OracleHarness.Righteous(OracleHarness.Witness("the elder exhorts Gaius to follow good and not evil, for the doer of good is of God", new Goodness(), new ObedienceToGod(), new Righteousness()));
        }

        [Fact]
        public void DemetriusHasGoodReportOfAllMenAndOfTheTruth_IsRighteous()
        {
            // "Demetrius hath good report of all men, and of the truth itself." (3 John 12)
            OracleHarness.Righteous(OracleHarness.Witness("Demetrius has a good report of all men and of the truth itself", new Trustworthiness(), new RejoicingInTruth(), new Fidelity()));
        }

        [Fact]
        public void ElderHopesToComeAndSpeakFaceToFaceAndGreetsTheFriendsByName_IsRighteous()
        {
            // "I trust I shall shortly see thee, and we shall speak face to face. ... Greet the friends by name." (3 John 14)
            OracleHarness.Righteous(OracleHarness.Witness("the elder hopes to speak face to face and greets the friends by name", new Peace(), new Kindness(), new Peacemaking()));
        }
    }
}
