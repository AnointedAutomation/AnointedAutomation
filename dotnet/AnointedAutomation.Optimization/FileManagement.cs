// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Copyright 2024 Anointed Automation, LLC All rights reserved.
// Originally created by Alexander Fields. Ported and modernized for the AnointedAutomation monorepo.

using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

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

        /// <summary>Deletes a directory if it exists, returning any exception instead of throwing.</summary>
        /// <returns>The exception that was raised, or null on success.</returns>
        public static Exception? DeleteDirectoryTry(string path, bool deleteContents)
        {
            try
            {
                DeleteDirectory(path, deleteContents);
            }
            catch (Exception e)
            {
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

        /// <summary>Deletes a file if it exists, returning any exception instead of throwing.</summary>
        /// <returns>The exception that was raised, or null on success.</returns>
        public static Exception? DeleteFileTry(string path)
        {
            try
            {
                DeleteFile(path);
            }
            catch (Exception e)
            {
                return e;
            }
            return null;
        }

        /// <summary>Downloads the content at <paramref name="hyperlink"/> and writes it to <paramref name="filePath"/>.</summary>
        public static async Task GetFileFromInternetAsync(string hyperlink, string filePath)
        {
            using HttpClient client = new HttpClient();
            using HttpResponseMessage response = await client.GetAsync(hyperlink).ConfigureAwait(false);
            using HttpContent content = response.Content;
            using Stream stream = await content.ReadAsStreamAsync().ConfigureAwait(false);
            using StreamReader reader = new StreamReader(stream);
            string contentStr = await reader.ReadToEndAsync().ConfigureAwait(false);
            await File.WriteAllTextAsync(filePath, contentStr).ConfigureAwait(false);
        }
    }
}
