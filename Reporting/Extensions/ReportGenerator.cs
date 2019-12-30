using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;

namespace Reporting
{
    public static class ReportGenerator
    {

        public static ReportContentResult Export(byte[] fileDefinition, DataTable[] dataTableAsDataSources, DataTable parameters = null)
        {
            ReportContentResult result = null;
            using (var memoryStream = new MemoryStream(fileDefinition))
            {
                result = GenerateDataTablePdf(memoryStream, collection =>
                    {
                        for (int i = 0; i < dataTableAsDataSources.Length; i++)
                        {
                            collection.Add(new ReportDataSource(dataTableAsDataSources[i].TableName, dataTableAsDataSources[i]));
                        }

                    }, null, parameters);
            }

            return result;
        }
        public static ReportContentResult Export(byte[] fileDefinition, DataSource[] dataSources, string parametersJson = null)
        {
            ReportContentResult result = null;
            using (var memoryStream = new MemoryStream(fileDefinition))
            {
                result = GeneratePdf(memoryStream, collection =>
                {
                    for (int i = 0; i < dataSources.Length; i++)
                    {
                        collection.Add(dataSources[i].CreateReportDataSource(Guid.NewGuid()));
                    }

                }, null, parametersJson == null ? null : JsonConvert.DeserializeObject(parametersJson, typeof(Dictionary<string, string>)));
            }


            return result;
        }

        public static ReportContentResult GenerateDataTablePdf(Stream reportDefinitionStream,
            Action<ReportDataSourceCollection> action, string fileName, DataTable parameters = null)
        {
            var reportViewer = new ReportViewer();

            reportViewer.LocalReport.LoadReportDefinition(reportDefinitionStream);
            reportDefinitionStream.Close();

            reportViewer.LocalReport.EnableExternalImages = true;

            if (parameters != null)
            {
                reportViewer.LocalReport.SetParameters(parameters.ToReportParameters());
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

            var file = new ReportContentResult(bytes, mimeType);

            return file;
        }

        public static ReportContentResult GeneratePdf(Stream reportDefinitionStream,
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

            var file = new ReportContentResult(bytes, mimeType);

            return file;
        }

        public static ReportContentResult GenerateExcel(TextReader reportDefinitionStream,
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

            var file = new ReportContentResult(bytes, mimeType);

            return file;
        }
    }

    public class ReportContentResult
    {
        public byte[] Content { get; set; }
        public string MimeType { get; set; }
        public ReportContentResult() { }

        public ReportContentResult(byte[] content, string mimeType)
        {
            Content = content;
            MimeType = mimeType;
        }
    }

    public class DataSource
    {
        public string Name { get; set; }
        public Property[] Properties { get; set; }
        public string DataJson { get; set; }
    }

    public class DataTableDataSource
    {
        public string Name { get; set; }
        public DataTable DataTable { get; set; }
    }

    public static class ReportDataSourceExtensions
    {
        public static ReportDataSource CreateReportDataSource(this DataSource dataSource, Guid uniqueKey)
        {
            var typeOfData = DataTypeBuilder.CreateNewType("ReportName", 
                $"ReportName.Data.{dataSource.Name}_{uniqueKey.ToString()}" , dataSource.Properties);
            Type[] typeArgs = { typeOfData };
            var listOfTypeOfData = typeof(List<>).MakeGenericType(typeArgs);

            var data = JsonConvert.DeserializeObject(dataSource.DataJson, listOfTypeOfData);


            var reportDataSource = new ReportDataSource(dataSource.Name, data);
            return reportDataSource;
        }

        public static ReportParameter[] ToReportParameters(this DataTable parameters)
        {
            var reportParameters = new List<ReportParameter>();
            var objParams = new object[0];
            foreach (DataColumn column in parameters.Columns)
            {
                var reportParameter = new ReportParameter(column.ColumnName, parameters.Rows[0][column].ToString() );
                reportParameters.Add(reportParameter);
            }

            return reportParameters.ToArray();
        }
    }

}
