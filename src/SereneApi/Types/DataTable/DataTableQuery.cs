namespace DeltaWare.SereneApi.Types.DataTable
{
    public class DataTableQuery
    {
        public bool SortDescending { get; set; }

        public int ItemIndex { get; set; }

        public int PageSize { get; set; }

        public string SearchString { get; set; }

        public string SortString { get; set; }
    }
}
