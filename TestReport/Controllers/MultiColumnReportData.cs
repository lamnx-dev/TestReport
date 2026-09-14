using System.Collections.Generic;

namespace TestReport.Controllers
{
    public class MultiColumnReportData
    {
        public int TotalColumns { get; set; } = 18;
        public int ColumnsPerPage { get; set; } = 6;
        public List<string> ColumnNames { get; set; } = new();
        public List<MultiColumnRow> Rows { get; set; } = new();
    }

    public class MultiColumnRow
    {
        public int Stt { get; set; }
        public Dictionary<string, string> Values { get; set; } = new();
    }
}
