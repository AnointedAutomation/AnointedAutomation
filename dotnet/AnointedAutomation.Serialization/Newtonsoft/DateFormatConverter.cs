// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Derived from ShopifySharp (https://github.com/nozzlegear/ShopifySharp), Copyright (c) 2015 Joshua Harms, MIT License.
// Adapted by Anointed Automation, LLC. Moved from AnointedAutomation.Shopify.Converters (2026-09-30).

using global::Newtonsoft.Json.Converters;

namespace AnointedAutomation.Serialization.Newtonsoft
{
    /// <summary>
    /// Reads and writes a DateTime with a fixed format string, e.g.
    /// <c>[JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd")]</c> (Shopify GiftCard.ExpiresOn only accepts a date).
    /// </summary>
    public class DateFormatConverter : IsoDateTimeConverter
    {
        /// <summary>Creates the converter for <paramref name="format"/>.</summary>
        public DateFormatConverter(string format)
        {
            DateTimeFormat = format;
        }
    }
}
