// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests.Canon
{
    // 2 John narrative oracle: the engine must return the verdict Scripture gives at each step.
    public class SecondJohnNarrativeTests
    {
        [Fact]
        public void ElderLovesTheChosenLadyAndHerChildrenInTruth_IsRighteous()
        {
            // "The elder, to the elect lady and her children, whom I love in the truth." (2 John 1)
            OracleHarness.Righteous(OracleHarness.Witness("the elder loves the chosen lady and her children in the truth", new RejoicingInTruth(), new Fidelity()));
        }

        [Fact]
        public void GraceMercyAndPeaceFromGodTheFatherAndChrist_IsADivineAct()
        {
            // "Grace, mercy, and peace ... from God the Father and from Jesus Christ ... in truth and love." (2 John 3)
            OracleHarness.DivineAct(OracleHarness.Grounded("God the Father and Christ give grace, mercy, and peace in truth and love", Grounding.InGod(), new Pardon(), new Compassion(), new Peace()));
        }

        [Fact]
        public void ElderRejoicesToFindHerChildrenWalkingInTruth_IsRighteous()
        {
            // "I rejoiced greatly to find some of your children walking in the truth." (2 John 4)
            OracleHarness.Righteous(OracleHarness.Witness("the elder rejoices to find her children walking in the truth", new Joy(), new RejoicingInTruth(), new ObedienceToGod()));
        }

        [Fact]
        public void CommandmentThatWeLoveOneAnother_IsRighteous()
        {
            // "I ask that we love one another ... this is the commandment, that you walk in love." (2 John 5-6)
            OracleHarness.Righteous(OracleHarness.Witness("the elect lady is asked to love one another as from the beginning", new Kindness(), new CovenantFaithfulness(), new ObedienceToGod()));
        }

        [Fact]
        public void WalkingInLoveAccordingToHisCommandments_IsRighteous()
        {
            // "And this is love, that we walk according to his commandments." (2 John 6)
            OracleHarness.Righteous(OracleHarness.Witness("the believers walk in love according to God's commandments", new ObedienceToGod(), new CovenantFaithfulness(), new Fidelity()));
        }

        [Fact]
        public void ManyDeceiversGoneOutDenyingChristCameInFlesh_IsSin()
        {
            // "Many deceivers have gone out into the world, who do not confess Jesus Christ as coming in the flesh. This is the deceiver and the antichrist." (2 John 7)
            OracleHarness.Sin(OracleHarness.Witness("many deceivers deny that Jesus Christ has come in the flesh", new Deceit(), new Blasphemy(), new Treachery()));
        }

        [Fact]
        public void RunningAheadAndNotAbidingInChristsTeaching_IsSin()
        {
            // "Whoever goes on ahead and does not abide in the teaching of Christ, does not have God." (2 John 9)
            OracleHarness.Sin(OracleHarness.Witness("the false teacher runs ahead and abandons the doctrine of Christ", new Rebellion(), new Disobedience(), new Pride()));
        }

        [Fact]
        public void AbidingInTheTeachingOfChristHasBothFatherAndSon_IsRighteous()
        {
            // "Whoever abides in the teaching has both the Father and the Son." (2 John 9)
            OracleHarness.Righteous(OracleHarness.Witness("the faithful abide in the teaching of Christ and hold to sound doctrine", new Fidelity(), new CovenantFaithfulness(), new Endurance()));
        }

        [Fact]
        public void ReceivingAFalseTeacherSharesInHisWickedWorks_IsSin()
        {
            // "If anyone ... receives him into your house ... shares in his wicked works." (2 John 10-11)
            OracleHarness.Sin(OracleHarness.Witness("welcoming the false teacher and greeting him shares in his wicked works", new Deceit(), new Treachery(), new Wrongdoing()));
        }

        [Fact]
        public void RefusingToHostTheDeceiverGuardsTheHousehold_IsRighteous()
        {
            // "Do not receive him into your house or give him any greeting." (2 John 10) - guarding the flock from the deceiver
            OracleHarness.Righteous(OracleHarness.Witness("the elect lady protects her household by refusing to harbor the deceiver", new Protection(), new DefendingTheWeak(), new CourageousFaith()));
        }

        [Fact]
        public void ElderDesiresToComeAndSpeakFaceToFaceForFullJoy_IsRighteous()
        {
            // "I hope to come to you and talk face to face, so that our joy may be complete." (2 John 12)
            OracleHarness.Righteous(OracleHarness.Witness("the elder longs to visit face to face that their joy may be full", new Joy(), new Kindness(), new Peacemaking()));
        }
    }
}
