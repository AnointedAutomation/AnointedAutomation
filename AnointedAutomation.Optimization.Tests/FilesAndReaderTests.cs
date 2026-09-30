// Copyright 2026 Anointed Automation, LLC. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.IO;
using AnointedAutomation.Optimization;
using Xunit;

namespace AnointedAutomation.Optimization.Tests
{
    public class ChunkDataReaderTests
    {
        private static ChunkDataReader BuildReader() => new ChunkDataReader(
            new[] { "id", "name", "flag" },
            new[] { typeof(int), typeof(string), typeof(bool) },
            new List<object?[]>
            {
                new object?[] { 1, "a", true },
                new object?[] { 2, null, false },
            });

        [Fact]
        public void Read_IteratesRows()
        {
            ChunkDataReader reader = BuildReader();
            Assert.Equal(3, reader.FieldCount);
            Assert.True(reader.Read());
            Assert.Equal(1, reader.GetInt32(0));
            Assert.Equal("a", reader.GetString(1));
            Assert.True(reader.GetBoolean(2));
            Assert.True(reader.Read());
            Assert.True(reader.IsDBNull(1));
            Assert.False(reader.Read());
        }

        [Fact]
        public void GetOrdinal_IsCaseInsensitive()
        {
            ChunkDataReader reader = BuildReader();
            Assert.Equal(1, reader.GetOrdinal("NAME"));
            Assert.Equal(-1, reader.GetOrdinal("nope"));
        }

        [Fact]
        public void Indexers_AndValues()
        {
            ChunkDataReader reader = BuildReader();
            reader.Read();
            Assert.Equal(1, reader["id"]);
            Assert.Equal("a", reader[1]);
            object[] values = new object[reader.FieldCount];
            Assert.Equal(3, reader.GetValues(values));
        }
    }

    public class FileManagementTests
    {
        [Fact]
        public void CreateAndDelete_Directory_AndFile()
        {
            string dir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            FileManagement.CreateDirectory(dir);
            Assert.True(Directory.Exists(dir));

            string file = Path.Combine(dir, "a.txt");
            FileManagement.CreateFile(file);
            Assert.True(File.Exists(file));

            FileManagement.DeleteFile(file);
            Assert.False(File.Exists(file));

            FileManagement.DeleteDirectory(dir, true);
            Assert.False(Directory.Exists(dir));
        }

        [Fact]
        public void DeleteTry_ReturnsNullOnSuccess_AndNoThrowOnMissing()
        {
            Assert.Null(FileManagement.DeleteFileTry(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName())));
            Assert.Null(FileManagement.DeleteDirectoryTry(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName()), true));
        }

        [Fact]
        public void LineChanger_ResizesWhenBeyondLength()
        {
            string[] arr = { "one", "two" };
            string[] result = FileManagement.LineChanger(arr, "five", 4);
            Assert.Equal(5, result.Length);
            Assert.Equal("five", result[4]);
        }
    }

    public class FileComparatorTests
    {
        [Fact]
        public void CompareFiles_TrueWhenDiffer_FalseWhenSame()
        {
            string a = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            string b = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            try
            {
                File.WriteAllText(a, "same");
                File.WriteAllText(b, "same");
                Assert.False(FileComparator.CompareFiles(a, b));

                File.WriteAllText(b, "different");
                Assert.True(FileComparator.CompareFiles(a, b));
            }
            finally
            {
                File.Delete(a);
                File.Delete(b);
            }
        }

        [Fact]
        public void ReplaceFile_OverwritesTarget()
        {
            string a = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            string b = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            try
            {
                File.WriteAllText(a, "old");
                File.WriteAllText(b, "new");
                FileComparator.ReplaceFile(a, b);
                Assert.Equal("new", File.ReadAllText(a));
            }
            finally
            {
                File.Delete(a);
                File.Delete(b);
            }
        }

        [Fact]
        public void CompareFiles_MissingFile_Throws()
        {
            Assert.Throws<FileNotFoundException>(() =>
                FileComparator.CompareFiles(
                    Path.Combine(Path.GetTempPath(), Path.GetRandomFileName()),
                    Path.Combine(Path.GetTempPath(), Path.GetRandomFileName())));
        }
    }
}
