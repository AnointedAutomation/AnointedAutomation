// Copyright 2026 Anointed Automation, LLC. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me
using System.Collections.Generic;
using Xunit;

namespace AnointedAutomation.Objects.Tests
{
    /// <summary>
    /// Unit tests for PaginatedResponse.
    /// </summary>
    public class PaginatedResponseTests
    {
        [Fact]
        public void Default_DataEmptyNotNull()
        {
            PaginatedResponse<string> page = new PaginatedResponse<string>();

            Assert.NotNull(page.Data);
            Assert.Empty(page.Data);
        }

        [Fact]
        public void Constructor_ComputesTotalPagesAndNavigation()
        {
            PaginatedResponse<string> page = new PaginatedResponse<string>(new List<string> { "a" }, 2, 20, 41);

            Assert.Equal(3, page.TotalPages);
            Assert.True(page.HasNextPage);
            Assert.True(page.HasPreviousPage);
        }

        [Fact]
        public void Constructor_ZeroPageSize_TotalPagesZeroInsteadOfOverflow()
        {
            PaginatedResponse<string> page = new PaginatedResponse<string>(new List<string>(), 1, 0, 5);

            Assert.Equal(0, page.TotalPages);
            Assert.False(page.HasNextPage);
        }

        [Fact]
        public void Constructor_NullData_BecomesEmpty()
        {
            PaginatedResponse<string> page = new PaginatedResponse<string>(null, 1, 10, 0);

            Assert.Empty(page.Data);
            Assert.False(page.HasPreviousPage);
        }
    }
}
