namespace TestReport.Controllers
{
    partial class Report1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Designer generated code

        private void InitializeComponent()
        {
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.ReportHeader = new DevExpress.XtraReports.UI.ReportHeaderBand();
            this.PageHeader = new DevExpress.XtraReports.UI.PageHeaderBand();
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.ReportFooter = new DevExpress.XtraReports.UI.ReportFooterBand();
            this.PageFooter = new DevExpress.XtraReports.UI.PageFooterBand();

            this.lblTitle = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSubtitle = new DevExpress.XtraReports.UI.XRLabel();
            this.tblHeader = new DevExpress.XtraReports.UI.XRTable();
            this.rowHeader = new DevExpress.XtraReports.UI.XRTableRow();
            this.colHeaderIndex = new DevExpress.XtraReports.UI.XRTableCell();
            this.colHeaderDate = new DevExpress.XtraReports.UI.XRTableCell();
            this.colHeaderTempC = new DevExpress.XtraReports.UI.XRTableCell();
            this.colHeaderTempF = new DevExpress.XtraReports.UI.XRTableCell();
            this.colHeaderSummary = new DevExpress.XtraReports.UI.XRTableCell();

            this.tblDetail = new DevExpress.XtraReports.UI.XRTable();
            this.rowDetail = new DevExpress.XtraReports.UI.XRTableRow();
            this.colDetailIndex = new DevExpress.XtraReports.UI.XRTableCell();
            this.colDetailDate = new DevExpress.XtraReports.UI.XRTableCell();
            this.colDetailTempC = new DevExpress.XtraReports.UI.XRTableCell();
            this.colDetailTempF = new DevExpress.XtraReports.UI.XRTableCell();
            this.colDetailSummary = new DevExpress.XtraReports.UI.XRTableCell();

            this.lblDateFooter = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSigner1 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSigner2 = new DevExpress.XtraReports.UI.XRLabel();
            this.pageInfoTime = new DevExpress.XtraReports.UI.XRPageInfo();
            this.pageInfoNumber = new DevExpress.XtraReports.UI.XRPageInfo();

            ((System.ComponentModel.ISupportInitialize)(this.tblHeader)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tblDetail)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();

            // 
            // TopMargin
            // 
            this.TopMargin.HeightF = 50F;
            this.TopMargin.Name = "TopMargin";

            // 
            // BottomMargin
            // 
            this.BottomMargin.HeightF = 50F;
            this.BottomMargin.Name = "BottomMargin";

            // 
            // ReportHeader
            // 
            this.ReportHeader.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
                this.lblTitle,
                this.lblSubtitle
            });
            this.ReportHeader.HeightF = 85F;
            this.ReportHeader.Name = "ReportHeader";

            // 
            // lblTitle
            // 
            this.lblTitle.Font = new DevExpress.Drawing.DXFont("Arial", 16F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(24, 43, 73);
            this.lblTitle.LocationFloat = new DevExpress.Utils.PointFloat(0F, 10F);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.lblTitle.SizeF = new System.Drawing.SizeF(727F, 35F);
            this.lblTitle.StylePriority.UseFont = false;
            this.lblTitle.StylePriority.UseForeColor = false;
            this.lblTitle.StylePriority.UseTextAlignment = false;
            this.lblTitle.Text = "BÁO CÁO THEO DÕI THỜI TIẾT";
            this.lblTitle.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;

            // 
            // lblSubtitle
            // 
            this.lblSubtitle.Font = new DevExpress.Drawing.DXFont("Arial", 10F, DevExpress.Drawing.DXFontStyle.Italic);
            this.lblSubtitle.ForeColor = System.Drawing.Color.DimGray;
            this.lblSubtitle.LocationFloat = new DevExpress.Utils.PointFloat(0F, 48F);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.lblSubtitle.SizeF = new System.Drawing.SizeF(727F, 22F);
            this.lblSubtitle.StylePriority.UseFont = false;
            this.lblSubtitle.StylePriority.UseForeColor = false;
            this.lblSubtitle.StylePriority.UseTextAlignment = false;
            this.lblSubtitle.Text = "Hệ thống trạm quan trắc tự động | DevExpress Reporting";
            this.lblSubtitle.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;

            // 
            // PageHeader
            // 
            this.PageHeader.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
                this.tblHeader
            });
            this.PageHeader.HeightF = 35F;
            this.PageHeader.Name = "PageHeader";

            // 
            // tblHeader
            // 
            this.tblHeader.BackColor = System.Drawing.Color.FromArgb(41, 128, 185);
            this.tblHeader.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.tblHeader.BorderColor = System.Drawing.Color.White;
            this.tblHeader.Font = new DevExpress.Drawing.DXFont("Arial", 9.75F, DevExpress.Drawing.DXFontStyle.Bold);
            this.tblHeader.ForeColor = System.Drawing.Color.White;
            this.tblHeader.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.tblHeader.Name = "tblHeader";
            this.tblHeader.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
                this.rowHeader
            });
            this.tblHeader.SizeF = new System.Drawing.SizeF(727F, 35F);
            this.tblHeader.StylePriority.UseBackColor = false;
            this.tblHeader.StylePriority.UseBorders = false;
            this.tblHeader.StylePriority.UseBorderColor = false;
            this.tblHeader.StylePriority.UseFont = false;
            this.tblHeader.StylePriority.UseForeColor = false;
            this.tblHeader.StylePriority.UseTextAlignment = false;
            this.tblHeader.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;

            // 
            // rowHeader
            // 
            this.rowHeader.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
                this.colHeaderIndex,
                this.colHeaderDate,
                this.colHeaderTempC,
                this.colHeaderTempF,
                this.colHeaderSummary
            });
            this.rowHeader.Name = "rowHeader";
            this.rowHeader.Weight = 1D;

            // 
            // colHeaderIndex
            // 
            this.colHeaderIndex.Name = "colHeaderIndex";
            this.colHeaderIndex.Text = "STT";
            this.colHeaderIndex.Weight = 0.45D;

            // 
            // colHeaderDate
            // 
            this.colHeaderDate.Name = "colHeaderDate";
            this.colHeaderDate.Text = "Ngày Ghi Nhận";
            this.colHeaderDate.Weight = 1.2D;

            // 
            // colHeaderTempC
            // 
            this.colHeaderTempC.Name = "colHeaderTempC";
            this.colHeaderTempC.Text = "Nhiệt Độ (°C)";
            this.colHeaderTempC.Weight = 1.1D;

            // 
            // colHeaderTempF
            // 
            this.colHeaderTempF.Name = "colHeaderTempF";
            this.colHeaderTempF.Text = "Nhiệt Độ (°F)";
            this.colHeaderTempF.Weight = 1.1D;

            // 
            // colHeaderSummary
            // 
            this.colHeaderSummary.Name = "colHeaderSummary";
            this.colHeaderSummary.Text = "Tình Trạng Thời Tiết";
            this.colHeaderSummary.Weight = 3.42D;

            // 
            // Detail
            // 
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
                this.tblDetail
            });
            this.Detail.HeightF = 28F;
            this.Detail.Name = "Detail";

            // 
            // tblDetail
            // 
            this.tblDetail.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.tblDetail.BorderColor = System.Drawing.Color.LightGray;
            this.tblDetail.Font = new DevExpress.Drawing.DXFont("Arial", 9.75F);
            this.tblDetail.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.tblDetail.Name = "tblDetail";
            this.tblDetail.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
                this.rowDetail
            });
            this.tblDetail.SizeF = new System.Drawing.SizeF(727F, 28F);
            this.tblDetail.StylePriority.UseBorders = false;
            this.tblDetail.StylePriority.UseBorderColor = false;
            this.tblDetail.StylePriority.UseFont = false;

            // 
            // rowDetail
            // 
            this.rowDetail.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
                this.colDetailIndex,
                this.colDetailDate,
                this.colDetailTempC,
                this.colDetailTempF,
                this.colDetailSummary
            });
            this.rowDetail.Name = "rowDetail";
            this.rowDetail.Weight = 11.5D;

            // 
            // colDetailIndex
            // 
            this.colDetailIndex.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "sumRecordNumber()")
            });
            this.colDetailIndex.Name = "colDetailIndex";
            this.colDetailIndex.StylePriority.UseTextAlignment = false;
            this.colDetailIndex.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.colDetailIndex.Weight = 0.45D;

            // 
            // colDetailDate
            // 
            this.colDetailDate.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Date]")
            });
            this.colDetailDate.Name = "colDetailDate";
            this.colDetailDate.StylePriority.UseTextAlignment = false;
            this.colDetailDate.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.colDetailDate.TextFormatString = "{0:dd/MM/yyyy}";
            this.colDetailDate.Weight = 1.2D;

            // 
            // colDetailTempC
            // 
            this.colDetailTempC.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[TemperatureC]")
            });
            this.colDetailTempC.Name = "colDetailTempC";
            this.colDetailTempC.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 8, 0, 0, 100F);
            this.colDetailTempC.StylePriority.UsePadding = false;
            this.colDetailTempC.StylePriority.UseTextAlignment = false;
            this.colDetailTempC.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.colDetailTempC.TextFormatString = "{0} °C";
            this.colDetailTempC.Weight = 1.1D;

            // 
            // colDetailTempF
            // 
            this.colDetailTempF.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[TemperatureF]")
            });
            this.colDetailTempF.Name = "colDetailTempF";
            this.colDetailTempF.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 8, 0, 0, 100F);
            this.colDetailTempF.StylePriority.UsePadding = false;
            this.colDetailTempF.StylePriority.UseTextAlignment = false;
            this.colDetailTempF.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.colDetailTempF.TextFormatString = "{0} °F";
            this.colDetailTempF.Weight = 1.1D;

            // 
            // colDetailSummary
            // 
            this.colDetailSummary.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Summary]")
            });
            this.colDetailSummary.Name = "colDetailSummary";
            this.colDetailSummary.Padding = new DevExpress.XtraPrinting.PaddingInfo(8, 0, 0, 0, 100F);
            this.colDetailSummary.StylePriority.UsePadding = false;
            this.colDetailSummary.StylePriority.UseTextAlignment = false;
            this.colDetailSummary.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.colDetailSummary.Weight = 3.42D;

            // 
            // ReportFooter
            // 
            this.ReportFooter.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
                this.lblDateFooter,
                this.lblSigner1,
                this.lblSigner2
            });
            this.ReportFooter.HeightF = 130F;
            this.ReportFooter.Name = "ReportFooter";

            // 
            // lblDateFooter
            // 
            this.lblDateFooter.Font = new DevExpress.Drawing.DXFont("Arial", 9.5F, DevExpress.Drawing.DXFontStyle.Italic);
            this.lblDateFooter.LocationFloat = new DevExpress.Utils.PointFloat(427F, 15F);
            this.lblDateFooter.Name = "lblDateFooter";
            this.lblDateFooter.SizeF = new System.Drawing.SizeF(300F, 22F);
            this.lblDateFooter.StylePriority.UseFont = false;
            this.lblDateFooter.StylePriority.UseTextAlignment = false;
            this.lblDateFooter.Text = "Hà Nội, ngày ..... tháng ..... năm 2026";
            this.lblDateFooter.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;

            // 
            // lblSigner1
            // 
            this.lblSigner1.Font = new DevExpress.Drawing.DXFont("Arial", 9.75F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblSigner1.LocationFloat = new DevExpress.Utils.PointFloat(30F, 45F);
            this.lblSigner1.Multiline = true;
            this.lblSigner1.Name = "lblSigner1";
            this.lblSigner1.SizeF = new System.Drawing.SizeF(250F, 40F);
            this.lblSigner1.StylePriority.UseFont = false;
            this.lblSigner1.StylePriority.UseTextAlignment = false;
            this.lblSigner1.Text = "NGƯỜI LẬP BÁO CÁO\r\n(Ký, ghi rõ họ tên)";
            this.lblSigner1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;

            // 
            // lblSigner2
            // 
            this.lblSigner2.Font = new DevExpress.Drawing.DXFont("Arial", 9.75F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblSigner2.LocationFloat = new DevExpress.Utils.PointFloat(450F, 45F);
            this.lblSigner2.Multiline = true;
            this.lblSigner2.Name = "lblSigner2";
            this.lblSigner2.SizeF = new System.Drawing.SizeF(250F, 40F);
            this.lblSigner2.StylePriority.UseFont = false;
            this.lblSigner2.StylePriority.UseTextAlignment = false;
            this.lblSigner2.Text = "TRƯỞNG PHÒNG QUẢN LÝ\r\n(Ký, đóng dấu)";
            this.lblSigner2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;

            // 
            // PageFooter
            // 
            this.PageFooter.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
                this.pageInfoTime,
                this.pageInfoNumber
            });
            this.PageFooter.HeightF = 30F;
            this.PageFooter.Name = "PageFooter";

            // 
            // pageInfoTime
            // 
            this.pageInfoTime.Font = new DevExpress.Drawing.DXFont("Arial", 8.5F, DevExpress.Drawing.DXFontStyle.Italic);
            this.pageInfoTime.ForeColor = System.Drawing.Color.Gray;
            this.pageInfoTime.LocationFloat = new DevExpress.Utils.PointFloat(0F, 5F);
            this.pageInfoTime.Name = "pageInfoTime";
            this.pageInfoTime.PageInfo = DevExpress.XtraPrinting.PageInfo.DateTime;
            this.pageInfoTime.SizeF = new System.Drawing.SizeF(300F, 20F);
            this.pageInfoTime.StylePriority.UseFont = false;
            this.pageInfoTime.StylePriority.UseForeColor = false;
            this.pageInfoTime.StylePriority.UseTextAlignment = false;
            this.pageInfoTime.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.pageInfoTime.TextFormatString = "Thời gian in: {0:dd/MM/yyyy HH:mm:ss}";

            // 
            // pageInfoNumber
            // 
            this.pageInfoNumber.Font = new DevExpress.Drawing.DXFont("Arial", 8.5F);
            this.pageInfoNumber.ForeColor = System.Drawing.Color.Gray;
            this.pageInfoNumber.LocationFloat = new DevExpress.Utils.PointFloat(527F, 5F);
            this.pageInfoNumber.Name = "pageInfoNumber";
            this.pageInfoNumber.SizeF = new System.Drawing.SizeF(200F, 20F);
            this.pageInfoNumber.StylePriority.UseFont = false;
            this.pageInfoNumber.StylePriority.UseForeColor = false;
            this.pageInfoNumber.StylePriority.UseTextAlignment = false;
            this.pageInfoNumber.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.pageInfoNumber.TextFormatString = "Trang {0}/{1}";

            // 
            // Report1
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
                this.TopMargin,
                this.ReportHeader,
                this.PageHeader,
                this.Detail,
                this.ReportFooter,
                this.PageFooter,
                this.BottomMargin
            });
            this.Font = new DevExpress.Drawing.DXFont("Arial", 9.75F);
            this.Margins = new DevExpress.Drawing.DXMargins(50F, 50F, 50F, 50F);
            this.PageHeight = 1169;
            this.PageWidth = 827;
            this.PaperKind = DevExpress.Drawing.Printing.DXPaperKind.A4;
            this.Version = "25.2";
            ((System.ComponentModel.ISupportInitialize)(this.tblHeader)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tblDetail)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
        }

        #endregion

        private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
        private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
        private DevExpress.XtraReports.UI.ReportHeaderBand ReportHeader;
        private DevExpress.XtraReports.UI.PageHeaderBand PageHeader;
        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.ReportFooterBand ReportFooter;
        private DevExpress.XtraReports.UI.PageFooterBand PageFooter;

        private DevExpress.XtraReports.UI.XRLabel lblTitle;
        private DevExpress.XtraReports.UI.XRLabel lblSubtitle;
        private DevExpress.XtraReports.UI.XRTable tblHeader;
        private DevExpress.XtraReports.UI.XRTableRow rowHeader;
        private DevExpress.XtraReports.UI.XRTableCell colHeaderIndex;
        private DevExpress.XtraReports.UI.XRTableCell colHeaderDate;
        private DevExpress.XtraReports.UI.XRTableCell colHeaderTempC;
        private DevExpress.XtraReports.UI.XRTableCell colHeaderTempF;
        private DevExpress.XtraReports.UI.XRTableCell colHeaderSummary;

        private DevExpress.XtraReports.UI.XRTable tblDetail;
        private DevExpress.XtraReports.UI.XRTableRow rowDetail;
        private DevExpress.XtraReports.UI.XRTableCell colDetailIndex;
        private DevExpress.XtraReports.UI.XRTableCell colDetailDate;
        private DevExpress.XtraReports.UI.XRTableCell colDetailTempC;
        private DevExpress.XtraReports.UI.XRTableCell colDetailTempF;
        private DevExpress.XtraReports.UI.XRTableCell colDetailSummary;

        private DevExpress.XtraReports.UI.XRLabel lblDateFooter;
        private DevExpress.XtraReports.UI.XRLabel lblSigner1;
        private DevExpress.XtraReports.UI.XRLabel lblSigner2;
        private DevExpress.XtraReports.UI.XRPageInfo pageInfoTime;
        private DevExpress.XtraReports.UI.XRPageInfo pageInfoNumber;
    }
}
