#nullable enable
namespace TestReport.Api.Infrastructure.Reports
{
    partial class S55HReport
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
            this.ReportHeader = new DevExpress.XtraReports.UI.ReportHeaderBand();
            this.GroupHeader = new DevExpress.XtraReports.UI.GroupHeaderBand();
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.ReportFooter = new DevExpress.XtraReports.UI.ReportFooterBand();
            this.lblDonViCapTren = new DevExpress.XtraReports.UI.XRLabel();
            this.lblDonViBaoCao = new DevExpress.XtraReports.UI.XRLabel();
            this.lblMauSo = new DevExpress.XtraReports.UI.XRLabel();
            this.lblCanCu = new DevExpress.XtraReports.UI.XRLabel();
            this.lblTitle = new DevExpress.XtraReports.UI.XRLabel();
            this.lblKyBaoCao = new DevExpress.XtraReports.UI.XRLabel();
            this.headingNgayGhiSo = new DevExpress.XtraReports.UI.XRLabel();
            this.headingChungTu = new DevExpress.XtraReports.UI.XRLabel();
            this.headingSoChungTu = new DevExpress.XtraReports.UI.XRLabel();
            this.headingNgayChungTu = new DevExpress.XtraReports.UI.XRLabel();
            this.headingDienGiai = new DevExpress.XtraReports.UI.XRLabel();
            this.headingDuocMienGiam = new DevExpress.XtraReports.UI.XRLabel();
            this.headingDaMienGiam = new DevExpress.XtraReports.UI.XRLabel();
            this.tblColumnCodes = new DevExpress.XtraReports.UI.XRTable();
            this.tblColumnCodesRow = new DevExpress.XtraReports.UI.XRTableRow();
            this.tblColumnCodesCell0 = new DevExpress.XtraReports.UI.XRTableCell();
            this.tblColumnCodesCell1 = new DevExpress.XtraReports.UI.XRTableCell();
            this.tblColumnCodesCell2 = new DevExpress.XtraReports.UI.XRTableCell();
            this.tblColumnCodesCell3 = new DevExpress.XtraReports.UI.XRTableCell();
            this.tblColumnCodesCell4 = new DevExpress.XtraReports.UI.XRTableCell();
            this.tblColumnCodesCell5 = new DevExpress.XtraReports.UI.XRTableCell();
            this.tblBody = new DevExpress.XtraReports.UI.XRTable();
            this.tblBodyRow = new DevExpress.XtraReports.UI.XRTableRow();
            this.tblBodyCell0 = new DevExpress.XtraReports.UI.XRTableCell();
            this.tblBodyCell1 = new DevExpress.XtraReports.UI.XRTableCell();
            this.tblBodyCell2 = new DevExpress.XtraReports.UI.XRTableCell();
            this.tblBodyCell3 = new DevExpress.XtraReports.UI.XRTableCell();
            this.tblBodyCell4 = new DevExpress.XtraReports.UI.XRTableCell();
            this.tblBodyCell5 = new DevExpress.XtraReports.UI.XRTableCell();
            this.pageNumber = new DevExpress.XtraReports.UI.XRPageInfo();
            this.pageCount = new DevExpress.XtraReports.UI.XRPageInfo();
            this.lblNgayMoSo = new DevExpress.XtraReports.UI.XRLabel();
            this.lblNgayKy = new DevExpress.XtraReports.UI.XRLabel();
            this.signatureTitle0 = new DevExpress.XtraReports.UI.XRLabel();
            this.signatureHint0 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblKeToan = new DevExpress.XtraReports.UI.XRLabel();
            this.signatureTitle1 = new DevExpress.XtraReports.UI.XRLabel();
            this.signatureHint1 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblKeToanTruong = new DevExpress.XtraReports.UI.XRLabel();
            this.signatureTitle2 = new DevExpress.XtraReports.UI.XRLabel();
            this.signatureHint2 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblThuTruongDonVi = new DevExpress.XtraReports.UI.XRLabel();
            this.objectDataSource1 = new DevExpress.DataAccess.ObjectBinding.ObjectDataSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.tblColumnCodes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tblBody)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.objectDataSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            //
            // TopMargin
            //
            this.TopMargin.Name = "TopMargin";
            this.TopMargin.HeightF = 40F;
            //
            // BottomMargin
            //
            this.BottomMargin.Name = "BottomMargin";
            this.BottomMargin.HeightF = 35F;
            this.BottomMargin.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] { this.pageNumber });
            //
            // ReportHeader
            //
            this.ReportHeader.Name = "ReportHeader";
            this.ReportHeader.HeightF = 185F;
            this.ReportHeader.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] { this.lblDonViCapTren, this.lblDonViBaoCao, this.lblMauSo, this.lblCanCu, this.lblTitle, this.lblKyBaoCao });
            //
            // GroupHeader
            //
            this.GroupHeader.Name = "GroupHeader";
            this.GroupHeader.HeightF = 84F;
            this.GroupHeader.RepeatEveryPage = true;
            this.GroupHeader.GroupUnion = DevExpress.XtraReports.UI.GroupUnion.WithFirstDetail;
            this.GroupHeader.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] { this.headingNgayGhiSo, this.headingChungTu, this.headingSoChungTu, this.headingNgayChungTu, this.headingDienGiai, this.headingDuocMienGiam, this.headingDaMienGiam, this.tblColumnCodes });
            //
            // Detail
            //
            this.Detail.Name = "Detail";
            this.Detail.HeightF = 28F;
            this.Detail.KeepTogether = true;
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] { this.tblBody });
            //
            // ReportFooter
            //
            this.ReportFooter.Name = "ReportFooter";
            this.ReportFooter.HeightF = 180F;
            this.ReportFooter.KeepTogether = true;
            this.ReportFooter.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] { this.pageCount, this.lblNgayMoSo, this.lblNgayKy, this.signatureTitle0, this.signatureHint0, this.lblKeToan, this.signatureTitle1, this.signatureHint1, this.lblKeToanTruong, this.signatureTitle2, this.signatureHint2, this.lblThuTruongDonVi });
            //
            // lblDonViCapTren
            //
            this.lblDonViCapTren.Name = "lblDonViCapTren";
            this.lblDonViCapTren.Text = "BẢO HIỂM XÃ HỘI VIỆT NAM";
            this.lblDonViCapTren.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.lblDonViCapTren.SizeF = new System.Drawing.SizeF(410F, 22F);
            this.lblDonViCapTren.Font = new DevExpress.Drawing.DXFont("Times New Roman", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblDonViCapTren.Multiline = true;
            this.lblDonViCapTren.CanGrow = true;
            this.lblDonViCapTren.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.lblDonViCapTren.Tag = "DonViCapTren";
            //
            // lblDonViBaoCao
            //
            this.lblDonViBaoCao.Name = "lblDonViBaoCao";
            this.lblDonViBaoCao.Text = "BẢO HIỂM XÃ HỘI …";
            this.lblDonViBaoCao.LocationFloat = new DevExpress.Utils.PointFloat(0F, 25F);
            this.lblDonViBaoCao.SizeF = new System.Drawing.SizeF(410F, 22F);
            this.lblDonViBaoCao.Font = new DevExpress.Drawing.DXFont("Times New Roman", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblDonViBaoCao.Multiline = true;
            this.lblDonViBaoCao.CanGrow = true;
            this.lblDonViBaoCao.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.lblDonViBaoCao.Tag = "DonViBaoCao";
            //
            // lblMauSo
            //
            this.lblMauSo.Name = "lblMauSo";
            this.lblMauSo.Text = "Mẫu số: S55-H";
            this.lblMauSo.LocationFloat = new DevExpress.Utils.PointFloat(447F, 0F);
            this.lblMauSo.SizeF = new System.Drawing.SizeF(300F, 22F);
            this.lblMauSo.Font = new DevExpress.Drawing.DXFont("Times New Roman", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblMauSo.Multiline = true;
            this.lblMauSo.CanGrow = true;
            this.lblMauSo.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            //
            // lblCanCu
            //
            this.lblCanCu.Name = "lblCanCu";
            this.lblCanCu.Text = "(Ban hành kèm theo Công văn số .../2024/BHXH-TCKT ngày .../.../2024 của BHXH Việt Nam)";
            this.lblCanCu.LocationFloat = new DevExpress.Utils.PointFloat(447F, 25F);
            this.lblCanCu.SizeF = new System.Drawing.SizeF(300F, 55F);
            this.lblCanCu.Font = new DevExpress.Drawing.DXFont("Times New Roman", 9F, DevExpress.Drawing.DXFontStyle.Regular);
            this.lblCanCu.Multiline = true;
            this.lblCanCu.CanGrow = true;
            this.lblCanCu.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
            this.lblCanCu.Tag = "CanCu";
            //
            // lblTitle
            //
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Text = "SỔ THEO DÕI THUẾ GIÁ TRỊ GIA TĂNG ĐƯỢC MIỄN GIẢM";
            this.lblTitle.LocationFloat = new DevExpress.Utils.PointFloat(0F, 88F);
            this.lblTitle.SizeF = new System.Drawing.SizeF(747F, 50F);
            this.lblTitle.Font = new DevExpress.Drawing.DXFont("Times New Roman", 14F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblTitle.Multiline = true;
            this.lblTitle.CanGrow = true;
            this.lblTitle.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.lblTitle.Tag = "ReportTitle";
            //
            // lblKyBaoCao
            //
            this.lblKyBaoCao.Name = "lblKyBaoCao";
            this.lblKyBaoCao.Text = "Năm: ………………..";
            this.lblKyBaoCao.LocationFloat = new DevExpress.Utils.PointFloat(0F, 150F);
            this.lblKyBaoCao.SizeF = new System.Drawing.SizeF(747F, 25F);
            this.lblKyBaoCao.Font = new DevExpress.Drawing.DXFont("Times New Roman", 9F, DevExpress.Drawing.DXFontStyle.Regular);
            this.lblKyBaoCao.Multiline = true;
            this.lblKyBaoCao.CanGrow = true;
            this.lblKyBaoCao.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.lblKyBaoCao.Tag = "KyBaoCao";
            //
            // headingNgayGhiSo
            //
            this.headingNgayGhiSo.Name = "headingNgayGhiSo";
            this.headingNgayGhiSo.Text = "Ngày tháng ghi sổ";
            this.headingNgayGhiSo.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.headingNgayGhiSo.SizeF = new System.Drawing.SizeF(75F, 60F);
            this.headingNgayGhiSo.Font = new DevExpress.Drawing.DXFont("Times New Roman", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.headingNgayGhiSo.Multiline = true;
            this.headingNgayGhiSo.CanGrow = false;
            this.headingNgayGhiSo.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.headingNgayGhiSo.Tag = "TableHeader";
            this.headingNgayGhiSo.Borders = (DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Bottom);
            this.headingNgayGhiSo.BorderWidth = 0.5F;
            this.headingNgayGhiSo.BorderColor = System.Drawing.Color.Black;
            //
            // headingChungTu
            //
            this.headingChungTu.Name = "headingChungTu";
            this.headingChungTu.Text = "Chứng từ";
            this.headingChungTu.LocationFloat = new DevExpress.Utils.PointFloat(75F, 0F);
            this.headingChungTu.SizeF = new System.Drawing.SizeF(155F, 30F);
            this.headingChungTu.Font = new DevExpress.Drawing.DXFont("Times New Roman", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.headingChungTu.Multiline = true;
            this.headingChungTu.CanGrow = false;
            this.headingChungTu.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.headingChungTu.Tag = "TableHeader";
            this.headingChungTu.Borders = (DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Bottom);
            this.headingChungTu.BorderWidth = 0.5F;
            this.headingChungTu.BorderColor = System.Drawing.Color.Black;
            //
            // headingSoChungTu
            //
            this.headingSoChungTu.Name = "headingSoChungTu";
            this.headingSoChungTu.Text = "Số hiệu";
            this.headingSoChungTu.LocationFloat = new DevExpress.Utils.PointFloat(75F, 30F);
            this.headingSoChungTu.SizeF = new System.Drawing.SizeF(80F, 30F);
            this.headingSoChungTu.Font = new DevExpress.Drawing.DXFont("Times New Roman", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.headingSoChungTu.Multiline = true;
            this.headingSoChungTu.CanGrow = false;
            this.headingSoChungTu.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.headingSoChungTu.Tag = "TableHeader";
            this.headingSoChungTu.Borders = (DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Bottom);
            this.headingSoChungTu.BorderWidth = 0.5F;
            this.headingSoChungTu.BorderColor = System.Drawing.Color.Black;
            //
            // headingNgayChungTu
            //
            this.headingNgayChungTu.Name = "headingNgayChungTu";
            this.headingNgayChungTu.Text = "Ngày, tháng";
            this.headingNgayChungTu.LocationFloat = new DevExpress.Utils.PointFloat(155F, 30F);
            this.headingNgayChungTu.SizeF = new System.Drawing.SizeF(75F, 30F);
            this.headingNgayChungTu.Font = new DevExpress.Drawing.DXFont("Times New Roman", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.headingNgayChungTu.Multiline = true;
            this.headingNgayChungTu.CanGrow = false;
            this.headingNgayChungTu.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.headingNgayChungTu.Tag = "TableHeader";
            this.headingNgayChungTu.Borders = (DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Bottom);
            this.headingNgayChungTu.BorderWidth = 0.5F;
            this.headingNgayChungTu.BorderColor = System.Drawing.Color.Black;
            //
            // headingDienGiai
            //
            this.headingDienGiai.Name = "headingDienGiai";
            this.headingDienGiai.Text = "Diễn giải";
            this.headingDienGiai.LocationFloat = new DevExpress.Utils.PointFloat(230F, 0F);
            this.headingDienGiai.SizeF = new System.Drawing.SizeF(257F, 60F);
            this.headingDienGiai.Font = new DevExpress.Drawing.DXFont("Times New Roman", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.headingDienGiai.Multiline = true;
            this.headingDienGiai.CanGrow = false;
            this.headingDienGiai.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.headingDienGiai.Tag = "TableHeader";
            this.headingDienGiai.Borders = (DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Bottom);
            this.headingDienGiai.BorderWidth = 0.5F;
            this.headingDienGiai.BorderColor = System.Drawing.Color.Black;
            //
            // headingDuocMienGiam
            //
            this.headingDuocMienGiam.Name = "headingDuocMienGiam";
            this.headingDuocMienGiam.Text = "Số tiền thuế GTGT được miễn giảm";
            this.headingDuocMienGiam.LocationFloat = new DevExpress.Utils.PointFloat(487F, 0F);
            this.headingDuocMienGiam.SizeF = new System.Drawing.SizeF(130F, 60F);
            this.headingDuocMienGiam.Font = new DevExpress.Drawing.DXFont("Times New Roman", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.headingDuocMienGiam.Multiline = true;
            this.headingDuocMienGiam.CanGrow = false;
            this.headingDuocMienGiam.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.headingDuocMienGiam.Tag = "TableHeader";
            this.headingDuocMienGiam.Borders = (DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Bottom);
            this.headingDuocMienGiam.BorderWidth = 0.5F;
            this.headingDuocMienGiam.BorderColor = System.Drawing.Color.Black;
            //
            // headingDaMienGiam
            //
            this.headingDaMienGiam.Name = "headingDaMienGiam";
            this.headingDaMienGiam.Text = "Số tiền thuế GTGT đã miễn giảm";
            this.headingDaMienGiam.LocationFloat = new DevExpress.Utils.PointFloat(617F, 0F);
            this.headingDaMienGiam.SizeF = new System.Drawing.SizeF(130F, 60F);
            this.headingDaMienGiam.Font = new DevExpress.Drawing.DXFont("Times New Roman", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.headingDaMienGiam.Multiline = true;
            this.headingDaMienGiam.CanGrow = false;
            this.headingDaMienGiam.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.headingDaMienGiam.Tag = "TableHeader";
            this.headingDaMienGiam.Borders = (DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Bottom);
            this.headingDaMienGiam.BorderWidth = 0.5F;
            this.headingDaMienGiam.BorderColor = System.Drawing.Color.Black;
            //
            // tblColumnCodes
            //
            this.tblColumnCodes.Name = "tblColumnCodes";
            this.tblColumnCodes.LocationFloat = new DevExpress.Utils.PointFloat(0F, 60F);
            this.tblColumnCodes.SizeF = new System.Drawing.SizeF(747F, 24F);
            this.tblColumnCodes.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.tblColumnCodes.BorderWidth = 0.5F;
            this.tblColumnCodes.Tag = "TableHeader";
            this.tblColumnCodes.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] { this.tblColumnCodesRow });
            //
            // tblColumnCodesRow
            //
            this.tblColumnCodesRow.Name = "tblColumnCodesRow";
            this.tblColumnCodesRow.Weight = 1D;
            this.tblColumnCodesRow.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] { this.tblColumnCodesCell0, this.tblColumnCodesCell1, this.tblColumnCodesCell2, this.tblColumnCodesCell3, this.tblColumnCodesCell4, this.tblColumnCodesCell5 });
            //
            // tblColumnCodesCell0
            //
            this.tblColumnCodesCell0.Name = "tblColumnCodesCell0";
            this.tblColumnCodesCell0.Weight = 75D;
            this.tblColumnCodesCell0.Borders = (DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Bottom);
            this.tblColumnCodesCell0.BorderWidth = 0.5F;
            this.tblColumnCodesCell0.BorderColor = System.Drawing.Color.Black;
            this.tblColumnCodesCell0.CanGrow = true;
            this.tblColumnCodesCell0.Multiline = true;
            this.tblColumnCodesCell0.Padding = new DevExpress.XtraPrinting.PaddingInfo(3, 3, 3, 3, 100F);
            this.tblColumnCodesCell0.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.tblColumnCodesCell0.Text = "A";
            //
            // tblColumnCodesCell1
            //
            this.tblColumnCodesCell1.Name = "tblColumnCodesCell1";
            this.tblColumnCodesCell1.Weight = 80D;
            this.tblColumnCodesCell1.Borders = (DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Bottom);
            this.tblColumnCodesCell1.BorderWidth = 0.5F;
            this.tblColumnCodesCell1.BorderColor = System.Drawing.Color.Black;
            this.tblColumnCodesCell1.CanGrow = true;
            this.tblColumnCodesCell1.Multiline = true;
            this.tblColumnCodesCell1.Padding = new DevExpress.XtraPrinting.PaddingInfo(3, 3, 3, 3, 100F);
            this.tblColumnCodesCell1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.tblColumnCodesCell1.Text = "B";
            //
            // tblColumnCodesCell2
            //
            this.tblColumnCodesCell2.Name = "tblColumnCodesCell2";
            this.tblColumnCodesCell2.Weight = 75D;
            this.tblColumnCodesCell2.Borders = (DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Bottom);
            this.tblColumnCodesCell2.BorderWidth = 0.5F;
            this.tblColumnCodesCell2.BorderColor = System.Drawing.Color.Black;
            this.tblColumnCodesCell2.CanGrow = true;
            this.tblColumnCodesCell2.Multiline = true;
            this.tblColumnCodesCell2.Padding = new DevExpress.XtraPrinting.PaddingInfo(3, 3, 3, 3, 100F);
            this.tblColumnCodesCell2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.tblColumnCodesCell2.Text = "C";
            //
            // tblColumnCodesCell3
            //
            this.tblColumnCodesCell3.Name = "tblColumnCodesCell3";
            this.tblColumnCodesCell3.Weight = 257D;
            this.tblColumnCodesCell3.Borders = (DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Bottom);
            this.tblColumnCodesCell3.BorderWidth = 0.5F;
            this.tblColumnCodesCell3.BorderColor = System.Drawing.Color.Black;
            this.tblColumnCodesCell3.CanGrow = true;
            this.tblColumnCodesCell3.Multiline = true;
            this.tblColumnCodesCell3.Padding = new DevExpress.XtraPrinting.PaddingInfo(3, 3, 3, 3, 100F);
            this.tblColumnCodesCell3.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.tblColumnCodesCell3.Text = "D";
            //
            // tblColumnCodesCell4
            //
            this.tblColumnCodesCell4.Name = "tblColumnCodesCell4";
            this.tblColumnCodesCell4.Weight = 130D;
            this.tblColumnCodesCell4.Borders = (DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Bottom);
            this.tblColumnCodesCell4.BorderWidth = 0.5F;
            this.tblColumnCodesCell4.BorderColor = System.Drawing.Color.Black;
            this.tblColumnCodesCell4.CanGrow = true;
            this.tblColumnCodesCell4.Multiline = true;
            this.tblColumnCodesCell4.Padding = new DevExpress.XtraPrinting.PaddingInfo(3, 3, 3, 3, 100F);
            this.tblColumnCodesCell4.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.tblColumnCodesCell4.Text = "1";
            //
            // tblColumnCodesCell5
            //
            this.tblColumnCodesCell5.Name = "tblColumnCodesCell5";
            this.tblColumnCodesCell5.Weight = 130D;
            this.tblColumnCodesCell5.Borders = (DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Bottom);
            this.tblColumnCodesCell5.BorderWidth = 0.5F;
            this.tblColumnCodesCell5.BorderColor = System.Drawing.Color.Black;
            this.tblColumnCodesCell5.CanGrow = true;
            this.tblColumnCodesCell5.Multiline = true;
            this.tblColumnCodesCell5.Padding = new DevExpress.XtraPrinting.PaddingInfo(3, 3, 3, 3, 100F);
            this.tblColumnCodesCell5.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.tblColumnCodesCell5.Text = "2";
            //
            // tblBody
            //
            this.tblBody.Name = "tblBody";
            this.tblBody.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.tblBody.SizeF = new System.Drawing.SizeF(747F, 28F);
            this.tblBody.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.tblBody.BorderWidth = 0.5F;
            this.tblBody.Tag = "TableBody";
            this.tblBody.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] { this.tblBodyRow });
            //
            // tblBodyRow
            //
            this.tblBodyRow.Name = "tblBodyRow";
            this.tblBodyRow.Weight = 1D;
            this.tblBodyRow.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] { this.tblBodyCell0, this.tblBodyCell1, this.tblBodyCell2, this.tblBodyCell3, this.tblBodyCell4, this.tblBodyCell5 });
            //
            // tblBodyCell0
            //
            this.tblBodyCell0.Name = "tblBodyCell0";
            this.tblBodyCell0.Weight = 75D;
            this.tblBodyCell0.Borders = (DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Bottom);
            this.tblBodyCell0.BorderWidth = 0.5F;
            this.tblBodyCell0.BorderColor = System.Drawing.Color.Black;
            this.tblBodyCell0.CanGrow = true;
            this.tblBodyCell0.Multiline = true;
            this.tblBodyCell0.Padding = new DevExpress.XtraPrinting.PaddingInfo(3, 3, 3, 3, 100F);
            this.tblBodyCell0.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.tblBodyCell0.TextFormatString = "{0:dd'/'MM'/'yyyy}";
            this.tblBodyCell0.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] { new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[NgayGhiSo]"), new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Font.Bold", "[InDam]") });
            //
            // tblBodyCell1
            //
            this.tblBodyCell1.Name = "tblBodyCell1";
            this.tblBodyCell1.Weight = 80D;
            this.tblBodyCell1.Borders = (DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Bottom);
            this.tblBodyCell1.BorderWidth = 0.5F;
            this.tblBodyCell1.BorderColor = System.Drawing.Color.Black;
            this.tblBodyCell1.CanGrow = true;
            this.tblBodyCell1.Multiline = true;
            this.tblBodyCell1.Padding = new DevExpress.XtraPrinting.PaddingInfo(3, 3, 3, 3, 100F);
            this.tblBodyCell1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.tblBodyCell1.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] { new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[SoChungTu]"), new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Font.Bold", "[InDam]") });
            //
            // tblBodyCell2
            //
            this.tblBodyCell2.Name = "tblBodyCell2";
            this.tblBodyCell2.Weight = 75D;
            this.tblBodyCell2.Borders = (DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Bottom);
            this.tblBodyCell2.BorderWidth = 0.5F;
            this.tblBodyCell2.BorderColor = System.Drawing.Color.Black;
            this.tblBodyCell2.CanGrow = true;
            this.tblBodyCell2.Multiline = true;
            this.tblBodyCell2.Padding = new DevExpress.XtraPrinting.PaddingInfo(3, 3, 3, 3, 100F);
            this.tblBodyCell2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.tblBodyCell2.TextFormatString = "{0:dd'/'MM'/'yyyy}";
            this.tblBodyCell2.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] { new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[NgayChungTu]"), new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Font.Bold", "[InDam]") });
            //
            // tblBodyCell3
            //
            this.tblBodyCell3.Name = "tblBodyCell3";
            this.tblBodyCell3.Weight = 257D;
            this.tblBodyCell3.Borders = (DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Bottom);
            this.tblBodyCell3.BorderWidth = 0.5F;
            this.tblBodyCell3.BorderColor = System.Drawing.Color.Black;
            this.tblBodyCell3.CanGrow = true;
            this.tblBodyCell3.Multiline = true;
            this.tblBodyCell3.Padding = new DevExpress.XtraPrinting.PaddingInfo(3, 3, 3, 3, 100F);
            this.tblBodyCell3.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.tblBodyCell3.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] { new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[DienGiai]"), new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Font.Bold", "[InDam]") });
            //
            // tblBodyCell4
            //
            this.tblBodyCell4.Name = "tblBodyCell4";
            this.tblBodyCell4.Weight = 130D;
            this.tblBodyCell4.Borders = (DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Bottom);
            this.tblBodyCell4.BorderWidth = 0.5F;
            this.tblBodyCell4.BorderColor = System.Drawing.Color.Black;
            this.tblBodyCell4.CanGrow = true;
            this.tblBodyCell4.Multiline = true;
            this.tblBodyCell4.Padding = new DevExpress.XtraPrinting.PaddingInfo(3, 3, 3, 3, 100F);
            this.tblBodyCell4.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.tblBodyCell4.TextFormatString = "{0:#,##0;(#,##0);0}";
            this.tblBodyCell4.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] { new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "Iif([SoTienDuocMienGiam] == 0, Null, [SoTienDuocMienGiam])"), new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Font.Bold", "[InDam]") });
            //
            // tblBodyCell5
            //
            this.tblBodyCell5.Name = "tblBodyCell5";
            this.tblBodyCell5.Weight = 130D;
            this.tblBodyCell5.Borders = (DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Bottom);
            this.tblBodyCell5.BorderWidth = 0.5F;
            this.tblBodyCell5.BorderColor = System.Drawing.Color.Black;
            this.tblBodyCell5.CanGrow = true;
            this.tblBodyCell5.Multiline = true;
            this.tblBodyCell5.Padding = new DevExpress.XtraPrinting.PaddingInfo(3, 3, 3, 3, 100F);
            this.tblBodyCell5.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.tblBodyCell5.TextFormatString = "{0:#,##0;(#,##0);0}";
            this.tblBodyCell5.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] { new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "Iif([DaMienGiam] == 0, Null, [DaMienGiam])"), new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Font.Bold", "[InDam]") });
            //
            // pageNumber
            //
            this.pageNumber.Name = "pageNumber";
            this.pageNumber.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.pageNumber.SizeF = new System.Drawing.SizeF(747F, 20F);
            this.pageNumber.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.pageNumber.TextFormatString = "Trang {0}/{1}";
            //
            // pageCount
            //
            this.pageCount.Name = "pageCount";
            this.pageCount.LocationFloat = new DevExpress.Utils.PointFloat(0F, 4F);
            this.pageCount.SizeF = new System.Drawing.SizeF(747F, 20F);
            this.pageCount.PageInfo = DevExpress.XtraPrinting.PageInfo.Total;
            this.pageCount.TextFormatString = "- Sổ này có {0} trang, đánh số từ trang 01 đến trang {0}";
            //
            // lblNgayMoSo
            //
            this.lblNgayMoSo.Name = "lblNgayMoSo";
            this.lblNgayMoSo.Text = "- Ngày mở sổ: ………………………";
            this.lblNgayMoSo.LocationFloat = new DevExpress.Utils.PointFloat(0F, 28F);
            this.lblNgayMoSo.SizeF = new System.Drawing.SizeF(747F, 22F);
            this.lblNgayMoSo.Font = new DevExpress.Drawing.DXFont("Times New Roman", 9F, DevExpress.Drawing.DXFontStyle.Regular);
            this.lblNgayMoSo.Multiline = true;
            this.lblNgayMoSo.CanGrow = true;
            this.lblNgayMoSo.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.lblNgayMoSo.Tag = "NgayMoSo";
            //
            // lblNgayKy
            //
            this.lblNgayKy.Name = "lblNgayKy";
            this.lblNgayKy.Text = "Ngày ... tháng ... năm ...";
            this.lblNgayKy.LocationFloat = new DevExpress.Utils.PointFloat(498F, 55F);
            this.lblNgayKy.SizeF = new System.Drawing.SizeF(249F, 22F);
            this.lblNgayKy.Font = new DevExpress.Drawing.DXFont("Times New Roman", 9F, DevExpress.Drawing.DXFontStyle.Regular);
            this.lblNgayKy.Multiline = true;
            this.lblNgayKy.CanGrow = true;
            this.lblNgayKy.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.lblNgayKy.Tag = "NgayKy";
            //
            // signatureTitle0
            //
            this.signatureTitle0.Name = "signatureTitle0";
            this.signatureTitle0.Text = "KẾ TOÁN";
            this.signatureTitle0.LocationFloat = new DevExpress.Utils.PointFloat(0F, 82F);
            this.signatureTitle0.SizeF = new System.Drawing.SizeF(249F, 22F);
            this.signatureTitle0.Font = new DevExpress.Drawing.DXFont("Times New Roman", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.signatureTitle0.Multiline = true;
            this.signatureTitle0.CanGrow = true;
            this.signatureTitle0.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            //
            // signatureHint0
            //
            this.signatureHint0.Name = "signatureHint0";
            this.signatureHint0.Text = "(Ký, họ tên)";
            this.signatureHint0.LocationFloat = new DevExpress.Utils.PointFloat(0F, 106F);
            this.signatureHint0.SizeF = new System.Drawing.SizeF(249F, 22F);
            this.signatureHint0.Font = new DevExpress.Drawing.DXFont("Times New Roman", 9F, DevExpress.Drawing.DXFontStyle.Regular);
            this.signatureHint0.Multiline = true;
            this.signatureHint0.CanGrow = true;
            this.signatureHint0.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            //
            // lblKeToan
            //
            this.lblKeToan.Name = "lblKeToan";
            this.lblKeToan.Text = "";
            this.lblKeToan.LocationFloat = new DevExpress.Utils.PointFloat(0F, 152F);
            this.lblKeToan.SizeF = new System.Drawing.SizeF(249F, 22F);
            this.lblKeToan.Font = new DevExpress.Drawing.DXFont("Times New Roman", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblKeToan.Multiline = true;
            this.lblKeToan.CanGrow = true;
            this.lblKeToan.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.lblKeToan.Tag = "KeToan";
            //
            // signatureTitle1
            //
            this.signatureTitle1.Name = "signatureTitle1";
            this.signatureTitle1.Text = "KẾ TOÁN TRƯỞNG";
            this.signatureTitle1.LocationFloat = new DevExpress.Utils.PointFloat(249F, 82F);
            this.signatureTitle1.SizeF = new System.Drawing.SizeF(249F, 22F);
            this.signatureTitle1.Font = new DevExpress.Drawing.DXFont("Times New Roman", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.signatureTitle1.Multiline = true;
            this.signatureTitle1.CanGrow = true;
            this.signatureTitle1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            //
            // signatureHint1
            //
            this.signatureHint1.Name = "signatureHint1";
            this.signatureHint1.Text = "(Ký, họ tên)";
            this.signatureHint1.LocationFloat = new DevExpress.Utils.PointFloat(249F, 106F);
            this.signatureHint1.SizeF = new System.Drawing.SizeF(249F, 22F);
            this.signatureHint1.Font = new DevExpress.Drawing.DXFont("Times New Roman", 9F, DevExpress.Drawing.DXFontStyle.Regular);
            this.signatureHint1.Multiline = true;
            this.signatureHint1.CanGrow = true;
            this.signatureHint1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            //
            // lblKeToanTruong
            //
            this.lblKeToanTruong.Name = "lblKeToanTruong";
            this.lblKeToanTruong.Text = "";
            this.lblKeToanTruong.LocationFloat = new DevExpress.Utils.PointFloat(249F, 152F);
            this.lblKeToanTruong.SizeF = new System.Drawing.SizeF(249F, 22F);
            this.lblKeToanTruong.Font = new DevExpress.Drawing.DXFont("Times New Roman", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblKeToanTruong.Multiline = true;
            this.lblKeToanTruong.CanGrow = true;
            this.lblKeToanTruong.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.lblKeToanTruong.Tag = "KeToanTruong";
            //
            // signatureTitle2
            //
            this.signatureTitle2.Name = "signatureTitle2";
            this.signatureTitle2.Text = "THỦ TRƯỞNG ĐƠN VỊ";
            this.signatureTitle2.LocationFloat = new DevExpress.Utils.PointFloat(498F, 82F);
            this.signatureTitle2.SizeF = new System.Drawing.SizeF(249F, 22F);
            this.signatureTitle2.Font = new DevExpress.Drawing.DXFont("Times New Roman", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.signatureTitle2.Multiline = true;
            this.signatureTitle2.CanGrow = true;
            this.signatureTitle2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            //
            // signatureHint2
            //
            this.signatureHint2.Name = "signatureHint2";
            this.signatureHint2.Text = "(Ký, họ tên, đóng dấu)";
            this.signatureHint2.LocationFloat = new DevExpress.Utils.PointFloat(498F, 106F);
            this.signatureHint2.SizeF = new System.Drawing.SizeF(249F, 22F);
            this.signatureHint2.Font = new DevExpress.Drawing.DXFont("Times New Roman", 9F, DevExpress.Drawing.DXFontStyle.Regular);
            this.signatureHint2.Multiline = true;
            this.signatureHint2.CanGrow = true;
            this.signatureHint2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            //
            // lblThuTruongDonVi
            //
            this.lblThuTruongDonVi.Name = "lblThuTruongDonVi";
            this.lblThuTruongDonVi.Text = "";
            this.lblThuTruongDonVi.LocationFloat = new DevExpress.Utils.PointFloat(498F, 152F);
            this.lblThuTruongDonVi.SizeF = new System.Drawing.SizeF(249F, 22F);
            this.lblThuTruongDonVi.Font = new DevExpress.Drawing.DXFont("Times New Roman", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblThuTruongDonVi.Multiline = true;
            this.lblThuTruongDonVi.CanGrow = true;
            this.lblThuTruongDonVi.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.lblThuTruongDonVi.Tag = "ThuTruongDonVi";
            //
            // objectDataSource1
            //
            this.objectDataSource1.Name = "objectDataSource1";
            this.objectDataSource1.DataSource = typeof(global::TestReport.Api.Domain.Models.S55HReportData);
            this.objectDataSource1.DataMember = "GetSampleRows";
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] { this.TopMargin, this.BottomMargin, this.ReportHeader, this.GroupHeader, this.Detail, this.ReportFooter });
            this.ComponentStorage.AddRange(new System.ComponentModel.IComponent[] { this.objectDataSource1 });
            this.DataSource = this.objectDataSource1;
            this.Font = new DevExpress.Drawing.DXFont("Times New Roman", 9F);
            this.Margins = new DevExpress.Drawing.DXMargins(40F, 40F, 40F, 35F);
            this.PageWidthF = 826.7717F;
            this.PageHeightF = 1169.291F;
            this.PaperKind = DevExpress.Drawing.Printing.DXPaperKind.A4;
            this.Name = "S55HReport";
            this.DisplayName = "S55-H";
            this.Version = "25.2";
            ((System.ComponentModel.ISupportInitialize)(this.tblColumnCodes)).EndInit();
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
        private DevExpress.XtraReports.UI.XRLabel lblDonViCapTren = null!;
        private DevExpress.XtraReports.UI.XRLabel lblDonViBaoCao = null!;
        private DevExpress.XtraReports.UI.XRLabel lblMauSo = null!;
        private DevExpress.XtraReports.UI.XRLabel lblCanCu = null!;
        private DevExpress.XtraReports.UI.XRLabel lblTitle = null!;
        private DevExpress.XtraReports.UI.XRLabel lblKyBaoCao = null!;
        private DevExpress.XtraReports.UI.XRLabel headingNgayGhiSo = null!;
        private DevExpress.XtraReports.UI.XRLabel headingChungTu = null!;
        private DevExpress.XtraReports.UI.XRLabel headingSoChungTu = null!;
        private DevExpress.XtraReports.UI.XRLabel headingNgayChungTu = null!;
        private DevExpress.XtraReports.UI.XRLabel headingDienGiai = null!;
        private DevExpress.XtraReports.UI.XRLabel headingDuocMienGiam = null!;
        private DevExpress.XtraReports.UI.XRLabel headingDaMienGiam = null!;
        private DevExpress.XtraReports.UI.XRTable tblColumnCodes = null!;
        private DevExpress.XtraReports.UI.XRTableRow tblColumnCodesRow = null!;
        private DevExpress.XtraReports.UI.XRTableCell tblColumnCodesCell0 = null!;
        private DevExpress.XtraReports.UI.XRTableCell tblColumnCodesCell1 = null!;
        private DevExpress.XtraReports.UI.XRTableCell tblColumnCodesCell2 = null!;
        private DevExpress.XtraReports.UI.XRTableCell tblColumnCodesCell3 = null!;
        private DevExpress.XtraReports.UI.XRTableCell tblColumnCodesCell4 = null!;
        private DevExpress.XtraReports.UI.XRTableCell tblColumnCodesCell5 = null!;
        private DevExpress.XtraReports.UI.XRTable tblBody = null!;
        private DevExpress.XtraReports.UI.XRTableRow tblBodyRow = null!;
        private DevExpress.XtraReports.UI.XRTableCell tblBodyCell0 = null!;
        private DevExpress.XtraReports.UI.XRTableCell tblBodyCell1 = null!;
        private DevExpress.XtraReports.UI.XRTableCell tblBodyCell2 = null!;
        private DevExpress.XtraReports.UI.XRTableCell tblBodyCell3 = null!;
        private DevExpress.XtraReports.UI.XRTableCell tblBodyCell4 = null!;
        private DevExpress.XtraReports.UI.XRTableCell tblBodyCell5 = null!;
        private DevExpress.XtraReports.UI.XRPageInfo pageNumber = null!;
        private DevExpress.XtraReports.UI.XRPageInfo pageCount = null!;
        private DevExpress.XtraReports.UI.XRLabel lblNgayMoSo = null!;
        private DevExpress.XtraReports.UI.XRLabel lblNgayKy = null!;
        private DevExpress.XtraReports.UI.XRLabel signatureTitle0 = null!;
        private DevExpress.XtraReports.UI.XRLabel signatureHint0 = null!;
        private DevExpress.XtraReports.UI.XRLabel lblKeToan = null!;
        private DevExpress.XtraReports.UI.XRLabel signatureTitle1 = null!;
        private DevExpress.XtraReports.UI.XRLabel signatureHint1 = null!;
        private DevExpress.XtraReports.UI.XRLabel lblKeToanTruong = null!;
        private DevExpress.XtraReports.UI.XRLabel signatureTitle2 = null!;
        private DevExpress.XtraReports.UI.XRLabel signatureHint2 = null!;
        private DevExpress.XtraReports.UI.XRLabel lblThuTruongDonVi = null!;
        private DevExpress.DataAccess.ObjectBinding.ObjectDataSource objectDataSource1 = null!;
    }
}
