using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Microsoft.Reporting.WebForms;

namespace Reporting.Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "ReportingService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select ReportingService.svc or ReportingService.svc.cs at the Solution Explorer and start debugging.
    public class ReportingService : IReportingService
    {
        public ReportContentResult Generate(byte[] fileDefinition)
        {
            throw new Exception("Not implemented");
        }

        public ReportContentResult ExportWithDataTables(byte[] fileDefinition, DataSet dataTablesAsDataSource)
        {
            DataTable[] tables = new DataTable[dataTablesAsDataSource.Tables.Count];
            var i = 0;
            foreach (DataTable dataTable in dataTablesAsDataSource.Tables)
            {
                tables[i++] = dataTable;
            }

            var result = ReportGenerator.Export(fileDefinition, tables);
            return result;
        }

        public ReportContentResult ExportWithDataTablesAndParameters(byte[] fileDefinition,
            DataTable[] dataTablesAsDataSource, DataTable parameters)
        {
            var result = ReportGenerator.Export(fileDefinition, dataTablesAsDataSource, parameters);
            return result;
        }
    }
}
