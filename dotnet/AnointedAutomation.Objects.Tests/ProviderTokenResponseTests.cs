// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//Stewarded by Alexander Fields

using System;
using System.Collections.Generic;
using AnointedAutomation.Objects.Amazon;
using AnointedAutomation.Objects.Discord;
using AnointedAutomation.Objects.GitHub;
using AnointedAutomation.Objects.LinkedIn;
using AnointedAutomation.Objects.Reddit;
using AnointedAutomation.Objects.Twitch;
using AnointedAutomation.Objects.Yahoo;
using Newtonsoft.Json;
using Xunit;

namespace AnointedAutomation.Objects.Tests
{
    /// <summary>Provider token / envelope responses deserialize from each provider's documented sample JSON.</summary>
    public class ProviderTokenResponseTests
    {
        [Fact]
        public void DiscordTokenResponse_DeserializesDocumentedSample()
        {
            // https://discord.com/developers/docs/topics/oauth2#authorization-code-grant-access-token-response
            const string json = "{\"access_token\":\"6qrZcUqja7812RVdnEKjpzOL4CvHBFG\",\"token_type\":\"Bearer\",\"expires_in\":604800,\"refresh_token\":\"D43f5y0ahjqew82jZ4NViEr2YafMKhue\",\"scope\":\"identify\"}";

            DiscordTokenResponse r = JsonConvert.DeserializeObject<DiscordTokenResponse>(json);

            Assert.Equal("6qrZcUqja7812RVdnEKjpzOL4CvHBFG", r.AccessToken);
            Assert.Equal("Bearer", r.TokenType);
            Assert.Equal(604800L, r.ExpiresIn);
            Assert.Equal("D43f5y0ahjqew82jZ4NViEr2YafMKhue", r.RefreshToken);
            Assert.Equal("identify", r.Scope);
        }

        [Fact]
        public void GitHubTokenResponse_DeserializesDocumentedSample()
        {
            // https://docs.github.com/en/apps/oauth-apps/building-oauth-apps/authorizing-oauth-apps (Accept: application/json)
            const string json = "{\"access_token\":\"gho_16C7e42F292c6912E7710c838347Ae178B4a\",\"scope\":\"repo,gist\",\"token_type\":\"bearer\"}";

            GitHubTokenResponse r = JsonConvert.DeserializeObject<GitHubTokenResponse>(json);

            Assert.Equal("gho_16C7e42F292c6912E7710c838347Ae178B4a", r.AccessToken);
            Assert.Equal("repo,gist", r.Scope);
            Assert.Equal("bearer", r.TokenType);
            Assert.Null(r.Error);
        }

        [Fact]
        public void GitHubTokenResponse_DeserializesDocumentedError()
        {
            // https://docs.github.com/en/apps/oauth-apps/maintaining-oauth-apps/troubleshooting-oauth-app-access-token-request-errors
            const string json = "{\"error\":\"bad_verification_code\",\"error_description\":\"The code passed is incorrect or expired.\",\"error_uri\":\"https://docs.github.com/apps/managing-oauth-apps/troubleshooting-oauth-app-access-token-request-errors/#bad-verification-code\"}";

            GitHubTokenResponse r = JsonConvert.DeserializeObject<GitHubTokenResponse>(json);

            Assert.Null(r.AccessToken);
            Assert.Equal("bad_verification_code", r.Error);
            Assert.Equal("The code passed is incorrect or expired.", r.ErrorDescription);
            Assert.StartsWith("https://docs.github.com/", r.ErrorUri);
        }

        [Fact]
        public void LinkedInTokenResponse_DeserializesDocumentedSample()
        {
            // https://learn.microsoft.com/en-us/linkedin/shared/authentication/authorization-code-flow (step 3 response),
            // the documented sample verbatim. refresh_token / refresh_token_expires_in are in the field table only;
            // id_token is returned for the openid scope (sign-in-with-linkedin-v2 page). Neither page shows them in a sample.
            const string json = "{\"access_token\":\"AQUvlL_DYEzvT2wz1QJiEPeLioeA\",\"expires_in\":5184000,\"scope\":\"r_basicprofile\"}";

            LinkedInTokenResponse r = JsonConvert.DeserializeObject<LinkedInTokenResponse>(json);

            Assert.Equal("AQUvlL_DYEzvT2wz1QJiEPeLioeA", r.AccessToken);
            Assert.Equal(5184000L, r.ExpiresIn);
            Assert.Equal("r_basicprofile", r.Scope);
            Assert.Null(r.RefreshToken);
            Assert.Null(r.IdToken);
        }

        [Fact]
        public void LinkedInTokenResponse_DeserializesErrorShape()
        {
            // https://learn.microsoft.com/en-us/linkedin/shared/authentication/authorization-code-flow (error responses)
            const string json = "{\"error\":\"invalid_request\",\"error_description\":\"A required parameter \\\"redirect_uri\\\" is missing\"}";

            LinkedInTokenResponse r = JsonConvert.DeserializeObject<LinkedInTokenResponse>(json);

            Assert.Null(r.AccessToken);
            Assert.Null(r.ExpiresIn);
            Assert.Equal("invalid_request", r.Error);
            Assert.Contains("redirect_uri", r.ErrorDescription);
        }

        [Fact]
        public void TwitchTokenResponse_DeserializesDocumentedSample()
        {
            // https://dev.twitch.tv/docs/authentication/getting-tokens-oidc/ (authorization code grant flow)
            // The documented sample verbatim.
            const string json = "{\"access_token\":\"rfx2uswqe8lu4g1mkagrvg5tv0ks3\",\"expires_in\":14124,\"id_token\":\"eyJhbGciOiJSUzI1...\",\"refresh_token\":\"5b93chm63hdve3mycz05zfzatkfdenfspp1h1ar2xxdalen01\",\"scope\":[\"channel:moderate\",\"chat:edit\",\"chat:read\"],\"token_type\":\"bearer\"}";

            TwitchTokenResponse r = JsonConvert.DeserializeObject<TwitchTokenResponse>(json);

            Assert.Equal("rfx2uswqe8lu4g1mkagrvg5tv0ks3", r.AccessToken);
            Assert.Equal(14124L, r.ExpiresIn);
            Assert.StartsWith("eyJ", r.IdToken);
            Assert.Equal("5b93chm63hdve3mycz05zfzatkfdenfspp1h1ar2xxdalen01", r.RefreshToken);
            Assert.Equal(new List<string> { "channel:moderate", "chat:edit", "chat:read" }, r.Scope);
            Assert.Equal("bearer", r.TokenType);
            Assert.Null(r.Status);
        }

        [Fact]
        public void TwitchTokenResponse_DeserializesDocumentedError()
        {
            // https://dev.twitch.tv/docs/authentication/getting-tokens-oauth/ (error body: status + message)
            const string json = "{\"status\":400,\"message\":\"Invalid authorization code\"}";

            TwitchTokenResponse r = JsonConvert.DeserializeObject<TwitchTokenResponse>(json);

            Assert.Null(r.AccessToken);
            Assert.Equal(400, r.Status);
            Assert.Equal("Invalid authorization code", r.Message);
        }

        [Fact]
        public void TwitchUsersResponse_DeserializesDocumentedGetUsers()
        {
            // https://dev.twitch.tv/docs/api/reference/#get-users (example response)
            const string json = "{\"data\":[{\"id\":\"141981764\",\"login\":\"twitchdev\",\"display_name\":\"TwitchDev\",\"type\":\"\",\"broadcaster_type\":\"partner\",\"description\":\"Supporting third-party developers building Twitch integrations from chatbots to game integrations.\",\"profile_image_url\":\"https://static-cdn.jtvnw.net/jtv_user_pictures/8a6381c7-d0c0-4576-b179-38bd5ce1d6af-profile_image-300x300.png\",\"offline_image_url\":\"https://static-cdn.jtvnw.net/jtv_user_pictures/3f13ab61-ec78-4fe6-8481-8682cb3b0ac2-channel_offline_image-1920x1080.png\",\"view_count\":5980557,\"email\":\"not-real@email.com\",\"created_at\":\"2016-12-14T20:32:28Z\"}]}";

            TwitchUsersResponse r = JsonConvert.DeserializeObject<TwitchUsersResponse>(json);

            Assert.NotNull(r.Data);
            TwitchUser user = Assert.Single(r.Data);
            Assert.Equal("141981764", user.Id);
            Assert.Equal("twitchdev", user.Login);
            Assert.Equal("TwitchDev", user.DisplayName);
            Assert.Equal("partner", user.BroadcasterType);
            Assert.Equal(5980557L, user.ViewCount);
            Assert.Equal("not-real@email.com", user.Email);
            Assert.Equal(new DateTime(2016, 12, 14, 20, 32, 28, DateTimeKind.Utc), user.CreatedAt.Value.ToUniversalTime());
        }

        [Fact]
        public void YahooTokenResponse_DeserializesDocumentedSample()
        {
            // https://developer.yahoo.com/oauth2/guide/openid_connect/getting_started.html (step 4 response)
            const string json = "{\"access_token\":\"Jzxbkqqcvjqik2IMxGFEE1cuaos--\",\"token_type\":\"bearer\",\"expires_in\":3600,\"refresh_token\":\"AOiRUlJn_qOmByVGTmUpwcMKW3XDcipToOrHBYYK8qHqdK9\",\"xoauth_yahoo_guid\":\"JT4FACLQZI2OCE\",\"id_token\":\"eyJhbGciOiJFUzI1NiIsImtpZCI6IjM0NjZkNTFmN2RkMGM3ODA1NjU2ODhjMTgzOTIxODE2YzQ1ODg5YWQifQ\"}";

            YahooTokenResponse r = JsonConvert.DeserializeObject<YahooTokenResponse>(json);

            Assert.Equal("Jzxbkqqcvjqik2IMxGFEE1cuaos--", r.AccessToken);
            Assert.Equal("bearer", r.TokenType);
            Assert.Equal(3600L, r.ExpiresIn);
            Assert.Equal("AOiRUlJn_qOmByVGTmUpwcMKW3XDcipToOrHBYYK8qHqdK9", r.RefreshToken);
            Assert.Equal("JT4FACLQZI2OCE", r.XoauthYahooGuid);
            Assert.StartsWith("eyJ", r.IdToken);
            Assert.Null(r.Error);
        }

        [Fact]
        public void YahooTokenResponse_DeserializesErrorShape()
        {
            // https://developer.yahoo.com/oauth2/guide/errors/ (error + error_description)
            const string json = "{\"error\":\"invalid_grant\",\"error_description\":\"Authorization code expired\"}";

            YahooTokenResponse r = JsonConvert.DeserializeObject<YahooTokenResponse>(json);

            Assert.Null(r.AccessToken);
            Assert.Equal("invalid_grant", r.Error);
            Assert.Equal("Authorization code expired", r.ErrorDescription);
        }

        [Fact]
        public void AmazonTokenResponse_DeserializesDocumentedSample()
        {
            // https://developer.amazon.com/docs/login-with-amazon/authorization-code-grant.html (Access Token Response)
            const string json = "{\"access_token\":\"Atza|IQEBLjAsAhRmHjNgHpi0U-Dme37rR6CuUpSR\",\"token_type\":\"bearer\",\"expires_in\":3600,\"refresh_token\":\"Atzr|IQEBLzAtAhRPpMJxdwVz2Nn6f2y-tpJX2DeX\"}";

            AmazonTokenResponse r = JsonConvert.DeserializeObject<AmazonTokenResponse>(json);

            Assert.Equal("Atza|IQEBLjAsAhRmHjNgHpi0U-Dme37rR6CuUpSR", r.AccessToken);
            Assert.Equal("bearer", r.TokenType);
            Assert.Equal(3600L, r.ExpiresIn);
            Assert.Equal("Atzr|IQEBLzAtAhRPpMJxdwVz2Nn6f2y-tpJX2DeX", r.RefreshToken);
            Assert.Null(r.Error);
        }

        [Fact]
        public void AmazonTokenResponse_DeserializesDocumentedErrorParameters()
        {
            // https://developer.amazon.com/docs/login-with-amazon/authorization-code-grant.html (Access Token Errors)
            const string json = "{\"error\":\"invalid_grant\",\"error_description\":\"The authorization code is invalid, expired or revoked\",\"error_uri\":\"https://developer.amazon.com/docs/login-with-amazon/authorization-code-grant.html\"}";

            AmazonTokenResponse r = JsonConvert.DeserializeObject<AmazonTokenResponse>(json);

            Assert.Null(r.AccessToken);
            Assert.Null(r.ExpiresIn);
            Assert.Equal("invalid_grant", r.Error);
            Assert.Equal("The authorization code is invalid, expired or revoked", r.ErrorDescription);
            Assert.Equal("https://developer.amazon.com/docs/login-with-amazon/authorization-code-grant.html", r.ErrorUri);
        }

        [Fact]
        public void RedditTokenResponse_DeserializesDocumentedSample()
        {
            // https://github.com/reddit-archive/reddit/wiki/OAuth2 (Retrieving the access token)
            const string json = "{\"access_token\":\"J1qK1c18UUGJFAzz9xnH56584l4\",\"token_type\":\"bearer\",\"expires_in\":3600,\"scope\":\"identity\",\"refresh_token\":\"16924386-Z4w9JQ8C1zJ-SqiJ6uOyCPVRTew\"}";

            RedditTokenResponse r = JsonConvert.DeserializeObject<RedditTokenResponse>(json);

            Assert.Equal("J1qK1c18UUGJFAzz9xnH56584l4", r.AccessToken);
            Assert.Equal("bearer", r.TokenType);
            Assert.Equal(3600L, r.ExpiresIn);
            Assert.Equal("identity", r.Scope);
            Assert.Equal("16924386-Z4w9JQ8C1zJ-SqiJ6uOyCPVRTew", r.RefreshToken);
            Assert.Null(r.Error);
        }

        [Fact]
        public void RedditTokenResponse_RefreshAnswer_HasNoRefreshToken()
        {
            // https://github.com/reddit-archive/reddit/wiki/OAuth2 (Refreshing the token: no refresh_token field)
            const string json = "{\"access_token\":\"J1qK1c18UUGJFAzz9xnH56584l4\",\"token_type\":\"bearer\",\"expires_in\":3600,\"scope\":\"identity\"}";

            RedditTokenResponse r = JsonConvert.DeserializeObject<RedditTokenResponse>(json);

            Assert.Equal("J1qK1c18UUGJFAzz9xnH56584l4", r.AccessToken);
            Assert.Null(r.RefreshToken);
        }

        [Fact]
        public void RedditTokenResponse_DeserializesDocumentedError()
        {
            // https://github.com/reddit-archive/reddit/wiki/OAuth2 (error: invalid_grant)
            const string json = "{\"error\":\"invalid_grant\"}";

            RedditTokenResponse r = JsonConvert.DeserializeObject<RedditTokenResponse>(json);

            Assert.Null(r.AccessToken);
            Assert.Equal("invalid_grant", r.Error);
        }
    }
}
