using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestReport.Api.Application.Interfaces;
using TestReport.Api.Domain.Models;
using TestReport.Api.Domain.Requests;
using TestReport.Api.Infrastructure.DbMappings;
using TestReport.Api.Infrastructure.SelectProviders;

namespace TestReport.Api.Infrastructure.Repositories
{
    /// <summary>
    /// Repository truy xuất dữ liệu báo cáo nhiều cột từ Oracle Database
    /// Hiện tại: Giả lập dữ liệu thô trả về từ Oracle thông qua SelectProvider & DbMapping
    /// Khi gắn kết nối thật: Chỉ cần mở OracleConnection và thực thi câu SQL do SelectProvider sinh ra
    /// </summary>
    public class MultiColumnReportRepository : IMultiColumnReportRepository
    {
        private readonly MultiColumnSelectProvider _selectProvider;
        private readonly MultiColumnDbMapping _dbMapping;

        public MultiColumnReportRepository(
            MultiColumnSelectProvider selectProvider,
            MultiColumnDbMapping dbMapping)
        {
            _selectProvider = selectProvider;
            _dbMapping = dbMapping;
        }

        public MultiColumnReportData GetReportData(MultiColumnReportRequest request)
        {
            int totalColumns = Math.Clamp(request.TotalColumns, 2, 50);
            int columnsPerPage = Math.Clamp(request.ColumnsPerPage, 2, 12);
            int rowCount = Math.Clamp(request.RowCount, 1, 1000);

            // 1. Sinh câu truy vấn Oracle thông qua SelectProvider
            string oracleQuery = _selectProvider.BuildOracleSelectQuery(new MultiColumnReportRequest
            {
                TotalColumns = totalColumns,
                ColumnsPerPage = columnsPerPage,
                RowCount = rowCount
            });

            // 2. Dữ liệu thô hardcode giả lập kết quả trả về từ Oracle Database
            var rawDbRows = GenerateSimulatedOracleRows(totalColumns, rowCount);

            // 3. Ánh xạ dữ liệu thô sang Domain Model thông qua DbMapping
            return _dbMapping.MapToDomain(totalColumns, columnsPerPage, rawDbRows);
        }

        public Task<MultiColumnReportData> GetReportDataAsync(MultiColumnReportRequest request)
        {
            return Task.FromResult(GetReportData(request));
        }

        /// <summary>
        /// Giả lập dữ liệu thô dưới dạng IDictionary đại diện cho các row trả về từ Oracle DataReader
        /// </summary>
        private static List<Dictionary<string, object>> GenerateSimulatedOracleRows(int totalColumns, int rowCount)
        {
            var rows = new List<Dictionary<string, object>>(rowCount);

            for (int r = 1; r <= rowCount; r++)
            {
                var row = new Dictionary<string, object>
                {
                    { "STT", r }
                };

                for (int c = 1; c <= totalColumns; c++)
                {
                    int val = (r * 10) + c * 3 + Random.Shared.Next(10, 99);
                    row[$"Cột {c}"] = val.ToString("#,##0");
                }

                rows.Add(row);
            }

            return rows;
        }
    }
}

