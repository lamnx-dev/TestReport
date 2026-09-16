using System.Collections.Generic;
using System.Text;
using TestReport.Api.Domain.Requests;

namespace TestReport.Api.Infrastructure.SelectProviders
{
    /// <summary>
    /// Chịu trách nhiệm sinh câu truy vấn SQL động tối ưu cho Oracle Database
    /// Hỗ trợ dynamic columns và phân trang bằng OFFSET ... FETCH NEXT ROWS ONLY (Oracle 12c+)
    /// </summary>
    public class MultiColumnSelectProvider
    {
        public string BuildOracleSelectQuery(MultiColumnReportRequest request)
        {
            var sb = new StringBuilder();
            sb.AppendLine("SELECT");
            sb.AppendLine("    STT,");

            var colList = new List<string>();
            for (int i = 1; i <= request.TotalColumns; i++)
            {
                colList.Add($"    COL_{i} AS \"Cột {i}\"");
            }
            sb.AppendLine(string.Join(",\n", colList));

            sb.AppendLine("FROM V_REPORT_MULTI_COLUMN_DATA");
            sb.AppendLine("ORDER BY STT ASC");
            sb.AppendLine($"OFFSET 0 ROWS FETCH NEXT {request.RowCount} ROWS ONLY");

            return sb.ToString();
        }
    }
}

