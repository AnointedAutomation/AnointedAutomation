// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests.Canon
{
    // Oracle 51-64: Acts and the Pauline epistles (Romans through Philemon).
    public class ActsAndPaulineOracleTests
    {
        [Fact]
        public void Acts_TheChurchSharesWithTheNeedy_IsRighteous()
        {
            // "They gave to anyone who had need." (Acts 2:44-45)
            OracleHarness.Righteous(OracleHarness.Witness("the church shares with the needy", new Generosity(), new Compassion()));
        }

        [Fact]
        public void Acts_AnaniasAndSapphirasDeceit_IsSin()
        {
            // Lying to the Holy Spirit about the gift (Acts 5:1-11).
            OracleHarness.Sin(OracleHarness.Witness("Ananias lies to the Spirit", new Deceit(), new Greed()));
        }

        [Fact]
        public void Romans_TheAtonementJustAndJustifying_IsADivineAct()
        {
            // "So as to be just and the one who justifies." (Romans 3:25-26) Justice and mercy meet.
            OracleHarness.DivineAct(OracleHarness.Witness("the atonement", new Atonement(), new Pardon()));
        }

        [Fact]
        public void Romans_LoveDoesNoHarmToANeighbor_IsRighteous()
        {
            // "Love does no harm to a neighbor. Therefore love is the fulfillment of the law." (Rom 13:10)
            OracleHarness.Righteous(OracleHarness.Witness("love fulfills the law", new Compassion()));
        }

        [Fact]
        public void FirstCorinthians_LoveIsPatientAndKind_IsRighteous()
        {
            // "Love is patient, love is kind." (1 Corinthians 13:4)
            OracleHarness.Righteous(OracleHarness.Witness("love is patient and kind", new Patience(), new Kindness()));
        }

        [Fact]
        public void SecondCorinthians_TheMinistryOfReconciliation_IsADivineAct()
        {
            // "God made him who had no sin to be sin for us." (2 Corinthians 5:21)
            OracleHarness.DivineAct(OracleHarness.Witness("the ministry of reconciliation", new Atonement(), new Peacemaking()));
        }

        [Fact]
        public void Galatians_BearOneAnothersBurdens_IsRighteous()
        {
            // "Carry each other's burdens, and so fulfill the law of Christ." (Galatians 6:2)
            OracleHarness.Righteous(OracleHarness.Witness("bear one another's burdens", new Compassion(), new Patience()));
        }

        [Fact]
        public void Ephesians_SavedByGraceThroughFaith_IsADivineAct()
        {
            // "It is by grace you have been saved, through faith." (Ephesians 2:8)
            OracleHarness.DivineAct(OracleHarness.Witness("saved by grace", new Pardon(), new Compassion()));
        }

        [Fact]
        public void Philippians_ChristEmptiesHimself_IsADivineAct()
        {
            // "He humbled himself ... to death on a cross." (Philippians 2:6-8)
            OracleHarness.DivineAct(OracleHarness.Witness("Christ empties Himself", new SelfSacrifice(), new Humility()));
        }

        [Fact]
        public void Colossians_InHimAllThingsHoldTogether_IsADivineAct()
        {
            // "In him all things hold together." (Colossians 1:17) The grounding of reality.
            OracleHarness.DivineAct(OracleHarness.Witness("in Him all things hold together", new CreationOrder()));
        }

        [Fact]
        public void Thessalonians_TheLawlessOne_IsSin()
        {
            // "The man of lawlessness." (2 Thessalonians 2:3)
            OracleHarness.Sin(OracleHarness.Witness("the man of lawlessness", new Lawlessness(), new Pride()));
        }

        [Fact]
        public void FirstTimothy_ChristCameToSaveSinners_IsADivineAct()
        {
            // "Christ Jesus came into the world to save sinners." (1 Timothy 1:15)
            OracleHarness.DivineAct(OracleHarness.Witness("Christ came to save sinners", new Pardon(), new Compassion()));
        }

        [Fact]
        public void FirstTimothy_TheLoveOfMoney_IsSin()
        {
            // "The love of money is a root of all kinds of evil." (1 Timothy 6:10)
            OracleHarness.Sin(OracleHarness.Witness("the love of money", new Greed()));
        }

        [Fact]
        public void SecondTimothy_HeRemainsFaithful_IsADivineAct()
        {
            // "If we are faithless, he remains faithful." (2 Timothy 2:13)
            OracleHarness.DivineAct(OracleHarness.Witness("He remains faithful", new CovenantFaithfulness()));
        }

        [Fact]
        public void Titus_SavedByHisMercyNotByWorks_IsADivineAct()
        {
            // "He saved us ... because of his mercy." (Titus 3:5)
            OracleHarness.DivineAct(OracleHarness.Witness("saved by His mercy", new Pardon(), new Compassion()));
        }

        [Fact]
        public void Philemon_ReceiveHimBackAsABrother_IsRighteous()
        {
            // Paul pleads for Onesimus to be received back, no longer as a slave but a brother (Philemon 15-17).
            OracleHarness.Righteous(OracleHarness.Witness("receive him back as a brother", new Forgiveness(), new Peacemaking()));
        }
    }
}
