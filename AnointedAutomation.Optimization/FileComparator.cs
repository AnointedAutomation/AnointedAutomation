// Copyright 2024 Anointed Automation, LLC All rights reserved.
// Originally created by Alexander Fields. Ported and modernized for the AnointedAutomation monorepo.

using System.IO;
using System.Linq;

namespace AnointedAutomation.Optimization
{
    /// <summary>Byte-for-byte file comparison and replacement helpers.</summary>
    public static class FileComparator
    {
        /// <summary>
        /// Compares two files byte for byte. Preserves the original library's return convention:
        /// returns <c>true</c> when the files DIFFER and <c>false</c> when they are identical.
        /// </summary>
        /// <param name="oldFile">Path to the existing file.</param>
        /// <param name="newFile">Path to the candidate file.</param>
        /// <returns><c>true</c> when the two files differ; otherwise <c>false</c>.</returns>
        /// <exception cref="FileNotFoundException">Thrown when either file does not exist.</exception>
        public static bool CompareFiles(string oldFile, string newFile)
        {
            if (!File.Exists(oldFile) || !File.Exists(newFile))
            {
                throw new FileNotFoundException("One or both files do not exist.");
            }

            byte[] oldFileBytes = File.ReadAllBytes(oldFile);
            byte[] newFileBytes = File.ReadAllBytes(newFile);

            return !oldFileBytes.SequenceEqual(newFileBytes);
        }

        /// <summary>Overwrites <paramref name="oldFile"/> with the contents of <paramref name="newFile"/>.</summary>
        /// <exception cref="FileNotFoundException">Thrown when either file does not exist.</exception>
        public static void ReplaceFile(string oldFile, string newFile)
        {
            if (!File.Exists(oldFile) || !File.Exists(newFile))
            {
                throw new FileNotFoundException("One or both files do not exist.");
            }

            File.Copy(newFile, oldFile, true);
        }
    }
}
