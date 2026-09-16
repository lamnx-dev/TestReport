namespace TestReport.Api.Domain.Requests
{
    public class MultiColumnReportRequest
    {
        public int TotalColumns { get; set; } = 18;
        public int ColumnsPerPage { get; set; } = 6;
        public int RowCount { get; set; } = 100;
    }
}

