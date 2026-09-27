// Copyright 2026 Anointed Automation, LLC. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me
using System.Collections.Generic;
using System.Text.Json;
using Xunit;

namespace AnointedAutomation.Objects.API.Tests
{
    /// <summary>
    /// Unit tests for PageResponse.
    /// </summary>
    public class PageResponseTests
    {
        [Fact]
        public void Default_ItemsEmptyNotNull()
        {
            PageResponse<string> page = new PageResponse<string>();

            Assert.NotNull(page.Items);
            Assert.Empty(page.Items);
        }

        [Fact]
        public void Constructor_NullItems_BecomesEmpty()
        {
            PageResponse<string> page = new PageResponse<string>(null, 5, 2, 10);

            Assert.Empty(page.Items);
            Assert.Equal(5, page.Total);
            Assert.Equal(2, page.Page);
            Assert.Equal(10, page.PageSize);
        }

        [Fact]
        public void Serialize_UsesAnointedCasing()
        {
            PageResponse<string> page = new PageResponse<string>(new List<string> { "a" }, 42, 1, 25);

            string json = JsonSerializer.Serialize(page, JsonCasingConvention.Options);

            Assert.Equal("{\"Items\":[\"a\"],\"total\":42,\"page\":1,\"pageSize\":25}", json);
        }
    }
}
