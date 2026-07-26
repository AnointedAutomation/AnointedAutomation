// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-07-26
//Stewarded by Alexander Fields

using AnointedAutomation.Objects.Account;
using AnointedAutomation.Objects.Google;
using Newtonsoft.Json;
using Xunit;

namespace AnointedAutomation.Objects.Tests
{
    public class GoogleObjectsTests
    {
        [Fact]
        public void UserProfile_SerializesWithGoogleJsonPropertyNames()
        {
            UserProfile profile = new UserProfile
            {
                id = "google-123",
                email = "user@example.com",
                verifiedEmail = true,
                givenName = "Jane",
                familyName = "Doe",
                picture = "https://pic.example/x.png"
            };

            string json = JsonConvert.SerializeObject(profile);

            Assert.Contains("\"verified_email\":true", json);
            Assert.Contains("\"given_name\":\"Jane\"", json);
            Assert.Contains("\"family_name\":\"Doe\"", json);
        }

        [Fact]
        public void GoogleTokenInfo_RoundTripsSubjectAndEmail()
        {
            GoogleTokenInfo token = new GoogleTokenInfo
            {
                sub = "google-123",
                email = "user@example.com",
                aud = "client-id"
            };

            string json = JsonConvert.SerializeObject(token);
            GoogleTokenInfo restored = JsonConvert.DeserializeObject<GoogleTokenInfo>(json);

            Assert.Equal("google-123", restored.sub);
            Assert.Equal("user@example.com", restored.email);
            Assert.Equal("client-id", restored.aud);
        }

        [Fact]
        public void User_RoundTripsGoogleProperty()
        {
            User user = new User
            {
                Email = "user@example.com",
                Google = new GoogleObjects(
                    new GoogleTokenInfo { sub = "google-123", email = "user@example.com" },
                    new UserProfile { id = "google-123", picture = "https://pic.example/x.png" })
            };

            string json = JsonConvert.SerializeObject(user);
            User restored = JsonConvert.DeserializeObject<User>(json);

            Assert.NotNull(restored.Google);
            Assert.Equal("google-123", restored.Google.GoogleTokenInfo.sub);
            Assert.Equal("https://pic.example/x.png", restored.Google.UserProfile.picture);
        }

        [Fact]
        public void User_WithoutGoogle_HasNullGoogle()
        {
            User user = new User { Email = "plain@example.com" };

            Assert.Null(user.Google);
        }
    }
}
