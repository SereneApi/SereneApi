using System;
using System.Collections.Generic;

namespace DeltaWare.SereneApi.Types.DataTable
{
    public class DataTableOptions
    {
        private readonly Dictionary<string, string> _sortStringOverrideMap = new Dictionary<string, string>();

        public void AddSortStringOverride(string sortString, string sortOverride)
        {
            if (_sortStringOverrideMap.ContainsKey(sortString))
            {
                throw new ArgumentException("Duplicate Override", sortOverride);
            }

            _sortStringOverrideMap.Add(sortString, sortOverride);
        }

        internal string GetSortString(string sortString)
        {
            if (_sortStringOverrideMap.TryGetValue(sortString, out string sortOverride))
            {
                return sortOverride;
            }

            return sortString;
        }
    }
}
