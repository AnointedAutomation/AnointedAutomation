// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-17 Jesus is King ✝️

using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AnointedAutomation.Optimization;
using Xunit;

namespace AnointedAutomation.Optimization.Tests
{
    /// <summary>Tests for FileManagement.GetFileFromInternetAsync: status check, byte-exact writes, caller-supplied client.</summary>
    public class FileManagementDownloadTests
    {
        private sealed class StubHandler : HttpMessageHandler
        {
            private readonly HttpStatusCode _status;
            private readonly byte[] _body;

            public StubHandler(HttpStatusCode status, byte[] body)
            {
                _status = status;
                _body = body;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                HttpResponseMessage response = new HttpResponseMessage(_status);
                response.Content = new ByteArrayContent(_body);
                return Task.FromResult(response);
            }
        }

        private static string NewTempPath()
        {
            return Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        }

        [Fact]
        public async Task GetFileFromInternetAsync_BinaryBody_IsWrittenByteForByte()
        {
            byte[] body = new byte[] { 0x00, 0xFF, 0x80, 0xC3, 0x28, 0x89, 0x50, 0x4E, 0x47, 0xFE };
            string path = NewTempPath();
            try
            {
                using HttpClient client = new HttpClient(new StubHandler(HttpStatusCode.OK, body));

                await FileManagement.GetFileFromInternetAsync(client, "http://example.test/a.bin", path);

                Assert.Equal(body, await File.ReadAllBytesAsync(path));
            }
            finally
            {
                File.Delete(path);
            }
        }

        [Theory]
        [InlineData(HttpStatusCode.NotFound)]
        [InlineData(HttpStatusCode.InternalServerError)]
        public async Task GetFileFromInternetAsync_ErrorStatus_ThrowsAndWritesNoFile(HttpStatusCode status)
        {
            string path = NewTempPath();
            using HttpClient client = new HttpClient(new StubHandler(status, new byte[] { 1, 2, 3 }));

            await Assert.ThrowsAsync<HttpRequestException>(() => FileManagement.GetFileFromInternetAsync(client, "http://example.test/x", path));

            Assert.False(File.Exists(path));
        }

        [Fact]
        public async Task GetFileFromInternetAsync_NullClient_Throws()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => FileManagement.GetFileFromInternetAsync(null!, "http://example.test/x", NewTempPath()));
        }
    }
}
