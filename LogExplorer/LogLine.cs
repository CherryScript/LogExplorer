using System;
using System.Text.RegularExpressions;

namespace LogExplorer
{
    public class LogLine
    {
        // TODO: требуется замена на фабрику

        public int NN { get; set; }
        public string Name { get; set; }
        public string Company { get; set; }
        public string IP { get; set; }
        public string ID { get; set; }
        public DateTime InDate { get; set; }
        public DateTime OutDate { get; set; }
        public string Error { get; set; }

        public string notValid { get; set; }


        private string LogString;
        public LogLine(String ls)
        {
            LogString = ls;

            string[] LogLineArray = LogString.Split(';');
            NN = int.Parse(LogLineArray[0]);
            Name = LogLineArray[1];
            Company = LogLineArray[2];

            if(IPisValid(LogLineArray[3]))
                IP = LogLineArray[3];
            else notValid += " IP адрес,";

            ID = LogLineArray[4];

            if (DateIsValid(LogLineArray[5]))
                InDate = DateTime.Parse(LogLineArray[5]);
            else notValid += " Дата входа,";
            if (DateIsValid(LogLineArray[6]))
                OutDate = DateTime.Parse(LogLineArray[6]);
            else notValid += " Дата выхода";

            Error = LogLineArray[7];

        }

        public bool IPisValid(string ipString)
        {
            Regex check = new Regex(@"\b(?:\d{1,3}\.){3}\d{1,3}\b");
            return check.IsMatch(ipString);    
        }
        public bool DateIsValid(string dateString) {
            Regex check = new Regex(@"(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d ([0-9]|[0-1][0-9])|(2[0-3])(:[0-5][0-9]){2}$");
            return check.IsMatch(dateString);

        }

    }


}
