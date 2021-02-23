# rdlc-report-in-dotnet-core
Simple way to support RDLC reports in Dotnet Core using WebFrom ReportViewer Control in WCF Service without all complex API of SSRS web service


I have used the WCF service which is using the WebForms ReportView Control to "generate" the report. "I am not showing the report in the viewer but exporting report as PDF, Excel, Word etc"

This solution is much easier than all "available workarounds" specially when you are generating the report data in C #.

And you can also use the same solution for database data sources as WebForms ReportViewer will execute your report.

You can utilize all the feature which are supported WebForms ReportViewer control, which mean all most all the features

The solution is consist of two Web Applications
- Reporting 
  A WebForms application, which is hosting a WCF service, `ExportWithDataTables` operation will accept the report defination and datatables
- ReportingClient
  A dotnet core 3 application with will call the Reporting service mathod to show how it will work.
  
The Reporting application controller also have some experimenting work of *defining* the c# class and creating it's object from *json string* at runtime, and then using the controller action to generate the report from the JSON Data which is used in the report like a table datasource
