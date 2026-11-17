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

        private sealed class FailingStream : Stream
        {
            public override bool CanRead => true;
            public override bool CanSeek => false;
            public override bool CanWrite => false;
            public override long Length => throw new NotSupportedException();
            public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
            public override void Flush() { }
            public override int Read(byte[] buffer, int offset, int count) => throw new IOException("connection reset mid-body");
            public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default) => throw new IOException("connection reset mid-body");
            public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
            public override void SetLength(long value) => throw new NotSupportedException();
            public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
        }

        private sealed class FailingBodyHandler : HttpMessageHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StreamContent(new FailingStream());
                return Task.FromResult(response);
            }
        }

        private sealed class CancellingStream : Stream
        {
            private readonly CancellationTokenSource _source;
            public CancellingStream(CancellationTokenSource source) { _source = source; }
            public override bool CanRead => true;
            public override bool CanSeek => false;
            public override bool CanWrite => false;
            public override long Length => throw new NotSupportedException();
            public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
            public override void Flush() { }
            public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();
            public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
            {
                _source.Cancel();
                cancellationToken.ThrowIfCancellationRequested();
                return ValueTask.FromResult(0);
            }
            public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
            public override void SetLength(long value) => throw new NotSupportedException();
            public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
        }

        private sealed class CancellingBodyHandler : HttpMessageHandler
        {
            private readonly CancellationTokenSource _source;
            public CancellingBodyHandler(CancellationTokenSource source) { _source = source; }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StreamContent(new CancellingStream(_source));
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

        [Fact]
        public async Task GetFileFromInternetAsync_BodyFailsMidCopy_RethrowsAndLeavesNoPartialFile()
        {
            string path = NewTempPath();
            using HttpClient client = new HttpClient(new FailingBodyHandler());

            await Assert.ThrowsAsync<IOException>(() => FileManagement.GetFileFromInternetAsync(client, "http://example.test/big.bin", path));

            Assert.False(File.Exists(path));
        }

        [Fact]
        public async Task GetFileFromInternetAsync_ErrorStatus_KeepsAnExistingFile()
        {
            string path = NewTempPath();
            await File.WriteAllBytesAsync(path, new byte[] { 9, 9, 9 });
            try
            {
                using HttpClient client = new HttpClient(new StubHandler(HttpStatusCode.NotFound, new byte[] { 1 }));

                await Assert.ThrowsAsync<HttpRequestException>(() => FileManagement.GetFileFromInternetAsync(client, "http://example.test/x", path));

                Assert.Equal(new byte[] { 9, 9, 9 }, await File.ReadAllBytesAsync(path));
            }
            finally
            {
                File.Delete(path);
            }
        }

        [Fact]
        public void DescribeUrlWithoutSecrets_DropsQueryFragmentAndUserInfo()
        {
            string described = FileManagement.DescribeUrlWithoutSecrets("https://user:pw@cdn.example.test/files/a.zip?token=SECRET&sig=abc#frag");

            Assert.Equal("https://cdn.example.test/files/a.zip", described);
            Assert.DoesNotContain("SECRET", described);
            Assert.DoesNotContain("pw", described);
        }

        [Fact]
        public void DescribeUrlWithoutSecrets_InvalidUrl_DoesNotEchoTheInput()
        {
            Assert.DoesNotContain("token=SECRET", FileManagement.DescribeUrlWithoutSecrets("not a url?token=SECRET"));
        }

        [Fact]
        public async Task GetFileFromInternetAsync_CancelledMidCopy_ThrowsCancellationAndLeavesNoFile()
        {
            string path = NewTempPath();
            using CancellationTokenSource source = new CancellationTokenSource();
            using HttpClient client = new HttpClient(new CancellingBodyHandler(source));

            await Assert.ThrowsAnyAsync<OperationCanceledException>(() => FileManagement.GetFileFromInternetAsync(client, "http://example.test/big.bin", path, source.Token));

            Assert.False(File.Exists(path));
        }
    }
}
