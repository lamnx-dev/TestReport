#nullable enable
namespace TestReport.Api.Infrastructure.Reports;

partial class S90HReport
{
    private System.ComponentModel.IContainer? components;
    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }

    #region Designer generated code
    private void InitializeComponent()
    {
            this.components = new System.ComponentModel.Container();
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.pageNumber = new DevExpress.XtraReports.UI.XRPageInfo();
            this.ReportHeader = new DevExpress.XtraReports.UI.ReportHeaderBand();
            this.lblDonViCapTren = new DevExpress.XtraReports.UI.XRLabel();
            this.lblDonViBaoCao = new DevExpress.XtraReports.UI.XRLabel();
            this.lblMauSo = new DevExpress.XtraReports.UI.XRLabel();
            this.lblCanCu = new DevExpress.XtraReports.UI.XRLabel();
            this.lblTitle = new DevExpress.XtraReports.UI.XRLabel();
            this.lblKyBaoCao = new DevExpress.XtraReports.UI.XRLabel();
            this.lblDonViTinh = new DevExpress.XtraReports.UI.XRLabel();
            this.GroupHeader = new DevExpress.XtraReports.UI.GroupHeaderBand();
            this.tblHeader = new DevExpress.XtraReports.UI.XRTable();
            this.tblHeaderRow0 = new DevExpress.XtraReports.UI.XRTableRow();
            this.tblHeaderRow0Cell0 = new DevExpress.XtraReports.UI.XRTableCell();
            this.tblHeaderRow0Cell1 = new DevExpress.XtraReports.UI.XRTableCell();
            this.tblHeaderRow0Cell2 = new DevExpress.XtraReports.UI.XRTableCell();
            this.tblHeaderRow0Cell3 = new DevExpress.XtraReports.UI.XRTableCell();
            this.tblHeaderRow0Cell4 = new DevExpress.XtraReports.UI.XRTableCell();
            this.tblHeaderRow1 = new DevExpress.XtraReports.UI.XRTableRow();
            this.tblHeaderRow1Cell0 = new DevExpress.XtraReports.UI.XRTableCell();
            this.tblHeaderRow1Cell1 = new DevExpress.XtraReports.UI.XRTableCell();
            this.tblHeaderRow1Cell2 = new DevExpress.XtraReports.UI.XRTableCell();
            this.tblHeaderRow1Cell3 = new DevExpress.XtraReports.UI.XRTableCell();
            this.tblHeaderRow1Cell4 = new DevExpress.XtraReports.UI.XRTableCell();
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.tblBody = new DevExpress.XtraReports.UI.XRTable();
            this.tblBodyRow0 = new DevExpress.XtraReports.UI.XRTableRow();
            this.tblBodyRow0Cell0 = new DevExpress.XtraReports.UI.XRTableCell();
            this.tblBodyRow0Cell1 = new DevExpress.XtraReports.UI.XRTableCell();
            this.tblBodyRow0Cell2 = new DevExpress.XtraReports.UI.XRTableCell();
            this.tblBodyRow0Cell3 = new DevExpress.XtraReports.UI.XRTableCell();
            this.tblBodyRow0Cell4 = new DevExpress.XtraReports.UI.XRTableCell();
            this.ReportFooter = new DevExpress.XtraReports.UI.ReportFooterBand();
            this.pageCount = new DevExpress.XtraReports.UI.XRPageInfo();
            this.lblNgayMoSo = new DevExpress.XtraReports.UI.XRLabel();
            this.lblNgayKy = new DevExpress.XtraReports.UI.XRLabel();
            this.lblChucDanhKeToan = new DevExpress.XtraReports.UI.XRLabel();
            this.lblHintKeToan = new DevExpress.XtraReports.UI.XRLabel();
            this.lblTenKeToan = new DevExpress.XtraReports.UI.XRLabel();
            this.lblChucDanhKeToanTruong = new DevExpress.XtraReports.UI.XRLabel();
            this.lblHintKeToanTruong = new DevExpress.XtraReports.UI.XRLabel();
            this.lblTenKeToanTruong = new DevExpress.XtraReports.UI.XRLabel();
            this.lblChucDanhThuTruong = new DevExpress.XtraReports.UI.XRLabel();
            this.lblHintThuTruong = new DevExpress.XtraReports.UI.XRLabel();
            this.lblTenThuTruong = new DevExpress.XtraReports.UI.XRLabel();
            this.objectDataSource1 = new DevExpress.DataAccess.ObjectBinding.ObjectDataSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.tblHeader)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tblBody)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.objectDataSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // TopMargin
            // 
            this.TopMargin.HeightF = 40F;
            this.TopMargin.Name = "TopMargin";
            // 
            // BottomMargin
            // 
            this.BottomMargin.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.pageNumber});
            this.BottomMargin.HeightF = 35F;
            this.BottomMargin.Name = "BottomMargin";
            // 
            // pageNumber
            // 
            this.pageNumber.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.pageNumber.Name = "pageNumber";
            this.pageNumber.SizeF = new System.Drawing.SizeF(747F, 20F);
            this.pageNumber.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.pageNumber.TextFormatString = "Trang {0}/{1}";
            // 
            // ReportHeader
            // 
            this.ReportHeader.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.lblDonViCapTren,
            this.lblDonViBaoCao,
            this.lblMauSo,
            this.lblCanCu,
            this.lblTitle,
            this.lblKyBaoCao,
            this.lblDonViTinh});
            this.ReportHeader.HeightF = 185F;
            this.ReportHeader.Name = "ReportHeader";
            // 
            // lblDonViCapTren
            // 
            this.lblDonViCapTren.Font = new DevExpress.Drawing.DXFont("Times New Roman", 10F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblDonViCapTren.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.lblDonViCapTren.Name = "lblDonViCapTren";
            this.lblDonViCapTren.Tag = "DonViCapTren";
            this.lblDonViCapTren.SizeF = new System.Drawing.SizeF(365F, 20F);
            this.lblDonViCapTren.Text = "BẢO HIỂM XÃ HỘI VIỆT NAM";
            this.lblDonViCapTren.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // lblDonViBaoCao
            // 
            this.lblDonViBaoCao.Font = new DevExpress.Drawing.DXFont("Times New Roman", 10F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblDonViBaoCao.LocationFloat = new DevExpress.Utils.PointFloat(0F, 22F);
            this.lblDonViBaoCao.Name = "lblDonViBaoCao";
            this.lblDonViBaoCao.Tag = "DonViBaoCao";
            this.lblDonViBaoCao.SizeF = new System.Drawing.SizeF(365F, 30F);
            this.lblDonViBaoCao.Text = "BẢO HIỂM XÃ HỘI …";
            this.lblDonViBaoCao.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // lblMauSo
            // 
            this.lblMauSo.Font = new DevExpress.Drawing.DXFont("Times New Roman", 10F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblMauSo.LocationFloat = new DevExpress.Utils.PointFloat(380F, 0F);
            this.lblMauSo.Name = "lblMauSo";
            this.lblMauSo.SizeF = new System.Drawing.SizeF(367F, 20F);
            this.lblMauSo.Text = "Mẫu số: S90-H";
            this.lblMauSo.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
            // 
            // lblCanCu
            // 
            this.lblCanCu.Font = new DevExpress.Drawing.DXFont("Times New Roman", 9F);
            this.lblCanCu.LocationFloat = new DevExpress.Utils.PointFloat(380F, 22F);
            this.lblCanCu.Name = "lblCanCu";
            this.lblCanCu.Tag = "CanCu";
            this.lblCanCu.SizeF = new System.Drawing.SizeF(367F, 54F);
            this.lblCanCu.Text = "(Ban hành kèm theo Công văn số 153/BHXH-TCKT ngày 14/01/2026 của BHXH Việt Nam)";
            this.lblCanCu.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
            // 
            // lblTitle
            // 
            this.lblTitle.Font = new DevExpress.Drawing.DXFont("Times New Roman", 14F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblTitle.LocationFloat = new DevExpress.Utils.PointFloat(0F, 87F);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.SizeF = new System.Drawing.SizeF(747F, 29F);
            this.lblTitle.Tag = "ReportTitle";
            this.lblTitle.Text = "BẢNG XÁC ĐỊNH CHÊNH LỆCH THU, CHI";
            this.lblTitle.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // lblKyBaoCao
            // 
            this.lblKyBaoCao.Font = new DevExpress.Drawing.DXFont("Times New Roman", 11F);
            this.lblKyBaoCao.LocationFloat = new DevExpress.Utils.PointFloat(0F, 124F);
            this.lblKyBaoCao.Name = "lblKyBaoCao";
            this.lblKyBaoCao.Tag = "KyBaoCao";
            this.lblKyBaoCao.SizeF = new System.Drawing.SizeF(747F, 24F);
            this.lblKyBaoCao.Text = "Tháng/Quý/Năm";
            this.lblKyBaoCao.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // lblDonViTinh
            // 
            this.lblDonViTinh.Font = new DevExpress.Drawing.DXFont("Times New Roman", 10F);
            this.lblDonViTinh.LocationFloat = new DevExpress.Utils.PointFloat(450F, 160F);
            this.lblDonViTinh.Name = "lblDonViTinh";
            this.lblDonViTinh.Tag = "DonViTinh";
            this.lblDonViTinh.SizeF = new System.Drawing.SizeF(297F, 20F);
            this.lblDonViTinh.Text = "Đơn vị tính: đồng";
            this.lblDonViTinh.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            // 
            // GroupHeader
            // 
            this.GroupHeader.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.tblHeader});
            this.GroupHeader.GroupUnion = DevExpress.XtraReports.UI.GroupUnion.WithFirstDetail;
            this.GroupHeader.HeightF = 46F;
            this.GroupHeader.Name = "GroupHeader";
            this.GroupHeader.RepeatEveryPage = true;
            // 
            // tblHeader
            // 
            this.tblHeader.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.tblHeader.BorderWidth = 0.5F;
            this.tblHeader.Font = new DevExpress.Drawing.DXFont("Times New Roman", 10F, DevExpress.Drawing.DXFontStyle.Bold);
            this.tblHeader.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.tblHeader.Name = "tblHeader";
            this.tblHeader.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.tblHeaderRow0,
            this.tblHeaderRow1});
            this.tblHeader.SizeF = new System.Drawing.SizeF(746F, 46F);
            this.tblHeader.Tag = "TableHeader";
            // 
            // tblHeaderRow0
            // 
            this.tblHeaderRow0.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.tblHeaderRow0Cell0,
            this.tblHeaderRow0Cell1,
            this.tblHeaderRow0Cell2,
            this.tblHeaderRow0Cell3,
            this.tblHeaderRow0Cell4});
            this.tblHeaderRow0.Name = "tblHeaderRow0";
            this.tblHeaderRow0.Weight = 1D;
            // 
            // tblHeaderRow0Cell0
            // 
            this.tblHeaderRow0Cell0.Name = "tblHeaderRow0Cell0";
            this.tblHeaderRow0Cell0.Padding = new DevExpress.XtraPrinting.PaddingInfo(4F, 4F, 3F, 3F, 100F);
            this.tblHeaderRow0Cell0.Text = "STT";
            this.tblHeaderRow0Cell0.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.tblHeaderRow0Cell0.Weight = 40D;
            // 
            // tblHeaderRow0Cell1
            // 
            this.tblHeaderRow0Cell1.Name = "tblHeaderRow0Cell1";
            this.tblHeaderRow0Cell1.Padding = new DevExpress.XtraPrinting.PaddingInfo(4F, 4F, 3F, 3F, 100F);
            this.tblHeaderRow0Cell1.Text = "CHỈ TIÊU";
            this.tblHeaderRow0Cell1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.tblHeaderRow0Cell1.Weight = 337D;
            // 
            // tblHeaderRow0Cell2
            // 
            this.tblHeaderRow0Cell2.Name = "tblHeaderRow0Cell2";
            this.tblHeaderRow0Cell2.Padding = new DevExpress.XtraPrinting.PaddingInfo(4F, 4F, 3F, 3F, 100F);
            this.tblHeaderRow0Cell2.Text = "Mã số";
            this.tblHeaderRow0Cell2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.tblHeaderRow0Cell2.Weight = 55D;
            // 
            // tblHeaderRow0Cell3
            // 
            this.tblHeaderRow0Cell3.Name = "tblHeaderRow0Cell3";
            this.tblHeaderRow0Cell3.Padding = new DevExpress.XtraPrinting.PaddingInfo(4F, 4F, 3F, 3F, 100F);
            this.tblHeaderRow0Cell3.Text = "Số tiền";
            this.tblHeaderRow0Cell3.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.tblHeaderRow0Cell3.Weight = 155D;
            // 
            // tblHeaderRow0Cell4
            // 
            this.tblHeaderRow0Cell4.Name = "tblHeaderRow0Cell4";
            this.tblHeaderRow0Cell4.Padding = new DevExpress.XtraPrinting.PaddingInfo(4F, 4F, 3F, 3F, 100F);
            this.tblHeaderRow0Cell4.Text = "Ghi chú";
            this.tblHeaderRow0Cell4.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.tblHeaderRow0Cell4.Weight = 160D;
            // 
            // tblHeaderRow1
            // 
            this.tblHeaderRow1.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.tblHeaderRow1Cell0,
            this.tblHeaderRow1Cell1,
            this.tblHeaderRow1Cell2,
            this.tblHeaderRow1Cell3,
            this.tblHeaderRow1Cell4});
            this.tblHeaderRow1.Name = "tblHeaderRow1";
            this.tblHeaderRow1.Weight = 1D;
            // 
            // tblHeaderRow1Cell0
            // 
            this.tblHeaderRow1Cell0.Name = "tblHeaderRow1Cell0";
            this.tblHeaderRow1Cell0.Padding = new DevExpress.XtraPrinting.PaddingInfo(4F, 4F, 3F, 3F, 100F);
            this.tblHeaderRow1Cell0.Text = "A";
            this.tblHeaderRow1Cell0.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.tblHeaderRow1Cell0.Weight = 40D;
            // 
            // tblHeaderRow1Cell1
            // 
            this.tblHeaderRow1Cell1.Name = "tblHeaderRow1Cell1";
            this.tblHeaderRow1Cell1.Padding = new DevExpress.XtraPrinting.PaddingInfo(4F, 4F, 3F, 3F, 100F);
            this.tblHeaderRow1Cell1.Text = "B";
            this.tblHeaderRow1Cell1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.tblHeaderRow1Cell1.Weight = 337D;
            // 
            // tblHeaderRow1Cell2
            // 
            this.tblHeaderRow1Cell2.Name = "tblHeaderRow1Cell2";
            this.tblHeaderRow1Cell2.Padding = new DevExpress.XtraPrinting.PaddingInfo(4F, 4F, 3F, 3F, 100F);
            this.tblHeaderRow1Cell2.Text = "C";
            this.tblHeaderRow1Cell2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.tblHeaderRow1Cell2.Weight = 55D;
            // 
            // tblHeaderRow1Cell3
            // 
            this.tblHeaderRow1Cell3.Name = "tblHeaderRow1Cell3";
            this.tblHeaderRow1Cell3.Padding = new DevExpress.XtraPrinting.PaddingInfo(4F, 4F, 3F, 3F, 100F);
            this.tblHeaderRow1Cell3.Text = "1";
            this.tblHeaderRow1Cell3.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.tblHeaderRow1Cell3.Weight = 155D;
            // 
            // tblHeaderRow1Cell4
            // 
            this.tblHeaderRow1Cell4.Name = "tblHeaderRow1Cell4";
            this.tblHeaderRow1Cell4.Padding = new DevExpress.XtraPrinting.PaddingInfo(4F, 4F, 3F, 3F, 100F);
            this.tblHeaderRow1Cell4.Text = "D";
            this.tblHeaderRow1Cell4.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.tblHeaderRow1Cell4.Weight = 160D;
            // 
            // Detail
            // 
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.tblBody});
            this.Detail.HeightF = 28F;
            this.Detail.KeepTogether = true;
            this.Detail.Name = "Detail";
            // 
            // tblBody
            // 
            this.tblBody.Borders = ((DevExpress.XtraPrinting.BorderSide)(((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.tblBody.BorderWidth = 0.5F;
            this.tblBody.Font = new DevExpress.Drawing.DXFont("Times New Roman", 10F);
            this.tblBody.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.tblBody.Name = "tblBody";
            this.tblBody.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.tblBodyRow0});
            this.tblBody.SizeF = new System.Drawing.SizeF(746F, 28F);
            this.tblBody.StylePriority.UseBorders = false;
            this.tblBody.Tag = "TableBody";
            // 
            // tblBodyRow0
            // 
            this.tblBodyRow0.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.tblBodyRow0Cell0,
            this.tblBodyRow0Cell1,
            this.tblBodyRow0Cell2,
            this.tblBodyRow0Cell3,
            this.tblBodyRow0Cell4});
            this.tblBodyRow0.Name = "tblBodyRow0";
            this.tblBodyRow0.Weight = 1D;
            // 
            // tblBodyRow0Cell0
            // 
            this.tblBodyRow0Cell0.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Stt]"),
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Font.Bold", "[InDam]")});
            this.tblBodyRow0Cell0.Name = "tblBodyRow0Cell0";
            this.tblBodyRow0Cell0.Padding = new DevExpress.XtraPrinting.PaddingInfo(4F, 4F, 3F, 3F, 100F);
            this.tblBodyRow0Cell0.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.tblBodyRow0Cell0.Weight = 40D;
            // 
            // tblBodyRow0Cell1
            // 
            this.tblBodyRow0Cell1.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[TenChiTieu]"),
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Font.Bold", "[InDam]")});
            this.tblBodyRow0Cell1.Name = "tblBodyRow0Cell1";
            this.tblBodyRow0Cell1.Padding = new DevExpress.XtraPrinting.PaddingInfo(4F, 4F, 3F, 3F, 100F);
            this.tblBodyRow0Cell1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.tblBodyRow0Cell1.Weight = 337D;
            // 
            // tblBodyRow0Cell2
            // 
            this.tblBodyRow0Cell2.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[MaIn]"),
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Font.Bold", "[InDam]")});
            this.tblBodyRow0Cell2.Name = "tblBodyRow0Cell2";
            this.tblBodyRow0Cell2.Padding = new DevExpress.XtraPrinting.PaddingInfo(4F, 4F, 3F, 3F, 100F);
            this.tblBodyRow0Cell2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.tblBodyRow0Cell2.Weight = 55D;
            // 
            // tblBodyRow0Cell3
            // 
            this.tblBodyRow0Cell3.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[SoTien]"),
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Font.Bold", "[InDam]")});
            this.tblBodyRow0Cell3.Name = "tblBodyRow0Cell3";
            this.tblBodyRow0Cell3.Padding = new DevExpress.XtraPrinting.PaddingInfo(4F, 4F, 3F, 3F, 100F);
            this.tblBodyRow0Cell3.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.tblBodyRow0Cell3.TextFitMode = DevExpress.XtraReports.UI.TextFitMode.ShrinkOnly;
            this.tblBodyRow0Cell3.TextFormatString = "{0:#,##0;(#,##0);0}";
            this.tblBodyRow0Cell3.Weight = 155D;
            this.tblBodyRow0Cell3.WordWrap = false;
            // 
            // tblBodyRow0Cell4
            // 
            this.tblBodyRow0Cell4.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[GhiChu]"),
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Font.Bold", "[InDam]")});
            this.tblBodyRow0Cell4.Name = "tblBodyRow0Cell4";
            this.tblBodyRow0Cell4.Padding = new DevExpress.XtraPrinting.PaddingInfo(4F, 4F, 3F, 3F, 100F);
            this.tblBodyRow0Cell4.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.tblBodyRow0Cell4.Weight = 160D;
            // 
            // ReportFooter
            // 
            this.ReportFooter.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.pageCount,
            this.lblNgayMoSo,
            this.lblNgayKy,
            this.lblChucDanhKeToan,
            this.lblHintKeToan,
            this.lblTenKeToan,
            this.lblChucDanhKeToanTruong,
            this.lblHintKeToanTruong,
            this.lblTenKeToanTruong,
            this.lblChucDanhThuTruong,
            this.lblHintThuTruong,
            this.lblTenThuTruong});
            this.ReportFooter.HeightF = 175F;
            this.ReportFooter.KeepTogether = true;
            this.ReportFooter.Name = "ReportFooter";
            // 
            // pageCount
            // 
            this.pageCount.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.pageCount.Name = "pageCount";
            this.pageCount.SizeF = new System.Drawing.SizeF(747F, 20F);
            this.pageCount.TextFormatString = "Sổ này có {1} trang, đánh số từ trang 01 đến trang {1}.";
            // 
            // lblNgayMoSo
            // 
            this.lblNgayMoSo.Font = new DevExpress.Drawing.DXFont("Times New Roman", 10F);
            this.lblNgayMoSo.LocationFloat = new DevExpress.Utils.PointFloat(0F, 23F);
            this.lblNgayMoSo.Name = "lblNgayMoSo";
            this.lblNgayMoSo.Tag = "NgayMoSo";
            this.lblNgayMoSo.SizeF = new System.Drawing.SizeF(747F, 20F);
            this.lblNgayMoSo.Text = "Ngày mở sổ: ……………….";
            this.lblNgayMoSo.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // lblNgayKy
            // 
            this.lblNgayKy.Font = new DevExpress.Drawing.DXFont("Times New Roman", 10F);
            this.lblNgayKy.LocationFloat = new DevExpress.Utils.PointFloat(480F, 48F);
            this.lblNgayKy.Name = "lblNgayKy";
            this.lblNgayKy.Tag = "NgayKy";
            this.lblNgayKy.SizeF = new System.Drawing.SizeF(267F, 20F);
            this.lblNgayKy.Text = "Ngày … tháng … năm …";
            this.lblNgayKy.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // lblChucDanhKeToan
            // 
            this.lblChucDanhKeToan.Font = new DevExpress.Drawing.DXFont("Times New Roman", 10F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblChucDanhKeToan.LocationFloat = new DevExpress.Utils.PointFloat(0F, 73F);
            this.lblChucDanhKeToan.Name = "lblChucDanhKeToan";
            this.lblChucDanhKeToan.Tag = "ChucDanhKeToan";
            this.lblChucDanhKeToan.SizeF = new System.Drawing.SizeF(245F, 24F);
            this.lblChucDanhKeToan.Text = "KẾ TOÁN";
            this.lblChucDanhKeToan.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
            // 
            // lblHintKeToan
            // 
            this.lblHintKeToan.Font = new DevExpress.Drawing.DXFont("Times New Roman", 10F);
            this.lblHintKeToan.LocationFloat = new DevExpress.Utils.PointFloat(0F, 99F);
            this.lblHintKeToan.Name = "lblHintKeToan";
            this.lblHintKeToan.SizeF = new System.Drawing.SizeF(245F, 20F);
            this.lblHintKeToan.Text = "(Ký, họ tên)";
            this.lblHintKeToan.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
            // 
            // lblTenKeToan
            // 
            this.lblTenKeToan.Font = new DevExpress.Drawing.DXFont("Times New Roman", 10F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblTenKeToan.LocationFloat = new DevExpress.Utils.PointFloat(0F, 148F);
            this.lblTenKeToan.Name = "lblTenKeToan";
            this.lblTenKeToan.Tag = "KeToan";
            this.lblTenKeToan.SizeF = new System.Drawing.SizeF(245F, 24F);
            this.lblTenKeToan.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
            // 
            // lblChucDanhKeToanTruong
            // 
            this.lblChucDanhKeToanTruong.Font = new DevExpress.Drawing.DXFont("Times New Roman", 10F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblChucDanhKeToanTruong.LocationFloat = new DevExpress.Utils.PointFloat(245F, 73F);
            this.lblChucDanhKeToanTruong.Name = "lblChucDanhKeToanTruong";
            this.lblChucDanhKeToanTruong.Tag = "ChucDanhKeToanTruong";
            this.lblChucDanhKeToanTruong.SizeF = new System.Drawing.SizeF(245F, 24F);
            this.lblChucDanhKeToanTruong.Text = "KẾ TOÁN TRƯỞNG";
            this.lblChucDanhKeToanTruong.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
            // 
            // lblHintKeToanTruong
            // 
            this.lblHintKeToanTruong.Font = new DevExpress.Drawing.DXFont("Times New Roman", 10F);
            this.lblHintKeToanTruong.LocationFloat = new DevExpress.Utils.PointFloat(245F, 99F);
            this.lblHintKeToanTruong.Name = "lblHintKeToanTruong";
            this.lblHintKeToanTruong.SizeF = new System.Drawing.SizeF(245F, 20F);
            this.lblHintKeToanTruong.Text = "(Ký, họ tên)";
            this.lblHintKeToanTruong.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
            // 
            // lblTenKeToanTruong
            // 
            this.lblTenKeToanTruong.Font = new DevExpress.Drawing.DXFont("Times New Roman", 10F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblTenKeToanTruong.LocationFloat = new DevExpress.Utils.PointFloat(245F, 148F);
            this.lblTenKeToanTruong.Name = "lblTenKeToanTruong";
            this.lblTenKeToanTruong.Tag = "KeToanTruong";
            this.lblTenKeToanTruong.SizeF = new System.Drawing.SizeF(245F, 24F);
            this.lblTenKeToanTruong.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
            // 
            // lblChucDanhThuTruong
            // 
            this.lblChucDanhThuTruong.Font = new DevExpress.Drawing.DXFont("Times New Roman", 10F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblChucDanhThuTruong.LocationFloat = new DevExpress.Utils.PointFloat(490F, 73F);
            this.lblChucDanhThuTruong.Name = "lblChucDanhThuTruong";
            this.lblChucDanhThuTruong.Tag = "ChucDanhThuTruong";
            this.lblChucDanhThuTruong.SizeF = new System.Drawing.SizeF(257F, 24F);
            this.lblChucDanhThuTruong.Text = "THỦ TRƯỞNG ĐƠN VỊ";
            this.lblChucDanhThuTruong.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
            // 
            // lblHintThuTruong
            // 
            this.lblHintThuTruong.Font = new DevExpress.Drawing.DXFont("Times New Roman", 10F);
            this.lblHintThuTruong.LocationFloat = new DevExpress.Utils.PointFloat(490F, 99F);
            this.lblHintThuTruong.Name = "lblHintThuTruong";
            this.lblHintThuTruong.SizeF = new System.Drawing.SizeF(257F, 20F);
            this.lblHintThuTruong.Text = "(Ký, họ tên, đóng dấu)";
            this.lblHintThuTruong.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
            // 
            // lblTenThuTruong
            // 
            this.lblTenThuTruong.Font = new DevExpress.Drawing.DXFont("Times New Roman", 10F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblTenThuTruong.LocationFloat = new DevExpress.Utils.PointFloat(490F, 148F);
            this.lblTenThuTruong.Name = "lblTenThuTruong";
            this.lblTenThuTruong.Tag = "ThuTruongDonVi";
            this.lblTenThuTruong.SizeF = new System.Drawing.SizeF(257F, 24F);
            this.lblTenThuTruong.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
            // 
            // objectDataSource1
            // 
            this.objectDataSource1.DataMember = "GetSampleRows";
            this.objectDataSource1.DataSourceType = null;
            this.objectDataSource1.Name = "objectDataSource1";
            // 
            // S90HReport
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.TopMargin,
            this.BottomMargin,
            this.ReportHeader,
            this.GroupHeader,
            this.Detail,
            this.ReportFooter});
            this.ComponentStorage.AddRange(new System.ComponentModel.IComponent[] {
            this.objectDataSource1});
            this.DataSource = this.objectDataSource1;
            this.Font = new DevExpress.Drawing.DXFont("Times New Roman", 10F);
            this.Margins = new DevExpress.Drawing.DXMargins(40F, 40F, 40F, 35F);
            this.PageHeightF = 1169.291F;
            this.PageWidthF = 826.7717F;
            this.PaperKind = DevExpress.Drawing.Printing.DXPaperKind.A4;
            this.DisplayName = "S90-H";
            this.Version = "25.2";
            ((System.ComponentModel.ISupportInitialize)(this.tblHeader)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tblBody)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.objectDataSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

    }
    #endregion

    private DevExpress.XtraReports.UI.TopMarginBand TopMargin = null!;
    private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin = null!;
    private DevExpress.XtraReports.UI.ReportHeaderBand ReportHeader = null!;
    private DevExpress.XtraReports.UI.GroupHeaderBand GroupHeader = null!;
    private DevExpress.XtraReports.UI.DetailBand Detail = null!;
    private DevExpress.XtraReports.UI.ReportFooterBand ReportFooter = null!;
    private DevExpress.DataAccess.ObjectBinding.ObjectDataSource objectDataSource1 = null!;
    private DevExpress.XtraReports.UI.XRLabel lblDonViCapTren = null!;
    private DevExpress.XtraReports.UI.XRLabel lblDonViBaoCao = null!;
    private DevExpress.XtraReports.UI.XRLabel lblMauSo = null!;
    private DevExpress.XtraReports.UI.XRLabel lblCanCu = null!;
    private DevExpress.XtraReports.UI.XRLabel lblTitle = null!;
    private DevExpress.XtraReports.UI.XRLabel lblKyBaoCao = null!;
    private DevExpress.XtraReports.UI.XRLabel lblDonViTinh = null!;
    private DevExpress.XtraReports.UI.XRTable tblHeader = null!;
    private DevExpress.XtraReports.UI.XRTableRow tblHeaderRow0 = null!;
    private DevExpress.XtraReports.UI.XRTableCell tblHeaderRow0Cell0 = null!;
    private DevExpress.XtraReports.UI.XRTableCell tblHeaderRow0Cell1 = null!;
    private DevExpress.XtraReports.UI.XRTableCell tblHeaderRow0Cell2 = null!;
    private DevExpress.XtraReports.UI.XRTableCell tblHeaderRow0Cell3 = null!;
    private DevExpress.XtraReports.UI.XRTableCell tblHeaderRow0Cell4 = null!;
    private DevExpress.XtraReports.UI.XRTableRow tblHeaderRow1 = null!;
    private DevExpress.XtraReports.UI.XRTableCell tblHeaderRow1Cell0 = null!;
    private DevExpress.XtraReports.UI.XRTableCell tblHeaderRow1Cell1 = null!;
    private DevExpress.XtraReports.UI.XRTableCell tblHeaderRow1Cell2 = null!;
    private DevExpress.XtraReports.UI.XRTableCell tblHeaderRow1Cell3 = null!;
    private DevExpress.XtraReports.UI.XRTableCell tblHeaderRow1Cell4 = null!;
    private DevExpress.XtraReports.UI.XRTable tblBody = null!;
    private DevExpress.XtraReports.UI.XRTableRow tblBodyRow0 = null!;
    private DevExpress.XtraReports.UI.XRTableCell tblBodyRow0Cell0 = null!;
    private DevExpress.XtraReports.UI.XRTableCell tblBodyRow0Cell1 = null!;
    private DevExpress.XtraReports.UI.XRTableCell tblBodyRow0Cell2 = null!;
    private DevExpress.XtraReports.UI.XRTableCell tblBodyRow0Cell3 = null!;
    private DevExpress.XtraReports.UI.XRTableCell tblBodyRow0Cell4 = null!;
    private DevExpress.XtraReports.UI.XRPageInfo pageCount = null!;
    private DevExpress.XtraReports.UI.XRLabel lblNgayMoSo = null!;
    private DevExpress.XtraReports.UI.XRLabel lblNgayKy = null!;
    private DevExpress.XtraReports.UI.XRLabel lblChucDanhKeToan = null!;
    private DevExpress.XtraReports.UI.XRLabel lblHintKeToan = null!;
    private DevExpress.XtraReports.UI.XRLabel lblTenKeToan = null!;
    private DevExpress.XtraReports.UI.XRLabel lblChucDanhKeToanTruong = null!;
    private DevExpress.XtraReports.UI.XRLabel lblHintKeToanTruong = null!;
    private DevExpress.XtraReports.UI.XRLabel lblTenKeToanTruong = null!;
    private DevExpress.XtraReports.UI.XRLabel lblChucDanhThuTruong = null!;
    private DevExpress.XtraReports.UI.XRLabel lblHintThuTruong = null!;
    private DevExpress.XtraReports.UI.XRLabel lblTenThuTruong = null!;
    private DevExpress.XtraReports.UI.XRPageInfo pageNumber = null!;
}
