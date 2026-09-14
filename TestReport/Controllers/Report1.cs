using System;
using System.Collections.Generic;
using DevExpress.XtraReports.UI;

namespace TestReport.Controllers
{
    public partial class Report1 : DevExpress.XtraReports.UI.XtraReport
    {
        public Report1()
        {
            InitializeComponent();
        }

        public Report1(IEnumerable<WeatherForecast> data) : this()
        {
            this.DataSource = data;
        }
    }
}
