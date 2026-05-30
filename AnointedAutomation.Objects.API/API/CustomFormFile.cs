// Copyright © Anointed Automation, LLC., 2024. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2024-01-16 19:19:01
// Edited by Alexander Fields https://www.alexanderfields.me 2025-07-02 11:48:25
//Stewarded by Alexander Fields

using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AnointedAutomation.Objects.API
{
    /// <summary>
    /// Custom implementation of IFormFile for handling file uploads in ASP.NET Core.
    /// Provides in-memory storage of file content and metadata.
    /// </summary>
    public class CustomFormFile : IFormFile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomFormFile"/> class.
        /// </summary>
        public CustomFormFile()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomFormFile"/> class with a filename and content.
        /// </summary>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="content">The file content as a byte array.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if fileName is null.</exception>
        public CustomFormFile(string fileName, byte[] content)
        {
            FileName = fileName ?? throw new System.ArgumentNullException(nameof(fileName));
            Content = content ?? new byte[0];
        }

        /// <summary>
        /// Gets or sets the file content as a byte array.
        /// </summary>
        public byte[] Content
        {
            get; set;
        }

        /// <summary>
        /// Gets the content disposition header for the file.
        /// </summary>
        public string ContentDisposition => $"form-data; name=\"file\"; filename=\"{FileName}\"";
        /// <summary>
        /// Gets the content type of the file (defaults to application/octet-stream).
        /// </summary>
        public string ContentType => "application/octet-stream";

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        public string FileName
        {
            get; set;
        }

        /// <summary>
        /// Gets the HTTP headers associated with the file.
        /// </summary>
        public IHeaderDictionary Headers => new HeaderDictionary();

        /// <summary>
        /// Gets the length of the file content in bytes.
        /// </summary>
        public long Length
        {
            get => Content.Length;
        }

        /// <summary>
        /// Gets the form field name for the file upload (always "file").
        /// </summary>
        public string Name => "file";

        /// <summary>
        /// Copies the file content to the specified stream.
        /// </summary>
        /// <param name="target">The stream to copy the content to.</param>
        public void CopyTo(Stream target)
        {
            target.Write(Content, 0, Content.Length);
        }

        /// <summary>
        /// Asynchronously copies the file content to the specified stream.
        /// </summary>
        /// <param name="target">The stream to copy the content to.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task CopyToAsync(Stream target, CancellationToken cancellationToken = default)
        {
            return target.WriteAsync(Content, 0, Content.Length, cancellationToken);
        }

        /// <summary>
        /// Opens a read-only stream containing the file content.
        /// </summary>
        /// <returns>A stream containing the file content.</returns>
        public Stream OpenReadStream()
        {
            return new MemoryStream(Content);
        }
    }
}
