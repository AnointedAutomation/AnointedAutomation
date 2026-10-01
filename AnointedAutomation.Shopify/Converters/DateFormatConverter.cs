// Derived from ShopifySharp (https://github.com/nozzlegear/ShopifySharp), Copyright (c) 2015 Joshua Harms, MIT License.
// Adapted by Anointed Automation, LLC. See THIRD-PARTY-NOTICES.md.
using Newtonsoft.Json.Converters;

namespace AnointedAutomation.Shopify.Converters;

/// <summary>
/// Converts a Date to and from the provided date format string.
/// In Particular, GiftCard.ExpiresOn only accepts the format 'yyyy-MM-dd'. If the time is included the value is ignored
/// </summary>
public class DateFormatConverter : IsoDateTimeConverter
{
    public DateFormatConverter(string format)
    {
        base.DateTimeFormat = format;
    }
}