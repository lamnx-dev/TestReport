using System.Collections.Generic;
using TestReport.Api.Domain.Models;

namespace TestReport.Api.Infrastructure.DbMappings
{
    /// <summary>
    /// Chịu trách nhiệm ánh xạ (mapping) dữ liệu thô từ Oracle Database (Dictionary/DataReader/DataTable)
    /// sang đối tượng Domain Model MultiColumnReportData
    /// </summary>
    public class MultiColumnDbMapping
    {
        public MultiColumnReportData MapToDomain(
            int totalColumns,
            int columnsPerPage,
            IEnumerable<IDictionary<string, object>> rawDbRows)
        {
            var reportData = new MultiColumnReportData
            {
                TotalColumns = totalColumns,
                ColumnsPerPage = columnsPerPage
            };

            for (int i = 1; i <= totalColumns; i++)
            {
                reportData.ColumnNames.Add($"Cột {i}");
            }

            foreach (var dbRow in rawDbRows)
            {
                var row = new MultiColumnRow();

                if (dbRow.TryGetValue("STT", out var sttVal) && int.TryParse(sttVal?.ToString(), out int stt))
                {
                    row.Stt = stt;
                }

                for (int c = 1; c <= totalColumns; c++)
                {
                    string colKey = $"Cột {c}";
                    if (dbRow.TryGetValue(colKey, out var val))
                    {
                        row.Values[colKey] = val?.ToString() ?? string.Empty;
                    }
                    else if (dbRow.TryGetValue($"COL_{c}", out var colVal))
                    {
                        row.Values[colKey] = colVal?.ToString() ?? string.Empty;
                    }
                }

                reportData.Rows.Add(row);
            }

            return reportData;
        }
    }
}

