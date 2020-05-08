using DeltaWare.SereneApi.Types.DataTable;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Linq;

namespace DeltaWare.SereneApi.Helpers
{
    public static class DataTableHelper
    {
        public static DataTableQuery GetQuery(HttpRequest request, Action<DataTableOptions> options = null)
        {
            DataTableOptions option = new DataTableOptions();

            options?.Invoke(option);

            IQueryCollection query = request.Query;

            if (!query.TryGetValue("start", out StringValues startValues))
            {
                throw new ArgumentException("Could not Find", "start");
            }

            if (!query.TryGetValue("length", out StringValues lengthValues))
            {
                throw new ArgumentException("Could not Find", "length");
            }

            if (!query.TryGetValue("search[value]", out StringValues searchValues))
            {
                throw new ArgumentException("Could not Find", "search[value]");
            }

            if (!query.TryGetValue("order[0][column]", out StringValues sortIndexValues))
            {
                throw new ArgumentException("Could not Find", "order[0][column]");
            }

            if (!query.TryGetValue("order[0][dir]", out StringValues sortDirectionValues))
            {
                throw new ArgumentException("Could not Find", "order[0][dir]");
            }

            int itemIndex = Convert.ToInt32(startValues.First());
            int pageSize = Convert.ToInt32(lengthValues.First());
            int sortIndex = Convert.ToInt32(sortIndexValues.First());

            string searchString = searchValues.First().ToLower();
            string sortDirection = sortDirectionValues.First();

            if (!query.TryGetValue($"columns[{sortIndex}][data]", out StringValues sortPropertyNameValues))
            {
                throw new ArgumentException("Could not Find", $"columns[{sortIndex}][data]");
            }

            string sortString = option.GetSortString(sortPropertyNameValues.First());

            return new DataTableQuery
            {
                SortDescending = sortDirection != "asc",
                ItemIndex = itemIndex,
                PageSize = pageSize,
                SearchString = searchString,
                SortString = sortString
            };
        }

        public static int GetDrawCounter(HttpRequest request)
        {
            if (!request.Query.TryGetValue("draw", out StringValues drawValues))
            {
                throw new ArgumentException("Could not Find", "draw");
            }

            return Convert.ToInt32(drawValues.First());
        }
    }
}
