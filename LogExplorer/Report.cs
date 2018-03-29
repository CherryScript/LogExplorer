using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using Excel = Microsoft.Office.Interop.Excel;

namespace LogExplorer
{
    public abstract class Report
    {
        public List<ReportLine> ReportList;
        public string FileName;
        public abstract void CreateReport(List<LogLine> logLineList);

        internal void Write(string filePath)
        {
            //WriteToXML<List<ReportLine>>(ReportList, filePath);
            WriteToXLS<List<ReportLine>>(ReportList, filePath);
        }

        internal void WriteToXML<T>(T rl, string filePath)
        {
            XmlDocument xmlDoc = new XmlDocument();
            XPathNavigator nav = xmlDoc.CreateNavigator();
            using (XmlWriter writer = nav.AppendChild())
            {
                XmlSerializer ser = new XmlSerializer(typeof(T), new XmlRootAttribute("TheRootElementName"));
                ser.Serialize(writer, rl);
            }
            File.WriteAllText(filePath, xmlDoc.InnerXml);
        }

        private void WriteToXLS<T>(T rl, string filePath)
        {
            System.Reflection.Missing missingValue = System.Reflection.Missing.Value;

            Excel.Application application;
            Excel.Workbook book;
            Excel.Worksheet sheet;

            application = new Microsoft.Office.Interop.Excel.Application();
            book = application.Workbooks.Add(missingValue);
            sheet = (Excel.Worksheet)book.Worksheets.get_Item(1);


            int i = 1;
            foreach (ReportLine line in ReportList)
            {
                i++;
             //   sheet.Cells[i, "A"] = ((typeof(T))line);

            }

                      
            book.SaveAs(@"C:\excel_text.xls",
                         Excel.XlFileFormat.xlExcel12,
                         missingValue,
                         missingValue,
                         missingValue,
                         missingValue,
                         Excel.XlSaveAsAccessMode.xlNoChange,
                         missingValue,
                         missingValue,
                         missingValue,
                         missingValue,
                         missingValue);
                                                                             
            book.Close(true, missingValue, missingValue);
            application.Quit();
   
            System.Runtime.InteropServices.Marshal.ReleaseComObject(sheet);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(book);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(application);


        }
    }

    class UserReport : Report
    {
        private ReportFactory factory;
        public UserReport(ReportFactory factory) { this.factory = factory; }

        public override void CreateReport(List<LogLine> logLineList)
        {
            ReportList = new List<ReportLine>();
            DateTime setTime = new DateTime(2017, 7, 6, 00, 00, 00);

            List<LogLine> findList = logLineList.FindAll(ll => ll.OutDate > setTime.AddHours(-24) && ll.OutDate < setTime);
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
        public override void CreateReport(List<LogLine> logLineList)
        {
            ReportList = new List<ReportLine>();
            DateTime startTime = new DateTime(2017, 6, 18, 00, 00, 00);
            DateTime endTime = new DateTime(2017, 6, 21, 00, 00, 00);

            List<LogLine> findList = logLineList.FindAll(ll => ll.OutDate > startTime && ll.OutDate < endTime);
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
        public override void CreateReport(List<LogLine> logLineList)
        {
            ReportList = new List<ReportLine>();
            List<LogLine> findList = logLineList.FindAll(ll => ll.Company == "РНПК");

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
        public override void CreateReport(List<LogLine> logLineList)
        {
            ReportList = new List<ReportLine>();

            DateTime startTime = new DateTime(2017, 6, 18, 00, 00, 00);
            DateTime endTime = new DateTime(2017, 6, 21, 00, 00, 00);

            List<LogLine> findList = logLineList.FindAll(ll => ll.OutDate > startTime && ll.OutDate < endTime);

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
            // reportList.Add(factory.CreateLine(line));
        }
    }
    class ErrorReport : Report
    {
        private ReportFactory factory;
        public ErrorReport(ReportFactory factory) { this.factory = factory; }

        public override void CreateReport(List<LogLine> logLineList)
        {
            ReportList = new List<ReportLine>();
            DateTime startTime = new DateTime(2017, 6, 18, 00, 00, 00);
            DateTime endTime = new DateTime(2017, 6, 21, 00, 00, 00);

            List<LogLine> findList = logLineList.FindAll(ll => ll.OutDate > startTime && ll.OutDate < endTime && ll.Error.Equals("1"));
            Dictionary<string, int> dict = new Dictionary<string, int>();

            foreach (LogLine line in findList)
            {
                ReportList.Add(factory.CreateLine(line));
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
            return new IPReportLine(ll);
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









    //  abstract class Report
    //  {
    //      public List<LogLine> logLineList;
    //public List<IReportLine> reportList;

    //      internal void Write(string filePath)
    //      {
    //          WriteXML<List<IReportLine>>(reportList, filePath);
    //      }


    //      internal void WriteXML<T>(T rl, string filePath)
    //      {
    //          XmlDocument xmlDoc = new XmlDocument();
    //          XPathNavigator nav = xmlDoc.CreateNavigator();
    //          using (XmlWriter writer = nav.AppendChild())
    //          {
    //              XmlSerializer ser = new XmlSerializer(typeof(IReportLine), new XmlRootAttribute("TheRootElementName"));
    //              ser.Serialize(writer, rl);
    //          }
    //          File.WriteAllText(filePath, xmlDoc.InnerXml);
    //      }

    //  }
    //  public interface IReportLine {}

    //  class UserReport : Report
    //  {
    //      public UserReport(List<LogLine> logLineList)
    //      {
    //	DateTime setTime = new DateTime(2017, 7, 6, 00, 00, 00);
    //	this.logLineList = logLineList;
    //	DateTime startTime = setTime.AddHours (-24);

    //	reportList = new List<IReportLine> ();
    //	reportList.Add (UserReportLine.GetHeader());
    //	foreach (LogLine line in logLineList) {
    //		if (line.OutDate <= startTime || line.OutDate >= setTime) { continue; }
    //		reportList.Add(UserReportLine.Obtain(line));
    //	}
    //	//            reportList = logLineList.FindAll(ll => );
    //      }
    //class UserReportLine : IReportLine {
    //	private string NN;
    //          private string Name;
    //          private string IP;
    //	private string InDate;
    //	private string OutDate;
    //          private string QConnect;

    //          public static string NN_HEADER = "Порядковый номер";
    //          public static string NAME_HEADER = "Пользователь";
    //          public static string IP_HEADER = "IP адрес";
    //          public static string INDATE_HEADER = "Дата входа";
    //	public static string OUTDATE_HEADER = "Дата выхода";


    //	private static UserReportLine _header;

    //	public static UserReportLine GetHeader() {
    //		if (_header == null) {
    //			_header = new UserReportLine();
    //			_header.NN = NN_HEADER;
    //                  _header.Name = NAME_HEADER;
    //                  _header.InDate = INDATE_HEADER;
    //			_header.OutDate = OUTDATE_HEADER;
    //			_header.IP = IP_HEADER;

    //		}
    //		return _header;
    //	}

    //	public static UserReportLine Obtain(LogLine line) {
    //		UserReportLine toReturn = new UserReportLine ();
    //              toReturn.NN = line.NN.ToString();
    //		toReturn.InDate = line.InDate.ToString();
    //		toReturn.OutDate = line.OutDate.ToString();
    //		toReturn.IP = line.IP;
    //		toReturn.Name = line.Name;
    //		return toReturn;
    //	}
    //}
    //  }

    //  class IPReport : Report
    //  {
    //      public IPReport(List<LogLine> logLineList)
    //      {
    //          this.logLineList = logLineList;
    //          DateTime startTime = new DateTime(2017, 6, 18, 00, 00, 00);
    //          DateTime endTime = new DateTime(2017, 6, 21, 00, 00, 00);
    //          string IP = "192.168.13.1";

    //         // reportList = logLineList.FindAll(ll => ll.OutDate > startTime && ll.OutDate < endTime && ll.IP.Equals(IP));
    //      }
    //  }

    //  class OrgReport : Report
    //  {
    //      public OrgReport(List<LogLine> logLineList)
    //      {
    //          this.logLineList = logLineList;
    //      }
    //  }

    //  class OrgUserReport : Report
    //  {
    //      public OrgUserReport(List<LogLine> logLineList)
    //      {
    //          this.logLineList = logLineList;
    //      }
    //  }

    //  class ErrorReport : Report
    //  {
    //      public ErrorReport(List<LogLine> logLineList)
    //      {
    //          this.logLineList = logLineList;
    //      }
    //  }
}