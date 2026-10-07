// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
using AnointedAutomation.SSO;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace AnointedAutomation.SSO.Tests
{
    public class AnointedLogoutTokenValidatorTests
    {
        private static readonly DateTimeOffset Now = new DateTimeOffset(2026, 10, 7, 12, 0, 0, TimeSpan.Zero);
        private readonly TestKeys _keys = new TestKeys("k1");
        private readonly MutableTimeProvider _clock = new MutableTimeProvider(Now);
        private readonly FakeConfigurationManager _configuration;
        private readonly AnointedLogoutTokenValidator _validator;

        /// <summary>Builds a validator over the test key.</summary>
        public AnointedLogoutTokenValidatorTests()
        {
            _configuration = new FakeConfigurationManager(TestKeys.ConfigurationWith(_keys), null);
            _validator = CreateValidator(_configuration, _clock);
        }

        /// <summary>A validator with the test issuer and client id.</summary>
        /// <param name="configuration">The key source.</param>
        /// <param name="clock">The clock.</param>
        /// <returns>The validator.</returns>
        internal static AnointedLogoutTokenValidator CreateValidator(FakeConfigurationManager configuration, TimeProvider clock)
        {
            AnointedLogoutValidationOptions options = new AnointedLogoutValidationOptions { Issuer = TestKeys.Issuer, ClientId = TestKeys.ClientId };
            return new AnointedLogoutTokenValidator(options, configuration, clock);
        }

        /// <summary>Validates a token with no cancellation.</summary>
        /// <param name="token">The token.</param>
        /// <returns>The result.</returns>
        private Task<AnointedLogoutValidationResult> Validate(string token)
        {
            return _validator.ValidateAsync(token, CancellationToken.None);
        }

        /// <summary>Signs a payload with the valid header.</summary>
        /// <param name="payload">The payload.</param>
        /// <returns>The token.</returns>
        private string Sign(Dictionary<string, object> payload)
        {
            return _keys.SignRs256(_keys.ValidHeader(), payload);
        }

        [Fact]
        public async Task Valid_Token_ReturnsLogout()
        {
            Dictionary<string, object> payload = TestKeys.ValidPayload(Now);
            AnointedLogoutValidationResult result = await Validate(Sign(payload));
            Assert.True(result.IsValid, result.Error);
            Assert.Equal("user-1", result.Logout.Sub);
            Assert.Equal("grant-1", result.Logout.Sid);
            Assert.Equal((string)payload["jti"], result.Logout.Jti);
            Assert.Equal(Now.AddMinutes(2), result.Logout.ExpiresAt);
        }

        [Fact]
        public async Task Valid_AudienceArrayContainingClient()
        {
            Dictionary<string, object> payload = TestKeys.ValidPayload(Now);
            payload["aud"] = new[] { "someone-else", TestKeys.ClientId };
            AnointedLogoutValidationResult result = await Validate(Sign(payload));
            Assert.True(result.IsValid, result.Error);
        }

        [Fact]
        public async Task Valid_OnlySid()
        {
            Dictionary<string, object> payload = TestKeys.ValidPayload(Now);
            payload.Remove("sub");
            AnointedLogoutValidationResult result = await Validate(Sign(payload));
            Assert.True(result.IsValid, result.Error);
            Assert.Null(result.Logout.Sub);
            Assert.Equal("grant-1", result.Logout.Sid);
        }

        [Fact]
        public async Task Valid_WithinClockSkew()
        {
            _clock.Now = Now.AddMinutes(2).AddSeconds(30);
            AnointedLogoutValidationResult result = await Validate(Sign(TestKeys.ValidPayload(Now)));
            Assert.True(result.IsValid, result.Error);
        }

        [Fact]
        public async Task Rejects_Hs256()
        {
            Dictionary<string, object> header = _keys.ValidHeader();
            header["alg"] = "HS256";
            AnointedLogoutValidationResult result = await Validate(TestKeys.SignHs256(header, TestKeys.ValidPayload(Now)));
            Assert.False(result.IsValid);
            Assert.Contains("alg", result.Error, StringComparison.Ordinal);
        }

        [Fact]
        public async Task Rejects_AlgNone()
        {
            Dictionary<string, object> header = _keys.ValidHeader();
            header["alg"] = "none";
            AnointedLogoutValidationResult result = await Validate(TestKeys.Unsigned(header, TestKeys.ValidPayload(Now)));
            Assert.False(result.IsValid);
        }

        [Fact]
        public async Task Rejects_Rs256HeaderWithEmptySignature()
        {
            AnointedLogoutValidationResult result = await Validate(TestKeys.Unsigned(_keys.ValidHeader(), TestKeys.ValidPayload(Now)));
            Assert.False(result.IsValid);
        }

        [Fact]
        public async Task Rejects_BadSignature()
        {
            TestKeys impostor = new TestKeys("k1");
            AnointedLogoutValidationResult result = await Validate(impostor.SignRs256(impostor.ValidHeader(), TestKeys.ValidPayload(Now)));
            Assert.False(result.IsValid);
        }

        [Fact]
        public async Task Rejects_TamperedPayload()
        {
            string token = Sign(TestKeys.ValidPayload(Now));
            string[] parts = token.Split('.');
            Dictionary<string, object> other = TestKeys.ValidPayload(Now);
            other["sub"] = "someone-else";
            string forged = _keys.SignRs256(_keys.ValidHeader(), other).Split('.')[1];
            AnointedLogoutValidationResult result = await Validate(parts[0] + "." + forged + "." + parts[2]);
            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData("JWT")]
        [InlineData("at+jwt")]
        [InlineData("application/logout+jwt")]
        public async Task Rejects_WrongTyp(string typ)
        {
            Dictionary<string, object> header = _keys.ValidHeader();
            header["typ"] = typ;
            AnointedLogoutValidationResult result = await Validate(_keys.SignRs256(header, TestKeys.ValidPayload(Now)));
            Assert.False(result.IsValid);
        }

        [Fact]
        public async Task Rejects_MissingTyp()
        {
            Dictionary<string, object> header = _keys.ValidHeader();
            header.Remove("typ");
            AnointedLogoutValidationResult result = await Validate(_keys.SignRs256(header, TestKeys.ValidPayload(Now)));
            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData("https://issuer.test")]
        [InlineData("https://evil.test/")]
        public async Task Rejects_WrongIssuer(string issuer)
        {
            Dictionary<string, object> payload = TestKeys.ValidPayload(Now);
            payload["iss"] = issuer;
            AnointedLogoutValidationResult result = await Validate(Sign(payload));
            Assert.False(result.IsValid);
        }

        [Fact]
        public async Task Rejects_WrongAudience()
        {
            Dictionary<string, object> payload = TestKeys.ValidPayload(Now);
            payload["aud"] = "aa_other";
            AnointedLogoutValidationResult result = await Validate(Sign(payload));
            Assert.False(result.IsValid);
        }

        [Fact]
        public async Task Rejects_TooOldIat()
        {
            Dictionary<string, object> payload = TestKeys.ValidPayload(Now.AddMinutes(-10));
            payload["exp"] = Now.AddMinutes(5).ToUnixTimeSeconds();
            AnointedLogoutValidationResult result = await Validate(Sign(payload));
            Assert.False(result.IsValid);
            Assert.Equal("token is too old", result.Error);
        }

        [Fact]
        public async Task Rejects_IatInFuture()
        {
            Dictionary<string, object> payload = TestKeys.ValidPayload(Now.AddMinutes(5));
            AnointedLogoutValidationResult result = await Validate(Sign(payload));
            Assert.False(result.IsValid);
        }

        [Fact]
        public async Task Rejects_Expired()
        {
            Dictionary<string, object> payload = TestKeys.ValidPayload(Now.AddMinutes(-4));
            AnointedLogoutValidationResult result = await Validate(Sign(payload));
            Assert.False(result.IsValid);
            Assert.Equal("token is expired", result.Error);
        }

        [Theory]
        [InlineData("iat")]
        [InlineData("exp")]
        [InlineData("jti")]
        [InlineData("events")]
        public async Task Rejects_MissingRequiredClaim(string claim)
        {
            Dictionary<string, object> payload = TestKeys.ValidPayload(Now);
            payload.Remove(claim);
            AnointedLogoutValidationResult result = await Validate(Sign(payload));
            Assert.False(result.IsValid);
        }

        [Fact]
        public async Task Rejects_EventsWithoutBackchannelMember()
        {
            Dictionary<string, object> payload = TestKeys.ValidPayload(Now);
            payload["events"] = new Dictionary<string, object> { { "http://schemas.openid.net/event/other", new Dictionary<string, object>() } };
            AnointedLogoutValidationResult result = await Validate(Sign(payload));
            Assert.False(result.IsValid);
        }

        [Fact]
        public async Task Rejects_EventMemberNotAnObject()
        {
            Dictionary<string, object> payload = TestKeys.ValidPayload(Now);
            payload["events"] = new Dictionary<string, object> { { AnointedAutomationDefaults.BackchannelLogoutEvent, "yes" } };
            AnointedLogoutValidationResult result = await Validate(Sign(payload));
            Assert.False(result.IsValid);
        }

        [Fact]
        public async Task Rejects_NoncePresent()
        {
            Dictionary<string, object> payload = TestKeys.ValidPayload(Now);
            payload["nonce"] = "n";
            AnointedLogoutValidationResult result = await Validate(Sign(payload));
            Assert.False(result.IsValid);
            Assert.Contains("nonce", result.Error, StringComparison.Ordinal);
        }

        [Fact]
        public async Task Rejects_MissingSubAndSid()
        {
            Dictionary<string, object> payload = TestKeys.ValidPayload(Now);
            payload.Remove("sub");
            payload.Remove("sid");
            AnointedLogoutValidationResult result = await Validate(Sign(payload));
            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData("")]
        [InlineData("not-a-jwt")]
        [InlineData("a.b.c")]
        public async Task Rejects_Malformed(string token)
        {
            AnointedLogoutValidationResult result = await Validate(token);
            Assert.False(result.IsValid);
        }

        [Fact]
        public async Task UnknownKid_RefreshesKeysOnce_ThenValidates()
        {
            TestKeys rotated = new TestKeys("k2");
            _configuration.AfterRefresh = TestKeys.ConfigurationWith(_keys, rotated);
            AnointedLogoutValidationResult result = await Validate(rotated.SignRs256(rotated.ValidHeader(), TestKeys.ValidPayload(Now)));
            Assert.True(result.IsValid, result.Error);
            Assert.Equal(1, _configuration.RefreshCount);
        }

        [Fact]
        public async Task UnknownKid_StillUnknownAfterRefresh_Rejects()
        {
            TestKeys stranger = new TestKeys("k9");
            AnointedLogoutValidationResult result = await Validate(stranger.SignRs256(stranger.ValidHeader(), TestKeys.ValidPayload(Now)));
            Assert.False(result.IsValid);
            Assert.Equal(1, _configuration.RefreshCount);
        }

        [Fact]
        public async Task KnownKid_DoesNotRefresh()
        {
            await Validate(Sign(TestKeys.ValidPayload(Now)));
            Assert.Equal(0, _configuration.RefreshCount);
        }

        [Fact]
        public async Task KeysUnavailable_Throws()
        {
            _configuration.Fail = true;
            await Assert.ThrowsAsync<InvalidOperationException>(() => Validate(Sign(TestKeys.ValidPayload(Now))));
        }

        [Fact]
        public void Options_MissingClientId_Throws()
        {
            AnointedLogoutValidationOptions options = new AnointedLogoutValidationOptions { Issuer = TestKeys.Issuer };
            Assert.Throws<InvalidOperationException>(() => new AnointedLogoutTokenValidator(options, _configuration, _clock));
        }
    }

    public class InMemoryAnointedLogoutReplayStoreTests
    {
        [Fact]
        public async Task TryAdd_NewThenDuplicate()
        {
            MutableTimeProvider clock = new MutableTimeProvider(new DateTimeOffset(2026, 10, 7, 0, 0, 0, TimeSpan.Zero));
            InMemoryAnointedLogoutReplayStore store = new InMemoryAnointedLogoutReplayStore(clock);
            Assert.True(await store.TryAddAsync("j1", clock.Now.AddMinutes(3), CancellationToken.None));
            Assert.False(await store.TryAddAsync("j1", clock.Now.AddMinutes(3), CancellationToken.None));
            Assert.True(await store.TryAddAsync("j2", clock.Now.AddMinutes(3), CancellationToken.None));
        }

        [Fact]
        public async Task TryAdd_AfterExpiry_IsNewAgain()
        {
            MutableTimeProvider clock = new MutableTimeProvider(new DateTimeOffset(2026, 10, 7, 0, 0, 0, TimeSpan.Zero));
            InMemoryAnointedLogoutReplayStore store = new InMemoryAnointedLogoutReplayStore(clock);
            Assert.True(await store.TryAddAsync("j1", clock.Now.AddMinutes(3), CancellationToken.None));
            clock.Advance(TimeSpan.FromMinutes(4));
            Assert.True(await store.TryAddAsync("j1", clock.Now.AddMinutes(3), CancellationToken.None));
        }

        [Fact]
        public async Task Remove_AllowsReAdd()
        {
            MutableTimeProvider clock = new MutableTimeProvider(new DateTimeOffset(2026, 10, 7, 0, 0, 0, TimeSpan.Zero));
            InMemoryAnointedLogoutReplayStore store = new InMemoryAnointedLogoutReplayStore(clock);
            Assert.True(await store.TryAddAsync("j1", clock.Now.AddMinutes(3), CancellationToken.None));
            await store.RemoveAsync("j1", CancellationToken.None);
            Assert.True(await store.TryAddAsync("j1", clock.Now.AddMinutes(3), CancellationToken.None));
        }
    }
}
