namespace TestReport.Api.Infrastructure.Reports
{
    public partial class S11HReport : DevExpress.XtraReports.UI.XtraReport
    {
        public S11HReport()
        {
            InitializeComponent();
        }

        public S11HReport(Domain.Models.S11HReportData data) : this()
        {
            ArgumentNullException.ThrowIfNull(data);
            DataSource = data.CreateRows();
            DataMember = "";
            ReportTextApplier.Apply(this, new Dictionary<string, string>
            {
                ["DonViCapTren"] = data.DonViCapTren,
                ["DonViBaoCao"] = data.DonViBaoCao,
                ["KyBaoCao"] = $"Tháng {data.Thang} năm {data.Nam}"
            });
        }
    }
}
