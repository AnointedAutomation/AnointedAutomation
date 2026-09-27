// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me
//
// One page of a paged list. Replaces the per-endpoint "XListResponse { Items, Total, Page, PageSize }" copies.
// Wire shape under JsonCasingConvention: {"Items":[...],"total":n,"page":n,"pageSize":n}.

using System.Collections.Generic;

namespace AnointedAutomation.Objects.API
{
    /// <summary>
    /// One page of results plus the numbers a client needs to page through the rest.
    /// </summary>
    /// <typeparam name="T">The item type.</typeparam>
    public class PageResponse<T>
    {
        /// <summary>The items on this page.</summary>
        public IReadOnlyList<T> Items { get; set; } = new List<T>();

        /// <summary>How many items match in total, across every page.</summary>
        public long Total { get; set; }

        /// <summary>The 1-based page number of this page.</summary>
        public int Page { get; set; }

        /// <summary>The page size that was requested.</summary>
        public int PageSize { get; set; }

        /// <summary>Creates an empty page.</summary>
        public PageResponse()
        {
        }

        /// <summary>Creates a page from its items and paging numbers.</summary>
        /// <param name="items">The items on this page.</param>
        /// <param name="total">How many items match in total.</param>
        /// <param name="page">The 1-based page number.</param>
        /// <param name="pageSize">The requested page size.</param>
        public PageResponse(IReadOnlyList<T> items, long total, int page, int pageSize)
        {
            Items = items ?? new List<T>();
            Total = total;
            Page = page;
            PageSize = pageSize;
        }
    }
}
