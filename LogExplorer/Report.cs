using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using System.Linq;

using ClosedXML.Excel;
using System.Data;
using System.Reflection;

namespace LogExplorer
{

    public abstract class Report
    {
        public List<ReportLine> ReportList;
        public string FileName;


        public abstract void CreateReport(ObservableCollection<LogLine> logLineList, Dictionary<string, object> formData);

       

        internal void Write(string filePath, bool xml, bool xls)
        {
            if (xml)
                WriteToXML<List<ReportLine>>(ReportList, filePath);
            if (xls)
                WriteToXLS( filePath);
        }


        internal void WriteToXML<T>(T reportList, string filePath)
        {
            string fullPath = filePath + "\\" + FileName + ".xml";
            XmlDocument xmlDoc = new XmlDocument();
            XPathNavigator nav = xmlDoc.CreateNavigator();
            using (XmlWriter writer = nav.AppendChild())
            {
                XmlSerializer ser = new XmlSerializer(typeof(T), new XmlRootAttribute("TheRootElementName"));
                ser.Serialize(writer, reportList);
            }
            File.WriteAllText(fullPath, xmlDoc.InnerXml);
        }


        // Альтернативный способ используя COM объекты. Использовался в прошлой версии тестового.
        // Исключен из-за возможных проблем доступа к excel файлу, и обязательного условия наличия установленного MS Office 
        // Предполагаю что ошибки в этом методе были
        //  using Excel = Microsoft.Office.Interop.Excel;
        //          public Excel.Worksheet sheet;
        //private void WriteToXLS<T>(string filePath)
        //{

        //    System.Reflection.Missing missingValue = System.Reflection.Missing.Value;

        //    string fullPath = Environment.CurrentDirectory + "\\" + FileName + ".xls";

        //    Excel.Application application;
        //    Excel.Workbook book;


        //    application = new Microsoft.Office.Interop.Excel.Application();
        //    book = application.Workbooks.Add(missingValue);
        //    sheet = (Excel.Worksheet)book.Worksheets.get_Item(1);
        //    try
        //    {
        //        FillRows();
        //        book.SaveAs(fullPath);
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //    finally {
        //        book.Close(true, missingValue, missingValue);
        //        application.Quit();

        //        System.Runtime.InteropServices.Marshal.ReleaseComObject(sheet);
        //        System.Runtime.InteropServices.Marshal.ReleaseComObject(book);
        //        System.Runtime.InteropServices.Marshal.ReleaseComObject(application);
        //    }  
        //}


        private void WriteToXLS(string filePath)
        {
            string fullPath = filePath + "\\" + FileName + ".xlsx";
            DataTable excelDT = new DataTable();

            excelDT = ToDataTable(ReportList);

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(excelDT, "Отчет");
                wb.SaveAs(fullPath);
            }

        }

        public DataTable ToDataTable(List<ReportLine> lines)
        {
            
            Type type = lines[0].GetLineType();

            DataTable dataTable = new DataTable(type.Name);
            PropertyInfo[] Props = type.GetProperties();

            foreach (PropertyInfo prop in Props)
            {
                dataTable.Columns.Add(prop.Name);
            }
            foreach (ReportLine item in lines)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            return dataTable;
        }
    }

    class UserReport : Report
    {
        private ReportFactory factory;
        public UserReport(ReportFactory factory) { this.factory = factory; }

        
        public override void CreateReport(ObservableCollection<LogLine> logLineList, Dictionary<string, object> formData)
        {
            DateTime setTime = (DateTime)formData["InDate"];
            FileName = "Отчет по пользователям за сутки_" + setTime.ToShortDateString();
            ReportList = new List<ReportLine>();


            List<LogLine> findList = logLineList.Where(ll => ll.OutDate > setTime && ll.OutDate < setTime.AddHours(24)).ToList();
            Dictionary<string, int> dict = new Dictionary<string, int>();

            foreach (LogLine line in findList)
            {
                ReportList.Add(factory.CreateLine(line));
            }

        }
    }

    class IPReport : Report
    {
        private ReportFactory factory;

        public IPReport(ReportFactory factory)
        {
            this.factory = factory;
        }
        public override void CreateReport(ObservableCollection<LogLine> logLineList, Dictionary<string, object> formData)
        {
            DateTime startTime = (DateTime)formData["InDate"];
            DateTime endTime = (DateTime)formData["OutDate"];

            FileName = "Отчет по количеству подключений с каждого IP адреса за период_" + startTime.ToShortDateString() + "-" + endTime.ToShortDateString();
            ReportList = new List<ReportLine>();


            List<LogLine> findList = logLineList.Where(ll => ll.OutDate > startTime && ll.OutDate < endTime).ToList();
            Dictionary<string, int> dict = new Dictionary<string, int>();
            foreach (LogLine line in findList)
            {
                string ip = line.IP;
                if (dict.ContainsKey(ip))
                {
                    dict[ip] += 1;
                }
                else { dict.Add(ip, 1); }
                ReportList.Add(factory.CreateLine(line));
            }
            foreach (ReportLine line in ReportList)
            {
                ((IPReportLine)line).QConnect = dict[((IPReportLine)line).IP];
            }
        }
  


    }
    class CompanyReport : Report
    {
        private ReportFactory factory;

        public CompanyReport(ReportFactory factory)
        {
            this.factory = factory;
        }
        public override void CreateReport(ObservableCollection<LogLine> logLineList, Dictionary<string, object> formData)
        {

            string company = (string)formData["Company"];
            List<LogLine> findList = logLineList.Where(ll => ll.Company == company).ToList();
            FileName = "Отчет по организации_" + company;
            ReportList = new List<ReportLine>();


            foreach (LogLine line in findList)
            {
                ReportList.Add(factory.CreateLine(line));
            }
        }
      


    }
    class CompanyUserReport : Report
    {
        private ReportFactory factory;

        public CompanyUserReport(ReportFactory factory)
        {
            this.factory = factory;
        }
        public override void CreateReport(ObservableCollection<LogLine> logLineList, Dictionary<string, object> formData)
        {
            ReportList = new List<ReportLine>();

            DateTime startTime = (DateTime)formData["InDate"]; ;
            DateTime endTime = (DateTime)formData["OutDate"]; ;

            FileName = "Отчет по количеству пользователей от организаций за период " + startTime.ToShortDateString() + "-" + endTime.ToShortDateString();


            List<LogLine> findList = logLineList.Where(ll => ll.OutDate > startTime && ll.OutDate < endTime).ToList();

            Dictionary<string, int> dict = new Dictionary<string, int>();
            foreach (LogLine line in findList)
            {
                string company = line.Company;
                if (dict.ContainsKey(company))
                {
                    dict[company] += 1;
                }
                else { dict.Add(company, 1); }


            }
            System.Collections.IDictionaryEnumerator enumerator = dict.GetEnumerator();
            int i = 0;
            while (enumerator.MoveNext())
            {
                ReportLine rl = factory.CreateLine(null);
                ((CompanyUserReportLine)rl).NN = i++;
                ((CompanyUserReportLine)rl).Company = (string)enumerator.Key;
                ((CompanyUserReportLine)rl).QUser = (int)enumerator.Value;
                ReportList.Add(rl);
            }
        }
      

    }
    class ErrorReport : Report
    {
        private ReportFactory factory;
        public ErrorReport(ReportFactory factory) { this.factory = factory; }

        public override void CreateReport(ObservableCollection<LogLine> logLineList, Dictionary<string, object> formData)
        {
            ReportList = new List<ReportLine>();
            DateTime startTime = (DateTime)formData["InDate"];
            DateTime endTime = (DateTime)formData["OutDate"];

            FileName = "Отчет по ошибкам за период " + startTime.ToShortDateString() + "-" + endTime.ToShortDateString();


            List<LogLine> findList = logLineList.Where(ll => ll.OutDate > startTime && ll.OutDate < endTime && ll.Error.Equals("1")).ToList();
            Dictionary<string, int> dict = new Dictionary<string, int>();

            foreach (LogLine line in findList)
            {
                ReportList.Add(factory.CreateLine(line));
            }

        }      
    }

    [Serializable, XmlInclude(typeof(UserReportLine))]
    public abstract class ReportLine {
        public abstract Type GetLineType();
    }
    [Serializable, XmlInclude(typeof(IPReportLine))]
    public class UserReportLine : ReportLine
    {
        public int NN { get; set; }
        public string Name { get; set; }
        public string Company { get; set; }
        public string IP { get; set; }
        public string ID { get; set; }
        public DateTime InDate { get; set; }
        public DateTime OutDate { get; set; }
        public string Error { get; set; }

        public override Type GetLineType()
        {
            return typeof(UserReportLine);
        }

        private LogLine LOG_LINE;

        public UserReportLine() { }

        public UserReportLine(LogLine ll)
        {
            this.LOG_LINE = ll;
            this.NN = ll.NN;
            this.Name = ll.Name;
            this.Company = ll.Company;
            this.IP = ll.IP;
            this.ID = ll.ID;
            this.InDate = ll.InDate;
            this.OutDate = ll.OutDate;
            this.Error = ll.Error;
        }
    }
    [Serializable, XmlInclude(typeof(CompanyReportLine))]
    public class IPReportLine : ReportLine
    {
        public string NN { get; set; }
        public string Name { get; set; }
        public string IP { get; set; }
        public string InDate { get; set; }
        public string OutDate { get; set; }
        public int QConnect { get; set; }

        private LogLine LOG_LINE;

        public override Type GetLineType()
        {
            return typeof(IPReportLine);
        }

        public IPReportLine() { }

        public IPReportLine(LogLine ll)
        {
            this.LOG_LINE = ll;
            this.NN = ll.NN.ToString();
            this.InDate = ll.InDate.ToString();
            this.OutDate = ll.OutDate.ToString();
            this.IP = ll.IP;
            this.Name = ll.Name;
        }

        //Если потребуются заголовки
        //private static IPReportLine _header;
        //public static string NN_HEADER = "Порядковый номер";
        //public static string NAME_HEADER = "Пользователь";
        //public static string IP_HEADER = "IP адрес";
        //public static string INDATE_HEADER = "Дата входа";
        //public static string OUTDATE_HEADER = "Дата выхода";

        //public static IPReportLine GetHeader()
        //{
        //    if (_header == null)
        //    {
        //        _header = new IPReportLine(ll);
        //        _header.NN = NN_HEADER;
        //        _header.Name = NAME_HEADER;
        //        _header.InDate = INDATE_HEADER;
        //        _header.OutDate = OUTDATE_HEADER;
        //        _header.IP = IP_HEADER;

        //    }
        //    return _header;
        //}
    }
    [Serializable, XmlInclude(typeof(CompanyUserReportLine))]
    public class CompanyReportLine : ReportLine
    {
        public string NN { get; set; }
        public string Name { get; set; }
        public string IP { get; set; }
        public string SummDate { get; set; }


        private LogLine LOG_LINE;

        public CompanyReportLine() { }

        public override Type GetLineType()
        {
            return typeof(CompanyReportLine);
        }

        public CompanyReportLine(LogLine ll)
        {
            this.LOG_LINE = ll;
            this.NN = ll.NN.ToString();
            this.IP = ll.Name.ToString();
            this.Name = ll.IP;



        }

    }
    [Serializable, XmlInclude(typeof(ErrorReportLine))]
    public class CompanyUserReportLine : ReportLine
    {
        public int NN { get; set; }
        public string Company { get; set; }
        public int QUser { get; set; }

        private LogLine LOG_LINE;

        public override Type GetLineType()
        {
            return typeof(CompanyUserReportLine);
        }

        public CompanyUserReportLine() { }

        public CompanyUserReportLine(LogLine ll)
        {
            this.LOG_LINE = ll;
            this.QUser = 1;
        }
    }
    [Serializable]
    public class ErrorReportLine : ReportLine
    {
        public int NN { get; set; }
        public string Name { get; set; }
        public string Company { get; set; }
        public string IP { get; set; }
        public string ID { get; set; }
        public DateTime InDate { get; set; }
        public DateTime OutDate { get; set; }

        private LogLine LOG_LINE;

        public override Type GetLineType()
        {
            return typeof(ErrorReportLine);
        }

        public ErrorReportLine() { }

        public ErrorReportLine(LogLine ll)
        {
            this.LOG_LINE = ll;
            this.NN = ll.NN;
            this.Name = ll.Name;
            this.Company = ll.Company;
            this.IP = ll.IP;
            this.ID = ll.ID;
            this.InDate = ll.InDate;
            this.OutDate = ll.OutDate;
        }
    }

    abstract class ReportFactory
    {
        public abstract ReportLine CreateLine(LogLine ll);
    }
    class UserReportFactory : ReportFactory
    {
        public override ReportLine CreateLine(LogLine ll)
        {
            return new UserReportLine(ll);
        }
    }
    class IPReportFactory : ReportFactory
    {
        public override ReportLine CreateLine(LogLine ll)
        {
            return new IPReportLine(ll);
        }
    }
    class CompanyReportFactory : ReportFactory
    {
        public override ReportLine CreateLine(LogLine ll)
        {
            return new CompanyReportLine(ll);
        }
    }
    class CompanyUserReportFactory : ReportFactory
    {
        public override ReportLine CreateLine(LogLine ll)
        {
            return new CompanyUserReportLine(ll);
        }
    }
    class ErrorReportFactory : ReportFactory
    {
        public override ReportLine CreateLine(LogLine ll)
        {
            return new ErrorReportLine(ll);
        }
    }
}