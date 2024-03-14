using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.RequestFeatures
{
    public abstract class RequestParameters
    {
        private const int maxPageSize = 50;
        private int _pageSize = 10;

        public int PageNumber { get; set; } = 1;
        public string? OrderBy { get; set; }

        public string? Fields { get; set; }

        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = (value > maxPageSize) ? maxPageSize : value;
            }
        }
    }
}
