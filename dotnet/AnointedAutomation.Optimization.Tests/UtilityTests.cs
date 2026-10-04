// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Copyright 2026 Anointed Automation, LLC. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using AnointedAutomation.Optimization;
using Xunit;

namespace AnointedAutomation.Optimization.Tests
{
    public class UtilityTests
    {
        public class Person
        {
            public int Id { get; set; }
            public string? Name { get; set; }
            public DateTime When { get; set; }
        }

        [Fact]
        public void EncodeDecode_RoundTrips()
        {
            string encoded = Utility.EncodeString("hello world");
            Assert.True(Utility.IsBase64String(encoded));
            Assert.Equal("hello world", Utility.DecodeString(encoded));
        }

        [Fact]
        public void GenerateString_RespectsLength_AndRandomWhenNonPositive()
        {
            Assert.Equal(12, Utility.GenerateString(12).Length);
            int random = Utility.GenerateString(0).Length;
            Assert.InRange(random, 7, 254);
        }

        [Fact]
        public void ToChunks_SplitsIntoBoundedGroups()
        {
            List<int> source = Enumerable.Range(1, 10).ToList();
            List<List<int>> chunks = source.ToChunks(3).Select(c => c.ToList()).ToList();
            Assert.Equal(4, chunks.Count);
            Assert.Equal(new List<int> { 1, 2, 3 }, chunks[0]);
            Assert.Equal(new List<int> { 10 }, chunks[3]);
        }

        [Fact]
        public void ToJaggedArray_MirrorsTwoDimensionalArray()
        {
            int[,] grid = { { 1, 2 }, { 3, 4 } };
            int[][] jagged = grid.ToJaggedArray();
            Assert.Equal(2, jagged.Length);
            Assert.Equal(new[] { 3, 4 }, jagged[1]);
        }

        [Fact]
        public void IfStringContainsOneOf_MatchesAcrossOverloads()
        {
            Assert.True(Utility.IfStringContainsOneOf("the cat sat", new[] { "dog", "cat" }));
            Assert.False(Utility.IfStringContainsOneOf("the cat sat", new[] { "dog", "fish" }));
            Assert.True(Utility.IfStringContainsOneOf("the cat sat", "dog,cat", ','));
            Assert.True(Utility.IfStringContainsOneOf("the cat sat", "dog|cat", "|"));
            Assert.False(Utility.IfStringContainsOneOf("the cat sat", string.Empty, ','));
        }

        [Fact]
        public void WhereContains_HonoursIgnoreCase()
        {
            List<string> subs = new List<string> { "CAT", "dog" };
            Assert.Equal(new List<string> { "CAT" }, Utility.WhereContains("the cat", subs, true).ToList());
            Assert.Empty(Utility.WhereContains("the cat", subs, false));
            Assert.Empty(Utility.WhereContains("the cat", (List<string>?)null, true));
        }

        [Fact]
        public void GetReadableTimeString_Formats()
        {
            Assert.Equal("1h:2m:3s", Utility.GetReadableTimeString(new TimeSpan(1, 2, 3)));
        }

        [Fact]
        public void ConvertListToDataTable_And_Back()
        {
            List<Person> people = new List<Person>
            {
                new Person { Id = 1, Name = "Ann", When = new DateTime(2026, 1, 1) },
                new Person { Id = 2, Name = "Bob", When = new DateTime(2026, 2, 2) },
            };

            DataTable table = Utility.ConvertListToDataTable(people);
            Assert.Equal(3, table.Columns.Count);
            Assert.Equal(2, table.Rows.Count);

            List<Person> back = Utility.ConvertDataTableToList<Person>(table);
            Assert.Equal(2, back.Count);
            Assert.Equal("Bob", back[1].Name);
            Assert.Equal(2, back[1].Id);
        }

        [Fact]
        public void GetObjectFromDataReader_MapsCaseInsensitiveColumns()
        {
            List<object?[]> rows = new List<object?[]>
            {
                new object?[] { 5, "Cy", new DateTime(2026, 3, 3) },
            };
            ChunkDataReader reader = new ChunkDataReader(
                new[] { "id", "name", "when" },
                new[] { typeof(int), typeof(string), typeof(DateTime) },
                rows);

            List<Person> people = Utility.ConvertDataReaderToList<Person>(reader);
            Assert.Single(people);
            Assert.Equal(5, people[0].Id);
            Assert.Equal("Cy", people[0].Name);
        }

        [Fact]
        public void Csv_RoundTripsThroughDataTable()
        {
            List<Person> people = new List<Person>
            {
                new Person { Id = 1, Name = "Ann" },
                new Person { Id = 2, Name = "Bob" },
            };
            DataTable table = Utility.ConvertListToDataTable(people);

            string path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".csv");
            try
            {
                Utility.CreateCsvFromDataTable(table, path, ",");
                DataTable read = Utility.ConvertCSVtoDataTable(path, ',');
                Assert.Equal(3, read.Columns.Count);
                Assert.Equal(2, read.Rows.Count);
                Assert.Equal("Ann", read.Rows[0]["Name"]);
            }
            finally
            {
                File.Delete(path);
            }
        }

        [Fact]
        public void ConvertDataTableToHTML_DefaultsBackgroundColor()
        {
            DataTable table = new DataTable();
            table.Columns.Add("A");
            table.Rows.Add("x");
            string html = Utility.ConvertDataTableToHTML(table, 0, 0, 0, null);
            Assert.Contains("bgcolor=#ffffff", html);
            Assert.Contains("<td>x</td>", html);
        }

        [Fact]
        public void RecursiveDelete_RemovesTree()
        {
            string root = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(Path.Combine(root, "sub"));
            File.WriteAllText(Path.Combine(root, "sub", "f.txt"), "data");

            Utility.RecursiveDelete(new DirectoryInfo(root));
            Assert.False(Directory.Exists(root));
        }
    }
}
