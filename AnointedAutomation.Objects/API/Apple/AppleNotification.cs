// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me
//Stewarded by Alexander Fields

using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace AnointedAutomation.Objects.Apple
{
    /// <summary>
    /// The claim set of a Sign in with Apple server-to-server notification JWT (the <c>payload</c> Apple posts
    /// to the registered notification endpoint). Modeled on apple-signin-auth's
    /// <c>AppleNotificationPayload</c>. Apple sends <c>events</c> as a JSON STRING inside the token, not as an
    /// object, so read it through <see cref="ParseEvent"/>.
    /// </summary>
    public class AppleNotificationPayload
    {
        /// <summary>Issuer, always <c>https://appleid.apple.com</c>.</summary>
        [JsonProperty("iss")]
        public string Iss { get; set; }

        /// <summary>Audience: our App ID or Services ID.</summary>
        [JsonProperty("aud")]
        public string Aud { get; set; }

        /// <summary>Issued-at, epoch seconds.</summary>
        [JsonProperty("iat")]
        public long? Iat { get; set; }

        /// <summary>Unique token id, the dedup key for redeliveries.</summary>
        [JsonProperty("jti")]
        public string Jti { get; set; }

        /// <summary>The event, as the raw JSON string Apple sends.</summary>
        [JsonProperty("events")]
        public string Events { get; set; }

        /// <summary>Parses <see cref="Events"/> into an <see cref="AppleNotificationEvent"/>; null when empty.</summary>
        public AppleNotificationEvent ParseEvent()
        {
            if (string.IsNullOrWhiteSpace(Events))
            {
                return null;
            }

            return JsonConvert.DeserializeObject<AppleNotificationEvent>(Events);
        }
    }

    /// <summary>One Sign in with Apple notification event (the parsed <c>events</c> member).</summary>
    public class AppleNotificationEvent
    {
        /// <summary>What happened.</summary>
        [JsonProperty("type")]
        [JsonConverter(typeof(AppleNotificationEventTypeConverter))]
        public AppleNotificationEventType Type { get; set; }

        /// <summary>Apple's team-scoped user id, the same <c>sub</c> as the identity token.</summary>
        [JsonProperty("sub")]
        public string Sub { get; set; }

        /// <summary>When the event happened, epoch MILLISECONDS as Apple sends it.</summary>
        [JsonProperty("event_time")]
        public long EventTime { get; set; }

        /// <summary>The relay email, present on the email events.</summary>
        [JsonProperty("email")]
        public string Email { get; set; }

        /// <summary>Whether the email is a private relay address, sent as a bool or a "true"/"false" string.</summary>
        [JsonProperty("is_private_email")]
        [JsonConverter(typeof(StringOrBoolConverter))]
        public bool? IsPrivateEmail { get; set; }
    }

    /// <summary>
    /// Sign in with Apple notification event types. 0 is reserved for a type this library does not know.
    /// </summary>
    public enum AppleNotificationEventType
    {
        /// <summary>An event type this library does not recognize.</summary>
        Unknown = 0,

        /// <summary>The user stopped forwarding mail from the private relay address.</summary>
        [EnumMember(Value = "email-disabled")]
        EmailDisabled = 1,

        /// <summary>The user resumed forwarding mail from the private relay address.</summary>
        [EnumMember(Value = "email-enabled")]
        EmailEnabled = 2,

        /// <summary>The user stopped using Sign in with Apple with this app.</summary>
        [EnumMember(Value = "consent-revoked")]
        ConsentRevoked = 3,

        /// <summary>The user deleted their Apple account.</summary>
        [EnumMember(Value = "account-delete")]
        AccountDelete = 4
    }

    /// <summary>
    /// Maps Apple's event type strings to <see cref="AppleNotificationEventType"/> and back. An unrecognized
    /// string reads as <see cref="AppleNotificationEventType.Unknown"/> instead of throwing, so a new Apple
    /// event type never breaks parsing.
    /// </summary>
    public class AppleNotificationEventTypeConverter : JsonConverter
    {
        /// <inheritdoc />
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(AppleNotificationEventType);
        }

        /// <inheritdoc />
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.String)
            {
                return AppleNotificationEventType.Unknown;
            }

            return FromWire((string)reader.Value);
        }

        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(ToWire((AppleNotificationEventType)value));
        }

        /// <summary>Apple's wire string to the enum; Unknown for anything unrecognized.</summary>
        public static AppleNotificationEventType FromWire(string value)
        {
            switch (value)
            {
                case "email-disabled":
                    return AppleNotificationEventType.EmailDisabled;
                case "email-enabled":
                    return AppleNotificationEventType.EmailEnabled;
                case "consent-revoked":
                    return AppleNotificationEventType.ConsentRevoked;
                case "account-delete":
                    return AppleNotificationEventType.AccountDelete;
                default:
                    return AppleNotificationEventType.Unknown;
            }
        }

        /// <summary>The enum to Apple's wire string; "unknown" for Unknown.</summary>
        public static string ToWire(AppleNotificationEventType value)
        {
            switch (value)
            {
                case AppleNotificationEventType.EmailDisabled:
                    return "email-disabled";
                case AppleNotificationEventType.EmailEnabled:
                    return "email-enabled";
                case AppleNotificationEventType.ConsentRevoked:
                    return "consent-revoked";
                case AppleNotificationEventType.AccountDelete:
                    return "account-delete";
                default:
                    return "unknown";
            }
        }
    }
}
