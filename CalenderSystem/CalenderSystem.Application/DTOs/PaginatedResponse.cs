using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalenderSystem.Application.DTOs
{
    public class PaginatedResponse<T>
    {
        public List<T> Data { get; set; }
        public int Page { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public PaginatedResponse(List<T> data, int page, int totalPages, int pageSize, int totalCount)
        {
            Data = data;
            Page = page;
            TotalPages = totalPages;
            PageSize = pageSize;
            TotalCount = totalCount;
        }
    }
}
