using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Reporting.Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IReportingService" in both code and config file together.
    [ServiceContract]
    public interface IReportingService
    {
        [OperationContract]
        ReportContentResult Generate(byte[] fileDefinition);

        [OperationContract]
        ReportContentResult ExportWithDataTables(byte[] fileDefinition, DataSet dataTablesAsDataSource);

        [OperationContract]
        ReportContentResult ExportWithDataTablesAndParameters(byte[] fileDefinition, DataTable[] dataTablesAsDataSource, DataTable parameters);
    }
}
