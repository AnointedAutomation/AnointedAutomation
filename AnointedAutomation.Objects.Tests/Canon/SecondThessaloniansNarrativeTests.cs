// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests.Canon
{
    // 2 Thessalonians narrative oracle: the engine must return the verdict Scripture gives at each step.
    public class SecondThessaloniansNarrativeTests
    {
        [Fact]
        public void TheirFaithGrowsAndTheirLoveAbounds_IsRighteous()
        {
            // "Your faith is growing abundantly, and the love of every one of you for one another is increasing." (2 Thessalonians 1:3)
            OracleHarness.Righteous(OracleHarness.Witness("the church grows in faith and abounding love for one another", new Fidelity(), new Kindness(), new Compassion()));
        }

        [Fact]
        public void TheyEndurePersecutionsAndAfflictionsWithSteadfastness_IsRighteous()
        {
            // "Your steadfastness and faith in all your persecutions and in the afflictions that you are enduring." (2 Thessalonians 1:4)
            OracleHarness.Righteous(OracleHarness.Witness("the believers endure persecution with steadfast faith", new Endurance(), new Fidelity(), new CourageousFaith()));
        }

        [Fact]
        public void GodRepaysWithAfflictionThoseWhoAfflictTheChurch_IsADivineAct()
        {
            // "God considers it just to repay with affliction those who afflict you ... this is evidence of the righteous judgment of God." (2 Thessalonians 1:5-6)
            OracleHarness.DivineAct(OracleHarness.Witness("God justly repays the afflicters and grants relief to the afflicted", new JustRecompense(), new Righteousness(), new Deliverance()));
        }

        [Fact]
        public void GodGrantsReliefToTheAfflictedWhenChristIsRevealed_IsADivineAct()
        {
            // "To grant relief to you who are afflicted ... when the Lord Jesus is revealed from heaven." (2 Thessalonians 1:7)
            OracleHarness.DivineAct(OracleHarness.Witness("the Lord Jesus is revealed from heaven and rescues his afflicted people", new Deliverance(), new Protection(), new PromiseFulfilled()));
        }

        [Fact]
        public void GodFulfillsEveryGoodResolveAndWorkOfFaith_IsADivineAct()
        {
            // "That our God ... may fulfill every resolve for good and every work of faith by his power." (2 Thessalonians 1:11-12)
            OracleHarness.DivineAct(OracleHarness.Witness("God fulfills every good resolve and work of faith by his power", new PromiseFulfilled(), new Goodness(), new Fidelity()));
        }

        [Fact]
        public void TheManOfLawlessnessExaltsHimselfAboveGod_IsSin()
        {
            // "The man of lawlessness ... opposes and exalts himself against every so-called god ... proclaiming himself to be God." (2 Thessalonians 2:3-4)
            OracleHarness.Sin(OracleHarness.Grounded("the man of lawlessness exalts himself above God and claims to be God", Grounding.InIdol("self"), new Lawlessness(), new Pride(), new Blasphemy(), new Rebellion()));
        }

        [Fact]
        public void TheLawlessOneWorksFalseSignsAndDeceivesThePerishing_IsSin()
        {
            // "The lawless one ... with all power and false signs and wonders, and with all wicked deception." (2 Thessalonians 2:9-10)
            OracleHarness.Sin(OracleHarness.Grounded("the lawless one works false signs and wicked deception against the perishing", Grounding.InIdol("Satan"), new Deceit(), new Lawlessness(), new Sorcery()));
        }

        [Fact]
        public void TheLordJesusSlaysTheLawlessOneWithTheBreathOfHisMouth_IsADivineAct()
        {
            // "The Lord Jesus will kill with the breath of his mouth and bring to nothing by the appearance of his coming." (2 Thessalonians 2:8)
            OracleHarness.DivineAct(OracleHarness.Witness("the Lord Jesus destroys the lawless one and overthrows his rebellion", new JustRecompense(), new Righteousness(), new Deliverance()));
        }

        [Fact]
        public void GodChoseThemForSalvationThroughSanctificationOfTheSpirit_IsADivineAct()
        {
            // "God chose you ... to be saved, through sanctification by the Spirit and belief in the truth." (2 Thessalonians 2:13)
            OracleHarness.DivineAct(OracleHarness.Witness("God chooses them for salvation through sanctification and belief in the truth", new Deliverance(), new RejoicingInTruth(), new CovenantFaithfulness()));
        }

        [Fact]
        public void GodComfortsTheirHeartsAndEstablishesThemInEveryGoodWork_IsADivineAct()
        {
            // "Comfort your hearts and establish them in every good work and word." (2 Thessalonians 2:16-17)
            OracleHarness.DivineAct(OracleHarness.Witness("God comforts their hearts and establishes them in every good work", new Compassion(), new Goodness(), new Protection()));
        }

        [Fact]
        public void TheLordIsFaithfulAndGuardsThemFromTheEvilOne_IsADivineAct()
        {
            // "The Lord is faithful. He will establish you and guard you against the evil one." (2 Thessalonians 3:3)
            OracleHarness.DivineAct(OracleHarness.Witness("the faithful Lord establishes and guards them from the evil one", new Fidelity(), new Protection(), new Trustworthiness()));
        }

        [Fact]
        public void PaulWorkedNightAndDaySoAsNotToBurdenAnyone_IsRighteous()
        {
            // "With toil and labor we worked night and day, that we might not be a burden to any of you." (2 Thessalonians 3:8)
            OracleHarness.Righteous(OracleHarness.Witness("Paul labors night and day to burden no one and to set an example", new SelfSacrifice(), new Generosity(), new SelfControl()));
        }

        [Fact]
        public void TheIdleBusybodiesWhoWillNotWork_IsSin()
        {
            // "Some who walk among you in idleness, not busy at work, but busybodies." (2 Thessalonians 3:11)
            OracleHarness.Sin(OracleHarness.Witness("certain men live in idleness, refusing to work and meddling as busybodies", new Sloth(), new Disobedience(), new Chaos()));
        }

        [Fact]
        public void TheBelieversDoNotGrowWearyInDoingGood_IsRighteous()
        {
            // "As for you, brothers, do not grow weary in doing good." (2 Thessalonians 3:13)
            OracleHarness.Righteous(OracleHarness.Witness("the believers persevere in doing good and do not grow weary", new Goodness(), new Endurance(), new Fidelity()));
        }

        [Fact]
        public void TheLordOfPeaceGivesThemPeaceAtAllTimes_IsADivineAct()
        {
            // "Now may the Lord of peace himself give you peace at all times in every way." (2 Thessalonians 3:16)
            OracleHarness.DivineAct(OracleHarness.Witness("the Lord of peace himself grants them peace at all times in every way", new Peace(), new Goodness(), new Fidelity()));
        }
    }
}
