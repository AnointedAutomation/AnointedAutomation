// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Concepts;

namespace AnointedAutomation.Concepts.Tests.Canon
{
    // 1 John narrative oracle: the engine must return the verdict Scripture gives at each step.
    public class FirstJohnNarrativeTests
    {
        [Fact]
        public void ChristTheWordOfLifeIsProclaimedThatJoyMayBeComplete_IsADivineAct()
        {
            // "the life was made manifest ... we proclaim to you ... so that our joy may be complete." (1 John 1:1-4)
            OracleHarness.DivineAct(OracleHarness.Witness("the eternal Word of life is made manifest and proclaimed by the apostles, so that the fellowship of the believers may be complete and their joy full", new TeachingTruth(), new RejoicingInTruth(), new RestoringLife(), new Joy()));
        }

        [Fact]
        public void TheBloodOfJesusCleansesUsFromAllSin_IsADivineAct()
        {
            // "the blood of Jesus his Son cleanses us from all sin." (1 John 1:7)
            OracleHarness.DivineAct(OracleHarness.Witness("walking in the light, the blood of Jesus the Son cleanses the believers from all sin and brings them into true fellowship", new Atonement(), new Purity(), new Deliverance()));
        }

        [Fact]
        public void IfWeConfessOurSinsGodIsFaithfulToForgiveAndCleanse_IsADivineAct()
        {
            // "If we confess our sins, he is faithful and just to forgive us our sins and to cleanse us." (1 John 1:9)
            OracleHarness.DivineAct(OracleHarness.Witness("God, faithful and just, forgives the confessed sins of the believers and cleanses them from all unrighteousness", new Pardon(), new Fidelity(), new Righteousness(), new Purity()));
        }

        [Fact]
        public void ClaimingToBeWithoutSinIsSelfDeceptionThatRejectsTheTruth_IsSin()
        {
            // "If we say we have no sin, we deceive ourselves, and the truth is not in us." (1 John 1:8,10)
            OracleHarness.Sin(OracleHarness.Witness("the proud heart claims to have no sin, deceiving itself, making God a liar and refusing the truth", new Deceit(), new Pride(), new Blasphemy()));
        }

        [Fact]
        public void JesusChristTheRighteousIsOurAdvocateAndPropitiation_IsADivineAct()
        {
            // "we have an advocate ... Jesus Christ the righteous. He is the propitiation for our sins ... and for the whole world." (1 John 2:1-2)
            OracleHarness.DivineAct(OracleHarness.Witness("Jesus Christ the righteous stands as advocate before the Father and is Himself the propitiation for the sins of the whole world", new Atonement(), new Righteousness(), new SelfSacrifice(), new Deliverance()));
        }

        [Fact]
        public void WhoeverKeepsHisWordHasTheLoveOfGodPerfectedInHim_IsRighteous()
        {
            // "whoever keeps his word, in him truly the love of God is perfected ... walk in the same way in which he walked." (1 John 2:5-6)
            OracleHarness.Righteous(OracleHarness.Witness("the believer keeps God's word and walks as Christ walked, so that the love of God is perfected in him", new ObedienceToGod(), new Kindness(), new Fidelity()));
        }

        [Fact]
        public void WhoeverHatesHisBrotherWalksInDarknessAndBlindness_IsSin()
        {
            // "Whoever ... hates his brother is still in darkness ... and walks in the darkness, and does not know where he is going." (1 John 2:9-11)
            OracleHarness.Sin(OracleHarness.Witness("the one who claims the light yet hates his brother walks in darkness, blinded, and stumbles in discord", new Hatred(), new Discord(), new Deceit()));
        }

        [Fact]
        public void LovingTheWorldAndItsDesiresIsNotFromTheFather_IsSin()
        {
            // "the desires of the flesh and the desires of the eyes and pride of life ... is not from the Father." (1 John 2:15-17)
            OracleHarness.Sin(OracleHarness.Witness("the heart loves the world with the lust of the flesh, the lust of the eyes, and the proud boasting of life, which is not from the Father", new Lust(), new HaughtyEyes(), new Pride(), new Boasting()));
        }

        [Fact]
        public void TheAntichristIsTheLiarWhoDeniesTheFatherAndTheSon_IsSin()
        {
            // "Who is the liar but he who denies that Jesus is the Christ? This is the antichrist." (1 John 2:22-23)
            OracleHarness.Sin(OracleHarness.Witness("the antichrist lies and denies that Jesus is the Christ, rejecting both the Father and the Son", new Deceit(), new FalseWitness(), new Blasphemy(), new Rebellion()));
        }

        [Fact]
        public void TheFatherLavishesHisLoveToMakeUsChildrenOfGod_IsADivineAct()
        {
            // "See what kind of love the Father has given to us, that we should be called children of God." (1 John 3:1-2)
            OracleHarness.DivineAct(OracleHarness.Witness("the Father lavishes His love upon the believers, naming them children of God and promising they shall be like Him when He appears", new Compassion(), new Kindness(), new CovenantFaithfulness(), new PromiseFulfilled()));
        }

        [Fact]
        public void ChristAppearedToTakeAwaySinsAndDestroyTheWorksOfTheDevil_IsADivineAct()
        {
            // "he appeared to take away sins ... The reason the Son of God appeared was to destroy the works of the devil." (1 John 3:5,8)
            OracleHarness.DivineAct(OracleHarness.Witness("the sinless Son of God appears to take away sins and to destroy the works of the devil, delivering the children of God", new Atonement(), new Deliverance(), new Righteousness(), new Protection()));
        }

        [Fact]
        public void CainWhoWasOfTheEvilOneMurderedHisRighteousBrother_IsSin()
        {
            // "not like Cain, who was of the evil one and murdered his brother. And why did he murder him? Because ... his brother's were righteous." (1 John 3:12)
            Resolution resolution = OracleHarness.Witness("Cain, being of the evil one, hates his righteous brother and rises up to murder him out of envy", new Murder(), new Hatred(), new Envy(), new Bloodshed());
            OracleHarness.Sin(resolution);
            Assert.Equal(1.0, resolution.Disorder);
        }

        [Fact]
        public void WhoeverHatesHisBrotherIsAMurderer_IsSin()
        {
            // "Everyone who hates his brother is a murderer, and ... no murderer has eternal life abiding in him." (1 John 3:15)
            Resolution resolution = OracleHarness.Witness("the one who hates his brother is in truth a murderer, having no eternal life abiding in him", new Hatred(), new Murder(), new Malice());
            OracleHarness.Sin(resolution);
            Assert.Equal(1.0, resolution.Disorder);
        }

        [Fact]
        public void ChristLaidDownHisLifeAndSoWeLayDownOurLivesForTheBrothers_IsADivineAct()
        {
            // "he laid down his life for us, and we ought to lay down our lives for the brothers." (1 John 3:16)
            OracleHarness.DivineAct(OracleHarness.Witness("Christ lays down His life for the brethren, the very measure by which we know love", new SelfSacrifice(), new Compassion(), new Protection()));
        }

        [Fact]
        public void WeLoveNotInWordButInDeedByMeetingTheBrothersNeed_IsRighteous()
        {
            // "if anyone has the world's goods and sees his brother in need ... let us not love in word but in deed and in truth." (1 John 3:17-18)
            OracleHarness.Righteous(OracleHarness.Witness("the believer who has goods sees his brother in need and opens his heart, loving not in word only but in deed and in truth", new Compassion(), new Generosity(), new RejoicingInTruth(), new HonoringTheVulnerable()));
        }

        [Fact]
        public void GodSentHisOnlySonAsThePropitiationBecauseGodIsLove_IsADivineAct()
        {
            // "God is love ... God sent his only Son ... to be the propitiation for our sins." (1 John 4:8-10)
            OracleHarness.DivineAct(OracleHarness.Witness("because God is love, He sends His only Son into the world as the propitiation for our sins, that we might live through Him", new SelfSacrifice(), new Atonement(), new Compassion(), new RestoringLife()));
        }

        [Fact]
        public void IfGodSoLovedUsWeAlsoOughtToLoveOneAnother_IsRighteous()
        {
            // "if God so loved us, we also ought to love one another ... his love is perfected in us." (1 John 4:11-12)
            OracleHarness.Righteous(OracleHarness.Witness("since God so loved them, the believers also love one another, and God's love is perfected and abides in them", new Kindness(), new Compassion(), new Fidelity()));
        }

        [Fact]
        public void ClaimingToLoveGodWhileHatingABrotherIsTheWordOfALiar_IsSin()
        {
            // "If anyone says, 'I love God,' and hates his brother, he is a liar." (1 John 4:20)
            OracleHarness.Sin(OracleHarness.Witness("the man claims to love God whom he has not seen while he hates the brother whom he has seen, and so is shown to be a liar", new Hatred(), new Deceit(), new FalseWitness()));
        }

        [Fact]
        public void WhoeverIsBornOfGodOvercomesTheWorldThroughFaith_IsRighteous()
        {
            // "everyone who has been born of God overcomes the world. And this is the victory ... our faith." (1 John 5:4-5)
            OracleHarness.Righteous(OracleHarness.Witness("the one born of God overcomes the world by believing that Jesus is the Son of God, and this faith is the victory", new CourageousFaith(), new Fidelity(), new Trust(), new Endurance()));
        }

        [Fact]
        public void GodGivesUsEternalLifeWhichIsInHisSon_IsADivineAct()
        {
            // "God gave us eternal life, and this life is in his Son. Whoever has the Son has life." (1 John 5:11-13)
            OracleHarness.DivineAct(OracleHarness.Witness("God testifies and gives the believers eternal life, and this life is in His Son, so that whoever has the Son has life", new RestoringLife(), new PromiseFulfilled(), new Trustworthiness(), new Goodness()));
        }
    }
}
