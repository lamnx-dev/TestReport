using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using DevExpress.Drawing;
using DevExpress.Drawing.Printing;
using DevExpress.XtraReports.UI;
using TestReport.Api.Domain.Models;

namespace TestReport.Api.Infrastructure.Reports
{
    public partial class MultiColumnSplitReport : XtraReport
    {
        public MultiColumnSplitReport()
        {
            InitializeComponent();
        }

        public MultiColumnSplitReport(MultiColumnReportData reportData) : this()
        {
            InitializeReport(reportData);
        }

        private void InitializeReport(MultiColumnReportData reportData)
        {
            if (reportData == null) return;

            // Cập nhật phụ đề với số liệu thực tế
            lblSub.Text = $"Tổng số cột: {reportData.TotalColumns} | Số cột/trang in: {reportData.ColumnsPerPage} | Cột STT cố định đầu dòng (tiêu đề trống)";

            // Nếu cấu hình dữ liệu khớp với thiết kế 18 cột (6 cột/trang) trên Designer:
            // Sử dụng trực tiếp 3 DetailReportBand đã thiết kế trên Visual Studio Designer!
            if (reportData.TotalColumns == 18 && reportData.ColumnsPerPage == 6 &&
                reportData.ColumnNames.Count == 18 &&
                this.DetailReport1 != null && this.DetailReport2 != null && this.DetailReport3 != null)
            {
                BindStaticBatches(reportData);
            }
            else
            {
                // Cấu hình cột tùy biến: gỡ bỏ các DetailReport tĩnh và sinh batch động
                this.Bands.Remove(this.DetailReport1);
                this.Bands.Remove(this.DetailReport2);
                this.Bands.Remove(this.DetailReport3);

                BuildDynamicBatches(reportData);
            }
        }

        /// <summary>
        /// Gán dữ liệu trực tiếp vào 3 DetailReportBand đã vẽ sẵn trên file Designer
        /// </summary>
        private void BindStaticBatches(MultiColumnReportData reportData)
        {
            var reports = new[] { this.DetailReport1, this.DetailReport2, this.DetailReport3 };
            var batchColumns = new[]
            {
                new[] { "Cột 1", "Cột 2", "Cột 3", "Cột 4", "Cột 5", "Cột 6" },
                new[] { "Cột 7", "Cột 8", "Cột 9", "Cột 10", "Cột 11", "Cột 12" },
                new[] { "Cột 13", "Cột 14", "Cột 15", "Cột 16", "Cột 17", "Cột 18" }
            };

            for (int b = 0; b < 3; b++)
            {
                var dt = new DataTable($"Batch_{b + 1}");
                dt.Columns.Add("Stt", typeof(int));
                foreach (var col in batchColumns[b])
                {
                    dt.Columns.Add(col, typeof(string));
                }

                foreach (var r in reportData.Rows)
                {
                    var dr = dt.NewRow();
                    dr["Stt"] = r.Stt;
                    foreach (var col in batchColumns[b])
                    {
                        dr[col] = r.Values.TryGetValue(col, out var val) ? val : string.Empty;
                    }
                    dt.Rows.Add(dr);
                }

                reports[b].DataSource = dt;
                reports[b].DataMember = dt.TableName;
            }
        }

        /// <summary>
        /// Sinh các DetailReportBand linh hoạt cho bất kỳ số lượng cột nào
        /// </summary>
        private void BuildDynamicBatches(MultiColumnReportData reportData)
        {
            var columnBatches = new List<List<string>>();
            for (int i = 0; i < reportData.ColumnNames.Count; i += reportData.ColumnsPerPage)
            {
                int count = Math.Min(reportData.ColumnsPerPage, reportData.ColumnNames.Count - i);
                columnBatches.Add(reportData.ColumnNames.GetRange(i, count));
            }

            int printableWidth = 1169 - 80; // 1089px
            float sttWidth = 65F; // Chiều rộng cột STT

            for (int batchIdx = 0; batchIdx < columnBatches.Count; batchIdx++)
            {
                var batch = columnBatches[batchIdx];
                bool isLastBatch = (batchIdx == columnBatches.Count - 1);

                var dt = new DataTable($"DynamicBatch_{batchIdx + 1}");
                dt.Columns.Add("Stt", typeof(int));
                foreach (var colName in batch)
                {
                    dt.Columns.Add(colName, typeof(string));
                }

                foreach (var r in reportData.Rows)
                {
                    var dr = dt.NewRow();
                    dr["Stt"] = r.Stt;
                    foreach (var colName in batch)
                    {
                        dr[colName] = r.Values.TryGetValue(colName, out var val) ? val : string.Empty;
                    }
                    dt.Rows.Add(dr);
                }

                var detailReport = new DetailReportBand
                {
                    DataSource = dt,
                    DataMember = dt.TableName,
                    PageBreak = isLastBatch ? PageBreak.None : PageBreak.AfterBand
                };

                var groupHeader = new GroupHeaderBand
                {
                    HeightF = 65F,
                    RepeatEveryPage = true,
                    GroupUnion = GroupUnion.WithFirstDetail
                };

                var lblBatchInfo = new XRLabel
                {
                    Text = $"► PHẦN IN {batchIdx + 1}: {batch[0]} ĐẾN {batch[^1]} (Kèm cột STT đối chiếu)",
                    Font = new DXFont("Arial", 9F, DXFontStyle.Bold),
                    ForeColor = Color.FromArgb(37, 99, 235),
                    LocationFloat = new DevExpress.Utils.PointFloat(0F, 5F),
                    SizeF = new SizeF(printableWidth, 20F),
                    TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft
                };
                groupHeader.Controls.Add(lblBatchInfo);

                float remainWidth = printableWidth - sttWidth;
                float dataColWidth = remainWidth / batch.Count;

                var tblHeader = new XRTable
                {
                    LocationFloat = new DevExpress.Utils.PointFloat(0F, 30F),
                    SizeF = new SizeF(printableWidth, 32F),
                    Borders = DevExpress.XtraPrinting.BorderSide.All,
                    BorderColor = Color.FromArgb(203, 213, 225),
                    BorderWidth = 1F,
                    BackColor = Color.FromArgb(241, 245, 249),
                    Font = new DXFont("Arial", 9F, DXFontStyle.Bold),
                    TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter
                };

                var rowHeader = new XRTableRow { HeightF = 32F };
                var cellSttHeader = new XRTableCell
                {
                    Text = string.Empty,
                    WidthF = sttWidth,
                    BackColor = Color.FromArgb(226, 232, 240)
                };
                rowHeader.Cells.Add(cellSttHeader);

                foreach (var colName in batch)
                {
                    var cellCol = new XRTableCell
                    {
                        Text = colName,
                        WidthF = dataColWidth,
                        ForeColor = Color.FromArgb(15, 23, 42)
                    };
                    rowHeader.Cells.Add(cellCol);
                }

                tblHeader.Rows.Add(rowHeader);
                groupHeader.Controls.Add(tblHeader);
                detailReport.Bands.Add(groupHeader);

                var detailBand = new DetailBand
                {
                    HeightF = 26F
                };

                var tblDetail = new XRTable
                {
                    LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F),
                    SizeF = new SizeF(printableWidth, 26F),
                    Borders = DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Bottom,
                    BorderColor = Color.FromArgb(226, 232, 240),
                    BorderWidth = 1F,
                    Font = new DXFont("Arial", 9F, DXFontStyle.Regular)
                };

                var rowDetail = new XRTableRow { HeightF = 26F };
                var cellSttDetail = new XRTableCell
                {
                    WidthF = sttWidth,
                    TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter,
                    Font = new DXFont("Arial", 9F, DXFontStyle.Bold),
                    ForeColor = Color.FromArgb(71, 85, 105),
                    BackColor = Color.FromArgb(248, 250, 252)
                };
                cellSttDetail.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "[Stt]"));
                rowDetail.Cells.Add(cellSttDetail);

                foreach (var colName in batch)
                {
                    var cellData = new XRTableCell
                    {
                        WidthF = dataColWidth,
                        TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter,
                        ForeColor = Color.FromArgb(30, 41, 59)
                    };
                    cellData.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", $"[{colName}]"));
                    rowDetail.Cells.Add(cellData);
                }

                tblDetail.Rows.Add(rowDetail);
                detailBand.Controls.Add(tblDetail);
                detailReport.Bands.Add(detailBand);

                this.Bands.Add(detailReport);
            }
        }
    }
}

