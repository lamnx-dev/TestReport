using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using DevExpress.Drawing;
using DevExpress.Drawing.Printing;
using DevExpress.XtraReports.UI;

namespace TestReport.Controllers
{
    public class MultiColumnSplitReport : XtraReport
    {
        public MultiColumnSplitReport(MultiColumnReportData reportData)
        {
            InitializeReport(reportData);
        }

        private void InitializeReport(MultiColumnReportData reportData)
        {
            // Thiết lập trang in A4 Landscape để báo cáo nhiều cột có không gian rộng rãi
            this.PaperKind = DXPaperKind.A4;
            this.Landscape = true;
            this.PageWidth = 1169;
            this.PageHeight = 827;
            this.Margins = new DXMargins(40F, 40F, 40F, 40F);
            this.Font = new DXFont("Arial", 9.5F);

            // Report Header tổng thể
            var reportHeader = new ReportHeaderBand
            {
                HeightF = 75F
            };

            var lblTitle = new XRLabel
            {
                Text = "BÁO CÁO CHI TIẾT (TỰ ĐỘNG PHÂN TÁCH TRANG IN THEO CỘT)",
                Font = new DXFont("Arial", 14F, DXFontStyle.Bold),
                TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(30, 41, 59),
                BoundsF = new RectangleF(0, 5, 1089, 30)
            };

            var lblSub = new XRLabel
            {
                Text = $"Tổng số cột: {reportData.TotalColumns} | Số cột/trang in: {reportData.ColumnsPerPage} | Cột STT cố định đầu dòng (tiêu đề trống)",
                Font = new DXFont("Arial", 9.5F, DXFontStyle.Italic),
                TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(100, 116, 139),
                BoundsF = new RectangleF(0, 38, 1089, 20)
            };

            reportHeader.Controls.Add(lblTitle);
            reportHeader.Controls.Add(lblSub);
            this.Bands.Add(reportHeader);

            // Bắt buộc phải có 1 DetailBand trong root report của DevExpress XtraReport
            this.Bands.Add(new DetailBand { HeightF = 0 });


            // Chia danh sách cột thành các batch kích thước ColumnsPerPage
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

                // Tạo DataTable cho batch này để DevExpress binding tự nhiên và phân trang chuẩn
                var dt = new DataTable($"Batch_{batchIdx + 1}");
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
                    // Ngắt trang sau mỗi phân đoạn cột (trừ phân đoạn cuối cùng)
                    PageBreak = isLastBatch ? PageBreak.None : PageBreak.AfterBand
                };

                // GroupHeaderBand: chứa tiêu đề phân đoạn và Header Table
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
                    BoundsF = new RectangleF(0, 5, printableWidth, 20),
                    TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft
                };
                groupHeader.Controls.Add(lblBatchInfo);

                // Header Table
                float remainWidth = printableWidth - sttWidth;
                float dataColWidth = remainWidth / batch.Count;

                var tblHeader = new XRTable
                {
                    BoundsF = new RectangleF(0, 30, printableWidth, 32F),
                    Borders = DevExpress.XtraPrinting.BorderSide.All,
                    BorderColor = Color.FromArgb(203, 213, 225),
                    BorderWidth = 1F,
                    BackColor = Color.FromArgb(241, 245, 249),
                    Font = new DXFont("Arial", 9F, DXFontStyle.Bold),
                    TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter
                };

                var rowHeader = new XRTableRow { HeightF = 32F };

                // CỘT STT: TIÊU ĐỀ TRỐNG theo đúng yêu cầu
                var cellSttHeader = new XRTableCell
                {
                    Text = string.Empty, // Để trống tiêu đề
                    WidthF = sttWidth,
                    BackColor = Color.FromArgb(226, 232, 240)
                };
                rowHeader.Cells.Add(cellSttHeader);

                // CÁC CỘT DỮ LIỆU: Cột 1, Cột 2, ...
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

                // DetailBand: Dòng dữ liệu
                var detailBand = new DetailBand
                {
                    HeightF = 26F
                };

                var tblDetail = new XRTable
                {
                    BoundsF = new RectangleF(0, 0, printableWidth, 26F),
                    Borders = DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Bottom,
                    BorderColor = Color.FromArgb(226, 232, 240),
                    BorderWidth = 1F,
                    Font = new DXFont("Arial", 9F, DXFontStyle.Regular)
                };

                var rowDetail = new XRTableRow { HeightF = 26F };

                // Ô dữ liệu STT (1, 2, 3...)
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

                // Các ô dữ liệu theo cột
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

            // PageFooter: Hiển thị thời gian và số trang in
            var pageFooter = new PageFooterBand
            {
                HeightF = 35F
            };

            var pageInfoTime = new XRPageInfo
            {
                PageInfo = DevExpress.XtraPrinting.PageInfo.DateTime,
                TextFormatString = "Thời gian tạo: {0:dd/MM/yyyy HH:mm}",
                BoundsF = new RectangleF(0, 10, 400, 20),
                Font = new DXFont("Arial", 8.5F, DXFontStyle.Italic),
                ForeColor = Color.FromArgb(148, 163, 184)
            };

            var pageInfoNumber = new XRPageInfo
            {
                PageInfo = DevExpress.XtraPrinting.PageInfo.NumberOfTotal,
                TextFormatString = "Trang {0} / {1}",
                BoundsF = new RectangleF(689, 10, 400, 20),
                TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight,
                Font = new DXFont("Arial", 8.5F, DXFontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139)
            };

            pageFooter.Controls.Add(pageInfoTime);
            pageFooter.Controls.Add(pageInfoNumber);
            this.Bands.Add(pageFooter);
        }
    }
}
