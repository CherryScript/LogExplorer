using System;

public class LogLine
{

    // TODO: требуется замена на фабрику

    public int id { get; set; }
    public string Name { get; set; }
    public string Company { get; set; }
    public string IP { get; set; }
    public string Hash { get; set; }
    public DateTime InDate { get; set; }
    public DateTime OutDate { get; set; }
    public bool Error { get; set; }


    public LogLine(String LogString)
    {
        string[] LogLineArray = LogString.Split(";");
        Name = LogLineArray[1];
    }
    public static Parse(String LogString)
    {





    }
}
