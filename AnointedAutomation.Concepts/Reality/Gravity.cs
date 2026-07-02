// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

namespace AnointedAutomation.Concepts
{
    /// <summary>
    /// How heinous a wrong is, and so how much destruction it unleashes into reality. Scripture
    /// grades sin: not all wrongs are equal (Matthew 23:23, "the weightier matters of the law";
    /// John 19:11, "the greater sin"). The Torah reserves death for the gravest, capital wrongs
    /// (murder, the shedding of innocent blood, child sacrifice, the violation of a person), while
    /// answering lesser wrongs with restitution or rebuke. Ordered from least to most heinous so the
    /// weights can be compared.
    /// </summary>
    public enum Gravity
    {
        /// <summary>No wrong; introduces no disorder (a virtue).</summary>
        None = 0,

        /// <summary>A real wrong, but not heinous (rudeness, a boast, a grudge).</summary>
        Minor = 1,

        /// <summary>A grave wrong answered in the Law by restitution or rebuke (theft, withholding wages).</summary>
        Serious = 2,

        /// <summary>A heavy wrong that provokes God's judgment (oppression, rebellion, treachery).</summary>
        Grave = 3,

        /// <summary>The most heinous: a capital wrong (murder, innocent blood, child sacrifice, defilement).</summary>
        Capital = 4
    }
}
