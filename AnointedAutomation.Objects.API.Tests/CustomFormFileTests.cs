// Copyright 2026 Anointed Automation, LLC. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace AnointedAutomation.Objects.API.Tests
{
    /// <summary>
    /// Unit tests for the CustomFormFile class.
    /// Tests cover success, failure, and edge cases per CLAUDE_TESTING.md standards.
    /// </summary>
    public class CustomFormFileTests
    {
        #region Constructor Tests

        [Fact]
        public void DefaultConstructor_CreatesInstance()
        {
            // Act
            CustomFormFile formFile = new CustomFormFile();

            // Assert
            Assert.NotNull(formFile);
        }

        [Fact]
        public void Constructor_WithFileNameAndContent_SetsProperties()
        {
            // Arrange
            string fileName = "test.txt";
            byte[] content = Encoding.UTF8.GetBytes("Hello World");

            // Act
            CustomFormFile formFile = new CustomFormFile(fileName, content);

            // Assert
            Assert.Equal(fileName, formFile.FileName);
            Assert.Equal(content, formFile.Content);
            Assert.Equal(content.Length, formFile.Length);
        }

        [Fact]
        public void Constructor_WithNullFileName_ThrowsArgumentNullException()
        {
            // Arrange
            string fileName = null;
            byte[] content = new byte[] { 1, 2, 3 };

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new CustomFormFile(fileName, content));
        }

        [Fact]
        public void Constructor_WithNullContent_SetsEmptyByteArray()
        {
            // Arrange
            string fileName = "test.txt";
            byte[] content = null;

            // Act
            CustomFormFile formFile = new CustomFormFile(fileName, content);

            // Assert
            Assert.NotNull(formFile.Content);
            Assert.Empty(formFile.Content);
            Assert.Equal(0, formFile.Length);
        }

        [Fact]
        public void Constructor_WithEmptyFileName_SetsFileName()
        {
            // Arrange
            string fileName = "";
            byte[] content = new byte[] { 1, 2, 3 };

            // Act
            CustomFormFile formFile = new CustomFormFile(fileName, content);

            // Assert
            Assert.Equal("", formFile.FileName);
        }

        [Fact]
        public void Constructor_WithEmptyContent_SetsEmptyArray()
        {
            // Arrange
            string fileName = "test.txt";
            byte[] content = new byte[0];

            // Act
            CustomFormFile formFile = new CustomFormFile(fileName, content);

            // Assert
            Assert.Empty(formFile.Content);
            Assert.Equal(0, formFile.Length);
        }

        #endregion

        #region Property Tests

        [Fact]
        public void Content_CanBeSetAndRetrieved()
        {
            // Arrange
            CustomFormFile formFile = new CustomFormFile();
            byte[] content = new byte[] { 1, 2, 3, 4, 5 };

            // Act
            formFile.Content = content;

            // Assert
            Assert.Equal(content, formFile.Content);
        }

        [Fact]
        public void FileName_CanBeSetAndRetrieved()
        {
            // Arrange
            CustomFormFile formFile = new CustomFormFile();
            string fileName = "document.pdf";

            // Act
            formFile.FileName = fileName;

            // Assert
            Assert.Equal(fileName, formFile.FileName);
        }

        [Fact]
        public void ContentDisposition_ReturnsCorrectFormat()
        {
            // Arrange
            string fileName = "test.txt";
            byte[] content = new byte[] { 1, 2, 3 };
            CustomFormFile formFile = new CustomFormFile(fileName, content);

            // Act
            string contentDisposition = formFile.ContentDisposition;

            // Assert
            Assert.Equal("form-data; name=\"file\"; filename=\"test.txt\"", contentDisposition);
        }

        [Fact]
        public void ContentType_ReturnsOctetStream()
        {
            // Arrange
            CustomFormFile formFile = new CustomFormFile("test.txt", new byte[] { 1, 2, 3 });

            // Act
            string contentType = formFile.ContentType;

            // Assert
            Assert.Equal("application/octet-stream", contentType);
        }

        [Fact]
        public void Headers_ReturnsHeaderDictionary()
        {
            // Arrange
            CustomFormFile formFile = new CustomFormFile("test.txt", new byte[] { 1, 2, 3 });

            // Act
            IHeaderDictionary headers = formFile.Headers;

            // Assert
            Assert.NotNull(headers);
            Assert.IsType<HeaderDictionary>(headers);
        }

        [Fact]
        public void Length_ReturnsContentLength()
        {
            // Arrange
            byte[] content = new byte[] { 1, 2, 3, 4, 5 };
            CustomFormFile formFile = new CustomFormFile("test.txt", content);

            // Act
            long length = formFile.Length;

            // Assert
            Assert.Equal(5, length);
        }

        [Fact]
        public void Name_ReturnsFile()
        {
            // Arrange
            CustomFormFile formFile = new CustomFormFile("test.txt", new byte[] { 1, 2, 3 });

            // Act
            string name = formFile.Name;

            // Assert
            Assert.Equal("file", name);
        }

        #endregion

        #region CopyTo Tests

        [Fact]
        public void CopyTo_CopiesToStream()
        {
            // Arrange
            byte[] content = Encoding.UTF8.GetBytes("Test content");
            CustomFormFile formFile = new CustomFormFile("test.txt", content);
            MemoryStream targetStream = new MemoryStream();

            // Act
            formFile.CopyTo(targetStream);

            // Assert
            Assert.Equal(content.Length, targetStream.Length);
            Assert.Equal(content, targetStream.ToArray());
        }

        [Fact]
        public void CopyTo_WithEmptyContent_WritesNothing()
        {
            // Arrange
            byte[] content = new byte[0];
            CustomFormFile formFile = new CustomFormFile("test.txt", content);
            MemoryStream targetStream = new MemoryStream();

            // Act
            formFile.CopyTo(targetStream);

            // Assert
            Assert.Equal(0, targetStream.Length);
        }

        [Fact]
        public void CopyTo_WithLargeContent_CopiesSuccessfully()
        {
            // Arrange
            byte[] content = new byte[10000];
            new Random(42).NextBytes(content);
            CustomFormFile formFile = new CustomFormFile("large.bin", content);
            MemoryStream targetStream = new MemoryStream();

            // Act
            formFile.CopyTo(targetStream);

            // Assert
            Assert.Equal(content.Length, targetStream.Length);
            Assert.Equal(content, targetStream.ToArray());
        }

        #endregion

        #region CopyToAsync Tests

        [Fact]
        public async Task CopyToAsync_CopiesToStream()
        {
            // Arrange
            byte[] content = Encoding.UTF8.GetBytes("Async test content");
            CustomFormFile formFile = new CustomFormFile("test.txt", content);
            MemoryStream targetStream = new MemoryStream();

            // Act
            await formFile.CopyToAsync(targetStream);

            // Assert
            Assert.Equal(content.Length, targetStream.Length);
            Assert.Equal(content, targetStream.ToArray());
        }

        [Fact]
        public async Task CopyToAsync_WithCancellationToken_CopiesToStream()
        {
            // Arrange
            byte[] content = Encoding.UTF8.GetBytes("Cancellation token test");
            CustomFormFile formFile = new CustomFormFile("test.txt", content);
            MemoryStream targetStream = new MemoryStream();
            CancellationToken cancellationToken = CancellationToken.None;

            // Act
            await formFile.CopyToAsync(targetStream, cancellationToken);

            // Assert
            Assert.Equal(content.Length, targetStream.Length);
            Assert.Equal(content, targetStream.ToArray());
        }

        [Fact]
        public async Task CopyToAsync_WithEmptyContent_WritesNothing()
        {
            // Arrange
            byte[] content = new byte[0];
            CustomFormFile formFile = new CustomFormFile("test.txt", content);
            MemoryStream targetStream = new MemoryStream();

            // Act
            await formFile.CopyToAsync(targetStream);

            // Assert
            Assert.Equal(0, targetStream.Length);
        }

        [Fact]
        public async Task CopyToAsync_WithLargeContent_CopiesSuccessfully()
        {
            // Arrange
            byte[] content = new byte[10000];
            new Random(42).NextBytes(content);
            CustomFormFile formFile = new CustomFormFile("large.bin", content);
            MemoryStream targetStream = new MemoryStream();

            // Act
            await formFile.CopyToAsync(targetStream);

            // Assert
            Assert.Equal(content.Length, targetStream.Length);
            Assert.Equal(content, targetStream.ToArray());
        }

        #endregion

        #region OpenReadStream Tests

        [Fact]
        public void OpenReadStream_ReturnsReadableStream()
        {
            // Arrange
            byte[] content = Encoding.UTF8.GetBytes("Stream content");
            CustomFormFile formFile = new CustomFormFile("test.txt", content);

            // Act
            Stream stream = formFile.OpenReadStream();

            // Assert
            Assert.NotNull(stream);
            Assert.True(stream.CanRead);
            Assert.Equal(content.Length, stream.Length);
        }

        [Fact]
        public void OpenReadStream_StreamContainsCorrectContent()
        {
            // Arrange
            byte[] content = Encoding.UTF8.GetBytes("Hello World");
            CustomFormFile formFile = new CustomFormFile("test.txt", content);

            // Act
            using (Stream stream = formFile.OpenReadStream())
            {
                byte[] readContent = new byte[content.Length];
                int bytesRead = stream.Read(readContent, 0, readContent.Length);

                // Assert
                Assert.Equal(content.Length, bytesRead);
                Assert.Equal(content, readContent);
            }
        }

        [Fact]
        public void OpenReadStream_WithEmptyContent_ReturnsEmptyStream()
        {
            // Arrange
            byte[] content = new byte[0];
            CustomFormFile formFile = new CustomFormFile("test.txt", content);

            // Act
            using (Stream stream = formFile.OpenReadStream())
            {
                // Assert
                Assert.Equal(0, stream.Length);
            }
        }

        [Fact]
        public void OpenReadStream_ReturnsMemoryStream()
        {
            // Arrange
            byte[] content = new byte[] { 1, 2, 3 };
            CustomFormFile formFile = new CustomFormFile("test.txt", content);

            // Act
            Stream stream = formFile.OpenReadStream();

            // Assert
            Assert.IsType<MemoryStream>(stream);
        }

        #endregion

        #region IFormFile Interface Tests

        [Fact]
        public void CustomFormFile_ImplementsIFormFile()
        {
            // Arrange
            CustomFormFile formFile = new CustomFormFile("test.txt", new byte[] { 1, 2, 3 });

            // Assert
            Assert.IsAssignableFrom<IFormFile>(formFile);
        }

        #endregion

        #region Edge Case Tests

        [Fact]
        public void Constructor_WithSpecialCharactersInFileName_SetsFileName()
        {
            // Arrange
            string fileName = "test file (1) [copy].txt";
            byte[] content = new byte[] { 1, 2, 3 };

            // Act
            CustomFormFile formFile = new CustomFormFile(fileName, content);

            // Assert
            Assert.Equal(fileName, formFile.FileName);
        }

        [Fact]
        public void Constructor_WithUnicodeFileName_SetsFileName()
        {
            // Arrange
            string fileName = "test_file.txt";
            byte[] content = new byte[] { 1, 2, 3 };

            // Act
            CustomFormFile formFile = new CustomFormFile(fileName, content);

            // Assert
            Assert.Equal(fileName, formFile.FileName);
        }

        [Fact]
        public void Constructor_WithVeryLongFileName_SetsFileName()
        {
            // Arrange
            string fileName = new string('a', 1000) + ".txt";
            byte[] content = new byte[] { 1, 2, 3 };

            // Act
            CustomFormFile formFile = new CustomFormFile(fileName, content);

            // Assert
            Assert.Equal(fileName, formFile.FileName);
        }

        [Fact]
        public void ContentDisposition_WithSpecialCharactersInFileName_FormatsCorrectly()
        {
            // Arrange
            string fileName = "test file.txt";
            byte[] content = new byte[] { 1, 2, 3 };
            CustomFormFile formFile = new CustomFormFile(fileName, content);

            // Act
            string contentDisposition = formFile.ContentDisposition;

            // Assert
            Assert.Equal("form-data; name=\"file\"; filename=\"test file.txt\"", contentDisposition);
        }

        [Fact]
        public void Headers_ReturnsNewInstanceEachCall()
        {
            // Arrange
            CustomFormFile formFile = new CustomFormFile("test.txt", new byte[] { 1, 2, 3 });

            // Act
            IHeaderDictionary headers1 = formFile.Headers;
            IHeaderDictionary headers2 = formFile.Headers;

            // Assert
            Assert.NotSame(headers1, headers2);
        }

        [Fact]
        public void OpenReadStream_CanBeCalledMultipleTimes()
        {
            // Arrange
            byte[] content = new byte[] { 1, 2, 3 };
            CustomFormFile formFile = new CustomFormFile("test.txt", content);

            // Act
            Stream stream1 = formFile.OpenReadStream();
            Stream stream2 = formFile.OpenReadStream();

            // Assert
            Assert.NotNull(stream1);
            Assert.NotNull(stream2);
            Assert.NotSame(stream1, stream2);

            // Cleanup
            stream1.Dispose();
            stream2.Dispose();
        }

        [Fact]
        public void Length_WithNullContent_ThrowsNullReferenceException()
        {
            // Arrange
            CustomFormFile formFile = new CustomFormFile();
            formFile.Content = null;

            // Act & Assert
            Assert.Throws<NullReferenceException>(() => { long length = formFile.Length; });
        }

        [Fact]
        public void CopyTo_WithNullContent_ThrowsNullReferenceException()
        {
            // Arrange
            CustomFormFile formFile = new CustomFormFile();
            formFile.Content = null;
            MemoryStream targetStream = new MemoryStream();

            // Act & Assert
            Assert.Throws<NullReferenceException>(() => formFile.CopyTo(targetStream));
        }

        [Fact]
        public async Task CopyToAsync_WithNullContent_ThrowsNullReferenceException()
        {
            // Arrange
            CustomFormFile formFile = new CustomFormFile();
            formFile.Content = null;
            MemoryStream targetStream = new MemoryStream();

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(() => formFile.CopyToAsync(targetStream));
        }

        [Fact]
        public void OpenReadStream_WithNullContent_ThrowsArgumentNullException()
        {
            // Arrange
            CustomFormFile formFile = new CustomFormFile();
            formFile.Content = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => formFile.OpenReadStream());
        }

        #endregion

        #region Binary Content Tests

        [Fact]
        public void Constructor_WithBinaryContent_PreservesContent()
        {
            // Arrange
            byte[] binaryContent = new byte[] { 0x00, 0xFF, 0x7F, 0x80, 0x01, 0xFE };
            string fileName = "binary.dat";

            // Act
            CustomFormFile formFile = new CustomFormFile(fileName, binaryContent);

            // Assert
            Assert.Equal(binaryContent, formFile.Content);
        }

        [Fact]
        public void CopyTo_WithBinaryContent_PreservesContent()
        {
            // Arrange
            byte[] binaryContent = new byte[] { 0x00, 0xFF, 0x7F, 0x80, 0x01, 0xFE };
            CustomFormFile formFile = new CustomFormFile("binary.dat", binaryContent);
            MemoryStream targetStream = new MemoryStream();

            // Act
            formFile.CopyTo(targetStream);

            // Assert
            Assert.Equal(binaryContent, targetStream.ToArray());
        }

        #endregion
    }
}
