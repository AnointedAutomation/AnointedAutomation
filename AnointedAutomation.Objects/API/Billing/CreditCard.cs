// Copyright © Anointed Automation, LLC., 2024. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2024-01-16 19:19:01
// Edited by Alexander Fields https://www.alexanderfields.me 2025-07-02 11:48:25
//Stewarded by Alexander Fields

using System.Runtime.Serialization;
using AnointedAutomation.Enums;

namespace AnointedAutomation.Objects.Billing
{
    /// <summary>
    /// Represents a credit card with payment information, validation, and security features.
    /// Includes Luhn algorithm validation and card type detection.
    /// </summary>
    [System.Serializable]
    public class CreditCard
    {
        private string _cardNumber;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreditCard"/> class with specified properties.
        /// </summary>
        /// <param name="cardNumber">The credit card number.</param>
        /// <param name="cardHolderName">The name of the card holder.</param>
        /// <param name="expirationDate">The card's expiration date.</param>
        /// <param name="cvv">The card's security code (CVV).</param>
        public CreditCard(string cardNumber, string cardHolderName, System.DateTime expirationDate, int cvv)
        {
            CardNumber = cardNumber;
            CardHolderName = cardHolderName;
            this.expirationDate = expirationDate;
            this.cvv = cvv;
            DetectAndSetCardType();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreditCard"/> class.
        /// </summary>
        public CreditCard()
        {
        }

        /// <summary>
        /// Gets or sets the name of the card holder.
        /// </summary>
        [DataMember]
        public string CardHolderName
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the credit card number.
        /// </summary>
        [DataMember]
        public string CardNumber
        {
            get { return _cardNumber; }
            set
            {
                _cardNumber = value;
                if (!string.IsNullOrEmpty(value))
                {
                    string digitsOnly = GetDigitsOnly(value);
                    LastFourDigits = digitsOnly.Length >= 4 ? digitsOnly.Substring(digitsOnly.Length - 4) : digitsOnly;
                    MaskedNumber = MaskCardNumber(value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the last four digits of the card number.
        /// </summary>
        [DataMember]
        public string LastFourDigits
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the masked card number (e.g., "****-****-****-1234").
        /// </summary>
        [DataMember]
        public string MaskedNumber
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the detected card type.
        /// </summary>
        [DataMember]
        public CardType CardType
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the card's security code (CVV).
        /// Note: CVV should never be stored permanently. Use only for immediate processing.
        /// </summary>
        public int cvv
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the card's expiration date.
        /// </summary>
        [DataMember]
        public System.DateTime expirationDate
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the expiration month (1-12).
        /// </summary>
        [DataMember]
        public int ExpirationMonth
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the expiration year (4-digit).
        /// </summary>
        [DataMember]
        public int ExpirationYear
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the card funding type (credit, debit, prepaid).
        /// </summary>
        [DataMember]
        public string Funding
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the issuing bank name.
        /// </summary>
        [DataMember]
        public string IssuingBank
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the card's country of issue (ISO 2-letter code).
        /// </summary>
        [DataMember]
        public string Country
        {
            get; set;
        }

        /// <summary>
        /// Checks if the credit card is still valid based on its expiration date.
        /// </summary>
        /// <returns>True if the card has not expired, false otherwise.</returns>
        public bool IsValid()
        {
            return System.DateTime.Now < expirationDate;
        }

        /// <summary>
        /// Checks if the credit card has expired.
        /// </summary>
        /// <returns>True if the card has expired, false otherwise.</returns>
        public bool IsExpired()
        {
            if (ExpirationYear > 0 && ExpirationMonth > 0)
            {
                System.DateTime expiry = new System.DateTime(ExpirationYear, ExpirationMonth, 1).AddMonths(1).AddDays(-1);
                return System.DateTime.Now > expiry;
            }
            return System.DateTime.Now >= expirationDate;
        }

        /// <summary>
        /// Validates the card number using the Luhn algorithm (Mod 10 check).
        /// </summary>
        /// <returns>True if the card number passes the Luhn check, false otherwise.</returns>
        public bool ValidateLuhn()
        {
            return ValidateLuhn(CardNumber);
        }

        /// <summary>
        /// Validates a card number using the Luhn algorithm (Mod 10 check).
        /// </summary>
        /// <param name="cardNumber">The card number to validate.</param>
        /// <returns>True if the card number passes the Luhn check, false otherwise.</returns>
        public static bool ValidateLuhn(string cardNumber)
        {
            if (string.IsNullOrWhiteSpace(cardNumber))
            {
                return false;
            }

            string digits = GetDigitsOnly(cardNumber);
            if (digits.Length < 13 || digits.Length > 19)
            {
                return false;
            }

            int sum = 0;
            bool alternate = false;

            for (int i = digits.Length - 1; i >= 0; i--)
            {
                int digit = digits[i] - '0';

                if (alternate)
                {
                    digit *= 2;
                    if (digit > 9)
                    {
                        digit -= 9;
                    }
                }

                sum += digit;
                alternate = !alternate;
            }

            return sum % 10 == 0;
        }

        /// <summary>
        /// Detects the card type based on the card number prefix.
        /// </summary>
        /// <param name="cardNumber">The card number to check.</param>
        /// <returns>The detected card type.</returns>
        public static CardType DetectCardType(string cardNumber)
        {
            if (string.IsNullOrWhiteSpace(cardNumber))
            {
                return CardType.Unknown;
            }

            string digits = GetDigitsOnly(cardNumber);
            if (digits.Length < 4)
            {
                return CardType.Unknown;
            }

            // Visa: Starts with 4
            if (digits.StartsWith("4"))
            {
                return CardType.Visa;
            }

            // MasterCard: Starts with 51-55 or 2221-2720
            int prefix2 = int.Parse(digits.Substring(0, 2));
            if (prefix2 >= 51 && prefix2 <= 55)
            {
                return CardType.MasterCard;
            }
            if (digits.Length >= 4)
            {
                int prefix4 = int.Parse(digits.Substring(0, 4));
                if (prefix4 >= 2221 && prefix4 <= 2720)
                {
                    return CardType.MasterCard;
                }
            }

            // American Express: Starts with 34 or 37
            if (digits.StartsWith("34") || digits.StartsWith("37"))
            {
                return CardType.AmericanExpress;
            }

            // Discover: Starts with 6011, 644-649, or 65
            if (digits.StartsWith("6011") || digits.StartsWith("65"))
            {
                return CardType.Discover;
            }
            if (digits.Length >= 3)
            {
                int prefix3 = int.Parse(digits.Substring(0, 3));
                if (prefix3 >= 644 && prefix3 <= 649)
                {
                    return CardType.Discover;
                }
            }

            // Diners Club: Starts with 300-305, 36, or 38
            if (digits.StartsWith("36") || digits.StartsWith("38"))
            {
                return CardType.DinersClub;
            }
            if (digits.Length >= 3)
            {
                int prefix3 = int.Parse(digits.Substring(0, 3));
                if (prefix3 >= 300 && prefix3 <= 305)
                {
                    return CardType.DinersClub;
                }
            }

            // JCB: Starts with 3528-3589
            if (digits.Length >= 4)
            {
                int prefix4 = int.Parse(digits.Substring(0, 4));
                if (prefix4 >= 3528 && prefix4 <= 3589)
                {
                    return CardType.JCB;
                }
            }

            // UnionPay: Starts with 62
            if (digits.StartsWith("62"))
            {
                return CardType.UnionPay;
            }

            // Maestro: Starts with 5018, 5020, 5038, 6304, 6759, 6761, 6763
            string[] maestroPrefixes = { "5018", "5020", "5038", "6304", "6759", "6761", "6763" };
            foreach (string prefix in maestroPrefixes)
            {
                if (digits.StartsWith(prefix))
                {
                    return CardType.Maestro;
                }
            }

            return CardType.Unknown;
        }

        /// <summary>
        /// Masks the card number, showing only the last 4 digits.
        /// </summary>
        /// <param name="cardNumber">The card number to mask.</param>
        /// <returns>The masked card number (e.g., "****-****-****-1234").</returns>
        public static string MaskCardNumber(string cardNumber)
        {
            if (string.IsNullOrWhiteSpace(cardNumber))
            {
                return string.Empty;
            }

            string digits = GetDigitsOnly(cardNumber);
            if (digits.Length < 4)
            {
                return new string('*', digits.Length);
            }

            string lastFour = digits.Substring(digits.Length - 4);
            int maskedLength = digits.Length - 4;

            // Format as ****-****-****-1234
            if (digits.Length == 16)
            {
                return "****-****-****-" + lastFour;
            }
            else if (digits.Length == 15) // Amex
            {
                return "****-******-*" + lastFour;
            }
            else
            {
                return new string('*', maskedLength) + lastFour;
            }
        }

        /// <summary>
        /// Gets the formatted expiration date as MM/YY.
        /// </summary>
        /// <returns>The formatted expiration date.</returns>
        public string GetFormattedExpiration()
        {
            if (ExpirationMonth > 0 && ExpirationYear > 0)
            {
                return string.Format("{0:D2}/{1}", ExpirationMonth, ExpirationYear % 100);
            }
            return expirationDate.ToString("MM/yy");
        }

        /// <summary>
        /// Gets a display-friendly string for the card.
        /// </summary>
        /// <returns>A string like "Visa ending in 1234".</returns>
        public string GetDisplayString()
        {
            string cardTypeName = CardType.ToString();
            if (CardType == CardType.AmericanExpress)
            {
                cardTypeName = "American Express";
            }
            return string.Format("{0} ending in {1}", cardTypeName, LastFourDigits);
        }

        /// <summary>
        /// Returns a secure object representation without sensitive data.
        /// </summary>
        /// <returns>A new CreditCard object with masked data.</returns>
        public CreditCard ToSecureObject()
        {
            return new CreditCard
            {
                CardHolderName = this.CardHolderName,
                LastFourDigits = this.LastFourDigits,
                MaskedNumber = this.MaskedNumber,
                CardType = this.CardType,
                ExpirationMonth = this.ExpirationMonth,
                ExpirationYear = this.ExpirationYear,
                expirationDate = this.expirationDate,
                Funding = this.Funding,
                IssuingBank = this.IssuingBank,
                Country = this.Country
                // CardNumber and cvv are intentionally excluded
            };
        }

        /// <summary>
        /// Detects and sets the card type based on the current card number.
        /// </summary>
        private void DetectAndSetCardType()
        {
            CardType = DetectCardType(CardNumber);
        }

        /// <summary>
        /// Extracts only digits from a string.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>A string containing only digits.</returns>
        private static string GetDigitsOnly(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach (char c in input)
            {
                if (char.IsDigit(c))
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }
    }
}
