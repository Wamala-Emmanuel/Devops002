using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GatewayService.DTOs
{
    /// <summary>
    /// The details of pagination of the requests
    /// </summary>
    public class SearchPagination
    {
        const int maxPageSize = 100;

        /// <summary>
        /// Page number
        /// </summary>
        public int Page { get; set; } = 1;

        private int _itemsPerPage = 50;

        /// <summary>
        /// Number of requests returned per page
        /// </summary>
        public int ItemsPerPage
        {
            get => _itemsPerPage;

            set => _itemsPerPage = (value > maxPageSize) ? maxPageSize : value;
        }

        /// <summary>
        /// Total number of requests
        /// </summary>
        public int TotalItems { get; set; }

    }
}
