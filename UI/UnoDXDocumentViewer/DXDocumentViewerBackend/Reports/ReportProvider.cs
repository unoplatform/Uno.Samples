using DevExpress.XtraReports.Services;
using DevExpress.XtraReports.UI;

namespace DXDocumentViewerBackend.Reports
{
    public class ReportProvider : IReportProvider
    {
        public XtraReport GetReport(string id, ReportProviderContext context)
        {
            if (id.StartsWith("TestReport"))
            {
                var report = new TestReport();

                var band = new GroupHeaderBand();
                band.Controls.Add(new XRLabel()
                {
                    Text = "Test: " + id
                });

                report.Bands.Add(band);

                return report;
            }

            return new XtraReport();
        }
    }
}
