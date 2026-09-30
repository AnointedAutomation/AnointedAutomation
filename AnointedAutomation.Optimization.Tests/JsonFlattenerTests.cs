// Copyright 2026 Anointed Automation, LLC. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Data;
using AnointedAutomation.Optimization;
using Xunit;

namespace AnointedAutomation.Optimization.Tests
{
    public class JsonFlattenerTests
    {
        [Fact]
        public void FlattenJson_NestedObject_UnderscoreKeys()
        {
            Dictionary<string, object?> flat = JsonFlattener.FlattenJson(
                "{\"a\":1,\"b\":{\"c\":\"x\",\"d\":true}}", false, false);

            Assert.Equal(1L, flat["a"]);
            Assert.Equal("x", flat["b_c"]);
            Assert.Equal(true, flat["b_d"]);
            Assert.True(flat.ContainsKey("b")); // parent object stored as raw json
        }

        [Fact]
        public void FlattenJson_ChangeCase_Upper()
        {
            Dictionary<string, object?> flat = JsonFlattener.FlattenJson(
                "{\"name\":\"widget\"}", true, true);
            Assert.Equal("widget", flat["NAME"]);
        }

        [Fact]
        public void FlattenJson_Array_FlattensFirstElement()
        {
            Dictionary<string, object?> flat = JsonFlattener.FlattenJson(
                "[{\"id\":1},{\"id\":2}]", false, false);
            Assert.Equal(1L, flat["[0]_id"]);
        }

        [Fact]
        public void FlattenJson_InvalidJson_Throws()
        {
            Assert.Throws<ArgumentException>(() => JsonFlattener.FlattenJson("plain text", false, false));
        }

        [Fact]
        public void DictionaryToDataTable_SingleRow()
        {
            Dictionary<string, object?> dict = new Dictionary<string, object?>
            {
                { "id", 5 },
                { "name", "widget" },
                { "missing", null },
            };
            DataTable table = JsonFlattener.DictionaryToDataTable(dict);
            Assert.Equal(3, table.Columns.Count);
            Assert.Single(table.Rows);
            Assert.Equal(5, table.Rows[0]["id"]);
            Assert.Equal(DBNull.Value, table.Rows[0]["missing"]);
        }
    }
}
