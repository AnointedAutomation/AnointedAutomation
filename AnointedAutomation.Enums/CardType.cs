// Copyright © Anointed Automation, LLC., 2024. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2024-01-16 19:19:01
// Edited by Alexander Fields https://www.alexanderfields.me 2025-07-02 11:48:25
//Stewarded by Alexander Fields

namespace AnointedAutomation.Enums
{
    /// <summary>
    /// Defines the types of credit/debit cards.
    /// </summary>
    public enum CardType : int
    {
        /// <summary>
        /// Unknown or unrecognized card type.
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// Visa card (starts with 4).
        /// </summary>
        Visa = 1,
        /// <summary>
        /// MasterCard (starts with 51-55 or 2221-2720).
        /// </summary>
        MasterCard = 2,
        /// <summary>
        /// American Express (starts with 34 or 37).
        /// </summary>
        AmericanExpress = 3,
        /// <summary>
        /// Discover card (starts with 6011, 644-649, or 65).
        /// </summary>
        Discover = 4,
        /// <summary>
        /// Diners Club card (starts with 300-305, 36, or 38).
        /// </summary>
        DinersClub = 5,
        /// <summary>
        /// JCB card (starts with 3528-3589).
        /// </summary>
        JCB = 6,
        /// <summary>
        /// UnionPay card (starts with 62).
        /// </summary>
        UnionPay = 7,
        /// <summary>
        /// Maestro card (starts with 5018, 5020, 5038, 6304, 6759, 6761, 6763).
        /// </summary>
        Maestro = 8
    }
}
