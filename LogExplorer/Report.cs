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
            reportList = logLineList.FindAll(ll => ll.OutDate > setTime.AddHours(-24)&& ll.OutDate< setTime);




        }
    }
    class IPReport : Report
    {
        public IPReport(List<LogLine> logLineList)
        {
            this.logLineList = logLineList;
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
}