using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace LogExplorer
{
    abstract class Report
    {
        public List<LogLine> logLineList;
		public List<IReportLine> reportList;

        internal void Write(string filePath)
        {
            WriteXML<List<IReportLine>>(reportList, filePath);
        }


        internal void WriteXML<T>(T rl, string filePath)
        {
            XmlDocument xmlDoc = new XmlDocument();
            XPathNavigator nav = xmlDoc.CreateNavigator();
            using (XmlWriter writer = nav.AppendChild())
            {
                XmlSerializer ser = new XmlSerializer(typeof(IReportLine), new XmlRootAttribute("TheRootElementName"));
                ser.Serialize(writer, rl);
            }
            File.WriteAllText(filePath, xmlDoc.InnerXml);
        }

        //Сюда можно запихнуть общие для всех лайнов методы. Ну, типа компаратора, который при сортировке будет срабатывать


    }
    public interface IReportLine {}

    class UserReport : Report
    {
        public UserReport(List<LogLine> logLineList)
        {
			DateTime setTime = new DateTime(2017, 7, 6, 00, 00, 00);
			this.logLineList = logLineList;
			DateTime startTime = setTime.AddHours (-24);

			reportList = new List<IReportLine> ();
			reportList.Add (UserReportLine.GetHeader());
			foreach (LogLine line in logLineList) {
				if (line.OutDate <= startTime || line.OutDate >= setTime) { continue; }
				reportList.Add(UserReportLine.Obtain(line));
			}
			//            reportList = logLineList.FindAll(ll => );
        }
		class UserReportLine : IReportLine {
			//Я не помню, какие тут должны быть поля, даю наобум. Поправь есичо
			private string NN;
            private string Name;
            private string IP;
			private string InDate;
			private string OutDate;
            private string QConnect;

            //Не забудь сменить, ага?
            public static string NN_HEADER = "Порядковый номер";
            public static string NAME_HEADER = "Пользователь";
            public static string IP_HEADER = "IP адрес";
            public static string INDATE_HEADER = "Дата входа";
			public static string OUTDATE_HEADER = "Дата выхода";
			

			private static UserReportLine _header;

			public static UserReportLine GetHeader() {
				if (_header == null) {
					_header = new UserReportLine();
					_header.NN = NN_HEADER;
                    _header.Name = NAME_HEADER;
                    _header.InDate = INDATE_HEADER;
					_header.OutDate = OUTDATE_HEADER;
					_header.IP = IP_HEADER;
					
				}
				return _header;
			}

			public static UserReportLine Obtain(LogLine line) {
				UserReportLine toReturn = new UserReportLine ();
                toReturn.NN = line.NN.ToString();
				toReturn.InDate = line.InDate.ToString();
				toReturn.OutDate = line.OutDate.ToString();
				toReturn.IP = line.IP;
				toReturn.Name = line.Name;
				return toReturn;
			}
		}
    }

    class IPReport : Report
    {
        public IPReport(List<LogLine> logLineList)
        {
            this.logLineList = logLineList;
            DateTime startTime = new DateTime(2017, 6, 18, 00, 00, 00);
            DateTime endTime = new DateTime(2017, 6, 21, 00, 00, 00);
            string IP = "192.168.13.1";

           // reportList = logLineList.FindAll(ll => ll.OutDate > startTime && ll.OutDate < endTime && ll.IP.Equals(IP));
        }
    }

    class OrgReport : Report
    {
        public OrgReport(List<LogLine> logLineList)
        {
            this.logLineList = logLineList;
        }
    }

    class OrgUserReport : Report
    {
        public OrgUserReport(List<LogLine> logLineList)
        {
            this.logLineList = logLineList;
        }
    }

    class ErrorReport : Report
    {
        public ErrorReport(List<LogLine> logLineList)
        {
            this.logLineList = logLineList;
        }
    }


















    //абстрактная фабрика, сериализатор ругается на абстракцию

    //class Report
    //{
    //    public ReportLine rl;

    //    public List<ReportLine> reportList;
    //    public List<LogLine> logLineList;
    //    private ReportFactory factory;
    //    public Report(ReportFactory factory) {
    //        this.factory = factory;
    //    }

    //    public void CreateReport(List<LogLine>  logLineList)
    //    {
    //        reportList = new List<ReportLine>();
    //        foreach (var ll in logLineList)
    //        {
    //            reportList.Add(factory.CreateLine(ll));
    //        }
    //    }

    //    internal void WriteToXML(string filePath)
    //    {
    //        XmlDocument xmlDoc = new XmlDocument();
    //        XPathNavigator nav = xmlDoc.CreateNavigator();
    //        using (XmlWriter writer = nav.AppendChild())
    //        {
    //            XmlSerializer ser = new XmlSerializer(typeof(List<ReportLine>), new XmlRootAttribute("TheRootElementName"));
    //            ser.Serialize(writer, reportList);
    //        }
    //        File.WriteAllText(filePath, xmlDoc.InnerXml);
    //    }

    //}

    //abstract class ReportLine {
    //  public abstract void CreateLine(LogLine ll);
    //}

    //class IPReportLine:ReportLine
    //{
    //    public string Name { get; set; }
    //    public string IP { get; set; }
    //    public DateTime InDate { get; set; }
    //    public DateTime OutDate { get; set; }

    //    public override void CreateLine(LogLine ll) {
    //        DateTime startTime = new DateTime(2017, 6, 18, 00, 00, 00);
    //        DateTime endTime = new DateTime(2017, 6, 21, 00, 00, 00);
    //        string IP = "192.168.13.1";


    //        this.Name = "";
    //        this.IP = "192.168.13.1";
    //        this.InDate = new DateTime(2017, 6, 18, 00, 00, 00);
    //        this.OutDate = new DateTime(2017, 6, 21, 00, 00, 00);

    //    }
    //}


    //abstract class ReportFactory
    //{
    //    public abstract ReportLine CreateLine(LogLine ll);
    //}

    //class IPReportFactory : ReportFactory
    //{
    //    public override ReportLine CreateLine(LogLine ll) {

    //        return new IPReportLine();
    //    }
    //}









}