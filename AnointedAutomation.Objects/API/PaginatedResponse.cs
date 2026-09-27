// Copyright © Anointed Automation, Ltd., 2025. All Rights Reserved.

// =============================================================================
// NAMING: C# properties are PascalCase. The wire casing (value types camelCase,
// reference types PascalCase) comes from JsonCasingConvention, not the C# names.
// =============================================================================

using System;
using System.Collections.Generic;

namespace AnointedAutomation.Objects
{
    /// <summary>
    /// Represents a paginated response from an API
    /// </summary>
    /// <typeparam name="T">The type of data being paginated</typeparam>
    public class PaginatedResponse<T>
    {
        /// <summary>
        /// The data items for this page
        /// </summary>
        public IReadOnlyList<T> Data { get; set; }

        /// <summary>
        /// The current page number (1-based)
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// The number of items per page
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// The total number of items across all pages
        /// </summary>
        public long TotalItems { get; set; }

        /// <summary>
        /// The total number of pages (0 when the page size is 0)
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// Whether there is a next page available
        /// </summary>
        public bool HasNextPage => CurrentPage < TotalPages;

        /// <summary>
        /// Whether there is a previous page available
        /// </summary>
        public bool HasPreviousPage => CurrentPage > 1;

        /// <summary>
        /// Creates an empty paginated response
        /// </summary>
        public PaginatedResponse()
        {
            Data = new List<T>();
        }

        /// <summary>
        /// Creates a new paginated response with data; TotalPages is computed from the counts
        /// </summary>
        /// <param name="data">The data items for this page</param>
        /// <param name="currentPage">The current page number</param>
        /// <param name="pageSize">The number of items per page</param>
        /// <param name="totalItems">The total number of items</param>
        public PaginatedResponse(IReadOnlyList<T> data, int currentPage, int pageSize, long totalItems)
        {
            Data = data ?? new List<T>();
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalItems = totalItems;
            TotalPages = pageSize > 0 ? (int)Math.Ceiling(totalItems / (double)pageSize) : 0;
        }
    }
}
