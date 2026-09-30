using DevExpress.XtraReports.UI;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace TestReport.Api.Infrastructure.Reports
{
    public partial class Report1 : DevExpress.XtraReports.UI.XtraReport
    {
        public Report1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Constructor truyền dữ liệu thực tế từ Service / Controller
        /// </summary>
        /// <param name="data">Danh sách dữ liệu lấy từ Database</param>
        public Report1(IEnumerable<TestReport.Api.Domain.Models.ReportItemDto> data) : this()
        {
            this.DataSource = data;
        }
    }
}
