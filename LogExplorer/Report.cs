using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using System.Linq;
using Excel = Microsoft.Office.Interop.Excel;

namespace LogExplorer
{
    public abstract class Report
    {
        public List<ReportLine> ReportList;
        public string FileName;
        public Excel.Worksheet sheet;


        public abstract void CreateReport(ObservableCollection<LogLine> logLineList, Dictionary<string, object> formData);
        public abstract void FillRows();

        internal void Write(string filePath, bool xml, bool xls)
        {
            if (xml)
                WriteToXML<List<ReportLine>>(ReportList, filePath);
            if (xls)
                WriteToXLS<List<ReportLine>>(ReportList, filePath);
        }

        internal void WriteToXML<T>(T rl, string filePath)
        {
            string fullPath = filePath + "\\" + FileName + ".xml";
            XmlDocument xmlDoc = new XmlDocument();
            XPathNavigator nav = xmlDoc.CreateNavigator();
            using (XmlWriter writer = nav.AppendChild())
            {
                XmlSerializer ser = new XmlSerializer(typeof(T), new XmlRootAttribute("TheRootElementName"));
                ser.Serialize(writer, rl);
            }
            File.WriteAllText(fullPath, xmlDoc.InnerXml);
        }

        private void WriteToXLS<T>(T rl, string filePath)
        {

            System.Reflection.Missing missingValue = System.Reflection.Missing.Value;

            string fullPath = Environment.CurrentDirectory + "\\" + FileName + ".xls";

            Excel.Application application;
            Excel.Workbook book;


            application = new Microsoft.Office.Interop.Excel.Application();
            book = application.Workbooks.Add(missingValue);
            sheet = (Excel.Worksheet)book.Worksheets.get_Item(1);
            try
            {
                FillRows();
                book.SaveAs(fullPath);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                book.Close(true, missingValue, missingValue);
                application.Quit();

                System.Runtime.InteropServices.Marshal.ReleaseComObject(sheet);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(book);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(application);
            }
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

            foreach (LogLine line in findList)
            {
                ReportList.Add(factory.CreateLine(line));
            }

        }
        public override void FillRows()
        {
            int i = 1;
            foreach (ReportLine line in ReportList)
            {
                sheet.Cells[i, "A"] = ((UserReportLine)line).NN;
                sheet.Cells[i, "B"] = ((UserReportLine)line).Name;
                sheet.Cells[i, "C"] = ((UserReportLine)line).Company;
                sheet.Cells[i, "D"] = ((UserReportLine)line).IP;
                sheet.Cells[i, "E"] = ((UserReportLine)line).ID;
                sheet.Cells[i, "F"] = ((UserReportLine)line).InDate;
                sheet.Cells[i, "G"] = ((UserReportLine)line).OutDate;
                sheet.Cells[i++, "H"] = ((UserReportLine)line).Error;
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
        public override void FillRows()
        {
            int i = 1;
            foreach (ReportLine line in ReportList)
            {
                sheet.Cells[i, "A"] = ((IPReportLine)line).NN;
                sheet.Cells[i, "B"] = ((IPReportLine)line).Name;
                sheet.Cells[i, "D"] = ((IPReportLine)line).IP;
                sheet.Cells[i, "E"] = ((IPReportLine)line).InDate;
                sheet.Cells[i, "F"] = ((IPReportLine)line).OutDate;
                sheet.Cells[i++, "G"] = ((IPReportLine)line).QConnect;
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
            FileName = "Отчет по организации: " + company;
            ReportList = new List<ReportLine>();


            foreach (LogLine line in findList)
            {
                ReportList.Add(factory.CreateLine(line));
            }
        }
        public override void FillRows()
        {
            int i = 1;
            foreach (ReportLine line in ReportList)
            {
                sheet.Cells[i, "A"] = ((CompanyReportLine)line).NN;
                sheet.Cells[i, "B"] = ((CompanyReportLine)line).Name;
                sheet.Cells[i, "C"] = ((CompanyReportLine)line).IP;
                sheet.Cells[i, "C"] = ((CompanyReportLine)line).SummDate;
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
        public override void FillRows()
        {
            int i = 1;
            foreach (ReportLine line in ReportList)
            {
                sheet.Cells[i, "A"] = ((CompanyUserReportLine)line).NN;
                sheet.Cells[i, "B"] = ((CompanyUserReportLine)line).Company;
                sheet.Cells[i, "C"] = ((CompanyUserReportLine)line).QUser;


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
        public override void FillRows()
        {
            int i = 1;
            foreach (ReportLine line in ReportList)
            {
                sheet.Cells[i, "A"] = ((ErrorReportLine)line).NN;
                sheet.Cells[i, "B"] = ((ErrorReportLine)line).Name;
                sheet.Cells[i, "C"] = ((ErrorReportLine)line).Company;
                sheet.Cells[i, "D"] = ((ErrorReportLine)line).IP;
                sheet.Cells[i, "E"] = ((ErrorReportLine)line).ID;
                sheet.Cells[i, "F"] = ((ErrorReportLine)line).InDate;
                sheet.Cells[i, "G"] = ((ErrorReportLine)line).OutDate;
            }
        }
    }

    [Serializable, XmlInclude(typeof(UserReportLine))]
    public abstract class ReportLine { }
    [Serializable, XmlInclude(typeof(IPReportLine))]
    public class UserReportLine : ReportLine
    {
        public int NN;
        public string Name;
        public string Company;
        public string IP;
        public string ID;
        public DateTime InDate;
        public DateTime OutDate;
        public string Error;

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
        public string NN;
        public string Name;
        public string IP;
        public string InDate;
        public string OutDate;
        public int QConnect;

        private LogLine LOG_LINE;

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
        public string NN;
        public string Name;
        public string IP;
        public string SummDate;


        private LogLine LOG_LINE;

        public CompanyReportLine() { }

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
        public int NN;
        public string Company;
        public int QUser;

        private LogLine LOG_LINE;

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
        public int NN;
        public string Name;
        public string Company;
        public string IP;
        public string ID;
        public DateTime InDate;
        public DateTime OutDate;

        private LogLine LOG_LINE;

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