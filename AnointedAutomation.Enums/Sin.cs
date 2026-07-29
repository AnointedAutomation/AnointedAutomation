// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-07-29 00:00:00
// Edited by Alexander Fields https://www.alexanderfields.me 2026-07-29 00:00:00
//Stewarded by Alexander Fields

namespace AnointedAutomation.Enums
{
	/// <summary>
	/// A prohibited way of being ("work of the flesh") that is not allowed on Anointed Automation
	/// platforms. Grounded in Galatians 5:19-21 and the classic deadly sins, these categorize why a
	/// person may be singled out or banned. Serialized as its unsigned integer value.
	/// </summary>
	public enum Sin : uint
	{
		/// <summary>
		/// Speaking impiously or irreverently of God, the sacred, or the Holy Spirit.
		/// </summary>
		Blasphemy = 0,

		/// <summary>
		/// Worship of, or devotion to, anything set up in the place of God.
		/// </summary>
		Idolatry = 1,

		/// <summary>
		/// Sorcery, witchcraft, or occult / demonic practice.
		/// </summary>
		Sorcery = 2,

		/// <summary>
		/// Sexual immorality, obscenity, or other impurity of the flesh.
		/// </summary>
		SexualImmorality = 3,

		/// <summary>
		/// Inordinate self-exaltation; contempt of others.
		/// </summary>
		Pride = 4,

		/// <summary>
		/// Inordinate desire for gain or possessions; covetousness.
		/// </summary>
		Greed = 5,

		/// <summary>
		/// Disordered craving, whether of the flesh or of possessions.
		/// </summary>
		Lust = 6,

		/// <summary>
		/// Uncontrolled anger, hatred, threats, or fits of rage.
		/// </summary>
		Wrath = 7,

		/// <summary>
		/// Resentment or jealousy of another's good; ill will.
		/// </summary>
		Envy = 8,

		/// <summary>
		/// Excess and self-indulgence, including drunkenness and carousing.
		/// </summary>
		Gluttony = 9,

		/// <summary>
		/// Habitual idleness or neglect of what one ought to do.
		/// </summary>
		Sloth = 10,

		/// <summary>
		/// Lying, fraud, bearing false witness, or deliberate deception.
		/// </summary>
		Deceit = 11,

		/// <summary>
		/// Taking what belongs to another; theft in any form.
		/// </summary>
		Theft = 12,

		/// <summary>
		/// The unlawful taking of life, or threats and violence against it.
		/// </summary>
		Murder = 13,
	}
}
