using System.Configuration;

namespace LogExplorer
{
    class LogColumnsConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("Columns")]
        public ColumnsCollection ColumnItems
        {
            get { return ((ColumnsCollection)(base["Columns"])); }
        }
    }

    [ConfigurationCollection(typeof(ColumnElement))]
    public class ColumnsCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ColumnElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ColumnElement)(element)).ColumnName;
        }

        public ColumnElement this[int idx]
        {
            get { return (ColumnElement)BaseGet(idx); }
        }
    }

    public class ColumnElement : ConfigurationElement
    {

        [ConfigurationProperty("columnName", DefaultValue = "", IsKey = true, IsRequired = true)]
        public string ColumnName
        {
            get { return ((string)(base["columnName"])); }
            set { base["columnName"] = value; }
        }

        [ConfigurationProperty("paramString", DefaultValue = "", IsKey = false, IsRequired = false)]
        public string ParamString
        {
            get { return ((string)(base["paramString"])); }
            set { base["paramString"] = value; }
        }
    }
}
