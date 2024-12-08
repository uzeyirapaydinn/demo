using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data;
using System.Text;
using System.Xml;
using System.IO;


namespace QuickCode.Demo.Portal.Helpers
{
    public class DataSetReader
    {
        public DataSet data;

        /// <summary>
        /// Initializes a new instance of the DataSetReader class 
        /// </summary>
        /// <param name="data">Data set element</param>
        public DataSetReader(DataSet data)
        {
            this.data = data;
        }

        public DataSetReader(DataTable data)
        {
            this.data = new DataSet();
            this.data.Tables.Add(data.Copy());
        }



        public DataSetReader(object data)
        {
            if (data.GetType() == typeof(DataSet))
            {
                this.data = data as DataSet;
            }
            else if (data.GetType() == typeof(DataTable))
            {
                this.data = new DataSet();
                this.data.Tables.Add((data as DataTable).Copy());
            }
            else
            {
                DataSet dsData = new DataSet();
                string s = (data as XmlNode).InnerXml;

                s = string.Format("<{0}>{1}</{0}>", "XmlValue", s);
                StringReader sr = new StringReader(s);
                dsData.ReadXml(sr);

                this.data = dsData;
            }
        }


        /// <summary>
        /// Initializes a new instance of the DataSetReader class 
        /// </summary>
        /// <param name="data">Data set element</param>
        public DataSetReader(XmlNode dataNode)
        {
            DataSet data = new DataSet();
            string s = dataNode.InnerXml;
            s = string.Format("<{0}>{1}</{0}>", "XmlValue", s);
            StringReader sr = new StringReader(s);
            data.ReadXml(sr);
            this.data = data;
        }

        public int GetRowCount(string tableName)
        {
            if (this.data.Tables.Contains(tableName))
            {
                return this.data.Tables[tableName].Rows.Count;
            }

            return 0;
        }

        public string GetValueList(string tableName, Dictionary<string, string> columnNames, string listItemName, string listName)
        {
            StringBuilder sb = new StringBuilder();
            if (this.data.Tables.Contains(tableName))
            {
                DataTable dt = this.data.Tables[tableName];

                foreach (DataRow dr in dt.Rows)
                {
                     sb.AppendLine(string.Format("<{0}>", listName));
                    foreach (string colName in columnNames.Keys)
                    {
                         sb.AppendLine(string.Format("<{0}>", listItemName));
                        if (dt.Columns.Contains(colName))
                        {
                             sb.Append(string.Format("<{0}>{1}</{1}>", columnNames[colName], dr[colName]));
                        }

                         sb.AppendLine(string.Format("</{0}>", listItemName));
                    }

                     sb.AppendLine(string.Format("</{0}>", listName));
                }

                return  sb.ToString();
            }

            return string.Empty;
        }

        public string GetValue(string tableName, string columnName, string displayName, string value)
        {
            if (this.data.Tables.Contains(tableName))
            {
                DataTable dt = this.data.Tables[tableName];
                DataRow[] drs = dt.Select(string.Format("{0}='{1}'", columnName, value));
                string returnValue = string.Empty;
                if (drs.Length > 0)
                {
                    returnValue = drs[0][columnName].AsString();
                }

                return string.Format("<{0}>{1}</{0}>", displayName, returnValue);
            }

            return string.Empty;
        }

        public string GetValue(string tableName, string columnName, string displayName)
        {
            if (this.data.Tables.Contains(tableName))
            {
                DataTable dt = this.data.Tables[tableName];
                string returnValue = string.Empty;
                if (dt.Rows.Count > 0)
                {
                    if (dt.Columns.Contains(columnName))
                    {
                        returnValue = dt.Rows[0][columnName].AsString();
                    }
                    else
                    {
                        returnValue = string.Empty;
                    }
                }

                return string.Format("<{0}>{1}</{0}>", displayName, returnValue);
            }

            return string.Empty;
        }

        public object GetValue(string tableName, string columnName)
        {
            if (this.data.Tables.Contains(tableName))
            {
                DataTable dt = this.data.Tables[tableName];
                object returnValue = null;
                if (dt.Rows.Count > 0)
                {
                    if (dt.Columns.Contains(columnName))
                    {
                        returnValue = dt.Rows[0][columnName];
                    }
                    else
                    {
                        returnValue = string.Empty;
                    }
                }

                return returnValue;
            }

            return string.Empty;
        }

        public object GetValue(string tableName, string columnName, int index)
        {
            if (this.data.Tables.Contains(tableName))
            {
                DataTable dt = this.data.Tables[tableName];
                object returnValue = null;
                string concatChar = string.Empty;

                if (columnName.Contains("[") && columnName.Contains("]"))
                {
                    int startIndex = columnName.LastIndexOf("[") + 1;
                    int lenght = columnName.LastIndexOf("]") - startIndex;
                    concatChar = columnName.Substring(startIndex, lenght);
                    columnName = columnName.Split(new char[] { '[' })[0];
                }

                if (columnName.Contains("|"))
                {
                    string[] cNames = columnName.Split(new char[] { '|' });
                    for (int i = 0; i < cNames.Length; i++)
                    {
                        returnValue += dt.Rows[index][cNames[i]].AsString() + (cNames[i].Length - 1 == i ? string.Empty : concatChar);
                    }
                }
                else
                {
                    if (dt.Rows.Count > index)
                    {
                        returnValue = dt.Rows[index][columnName];
                    }
                }

                return returnValue;
            }

            return string.Empty;
        }
    }
}