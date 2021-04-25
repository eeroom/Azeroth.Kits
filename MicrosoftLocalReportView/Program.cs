using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicrosoftLocalReportView {
    class Program {
        static void Main(string[] args) {
            Microsoft.Reporting.WinForms.LocalReport report = new Microsoft.Reporting.WinForms.LocalReport();
            using (System.IO.FileStream fs = new System.IO.FileStream("FaceSheet.rdlc", System.IO.FileMode.Open)) {
                report.LoadReportDefinition(fs);
                report.SetParameters(new Microsoft.Reporting.WinForms.ReportParameter("SenderName", "中国湖南省深圳市龙岗区中"));
                report.SetParameters(new Microsoft.Reporting.WinForms.ReportParameter("ReciverName", "召唤师峡谷"));
                var lst = report.ListRenderingExtensions().Select(x => x.Name).ToList();
                string deviceInfo = "<DeviceInfo><OutputFormat>EMF</OutputFormat></DeviceInfo>";
                string mimtype, encoding, fileExt;
                string[] stream;
                Microsoft.Reporting.WinForms.Warning[] warning;
                byte[] buffer = report.Render("IMAGE", deviceInfo, Microsoft.Reporting.WinForms.PageCountMode.Actual, out mimtype, out encoding, out fileExt, out stream, out warning);
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream(buffer)) {
                    var bmp = System.Drawing.Imaging.Metafile.FromStream(ms);
                    bmp.Save("reportview.WMF", System.Drawing.Imaging.ImageFormat.Emf);
                }

            }
        }
    }
}
