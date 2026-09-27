// Copyright © Anointed Automation, Ltd., 2025. All Rights Reserved.

// =============================================================================
// NAMING CONVENTION:
// This codebase follows a specific property naming pattern:
//   - Value types (structs): lowercase (e.g., bool success, int statusCode)
//   - Reference types (objects): PascalCase (e.g., string Message, object Data)
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
        /// The current page number (1-based)
        /// </summary>
        public int currentPage { get; set; }

        /// <summary>
        /// The data items for this page
        /// </summary>
        public List<T> Data { get; set; }

        /// <summary>
        /// Whether there is a next page available
        /// </summary>
        public bool hasNextPage => currentPage < totalPages;

        /// <summary>
        /// Whether there is a previous page available
        /// </summary>
        public bool hasPreviousPage => currentPage > 1;

        /// <summary>
        /// The number of items per page
        /// </summary>
        public int pageSize { get; set; }

        /// <summary>
        /// The total number of items across all pages
        /// </summary>
        public long totalItems { get; set; }

        /// <summary>
        /// The total number of pages
        /// </summary>
        public int totalPages { get; set; }


        /// <summary>
        /// Creates a new paginated response
        /// </summary>
        public PaginatedResponse()
        {
            Data = new List<T>();
        }

        /// <summary>
        /// Creates a new paginated response with data
        /// </summary>
        /// <param name="data">The data items for this page</param>
        /// <param name="currentPage">The current page number</param>
        /// <param name="pageSize">The number of items per page</param>
        /// <param name="totalItems">The total number of items</param>
        public PaginatedResponse(List<T> data, int currentPage, int pageSize, long totalItems)
        {
            Data = data ?? new List<T>();
            this.currentPage = currentPage;
            this.pageSize = pageSize;
            this.totalItems = totalItems;
            this.totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        }
    }
}
