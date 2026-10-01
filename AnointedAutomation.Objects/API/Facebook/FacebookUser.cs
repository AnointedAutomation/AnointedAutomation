// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//Stewarded by Alexander Fields

using Newtonsoft.Json;

namespace AnointedAutomation.Objects.Facebook
{
    /// <summary>
    /// A Graph API <c>User</c> node as <c>/me</c> returns it. Field names and types follow Meta's official
    /// Business SDK (<c>facebook_business/adobjects/user.py</c>: <c>Field</c> list and <c>_field_types</c>),
    /// limited to the fields a Facebook Login app can actually read, plus the requested <c>picture</c> edge.
    /// Every member is optional: Graph returns only the fields requested and granted.
    /// </summary>
    public class FacebookUser
    {
        /// <summary>App-scoped user id, a STRING in Graph JSON.</summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>Full display name.</summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>First name.</summary>
        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        /// <summary>Last name.</summary>
        [JsonProperty("last_name")]
        public string LastName { get; set; }

        /// <summary>Middle name.</summary>
        [JsonProperty("middle_name")]
        public string MiddleName { get; set; }

        /// <summary>Shortened, locale-aware name.</summary>
        [JsonProperty("short_name")]
        public string ShortName { get; set; }

        /// <summary>Locale-aware name format (e.g. <c>{first} {last}</c>).</summary>
        [JsonProperty("name_format")]
        public string NameFormat { get; set; }

        /// <summary>Primary email, absent when the user did not grant it.</summary>
        [JsonProperty("email")]
        public string Email { get; set; }

        /// <summary>Locale, e.g. <c>en_US</c>.</summary>
        [JsonProperty("locale")]
        public string Locale { get; set; }

        /// <summary>Profile link (app-scoped redirect).</summary>
        [JsonProperty("link")]
        public string Link { get; set; }

        /// <summary>Whether Facebook verified the account.</summary>
        [JsonProperty("verified")]
        public bool? Verified { get; set; }

        /// <summary>Last profile update, ISO-8601 as Graph sends it.</summary>
        [JsonProperty("updated_time")]
        public string UpdatedTime { get; set; }

        /// <summary>UTC offset in hours (fractional for some zones).</summary>
        [JsonProperty("timezone")]
        public float? Timezone { get; set; }

        /// <summary>Profile picture URL field (distinct from the <see cref="Picture"/> edge).</summary>
        [JsonProperty("profile_pic")]
        public string ProfilePic { get; set; }

        /// <summary>
        /// Business-scoped token: the SAME value for this person across every app owned by the same business,
        /// so it links identities across our apps where the app-scoped <see cref="Id"/> cannot.
        /// </summary>
        [JsonProperty("token_for_business")]
        public string TokenForBusiness { get; set; }

        /// <summary>Third-party id, stable for this app.</summary>
        [JsonProperty("third_party_id")]
        public string ThirdPartyId { get; set; }

        /// <summary>Age bracket (<c>{ min, max }</c>).</summary>
        [JsonProperty("age_range")]
        public FacebookAgeRange AgeRange { get; set; }

        /// <summary>The profile picture edge, nested as Graph returns it (<c>picture.data</c>).</summary>
        [JsonProperty("picture")]
        public FacebookPicture Picture { get; set; }
    }

    /// <summary>Graph <c>AgeRange</c>: either bound may be absent.</summary>
    public class FacebookAgeRange
    {
        /// <summary>Lower bound.</summary>
        [JsonProperty("min")]
        public int? Min { get; set; }

        /// <summary>Upper bound.</summary>
        [JsonProperty("max")]
        public int? Max { get; set; }
    }

    /// <summary>The <c>picture</c> edge wrapper: Graph nests the picture under <c>data</c>.</summary>
    public class FacebookPicture
    {
        /// <summary>The picture itself.</summary>
        [JsonProperty("data")]
        public FacebookPictureData Data { get; set; }
    }

    /// <summary>Graph ProfilePictureSource: <c>{ height, width, is_silhouette, url }</c>.</summary>
    public class FacebookPictureData
    {
        /// <summary>Height in pixels.</summary>
        [JsonProperty("height")]
        public int? Height { get; set; }

        /// <summary>Width in pixels.</summary>
        [JsonProperty("width")]
        public int? Width { get; set; }

        /// <summary>True when this is the default silhouette placeholder.</summary>
        [JsonProperty("is_silhouette")]
        public bool? IsSilhouette { get; set; }

        /// <summary>The picture URL.</summary>
        [JsonProperty("url")]
        public string Url { get; set; }
    }
}
