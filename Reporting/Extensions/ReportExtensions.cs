using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web.Mvc;
using Microsoft.Reporting.WebForms;

namespace Reporting
{
    public static class ReportExtensions
    {
        public static FileContentResult GeneratePdf(TextReader reportDefinitionStream,
            Action<ReportDataSourceCollection> action, string fileName, object parameters = null)
        {
            var reportViewer = new ReportViewer();

            reportViewer.LocalReport.LoadReportDefinition(reportDefinitionStream);
            reportDefinitionStream.Close();

            reportViewer.LocalReport.EnableExternalImages = true;

            if (parameters != null)
            {
                reportViewer.LocalReport.SetParameters(parameters.ToReportParamenters());
            }

            action(reportViewer.LocalReport.DataSources);

            reportViewer.LocalReport.Refresh();
            Warning[] warnings;
            string[] streamids;
            string mimeType;
            string encoding;
            string filenameExtension;
            string deviceInfo =
                "<DeviceInfo>" +
                "   <EmbedFonts>None</EmbedFonts>" +
                "</DeviceInfo>";

            byte[] bytes = reportViewer.LocalReport.Render("PDF" /* PDF / EXCEL */, deviceInfo, out mimeType, out encoding, out filenameExtension,
                    out streamids, out warnings);

            var file = new FileContentResult(bytes, mimeType);
            file.FileDownloadName = $"{fileName}.pdf";

            return file;
        }

        public static FileContentResult GenerateExcel(TextReader reportDefinitionStream,
            Action<ReportDataSourceCollection> action, string fileName, object parameters = null)
        {
            var reportViewer = new ReportViewer();

            reportViewer.LocalReport.LoadReportDefinition(reportDefinitionStream);
            reportDefinitionStream.Close();

            reportViewer.LocalReport.EnableExternalImages = true;

            if (parameters != null)
            {
                reportViewer.LocalReport.SetParameters(parameters.ToReportParamenters());
            }

            action(reportViewer.LocalReport.DataSources);

            reportViewer.LocalReport.Refresh();
            Warning[] warnings;
            string[] streamids;
            string mimeType;
            string encoding;
            string filenameExtension;

            byte[] bytes = reportViewer.LocalReport.Render("Excel" /* PDF / EXCEL */, null, out mimeType, out encoding, out filenameExtension,
                    out streamids, out warnings);

            var file = new FileContentResult(bytes, mimeType);
            file.FileDownloadName = $"{fileName}.xls";

            return file;
        }

        public static List<MetaData> ToMetaDataList<T>(this IEnumerable<T> objects, Func<T, MetaData> convert)
            where T : class
        {
            var data = new List<MetaData>();

            foreach (var obj in objects)
            {
                data.Add(convert.Invoke(obj));
            }
            return data;
        }




        public static ReportParameter[] ToReportParamenters(this object parameters)
        {
            var reportParameters = new List<ReportParameter>();
            var objParams = new object[0];
            foreach (var propertyInfo in parameters.GetType().GetProperties())
            {

                var reportParameter = new ReportParameter(propertyInfo.Name, propertyInfo.GetMethod.Invoke(parameters, objParams).ToString());
                reportParameters.Add(reportParameter);
            }

            return reportParameters.ToArray();
        }

        public static List<TOut> ToList<T, TOut>(this IEnumerable<T> objects, Func<T, TOut> convert)
            where T : class
            where TOut : class
        {
            var data = new List<TOut>();

            foreach (var obj in objects)
            {
                data.Add(convert.Invoke(obj));
            }
            return data;
        }
    }

    public class MetaData
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
