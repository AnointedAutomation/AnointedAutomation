// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Copyright 2024 Anointed Automation Corporation. Open source, provided "as is" without warranty (see LICENSE).
// Originally created by Alexander Fields. Ported and modernized for the AnointedAutomation monorepo.

using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using AnointedAutomation.Optimization.Logging;

namespace AnointedAutomation.Optimization
{
    /// <summary>File and directory helpers that are safe to call idempotently.</summary>
    public static class FileManagement
    {
        /// <summary>Creates the directory if it does not already exist.</summary>
        public static void CreateDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        /// <summary>Creates the file if it does not already exist and closes the handle.</summary>
        public static void CreateFile(string path)
        {
            if (!File.Exists(path))
            {
                using FileStream file = File.Create(path);
            }
        }

        /// <summary>
        /// Replaces a line in a file represented as a string array, resizing the array if the target
        /// line is beyond its current length.
        /// </summary>
        /// <param name="fileAsArray">The file contents as a string array.</param>
        /// <param name="newText">The replacement line.</param>
        /// <param name="lineToEdit">The zero-based line index to edit.</param>
        /// <returns>The edited array.</returns>
        public static string[] LineChanger(string[] fileAsArray, string newText, long lineToEdit)
        {
            if (fileAsArray.LongLength <= lineToEdit)
            {
                Array.Resize(ref fileAsArray, (int)lineToEdit + 1);
            }

            fileAsArray[lineToEdit] = newText;
            return fileAsArray;
        }

        /// <summary>Deletes a directory if it exists.</summary>
        /// <param name="path">The directory path.</param>
        /// <param name="deleteContents">When true, deletes files and subdirectories too.</param>
        public static void DeleteDirectory(string path, bool deleteContents)
        {
            if (!Directory.Exists(path))
            {
                return;
            }

            if (deleteContents)
            {
                Directory.Delete(path, true);
            }
            else
            {
                Directory.Delete(path);
            }
        }

        /// <summary>Deletes a directory if it exists, returning any exception instead of throwing. A failure is also logged as a Warning.</summary>
        /// <returns>The exception that was raised, or null on success.</returns>
        public static Exception? DeleteDirectoryTry(string path, bool deleteContents)
        {
            try
            {
                DeleteDirectory(path, deleteContents);
            }
            catch (Exception e)
            {
                LogMessage.Warning($"FileManagement.DeleteDirectoryTry: could not delete directory '{path}' (deleteContents={deleteContents})", e);
                return e;
            }
            return null;
        }

        /// <summary>Deletes a file if it exists.</summary>
        public static void DeleteFile(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        /// <summary>Deletes a file if it exists, returning any exception instead of throwing. A failure is also logged as a Warning.</summary>
        /// <returns>The exception that was raised, or null on success.</returns>
        public static Exception? DeleteFileTry(string path)
        {
            try
            {
                DeleteFile(path);
            }
            catch (Exception e)
            {
                LogMessage.Warning($"FileManagement.DeleteFileTry: could not delete file '{path}'", e);
                return e;
            }
            return null;
        }

        private static readonly HttpClient SharedHttpClient = new HttpClient();

        /// <summary>
        /// Downloads the content at <paramref name="hyperlink"/> to <paramref name="filePath"/> using a shared
        /// <see cref="HttpClient"/>. See <see cref="GetFileFromInternetAsync(HttpClient, string, string)"/>.
        /// </summary>
        /// <param name="hyperlink">The URL to download.</param>
        /// <param name="filePath">The file to write.</param>
        /// <exception cref="HttpRequestException">The server answered with a non-success status code; no file is written.</exception>
        public static Task GetFileFromInternetAsync(string hyperlink, string filePath)
        {
            return GetFileFromInternetAsync(SharedHttpClient, hyperlink, filePath);
        }

        /// <summary>
        /// Downloads the content at <paramref name="hyperlink"/> and streams the raw bytes to <paramref name="filePath"/>.
        /// A non-success status throws before any file is created, and the bytes are never decoded, so binary files stay intact.
        /// A partially written file is deleted when the download fails.
        /// </summary>
        /// <param name="client">The caller-owned client (carries the proxy, timeout and handler the host wants).</param>
        /// <param name="hyperlink">The URL to download.</param>
        /// <param name="filePath">The file to write.</param>
        /// <exception cref="ArgumentNullException"><paramref name="client"/> is null.</exception>
        /// <exception cref="HttpRequestException">The server answered with a non-success status code.</exception>
        public static async Task GetFileFromInternetAsync(HttpClient client, string hyperlink, string filePath)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            using HttpResponseMessage response = await client.GetAsync(hyperlink, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            try
            {
                using Stream source = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                using FileStream destination = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
                await source.CopyToAsync(destination).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                LogMessage.Error($"FileManagement.GetFileFromInternetAsync: download of '{hyperlink}' to '{filePath}' failed", e);
                DeleteFileTry(filePath);
                throw;
            }
        }
    }
}
