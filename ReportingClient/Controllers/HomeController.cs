using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ReportingClient.Models;
using ReportingService;

namespace ReportingClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IHostEnvironment _environment;
        public HomeController(ILogger<HomeController> logger, IHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GeneratePdf()
        {
            var client = new ReportingServiceClient();

            var fileDef = System.IO.File.ReadAllBytes(_environment.ContentRootPath + "\\Reports\\SampleReport.rdl");

            var objects = new[] {new {Id = 1, FullName = "Ahmed 1"}, new {Id = 2, FullName = "Ahmed 2"}};

            var ds = new DataSet();

            ds.Tables.Add(objects.ToDataTable("DataSet1"));

            var reportContent = await client.ExportWithDataTablesAsync(fileDef, ds);

            await client.CloseAsync();

            return File(reportContent.ExportWithDataTablesResult.Content, reportContent.ExportWithDataTablesResult.MimeType, "NewFile.pdf");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
     
    public static class DataTableExtensions
    {
        public static DataTable ToDataTable(this object obj, string tableName)
        {
            var dataTable = new DataTable(tableName);
            var values = new List<object>();
            
            foreach (var propertyInfo in obj.GetType().GetProperties())
            {
                dataTable.Columns.Add(propertyInfo.Name, propertyInfo.PropertyType);
                values.Add(propertyInfo.GetValue(obj));
            }

            dataTable.Rows.Add(values.ToArray());

            return dataTable;
        }

        public static DataTable ToDataTable<T>(this T[] objects, string tableName)
        {
            var dataTable = new DataTable(tableName);
            var firstObject = objects.FirstOrDefault();
            if (firstObject == null) return null;
            var properties = firstObject.GetType().GetProperties();

            foreach (var propertyInfo in properties)
            {
                dataTable.Columns.Add(propertyInfo.Name, propertyInfo.PropertyType);
            }

            foreach (var obj in objects)
            {
                var values = new List<object>();

                foreach (var propertyInfo in properties)
                {
                    values.Add(propertyInfo.GetValue(obj));
                }
                dataTable.Rows.Add(values.ToArray());

            }

            return dataTable;
        }
    }
}
