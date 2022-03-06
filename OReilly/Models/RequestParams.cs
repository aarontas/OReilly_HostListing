using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OReilly.Models
{
    /// <summary>
    /// We want to limited the request to the database. If we want all hotels, maybe several request at same time can collapse the database.
    /// we can limited with pages and data return by page.
    /// </summary>
    public class RequestParams
    {
        const int maxPageSize = 50;
        public int PageNumber { get; set; } = 1;
        private int _pageSize = 10;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > maxPageSize ? maxPageSize : value;
        }
    }
}
