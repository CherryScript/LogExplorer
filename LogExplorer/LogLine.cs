using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace LogExplorer
{
    public class LogLine : INotifyPropertyChanged
    {
        public int NN { get; set; }
        public string Name { get; set; }

        private string company;
        public string Company
        {
            get { return company; }
            set
            {
                company = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Company"));
            }
        }



        public string IP { get; set; }
        public string ID { get; set; }
        public DateTime InDate { get; set; }
        public DateTime OutDate { get; set; }
        public string Error { get; set; }
        public string NotValid { get; set; }


        public string StringColumn_1 { get; set; }
        public string StringColumn_2 { get; set; }
        public string StringColumn_3 { get; set; }
        public DateTime DateColumn_4 { get; set; }
        public DateTime DateColumn_5 { get; set; }



        public LogLine() { }

        public event PropertyChangedEventHandler PropertyChanged;


        public LogLine(Dictionary<string,string> dict)
        {
            StringColumn_1 = dict["StringColumn_1"];
            StringColumn_2 = dict["StringColumn_2"];
            StringColumn_3 = dict["StringColumn_3"];



        }

        private string LogString;
        public LogLine(String ls)
        {
            LogString = ls;




            string[] LogLineArray = LogString.Split(';');

            // prop string {time: "", "process": "task", ....... }
            // for(prop in propStringArray) if()string new StringCell() if(date) new DateCell()


            NN = int.Parse(LogLineArray[0]);
            Name = LogLineArray[1];
            Company = LogLineArray[2];
            ID = LogLineArray[4];
            Error = LogLineArray[7];


            //valid check
            if (IPisValid(LogLineArray[3]))
                IP = LogLineArray[3];
            else NotValid += " IP адрес,";
            if (DateIsValid(LogLineArray[5]))
                InDate = DateTime.Parse(LogLineArray[5]);
            else NotValid += " Дата входа,";
            if (DateIsValid(LogLineArray[6]))
                OutDate = DateTime.Parse(LogLineArray[6]);
            else NotValid += " Дата выхода";
            if(NotValid != null)
                NotValid = "Некоректные данные:"+ NotValid;

            
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
