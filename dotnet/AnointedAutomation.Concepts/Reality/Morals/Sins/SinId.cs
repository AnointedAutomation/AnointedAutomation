// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields

namespace AnointedAutomation.Concepts
{
    /// <summary>
    /// Stable numeric id of every catalogued sin. The value is the entry's number in the source checklist,
    /// so ids are never reused or renumbered. Every member is a real catalogued sin (or the omitted placeholder for a
    /// skipped number): there is deliberately no Unknown or None member and nothing is numbered 0, because a sin is
    /// never "unknown". Use a nullable <c>SinId?</c> for "no sin". Persist the number.
    /// </summary>
    public enum SinId : uint
    {
        /// <summary>Abortion (Exodus 21:22-25; Jeremiah 1:4; Jeremiah 1:5)</summary>
        Abortion = 1,

        /// <summary>Not Abstaining From All Appearance Of Evil (FirstThessalonians 5:22)</summary>
        NotAbstainingFromAllAppearanceOfEvil = 2,

        /// <summary>Accusing (Jude 1:9; SecondPeter 2:11)</summary>
        Accusing = 3,

        /// <summary>Not Acknowledging Thine Iniquity (Jeremiah 3:13)</summary>
        NotAcknowledgingThineIniquity = 4,

        /// <summary>Adultery (Matthew 19:18; Genesis 39:7-9; Exodus 20:14; Deuteronomy 22:22; Leviticus 20:10-12; Matthew 5:28; SecondPeter 2:14)</summary>
        Adultery = 5,

        /// <summary>Afraid (Psalms 112:7)</summary>
        Afraid = 6,

        /// <summary>Afraid To Confess Jesus To People (John 12:42)</summary>
        AfraidToConfessJesusToPeople = 7,

        /// <summary>Unjustified Anger (Wraths) (Proverbs 27:4; Proverbs 29:22; Exodus 6:9; SecondCorinthians 12:20)</summary>
        UnjustifiedAngerWraths = 8,

        /// <summary>Angry With Your Brother (Matthew 5:22)</summary>
        AngryWithYourBrother = 9,

        /// <summary>Anxious (Philippians 4:6)</summary>
        Anxious = 10,

        /// <summary>Arguing (Proverbs 17:14; Proverbs 18:6; Titus 3:9; SecondTimothy 2:23)</summary>
        Arguing = 11,

        /// <summary>Arrogance (Swellings/Proud) (Mark 7:22; Isaiah 2:17; Isaiah 2:11; Romans 1:30; SecondCorinthians 11:20; Isaiah 13:11)</summary>
        ArroganceSwellingsProud = 12,

        /// <summary>Ashamed, Hiding Your Light For Jesus (Matthew 5:14-16; Luke 9:26; Romans 10:11)</summary>
        AshamedHidingYourLightForJesus = 13,

        /// <summary>Ashamed Of Jesus And His Words (Mark 8:38)</summary>
        AshamedOfJesusAndHisWords = 14,

        /// <summary>Assault (Exodus 21:18; Exodus 21:19; Acts 14:5)</summary>
        Assault = 15,

        /// <summary>Astrology (Deuteronomy 4:19; Deuteronomy 17:3-7; Isaiah 47:13; Isaiah 47:14; Acts 7:42; Ezekiel 8:16; Deuteronomy 17:7)</summary>
        Astrology = 16,

        /// <summary>Vain Babblings (FirstTimothy 6:20; SecondTimothy 2:16)</summary>
        VainBabblings = 17,

        /// <summary>Backbiting (Psalms 15:1-3)</summary>
        Backbiting = 18,

        /// <summary>Refused To Be Baptized (Luke 7:29; Luke 7:30)</summary>
        RefusedToBeBaptized = 19,

        /// <summary>Baptized Before Believing On Jesus (Acts 8:36; Acts 8:37)</summary>
        BaptizedBeforeBelievingOnJesus = 20,

        /// <summary>Not Believing On Name Of His Son Jesus Christ (FirstJohn 3:23)</summary>
        NotBelievingOnNameOfHisSonJesusChrist = 21,

        /// <summary>Bitterness (Acts 8:23; Romans 3:14; Ephesians 4:31; Hebrews 12:15)</summary>
        Bitterness = 22,

        /// <summary>Blasphemous (Mark 7:22; Jude 1:10)</summary>
        Blasphemous = 23,

        /// <summary>Blasphemy Against The Holy Ghost (Mark 3:29; Luke 12:10)</summary>
        BlasphemyAgainstTheHolyGhost = 24,

        /// <summary>Not Blessing Them That Curse You (Matthew 12:31; Luke 6:28)</summary>
        NotBlessingThemThatCurseYou = 25,

        /// <summary>Not Blessing Them Which Persecute You (Romans 12:14)</summary>
        NotBlessingThemWhichPersecuteYou = 26,

        /// <summary>A Person That Leads The Blind Astray Is Cursed (Deuteronomy 27:18; Matthew 23:16)</summary>
        APersonThatLeadsTheBlindAstrayIsCursed = 27,

        /// <summary>Eating Blood (FirstSamuel 14:33)</summary>
        EatingBlood = 28,

        /// <summary>Boasting Is Evil (James 4:16; James 3:5; Romans 1:30; Matthew 18:3; Mark 16:16; John 3:3; Acts 2:38; Colossians 2:12; Hebrews 5:9; FirstJohn 5:8)</summary>
        BoastingIsEvil = 29,

        /// <summary>Being Against A Child Of God (Psalms 11:2)</summary>
        BeingAgainstAChildOfGod = 30,

        /// <summary>Causing A Child Of God To Sin (Great Sin) (Matthew 18:6)</summary>
        CausingAChildOfGodToSinGreatSin = 31,

        /// <summary>Deceiving A Child Of God (Ephesians 4:14)</summary>
        DeceivingAChildOfGod = 32,

        /// <summary>Despising One Of God'S Children (Matthew 18:10; Luke 10:16)</summary>
        DespisingOneOfGodsChildren = 33,

        /// <summary>Enticing A Child Of God To Turn (Deuteronomy 13:6-10; Acts 13:8-12; Acts 20:29-31)</summary>
        EnticingAChildOfGodToTurn = 34,

        /// <summary>Enticing A Child Of God Through Their Lusts And Desires (SecondPeter 2:18)</summary>
        EnticingAChildOfGodThroughTheirLustsAndDesires = 35,

        /// <summary>Hindering Or Beating God'S Servants (Luke 12:45)</summary>
        HinderingOrBeatingGodsServants = 36,

        /// <summary>Judging God'S Servants (Romans 14:4; Romans 14:10)</summary>
        JudgingGodsServants = 37,

        /// <summary>Laying In Wait To Accuse God'S Servants (Luke 11:54)</summary>
        LayingInWaitToAccuseGodsServants = 38,

        /// <summary>Not Clothing God'S Children That Have Need (Matthew 25:43)</summary>
        NotClothingGodsChildrenThatHaveNeed = 39,

        /// <summary>Not Feeding God'S Children That Are Hungry (Matthew 25:42)</summary>
        NotFeedingGodsChildrenThatAreHungry = 40,

        /// <summary>Not Receiving A Child Of God Sent By God (Mark 6:11; Matthew 10:14)</summary>
        NotReceivingAChildOfGodSentByGod = 41,

        /// <summary>Not Taking In A Homeless Child Of God (Matthew 25:43; Matthew 25:45; FirstTimothy 5:10)</summary>
        NotTakingInAHomelessChildOfGod = 42,

        /// <summary>Not Visiting God'S Children That Are Sick (Matthew 25:43)</summary>
        NotVisitingGodsChildrenThatAreSick = 43,

        /// <summary>Not Visiting God'S Children In Prison (Matthew 25:43)</summary>
        NotVisitingGodsChildrenInPrison = 44,

        /// <summary>Offending A Child Of God (Mark 9:42)</summary>
        OffendingAChildOfGod = 45,

        /// <summary>Persecuting A Child Of God (Acts 9:1; Acts 9:4; Acts 9:5)</summary>
        PersecutingAChildOfGod = 46,

        /// <summary>Putting Stumbling Blocks In Path Of God'S Children (Acts 15:1; Acts 15:2; Romans 14:13)</summary>
        PuttingStumblingBlocksInPathOfGodsChildren = 47,

        /// <summary>Refusing To Hear God'S Servants (FirstJohn 4:6)</summary>
        RefusingToHearGodsServants = 48,

        /// <summary>Speaking Against God'S Children (Acts 21:28; Numbers 14:2; Numbers 14:3; Numbers 14:11; Numbers 16:3)</summary>
        SpeakingAgainstGodsChildren = 49,

        /// <summary>Smiting God'S Children (Matthew 24:49)</summary>
        SmitingGodsChildren = 50,

        /// <summary>Sowing Any Form Of Discord Among God'S Children (Proverbs 6:16; Proverbs 6:19)</summary>
        SowingAnyFormOfDiscordAmongGodsChildren = 51,

        /// <summary>Thinking Evil In Your Heart Against God'S Children (Matthew 9:4)</summary>
        ThinkingEvilInYourHeartAgainstGodsChildren = 52,

        /// <summary>Threatening God'S People (Acts 9:1; Acts 9:4; Acts 9:5; John 9:22; John 9:28; John 9:34)</summary>
        ThreateningGodsPeople = 53,

        /// <summary>Touching God'S Servants (Acts 5:18; Acts 5:35; Acts 5:39)</summary>
        TouchingGodsServants = 54,

        /// <summary>Zealous For God But Hate God'S Children (Acts 22:3; Acts 22:22)</summary>
        ZealousForGodButHateGodsChildren = 55,

        /// <summary>Breaking Up Homes (Matthew 19:6; Matthew 5:31; Matthew 5:32)</summary>
        BreakingUpHomes = 56,

        /// <summary>Accepting A Bribe (Exodus 23:8; Psalms 26:10)</summary>
        AcceptingABribe = 57,

        /// <summary>Busy Bodies (Proverbs 20:3; FirstTimothy 5:13)</summary>
        BusyBodies = 58,

        /// <summary>Putting Cares Of This World Before God (Mark 4:19; Luke 8:14; Luke 21:34)</summary>
        PuttingCaresOfThisWorldBeforeGod = 59,

        /// <summary>Being Carnal Or Worldly (FirstCorinthians 3:1-3; Romans 8:6-8)</summary>
        BeingCarnalOrWorldly = 60,

        /// <summary>Not Having Charity (FirstCorinthians 13:2)</summary>
        NotHavingCharity = 61,

        /// <summary>Not Enduring God'S Chastening (Hebrews 12:7; Hebrews 12:8)</summary>
        NotEnduringGodsChastening = 62,

        /// <summary>Not Becoming As A Little Child Before God (Not Humbling Yourself) (Matthew 18:3)</summary>
        NotBecomingAsALittleChildBeforeGodNotHumblingYourself = 63,

        /// <summary>Not Spanking A Disobedient Child (Proverbs 13:24)</summary>
        NotSpankingADisobedientChild = 64,

        /// <summary>Father'S Provoking Their Children To Wrath (Ephesians 6:4)</summary>
        FathersProvokingTheirChildrenToWrath = 65,

        /// <summary>Not Bringing Children Up In Training And Instruction Of The Lord (Ephesians 6:4)</summary>
        NotBringingChildrenUpInTrainingAndInstructionOfTheLord = 66,

        /// <summary>Not Going To Church Or Assembling Together (Hebrews 10:25; Acts 2:46; Ecclesiastes 4:9-12)</summary>
        NotGoingToChurchOrAssemblingTogether = 67,

        /// <summary>Going To The Church Of The Mother Of Harlots (Revelation 17:5; Revelation 18:4; Revelation 18:5)</summary>
        GoingToTheChurchOfTheMotherOfHarlots = 68,

        /// <summary>Wearing The Clothing Of The Opposite Sex (Deuteronomy 22:5)</summary>
        WearingTheClothingOfTheOppositeSex = 69,

        /// <summary>Dressing Indecently (FirstTimothy 2:9; Proverbs 7:10)</summary>
        DressingIndecently = 70,

        /// <summary>Love Fancy Clothing To Show Off-To Be Admired By Men (Mark 12:38)</summary>
        LoveFancyClothingToShowOffToBeAdmiredByMen = 71,

        /// <summary>Women-Beauty That Comes From Wearing Gold Jewelry (FirstPeter 3:3)</summary>
        WomenBeautyThatComesFromWearingGoldJewelry = 72,

        /// <summary>Women-That Do Not Dress Modestly (FirstTimothy 2:9)</summary>
        WomenThatDoNotDressModestly = 73,

        /// <summary>Women-That Adorn Themselves With Gold, Pearls, Costly Array (FirstTimothy 2:9)</summary>
        WomenThatAdornThemselvesWithGoldPearlsCostlyArray = 74,

        /// <summary>Disobeying God'S Commandments (FirstJohn 2:3)</summary>
        DisobeyingGodsCommandments = 75,

        /// <summary>Take Communion But Have Sin In Your Life (Cursed) (FirstCorinthians 11:29; FirstCorinthians 11:30)</summary>
        TakeCommunionButHaveSinInYourLifeCursed = 76,

        /// <summary>Not Having Compassion (FirstPeter 3:8)</summary>
        NotHavingCompassion = 77,

        /// <summary>Complainers (Jude 1:16)</summary>
        Complainers = 78,

        /// <summary>Complaining About Hardships (Numbers 11:1)</summary>
        ComplainingAboutHardships = 79,

        /// <summary>Complaining About What You Do Not Have (Numbers 11:4-6; Numbers 21:5)</summary>
        ComplainingAboutWhatYouDoNotHave = 80,

        /// <summary>Conceited (Pride) (FirstTimothy 3:6; SecondTimothy 3:2; SecondTimothy 3:4)</summary>
        ConceitedPride = 81,

        /// <summary>Condemning (Luke 6:37)</summary>
        Condemning = 82,

        /// <summary>Contemns God (Psalms 10:13)</summary>
        ContemnsGod = 83,

        /// <summary>Contempt (Proverbs 23:22; Matthew 18:10; Romans 14:3)</summary>
        Contempt = 84,

        /// <summary>Not Being Content With What You Have (Hebrews 13:5)</summary>
        NotBeingContentWithWhatYouHave = 85,

        /// <summary>Speaking Boastful Words (SecondPeter 2:18)</summary>
        SpeakingBoastfulWords = 86,

        /// <summary>Not Bridling (James 1:26)</summary>
        NotBridling = 87,

        /// <summary>Conversation That Does Not Become Jesus (The Gospel) (Philippians 1:27)</summary>
        ConversationThatDoesNotBecomeJesusTheGospel = 88,

        /// <summary>Conversation According To The Deceitful Lusts (Ephesians 4:22)</summary>
        ConversationAccordingToTheDeceitfulLusts = 89,

        /// <summary>Corrupt Conversation (Ephesians 4:29)</summary>
        CorruptConversation = 90,

        /// <summary>Covetousness Conversation (Hebrews 13:5)</summary>
        CovetousnessConversation = 91,

        /// <summary>Crafty Conversation (Job 15:4-6)</summary>
        CraftyConversation = 92,

        /// <summary>Double-Tongued (FirstTimothy 3:8)</summary>
        DoubleTongued = 93,

        /// <summary>Speaking Evil About God'S Word (Acts 19:9)</summary>
        SpeakingEvilAboutGodsWord = 94,

        /// <summary>Speaking About Other People'S Evil (Ephesians 5:11; Ephesians 5:12)</summary>
        SpeakingAboutOtherPeoplesEvil = 95,

        /// <summary>Speaking Evil Of Brothers (One Of Another) (James 4:11)</summary>
        SpeakingEvilOfBrothersOneOfAnother = 96,

        /// <summary>Speaking Evil Of Dignities (Jude 1:8)</summary>
        SpeakingEvilOfDignities = 97,

        /// <summary>Speaking Evil Or Guile (FirstPeter 2:1; FirstPeter 3:10; Psalms 34:13)</summary>
        SpeakingEvilOrGuile = 98,

        /// <summary>Filthy Conversation (Colossians 3:8)</summary>
        FilthyConversation = 99,

        /// <summary>Foolish Conversation (Ephesians 5:4)</summary>
        FoolishConversation = 100,

        /// <summary>Bidding God Speed To Christ Rejecting People (SecondJohn 1:11)</summary>
        BiddingGodSpeedToChristRejectingPeople = 101,

        /// <summary>Hasty In Words (Proverbs 29:20)</summary>
        HastyInWords = 102,

        /// <summary>Not Being Slow To Speak (James 1:19)</summary>
        NotBeingSlowToSpeak = 103,

        /// <summary>Not Being Meek And Quiet (FirstPeter 3:4)</summary>
        NotBeingMeekAndQuiet = 104,

        /// <summary>Proud Speech (Matthew 12:36; Psalms 17:10)</summary>
        ProudSpeech = 105,

        /// <summary>Speaking Things They Ought Not (FirstTimothy 5:13)</summary>
        SpeakingThingsTheyOughtNot = 106,

        /// <summary>Turning To Vain Jangling (Meaningless Talk) (FirstTimothy 1:6)</summary>
        TurningToVainJanglingMeaninglessTalk = 107,

        /// <summary>Partaking Of Vain Words (Ephesians 5:6; Ephesians 5:7; Jonah 2:8)</summary>
        PartakingOfVainWords = 108,

        /// <summary>Saying I Will Go, I Will Buy, But Not If God Will (James 4:13-17)</summary>
        SayingIWillGoIWillBuyButNotIfGodWill = 109,

        /// <summary>Professing To Be Wise (Pride) (Romans 1:22)</summary>
        ProfessingToBeWisePride = 110,

        /// <summary>Being Involved In Wive'S Fables (FirstTimothy 4:7)</summary>
        BeingInvolvedInWivesFables = 111,

        /// <summary>Contention (Strife) (Proverbs 17:14)</summary>
        ContentionStrife = 112,

        /// <summary>Not Being Converted-Perish (Matthew 18:3)</summary>
        NotBeingConvertedPerish = 113,

        /// <summary>Despising Correction (Hebrews 12:5)</summary>
        DespisingCorrection = 114,

        /// <summary>Refusing Correction-Perish (Jeremiah 5:3; Jeremiah 5:4)</summary>
        RefusingCorrectionPerish = 115,

        /// <summary>Refusing To Separate From False Counsel (Proverbs 19:27)</summary>
        RefusingToSeparateFromFalseCounsel = 116,

        /// <summary>Seeking Counsel That Is Not Of God (Isaiah 30:1)</summary>
        SeekingCounselThatIsNotOfGod = 117,

        /// <summary>Turning To False Counsel (Fables) (SecondTimothy 4:4)</summary>
        TurningToFalseCounselFables = 118,

        /// <summary>Walking In Counsel Of Unsaved (Psalms 1:1)</summary>
        WalkingInCounselOfUnsaved = 119,

        /// <summary>Not Being Courteous (Proverbs 12:5; FirstPeter 3:8)</summary>
        NotBeingCourteous = 120,

        /// <summary>Covetousness-An Insatiable Desire For Worldly Gain (Habakkuk 2:9)</summary>
        CovetousnessAnInsatiableDesireForWorldlyGain = 121,

        /// <summary>Covetousness-Desire Neighbor'S Things (Mark 7:22; Exodus 20:17; Ezekiel 33:31)</summary>
        CovetousnessDesireNeighborsThings = 122,

        /// <summary>Craftines (Colossians 3:5; Ephesians 5:5; SecondCorinthians 4:2)</summary>
        Craftines = 123,

        /// <summary>Bidding Cults, God Speed Or Welcoming Them Into Your House (SecondJohn 1:10)</summary>
        BiddingCultsGodSpeedOrWelcomingThemIntoYourHouse = 124,

        /// <summary>Cursing (Romans 3:14; Psalms 10:7; Psalms 59:12)</summary>
        Cursing = 125,

        /// <summary>Cursing The Lord'S Name (Exodus 20:7)</summary>
        CursingTheLordsName = 126,

        /// <summary>Cursing The Rich (Leviticus 24:16; Ecclesiastes 10:20)</summary>
        CursingTheRich = 127,

        /// <summary>Cursing Men (James 3:9)</summary>
        CursingMen = 128,

        /// <summary>Debate (Romans 1:29)</summary>
        Debate = 129,

        /// <summary>Making Bad Debts (Romans 13:8)</summary>
        MakingBadDebts = 130,

        /// <summary>Not Paying What You Owe (Debts) (Matthew 5:26)</summary>
        NotPayingWhatYouOweDebts = 131,

        /// <summary>Holding Fast To Deceit (Light In You Is Darkness) (Jeremiah 8:5; Mark 7:22)</summary>
        HoldingFastToDeceitLightInYouIsDarkness = 132,

        /// <summary>Working Deceit (Psalms 101:7; Psalms 10:7; Psalms 35:20)</summary>
        WorkingDeceit = 133,

        /// <summary>Deceitful (SecondCorinthians 11:13; Romans 1:29)</summary>
        Deceitful = 134,

        /// <summary>Deceiving Your Neighbor (Jeremiah 9:5; Jeremiah 9:6)</summary>
        DeceivingYourNeighbor = 135,

        /// <summary>Defrauding Others (Leviticus 19:13; FirstThessalonians 4:6)</summary>
        DefraudingOthers = 136,

        /// <summary>Not Delighting Yourself In The Lord (Psalms 37:4)</summary>
        NotDelightingYourselfInTheLord = 137,

        /// <summary>Desire Sinful Ways (Luke 5:39)</summary>
        DesireSinfulWays = 138,

        /// <summary>Despising (FirstThessalonians 4:8)</summary>
        Despising = 139,

        /// <summary>Despising Dominion (Authority) (Jude 1:8)</summary>
        DespisingDominionAuthority = 140,

        /// <summary>Despising God'S Servants (Luke 10:16)</summary>
        DespisingGodsServants = 141,

        /// <summary>Despising Government (SecondPeter 2:10)</summary>
        DespisingGovernment = 142,

        /// <summary>Despising His Neighbor (Proverbs 14:21)</summary>
        DespisingHisNeighbor = 143,

        /// <summary>Despising Others (SecondTimothy 3:3; Luke 18:9)</summary>
        DespisingOthers = 144,

        /// <summary>Despising Those That Are Good (SecondTimothy 3:1-3)</summary>
        DespisingThoseThatAreGood = 145,

        /// <summary>Giving Place To The Devil (Doing Something The Bible Says Not To Do) (Ephesians 4:27)</summary>
        GivingPlaceToTheDevilDoingSomethingTheBibleSaysNotToDo = 146,

        /// <summary>Not Resisting The Devil (James 4:7-10)</summary>
        NotResistingTheDevil = 147,

        /// <summary>Discouraged (Deuteronomy 1:21; FirstPeter 5:7)</summary>
        Discouraged = 148,

        /// <summary>Disobedient To Parents (SecondTimothy 3:1; SecondTimothy 3:2; Romans 1:30)</summary>
        DisobedientToParents = 149,

        /// <summary>Disobeying Man'S Laws (Romans 13:1-5)</summary>
        DisobeyingMansLaws = 150,

        /// <summary>Disputing (Philippians 2:14)</summary>
        Disputing = 151,

        /// <summary>Causing Division Contrary To God'S Word (Romans 16:17)</summary>
        CausingDivisionContraryToGodsWord = 152,

        /// <summary>Divorce, Except For Certain Conditions (Matthew 19:9)</summary>
        DivorceExceptForCertainConditions = 153,

        /// <summary>Getting Involved In The Doctrines Of Devils (FirstTimothy 4:1)</summary>
        GettingInvolvedInTheDoctrinesOfDevils = 154,

        /// <summary>Getting Involved In Any Doctrines Other Than Sound Doctrine (FirstTimothy 1:10)</summary>
        GettingInvolvedInAnyDoctrinesOtherThanSoundDoctrine = 155,

        /// <summary>Filthy Dreamers (Jude 1:8)</summary>
        FilthyDreamers = 156,

        /// <summary>Drinking (FirstTimothy 3:3; Proverbs 20:1)</summary>
        Drinking = 157,

        /// <summary>Drugs (Revelation 9:21; Revelation 18:23; Revelation 21:8; Revelation 22:15)</summary>
        Drugs = 158,

        /// <summary>Getting Drunk (Drunken) (Luke 12:45; Luke 12:46; Romans 13:13; FirstCorinthians 6:10)</summary>
        GettingDrunkDrunken = 159,

        /// <summary>Being Discontent (Philippians 4:11; FirstTimothy 6:8; Luke 3:1)</summary>
        BeingDiscontent = 160,

        /// <summary>Eating And Drinking With The Drunken (Matthew 24:49)</summary>
        EatingAndDrinkingWithTheDrunken = 161,

        /// <summary>Eating Blood (Acts 15:20)</summary>
        EatingBlood162 = 162,

        /// <summary>Eating Things Strangled (Acts 15:20)</summary>
        EatingThingsStrangled = 163,

        /// <summary>Employees, Arguing With Employers (Titus 2:9)</summary>
        EmployeesArguingWithEmployers = 164,

        /// <summary>Employees, Not Being Obedient To Your Employer (Colossians 3:22; Colossians 3:23; Ephesians 6:5-8)</summary>
        EmployeesNotBeingObedientToYourEmployer = 165,

        /// <summary>Employees, Not Being Subject To Your Employer (FirstPeter 2:18)</summary>
        EmployeesNotBeingSubjectToYourEmployer = 166,

        /// <summary>Employers, Not Giving That Which Is Just And Equal (Colossians 4:1)</summary>
        EmployersNotGivingThatWhichIsJustAndEqual = 167,

        /// <summary>Employers, Threatening Employees (Ephesians 6:9)</summary>
        EmployersThreateningEmployees = 168,

        /// <summary>Not Feeding Your Enemy And Giving Him Drink (Romans 12:20)</summary>
        NotFeedingYourEnemyAndGivingHimDrink = 169,

        /// <summary>Envy (FirstTimothy 6:4; Acts 13:45; Romans 1:29)</summary>
        Envy = 170,

        /// <summary>Making Excuses When You Are Asked To Follow Jesus (Luke 14:18-24)</summary>
        MakingExcusesWhenYouAreAskedToFollowJesus = 171,

        /// <summary>Doeth Evil, Hates The Word (Hates The Light) (John 3:20)</summary>
        DoethEvilHatesTheWordHatesTheLight = 172,

        /// <summary>Evil Desires (Colossians 3:5; Colossians 3:6)</summary>
        EvilDesires = 173,

        /// <summary>Following Evil (ThirdJohn 1:11)</summary>
        FollowingEvil = 174,

        /// <summary>Not Hating Evil (Romans 12:9; Psalms 36:4)</summary>
        NotHatingEvil = 175,

        /// <summary>Hearing Evil (FirstTimothy 5:13; Psalms 101:5; Acts 17:21)</summary>
        HearingEvil = 176,

        /// <summary>Loving Evil (Psalms 52:3)</summary>
        LovingEvil = 177,

        /// <summary>Not Abstaining From All Appearance Of Evil (FirstThessalonians 5:22)</summary>
        NotAbstainingFromAllAppearanceOfEvil178 = 178,

        /// <summary>Rewarding Evil For Good, Hate For Love (Psalms 109:5)</summary>
        RewardingEvilForGoodHateForLove = 179,

        /// <summary>Rewarding Evil For Evil (Romans 12:17)</summary>
        RewardingEvilForEvil = 180,

        /// <summary>Believing In Evolution (FirstPeter 3:12; Psalms 100:3)</summary>
        BelievingInEvolution = 181,

        /// <summary>Extortion (Matthew 23:25; Luke 18:11)</summary>
        Extortion = 182,

        /// <summary>Listening To Fables (FirstTimothy 1:4)</summary>
        ListeningToFables = 183,

        /// <summary>Having No Faith (Deuteronomy 32:20-22)</summary>
        HavingNoFaith = 184,

        /// <summary>Not Living By Faith (Romans 1:17; Hebrews 10:38)</summary>
        NotLivingByFaith = 185,

        /// <summary>Faith That Is Like A Wave (Goes In And Out) (James 1:6-8; Hebrews 10:23)</summary>
        FaithThatIsLikeAWaveGoesInAndOut = 186,

        /// <summary>Whatsoever Is Not Of Faith Is Sin (Romans 14:23)</summary>
        WhatsoeverIsNotOfFaithIsSin = 187,

        /// <summary>Partaking Of False Doctrines (Galatians 5:6; SecondJohn 1:10; Jude 1:4; Romans 16:17; Romans 16:18; Proverbs 19:27)</summary>
        PartakingOfFalseDoctrines = 188,

        /// <summary>Giving A False Report (Exodus 23:1)</summary>
        GivingAFalseReport = 189,

        /// <summary>Using False Scales (Proverbs 11:1)</summary>
        UsingFalseScales = 190,

        /// <summary>Being A False Witness (Exodus 20:16; Matthew 15:19; Matthew 19:18)</summary>
        BeingAFalseWitness = 191,

        /// <summary>Fasting Unto God, But Not Secretly (To Be Seen By Men) (Matthew 6:16-18; Acts 14:23)</summary>
        FastingUntoGodButNotSecretlyToBeSeenByMen = 192,

        /// <summary>Calling A Man Your Father Upon The Earth (Matthew 23:9)</summary>
        CallingAManYourFatherUponTheEarth = 193,

        /// <summary>Fault Finders (Jude 1:16)</summary>
        FaultFinders = 194,

        /// <summary>Fear (Being Afraid) Being Fearful (Galatians 2:12-14; FirstJohn 4:18; Revelation 21:8)</summary>
        FearBeingAfraidBeingFearful = 195,

        /// <summary>Fearing Men (Luke 12:4; Luke 12:5)</summary>
        FearingMen = 196,

        /// <summary>Not Fearing Your Mother And Father (Leviticus 19:3)</summary>
        NotFearingYourMotherAndFather = 197,

        /// <summary>Not Fearing God (FirstTimothy 5:20; Matthew 10:28; Romans 3:18)</summary>
        NotFearingGod = 198,

        /// <summary>Fighting Among Brothers (James 4:1; James 4:2)</summary>
        FightingAmongBrothers = 199,

        /// <summary>Flattering For Gain (Jude 1:16; Proverbs 26:28; Psalms 12:3)</summary>
        FlatteringForGain = 200,

        /// <summary>Not Crucifying The Flesh (Galatians 5:24)</summary>
        NotCrucifyingTheFlesh = 201,

        /// <summary>Walking After The Flesh (Romans 8:1; Romans 8:5; Romans 8:13)</summary>
        WalkingAfterTheFlesh = 202,

        /// <summary>Following The Desires Of The Flesh (Ephesians 2:3)</summary>
        FollowingTheDesiresOfTheFlesh = 203,

        /// <summary>Getting Involved With Foolish Questions (SecondTimothy 2:23; Titus 3:9)</summary>
        GettingInvolvedWithFoolishQuestions = 204,

        /// <summary>Foolishness (Mark 7:22; Titus 3:3)</summary>
        Foolishness = 205,

        /// <summary>Feeding On Foolishness (Proverbs 15:14)</summary>
        FeedingOnFoolishness = 206,

        /// <summary>Proclaiming Foolishness (Foolish Talking) (Proverbs 12:23; Proverbs 15:2)</summary>
        ProclaimingFoolishnessFoolishTalking = 207,

        /// <summary>Thinking Of Foolishness (Proverbs 24:9)</summary>
        ThinkingOfFoolishness = 208,

        /// <summary>Forbidding Those Seeking Jesus To Come To Jesus (Matthew 19:14; Luke 11:52)</summary>
        ForbiddingThoseSeekingJesusToComeToJesus = 209,

        /// <summary>Forbidding To Marry (FirstTimothy 4:3)</summary>
        ForbiddingToMarry = 210,

        /// <summary>Not Forgiving Others (Matthew 6:14; Matthew 6:15; Mark 11:25; Mark 11:26; Luke 17:4)</summary>
        NotForgivingOthers = 211,

        /// <summary>Fornication (Acts 15:20; Mark 7:21; Romans 1:29)</summary>
        Fornication = 212,

        /// <summary>Forsaking Assembling With Other Believers (Hebrews 10:25)</summary>
        ForsakingAssemblingWithOtherBelievers = 213,

        /// <summary>Fortune-Telling (Acts 16:16-18; Deuteronomy 18:10; Deuteronomy 18:11)</summary>
        FortuneTelling = 214,

        /// <summary>Fraud (James 5:4; Psalms 10:7; FirstThessalonians 4:6)</summary>
        Fraud = 215,

        /// <summary>Bring Not Forth Good Fruit (Matthew 3:10)</summary>
        BringNotForthGoodFruit = 216,

        /// <summary>Not Bearing Good Fruit (Matthew 7:19-20; Luke 13:7-9)</summary>
        NotBearingGoodFruit = 217,

        /// <summary>Gambling (Luke 12:15)</summary>
        Gambling = 218,

        /// <summary>Refusing To Be Reconciled To Brother Yet Giving Gift (Matthew 5:24)</summary>
        RefusingToBeReconciledToBrotherYetGivingGift = 219,

        /// <summary>Giving Grudgingly (SecondCorinthians 9:7)</summary>
        GivingGrudgingly = 220,

        /// <summary>Giving To The Poor To Be Seen By Men (Matthew 6:1-4)</summary>
        GivingToThePoorToBeSeenByMen = 221,

        /// <summary>Not Giving To Those Who Ask (Luke 6:30; Matthew 5:42)</summary>
        NotGivingToThoseWhoAsk = 222,

        /// <summary>Not Giving To Your Brother Who Has Need (FirstJohn 3:17)</summary>
        NotGivingToYourBrotherWhoHasNeed = 223,

        /// <summary>Being A Glutton (Deuteronomy 21:20; Proverbs 23:21; Luke 21:34)</summary>
        BeingAGlutton = 224,

        /// <summary>Having Any Gods That Come Before God (Exodus 20:3; Psalms 81:9; Acts 15:20)</summary>
        HavingAnyGodsThatComeBeforeGod = 225,

        /// <summary>Acting Contrary To God'S Decrees (Leviticus 26:21; FirstTimothy 1:10)</summary>
        ActingContraryToGodsDecrees = 226,

        /// <summary>Ashamed Of Jesus (Romans 9:33; Romans 10:11; Mark 8:38)</summary>
        AshamedOfJesus = 227,

        /// <summary>Denying Jesus Before Men (Matthew 10:33; FirstJohn 2:22)</summary>
        DenyingJesusBeforeMen = 228,

        /// <summary>Denying Jesus Christ Is From Everlasting To Everlasting (Revelation 1:6; Revelation 1:8; Revelation 1:11)</summary>
        DenyingJesusChristIsFromEverlastingToEverlasting = 229,

        /// <summary>Denying Jesus Christ Is God In The Flesh (John 1:1; John 1:14; FirstJohn 4:3; SecondJohn 1:7)</summary>
        DenyingJesusChristIsGodInTheFlesh = 230,

        /// <summary>Denying Jesus Christ Is God'S Only Begotten Son (John 3:16)</summary>
        DenyingJesusChristIsGodsOnlyBegottenSon = 231,

        /// <summary>Denying Jesus Is The Christ (FirstJohn 5:10; FirstJohn 5:1; FirstJohn 2:22)</summary>
        DenyingJesusIsTheChrist = 232,

        /// <summary>Denying Jesus Christ Is The Second Person Of The Trinity: God The Father, God The Son, And God The Holy Spirit (FirstJohn 5:7; Isaiah 48:16; Matthew 28:19)</summary>
        DenyingJesusChristIsTheSecondPersonOfTheTrinityGodTheFatherGodTheSonAndGodTheHolySpirit = 233,

        /// <summary>Denying Jesus Christ Is The King Of Kings And Lord Of Lords (Revelation 19:16)</summary>
        DenyingJesusChristIsTheKingOfKingsAndLordOfLords = 234,

        /// <summary>Denying Jesus Christ Made All Things (John 1:3; Hebrews 1:1; Hebrews 1:2; Colossians 1:14-17)</summary>
        DenyingJesusChristMadeAllThings = 235,

        /// <summary>Denying Jesus Christ Was Raised From The Dead By God (Romans 10:9)</summary>
        DenyingJesusChristWasRaisedFromTheDeadByGod = 236,

        /// <summary>Despising God'S Goodness (Romans 2:4)</summary>
        DespisingGodsGoodness = 237,

        /// <summary>Forsaking God (Jeremiah 16:11)</summary>
        ForsakingGod = 238,

        /// <summary>Hardening Your Heart Towards God'S Ways (Hebrews 3:8)</summary>
        HardeningYourHeartTowardsGodsWays = 239,

        /// <summary>Speaking Harsh Words Against God (Jude 1:15)</summary>
        SpeakingHarshWordsAgainstGod = 240,

        /// <summary>Knowing God'S Will But Not Doing It (Luke 12:47; James 4:17)</summary>
        KnowingGodsWillButNotDoingIt = 241,

        /// <summary>Laying Aside God'S Commandments (Mark 7:8; Mark 7:9)</summary>
        LayingAsideGodsCommandments = 242,

        /// <summary>Not Confessing Jesus Christ Is God Come In The Flesh (FirstJohn 4:3; SecondJohn 1:7)</summary>
        NotConfessingJesusChristIsGodComeInTheFlesh = 243,

        /// <summary>Not Doing Everything As Unto The Lord (Colossians 3:23)</summary>
        NotDoingEverythingAsUntoTheLord = 244,

        /// <summary>Not Doing God'S Will (Matthew 7:21; FirstJohn 2:17)</summary>
        NotDoingGodsWill = 245,

        /// <summary>Not Doing Those Things Which Please God (John 8:29; FirstJohn 3:22)</summary>
        NotDoingThoseThingsWhichPleaseGod = 246,

        /// <summary>Not Forsaking All For Jesus (Luke 14:33)</summary>
        NotForsakingAllForJesus = 247,

        /// <summary>Not Fearing God (Deuteronomy 6:13; Deuteronomy 10:12; Psalms 36:1)</summary>
        NotFearingGod248 = 248,

        /// <summary>Not For God (Is Against God) (FirstCorinthians 10:21; Matthew 12:30; Luke 11:33)</summary>
        NotForGodIsAgainstGod = 249,

        /// <summary>Not Giving God The Glory (Luke 17:17; Luke 17:18; Romans 1:21; Acts 12:23)</summary>
        NotGivingGodTheGlory = 250,

        /// <summary>Not Giving Thanks To God For All Things (Ephesians 5:20; Colossians 3:17)</summary>
        NotGivingThanksToGodForAllThings = 251,

        /// <summary>Not Honoring Jesus (John 5:23)</summary>
        NotHonoringJesus = 252,

        /// <summary>Not Led By The Spirit Of God (Romans 8:14)</summary>
        NotLedByTheSpiritOfGod = 253,

        /// <summary>Not Letting The Lord Reign Over Your Lips (Psalms 12:3; Psalms 12:4)</summary>
        NotLettingTheLordReignOverYourLips = 254,

        /// <summary>Not Looking For Jesus To Return (Hebrews 9:28)</summary>
        NotLookingForJesusToReturn = 255,

        /// <summary>Not Loving God With All Your Heart, Soul, And Mind (Matthew 22:37; Deuteronomy 6:5)</summary>
        NotLovingGodWithAllYourHeartSoulAndMind = 256,

        /// <summary>Not Receiving A Person God Sends (Luke 9:5)</summary>
        NotReceivingAPersonGodSends = 257,

        /// <summary>Not Regarding The Works Of God (Psalms 28:5)</summary>
        NotRegardingTheWorksOfGod = 258,

        /// <summary>Not Seeking First The Kingdom Of God (Matthew 6:33)</summary>
        NotSeekingFirstTheKingdomOfGod = 259,

        /// <summary>Not Seeking God'S Face (SecondChronicles 7:14)</summary>
        NotSeekingGodsFace = 260,

        /// <summary>Not Submitting To God (James 4:7-10)</summary>
        NotSubmittingToGod = 261,

        /// <summary>Not Submitting To The Righteousness Of God (Romans 10:2; Romans 10:3)</summary>
        NotSubmittingToTheRighteousnessOfGod = 262,

        /// <summary>Not Testifying Of Jesus (FirstPeter 3:15; Romans 10:10; Romans 10:11; Romans 10:9)</summary>
        NotTestifyingOfJesus = 263,

        /// <summary>Not Trusting In God (Numbers 14:9; Numbers 14:11; Proverbs 3:5; Proverbs 3:7)</summary>
        NotTrustingInGod = 264,

        /// <summary>Not Turning Away From Those Who Don'T Glorify Jesus (SecondTimothy 3:5)</summary>
        NotTurningAwayFromThoseWhoDontGlorifyJesus = 265,

        /// <summary>Not Understanding God'S Will (Ephesians 5:17)</summary>
        NotUnderstandingGodsWill = 266,

        /// <summary>Not Using God Given Talents For God (FirstPeter 4:10; Matthew 25:24-30)</summary>
        NotUsingGodGivenTalentsForGod = 267,

        /// <summary>Not Walking In God'S Ways (Hebrews 3:10)</summary>
        NotWalkingInGodsWays = 268,

        /// <summary>Not Worshipping God In Spirit And Truth (John 4:24)</summary>
        NotWorshippingGodInSpiritAndTruth = 269,

        /// <summary>Not Yielding Unto God (Romans 6:13)</summary>
        NotYieldingUntoGod = 270,

        /// <summary>Rebelling Against God (Isaiah 1:2; Isaiah 1:20)</summary>
        RebellingAgainstGod = 271,

        /// <summary>Refusing To Turn To God (Jeremiah 8:5)</summary>
        RefusingToTurnToGod = 272,

        /// <summary>Rejecting God'S Calling (Matthew 22:5)</summary>
        RejectingGodsCalling = 273,

        /// <summary>Rejecting Jesus Christ (John 12:48)</summary>
        RejectingJesusChrist = 274,

        /// <summary>Appears Godly But Denies Jesus (SecondTimothy 3:5)</summary>
        AppearsGodlyButDeniesJesus = 275,

        /// <summary>Being A Deceitful Worker Of Christ (SecondCorinthians 11:13)</summary>
        BeingADeceitfulWorkerOfChrist = 276,

        /// <summary>Denying Jesus In Deeds (Titus 1:16)</summary>
        DenyingJesusInDeeds = 277,

        /// <summary>Drawing Near To God With Words But Not With Your Heart (Matthew 15:8)</summary>
        DrawingNearToGodWithWordsButNotWithYourHeart = 278,

        /// <summary>God Not In All Your Thoughts (Psalms 10:4)</summary>
        GodNotInAllYourThoughts = 279,

        /// <summary>Ignorantly Worship God (Acts 17:23)</summary>
        IgnorantlyWorshipGod = 280,

        /// <summary>Listen To God'S Word, But Won'T Obey Them (Ezekiel 33:31)</summary>
        ListenToGodsWordButWontObeyThem = 281,

        /// <summary>Lukewarm For Jesus (Revelation 3:15; Revelation 3:16)</summary>
        LukewarmForJesus = 282,

        /// <summary>Profess To Be Saved, But Deny God By Works (Titus 1:16)</summary>
        ProfessToBeSavedButDenyGodByWorks = 283,

        /// <summary>Say You Are Saved, But Don'T Keep His Commandments (FirstJohn 2:4)</summary>
        SayYouAreSavedButDontKeepHisCommandments = 284,

        /// <summary>Show Much Love For God With Words, But Heart Covetous (Ezekiel 33:31)</summary>
        ShowMuchLoveForGodWithWordsButHeartCovetous = 285,

        /// <summary>Zeal For God, But Not According To Knowledge (Romans 10:2; Romans 10:3)</summary>
        ZealForGodButNotAccordingToKnowledge = 286,

        /// <summary>Zealous For God According To Traditions Of People (Galatians 1:14)</summary>
        ZealousForGodAccordingToTraditionsOfPeople = 287,

        /// <summary>Resisting God'S Spirit (Acts 7:51)</summary>
        ResistingGodsSpirit = 288,

        /// <summary>Rob God Not Giving 10% Plus Offerings (Cursed) (Malachi 3:8; Malachi 3:9)</summary>
        RobGodNotGiving10PlusOfferingsCursed = 289,

        /// <summary>Fearing To Serve God (Luke 19:21; Luke 19:22)</summary>
        FearingToServeGod = 290,

        /// <summary>Not Serving God (Malachi 3:18; Ephesians 2:10; Luke 4:8)</summary>
        NotServingGod = 291,

        /// <summary>Not Serving God With Humble Mind (Acts 20:19)</summary>
        NotServingGodWithHumbleMind = 292,

        /// <summary>Tempting God (Matthew 22:18; Matthew 4:7)</summary>
        TemptingGod = 293,

        /// <summary>Thinking Godliness Is Means To Financial Gain (FirstTimothy 6:5)</summary>
        ThinkingGodlinessIsMeansToFinancialGain = 294,

        /// <summary>Trusting In Man, Not God (Jeremiah 17:5)</summary>
        TrustingInManNotGod = 295,

        /// <summary>Trusting In Riches, Not God (Psalms 52:7)</summary>
        TrustingInRichesNotGod = 296,

        /// <summary>Trusting In Your Own Ways, Not God'S (Hosea 10:13; Job 33:27-30; Proverbs 3:5; Proverbs 3:7)</summary>
        TrustingInYourOwnWaysNotGods = 297,

        /// <summary>Walking Contrary To God (Leviticus 26:27; Leviticus 26:28)</summary>
        WalkingContraryToGod = 298,

        /// <summary>Walking After The Imaginations Of Your Own Heart (Jeremiah 16:12)</summary>
        WalkingAfterTheImaginationsOfYourOwnHeart = 299,

        /// <summary>Walking After Your Own Devices (Jeremiah 18:12)</summary>
        WalkingAfterYourOwnDevices = 300,

        /// <summary>Hearken To The Voice Of Your Wife Instead Of God (Genesis 3:17)</summary>
        HearkenToTheVoiceOfYourWifeInsteadOfGod = 301,

        /// <summary>Worshipping Angels Rather Than God (Colossians 2:18)</summary>
        WorshippingAngelsRatherThanGod = 302,

        /// <summary>Whispering Or Gossip (Proverbs 16:28; Romans 1:29; SecondCorinthians 12:20)</summary>
        WhisperingOrGossip = 303,

        /// <summary>Refusing To Leave The Great Harlot (Revelation 18:4; SecondCorinthians 6:16; SecondCorinthians 6:17)</summary>
        RefusingToLeaveTheGreatHarlot = 304,

        /// <summary>Greediness (Ephesians 4:19; Colossians 3:5; Colossians 3:6; Proverbs 15:27)</summary>
        Greediness = 305,

        /// <summary>Striking A Man In The Groin (Deuteronomy 25:11; Deuteronomy 25:12)</summary>
        StrikingAManInTheGroin = 306,

        /// <summary>Carrying A Grudge (FirstPeter 4:9; James 5:9; James 2:10)</summary>
        CarryingAGrudge = 307,

        /// <summary>Hate (Proverbs 15:17; Galatians 5:19; Galatians 5:20)</summary>
        Hate = 308,

        /// <summary>Haters Of God (Romans 1:30)</summary>
        HatersOfGod = 309,

        /// <summary>Hates Reproof (Hates Correction) (Proverbs 15:10)</summary>
        HatesReproofHatesCorrection = 310,

        /// <summary>Hating One Another (Titus 3:3)</summary>
        HatingOneAnother = 311,

        /// <summary>Hating Your Brother (FirstJohn 2:11; FirstJohn 4:20)</summary>
        HatingYourBrother = 312,

        /// <summary>Not Hating The Works Of Them That Turn Aside From God (Psalms 101:3)</summary>
        NotHatingTheWorksOfThemThatTurnAsideFromGod = 313,

        /// <summary>Refusing Instruction (Not Hearing Instruction) (Proverbs 8:33)</summary>
        RefusingInstructionNotHearingInstruction = 314,

        /// <summary>Double Hearted (Psalms 12:2)</summary>
        DoubleHearted = 315,

        /// <summary>Heart Hardened (Mark 8:17; Hebrews 3:15; Ephesians 4:18)</summary>
        HeartHardened = 316,

        /// <summary>Heart Hard Toward God (Closed Ears, Closed Eyes) (Acts 28:27)</summary>
        HeartHardTowardGodClosedEarsClosedEyes = 317,

        /// <summary>Mischief In Your Heart (Psalms 28:3)</summary>
        MischiefInYourHeart = 318,

        /// <summary>Seeking That Your Heart May Discover Itself (Proverbs 18:2; Numbers 15:39)</summary>
        SeekingThatYourHeartMayDiscoverItself = 319,

        /// <summary>Seeking Your Own Interest (Own Heart'S Desires) Not Christ'S (Philippians 2:21)</summary>
        SeekingYourOwnInterestOwnHeartsDesiresNotChrists = 320,

        /// <summary>Not Setting Your Heart On Things Above (Colossians 3:2)</summary>
        NotSettingYourHeartOnThingsAbove = 321,

        /// <summary>Slow Of Heart To Believe (Fool) (Luke 24:25)</summary>
        SlowOfHeartToBelieveFool = 322,

        /// <summary>Stiff-Hearted (Ezekiel 2:4)</summary>
        StiffHearted = 323,

        /// <summary>Work Wickedness In Your Heart (Psalms 58:2)</summary>
        WorkWickednessInYourHeart = 324,

        /// <summary>Heartless (Romans 1:31)</summary>
        Heartless = 325,

        /// <summary>Hindering Those Who Wish To Be Saved (Luke 11:52)</summary>
        HinderingThoseWhoWishToBeSaved = 326,

        /// <summary>Not Following Holiness (Hebrews 12:14)</summary>
        NotFollowingHoliness = 327,

        /// <summary>Homosexual (Leviticus 18:22; Romans 1:24-28; FirstCorinthians 6:9)</summary>
        Homosexual = 328,

        /// <summary>Not Walking Honestly (Romans 13:13)</summary>
        NotWalkingHonestly = 329,

        /// <summary>Not Honoring All Men (FirstPeter 2:17)</summary>
        NotHonoringAllMen = 330,

        /// <summary>Not Honoring Your Father And Mother (Exodus 20:12; Matthew 19:19)</summary>
        NotHonoringYourFatherAndMother = 331,

        /// <summary>Not Honoring The President (FirstPeter 2:17)</summary>
        NotHonoringThePresident = 332,

        /// <summary>Honoring Men As Gods Or As Infallible (Acts 14:11-18)</summary>
        HonoringMenAsGodsOrAsInfallible = 333,

        /// <summary>Not Being Hospitable (FirstPeter 4:9)</summary>
        NotBeingHospitable = 334,

        /// <summary>Not Humbling Yourself (James 4:10)</summary>
        NotHumblingYourself = 335,

        /// <summary>Not Humbling Yourselves To God (FirstPeter 5:6)</summary>
        NotHumblingYourselvesToGod = 336,

        /// <summary>Wounding Someone (Hurting Someone) (Leviticus 24:19; Leviticus 24:20)</summary>
        WoundingSomeoneHurtingSomeone = 337,

        /// <summary>Not Blessing Them That Curse You (Matthew 5:44)</summary>
        NotBlessingThemThatCurseYou338 = 338,

        /// <summary>Not Doing Good To Them Which Hate You (Luke 6:27; Matthew 5:44)</summary>
        NotDoingGoodToThemWhichHateYou = 339,

        /// <summary>Not Praying For Them Which Persecute You (Matthew 5:44)</summary>
        NotPrayingForThemWhichPersecuteYou = 340,

        /// <summary>Husband, Not Dwelling With Wife According To The Bible (FirstPeter 3:7)</summary>
        HusbandNotDwellingWithWifeAccordingToTheBible = 341,

        /// <summary>Husband, Not Honoring Your Wife (FirstPeter 3:7)</summary>
        HusbandNotHonoringYourWife = 342,

        /// <summary>Husband, Not Loving Your Wife (Ephesians 5:25)</summary>
        HusbandNotLovingYourWife = 343,

        /// <summary>Hypocritical (Matthew 15:7-9; Matthew 23:3)</summary>
        Hypocritical = 344,

        /// <summary>Abundance Of Idleness (Ezekiel 16:49)</summary>
        AbundanceOfIdleness = 345,

        /// <summary>Being Idle (Not Working When Work Is Available) (FirstTimothy 5:13; Matthew 20:6; Matthew 20:7; SecondThessalonians 3:10)</summary>
        BeingIdleNotWorkingWhenWorkIsAvailable = 346,

        /// <summary>Idols (Ezekiel 8:10; Romans 1:22; Romans 1:23; FirstJohn 5:21; Daniel 5:23; Habakkuk 2:18; Jeremiah 50:38; Jeremiah 51:17; Jeremiah 51:18)</summary>
        Idols = 347,

        /// <summary>Making A Graven Image (Exodus 20:4; Exodus 32:8; Exodus 32:21; Acts 15:20)</summary>
        MakingAGravenImage = 348,

        /// <summary>Bringing A Graven Image Into Your House (Cursed) (Deuteronomy 7:25; Deuteronomy 7:26)</summary>
        BringingAGravenImageIntoYourHouseCursed = 349,

        /// <summary>Burning Memorial Incense (Like One Who Worships Idols) (Images) (Isaiah 66:3)</summary>
        BurningMemorialIncenseLikeOneWhoWorshipsIdolsImages = 350,

        /// <summary>Covetous-An Idolater (Image) (Ephesians 5:5)</summary>
        CovetousAnIdolaterImage = 351,

        /// <summary>Idols Of Gold And Silver (Image) (Isaiah 31:7)</summary>
        IdolsOfGoldAndSilverImage = 352,

        /// <summary>Stubbornness Is As Idolatry (Image) (FirstSamuel 15:23)</summary>
        StubbornnessIsAsIdolatryImage = 353,

        /// <summary>A Heart That Devises Wicked Imaginations (Abomination) (Proverbs 6:18; Proverbs 6:16)</summary>
        AHeartThatDevisesWickedImaginationsAbomination = 354,

        /// <summary>Impudent (Ezekiel 2:4)</summary>
        Impudent = 355,

        /// <summary>Returning Insult For Insult (FirstPeter 3:9)</summary>
        ReturningInsultForInsult = 356,

        /// <summary>Jealous (Envyings) (SecondCorinthians 12:20; Luke 15:27-31; Romans 15:3)</summary>
        JealousEnvyings = 357,

        /// <summary>Putting Your Job Before God (The Work Of One'S Hands) (Deuteronomy 31:29; Revelation 9:20)</summary>
        PuttingYourJobBeforeGodTheWorkOfOnesHands = 358,

        /// <summary>Joking (Course Joking, Jesting) (Ephesians 5:4)</summary>
        JokingCourseJokingJesting = 359,

        /// <summary>Judging Others (Judging Brothers In Christ) (Matthew 7:1-5; Romans 2:1; James 4:11; James 4:12)</summary>
        JudgingOthersJudgingBrothersInChrist = 360,

        /// <summary>Justifying The Wicked (Proverbs 17:15)</summary>
        JustifyingTheWicked = 361,

        /// <summary>Kidnapping (Exodus 21:16)</summary>
        Kidnapping = 362,

        /// <summary>Killing A Mother Bird In The Nest (Deuteronomy 22:6; Deuteronomy 22:7)</summary>
        KillingAMotherBirdInTheNest = 363,

        /// <summary>Killing An Animal And Not Making It Good (Leviticus 24:18)</summary>
        KillingAnAnimalAndNotMakingItGood = 364,

        /// <summary>Killing A Person (Exodus 20:13; Matthew 19:18; Leviticus 24:17)</summary>
        KillingAPerson = 365,

        /// <summary>To Hate Your Brother Is The Same As Murder (Killing) (FirstJohn 3:15; Deuteronomy 19:13)</summary>
        ToHateYourBrotherIsTheSameAsMurderKilling = 366,

        /// <summary>The source list skips this number; kept as a placeholder so ids stay aligned.</summary>
        Omitted367 = 367,

        /// <summary>Not Being Kind (Deuteronomy 27:24; Colossians 3:12)</summary>
        NotBeingKind = 368,

        /// <summary>Not Knowing That The Return Of Jesus Christ Is Near (Mark 13:28; Mark 13:29)</summary>
        NotKnowingThatTheReturnOfJesusChristIsNear = 369,

        /// <summary>Rejecting The Knowledge Of The Word Of God (Luke 11:31)</summary>
        RejectingTheKnowledgeOfTheWordOfGod = 370,

        /// <summary>Being Lazy (SecondThessalonians 3:10-12; Proverbs 6:6-8; Ephesians 5:16; Hebrews 6:12; Matthew 20:3; Matthew 20:6; Ecclesiastes 10:18)</summary>
        BeingLazy = 371,

        /// <summary>A False Witness That Speaks Lies (An Abomination To God) (Proverbs 6:16; Proverbs 6:19; Matthew 15:19; Exodus 20:16)</summary>
        AFalseWitnessThatSpeaksLiesAnAbominationToGod = 372,

        /// <summary>Trusting In Lies (Jeremiah 7:4; Deuteronomy 19:18-21)</summary>
        TrustingInLies = 373,

        /// <summary>Lies, Lying (Psalms 59:12; Psalms 12:2; Psalms 31:18; Psalms 58:3; John 8:44; Revelation 21:8; Acts 5:3-5)</summary>
        LiesLying = 374,

        /// <summary>Lies, Hides Hatred With Lying (Proverbs 10:18)</summary>
        LiesHidesHatredWithLying = 375,

        /// <summary>Lies, Not To Hate Lying (Proverbs 13:5)</summary>
        LiesNotToHateLying = 376,

        /// <summary>Lies, Lying Tongue (Proverbs 6:17)</summary>
        LiesLyingTongue = 377,

        /// <summary>Not To Loan To Him That Asks (Matthew 5:42)</summary>
        NotToLoanToHimThatAsks = 378,

        /// <summary>Have Not Love (FirstCorinthians 13:2)</summary>
        HaveNotLove = 379,

        /// <summary>Love To Be Noticed (Luke 11:43)</summary>
        LoveToBeNoticed = 380,

        /// <summary>Not To Love In Deed And In Truth (Loving Just In Words) (FirstJohn 3:18)</summary>
        NotToLoveInDeedAndInTruthLovingJustInWords = 381,

        /// <summary>Receiving Not The Love Of The Truth (SecondThessalonians 2:10; SecondThessalonians 2:1)</summary>
        ReceivingNotTheLoveOfTheTruth = 382,

        /// <summary>Loving Others (FirstJohn 4:8; FirstJohn 5:2; Philippians 1:9)</summary>
        LovingOthers = 383,

        /// <summary>Loving Not The Brethren, Loving Not Your Brother (FirstJohn 3:14)</summary>
        LovingNotTheBrethrenLovingNotYourBrother = 384,

        /// <summary>Loving Not Your Enemies (Matthew 5:44; Luke 6:27)</summary>
        LovingNotYourEnemies = 385,

        /// <summary>Loving Not Your Neighbor (Romans 13:9; Matthew 19:19; Luke 10:27)</summary>
        LovingNotYourNeighbor = 386,

        /// <summary>Loving The World (FirstJohn 2:15)</summary>
        LovingTheWorld = 387,

        /// <summary>Loving Your Own Life (John 12:25)</summary>
        LovingYourOwnLife = 388,

        /// <summary>Loving Your Own Self (SecondTimothy 3:2)</summary>
        LovingYourOwnSelf = 389,

        /// <summary>Women, Not Loving Your Children (Titus 2:4)</summary>
        WomenNotLovingYourChildren = 390,

        /// <summary>Lukewarm Toward God (Revelation 3:16)</summary>
        LukewarmTowardGod = 391,

        /// <summary>Lusts Or Lustful (John 8:44; SecondTimothy 4:3; Colossians 3:5; Colossians 3:6; Matthew 5:28)</summary>
        LustsOrLustful = 392,

        /// <summary>Lusting After Evil Things (FirstCorinthians 10:6)</summary>
        LustingAfterEvilThings = 393,

        /// <summary>Lusts Of Other Things (Mark 4:19)</summary>
        LustsOfOtherThings = 394,

        /// <summary>Making A Way To Fulfill Lusts (Romans 13:14)</summary>
        MakingAWayToFulfillLusts = 395,

        /// <summary>Youthful Lusts (SecondTimothy 2:22)</summary>
        YouthfulLusts = 396,

        /// <summary>Malice (Ill Will) (SecondPeter 1:4; James 4:1; James 1:14; James 1:1; Mark 7:22; Romans 1:29; FirstPeter 2:1)</summary>
        MaliceIllWill = 397,

        /// <summary>Effeminate (Male Prostitutes) (FirstCorinthians 6:9; FirstCorinthians 6:10)</summary>
        EffeminateMaleProstitutes = 398,

        /// <summary>Bad Manners (FirstCorinthians 15:33)</summary>
        BadManners = 399,

        /// <summary>Not Submitting To Man'S Laws (Romans 13:2; Romans 13:3; FirstPeter 2:13-15)</summary>
        NotSubmittingToMansLaws = 400,

        /// <summary>Abstaining From Eating Meat (FirstTimothy 4:1-4)</summary>
        AbstainingFromEatingMeat = 401,

        /// <summary>Not Being Merciful (Colossians 3:12; Luke 6:36)</summary>
        NotBeingMerciful = 402,

        /// <summary>Following The Desires Of The Mind (Ephesians 2:3)</summary>
        FollowingTheDesiresOfTheMind = 403,

        /// <summary>Setting Your Mind On The Things Of The Earth (Colossians 3:2)</summary>
        SettingYourMindOnTheThingsOfTheEarth = 404,

        /// <summary>Double Minded (James 4:7-10; James 1:8)</summary>
        DoubleMinded = 405,

        /// <summary>Mischief (Proverbs 6:16; Proverbs 6:18; Proverbs 10:23; Psalms 28:3)</summary>
        Mischief = 406,

        /// <summary>Mockers (Proverbs 6:18; Jude 1:18)</summary>
        Mockers = 407,

        /// <summary>Murmuring (Philippians 2:14; John 6:43; Jude 1:16)</summary>
        Murmuring = 408,

        /// <summary>Murmuring About Just Wages (Matthew 20:11)</summary>
        MurmuringAboutJustWages = 409,

        /// <summary>Murmuring Among Yourselves (John 6:43)</summary>
        MurmuringAmongYourselves = 410,

        /// <summary>Naughty (Proverbs 6:12; James 1:21)</summary>
        Naughty = 411,

        /// <summary>Not Keeping Oaths (Numbers 30:2-16)</summary>
        NotKeepingOaths = 412,

        /// <summary>Not Obeying God'S Word (Hebrews 5:9; Acts 5:32)</summary>
        NotObeyingGodsWord = 413,

        /// <summary>Not Obeying Man'S Laws (FirstPeter 2:13-15)</summary>
        NotObeyingMansLaws = 414,

        /// <summary>Obeying Wrong Thoughts Rather Than God (Genesis 3:12; Genesis 3:13)</summary>
        ObeyingWrongThoughtsRatherThanGod = 415,

        /// <summary>Observing Days, Months, Times (Galatians 4:10)</summary>
        ObservingDaysMonthsTimes = 416,

        /// <summary>Obstinate (Stubborn) (Romans 10:21)</summary>
        ObstinateStubborn = 417,

        /// <summary>Studying The Occult (Deuteronomy 18:9-14; Leviticus 20:6; Isaiah 8:19; Isaiah 47:13; Isaiah 47:14)</summary>
        StudyingTheOccult = 418,

        /// <summary>Offending (Matthew 13:41; Matthew 17:27; James 3:2)</summary>
        Offending = 419,

        /// <summary>Offending A Little One (Matthew 18:6; Mark 9:42; Luke 17:2; FirstCorinthians 8:9-13; Romans 14:20; Romans 14:21)</summary>
        OffendingALittleOne = 420,

        /// <summary>Being Offended Because Of Jesus (Luke 7:23; Mark 14:27; Mark 14:29; Mark 4:17; Matthew 26:33; Matthew 24:10; Matthew 13:21)</summary>
        BeingOffendedBecauseOfJesus = 421,

        /// <summary>Not Keeping A Vow-Freewill Offering (Deuteronomy 23:21-23)</summary>
        NotKeepingAVowFreewillOffering = 422,

        /// <summary>Omitting Judgment, Mercy, Faith (Matthew 23:23)</summary>
        OmittingJudgmentMercyFaith = 423,

        /// <summary>Oppressing The Widow Or Fatherless Child (Exodus 22:22-24)</summary>
        OppressingTheWidowOrFatherlessChild = 424,

        /// <summary>Following Ordinances, Touch, Taste, Handle (Colossians 2:20; Colossians 2:21)</summary>
        FollowingOrdinancesTouchTasteHandle = 425,

        /// <summary>Revellings (Orgies) (Galatians 5:21)</summary>
        RevellingsOrgies = 426,

        /// <summary>Overindulging (Surfeiting) (Luke 21:34)</summary>
        OverindulgingSurfeiting = 427,

        /// <summary>Partiality (James 2:1-5)</summary>
        Partiality = 428,

        /// <summary>Not Being Patient (Luke 21:19; FirstTimothy 3:3; SecondTimothy 2:24; Hebrews 10:36; James 5:8)</summary>
        NotBeingPatient = 429,

        /// <summary>Not Being Perfect (Matthew 5:48; Genesis 17:1; FirstKings 8:61; SecondCorinthians 13:11)</summary>
        NotBeingPerfect = 430,

        /// <summary>Causing A Weak Brother To Perish (FirstCorinthians 8:11; FirstCorinthians 8:12)</summary>
        CausingAWeakBrotherToPerish = 431,

        /// <summary>Persecuting (FirstTimothy 1:13)</summary>
        Persecuting = 432,

        /// <summary>Philosophy (Colossians 2:8)</summary>
        Philosophy = 433,

        /// <summary>Playing Religion (If God Were Your Father Ye Would Love Me) (John 8:42)</summary>
        PlayingReligionIfGodWereYourFatherYeWouldLoveMe = 434,

        /// <summary>Living In Pleasure (FirstTimothy 5:6; James 5:5)</summary>
        LivingInPleasure = 435,

        /// <summary>Take Pleasure In Your Own Deceivings (SecondPeter 2:13)</summary>
        TakePleasureInYourOwnDeceivings = 436,

        /// <summary>Being Enslaved By Passions And Pleasures (Titus 3:3)</summary>
        BeingEnslavedByPassionsAndPleasures = 437,

        /// <summary>Love Pleasures More Than God (SecondTimothy 3:4)</summary>
        LovePleasuresMoreThanGod = 438,

        /// <summary>Being Against The Poor (FirstTimothy 5:6; Luke 8:14; Psalms 10:8; Psalms 10:9)</summary>
        BeingAgainstThePoor = 439,

        /// <summary>Despising The Poor (James 2:6)</summary>
        DespisingThePoor = 440,

        /// <summary>Not Giving To The Poor-Cursed (Proverbs 28:27; Luke 18:22; Luke 19:8; Luke 19:9; Luke 11:41)</summary>
        NotGivingToThePoorCursed = 441,

        /// <summary>Not Helping A Poor Brother (Leviticus 25:35-40)</summary>
        NotHelpingAPoorBrother = 442,

        /// <summary>Not Helping The Poor And Needy (Ezekiel 16:49)</summary>
        NotHelpingThePoorAndNeedy = 443,

        /// <summary>Not Remembering The Poor (Galatians 2:10)</summary>
        NotRememberingThePoor = 444,

        /// <summary>Withholding Wages From The Poor (Deuteronomy 24:15)</summary>
        WithholdingWagesFromThePoor = 445,

        /// <summary>The Prayer Of The Wicked (Psalms 109:7)</summary>
        ThePrayerOfTheWicked = 446,

        /// <summary>The Prayer Of Those Who Turn From Hearing The Bible (Abomination) (Proverbs 28:9)</summary>
        ThePrayerOfThoseWhoTurnFromHearingTheBibleAbomination = 447,

        /// <summary>The Prayer To Consume What You Ask For On Your Lusts (James 4:3)</summary>
        ThePrayerToConsumeWhatYouAskForOnYourLusts = 448,

        /// <summary>Making Long Meaningless Prayer (Matthew 23:14)</summary>
        MakingLongMeaninglessPrayer = 449,

        /// <summary>Making Long Prayer (For Pretense) To Be Seen Of Men (Mark 12:40)</summary>
        MakingLongPrayerForPretenseToBeSeenOfMen = 450,

        /// <summary>Not Praying, Prayerless (FirstSamuel 12:23; FirstThessalonians 5:17)</summary>
        NotPrayingPrayerless = 451,

        /// <summary>Not Praying For Them Which Despitefully Use You (Luke 6:28)</summary>
        NotPrayingForThemWhichDespitefullyUseYou = 452,

        /// <summary>Not Praying In Jesus Name (John 14:13; Psalms 66:18)</summary>
        NotPrayingInJesusName = 453,

        /// <summary>A False Teacher That Is Allowed To Teach (Revelation 2:20)</summary>
        AFalseTeacherThatIsAllowedToTeach = 454,

        /// <summary>Appointed False Priests Or Being A False Shepherd (FirstKings 13:33; FirstKings 13:34)</summary>
        AppointedFalsePriestsOrBeingAFalseShepherd = 455,

        /// <summary>Following False Christs, False Prophets (Preachers) (Matthew 24:11; Matthew 24:24)</summary>
        FollowingFalseChristsFalseProphetsPreachers = 456,

        /// <summary>Going To Preachers Or Teachers That Do Not Teach Sound Doctrine, Who Say What You Want To Hear (SecondTimothy 4:3)</summary>
        GoingToPreachersOrTeachersThatDoNotTeachSoundDoctrineWhoSayWhatYouWantToHear = 457,

        /// <summary>Not Obeying Your Preacher. Not Obeying Your Preacher (In The Lord) (Hebrews 13:17; Matthew 23:2; Matthew 23:3)</summary>
        NotObeyingYourPreacherNotObeyingYourPreacherInTheLord = 458,

        /// <summary>To Depart From A Preacher That Is Teaching God'S Word Because The Word Is Taught Uncompromised (John 6:60; John 6:66; SecondTimothy 4:3; SecondTimothy 4:4; SecondTimothy 4:1; SecondCorinthians 11:13-15)</summary>
        ToDepartFromAPreacherThatIsTeachingGodsWordBecauseTheWordIsTaughtUncompromised = 459,

        /// <summary>Preaching After The Traditions Of Men (FirstCorinthians 2:13)</summary>
        PreachingAfterTheTraditionsOfMen = 460,

        /// <summary>Preaching, Seeking The Glory Of Men (FirstThessalonians 2:4-6)</summary>
        PreachingSeekingTheGloryOfMen = 461,

        /// <summary>Preaching To Please Men (FirstThessalonians 2:4-6)</summary>
        PreachingToPleaseMen = 462,

        /// <summary>Preaching With Doctrines Other Than The Bible (FirstTimothy 1:3; FirstTimothy 1:4; FirstTimothy 1:6)</summary>
        PreachingWithDoctrinesOtherThanTheBible = 463,

        /// <summary>Preaching With Fables (FirstTimothy 1:3; FirstTimothy 1:4; FirstTimothy 1:6)</summary>
        PreachingWithFables = 464,

        /// <summary>Preaching With Flattering Words (FirstThessalonians 2:4-6)</summary>
        PreachingWithFlatteringWords = 465,

        /// <summary>Preaching With Foolishness (Joking) (Ephesians 5:4)</summary>
        PreachingWithFoolishnessJoking = 466,

        /// <summary>Preaching With Man'S Wisdom (FirstCorinthians 2:13)</summary>
        PreachingWithMansWisdom = 467,

        /// <summary>Preaching With Philosophy And Vain Deceit (Colossians 2:4; Colossians 2:8)</summary>
        PreachingWithPhilosophyAndVainDeceit = 468,

        /// <summary>Preaching With The Rudiments Of The World (Colossians 2:4; Colossians 2:8)</summary>
        PreachingWithTheRudimentsOfTheWorld = 469,

        /// <summary>Preaching With Things Which Minister Questions (FirstTimothy 1:3; FirstTimothy 1:4; FirstTimothy 1:6)</summary>
        PreachingWithThingsWhichMinisterQuestions = 470,

        /// <summary>Preaching With Vain Babblings (SecondTimothy 2:16)</summary>
        PreachingWithVainBabblings = 471,

        /// <summary>Preaching With Vain Jangling (FirstTimothy 1:3; FirstTimothy 1:4; FirstTimothy 1:6)</summary>
        PreachingWithVainJangling = 472,

        /// <summary>Preaching With Vain Words (Ephesians 5:6)</summary>
        PreachingWithVainWords = 473,

        /// <summary>Preaching With The Wisdom Of Man'S Words (FirstCorinthians 1:17)</summary>
        PreachingWithTheWisdomOfMansWords = 474,

        /// <summary>Prejudice, Having Respect Of Persons (James 2:1-4; James 2:6; James 2:9)</summary>
        PrejudiceHavingRespectOfPersons = 475,

        /// <summary>Prejudice, Not Esteeming Others Better Than Yourself (Philippians 2:3)</summary>
        PrejudiceNotEsteemingOthersBetterThanYourself = 476,

        /// <summary>Not Honoring The President (King) (FirstPeter 2:17)</summary>
        NotHonoringThePresidentKing = 477,

        /// <summary>Pride (Proud) (James 4:6; Romans 1:30; Proverbs 16:18; Psalms 10:4)</summary>
        PrideProud = 478,

        /// <summary>Exalting Yourself (Pride) (Luke 18:10-14)</summary>
        ExaltingYourselfPride = 479,

        /// <summary>Proud Look, The Lord Hates (An Abomination) (Proverbs 6:16; Proverbs 6:17)</summary>
        ProudLookTheLordHatesAnAbomination = 480,

        /// <summary>Provide Not For His Own (James 4:6; FirstTimothy 5:8)</summary>
        ProvideNotForHisOwn = 481,

        /// <summary>Provoking Your Children (Ephesians 6:4)</summary>
        ProvokingYourChildren = 482,

        /// <summary>Quarreling (SecondCorinthians 12:20)</summary>
        Quarreling = 483,

        /// <summary>To Be Called Rabbi (Matthew 23:8)</summary>
        ToBeCalledRabbi = 484,

        /// <summary>Fits Of Rage (Wrath) (Galatians 5:20)</summary>
        FitsOfRageWrath = 485,

        /// <summary>Rape (Death Sentence In Certain Cases) (Deuteronomy 22:25-27)</summary>
        RapeDeathSentenceInCertainCases = 486,

        /// <summary>Rebellion Against God (Daniel 9:5; Daniel 9:9)</summary>
        RebellionAgainstGod = 487,

        /// <summary>A Rebellious Son (Son To Be Stoned) (Deuteronomy 21:18-21)</summary>
        ARebelliousSonSonToBeStoned = 488,

        /// <summary>Rebuking An Elder (FirstTimothy 5:1)</summary>
        RebukingAnElder = 489,

        /// <summary>Refuse To Go Where The Holy Ghost Bids You (Acts 16:6)</summary>
        RefuseToGoWhereTheHolyGhostBidsYou = 490,

        /// <summary>Refuse To Lay Down Your Life For The Brethren (FirstJohn 3:16)</summary>
        RefuseToLayDownYourLifeForTheBrethren = 491,

        /// <summary>Refuse To Let Jesus Reign Over You (Luke 19:14; Luke 19:27; Luke 20:9-18)</summary>
        RefuseToLetJesusReignOverYou = 492,

        /// <summary>Refuse To Repent Repent So You Can Believe (Acts 2:38; Luke 13:3; Luke 13:5; Matthew 4:17; Matthew 21:32; John 3:19-21; Genesis 4:7)</summary>
        RefuseToRepentRepentSoYouCanBelieve = 493,

        /// <summary>Resisting The Holy Spirit (Acts 7:51)</summary>
        ResistingTheHolySpirit = 494,

        /// <summary>Riches, Refusing To Keep Free From The Love Of Money (Hebrews 13:5)</summary>
        RichesRefusingToKeepFreeFromTheLoveOfMoney = 495,

        /// <summary>Riches, The Love Of Money (Filthy Lucre-Covetous) (FirstTimothy 3:3; FirstTimothy 6:10; SecondTimothy 3:2)</summary>
        RichesTheLoveOfMoneyFilthyLucreCovetous = 496,

        /// <summary>Riches, Laying Up Treasures On Earth (Matthew 6:19-21; Luke 12:16-21; Luke 14:33; Philippians 3:8; James 5:1-3)</summary>
        RichesLayingUpTreasuresOnEarth = 497,

        /// <summary>Riches, Putting Personal Belongings Before God (Revelation 9:20)</summary>
        RichesPuttingPersonalBelongingsBeforeGod = 498,

        /// <summary>Riches, Seeking To Be Rich (FirstTimothy 6:9)</summary>
        RichesSeekingToBeRich = 499,

        /// <summary>Riches, Trusting In Riches (Mark 10:24; FirstTimothy 6:17)</summary>
        RichesTrustingInRiches = 500,

        /// <summary>To Think Yourself Righteous (Matthew 13:22; Mark 2:17)</summary>
        ToThinkYourselfRighteous = 501,

        /// <summary>Rioting (SecondPeter 2:13)</summary>
        Rioting = 502,

        /// <summary>Riotous Living (Luke 15:13; Luke 15:18)</summary>
        RiotousLiving = 503,

        /// <summary>Not Keeping The Sabbath Or Day Of The Lord (Exodus 20:8; Exodus 20:9; Nehemiah 13:15-19)</summary>
        NotKeepingTheSabbathOrDayOfTheLord = 504,

        /// <summary>Salvation, Saying Can'T Turn From Righteousness And Perish (Ezekiel 18:24; Ezekiel 18:25)</summary>
        SalvationSayingCantTurnFromRighteousnessAndPerish = 505,

        /// <summary>Buying And Selling For Personal Profit In God'S Sanctuary (Mark 11:15)</summary>
        BuyingAndSellingForPersonalProfitInGodsSanctuary = 506,

        /// <summary>Not Reverencing God'S Sanctuary (Leviticus 26:2)</summary>
        NotReverencingGodsSanctuary = 507,

        /// <summary>To Say, The Lord Hath Forsaken The Earth (Ezekiel 8:12)</summary>
        ToSayTheLordHathForsakenTheEarth = 508,

        /// <summary>To Say, The Lord Seeth Us Not (Ezekiel 8:12)</summary>
        ToSayTheLordSeethUsNot = 509,

        /// <summary>Scoffers (SecondPeter 3:3)</summary>
        Scoffers = 510,

        /// <summary>Being Scornful (Psalms 1:1; Proverbs 9:12; Proverbs 24:9)</summary>
        BeingScornful = 511,

        /// <summary>Seeking A Sign (Matthew 16:4; Luke 11:29)</summary>
        SeekingASign = 512,

        /// <summary>Not Seeking First The Kingdom Of God (Matthew 6:33)</summary>
        NotSeekingFirstTheKingdomOfGod513 = 513,

        /// <summary>Selfishness (Philippians 2:21; SecondTimothy 3:2; ThirdJohn 1:9-11)</summary>
        Selfishness = 514,

        /// <summary>Self Ish Ambition (Galatians 5:19; Galatians 5:20)</summary>
        SelfIshAmbition = 515,

        /// <summary>Self-Righteous (High Minded) Self-Condemned (Luke 16:15; Romans 11:20; Job 9:20; Matthew 5:20)</summary>
        SelfRighteousHighMindedSelfCondemned = 516,

        /// <summary>Self-Serving (Luke 9:23)</summary>
        SelfServing = 517,

        /// <summary>Self-Willed (SecondPeter 2:10)</summary>
        SelfWilled = 518,

        /// <summary>Sex Outside Marriage (Fornication) (FirstCorinthians 5:11; FirstCorinthians 6:18; FirstCorinthians 6:20; Exodus 22:16; Exodus 22:17)</summary>
        SexOutsideMarriageFornication = 519,

        /// <summary>Even A Hint Of Sexual Immorality (Sin) (Ephesians 5:3)</summary>
        EvenAHintOfSexualImmoralitySin = 520,

        /// <summary>Sex With A Prostitute (FirstCorinthians 6:15-18)</summary>
        SexWithAProstitute = 521,

        /// <summary>Sex With A Virgin-Marry Her (Deuteronomy 22:28; Deuteronomy 22:29)</summary>
        SexWithAVirginMarryHer = 522,

        /// <summary>Sex With A Virgin (Betrothed-If She Doesn'T Cry Out-Both Die) (Deuteronomy 22:23; Deuteronomy 22:24)</summary>
        SexWithAVirginBetrothedIfSheDoesntCryOutBothDie = 523,

        /// <summary>Sex With A Virgin (Betrothed-If She Cries Out-He Dies) (Deuteronomy 27:20-23)</summary>
        SexWithAVirginBetrothedIfSheCriesOutHeDies = 524,

        /// <summary>Sex With A Relative-Cursed (Deuteronomy 27:20-23)</summary>
        SexWithARelativeCursed = 525,

        /// <summary>Idol (Worthless) Shepherd Leaving The Flock (John 10:12; Zechariah 11:17)</summary>
        IdolWorthlessShepherdLeavingTheFlock = 526,

        /// <summary>Shepherd, Not Searching For God'S Flock (Ezekiel 34:6-8)</summary>
        ShepherdNotSearchingForGodsFlock = 527,

        /// <summary>Shepherd, Not Feeding God'S Flock (Ezekiel 34:8)</summary>
        ShepherdNotFeedingGodsFlock = 528,

        /// <summary>Shepherd, Not Strengthen The Diseased (Ezekiel 34:4)</summary>
        ShepherdNotStrengthenTheDiseased = 529,

        /// <summary>Shepherd, Did Not Bind Up That Which Was Broken (Ezekiel 34:4)</summary>
        ShepherdDidNotBindUpThatWhichWasBroken = 530,

        /// <summary>Shepherd, Did Not Bring Again That Which Was Driven Away (Ezekiel 34:4)</summary>
        ShepherdDidNotBringAgainThatWhichWasDrivenAway = 531,

        /// <summary>Shepherd, Ruled With Cruelty (Ezekiel 34:4)</summary>
        ShepherdRuledWithCruelty = 532,

        /// <summary>Enticing A Nation To Sin (FirstKings 14:16)</summary>
        EnticingANationToSin = 533,

        /// <summary>Know To Do Good, But Don'T Do It, Sin (James 4:17)</summary>
        KnowToDoGoodButDontDoItSin = 534,

        /// <summary>Mocking Sin (Fools) (Proverbs 14:9)</summary>
        MockingSinFools = 535,

        /// <summary>Not Ashamed Of Your Sin (Jeremiah 8:12)</summary>
        NotAshamedOfYourSin = 536,

        /// <summary>Not Confessing Your Sin (FirstJohn 1:9; James 5:16; Psalms 32:5; Proverbs 28:13)</summary>
        NotConfessingYourSin = 537,

        /// <summary>Sin, Yet Not Know It, Still Sin (Leviticus 4:13; Leviticus 4:14; Leviticus 4:23)</summary>
        SinYetNotKnowItStillSin = 538,

        /// <summary>Yielding Your Members Unto Sin (Romans 6:13)</summary>
        YieldingYourMembersUntoSin = 539,

        /// <summary>Say You Have Not Sinned (FirstJohn 1:10)</summary>
        SayYouHaveNotSinned = 540,

        /// <summary>Refuse To Admit You Are A Sinner (Jeremiah 8:6; FirstJohn 1:8; FirstJohn 1:10)</summary>
        RefuseToAdmitYouAreASinner = 541,

        /// <summary>Refuse To Confess Your Sins (FirstJohn 1:9)</summary>
        RefuseToConfessYourSins = 542,

        /// <summary>Slander (Backbiters) (FirstJohn 3:6; Mark 7:20-23; Romans 1:30; SecondCorinthians 12:20)</summary>
        SlanderBackbiters = 543,

        /// <summary>Slandering Your Neighbor In Secret (Psalms 101:5)</summary>
        SlanderingYourNeighborInSecret = 544,

        /// <summary>Smoking (Romans 12:1; Romans 13:14; FirstCorinthians 3:16-18; FirstCorinthians 6:20; SecondCorinthians 4:10; FirstPeter 1:15; FirstThessalonians 4:4; FirstThessalonians 4:5)</summary>
        Smoking = 545,

        /// <summary>Sorceries (Acts 8:11)</summary>
        Sorceries = 546,

        /// <summary>To Start A Fire That Does Damage-Not Making Restitution (Exodus 22:6)</summary>
        ToStartAFireThatDoesDamageNotMakingRestitution = 547,

        /// <summary>Stealing (Exodus 20:15; Mark 7:22)</summary>
        Stealing = 548,

        /// <summary>Strife In Your Hearts (James 3:14)</summary>
        StrifeInYourHearts = 549,

        /// <summary>Not Striving To Enter Heaven (Luke 13:24)</summary>
        NotStrivingToEnterHeaven = 550,

        /// <summary>Not Striving To Enter Heaven-Not Watching And Praying May Be Accounted Worthy (Luke 21:36)</summary>
        NotStrivingToEnterHeavenNotWatchingAndPrayingMayBeAccountedWorthy = 551,

        /// <summary>Stubborn, Stubbornness Is As Iniquity And Idolatry (Isaiah 48:4-8; FirstSamuel 15:23)</summary>
        StubbornStubbornnessIsAsIniquityAndIdolatry = 552,

        /// <summary>Stubborn Son-Death, He Is Cursed (Deuteronomy 21:18-21; Deuteronomy 27:16; Mark 7:10)</summary>
        StubbornSonDeathHeIsCursed = 553,

        /// <summary>Stubborn Son-Refusing Too Bey Parents In The Lord (Ephesians 6:1)</summary>
        StubbornSonRefusingTooBeyParentsInTheLord = 554,

        /// <summary>Faring Sumptuously, Fullness Of Bread (Ezekiel 16:49)</summary>
        FaringSumptuouslyFullnessOfBread = 555,

        /// <summary>Faring Sumptuously, Belly Is Your God (Philippians 3:19)</summary>
        FaringSumptuouslyBellyIsYourGod = 556,

        /// <summary>Faring Sumptuously, Living In Luxury Everyday (Luke 16:19)</summary>
        FaringSumptuouslyLivingInLuxuryEveryday = 557,

        /// <summary>Faring Sumptuously, Full Of Excess (Matthew 23:25)</summary>
        FaringSumptuouslyFullOfExcess = 558,

        /// <summary>Swearing (Jeremiah 23:10)</summary>
        Swearing = 559,

        /// <summary>Taking By Force (FirstSamuel 2:16)</summary>
        TakingByForce = 560,

        /// <summary>Tattlers (FirstTimothy 5:13)</summary>
        Tattlers = 561,

        /// <summary>Tattooing (Leviticus 19:28; Deuteronomy 14:1)</summary>
        Tattooing = 562,

        /// <summary>Not Paying All The Taxes You Owe (Romans 13:6-8; Luke 20:25)</summary>
        NotPayingAllTheTaxesYouOwe = 563,

        /// <summary>Teaching Any Doctrine, Other Than The Bible (FirstTimothy 1:3)</summary>
        TeachingAnyDoctrineOtherThanTheBible = 564,

        /// <summary>Television, Watching Things That Are Contrary To The Bible (Psalms 101:3; James 1:27)</summary>
        TelevisionWatchingThingsThatAreContraryToTheBible = 565,

        /// <summary>Watching Sin Or Worldliness (Psalms 101:3; James 1:27)</summary>
        WatchingSinOrWorldliness = 566,

        /// <summary>Losing Your Temper (Anger) (Colossians 3:8) The source list numbers two entries 566; this second one gets id 668 so ids stay unique.</summary>
        LosingYourTemperAnger = 668,

        /// <summary>Being Quick-Tempered (Proverbs 14:17)</summary>
        BeingQuickTempered = 567,

        /// <summary>Causing Tension (Strife) (Philippians 2:3)</summary>
        CausingTensionStrife = 568,

        /// <summary>Not Giving Thanks In All Circumstances, Not Thankful To God (FirstThessalonians 5:18; Romans 1:21)</summary>
        NotGivingThanksInAllCircumstancesNotThankfulToGod = 569,

        /// <summary>Doing Things That Are Not In God'S Will (Matthew 7:21)</summary>
        DoingThingsThatAreNotInGodsWill = 570,

        /// <summary>Doing Things That Are Not Pleasing To God (FirstThessalonians 2:15)</summary>
        DoingThingsThatAreNotPleasingToGod = 571,

        /// <summary>Thinking How To Fulfill The Desires Of The Mind, And Of The Sinful Nature (Romans 13:13; Romans 13:14)</summary>
        ThinkingHowToFulfillTheDesiresOfTheMindAndOfTheSinfulNature = 572,

        /// <summary>Taking Thought For Tomorrow (Matthew 6:34)</summary>
        TakingThoughtForTomorrow = 573,

        /// <summary>Taking Thought For Your Life, What To Eat, Drink, Put On (Matthew 6:25)</summary>
        TakingThoughtForYourLifeWhatToEatDrinkPutOn = 574,

        /// <summary>Evil Thoughts (Matthew 15:19; Mark 7:21)</summary>
        EvilThoughts = 575,

        /// <summary>Not Giving Tithes And Offerings (Cursed) (Malachi 3:8; Malachi 3:9; Leviticus 27:32; Deuteronomy 14:22)</summary>
        NotGivingTithesAndOfferingsCursed = 576,

        /// <summary>Tongue, Calling Evil Good And Good Evil (Isaiah 5:20)</summary>
        TongueCallingEvilGoodAndGoodEvil = 577,

        /// <summary>Tongue, Cursing Them That Hate You (Job 31:29; Job 31:30; Matthew 5:44)</summary>
        TongueCursingThemThatHateYou = 578,

        /// <summary>Tongue, Idle Words, Sin With Tongue (Matthew 12:36; Psalms 39:1)</summary>
        TongueIdleWordsSinWithTongue = 579,

        /// <summary>Tongue, Slandering Or Speaking Against A Child Of God (Psalms 50:20; Proverbs 6:19)</summary>
        TongueSlanderingOrSpeakingAgainstAChildOfGod = 580,

        /// <summary>Tongue, Speaking Words Of Hatred (Psalms 109:3)</summary>
        TongueSpeakingWordsOfHatred = 581,

        /// <summary>The Tongue Is A World Of Sin (James 3:3-13)</summary>
        TheTongueIsAWorldOfSin = 582,

        /// <summary>Forbidding To Speak In Tongues (FirstCorinthians 14:39)</summary>
        ForbiddingToSpeakInTongues = 583,

        /// <summary>Following The Traditions Of Men (Colossians 2:8)</summary>
        FollowingTheTraditionsOfMen = 584,

        /// <summary>Traditions That Override God'S Commandments (Matthew 15:3)</summary>
        TraditionsThatOverrideGodsCommandments = 585,

        /// <summary>Traitorous (SecondTimothy 3:4; Luke 6:16)</summary>
        Traitorous = 586,

        /// <summary>Trusting In Ourselves (SecondCorinthians 1:9)</summary>
        TrustingInOurselves = 587,

        /// <summary>Truce Breaking (SecondTimothy 3:3)</summary>
        TruceBreaking = 588,

        /// <summary>Unbelieving (Revelation 21:8; Hebrews 3:12)</summary>
        Unbelieving = 589,

        /// <summary>Uncleanness (Matthew 23:27)</summary>
        Uncleanness = 590,

        /// <summary>Ungrateful (SecondTimothy 3:1; SecondTimothy 3:2)</summary>
        Ungrateful = 591,

        /// <summary>Unfaithful With Another'S Items (Luke 16:10; Luke 16:12)</summary>
        UnfaithfulWithAnothersItems = 592,

        /// <summary>Unmerciful (Romans 1:31)</summary>
        Unmerciful = 593,

        /// <summary>Unrighteousness (FirstJohn 5:17)</summary>
        Unrighteousness = 594,

        /// <summary>Unthankful (Romans 1:21; SecondTimothy 3:2)</summary>
        Unthankful = 595,

        /// <summary>Being Vain (James 2:20; Titus 1:10)</summary>
        BeingVain = 596,

        /// <summary>Making A Vow And Not Keeping It (Ecclesiastes 5:4-6)</summary>
        MakingAVowAndNotKeepingIt = 597,

        /// <summary>Being A Violent Man Or Doing Violence (Psalms 18:48; Luke 3:14; Ezekiel 8:17)</summary>
        BeingAViolentManOrDoingViolence = 598,

        /// <summary>Wages, Not Being Content With Your Wages (Luke 3:14; Matthew 20:1-16)</summary>
        WagesNotBeingContentWithYourWages = 599,

        /// <summary>Loving The Wages Of Unrighteousness (SecondPeter 2:15)</summary>
        LovingTheWagesOfUnrighteousness = 600,

        /// <summary>Paying Unfair Wages (James 5:4)</summary>
        PayingUnfairWages = 601,

        /// <summary>Not Watching For The Soon Return Of Jesus Christ (Revelation 3:3; Mark 13:35-37)</summary>
        NotWatchingForTheSoonReturnOfJesusChrist = 602,

        /// <summary>Watching Evil (Psalms 101:3)</summary>
        WatchingEvil = 603,

        /// <summary>Whisperer (Proverbs 16:28)</summary>
        Whisperer = 604,

        /// <summary>Wickedness (Romans 1:29)</summary>
        Wickedness = 605,

        /// <summary>Being Wise In Your Own Eyes (Isaiah 5:21)</summary>
        BeingWiseInYourOwnEyes = 606,

        /// <summary>Being A Witch, Or Having Anything To Do With A Witch (Deuteronomy 18:10; Deuteronomy 18:1)</summary>
        BeingAWitchOrHavingAnythingToDoWithAWitch = 607,

        /// <summary>Witchcraft (Galatians 5:20; Exodus 22:18)</summary>
        Witchcraft = 608,

        /// <summary>Rebellion As Witchcraft (FirstSamuel 15:23)</summary>
        RebellionAsWitchcraft = 609,

        /// <summary>Wives Not Submitting To Their Husbands (Ephesians 5:22)</summary>
        WivesNotSubmittingToTheirHusbands = 610,

        /// <summary>Women Wearing Men'S Clothes (Deuteronomy 22:5)</summary>
        WomenWearingMensClothes = 611,

        /// <summary>Adding Or Taking Away From The Words Of The Book Of This Prophecy (Revelation 22:18; Revelation 22:19)</summary>
        AddingOrTakingAwayFromTheWordsOfTheBookOfThisProphecy = 612,

        /// <summary>Being Stiff Necked Towards God'S Word (Acts 7:51)</summary>
        BeingStiffNeckedTowardsGodsWord = 613,

        /// <summary>Contradicting The Word Of God (Matthew 16:22; Matthew 16:23)</summary>
        ContradictingTheWordOfGod = 614,

        /// <summary>Corrupting The Word Of God (SecondCorinthians 2:17)</summary>
        CorruptingTheWordOfGod = 615,

        /// <summary>Despising The Word Of God (Isaiah 5:24; Numbers 15:30; Numbers 15:31)</summary>
        DespisingTheWordOfGod = 616,

        /// <summary>Disobeying The Word Of God (Genesis 2:16; Genesis 2:17; Genesis 3:12; Genesis 3:13; FirstJohn 3:4)</summary>
        DisobeyingTheWordOfGod = 617,

        /// <summary>Distorting The Word Of God (The Truth) (SecondPeter 3:16; Acts 20:29; Acts 20:30)</summary>
        DistortingTheWordOfGodTheTruth = 618,

        /// <summary>Not Turning Away From False Science That Contradicts God'S Word (FirstTimothy 6:20)</summary>
        NotTurningAwayFromFalseScienceThatContradictsGodsWord = 619,

        /// <summary>Forgetting God'S Truth (James 1:25; Proverbs 3:1)</summary>
        ForgettingGodsTruth = 620,

        /// <summary>Handling God'S Word Deceitful (SecondCorinthians 4:2)</summary>
        HandlingGodsWordDeceitful = 621,

        /// <summary>Hating Instruction (Psalms 50:17)</summary>
        HatingInstruction = 622,

        /// <summary>Making God'S Word Void By Traditions (Mark 7:13)</summary>
        MakingGodsWordVoidByTraditions = 623,

        /// <summary>Not Believing All Of The Bible (All Of God'S Word) (Fool) (FirstJohn 5:10; Luke 24:25; John 8:47; John 10:26; John 12:48)</summary>
        NotBelievingAllOfTheBibleAllOfGodsWordFool = 624,

        /// <summary>Not Doing God'S Word (Deceived) (James 1:22; Luke 6:46; Luke 8:21; Ezekiel 33:32)</summary>
        NotDoingGodsWordDeceived = 625,

        /// <summary>Not Hiding God'S Word In Your Heart (Psalms 119:11)</summary>
        NotHidingGodsWordInYourHeart = 626,

        /// <summary>Not Keeping The Words Of Jesus (John 14:24)</summary>
        NotKeepingTheWordsOfJesus = 627,

        /// <summary>Not Living By All Of God'S Word (Matthew 4:4; Luke 4:4)</summary>
        NotLivingByAllOfGodsWord = 628,

        /// <summary>Not Obeying God'S Word (The Gospel) (Luke 6:46; Luke 6:49; Acts 5:32; Romans 1:5; SecondThessalonians 1:8; FirstTimothy 4:16; Hebrews 5:9)</summary>
        NotObeyingGodsWordTheGospel = 629,

        /// <summary>Not Putting God'S Word In Your Heart (Deuteronomy 6:6)</summary>
        NotPuttingGodsWordInYourHeart = 630,

        /// <summary>Not Reading The Bible (Daily) (Psalms 1:2; Acts 17:11; FirstPeter 2:2; FirstTimothy 4:13)</summary>
        NotReadingTheBibleDaily = 631,

        /// <summary>Not Receiving The Love Of The Truth (SecondThessalonians 2:10)</summary>
        NotReceivingTheLoveOfTheTruth = 632,

        /// <summary>Not Rightly Dividing God'S Word (SecondTimothy 2:15)</summary>
        NotRightlyDividingGodsWord = 633,

        /// <summary>Not Studying God'S Word (Proverbs 4:4; Proverbs 4:5; Proverbs 4:20; Proverbs 4:21; Joshua 1:8; SecondTimothy 2:15)</summary>
        NotStudyingGodsWord = 634,

        /// <summary>Not Teaching God'S Word To Your Children (Deuteronomy 6:7)</summary>
        NotTeachingGodsWordToYourChildren = 635,

        /// <summary>Not Tolerating God'S Word (SecondTimothy 4:3)</summary>
        NotToleratingGodsWord = 636,

        /// <summary>Not Valiant For The Truth (For God'S Word) (Jeremiah 9:3)</summary>
        NotValiantForTheTruthForGodsWord = 637,

        /// <summary>Perverting The Gospel (Galatians 1:7; Galatians 1:8)</summary>
        PervertingTheGospel = 638,

        /// <summary>Rejecting The Word Of God (FirstSamuel 15:23; FirstSamuel 15:24; Jeremiah 8:9; Luke 7:30)</summary>
        RejectingTheWordOfGod = 639,

        /// <summary>Resisting The Truth (The Word Of God) (SecondTimothy 3:8)</summary>
        ResistingTheTruthTheWordOfGod = 640,

        /// <summary>Stopping Your Ears From Hearing God'S Word (Acts 7:57)</summary>
        StoppingYourEarsFromHearingGodsWord = 641,

        /// <summary>Taking Away The Knowledge Of How To Be Saved (Luke 11:52)</summary>
        TakingAwayTheKnowledgeOfHowToBeSaved = 642,

        /// <summary>Taking Away From The Words Of This Prophecy (Revelation 22:19)</summary>
        TakingAwayFromTheWordsOfThisProphecy = 643,

        /// <summary>To Be Destitute Of The Truth (FirstTimothy 6:5)</summary>
        ToBeDestituteOfTheTruth = 644,

        /// <summary>To Err Concerning The Truth (SecondTimothy 2:18)</summary>
        ToErrConcerningTheTruth = 645,

        /// <summary>Trying To Enter Heaven By Away Other Than The Gospel'S Way (John 10:1-7)</summary>
        TryingToEnterHeavenByAwayOtherThanTheGospelsWay = 646,

        /// <summary>Twisting God'S Word (To Wrest Or Distort) (SecondPeter 3:16)</summary>
        TwistingGodsWordToWrestOrDistort = 647,

        /// <summary>Willingly Ignorant Of God'S Word (SecondPeter 3:5; FirstSamuel 15:19-24; Acts 5:32; Ezekiel 33:31)</summary>
        WillinglyIgnorantOfGodsWord = 648,

        /// <summary>A Man Who Will Not Work (SecondThessalonians 3:10)</summary>
        AManWhoWillNotWork = 649,

        /// <summary>Conforming To This World (Romans 12:2)</summary>
        ConformingToThisWorld = 650,

        /// <summary>Forsaking God'S Calling For This World (SecondTimothy 4:10)</summary>
        ForsakingGodsCallingForThisWorld = 651,

        /// <summary>Friendship Of The World (Is Hatred With God) (James 4:4)</summary>
        FriendshipOfTheWorldIsHatredWithGod = 652,

        /// <summary>Getting Entangled With The Affairs Of This Life (SecondTimothy 2:4)</summary>
        GettingEntangledWithTheAffairsOfThisLife = 653,

        /// <summary>Getting Spotted From The World (James 1:27)</summary>
        GettingSpottedFromTheWorld = 654,

        /// <summary>Living For Yourself (SecondCorinthians 5:15; Luke 9:23)</summary>
        LivingForYourself = 655,

        /// <summary>Loving This World (FirstJohn 2:15-17; James 1:27; James 4:4)</summary>
        LovingThisWorld = 656,

        /// <summary>Loving Your Life (John 12:25)</summary>
        LovingYourLife = 657,

        /// <summary>Mind Or Thoughts On This World (Philippians 3:18; Philippians 3:19)</summary>
        MindOrThoughtsOnThisWorld = 658,

        /// <summary>Serving Earthly Things (Romans 1:25)</summary>
        ServingEarthlyThings = 659,

        /// <summary>Speaking Of The World (John 3:31; FirstJohn 4:5; FirstJohn 4:6)</summary>
        SpeakingOfTheWorld = 660,

        /// <summary>Sorrow Of The World (SecondCorinthians 7:10)</summary>
        SorrowOfTheWorld = 661,

        /// <summary>Worldly Ambition (Acts 8:18-24; Luke 12:15-21)</summary>
        WorldlyAmbition = 662,

        /// <summary>Worldly Minded (Romans 8:5-7; Romans 8:13; Romans 12:2)</summary>
        WorldlyMinded = 663,

        /// <summary>Being Worried (Philippians 4:6; Matthew 6:25)</summary>
        BeingWorried = 664,

        /// <summary>False Worship (Isaiah 1:13)</summary>
        FalseWorship = 665,

        /// <summary>Letting The Sun Go Down On Your Wrath (Ephesians 4:26; Hebrews 3:13)</summary>
        LettingTheSunGoDownOnYourWrath = 666,

        /// <summary>Younger, Not Submitting To The Elder (FirstPeter 5:5)</summary>
        YoungerNotSubmittingToTheElder = 667,
    }
}
