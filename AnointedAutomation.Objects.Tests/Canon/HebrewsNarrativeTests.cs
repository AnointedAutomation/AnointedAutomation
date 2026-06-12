// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests.Canon
{
    // Hebrews narrative oracle: the engine must return the verdict Scripture gives at each step.
    public class HebrewsNarrativeTests
    {
        [Fact]
        public void GodSpeaksToUsByHisSonTheHeirOfAllThings_IsADivineAct()
        {
            // "In these last days he has spoken to us by his Son ... through whom also he made the universe." (Hebrews 1:1-2)
            OracleHarness.DivineAct(OracleHarness.Witness("God speaks his final word through the Son who upholds all things", new TeachingTruth(), new CreationOrder(), new Fidelity()));
        }

        [Fact]
        public void TheSonPurifiesSinsAndSitsDownAtTheRightHand_IsADivineAct()
        {
            // "After he had provided purification for sins, he sat down at the right hand of the Majesty in heaven." (Hebrews 1:3)
            OracleHarness.DivineAct(OracleHarness.Witness("the Son makes purification for sins and is enthroned", new Atonement(), new Righteousness()));
        }

        [Fact]
        public void TheSonLovedRighteousnessAndHatedWickedness_IsADivineAct()
        {
            // "You have loved righteousness and hated wickedness; therefore God ... has anointed you." (Hebrews 1:8-9)
            OracleHarness.DivineAct(OracleHarness.Witness("the Son loves righteousness and rules with a righteous scepter", new Righteousness(), new JustRecompense()));
        }

        [Fact]
        public void NeglectingSoGreatASalvationAndDriftingAway_IsSin()
        {
            // "How shall we escape if we ignore so great a salvation?" (Hebrews 2:1-3)
            OracleHarness.Sin(OracleHarness.Witness("the hearers drift away and neglect the great salvation", new Disobedience(), new Indifference()));
        }

        [Fact]
        public void ChristTastedDeathForEveryoneAndDestroyedTheDevilsPower_IsADivineAct()
        {
            // "That by the grace of God he might taste death for everyone ... and free those held in slavery by fear of death." (Hebrews 2:9-15)
            OracleHarness.DivineAct(OracleHarness.Witness("Christ tastes death for everyone and delivers those enslaved by fear", new SelfSacrifice(), new Deliverance(), new Atonement()));
        }

        [Fact]
        public void ChristTheMercifulHighPriestMakesAtonementForThePeople_IsADivineAct()
        {
            // "A merciful and faithful high priest ... to make atonement for the sins of the people." (Hebrews 2:17-18)
            OracleHarness.DivineAct(OracleHarness.Witness("Christ becomes a merciful high priest and makes atonement", new Compassion(), new Fidelity(), new Atonement()));
        }

        [Fact]
        public void TheWildernessGenerationRebelledAndHardenedTheirHearts_IsSin()
        {
            // "Do not harden your hearts as you did in the rebellion ... where your ancestors tested me." (Hebrews 3:7-12)
            OracleHarness.Sin(OracleHarness.Witness("the wilderness generation rebels and hardens its heart against God", new Rebellion(), new Disobedience()));
        }

        [Fact]
        public void LetUsHoldFirmlyToOurConfessionAndDrawNearWithConfidence_IsRighteous()
        {
            // "Let us hold firmly to the faith we profess ... let us then approach God's throne of grace with confidence." (Hebrews 4:14-16)
            OracleHarness.Righteous(OracleHarness.Witness("the faithful hold fast their confession and draw near to the throne of grace", new CourageousFaith(), new Endurance(), new Trust()));
        }

        [Fact]
        public void ChristLearnedObedienceAndBecameTheSourceOfEternalSalvation_IsADivineAct()
        {
            // "He learned obedience from what he suffered ... he became the source of eternal salvation for all who obey him." (Hebrews 5:8-9)
            OracleHarness.DivineAct(OracleHarness.Witness("Christ obeys through suffering and becomes the source of eternal salvation", new ObedienceToGod(), new Deliverance(), new Endurance()));
        }

        [Fact]
        public void GodIsNotUnjustToForgetTheirWorkAndLoveShownToHisName_IsADivineAct()
        {
            // "God is not unjust; he will not forget your work and the love you have shown him as you have helped his people." (Hebrews 6:10)
            OracleHarness.DivineAct(OracleHarness.Witness("God justly remembers the love and labor shown to his name", new JustRecompense(), new Fidelity()));
        }

        [Fact]
        public void GodConfirmsHisPromiseWithAnOathThatCannotLie_IsADivineAct()
        {
            // "It is impossible for God to lie ... we who have fled to take hold of the hope set before us." (Hebrews 6:17-19)
            OracleHarness.DivineAct(OracleHarness.Witness("God confirms his unchanging promise with an oath he cannot break", new PromiseFulfilled(), new Trustworthiness(), new CovenantFaithfulness()));
        }

        [Fact]
        public void ChristTheEternalPriestAlwaysLivesToInterceedForThem_IsADivineAct()
        {
            // "He always lives to intercede for them ... a high priest who is holy, blameless, pure." (Hebrews 7:25-26)
            OracleHarness.DivineAct(OracleHarness.Witness("Christ the holy high priest forever lives to intercede and save completely", new Purity(), new Deliverance(), new Righteousness()));
        }

        [Fact]
        public void GodMediatesABetterCovenantAndRemembersTheirSinsNoMore_IsADivineAct()
        {
            // "I will forgive their wickedness and will remember their sins no more." (Hebrews 8:6-12)
            OracleHarness.DivineAct(OracleHarness.Witness("God establishes a better covenant and remembers their sins no more", new Pardon(), new CovenantFaithfulness(), new PromiseFulfilled()));
        }

        [Fact]
        public void ChristOffersHisOwnBloodOnceForAllToCleanseTheConscience_IsADivineAct()
        {
            // "He entered the Most Holy Place once for all by his own blood ... to cleanse our consciences." (Hebrews 9:12-14)
            OracleHarness.DivineAct(OracleHarness.Witness("Christ offers himself once for all to cleanse the conscience and obtain redemption", new SelfSacrifice(), new Atonement(), new Healing()));
        }

        [Fact]
        public void ChristCameToDoTheWillOfGodAndPerfectsThoseBeingMadeHoly_IsADivineAct()
        {
            // "Here I am ... I have come to do your will ... by that will we have been made holy through the sacrifice of the body of Jesus Christ once for all." (Hebrews 10:7-14)
            OracleHarness.DivineAct(OracleHarness.Witness("Christ does the Father's will and by one sacrifice perfects the sanctified", new ObedienceToGod(), new SelfSacrifice(), new Atonement()));
        }

        [Fact]
        public void DeliberatelyTramplingTheSonAndProfaningTheBloodOfTheCovenant_IsSin()
        {
            // "Anyone who has trampled the Son of God underfoot ... and who has insulted the Spirit of grace." (Hebrews 10:26-29)
            OracleHarness.Sin(OracleHarness.Witness("the apostate tramples the Son and profanes the blood of the covenant", new Blasphemy(), new CovenantBreaking(), new Rebellion()));
        }

        [Fact]
        public void NoahInHolyFearBuiltAnArkToSaveHisHousehold_IsRighteous()
        {
            // "By faith Noah ... in holy fear built an ark to save his family." (Hebrews 11:7)
            OracleHarness.Righteous(OracleHarness.Witness("Noah by faith builds the ark and saves his household", new CourageousFaith(), new ObedienceToGod(), new Protection()));
        }

        [Fact]
        public void AbrahamObeyedAndOfferedIsaacTrustingGodToRaiseTheDead_IsRighteous()
        {
            // "By faith Abraham, when God tested him, offered Isaac ... reasoned that God could raise the dead." (Hebrews 11:8-19)
            OracleHarness.Righteous(OracleHarness.Witness("Abraham by faith obeys and offers up Isaac, trusting God to raise the dead", new CourageousFaith(), new ObedienceToGod(), new Trust()));
        }

        [Fact]
        public void MosesChoseToSufferWithGodsPeopleRatherThanEnjoySinsPleasures_IsRighteous()
        {
            // "He chose to be mistreated along with the people of God rather than to enjoy the fleeting pleasures of sin." (Hebrews 11:24-26)
            OracleHarness.Righteous(OracleHarness.Witness("Moses chooses to suffer with God's people rather than the pleasures of sin", new CourageousFaith(), new SelfSacrifice(), new Endurance()));
        }

        [Fact]
        public void TheFaithfulEnduredSufferingAndTortureLookingForResurrection_IsRighteous()
        {
            // "Others were tortured ... so that they might gain an even better resurrection." (Hebrews 11:35-38)
            OracleHarness.Righteous(OracleHarness.Witness("the faithful endure torture and affliction, awaiting a better resurrection", new Endurance(), new CourageousFaith(), new Fidelity()));
        }

        [Fact]
        public void RunWithEnduranceFixingEyesOnJesusWhoEnduredTheCross_IsRighteous()
        {
            // "Let us run with perseverance ... fixing our eyes on Jesus ... who for the joy set before him endured the cross." (Hebrews 12:1-2)
            OracleHarness.Righteous(OracleHarness.Witness("the saints run with endurance, fixing their eyes on Jesus who endured the cross", new Endurance(), new CourageousFaith(), new Joy()));
        }

        [Fact]
        public void TheLordDisciplinesThoseHeLovesForTheirGood_IsADivineAct()
        {
            // "The Lord disciplines the one he loves ... that we may share in his holiness ... a harvest of righteousness." (Hebrews 12:6-11)
            OracleHarness.DivineAct(OracleHarness.Witness("the Lord disciplines those he loves to yield a harvest of righteousness", new JustRecompense(), new Righteousness(), new Fidelity()));
        }

        [Fact]
        public void EsauSoldHisInheritanceForASingleMealAndDespisedTheBirthright_IsSin()
        {
            // "See that no one is ... godless like Esau, who for a single meal sold his inheritance rights." (Hebrews 12:16-17)
            OracleHarness.Sin(OracleHarness.Grounded("Esau despises his birthright and sells his inheritance for one meal", Grounding.InIdol("his appetite"), new Gluttony(), new Ingratitude()));
        }

        [Fact]
        public void KeepOnLovingOneAnotherAndShowHospitalityToStrangers_IsRighteous()
        {
            // "Keep on loving one another ... show hospitality to strangers, for ... some have entertained angels." (Hebrews 13:1-2)
            OracleHarness.Righteous(OracleHarness.Witness("the brethren love one another and show hospitality to strangers", new Kindness(), new Hospitality(), new Compassion()));
        }

        [Fact]
        public void RememberThePrisonersAndThoseMistreatedAsThoughSufferingWithThem_IsRighteous()
        {
            // "Continue to remember those in prison as if you were together with them ... those who are mistreated." (Hebrews 13:3)
            OracleHarness.Righteous(OracleHarness.Witness("the faithful remember the prisoners and the mistreated as their own", new VisitingThePrisoner(), new Compassion(), new HonoringTheVulnerable()));
        }

        [Fact]
        public void KeepTheMarriageBedPureAndFreeYourselvesFromTheLoveOfMoney_IsRighteous()
        {
            // "Marriage should be honored by all, and the marriage bed kept pure ... keep your lives free from the love of money." (Hebrews 13:4-5)
            OracleHarness.Righteous(OracleHarness.Witness("the saints honor marriage in purity and keep free from the love of money", new Purity(), new SelfControl(), new CovenantFaithfulness()));
        }

        [Fact]
        public void DoNotForgetToDoGoodAndToShareForSuchSacrificesPleaseGod_IsRighteous()
        {
            // "Do not forget to do good and to share with others, for with such sacrifices God is pleased." (Hebrews 13:16)
            OracleHarness.Righteous(OracleHarness.Witness("the believers do good and share with others as a pleasing sacrifice", new Generosity(), new Goodness(), new SelfSacrifice()));
        }

        [Fact]
        public void TheGodOfPeaceRaisedJesusAndEquipsThemToDoHisWill_IsADivineAct()
        {
            // "May the God of peace, who through the blood of the eternal covenant brought back ... Jesus ... equip you." (Hebrews 13:20-21)
            OracleHarness.DivineAct(OracleHarness.Witness("the God of peace raises Jesus and equips his people for every good work", new RestoringLife(), new CovenantFaithfulness(), new Peace()));
        }
    }
}
