// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-05-28 10:42:10
// Edited by Alexander Fields https://www.alexanderfields.me 2026-05-28 12:05:18
//Stewarded by Alexander Fields

namespace AnointedAutomation.Objects.Concepts
{
    /// <summary>
    /// Agape: the perfect, self-giving love described in 1 Corinthians 13:4-8 and revealed at the
    /// cross. Every virtue is present, every vice absent, and it is sacrificial.
    ///
    /// <para>
    /// Its repertoire is a behavior tree of principle-based responses — forgive the wrong, meet the
    /// need, mourn with the grieving, rejoice with the glad, love the enemy — each keyed to facts in
    /// the <see cref="Situation"/>. These are principles, not scripted scenes: they fire for any
    /// situation that matches, including ones never named in Scripture. When nothing else matches,
    /// love still acts — patiently and kindly (1 Corinthians 13:4).
    /// </para>
    /// </summary>
    public class Agape : Love
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Agape"/> class conformed to the full
        /// Biblical standard of love.
        /// </summary>
        public Agape() : this(null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Agape"/> class directed from one party
        /// toward another, conformed to the full Biblical standard of love.
        /// </summary>
        /// <param name="lover">The one who loves.</param>
        /// <param name="beloved">The one (or thing) who is loved.</param>
        public Agape(string lover, string beloved) : base(lover, beloved)
        {
            patient = true;
            kind = true;
            envious = false;
            boastful = false;
            proud = false;
            rude = false;
            selfSeeking = false;
            easilyAngered = false;
            keepsRecordOfWrongs = false;
            delightsInEvil = false;
            rejoicesWithTruth = true;
            protects = true;
            trusts = true;
            hopeful = true;
            perseveres = true;
            neverFails = true;
            sacrificial = true;
        }

        /// <summary>
        /// Builds agape's repertoire as a behavior tree: the highest-priority applicable response
        /// wins, and a patient, kind deed catches every situation no branch matched.
        /// </summary>
        /// <returns>The root of agape's decision tree.</returns>
        protected override BehaviorNode BuildBehavior()
        {
            return new Selector(
                new Sequence(Condition.For(new Grievance()), new Deed(Forgive())),
                new Sequence(Condition.For(new Enmity()).And(Condition.For(new Hunger()).Or(new Thirst())), new Deed(FeedYourEnemy())),
                new Sequence(Condition.For(new Hunger()), new Deed(Feed())),
                new Sequence(Condition.For(new Thirst()), new Deed(GiveDrink())),
                new Sequence(Condition.For(new Estrangement()), new Deed(Welcome())),
                new Sequence(Condition.For(new Nakedness()), new Deed(Clothe())),
                new Sequence(Condition.For(new Sickness()), new Deed(CareForSick())),
                new Sequence(Condition.For(new Imprisonment()), new Deed(Visit())),
                new Sequence(Condition.For(new Need()).And(new Means()), new Deed(MeetTheNeed())),
                new Sequence(Condition.For(new Grief()), new Deed(MournWith())),
                new Sequence(Condition.For(new Gladness()), new Deed(RejoiceWith())),
                new Sequence(Condition.For(new Enmity()), new Deed(LoveEnemy())),
                new Deed(BePatientAndKind()));
        }

        private static LoveAction Forgive()
        {
            return new LoveAction(
                true,
                "Forgive — keep no record of the wrong and release the debt.",
                "kind; keeps no record of wrongs; not easily angered",
                "Colossians 3:13; 1 Corinthians 13:5",
                "Forgive, as the Lord forgave you.");
        }

        private static LoveAction MeetTheNeed()
        {
            return new LoveAction(
                true,
                "Go to them and meet the need at your own cost.",
                "kind; not self-seeking; protects; perseveres",
                "Luke 10:33-35",
                "Go and do likewise.");
        }

        private static LoveAction FeedYourEnemy()
        {
            return new LoveAction(
                true,
                "Feed your enemy and give them drink; overcome evil with good.",
                "kind; not self-seeking; keeps no record of wrongs",
                "Romans 12:20-21 (Proverbs 25:21)",
                "Do not be overcome by evil, but overcome evil with good.");
        }

        private static LoveAction Feed()
        {
            return new LoveAction(
                true,
                "Give them something to eat.",
                "kind; not self-seeking",
                "Matthew 25:35",
                "Whatever you did for one of the least of these, you did for Me.");
        }

        private static LoveAction GiveDrink()
        {
            return new LoveAction(
                true,
                "Give them something to drink.",
                "kind; not self-seeking",
                "Matthew 25:35",
                "Whatever you did for one of the least of these, you did for Me.");
        }

        private static LoveAction Welcome()
        {
            return new LoveAction(
                true,
                "Invite them in and make them welcome.",
                "kind; not self-seeking; protects",
                "Matthew 25:35",
                "Whatever you did for one of the least of these, you did for Me.");
        }

        private static LoveAction Clothe()
        {
            return new LoveAction(
                true,
                "Clothe them and cover their need.",
                "kind; not self-seeking",
                "Matthew 25:36",
                "Whatever you did for one of the least of these, you did for Me.");
        }

        private static LoveAction CareForSick()
        {
            return new LoveAction(
                true,
                "Look after them and tend their sickness.",
                "kind; patient; protects; perseveres",
                "Matthew 25:36",
                "Whatever you did for one of the least of these, you did for Me.");
        }

        private static LoveAction Visit()
        {
            return new LoveAction(
                true,
                "Go to them in prison and visit them.",
                "kind; not self-seeking; perseveres",
                "Matthew 25:36",
                "Whatever you did for one of the least of these, you did for Me.");
        }

        private static LoveAction MournWith()
        {
            return new LoveAction(
                true,
                "Mourn with them; sit in their sorrow and bear it alongside them.",
                "kind; patient; protects",
                "Romans 12:15",
                null);
        }

        private static LoveAction RejoiceWith()
        {
            return new LoveAction(
                true,
                "Rejoice with them; delight in their good without a trace of envy.",
                "kind; does not envy; rejoices with the truth",
                "Romans 12:15; 1 Corinthians 13:6",
                null);
        }

        private static LoveAction LoveEnemy()
        {
            return new LoveAction(
                true,
                "Love them and pray for them; return good for evil.",
                "kind; not self-seeking; keeps no record of wrongs",
                "Matthew 5:44",
                "Love your enemies and pray for those who persecute you.");
        }

        private static LoveAction BePatientAndKind()
        {
            return new LoveAction(
                true,
                "Be patient and kind — bear with them and seek their good in whatever this is.",
                "patient; kind",
                "1 Corinthians 13:4",
                null);
        }
    }
}
