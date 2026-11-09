using System;
using System.Collections.Generic;
using System.Data;
using DevExpress.XtraReports.UI;
using DevExpress.Drawing;
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

        public MultiColumnSplitReport(MultiColumnReportData reportData, MultiColumnPrintSettings settings) : this(reportData)
        {
            ApplyPrintSettings(settings);
        }

        private void ApplyPrintSettings(MultiColumnPrintSettings settings)
        {
            Landscape = settings.PaperOrientation == "landscape";
            PaperKind = DevExpress.Drawing.Printing.DXPaperKind.A4;
            var contentWidth = Landscape ? 1089f : 746.77f;

            lblTitle.Font = CreateFont(settings.TitleFont);
            lblSub.Font = CreateFont(settings.SubtitleFont);
            lblTitle.WidthF = contentWidth;
            lblSub.WidthF = contentWidth;
            lblTitle.WordWrap = true;
            lblTitle.CanGrow = false;
            lblTitle.HeightF = 50;
            lblSub.LocationFloat = new DevExpress.Utils.PointFloat(0, 56);
            ReportHeader.HeightF = 85;

            foreach (var table in new[] { tblHeader1, tblHeader2, tblHeader3 })
            {
                table.WidthF = contentWidth;
                table.Font = CreateFont(settings.HeaderFont);
                foreach (XRTableCell cell in table.Rows[0].Cells) cell.Font = CreateFont(settings.HeaderFont);
            }
            foreach (var table in new[] { tblDetail1, tblDetail2, tblDetail3 })
            {
                table.WidthF = contentWidth;
                table.Font = CreateFont(settings.DetailFont);
                foreach (XRTableCell cell in table.Rows[0].Cells) cell.Font = CreateFont(settings.DetailFont);
            }
            pageInfoNumber.LocationFloat = new DevExpress.Utils.PointFloat(contentWidth - 400, 10);

            if (settings.RepeatTitleOnEveryPage)
            {
                ReportHeader.Controls.Remove(lblTitle);
                ReportHeader.Controls.Remove(lblSub);
                ReportHeader.HeightF = 0;
                var pageHeader = new PageHeaderBand { HeightF = 85 };
                pageHeader.Controls.AddRange(new XRControl[] { lblTitle, lblSub });
                Bands.Add(pageHeader);
            }

            if (settings.ShowSigner)
            {
                var footer = new ReportFooterBand { HeightF = 80 };
                var title = new XRLabel
                {
                    Text = settings.SignerTitle,
                    Font = CreateFont(settings.HeaderFont),
                    BoundsF = new System.Drawing.RectangleF(contentWidth - 230, 0, 230, 24),
                    TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter
                };
                var name = new XRLabel
                {
                    Text = settings.SignerName,
                    Font = CreateFont(settings.DetailFont),
                    BoundsF = new System.Drawing.RectangleF(contentWidth - 230, 52, 230, 24),
                    TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter
                };
                footer.Controls.AddRange(new XRControl[] { title, name });
                Bands.Add(footer);
            }
        }

        private static DXFont CreateFont(PrintFontSettings settings)
        {
            var style = DXFontStyle.Regular;
            if (settings.Bold) style |= DXFontStyle.Bold;
            if (settings.Italic) style |= DXFontStyle.Italic;
            return new DXFont(settings.Family, settings.Size, style);
        }

        private void InitializeReport(MultiColumnReportData reportData)
        {
            if (reportData == null) return;

            // Cập nhật phụ đề báo cáo
            if (lblSub != null)
            {
                lblSub.Text = $"Tổng số cột: {reportData.TotalColumns} | Số cột/trang in: {reportData.ColumnsPerPage} | Cột STT cố định đầu dòng";
            }

            // Gán dữ liệu trực tiếp vào các DetailReportBand đã thiết kế sẵn trên DevExpress Designer
            // Không xóa band hay can thiệp giao diện bằng code C#
            BindDesignerBatches(reportData);
        }

        /// <summary>
        /// Gán dữ liệu vào các Band đã vẽ trên DevExpress Designer.
        /// Toàn bộ định dạng (Font, màu sắc, border, độ rộng, vị trí) đều do DevExpress Designer quản lý.
        /// </summary>
        private void BindDesignerBatches(MultiColumnReportData reportData)
        {
            var reports = new[] { this.DetailReport1, this.DetailReport2, this.DetailReport3 };
            var batchColumns = new[]
            {
                new[] { "Cột 1", "Cột 2", "Cột 3", "Cột 4", "Cột 5", "Cột 6" },
                new[] { "Cột 7", "Cột 8", "Cột 9", "Cột 10", "Cột 11", "Cột 12" },
                new[] { "Cột 13", "Cột 14", "Cột 15", "Cột 16", "Cột 17", "Cột 18" }
            };

            for (int b = 0; b < reports.Length; b++)
            {
                if (reports[b] == null) continue;

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
    }
}
