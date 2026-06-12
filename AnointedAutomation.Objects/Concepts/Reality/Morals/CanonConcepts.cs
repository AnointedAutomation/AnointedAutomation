// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

namespace AnointedAutomation.Objects.Concepts
{
    // The remaining moral concepts Scripture names in its explicit catalogues: the Ten Commandments
    // (Exodus 20), the works of the flesh and the fruit of the Spirit (Galatians 5:19-23), the seven
    // the LORD hates (Proverbs 6:16-19), the vice lists (Romans 1:29-31; Colossians 3:5-9;
    // 1 Corinthians 6:9-10; Mark 7:21-22; 2 Timothy 3:2-5), the works of mercy (Matthew 25:35-36),
    // the Beatitudes (Matthew 5), and the love attributes (1 Corinthians 13:4-8). Each is first-class
    // code with its own Scripture, the facets it bears on, and (for sins) its gravity. Concepts
    // already represented elsewhere are reused, not duplicated.

    // ---- Vices: works of the flesh, the seven the LORD hates, and the vice lists ----

    /// <summary>Sexual immorality: porneia, the works of the flesh. "The acts of the flesh ... sexual immorality." (Galatians 5:19)</summary>
    public sealed class SexualImmorality : MoralConcept
    {
        public override string Name => "SexualImmorality";
        public override string Scripture => "Galatians 5:19";
        public override Gravity Gravity => Gravity.Grave;
        public override bool Violates(DivineAttribute facet) => facet is Faithfulness || facet is LoveFacet;
    }

    /// <summary>Impurity: moral uncleanness. "The acts of the flesh ... impurity." (Galatians 5:19)</summary>
    public sealed class Impurity : MoralConcept
    {
        public override string Name => "Impurity";
        public override string Scripture => "Galatians 5:19";
        public override Gravity Gravity => Gravity.Serious;
        public override bool Violates(DivineAttribute facet) => facet is Faithfulness;
    }

    /// <summary>Debauchery: sensual excess and licentiousness. "The acts of the flesh ... debauchery." (Galatians 5:19)</summary>
    public sealed class Debauchery : MoralConcept
    {
        public override string Name => "Debauchery";
        public override string Scripture => "Galatians 5:19";
        public override Gravity Gravity => Gravity.Serious;
        public override bool Violates(DivineAttribute facet) => facet is Faithfulness;
    }

    /// <summary>Sorcery: pharmakeia, traffic with dark powers. "The acts of the flesh ... witchcraft." (Galatians 5:20)</summary>
    public sealed class Sorcery : MoralConcept
    {
        public override string Name => "Sorcery";
        public override string Scripture => "Galatians 5:20";
        public override Gravity Gravity => Gravity.Grave;
        public override bool Violates(DivineAttribute facet) => facet is Faithfulness || facet is LoveFacet;
    }

    /// <summary>Hatred: enmity toward neighbour. "The acts of the flesh ... hatred." (Galatians 5:20)</summary>
    public sealed class Hatred : MoralConcept
    {
        public override string Name => "Hatred";
        public override string Scripture => "Galatians 5:20";
        public override Gravity Gravity => Gravity.Grave;
        public override bool Violates(DivineAttribute facet) => facet is LoveFacet;
    }

    /// <summary>Discord: stirring up strife and dissension. "The acts of the flesh ... discord." (Galatians 5:20; Proverbs 6:19)</summary>
    public sealed class Discord : MoralConcept
    {
        public override string Name => "Discord";
        public override string Scripture => "Galatians 5:20";
        public override Gravity Gravity => Gravity.Serious;
        public override bool Violates(DivineAttribute facet) => facet is Faithfulness || facet is LoveFacet;
    }

    /// <summary>Jealousy: covetous rivalry of the heart. "The acts of the flesh ... jealousy." (Galatians 5:20)</summary>
    public sealed class Jealousy : MoralConcept
    {
        public override string Name => "Jealousy";
        public override string Scripture => "Galatians 5:20";
        public override Gravity Gravity => Gravity.Minor;
        public override bool Violates(DivineAttribute facet) => facet is LoveFacet;
    }

    /// <summary>Fits of rage: uncontrolled outbursts of wrath. "The acts of the flesh ... fits of rage." (Galatians 5:20)</summary>
    public sealed class FitsOfRage : MoralConcept
    {
        public override string Name => "FitsOfRage";
        public override string Scripture => "Galatians 5:20";
        public override Gravity Gravity => Gravity.Serious;
        public override bool Violates(DivineAttribute facet) => facet is LoveFacet || facet is Faithfulness;
    }

    /// <summary>Selfish ambition: scheming for self at others' cost. "The acts of the flesh ... selfish ambition." (Galatians 5:20)</summary>
    public sealed class SelfishAmbition : MoralConcept
    {
        public override string Name => "SelfishAmbition";
        public override string Scripture => "Galatians 5:20";
        public override Gravity Gravity => Gravity.Serious;
        public override bool Violates(DivineAttribute facet) => facet is LoveFacet || facet is Faithfulness;
    }

    /// <summary>Faction: party spirit that splits the body. "The acts of the flesh ... factions." (Galatians 5:20)</summary>
    public sealed class Faction : MoralConcept
    {
        public override string Name => "Faction";
        public override string Scripture => "Galatians 5:20";
        public override Gravity Gravity => Gravity.Serious;
        public override bool Violates(DivineAttribute facet) => facet is Faithfulness || facet is LoveFacet;
    }

    /// <summary>Drunkenness: surrender of the self to drink. "The acts of the flesh ... drunkenness." (Galatians 5:21)</summary>
    public sealed class Drunkenness : MoralConcept
    {
        public override string Name => "Drunkenness";
        public override string Scripture => "Galatians 5:21";
        public override Gravity Gravity => Gravity.Serious;
        public override bool Violates(DivineAttribute facet) => facet is Faithfulness;
    }

    /// <summary>Carousing: revelry and wild dissipation. "The acts of the flesh ... orgies." (Galatians 5:21)</summary>
    public sealed class Carousing : MoralConcept
    {
        public override string Name => "Carousing";
        public override string Scripture => "Galatians 5:21";
        public override Gravity Gravity => Gravity.Serious;
        public override bool Violates(DivineAttribute facet) => facet is Faithfulness;
    }

    /// <summary>Gluttony: the belly made a god. "Their god is their stomach." (Philippians 3:19; Proverbs 23:21)</summary>
    public sealed class Gluttony : MoralConcept
    {
        public override string Name => "Gluttony";
        public override string Scripture => "Proverbs 23:21";
        public override Gravity Gravity => Gravity.Minor;
        public override bool Violates(DivineAttribute facet) => facet is Faithfulness;
    }

    /// <summary>Slander: tearing down another with the tongue. "Slanderers ... will not inherit the kingdom." (Romans 1:30; 1 Corinthians 6:10)</summary>
    public sealed class Slander : MoralConcept
    {
        public override string Name => "Slander";
        public override string Scripture => "Romans 1:30";
        public override Gravity Gravity => Gravity.Serious;
        public override bool Violates(DivineAttribute facet) => facet is LoveFacet || facet is Faithfulness;
    }

    /// <summary>Gossip: whispering to wound. "Gossips, slanderers, God-haters." (Romans 1:29)</summary>
    public sealed class Gossip : MoralConcept
    {
        public override string Name => "Gossip";
        public override string Scripture => "Romans 1:29";
        public override Gravity Gravity => Gravity.Serious;
        public override bool Violates(DivineAttribute facet) => facet is LoveFacet;
    }

    /// <summary>Malice: the will bent on another's harm. "Full of envy, murder, strife, deceit and malice." (Romans 1:29)</summary>
    public sealed class Malice : MoralConcept
    {
        public override string Name => "Malice";
        public override string Scripture => "Romans 1:29";
        public override Gravity Gravity => Gravity.Serious;
        public override bool Violates(DivineAttribute facet) => facet is LoveFacet;
    }

    /// <summary>Sowing discord among brothers: the seventh the LORD hates. "A person who stirs up conflict in the community." (Proverbs 6:19)</summary>
    public sealed class SowingDiscord : MoralConcept
    {
        public override string Name => "SowingDiscord";
        public override string Scripture => "Proverbs 6:19";
        public override Gravity Gravity => Gravity.Serious;
        public override bool Violates(DivineAttribute facet) => facet is Faithfulness || facet is LoveFacet;
    }

    /// <summary>Haughty eyes: the proud look the LORD hates. "Haughty eyes, a lying tongue." (Proverbs 6:17)</summary>
    public sealed class HaughtyEyes : MoralConcept
    {
        public override string Name => "HaughtyEyes";
        public override string Scripture => "Proverbs 6:17";
        public override Gravity Gravity => Gravity.Serious;
        public override bool Violates(DivineAttribute facet) => facet is LoveFacet || facet is Faithfulness;
    }

    /// <summary>A scheming heart: a heart that devises wicked plans, which the LORD hates. (Proverbs 6:18)</summary>
    public sealed class WickedScheming : MoralConcept
    {
        public override string Name => "WickedScheming";
        public override string Scripture => "Proverbs 6:18";
        public override Gravity Gravity => Gravity.Serious;
        public override bool Violates(DivineAttribute facet) => facet is Justice || facet is Faithfulness;
    }

    /// <summary>Feet quick to rush into evil, which the LORD hates. "Feet that are quick to rush into evil." (Proverbs 6:18)</summary>
    public sealed class QuickToEvil : MoralConcept
    {
        public override string Name => "QuickToEvil";
        public override string Scripture => "Proverbs 6:18";
        public override Gravity Gravity => Gravity.Serious;
        public override bool Violates(DivineAttribute facet) => facet is Faithfulness;
    }

    /// <summary>False witness: a lying witness who pours out lies, which the LORD hates. "A false witness who pours out lies." (Proverbs 6:19; Exodus 20:16)</summary>
    public sealed class FalseWitness : MoralConcept
    {
        public override string Name => "FalseWitness";
        public override string Scripture => "Exodus 20:16";
        public override Gravity Gravity => Gravity.Serious;
        public override bool Violates(DivineAttribute facet) => facet is Justice || facet is Faithfulness;
    }

    /// <summary>Covetousness: craving what belongs to a neighbour. "You shall not covet." (Exodus 20:17)</summary>
    public sealed class Covetousness : MoralConcept
    {
        public override string Name => "Covetousness";
        public override string Scripture => "Exodus 20:17";
        public override Gravity Gravity => Gravity.Serious;
        public override bool Violates(DivineAttribute facet) => facet is LoveFacet || facet is Justice;
    }

    /// <summary>Dishonoring parents: refusing the honour the Law commands. "Honor your father and your mother." (Exodus 20:12)</summary>
    public sealed class DishonoringParents : MoralConcept
    {
        public override string Name => "DishonoringParents";
        public override string Scripture => "Exodus 20:12";
        public override Gravity Gravity => Gravity.Serious;
        public override bool Violates(DivineAttribute facet) => facet is Faithfulness || facet is LoveFacet;
    }

    /// <summary>Sabbath breaking: profaning the day God hallowed. "Remember the Sabbath day, to keep it holy." (Exodus 20:8)</summary>
    public sealed class SabbathBreaking : MoralConcept
    {
        public override string Name => "SabbathBreaking";
        public override string Scripture => "Exodus 20:8";
        public override Gravity Gravity => Gravity.Serious;
        public override bool Violates(DivineAttribute facet) => facet is Faithfulness;
    }

    /// <summary>Misusing the name of God: taking the LORD's name in vain. "You shall not misuse the name of the LORD your God." (Exodus 20:7)</summary>
    public sealed class MisusingGodsName : MoralConcept
    {
        public override string Name => "MisusingGodsName";
        public override string Scripture => "Exodus 20:7";
        public override Gravity Gravity => Gravity.Grave;
        public override bool Violates(DivineAttribute facet) => facet is Faithfulness;
    }

    /// <summary>Making graven images: bowing to the work of one's hands. "You shall not make for yourself an image." (Exodus 20:4)</summary>
    public sealed class GravenImage : MoralConcept
    {
        public override string Name => "GravenImage";
        public override string Scripture => "Exodus 20:4";
        public override Gravity Gravity => Gravity.Grave;
        public override bool Violates(DivineAttribute facet) => facet is Faithfulness || facet is LoveFacet;
    }

    /// <summary>Sloth: the sluggard's refusal of his charge. "A little sleep, a little slumber ... and poverty will come." (Proverbs 6:10-11; 2 Thessalonians 3:10)</summary>
    public sealed class Sloth : MoralConcept
    {
        public override string Name => "Sloth";
        public override string Scripture => "Proverbs 6:10-11";
        public override Gravity Gravity => Gravity.Minor;
        public override bool Violates(DivineAttribute facet) => facet is Faithfulness;
    }

    /// <summary>Ingratitude: the unthankful heart of the last days. "Ungrateful, unholy." (2 Timothy 3:2)</summary>
    public sealed class Ingratitude : MoralConcept
    {
        public override string Name => "Ingratitude";
        public override string Scripture => "2 Timothy 3:2";
        public override Gravity Gravity => Gravity.Minor;
        public override bool Violates(DivineAttribute facet) => facet is LoveFacet || facet is Faithfulness;
    }

    /// <summary>Heartlessness: without natural affection. "Without love, unforgiving, slanderous." (2 Timothy 3:3; Romans 1:31)</summary>
    public sealed class Heartlessness : MoralConcept
    {
        public override string Name => "Heartlessness";
        public override string Scripture => "Romans 1:31";
        public override Gravity Gravity => Gravity.Serious;
        public override bool Violates(DivineAttribute facet) => facet is LoveFacet || facet is Mercy;
    }

    /// <summary>Covenant-breaking: untrustworthy, breaking faith and pledge. "Faithless, heartless, ruthless." (Romans 1:31)</summary>
    public sealed class CovenantBreaking : MoralConcept
    {
        public override string Name => "CovenantBreaking";
        public override string Scripture => "Romans 1:31";
        public override Gravity Gravity => Gravity.Grave;
        public override bool Violates(DivineAttribute facet) => facet is Faithfulness;
    }

    /// <summary>Insolence: the violent, arrogant despiser of others. "Insolent, arrogant and boastful." (Romans 1:30)</summary>
    public sealed class Insolence : MoralConcept
    {
        public override string Name => "Insolence";
        public override string Scripture => "Romans 1:30";
        public override Gravity Gravity => Gravity.Serious;
        public override bool Violates(DivineAttribute facet) => facet is LoveFacet || facet is Faithfulness;
    }

    /// <summary>Disobedience to parents: the rebellious child of the vice lists. "Disobedient to their parents." (Romans 1:30; 2 Timothy 3:2)</summary>
    public sealed class DisobedienceToParents : MoralConcept
    {
        public override string Name => "DisobedienceToParents";
        public override string Scripture => "Romans 1:30";
        public override Gravity Gravity => Gravity.Serious;
        public override bool Violates(DivineAttribute facet) => facet is Faithfulness || facet is LoveFacet;
    }

    /// <summary>Filthy and foul language out of the mouth. "Filthy language from your lips." (Colossians 3:8)</summary>
    public sealed class FilthyLanguage : MoralConcept
    {
        public override string Name => "FilthyLanguage";
        public override string Scripture => "Colossians 3:8";
        public override Gravity Gravity => Gravity.Minor;
        public override bool Violates(DivineAttribute facet) => facet is Faithfulness || facet is LoveFacet;
    }

    /// <summary>Lust: the eye and heart that grasp in desire. "Anyone who looks ... lustfully." (Matthew 5:28)</summary>
    public sealed class Lust : MoralConcept
    {
        public override string Name => "Lust";
        public override string Scripture => "Matthew 5:28";
        public override Gravity Gravity => Gravity.Serious;
        public override bool Violates(DivineAttribute facet) => facet is Faithfulness || facet is LoveFacet;
    }

    // ---- Virtues: the fruit of the Spirit, the works of mercy, the Beatitudes ----

    /// <summary>Joy: gladness rooted in God, the fruit of the Spirit. "The fruit of the Spirit is love, joy, peace." (Galatians 5:22)</summary>
    public sealed class Joy : MoralConcept
    {
        public override string Name => "Joy";
        public override string Scripture => "Galatians 5:22";
        public override bool Upholds(DivineAttribute facet) => facet is LoveFacet;
    }

    /// <summary>Peace: the wholeness and concord God gives, the fruit of the Spirit. "The fruit of the Spirit is ... peace." (Galatians 5:22)</summary>
    public sealed class Peace : MoralConcept
    {
        public override string Name => "Peace";
        public override string Scripture => "Galatians 5:22";
        public override bool Upholds(DivineAttribute facet) => facet is Faithfulness || facet is LoveFacet;
    }

    /// <summary>Goodness: active uprightness toward others, the fruit of the Spirit. "The fruit of the Spirit is ... goodness." (Galatians 5:22)</summary>
    public sealed class Goodness : MoralConcept
    {
        public override string Name => "Goodness";
        public override string Scripture => "Galatians 5:22";
        public override bool Upholds(DivineAttribute facet) => facet is LoveFacet || facet is Justice;
    }

    /// <summary>Fidelity: steadfast human reliability, the fruit of the Spirit, reflecting God's own
    /// faithfulness. "The fruit of the Spirit is ... faithfulness." (Galatians 5:22)</summary>
    public sealed class Fidelity : MoralConcept
    {
        public override string Name => "Fidelity";
        public override string Scripture => "Galatians 5:22";
        public override bool Upholds(DivineAttribute facet) => facet is Faithfulness;
    }

    /// <summary>Gentleness: a meek and tender strength, the fruit of the Spirit. "The fruit of the Spirit is ... gentleness." (Galatians 5:23)</summary>
    public sealed class Gentleness : MoralConcept
    {
        public override string Name => "Gentleness";
        public override string Scripture => "Galatians 5:23";
        public override bool Upholds(DivineAttribute facet) => facet is LoveFacet || facet is Mercy;
    }

    /// <summary>Self-control: mastery over the appetites, the fruit of the Spirit. "The fruit of the Spirit is ... self-control." (Galatians 5:23)</summary>
    public sealed class SelfControl : MoralConcept
    {
        public override string Name => "SelfControl";
        public override string Scripture => "Galatians 5:23";
        public override bool Upholds(DivineAttribute facet) => facet is Faithfulness;
    }

    /// <summary>Feeding the hungry: a work of mercy. "I was hungry and you gave me something to eat." (Matthew 25:35)</summary>
    public sealed class FeedingTheHungry : MoralConcept
    {
        public override string Name => "FeedingTheHungry";
        public override string Scripture => "Matthew 25:35";
        public override bool Upholds(DivineAttribute facet) => facet is LoveFacet || facet is Mercy;
    }

    /// <summary>Giving drink to the thirsty: a work of mercy. "I was thirsty and you gave me something to drink." (Matthew 25:35)</summary>
    public sealed class GivingDrink : MoralConcept
    {
        public override string Name => "GivingDrink";
        public override string Scripture => "Matthew 25:35";
        public override bool Upholds(DivineAttribute facet) => facet is LoveFacet || facet is Mercy;
    }

    /// <summary>Clothing the naked: a work of mercy. "I needed clothes and you clothed me." (Matthew 25:36)</summary>
    public sealed class ClothingTheNaked : MoralConcept
    {
        public override string Name => "ClothingTheNaked";
        public override string Scripture => "Matthew 25:36";
        public override bool Upholds(DivineAttribute facet) => facet is LoveFacet || facet is Mercy;
    }

    /// <summary>Caring for the sick: a work of mercy. "I was sick and you looked after me." (Matthew 25:36)</summary>
    public sealed class CaringForTheSick : MoralConcept
    {
        public override string Name => "CaringForTheSick";
        public override string Scripture => "Matthew 25:36";
        public override bool Upholds(DivineAttribute facet) => facet is LoveFacet || facet is Mercy;
    }

    /// <summary>Visiting the prisoner: a work of mercy. "I was in prison and you came to visit me." (Matthew 25:36)</summary>
    public sealed class VisitingThePrisoner : MoralConcept
    {
        public override string Name => "VisitingThePrisoner";
        public override string Scripture => "Matthew 25:36";
        public override bool Upholds(DivineAttribute facet) => facet is LoveFacet || facet is Mercy;
    }

    /// <summary>Poverty of spirit: knowing one's need of God, the first Beatitude. "Blessed are the poor in spirit." (Matthew 5:3)</summary>
    public sealed class PovertyOfSpirit : MoralConcept
    {
        public override string Name => "PovertyOfSpirit";
        public override string Scripture => "Matthew 5:3";
        public override bool Upholds(DivineAttribute facet) => facet is Faithfulness || facet is LoveFacet;
    }

    /// <summary>Mourning: grieving over sin and loss, the blessed who shall be comforted. "Blessed are those who mourn." (Matthew 5:4)</summary>
    public sealed class Mourning : MoralConcept
    {
        public override string Name => "Mourning";
        public override string Scripture => "Matthew 5:4";
        public override bool Upholds(DivineAttribute facet) => facet is Mercy || facet is LoveFacet;
    }

    /// <summary>Meekness: strength surrendered to God, the meek who inherit the earth. "Blessed are the meek." (Matthew 5:5)</summary>
    public sealed class Meekness : MoralConcept
    {
        public override string Name => "Meekness";
        public override string Scripture => "Matthew 5:5";
        public override bool Upholds(DivineAttribute facet) => facet is LoveFacet || facet is Faithfulness;
    }

    /// <summary>Hungering for righteousness: longing to be made right, the Beatitude. "Blessed are those who hunger and thirst for righteousness." (Matthew 5:6)</summary>
    public sealed class HungerForRighteousness : MoralConcept
    {
        public override string Name => "HungerForRighteousness";
        public override string Scripture => "Matthew 5:6";
        public override bool Upholds(DivineAttribute facet) => facet is Justice || facet is Faithfulness;
    }

    /// <summary>Purity of heart: the single, undivided heart that sees God. "Blessed are the pure in heart." (Matthew 5:8)</summary>
    public sealed class Purity : MoralConcept
    {
        public override string Name => "Purity";
        public override string Scripture => "Matthew 5:8";
        public override bool Upholds(DivineAttribute facet) => facet is Faithfulness || facet is LoveFacet;
    }

    /// <summary>Endurance under persecution: standing firm for righteousness' sake, the Beatitude. "Blessed are those who are persecuted because of righteousness." (Matthew 5:10; 1 Corinthians 13:7)</summary>
    public sealed class Endurance : MoralConcept
    {
        public override string Name => "Endurance";
        public override string Scripture => "Matthew 5:10";
        public override bool Upholds(DivineAttribute facet) => facet is Faithfulness || facet is LoveFacet;
    }
}
