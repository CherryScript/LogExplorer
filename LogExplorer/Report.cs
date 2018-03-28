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
        public List<LogLine> reportList;

        internal void WriteToXML(string filePath)
        {
            XmlDocument xmlDoc = new XmlDocument();
            XPathNavigator nav = xmlDoc.CreateNavigator();
            using (XmlWriter writer = nav.AppendChild())
            {
                XmlSerializer ser = new XmlSerializer(typeof(List<LogLine>), new XmlRootAttribute("TheRootElementName"));
                ser.Serialize(writer, reportList);
            }
            File.WriteAllText(filePath, xmlDoc.InnerXml);
        }
    }

    class UserReport : Report
    {
        public UserReport(List<LogLine> logLineList)
        {
            DateTime setTime = new DateTime(2017, 7, 6, 00, 00, 00);
            this.logLineList = logLineList;
            reportList = logLineList.FindAll(ll => ll.OutDate > setTime.AddHours(-24) && ll.OutDate < setTime);
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

            reportList = logLineList.FindAll(ll => ll.OutDate > startTime && ll.OutDate < endTime && ll.IP.Equals(IP));
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


















    //фабрика

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