namespace TestReport.Api.Infrastructure.Reports
{
    public partial class S11_H_Report : DevExpress.XtraReports.UI.XtraReport
    {
        public S11_H_Report()
        {
            InitializeComponent();
        }

        public S11_H_Report(Domain.Models.S11HReportData data) : this()
        {
            ArgumentNullException.ThrowIfNull(data);
            DataSource = data.CreateRows();
            DataMember = "";
            xrLabel1.Text = data.DonViCapTren;
            xrLabel2.Text = data.DonViBaoCao;
            xrLabel5.Text = $"Tháng {data.Thang} năm {data.Nam}";
        }
    }
}
