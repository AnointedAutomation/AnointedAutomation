// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//Stewarded by Alexander Fields

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AnointedAutomation.Objects.LinkedIn
{
    /// <summary>
    /// LinkedIn's OIDC userinfo response ("Response Body Schema",
    /// <c>https://learn.microsoft.com/en-us/linkedin/consumer/integrations/self-serve/sign-in-with-linkedin-v2</c>).
    /// Field names follow the snake_case wire JSON. <c>email</c> and <c>email_verified</c> are optional and null
    /// when LinkedIn omits them.
    /// </summary>
    public class LinkedInUserInfo
    {
        /// <summary>Subject identifier (the member's id for this app).</summary>
        [JsonProperty("sub")]
        public string Sub { get; set; }

        /// <summary>Full name.</summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>Member's first name.</summary>
        [JsonProperty("given_name")]
        public string GivenName { get; set; }

        /// <summary>Member's last name.</summary>
        [JsonProperty("family_name")]
        public string FamilyName { get; set; }

        /// <summary>Member's profile picture URL.</summary>
        [JsonProperty("picture")]
        public string Picture { get; set; }

        /// <summary>
        /// Member's locale. The docs' sample shows a string (<c>"en-US"</c>) while the live endpoint answers an
        /// object (<c>{"country":"US","language":"en"}</c>); both read into <see cref="LinkedInLocale"/>.
        /// </summary>
        [JsonProperty("locale")]
        [JsonConverter(typeof(LinkedInLocaleConverter))]
        public LinkedInLocale Locale { get; set; }

        /// <summary>Member's primary email address (optional).</summary>
        [JsonProperty("email")]
        public string Email { get; set; }

        /// <summary>Whether the member's primary email has been verified (optional).</summary>
        [JsonProperty("email_verified")]
        public bool? EmailVerified { get; set; }
    }

    /// <summary>A LinkedIn member locale.</summary>
    public class LinkedInLocale
    {
        /// <summary>The country code (e.g. <c>US</c>).</summary>
        [JsonProperty("country")]
        public string Country { get; set; }

        /// <summary>The language code (e.g. <c>en</c>).</summary>
        [JsonProperty("language")]
        public string Language { get; set; }
    }

    /// <summary>
    /// Reads a LinkedIn <c>locale</c> given either as an object (<c>{"country","language"}</c>) or as a string
    /// (<c>"en-US"</c> or <c>"en_US"</c>: language before the separator, country after); writes the object form.
    /// </summary>
    public class LinkedInLocaleConverter : JsonConverter
    {
        /// <inheritdoc />
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(LinkedInLocale);
        }

        /// <inheritdoc />
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            if (reader.TokenType == JsonToken.String)
            {
                return Parse((string)reader.Value);
            }

            JObject obj = JObject.Load(reader);
            return new LinkedInLocale
            {
                Country = (string)obj["country"],
                Language = (string)obj["language"]
            };
        }

        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            LinkedInLocale locale = (LinkedInLocale)value;
            if (locale == null)
            {
                writer.WriteNull();
                return;
            }

            writer.WriteStartObject();
            writer.WritePropertyName("country");
            writer.WriteValue(locale.Country);
            writer.WritePropertyName("language");
            writer.WriteValue(locale.Language);
            writer.WriteEndObject();
        }

        /// <summary>Splits <c>"en-US"</c> / <c>"en_US"</c> into language and country; empty gives null.</summary>
        public static LinkedInLocale Parse(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            string[] parts = value.Split('-', '_');
            return new LinkedInLocale
            {
                Language = parts[0],
                Country = parts.Length > 1 ? parts[1] : null
            };
        }
    }
}
