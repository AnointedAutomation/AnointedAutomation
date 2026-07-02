// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Concepts;

namespace AnointedAutomation.Concepts.Tests.Canon
{
    // Jonah narrative oracle: the engine must return the verdict Scripture gives at each step.
    public class JonahNarrativeTests
    {
        [Fact]
        public void GodCommandsJonahToCryAgainstNineveh_IsADivineAct()
        {
            // "Arise, go to Nineveh, that great city, and cry against it." (Jonah 1:1-2)
            // God sends His prophet to a wicked Gentile city, in mercy giving them warning before judgment.
            OracleHarness.DivineAct(OracleHarness.Witness("God commands Jonah to go and cry against Nineveh",
                new TeachingTruth(), new Righteousness(), new Compassion(),
                new JustRecompense(), new RejoicingInTruth()));
        }

        [Fact]
        public void JonahFleesFromTheLordTowardTarshish_IsSin()
        {
            // "But Jonah rose up to flee unto Tarshish from the presence of the LORD." (Jonah 1:3)
            // Jonah disobeys the clear command of God, rebelling and running from his calling.
            Resolution r = OracleHarness.Witness("Jonah flees from the presence of the LORD toward Tarshish",
                new Disobedience(), new Rebellion(), new SelfSeeking());
            OracleHarness.Sin(r);
        }

        [Fact]
        public void GodHurlsAGreatWindAndTempestUponTheSea_IsADivineAct()
        {
            // "But the LORD sent out a great wind into the sea, and there was a mighty tempest." (Jonah 1:4)
            // God in righteous sovereignty disciplines His fleeing prophet, ordering even the storm.
            OracleHarness.DivineAct(OracleHarness.Witness("God hurls a great wind and mighty tempest upon the sea to halt Jonah",
                new Righteousness(), new JustRecompense(), new CreationOrder(),
                new TeachingTruth(), new CovenantFaithfulness()));
        }

        [Fact]
        public void JonahConfessesHeFearsTheLordGodOfHeaven_IsRighteous()
        {
            // "I am an Hebrew; and I fear the LORD, the God of heaven, which hath made the sea." (Jonah 1:9)
            // Jonah testifies truly to the sailors of the one Creator God whom he worships.
            OracleHarness.Righteous(OracleHarness.Witness("Jonah confesses to the sailors that he fears the LORD, God of heaven who made the sea",
                new TeachingTruth(), new Worship(), new RejoicingInTruth(),
                new Trustworthiness(), new Fidelity()));
        }

        [Fact]
        public void JonahAsksToBeCastIntoTheSeaToSpareTheSailors_IsRighteous()
        {
            // "Take me up, and cast me forth into the sea ... for my sake this great tempest is upon you." (Jonah 1:12)
            // Owning his guilt, Jonah offers his own life to save the imperiled mariners.
            OracleHarness.Righteous(OracleHarness.Witness("Jonah tells the sailors to cast him into the sea so they may be spared",
                new SelfSacrifice(), new Repentance(), new Protection(),
                new Humility(), new HonoringTheVulnerable()));
        }

        [Fact]
        public void GodPreparesAGreatFishToSwallowAndPreserveJonah_IsADivineAct()
        {
            // "Now the LORD had prepared a great fish to swallow up Jonah." (Jonah 1:17)
            // The judgment of the deep becomes the means of mercy; God preserves the prophet alive.
            OracleHarness.DivineAct(OracleHarness.Witness("God prepares a great fish to swallow Jonah and preserve him from the deep",
                new Deliverance(), new Protection(), new Compassion(),
                new RestoringLife(), new CreationOrder()));
        }

        [Fact]
        public void JonahPraysAndGivesThanksFromTheBellyOfTheFish_IsRighteous()
        {
            // "I cried by reason of mine affliction unto the LORD ... with the voice of thanksgiving." (Jonah 2:1-9)
            // From the depths Jonah turns back to God in repentant prayer, worship, and trust.
            OracleHarness.Righteous(OracleHarness.Witness("Jonah prays from the fish's belly with repentance, thanksgiving, and trust in the LORD",
                new Repentance(), new Worship(), new Trust(),
                new Humility(), new PromiseFulfilled()));
        }

        [Fact]
        public void GodDeliversJonahOntoDryLand_IsADivineAct()
        {
            // "And the LORD spake unto the fish, and it vomited out Jonah upon the dry land." (Jonah 2:10)
            // God answers the prophet's repentance, raising him out of the deep and restoring him to life and mission.
            OracleHarness.DivineAct(OracleHarness.Witness("God commands the fish to deliver Jonah safely onto the dry land",
                new Deliverance(), new RestoringLife(), new Compassion(),
                new Forgiveness(), new PromiseFulfilled()));
        }

        [Fact]
        public void JonahObeysAndProclaimsGodsWordToNineveh_IsRighteous()
        {
            // "So Jonah arose, and went unto Nineveh, according to the word of the LORD ... Yet forty days." (Jonah 3:1-4)
            // The second time, Jonah obeys and faithfully delivers the warning God gave him.
            OracleHarness.Righteous(OracleHarness.Witness("Jonah obeys the second call and proclaims God's warning throughout Nineveh",
                new ObedienceToGod(), new TeachingTruth(), new Fidelity(),
                new CourageousFaith(), new Trustworthiness()));
        }

        [Fact]
        public void NinevehRepentsInFastingAndSackcloth_IsRighteous()
        {
            // "So the people of Nineveh believed God ... and put on sackcloth ... and cried mightily unto God." (Jonah 3:5-8)
            // The whole city, from king to commoner, humbles itself, fasts, and turns from its evil way.
            OracleHarness.Righteous(OracleHarness.Witness("Nineveh believes God, fasts in sackcloth, and turns from its evil way",
                new Repentance(), new Humility(), new Mourning(),
                new Worship(), new HungerForRighteousness()));
        }

        [Fact]
        public void GodRelentsAndSparesRepentantNineveh_IsADivineAct()
        {
            // "And God saw their works, that they turned from their evil way; and God repented of the evil ... and did it not." (Jonah 3:10)
            // Seeing genuine repentance, God in mercy withholds the threatened judgment and spares the city.
            OracleHarness.DivineAct(OracleHarness.Witness("God sees Nineveh's repentance and relents, sparing the city from judgment",
                new Compassion(), new Forgiveness(), new Pardon(),
                new Deliverance(), new Kindness()));
        }

        [Fact]
        public void JonahIsAngryAndResentsGodsMercyToNineveh_IsSin()
        {
            // "But it displeased Jonah exceedingly, and he was very angry." (Jonah 4:1-3)
            // Jonah begrudges the grace shown to his enemies, nursing rage and a grudge against God's compassion.
            Resolution r = OracleHarness.Witness("Jonah is exceedingly angry and resents God's mercy toward Nineveh",
                new FitsOfRage(), new Grudge(), new Hatred(), new SelfSeeking());
            OracleHarness.Sin(r);
        }

        [Fact]
        public void GodGentlyTeachesJonahHisCompassionForNineveh_IsADivineAct()
        {
            // "Should not I spare Nineveh, that great city, wherein are more than sixscore thousand persons?" (Jonah 4:6-11)
            // God appoints the gourd, the worm, and the wind, then teaches Jonah the breadth of His pity.
            OracleHarness.DivineAct(OracleHarness.Witness("God gently reasons with Jonah by the gourd, teaching His compassion for Nineveh",
                new TeachingTruth(), new Compassion(), new Patience(),
                new Gentleness(), new HonoringTheVulnerable()));
        }
    }
}
