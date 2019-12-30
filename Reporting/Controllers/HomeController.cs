using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;

namespace Reporting.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        public ActionResult PDFSample()
        {
            var people = new List<object>
            {
                new { Id = 1, FullName = "Ahmed 1" },
                new { Id = 2, FullName = "Ahmed 2" },
                new { Id = 3, FullName = "Ahmed 3" },
                new { Id = 4, FullName = "Ahmed 4" },
                new { Id = 5, FullName = "Ahmed 5" },
                new { Id = 6, FullName = "Ahmed 6" },
                new { Id = 7, FullName = "Ahmed 7" }
            };

            var report = ReportExtensions.GenerateExcel(
                System.IO.File.OpenText(Server.MapPath("~/Reports/SampleReport.rdl")),
                collection => { collection.Add(new ReportDataSource("DataSet1", people)); }, "somefile");

            return report;
        }


        [HttpGet]
        public ActionResult GeneratePDF()
        {
            var userJson =
                "[{\"Id\":1, \"FullName\":\"Ahmed 1\"}, {\"Id\":2, \"FullName\":\"Ahmed 2\"}, {\"Id\":3, \"FullName\":\"Ahmed 3\"}]";
            var properties = new List<Property>();
            properties.Add(new Property("Id", typeof(int)));
            properties.Add(new Property("FullName", typeof(string)));

            var report = ReportGenerator.Export(System.IO.File.ReadAllBytes(Server.MapPath("~/Reports/SampleReport.rdl"))
                , new DataSource[] {
                new DataSource
                {
                    DataJson = userJson,
                    Name = "DataSet1",
                    Properties = properties.ToArray()
                }});

            var fileContent = File(report.Content, report.MimeType, "NewFile.pdf");

            return fileContent;
        }

        [HttpGet]
        public ActionResult GeneratePDF2()
        {
            var userDataTable = new DataTable("DataSet1");
            userDataTable.Columns.Add("Id", typeof(int));
            userDataTable.Columns.Add("FullName", typeof(string));

            userDataTable.Rows.Add(1, "Ahmed 1");
            userDataTable.Rows.Add(2, "Ahmed 2");
            userDataTable.Rows.Add(3, "Ahmed 3");
            userDataTable.Rows.Add(4, "Ahmed 4");
            userDataTable.Rows.Add(5, "Ahmed 5");

            var report = ReportGenerator.Export(System.IO.File.ReadAllBytes(Server.MapPath("~/Reports/SampleReport.rdl"))
                , new DataTable[] { userDataTable });

            var fileContent = File(report.Content, report.MimeType, "NewFile.pdf");

            return fileContent;
        }

        [HttpPost]
        public ActionResult PDFSample2()
        {
            var userJson =
                "[{\"Id\":1, \"FullName\":\"Ahmed 1\"}, {\"Id\":2, \"FullName\":\"Ahmed 2\"}, {\"Id\":3, \"FullName\":\"Ahmed 3\"}]";
            var properties = new List<Property>();
            properties.Add(new Property( "Id", typeof(int)) );
            properties.Add(new Property("FullName", typeof(string)));


            var newType = DataTypeBuilder.CreateNewType("ReportName", "ReportName.Data", properties.ToArray());
            Type[] typeArgs = { newType };
            var listOf =  typeof(List<>).MakeGenericType(typeArgs);
            
            var people = JsonConvert.DeserializeObject(userJson, listOf);
            
            //var people = new List<object>
            //{
            //    new { Id = 1, FullName = "Ahmed 1" },
            //    new { Id = 2, FullName = "Ahmed 2" },
            //    new { Id = 3, FullName = "Ahmed 3" },
            //    new { Id = 4, FullName = "Ahmed 4" },
            //    new { Id = 5, FullName = "Ahmed 5" },
            //    new { Id = 6, FullName = "Ahmed 6" },
            //    new { Id = 7, FullName = "Ahmed 7" }
            //};

            var report = ReportExtensions.GeneratePdf(
                System.IO.File.OpenText(Server.MapPath("~/Reports/SampleReport.rdl")),
                collection => { collection.Add(new ReportDataSource("DataSet1", people)); }, "somefile");

            return report;
        }
    }

    public class Person
    {
        public int Id { get; set; }
        public string FullName { get; set; }
    }

    //public class Field
    //{
    //    public string FieldName { get; set; }
    //    public Type FieldType { get; set; }
    //}
    //public static class MyTypeBuilder
    //{
    //    public static void CreateNewObject(Type type)
    //    {
    //        var myObject = Activator.CreateInstance(type);
    //    }
    //    public static Type CreateNewType(List<Field> fields)
    //    {
    //        return CompileResultType(fields);
    //    }

    //    public static Type CompileResultType(List<Field> fields)
    //    {
    //        TypeBuilder tb = GetTypeBuilder();
    //        ConstructorBuilder constructor =
    //            tb.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName |
    //                                        MethodAttributes.RTSpecialName);

    //        // NOTE: assuming your list contains Field objects with fields FieldName(string) and FieldType(Type)
    //        foreach (var field in fields)
    //            CreateProperty(tb, field.FieldName, field.FieldType);

    //        Type objectType = tb.CreateType();
    //        return objectType;
    //    }

    //    private static TypeBuilder GetTypeBuilder()
    //    {
    //        var typeSignature = "MyDynamicType";
    //        var an = new AssemblyName(typeSignature);
    //        AssemblyBuilder assemblyBuilder =
    //            AppDomain.CurrentDomain.DefineDynamicAssembly(an, AssemblyBuilderAccess.Run);
    //        ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
    //        TypeBuilder tb = moduleBuilder.DefineType(typeSignature,
    //            TypeAttributes.Public |
    //            TypeAttributes.Class |
    //            TypeAttributes.AutoClass |
    //            TypeAttributes.AnsiClass |
    //            TypeAttributes.BeforeFieldInit |
    //            TypeAttributes.AutoLayout,
    //            null);
    //        return tb;
    //    }

    //    private static void CreateProperty(TypeBuilder tb, string propertyName, Type propertyType)
    //    {
    //        FieldBuilder fieldBuilder = tb.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);

    //        PropertyBuilder propertyBuilder =
    //            tb.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);
    //        MethodBuilder getPropMthdBldr = tb.DefineMethod("get_" + propertyName,
    //            MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, propertyType,
    //            Type.EmptyTypes);
    //        ILGenerator getIl = getPropMthdBldr.GetILGenerator();

    //        getIl.Emit(OpCodes.Ldarg_0);
    //        getIl.Emit(OpCodes.Ldfld, fieldBuilder);
    //        getIl.Emit(OpCodes.Ret);

    //        MethodBuilder setPropMthdBldr =
    //            tb.DefineMethod("set_" + propertyName,
    //                MethodAttributes.Public |
    //                MethodAttributes.SpecialName |
    //                MethodAttributes.HideBySig,
    //                null, new[] {propertyType});

    //        ILGenerator setIl = setPropMthdBldr.GetILGenerator();
    //        Label modifyProperty = setIl.DefineLabel();
    //        Label exitSet = setIl.DefineLabel();

    //        setIl.MarkLabel(modifyProperty);
    //        setIl.Emit(OpCodes.Ldarg_0);
    //        setIl.Emit(OpCodes.Ldarg_1);
    //        setIl.Emit(OpCodes.Stfld, fieldBuilder);

    //        setIl.Emit(OpCodes.Nop);
    //        setIl.MarkLabel(exitSet);
    //        setIl.Emit(OpCodes.Ret);

    //        propertyBuilder.SetGetMethod(getPropMthdBldr);
    //        propertyBuilder.SetSetMethod(setPropMthdBldr);
    //    }
    //}
}
