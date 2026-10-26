// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//Stewarded by Alexander Fields

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AnointedAutomation.Objects.Account;
using AnointedAutomation.Objects.Amazon;
using AnointedAutomation.Objects.Apple;
using AnointedAutomation.Objects.Discord;
using AnointedAutomation.Objects.Facebook;
using AnointedAutomation.Objects.GitHub;
using AnointedAutomation.Objects.Google;
using AnointedAutomation.Objects.LinkedIn;
using AnointedAutomation.Objects.Microsoft;
using AnointedAutomation.Objects.Reddit;
using AnointedAutomation.Objects.ShopifyCustomerAccounts;
using AnointedAutomation.Objects.Twitch;
using AnointedAutomation.Objects.Yahoo;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace AnointedAutomation.Objects.Tests
{
    public class SSOTests
    {
        private static AnointedAutomation.Shopify.Customer LoadShopifyFixture()
        {
            string json = File.ReadAllText(Path.Combine("Fixtures", "shopify_customer_rest.json"));
                        return JObject.Parse(json)["customers"][0].ToObject<AnointedAutomation.Shopify.Customer>();
        }

        [Fact]
        public void SSO_ProviderSlotsDefaultToNull()
        {
            SSO sso = new SSO();

            Assert.Null(sso.Google);
            Assert.Null(sso.Microsoft);
            Assert.Null(sso.Apple);
            Assert.Null(sso.Facebook);
            Assert.Null(sso.Discord);
            Assert.Null(sso.GitHub);
            Assert.Null(sso.LinkedIn);
            Assert.Null(sso.Twitch);
            Assert.Null(sso.Yahoo);
            Assert.Null(sso.Amazon);
            Assert.Null(sso.Reddit);
            Assert.Null(sso.ShopifyCustomerAccount);
            Assert.Null(sso.Shopify);
        }

        // GitHub: https://docs.github.com/en/rest/users/users#get-the-authenticated-user (OpenAPI example "private-user")
        private const string GitHubPrivateUserSample = "{\"login\":\"octocat\",\"id\":1,\"node_id\":\"MDQ6VXNlcjE=\",\"avatar_url\":\"https://github.com/images/error/octocat_happy.gif\",\"gravatar_id\":\"\",\"url\":\"https://api.github.com/users/octocat\",\"html_url\":\"https://github.com/octocat\",\"followers_url\":\"https://api.github.com/users/octocat/followers\",\"following_url\":\"https://api.github.com/users/octocat/following{/other_user}\",\"gists_url\":\"https://api.github.com/users/octocat/gists{/gist_id}\",\"starred_url\":\"https://api.github.com/users/octocat/starred{/owner}{/repo}\",\"subscriptions_url\":\"https://api.github.com/users/octocat/subscriptions\",\"organizations_url\":\"https://api.github.com/users/octocat/orgs\",\"repos_url\":\"https://api.github.com/users/octocat/repos\",\"events_url\":\"https://api.github.com/users/octocat/events{/privacy}\",\"received_events_url\":\"https://api.github.com/users/octocat/received_events\",\"type\":\"User\",\"site_admin\":false,\"name\":\"monalisa octocat\",\"company\":\"GitHub\",\"blog\":\"https://github.com/blog\",\"location\":\"San Francisco\",\"email\":\"octocat@github.com\",\"hireable\":false,\"bio\":\"There once was...\",\"twitter_username\":\"monatheoctocat\",\"public_repos\":2,\"public_gists\":1,\"followers\":20,\"following\":0,\"created_at\":\"2008-01-14T04:33:35Z\",\"updated_at\":\"2008-01-14T04:33:35Z\",\"private_gists\":81,\"total_private_repos\":100,\"owned_private_repos\":100,\"disk_usage\":10000,\"collaborators\":8,\"two_factor_authentication\":true,\"plan\":{\"name\":\"Medium\",\"space\":400,\"private_repos\":20,\"collaborators\":0}}";

        // GitHub: https://docs.github.com/en/rest/users/emails#list-email-addresses-for-the-authenticated-user (OpenAPI example "email-items-3")
        private const string GitHubEmailsSample = "[{\"email\":\"octocat@github.com\",\"primary\":true,\"verified\":true,\"visibility\":\"private\"}]";

        [Fact]
        public void GitHubUser_DeserializesDocumentedPrivateUser()
        {
            GitHubUser user = JsonConvert.DeserializeObject<GitHubUser>(GitHubPrivateUserSample);

            Assert.Equal("octocat", user.Login);
            Assert.Equal(1L, user.Id);
            Assert.Equal("MDQ6VXNlcjE=", user.NodeId);
            Assert.Equal("", user.GravatarId);
            Assert.Equal("User", user.Type);
            Assert.False(user.SiteAdmin);
            Assert.Equal("monalisa octocat", user.Name);
            Assert.Equal("octocat@github.com", user.Email);
            Assert.False(user.Hireable);
            Assert.Equal("monatheoctocat", user.TwitterUsername);
            Assert.Equal(20, user.Followers);
            Assert.Equal(new DateTime(2008, 1, 14, 4, 33, 35, DateTimeKind.Utc), user.CreatedAt.Value.ToUniversalTime());
            Assert.Equal(81, user.PrivateGists);
            Assert.Equal(10000, user.DiskUsage);
            Assert.True(user.TwoFactorAuthentication);
            Assert.Equal("Medium", user.Plan.Name);
            Assert.Equal(400L, user.Plan.Space);
            Assert.Equal(20, user.Plan.PrivateRepos);
            Assert.Null(user.NotificationEmail);
            Assert.Null(user.BusinessPlus);
        }

        [Fact]
        public void GitHubEmail_DeserializesDocumentedList_NullVisibility()
        {
            List<GitHubEmail> emails = JsonConvert.DeserializeObject<List<GitHubEmail>>(GitHubEmailsSample);
            List<GitHubEmail> nullVisibility = JsonConvert.DeserializeObject<List<GitHubEmail>>("[{\"email\":\"octocat@github.com\",\"primary\":false,\"verified\":false,\"visibility\":null}]");

            Assert.Single(emails);
            Assert.Equal("octocat@github.com", emails[0].Email);
            Assert.True(emails[0].Primary);
            Assert.True(emails[0].Verified);
            Assert.Equal("private", emails[0].Visibility);
            Assert.Null(nullVisibility[0].Visibility);
            Assert.False(nullVisibility[0].Primary);
        }

        [Fact]
        public void SSO_GitHubSlot_BsonRoundTrips()
        {
            SSO sso = new SSO
            {
                GitHub = new GitHubLogin
                {
                    User = JsonConvert.DeserializeObject<GitHubUser>(GitHubPrivateUserSample),
                    VerifiedPrimaryEmail = "octocat@github.com",
                    Emails = JsonConvert.DeserializeObject<List<GitHubEmail>>(GitHubEmailsSample)
                }
            };

            BsonDocument doc = sso.ToBsonDocument();
            SSO back = BsonSerializer.Deserialize<SSO>(doc);

            Assert.True(doc.Contains("GitHub"));
            Assert.Equal(1L, back.GitHub.User.Id);
            Assert.Equal("octocat", back.GitHub.User.Login);
            Assert.Equal(sso.GitHub.User.CreatedAt.Value.ToUniversalTime(), back.GitHub.User.CreatedAt.Value.ToUniversalTime());
            Assert.Equal("Medium", back.GitHub.User.Plan.Name);
            Assert.Equal("octocat@github.com", back.GitHub.VerifiedPrimaryEmail);
            Assert.True(back.GitHub.Emails.Single().Verified);
            Assert.Null(back.Discord);
        }

        // LinkedIn: https://learn.microsoft.com/en-us/linkedin/consumer/integrations/self-serve/sign-in-with-linkedin-v2 ("Sample API Response")
        private const string LinkedInUserInfoSample = "{\"sub\":\"782bbtaQ\",\"name\":\"John Doe\",\"given_name\":\"John\",\"family_name\":\"Doe\",\"picture\":\"https://media.licdn-ei.com/dms/image/C5F03AQHqK8v7tB1HCQ/profile-displayphoto-shrink_100_100/0/\",\"locale\":\"en-US\",\"email\":\"doe@email.com\",\"email_verified\":true}";

        [Fact]
        public void LinkedInUserInfo_DeserializesDocumentedSample()
        {
            LinkedInUserInfo info = JsonConvert.DeserializeObject<LinkedInUserInfo>(LinkedInUserInfoSample);

            Assert.Equal("782bbtaQ", info.Sub);
            Assert.Equal("John Doe", info.Name);
            Assert.Equal("John", info.GivenName);
            Assert.Equal("Doe", info.FamilyName);
            Assert.StartsWith("https://media.licdn-ei.com/", info.Picture);
            Assert.Equal("en", info.Locale.Language);
            Assert.Equal("US", info.Locale.Country);
            Assert.Equal("doe@email.com", info.Email);
            Assert.True(info.EmailVerified);
        }

        [Fact]
        public void LinkedInUserInfo_ReadsObjectLocale_AndMissingEmail()
        {
            LinkedInUserInfo info = JsonConvert.DeserializeObject<LinkedInUserInfo>("{\"sub\":\"s\",\"locale\":{\"country\":\"US\",\"language\":\"en\"}}");

            Assert.Equal("US", info.Locale.Country);
            Assert.Equal("en", info.Locale.Language);
            Assert.Null(info.Email);
            Assert.Null(info.EmailVerified);
            Assert.Contains("\"locale\":{\"country\":\"US\",\"language\":\"en\"}", JsonConvert.SerializeObject(info));
        }

        [Fact]
        public void SSO_LinkedInSlot_BsonRoundTrips()
        {
            SSO sso = new SSO { LinkedIn = new LinkedInLogin { UserInfo = JsonConvert.DeserializeObject<LinkedInUserInfo>(LinkedInUserInfoSample) } };

            BsonDocument doc = sso.ToBsonDocument();
            SSO back = BsonSerializer.Deserialize<SSO>(doc);

            Assert.True(doc.Contains("LinkedIn"));
            Assert.Equal("782bbtaQ", back.LinkedIn.UserInfo.Sub);
            Assert.Equal("US", back.LinkedIn.UserInfo.Locale.Country);
            Assert.True(back.LinkedIn.UserInfo.EmailVerified);
        }

        // Twitch: https://dev.twitch.tv/docs/api/reference/#get-users ("Example Response")
        private const string TwitchGetUsersSample = "{\"data\":[{\"id\":\"141981764\",\"login\":\"twitchdev\",\"display_name\":\"TwitchDev\",\"type\":\"\",\"broadcaster_type\":\"partner\",\"description\":\"Supporting third-party developers building Twitch integrations from chatbots to game integrations.\",\"profile_image_url\":\"https://static-cdn.jtvnw.net/jtv_user_pictures/8a6381c7-d0c0-4576-b179-38bd5ce1d6af-profile_image-300x300.png\",\"offline_image_url\":\"https://static-cdn.jtvnw.net/jtv_user_pictures/3f13ab61-ec78-4fe6-8481-8682cb3b0ac2-channel_offline_image-1920x1080.png\",\"view_count\":0,\"email\":\"not-real@email.com\",\"created_at\":\"2016-12-14T20:32:28Z\"}]}";

        // Twitch: https://dev.twitch.tv/docs/authentication/getting-tokens-oidc/ (userinfo response sample)
        private const string TwitchClaimsSample = "{\"aud\":\"hof5gwx0su6owfnys0nyan9c87zr6t\",\"exp\":1644342250,\"iat\":1644341350,\"iss\":\"https://id.twitch.tv/oauth2\",\"sub\":\"12345678\",\"email\":\"foo@justin.tv\",\"email_verified\":true,\"picture\":\"https://static-cdn.jtvnw.net/user-default-pictures-uv/998f01ae-def8-11e9-b95c-784f43822e80-profile_image-150x150.png\",\"updated_at\":\"2022-02-03T16:16:16.968509Z\"}";

        [Fact]
        public void TwitchUser_DeserializesDocumentedGetUsers()
        {
            TwitchUser user = JObject.Parse(TwitchGetUsersSample)["data"][0].ToObject<TwitchUser>();

            Assert.Equal("141981764", user.Id);
            Assert.Equal("twitchdev", user.Login);
            Assert.Equal("TwitchDev", user.DisplayName);
            Assert.Equal("", user.Type);
            Assert.Equal("partner", user.BroadcasterType);
            Assert.StartsWith("Supporting third-party", user.Description);
            Assert.EndsWith("profile_image-300x300.png", user.ProfileImageUrl);
            Assert.EndsWith("channel_offline_image-1920x1080.png", user.OfflineImageUrl);
            Assert.Equal(0L, user.ViewCount);
            Assert.Equal("not-real@email.com", user.Email);
            Assert.Equal(new DateTime(2016, 12, 14, 20, 32, 28, DateTimeKind.Utc), user.CreatedAt.Value.ToUniversalTime());
        }

        [Fact]
        public void TwitchIdTokenClaims_DeserializesDocumentedSample()
        {
            TwitchIdTokenClaims claims = JsonConvert.DeserializeObject<TwitchIdTokenClaims>(TwitchClaimsSample);

            Assert.Equal("hof5gwx0su6owfnys0nyan9c87zr6t", claims.Aud);
            Assert.Equal(1644342250L, claims.Exp);
            Assert.Equal(1644341350L, claims.Iat);
            Assert.Equal("https://id.twitch.tv/oauth2", claims.Iss);
            Assert.Equal("12345678", claims.Sub);
            Assert.Equal("foo@justin.tv", claims.Email);
            Assert.True(claims.EmailVerified);
            Assert.EndsWith("profile_image-150x150.png", claims.Picture);
            Assert.Equal(new DateTime(2022, 2, 3, 16, 16, 16, 968, DateTimeKind.Utc).AddTicks(5090), claims.UpdatedAt.Value.ToUniversalTime());
            Assert.Null(claims.PreferredUsername);
            Assert.Null(claims.Azp);
        }

        [Fact]
        public void SSO_TwitchSlot_BsonRoundTrips()
        {
            SSO sso = new SSO
            {
                Twitch = new TwitchLogin
                {
                    User = JObject.Parse(TwitchGetUsersSample)["data"][0].ToObject<TwitchUser>(),
                    IdToken = JsonConvert.DeserializeObject<TwitchIdTokenClaims>(TwitchClaimsSample)
                }
            };

            BsonDocument doc = sso.ToBsonDocument();
            SSO back = BsonSerializer.Deserialize<SSO>(doc);

            Assert.True(doc.Contains("Twitch"));
            Assert.Equal("141981764", back.Twitch.User.Id);
            Assert.Equal(sso.Twitch.User.CreatedAt.Value.ToUniversalTime(), back.Twitch.User.CreatedAt.Value.ToUniversalTime());
            Assert.Equal("12345678", back.Twitch.IdToken.Sub);
            Assert.True(back.Twitch.IdToken.EmailVerified);
        }

        // Yahoo: https://developer.yahoo.com/oauth2/guide/openid_connect/getting_started.html (decoded id_token of the
        // "Example Response") plus the profile/email claims of https://api.login.yahoo.com/.well-known/openid-configuration
        private const string YahooIdTokenSample = "{\"at_hash\":\"ac9bDwz5LZ9yPEiukDpgsw==\",\"sub\":\"UQIDWJNWVNQD4GXZ5NGMZUSTQ4\",\"aud\":\"dj0yJmk9WGx0QlE0UWdCa0hKJmQ9WVdrOWNrNUhXVnBhTkhFbWNHbzlNQS0tJnM9Y29uc3VtZXJzZWNyZXQmeD01OA--\",\"iss\":\"https://login.yahoo.com\",\"exp\":1443827130,\"nonce\":\"12345\",\"iat\":1443823530}";

        [Fact]
        public void YahooUserInfo_DeserializesDocumentedIdToken_AndProfileClaims()
        {
            YahooUserInfo token = JsonConvert.DeserializeObject<YahooUserInfo>(YahooIdTokenSample);
            YahooUserInfo profile = JsonConvert.DeserializeObject<YahooUserInfo>("{\"sub\":\"UQIDWJNWVNQD4GXZ5NGMZUSTQ4\",\"name\":\"Jane Doe\",\"given_name\":\"Jane\",\"family_name\":\"Doe\",\"email\":\"jane@yahoo.com\",\"email_verified\":true,\"birthdate\":\"1990\",\"locale\":\"en-US\",\"picture\":\"https://s.yimg.com/x.png\",\"auth_time\":1443823500}");

            Assert.Equal("ac9bDwz5LZ9yPEiukDpgsw==", token.AtHash);
            Assert.Equal("UQIDWJNWVNQD4GXZ5NGMZUSTQ4", token.Sub);
            Assert.Equal("https://login.yahoo.com", token.Iss);
            Assert.Equal(1443827130L, token.Exp);
            Assert.Equal("12345", token.Nonce);
            Assert.Equal(1443823530L, token.Iat);
            Assert.Null(token.Email);
            Assert.Null(token.EmailVerified);
            Assert.Equal("Jane", profile.GivenName);
            Assert.Equal("Doe", profile.FamilyName);
            Assert.Equal("jane@yahoo.com", profile.Email);
            Assert.True(profile.EmailVerified);
            Assert.Equal("1990", profile.Birthdate);
            Assert.Equal("en-US", profile.Locale);
            Assert.Equal(1443823500L, profile.AuthTime);
        }

        [Fact]
        public void SSO_YahooSlot_BsonRoundTrips()
        {
            SSO sso = new SSO { Yahoo = new YahooLogin { UserInfo = JsonConvert.DeserializeObject<YahooUserInfo>(YahooIdTokenSample) } };
            sso.Yahoo.UserInfo.Email = "jane@yahoo.com";
            sso.Yahoo.UserInfo.EmailVerified = true;

            BsonDocument doc = sso.ToBsonDocument();
            SSO back = BsonSerializer.Deserialize<SSO>(doc);

            Assert.True(doc.Contains("Yahoo"));
            Assert.Equal("UQIDWJNWVNQD4GXZ5NGMZUSTQ4", back.Yahoo.UserInfo.Sub);
            Assert.Equal(1443827130L, back.Yahoo.UserInfo.Exp);
            Assert.Equal("jane@yahoo.com", back.Yahoo.UserInfo.Email);
            Assert.True(back.Yahoo.UserInfo.EmailVerified);
        }

        // Amazon: https://developer.amazon.com/docs/login-with-amazon/obtain-customer-profile.html (sample response)
        private const string AmazonProfileSample = "{\"user_id\":\"amznl.account.K2LI23KL2LK2\",\"email\":\"mhashimoto-04@plaxo.com\",\"name\":\"Mork Hashimoto\",\"postal_code\":\"98052\"}";

        [Fact]
        public void AmazonProfile_DeserializesDocumentedSample()
        {
            AmazonProfile profile = JsonConvert.DeserializeObject<AmazonProfile>(AmazonProfileSample);
            AmazonProfile idOnly = JsonConvert.DeserializeObject<AmazonProfile>("{\"user_id\":\"amznl.account.X\"}");

            Assert.Equal("amznl.account.K2LI23KL2LK2", profile.UserId);
            Assert.Equal("mhashimoto-04@plaxo.com", profile.Email);
            Assert.Equal("Mork Hashimoto", profile.Name);
            Assert.Equal("98052", profile.PostalCode);
            Assert.Null(idOnly.Email);
            Assert.Null(idOnly.PostalCode);
        }

        [Fact]
        public void SSO_AmazonSlot_BsonRoundTrips()
        {
            SSO sso = new SSO { Amazon = new AmazonLogin { Profile = JsonConvert.DeserializeObject<AmazonProfile>(AmazonProfileSample) } };

            BsonDocument doc = sso.ToBsonDocument();
            SSO back = BsonSerializer.Deserialize<SSO>(doc);

            Assert.True(doc.Contains("Amazon"));
            Assert.Equal("amznl.account.K2LI23KL2LK2", back.Amazon.Profile.UserId);
            Assert.Equal("98052", back.Amazon.Profile.PostalCode);
        }

        // Reddit: https://github.com/reddit-archive/reddit/wiki/JSON ("Example of raw account data", the data object)
        private const string RedditAccountSample = "{\"has_mail\":false,\"name\":\"fooBar\",\"created\":123456789.0,\"modhash\":\"f0f0f0f0f0f0f0f0...\",\"created_utc\":1315269998.0,\"link_karma\":31,\"comment_karma\":557,\"is_gold\":false,\"is_mod\":false,\"has_verified_email\":false,\"id\":\"5sryd\",\"has_mod_mail\":false}";

        [Fact]
        public void RedditUser_DeserializesDocumentedAccount_WithoutModhash()
        {
            RedditUser user = JsonConvert.DeserializeObject<RedditUser>(RedditAccountSample);
            RedditUser me = JsonConvert.DeserializeObject<RedditUser>("{\"id\":\"5sryd\",\"name\":\"fooBar\",\"icon_img\":\"https://styles.redditmedia.com/x.png\",\"verified\":true,\"has_verified_email\":true,\"total_karma\":588,\"over_18\":false,\"is_employee\":false,\"inbox_count\":2}");

            Assert.Equal("5sryd", user.Id);
            Assert.Equal("fooBar", user.Name);
            Assert.Equal(123456789.0, user.Created);
            Assert.Equal(1315269998.0, user.CreatedUtc);
            Assert.Equal(31, user.LinkKarma);
            Assert.Equal(557, user.CommentKarma);
            Assert.False(user.IsGold);
            Assert.False(user.IsMod);
            Assert.False(user.HasVerifiedEmail);
            Assert.False(user.HasMail);
            Assert.False(user.HasModMail);
            Assert.DoesNotContain("modhash", JsonConvert.SerializeObject(user));
            Assert.Equal("https://styles.redditmedia.com/x.png", me.IconImg);
            Assert.True(me.Verified);
            Assert.True(me.HasVerifiedEmail);
            Assert.Equal(588, me.TotalKarma);
            Assert.Equal(2, me.InboxCount);
        }

        [Fact]
        public void SSO_RedditSlot_BsonRoundTrips()
        {
            SSO sso = new SSO { Reddit = new RedditLogin { User = JsonConvert.DeserializeObject<RedditUser>(RedditAccountSample) } };

            BsonDocument doc = sso.ToBsonDocument();
            SSO back = BsonSerializer.Deserialize<SSO>(doc);

            Assert.True(doc.Contains("Reddit"));
            Assert.Equal("5sryd", back.Reddit.User.Id);
            Assert.Equal(1315269998.0, back.Reddit.User.CreatedUtc);
            Assert.False(back.Reddit.User.HasVerifiedEmail);
        }

        // Shopify Customer Account API: claims_supported of https://shopify.com/authentication/{shopId}/.well-known/openid-configuration
        // (https://shopify.dev/docs/api/customer); values are illustrative, Shopify publishes no decoded id_token sample.
        private const string ShopifyCustomerAccountClaimsSample = "{\"iss\":\"https://shopify.com/authentication/68761190511\",\"sub\":\"gid://shopify/Customer/23780308549743\",\"aud\":\"shp_client\",\"exp\":1760000600,\"iat\":1760000000,\"nonce\":\"n-1\",\"sid\":\"sid-1\",\"email\":\"customer@example.com\",\"email_verified\":true}";

        [Fact]
        public void ShopifyCustomerAccountClaims_DeserializesSupportedClaims()
        {
            ShopifyCustomerAccountClaims claims = JsonConvert.DeserializeObject<ShopifyCustomerAccountClaims>(ShopifyCustomerAccountClaimsSample);
            ShopifyCustomerAccountClaims named = JsonConvert.DeserializeObject<ShopifyCustomerAccountClaims>("{\"sub\":\"s\",\"given_name\":\"Ada\",\"family_name\":\"Lovelace\"}");

            Assert.Equal("https://shopify.com/authentication/68761190511", claims.Iss);
            Assert.Equal("gid://shopify/Customer/23780308549743", claims.Sub);
            Assert.Equal(1760000600L, claims.Exp);
            Assert.Equal("sid-1", claims.Sid);
            Assert.Equal("customer@example.com", claims.Email);
            Assert.True(claims.EmailVerified);
            Assert.Null(claims.GivenName);
            Assert.Equal("Ada", named.GivenName);
            Assert.Equal("Lovelace", named.FamilyName);
        }

        [Fact]
        public void SSO_ShopifyCustomerAccountSlot_BsonRoundTrips_IndependentOfShopify()
        {
            SSO sso = new SSO
            {
                ShopifyCustomerAccount = new ShopifyCustomerAccountLogin { IdToken = JsonConvert.DeserializeObject<ShopifyCustomerAccountClaims>(ShopifyCustomerAccountClaimsSample) },
                Shopify = new AnointedAutomation.Shopify.Customer { Id = 23780308549743, Email = "customer@example.com" }
            };

            BsonDocument doc = sso.ToBsonDocument();
            SSO back = BsonSerializer.Deserialize<SSO>(doc);

            Assert.True(doc.Contains("ShopifyCustomerAccount"));
            Assert.True(doc.Contains("Shopify"));
            Assert.Equal("gid://shopify/Customer/23780308549743", back.ShopifyCustomerAccount.IdToken.Sub);
            Assert.True(back.ShopifyCustomerAccount.IdToken.EmailVerified);
            Assert.Equal(23780308549743L, back.Shopify.Id);
        }

        [Fact]
        public void SSO_NewSlots_SerializeWithoutAnyTokenFields()
        {
            SSO sso = new SSO
            {
                GitHub = new GitHubLogin { User = JsonConvert.DeserializeObject<GitHubUser>(GitHubPrivateUserSample) },
                LinkedIn = new LinkedInLogin { UserInfo = JsonConvert.DeserializeObject<LinkedInUserInfo>(LinkedInUserInfoSample) },
                Twitch = new TwitchLogin { IdToken = JsonConvert.DeserializeObject<TwitchIdTokenClaims>(TwitchClaimsSample) },
                Yahoo = new YahooLogin { UserInfo = JsonConvert.DeserializeObject<YahooUserInfo>(YahooIdTokenSample) },
                Amazon = new AmazonLogin { Profile = JsonConvert.DeserializeObject<AmazonProfile>(AmazonProfileSample) },
                Reddit = new RedditLogin { User = JsonConvert.DeserializeObject<RedditUser>(RedditAccountSample) },
                ShopifyCustomerAccount = new ShopifyCustomerAccountLogin { IdToken = JsonConvert.DeserializeObject<ShopifyCustomerAccountClaims>(ShopifyCustomerAccountClaimsSample) }
            };

            string json = JsonConvert.SerializeObject(sso);
            string bson = sso.ToBsonDocument().ToString();

            foreach (string forbidden in new[] { "access_token", "refresh_token", "AccessToken", "RefreshToken", "modhash" })
            {
                Assert.DoesNotContain(forbidden, json);
                Assert.DoesNotContain(forbidden, bson);
            }
        }

        [Fact]
        public void User_WithAllNewSlots_BsonRoundTrips()
        {
            User user = new User
            {
                Email = "octocat@github.com",
                SSO = new SSO
                {
                    GitHub = new GitHubLogin { VerifiedPrimaryEmail = "octocat@github.com" },
                    LinkedIn = new LinkedInLogin { UserInfo = new LinkedInUserInfo { Sub = "li" } },
                    Twitch = new TwitchLogin { User = new TwitchUser { Id = "tw" } },
                    Yahoo = new YahooLogin { UserInfo = new YahooUserInfo { Sub = "ya" } },
                    Amazon = new AmazonLogin { Profile = new AmazonProfile { UserId = "am" } },
                    Reddit = new RedditLogin { User = new RedditUser { Id = "rd" } },
                    ShopifyCustomerAccount = new ShopifyCustomerAccountLogin { IdToken = new ShopifyCustomerAccountClaims { Sub = "sca" } }
                }
            };

            User back = BsonSerializer.Deserialize<User>(user.ToBsonDocument());

            Assert.Equal("octocat@github.com", back.SSO.GitHub.VerifiedPrimaryEmail);
            Assert.Null(back.SSO.GitHub.User);
            Assert.Equal("li", back.SSO.LinkedIn.UserInfo.Sub);
            Assert.Equal("tw", back.SSO.Twitch.User.Id);
            Assert.Null(back.SSO.Twitch.IdToken);
            Assert.Equal("ya", back.SSO.Yahoo.UserInfo.Sub);
            Assert.Equal("am", back.SSO.Amazon.Profile.UserId);
            Assert.Equal("rd", back.SSO.Reddit.User.Id);
            Assert.Equal("sca", back.SSO.ShopifyCustomerAccount.IdToken.Sub);
            Assert.Null(back.SSO.Discord);
        }

        [Fact]
        public void DiscordUser_DeserializesUsersMeShape()
        {
            string me = "{\"id\":\"80351110224678912\",\"username\":\"nelly\",\"discriminator\":\"0\",\"global_name\":\"Nelly\",\"avatar\":\"8342729096ea3675442027381ff50dfe\",\"verified\":true,\"email\":\"nelly@discord.com\",\"flags\":64,\"locale\":\"en-US\"}";

            DiscordUser user = JsonConvert.DeserializeObject<DiscordUser>(me);

            Assert.Equal("80351110224678912", user.Id);
            Assert.Equal("nelly", user.Username);
            Assert.Equal("0", user.Discriminator);
            Assert.Equal("Nelly", user.GlobalName);
            Assert.Equal("8342729096ea3675442027381ff50dfe", user.Avatar);
            Assert.True(user.Verified);
            Assert.Equal("nelly@discord.com", user.Email);
        }

        [Fact]
        public void DiscordUser_DeserializesFullDocumentedUserObject()
        {
            string me = "{\"id\":\"1\",\"username\":\"u\",\"discriminator\":\"0\",\"global_name\":\"U\",\"avatar\":\"a\",\"bot\":false,\"system\":false,\"mfa_enabled\":true,\"banner\":\"b\",\"accent_color\":16711680,\"locale\":\"en-US\",\"verified\":true,\"email\":\"u@x.com\",\"flags\":4194304,\"premium_type\":2,\"public_flags\":64,"
                + "\"avatar_decoration_data\":{\"asset\":\"deco\",\"sku_id\":\"111\"},"
                + "\"collectibles\":{\"nameplate\":{\"sku_id\":\"222\",\"asset\":\"nameplates/x/\",\"label\":\"\",\"palette\":\"bubble_gum\"}},"
                + "\"primary_guild\":{\"identity_guild_id\":\"333\",\"identity_enabled\":true,\"tag\":\"ANNT\",\"badge\":\"bdg\"}}";

            DiscordUser user = JsonConvert.DeserializeObject<DiscordUser>(me);

            Assert.False(user.Bot);
            Assert.False(user.System);
            Assert.True(user.MfaEnabled);
            Assert.Equal("b", user.Banner);
            Assert.Equal(16711680, user.AccentColor);
            Assert.Equal("en-US", user.Locale);
            Assert.Equal(4194304L, user.Flags);
            Assert.Equal(2, user.PremiumType);
            Assert.Equal(64L, user.PublicFlags);
            Assert.Equal("deco", user.AvatarDecorationData.Asset);
            Assert.Equal("111", user.AvatarDecorationData.SkuId);
            Assert.Equal("222", user.Collectibles.Nameplate.SkuId);
            Assert.Equal("bubble_gum", user.Collectibles.Nameplate.Palette);
            Assert.Equal("333", user.PrimaryGuild.IdentityGuildId);
            Assert.True(user.PrimaryGuild.IdentityEnabled);
            Assert.Equal("ANNT", user.PrimaryGuild.Tag);
            Assert.Equal("bdg", user.PrimaryGuild.Badge);
        }

        [Fact]
        public void DiscordPrimaryGuild_NullIdentityEnabled_StaysNull()
        {
            DiscordUser user = JsonConvert.DeserializeObject<DiscordUser>("{\"id\":\"1\",\"primary_guild\":{\"identity_guild_id\":null,\"identity_enabled\":null,\"tag\":null,\"badge\":null},\"collectibles\":null,\"avatar_decoration_data\":null}");

            Assert.Null(user.PrimaryGuild.IdentityEnabled);
            Assert.Null(user.PrimaryGuild.IdentityGuildId);
            Assert.Null(user.Collectibles);
            Assert.Null(user.AvatarDecorationData);
        }

        [Fact]
        public void DiscordUser_WithoutEmailScope_LeavesEmailAndVerifiedNull()
        {
            DiscordUser user = JsonConvert.DeserializeObject<DiscordUser>("{\"id\":\"1\",\"username\":\"u\",\"global_name\":null,\"avatar\":null}");

            Assert.Equal("1", user.Id);
            Assert.Null(user.Email);
            Assert.Null(user.Verified);
            Assert.Null(user.GlobalName);
            Assert.Null(user.Avatar);
        }

        [Fact]
        public void SSO_DiscordSlot_BsonRoundTrips()
        {
            SSO sso = new SSO { Discord = new DiscordLogin { User = new DiscordUser { Id = "d-1", Username = "u", Email = "u@x.com", Verified = false } } };

            BsonDocument doc = sso.ToBsonDocument();
            SSO back = BsonSerializer.Deserialize<SSO>(doc);

            Assert.True(doc.Contains("Discord"));
            Assert.Equal("d-1", back.Discord.User.Id);
            Assert.Equal("u@x.com", back.Discord.User.Email);
            Assert.False(back.Discord.User.Verified);
            Assert.Null(back.Facebook);
        }

        [Fact]
        public void UserEmailSource_Discord_IsAppendedAfterAdmin()
        {
            Assert.Equal(9, (int)UserEmailSource.Admin);
            Assert.Equal(10, (int)UserEmailSource.Discord);
        }

        [Fact]
        public void SSO_HoldsEachProviderRealObjectIndependently()
        {
            SSO sso = new SSO
            {
                Google = new GoogleObjects(new GoogleTokenInfo { sub = "g-tok" }, new UserProfile { id = "g-1", name = "G" }),
                Microsoft = new MicrosoftUser { Id = "ms-oid", DisplayName = "M", TenantId = "tid-1", IdentityProvider = "live.com" },
                Apple = new AppleSignIn { IdToken = new AppleIdTokenClaims { Sub = "apple-sub" } },
                Facebook = new FacebookLogin { User = new FacebookUser { Id = "fb-id", Name = "F" } },
                Shopify = new AnointedAutomation.Shopify.Customer { Id = 123456, Email = "s@x.com" }
            };

            Assert.Equal("g-1", sso.Google.UserProfile.id);
            Assert.Equal("ms-oid", sso.Microsoft.Id);
            Assert.Equal("live.com", sso.Microsoft.IdentityProvider);
            Assert.Equal("apple-sub", sso.Apple.IdToken.Sub);
            Assert.Equal("fb-id", sso.Facebook.User.Id);
            Assert.Equal(123456, sso.Shopify.Id);
        }

        [Fact]
        public void User_SSOIsNullByDefault_AndMetaIsIndependent()
        {
            User user = new User();

            Assert.Null(user.SSO);

            user.SSO = new SSO { Google = new GoogleObjects(new GoogleTokenInfo(), new UserProfile { id = "g-9" }) };
            user.Meta = "{\"ShopifyCustomerId\":\"555\"}";

            Assert.Equal("g-9", user.SSO.Google.UserProfile.id);
            Assert.Equal("{\"ShopifyCustomerId\":\"555\"}", user.Meta);
        }

        [Fact]
        public void ShopifyCustomer_DeserializesRealRestPayload()
        {
            AnointedAutomation.Shopify.Customer customer = LoadShopifyFixture();

            Assert.Equal(23780308549743L, customer.Id);
            Assert.Equal("customer@example.com", customer.Email);
            Assert.Equal(3, customer.OrdersCount);
            Assert.Equal(346.27m, customer.TotalSpent);
            Assert.Equal("gid://shopify/Customer/23780308549743", customer.AdminGraphQLAPIId);
            Assert.Single(customer.Addresses);
            Assert.Equal("US", customer.DefaultAddress.CountryCode);
            Assert.Equal("subscribed", customer.EmailMarketingConsent.State);
        }

        [Fact]
        public void ShopifyCustomer_BsonRoundTrip_PreservesRealPayload()
        {
            AnointedAutomation.Shopify.Customer customer = LoadShopifyFixture();

            BsonDocument doc = customer.ToBsonDocument();
            AnointedAutomation.Shopify.Customer back = BsonSerializer.Deserialize<AnointedAutomation.Shopify.Customer>(doc);

            Assert.Equal(customer.Id, back.Id);
            Assert.Equal(customer.Email, back.Email);
            Assert.Equal(customer.CreatedAt, back.CreatedAt);
            Assert.Equal(customer.TotalSpent, back.TotalSpent);
            Assert.Equal(customer.Tags, back.Tags);
            Assert.Equal(customer.Addresses.Single().Id, back.Addresses.Single().Id);
            Assert.Equal(customer.DefaultAddress.City, back.DefaultAddress.City);
            Assert.Equal(customer.EmailMarketingConsent.OptInLevel, back.EmailMarketingConsent.OptInLevel);
        }

        [Fact]
        public void AppleIdTokenClaims_AcceptsBooleansAsStringsOrBools()
        {
            string asStrings = "{\"iss\":\"https://appleid.apple.com\",\"aud\":\"com.x\",\"exp\":1700000600,\"iat\":1700000000,\"sub\":\"001.abc\",\"email\":\"r@privaterelay.appleid.com\",\"email_verified\":\"true\",\"is_private_email\":\"true\",\"nonce_supported\":\"false\",\"real_user_status\":2,\"auth_time\":1700000000,\"c_hash\":\"ch\",\"at_hash\":\"ah\",\"nonce\":\"n\",\"transfer_sub\":\"t\"}";
            string asBools = "{\"sub\":\"001.abc\",\"email_verified\":true,\"is_private_email\":false,\"nonce_supported\":true,\"real_user_status\":1}";

            AppleIdTokenClaims a = JsonConvert.DeserializeObject<AppleIdTokenClaims>(asStrings);
            AppleIdTokenClaims b = JsonConvert.DeserializeObject<AppleIdTokenClaims>(asBools);

            Assert.True(a.EmailVerified);
            Assert.True(a.IsPrivateEmail);
            Assert.False(a.NonceSupported);
            Assert.Equal(AppleRealUserStatus.LikelyReal, a.RealUserStatus);
            Assert.Equal(1700000600L, a.Exp);
            Assert.Equal("ch", a.CHash);
            Assert.Equal("t", a.TransferSub);
            Assert.True(b.EmailVerified);
            Assert.False(b.IsPrivateEmail);
            Assert.True(b.NonceSupported);
            Assert.Equal(AppleRealUserStatus.Unknown, b.RealUserStatus);
        }

        [Fact]
        public void AppleUser_DeserializesFirstConsentShape()
        {
            AppleUser user = JsonConvert.DeserializeObject<AppleUser>("{\"name\":{\"firstName\":\"Ada\",\"lastName\":\"Lovelace\"},\"email\":\"ada@x.com\"}");

            Assert.Equal("Ada", user.Name.FirstName);
            Assert.Equal("Lovelace", user.Name.LastName);
            Assert.Equal("ada@x.com", user.Email);
        }

        [Fact]
        public void FacebookUser_DeserializesGraphMeShape_IgnoringUnrequestedFields()
        {
            string me = "{\"id\":\"10229876543210987\",\"name\":\"Ada Lovelace\",\"first_name\":\"Ada\",\"last_name\":\"Lovelace\",\"email\":\"ada@x.com\",\"locale\":\"en_US\",\"picture\":{\"data\":{\"height\":200,\"width\":200,\"is_silhouette\":false,\"url\":\"https://p.example/a.jpg\"}}}";

            FacebookUser user = JsonConvert.DeserializeObject<FacebookUser>(me);

            Assert.Equal("10229876543210987", user.Id);
            Assert.Equal("Ada", user.FirstName);
            Assert.Equal(200, user.Picture.Data.Height);
            Assert.False(user.Picture.Data.IsSilhouette);
            Assert.Equal("https://p.example/a.jpg", user.Picture.Data.Url);
        }

        [Fact]
        public void FacebookDebugTokenData_DeserializesGraphDataShape()
        {
            string json = "{\"data\":{\"app_id\":\"138483919580948\",\"type\":\"USER\",\"application\":\"Anointed\",\"data_access_expires_at\":1700600000,\"expires_at\":0,\"is_valid\":true,\"issued_at\":1700000000,\"scopes\":[\"email\",\"public_profile\"],\"user_id\":\"10229876543210987\"}}";

            FacebookDebugTokenData data = JObject.Parse(json)["data"].ToObject<FacebookDebugTokenData>();

            Assert.Equal("138483919580948", data.AppId);
            Assert.Equal("USER", data.Type);
            Assert.Equal(0L, data.ExpiresAt);
            Assert.True(data.IsValid);
            Assert.Equal(new[] { "email", "public_profile" }, data.Scopes);
            Assert.Equal("10229876543210987", data.UserId);
        }

        [Fact]
        public void FacebookUser_DeserializesSdkFieldSet()
        {
            string me = "{\"id\":\"1\",\"middle_name\":\"Q\",\"short_name\":\"Ada\",\"name_format\":\"{first} {last}\",\"locale\":\"en_US\",\"link\":\"https://fb/x\",\"verified\":true,\"updated_time\":\"2026-01-01T00:00:00+0000\",\"timezone\":-4.5,\"profile_pic\":\"https://p/x\",\"token_for_business\":\"AbTfB\",\"third_party_id\":\"tp\",\"age_range\":{\"min\":21}}";

            FacebookUser user = JsonConvert.DeserializeObject<FacebookUser>(me);

            Assert.Equal("Q", user.MiddleName);
            Assert.Equal("Ada", user.ShortName);
            Assert.Equal("{first} {last}", user.NameFormat);
            Assert.Equal("en_US", user.Locale);
            Assert.True(user.Verified);
            Assert.Equal(-4.5f, user.Timezone);
            Assert.Equal("AbTfB", user.TokenForBusiness);
            Assert.Equal("tp", user.ThirdPartyId);
            Assert.Equal(21, user.AgeRange.Min);
            Assert.Null(user.AgeRange.Max);
        }

        [Fact]
        public void MicrosoftUser_UsesGraphJsonNames()
        {
            MicrosoftUser user = JsonConvert.DeserializeObject<MicrosoftUser>("{\"id\":\"o\",\"displayName\":\"D\",\"mail\":\"m@x.com\",\"userPrincipalName\":\"u@x.com\",\"givenName\":\"G\",\"surname\":\"S\",\"tenantId\":\"t\",\"identityProvider\":\"live.com\"}");

            Assert.Equal("o", user.Id);
            Assert.Equal("m@x.com", user.Mail);
            Assert.Equal("u@x.com", user.UserPrincipalName);
            Assert.Equal("S", user.Surname);
            Assert.Equal("t", user.TenantId);
            Assert.Equal("live.com", user.IdentityProvider);
        }

        [Theory]
        [InlineData("email-disabled", AppleNotificationEventType.EmailDisabled)]
        [InlineData("email-enabled", AppleNotificationEventType.EmailEnabled)]
        [InlineData("consent-revoked", AppleNotificationEventType.ConsentRevoked)]
        [InlineData("account-delete", AppleNotificationEventType.AccountDelete)]
        [InlineData("something-new", AppleNotificationEventType.Unknown)]
        public void AppleNotificationPayload_ParsesEventsJsonString(string wire, AppleNotificationEventType expected)
        {
            string events = "{\"type\":\"" + wire + "\",\"sub\":\"001.abc\",\"event_time\":1700000000123,\"email\":\"r@privaterelay.appleid.com\",\"is_private_email\":\"true\"}";
            string claims = JsonConvert.SerializeObject(new { iss = "https://appleid.apple.com", aud = "net.x", iat = 1700000000, jti = "j1", events });

            AppleNotificationPayload payload = JsonConvert.DeserializeObject<AppleNotificationPayload>(claims);
            AppleNotificationEvent evt = payload.ParseEvent();

            Assert.Equal("j1", payload.Jti);
            Assert.Equal(1700000000L, payload.Iat);
            Assert.Equal(expected, evt.Type);
            Assert.Equal("001.abc", evt.Sub);
            Assert.Equal(1700000000123L, evt.EventTime);
            Assert.True(evt.IsPrivateEmail);
            Assert.Equal("r@privaterelay.appleid.com", evt.Email);
        }

        [Fact]
        public void AppleNotificationEventType_WritesWireString()
        {
            string json = JsonConvert.SerializeObject(new AppleNotificationEvent { Type = AppleNotificationEventType.ConsentRevoked, Sub = "s" });

            Assert.Contains("\"type\":\"consent-revoked\"", json);
            Assert.Null(new AppleNotificationPayload().ParseEvent());
        }

        [Theory]
        [InlineData("true", true)]
        [InlineData("FALSE", false)]
        [InlineData("", null)]
        [InlineData(null, null)]
        [InlineData("yes", null)]
        public void StringOrBoolConverter_Parse(string input, bool? expected)
        {
            Assert.Equal(expected, StringOrBoolConverter.Parse(input));
        }
    }
}
