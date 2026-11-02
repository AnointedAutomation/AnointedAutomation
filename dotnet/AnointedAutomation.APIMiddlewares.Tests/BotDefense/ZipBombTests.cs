// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-02 00:00:00 Jesus is King ✝️
using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using AnointedAutomation.APIMiddleware.BotDefense;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Xunit;

namespace AnointedAutomation.APIMiddlewares.Tests.BotDefense
{
    [Collection("Sequential")]
    public class ZipBombTests
    {
        [Fact]
        public void Get_IsValidZipThatExpandsToConfiguredSizeAndStaysSmall()
        {
            long expanded = 8L * 1024 * 1024;
            byte[] bytes = ZipBomb.Get(expanded);
            Assert.True(bytes.Length < expanded / 100);
            using (ZipArchive archive = new ZipArchive(new MemoryStream(bytes), ZipArchiveMode.Read))
            {
                ZipArchiveEntry entry = Assert.Single(archive.Entries);
                Assert.Equal(".env", entry.Name);
                Assert.Equal(expanded, entry.Length);
                long total = 0;
                byte[] buffer = new byte[65536];
                using (Stream s = entry.Open())
                {
                    int n;
                    while ((n = s.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        total += n;
                    }
                }
                Assert.Equal(expanded, total);
            }
        }

        [Fact]
        public void Get_CachesPerSize()
        {
            Assert.Same(ZipBomb.Get(1024), ZipBomb.Get(1024));
            Assert.NotSame(ZipBomb.Get(1024), ZipBomb.Get(2048));
        }

        [Theory]
        [InlineData(0L)]
        [InlineData(-1L)]
        [InlineData(ZipBomb.MaxExpandedBytes + 1)]
        public void Get_RejectsOutOfRangeSize(long size)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => ZipBomb.Get(size));
        }

        private static BotDefenseMiddleware Build(Action<BotDefenseOptions> configure, out InMemoryBanStore bans)
        {
            BotDefenseOptions options = new BotDefenseOptions { Enabled = true };
            options.HoneypotPaths.Enabled = true;
            configure(options);
            bans = new InMemoryBanStore(() => DateTimeOffset.UtcNow);
            return new BotDefenseMiddleware(ctx => Task.CompletedTask, Options.Create(options), bans, new FakeVerifier(), new FixedWindowRateLimiter(() => DateTimeOffset.UtcNow));
        }

        private sealed class FakeVerifier : ICrawlerVerifier
        {
            public Task<bool> VerifyAsync(System.Net.IPAddress address, System.Collections.Generic.IReadOnlyList<string> hostSuffixes, System.Threading.CancellationToken cancellationToken)
            {
                return Task.FromResult(false);
            }
        }

        private static DefaultHttpContext Request(string path)
        {
            DefaultHttpContext context = new DefaultHttpContext();
            context.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("198.51.100.9");
            context.Request.Path = path;
            context.Response.Body = new MemoryStream();
            return context;
        }

        [Fact]
        public async Task EnvProbe_IsAnsweredWithZipBombAndIpBanned()
        {
            BotDefenseMiddleware middleware = Build(o =>
            {
                o.HoneypotPaths.ServeEnvZipBomb = true;
                o.HoneypotPaths.ZipBombExpandedBytes = 4L * 1024 * 1024;
            }, out InMemoryBanStore bans);
            DefaultHttpContext context = Request("/app/.env");
            await middleware.InvokeAsync(context);
            Assert.Equal(200, context.Response.StatusCode);
            Assert.Equal("application/zip", context.Response.ContentType);
            Assert.Equal(ZipBomb.Get(4L * 1024 * 1024).Length, context.Response.Body.Length);
            Assert.NotNull(await bans.GetBanAsync("198.51.100.9", System.Threading.CancellationToken.None));
        }

        [Fact]
        public async Task NonEnvHoneypot_StillGetsPlainResponseWhenBombEnabled()
        {
            BotDefenseMiddleware middleware = Build(o => o.HoneypotPaths.ServeEnvZipBomb = true, out InMemoryBanStore bans);
            DefaultHttpContext context = Request("/wp-admin");
            await middleware.InvokeAsync(context);
            Assert.Equal(404, context.Response.StatusCode);
            Assert.Equal(0, context.Response.Body.Length);
        }

        [Fact]
        public async Task EnvProbe_WithBombOff_GetsPlainResponse()
        {
            BotDefenseMiddleware middleware = Build(o => { }, out InMemoryBanStore bans);
            DefaultHttpContext context = Request("/.env");
            await middleware.InvokeAsync(context);
            Assert.Equal(404, context.Response.StatusCode);
            Assert.Equal(0, context.Response.Body.Length);
        }

        [Fact]
        public void Validate_RejectsBadBombSizeOnlyWhenEnabled()
        {
            BotDefenseOptions options = new BotDefenseOptions { Enabled = true };
            options.HoneypotPaths.Enabled = true;
            options.HoneypotPaths.ZipBombExpandedBytes = 0;
            options.Validate();
            options.HoneypotPaths.ServeEnvZipBomb = true;
            Assert.Throws<InvalidOperationException>(() => options.Validate());
        }
    }
}
