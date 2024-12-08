using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Concurrent;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Globalization;
using System.Data;
using System.Collections;
using System.Xml.Linq;
using System.Linq.Expressions;
using System.IO.Compression;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Drawing;
using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using QuickCode.Demo.Portal.Models;

namespace QuickCode.Demo.Portal.Helpers
{
    public static class ExtensionLibrary
    {
        #region Extension Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Description(this Enum value)
        {
            var enumType = value.GetType();
            var field = enumType.GetField(value.ToString());
            var descriptionAttr = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
            var enumMemberAttr = field.GetCustomAttributes(typeof(EnumMemberAttribute), false);
            if (descriptionAttr.Length > 0 || enumMemberAttr.Length > 0)
            {
                if (descriptionAttr.Length > 0)
                    return ((DescriptionAttribute)descriptionAttr[0]).Description;
                else if (enumMemberAttr.Length > 0)
                    return ((EnumMemberAttribute)enumMemberAttr[0]).Value;
            }

            return value.ToString();

        }

        /// <summary>
        /// Object -> String : Object null ise string.Empty döner
        /// </summary>
        /// <param name="obj">object param</param>
        /// <returns>string object</returns>
        public static string AsString(this object obj)
        {
            if (obj == null)
            {
                obj = string.Empty;
            }

            return obj.ToString();
        }

        public static string SplitCamelCaseToString(this string source)
        {
            const string pattern = @"[A-Z][a-z]*|[a-z]+|\d+";
            var matches = Regex.Matches(source, pattern);
            return String.Join(" ", matches);
        }

        public static IEnumerable<string> SplitCamelCase(this string source)
        {
            const string pattern = @"[A-Z][a-z]*|[a-z]+|\d+";
            var matches = Regex.Matches(source, pattern);
            foreach (Match match in matches)
            {
                yield return match.Value;
            }
        }

        public static string ToJoin<T>(this List<T> obj, string separator)
        {
            object[] objArr = new object[obj.Count];
            for (int i = 0; i < obj.Count; i++)
            {
                objArr[i] = obj[i];
            }

            return string.Join("|", obj.ToArray());
        }

        public static List<T> ToList<T>(this string obj, char separator)
        {
            List<T> list = new List<T>();
            if (obj != null && obj.Length > 0)
            {
                foreach (string s in obj.Split(new char[] { separator }))
                {
                    list.Add((T)Convert.ChangeType(s, typeof(T)));
                }
            }

            return list;
        }

        public static void Put<T>(this ITempDataDictionary tempData, string key, T value) where T : class
        {
            tempData[key] = JsonConvert.SerializeObject(value);
        }

        public static T Get<T>(this ITempDataDictionary tempData, string key) where T : class
        {
            object o;
            tempData.TryGetValue(key, out o);
            return o == null ? null : JsonConvert.DeserializeObject<T>((string)o);
        }

        public static IEnumerable<SelectListItem> SetListWithSelected(this IEnumerable<SelectListItem> list,
            object value)
        {
            List<SelectListItem> returnItem = new List<SelectListItem>();
            foreach (var item in list)
            {
                returnItem.Add(new SelectListItem()
                {
                    Selected = (value.AsString() == item.Value),
                    Value = item.Value,
                    Text = item.Text
                });
            }

            return returnItem;
        }

        /// <summary>
        /// Get xmlDocument inner text
        /// </summary>
        /// <param name="obj">XmlDocument object</param>
        /// <param name="path">XMl node path</param>
        /// <returns></returns>
        public static string GetInnerText(this XmlDocument obj, string path)
        {
            if (obj != null)
            {
                return obj.SelectSingleNode(path).InnerText;
            }

            return string.Empty;
        }



        public static IEnumerable<string> SplitOnCapitals(this string text)
        {
            Regex regex = new Regex(@"\p{Lu}\p{Ll}*");
            foreach (Match match in regex.Matches(text))
            {
                yield return match.Value;
            }
        }

        public static string AsSplitCapitalWithUnderline(this string text)
        {
            StringBuilder sb = new StringBuilder();
            var list = SplitOnCapitals(text).ToArray();
            return string.Join("_", list);
        }

        public static string GetPascalCase(this string name)
        {
            name = name.AsString().ToLower(CultureInfo.CreateSpecificCulture("en-US"));
            return Regex.Replace(name, @"^\w|_\w",
                (match) => match.Value.Replace("_", "").ToUpper(CultureInfo.CreateSpecificCulture("en-US")));
        }


        public static string Check<T>(Expression<Func<T>> expr)
        {
            var body = ((MemberExpression)expr.Body);
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Name is: {0}", body.Member.Name);
            sb.AppendFormat("Value is: {0}", ((FieldInfo)body.Member)
                .GetValue(((ConstantExpression)body.Expression).Value));
            return sb.ToString();
        }

        public static string GetEnumName(this object obj)
        {
            Type objType = obj.GetType();
            if (objType.IsEnum)
            {
                return Enum.GetName(objType, obj);
            }

            return string.Empty;
        }

        public static bool IsInList(this string obj, string[] list)
        {
            for (int i = 0; i < list.Length; i++)
            {
                if (list[i] == obj)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsIn<T>(this T obj, params T[] list)
        {
            if (obj != null)
            {
                for (int i = 0; i < list.Length; i++)
                {
                    if (list[i].ToString() == obj.ToString())
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static T ToValue<T>(this bool obj, T ifTrue, T ifFalse)
        {
            if (obj)
            {
                return ifTrue;
            }

            return ifFalse;
        }

        /// <summary>
        /// Object -> String : Object null ise string.Empty döner
        /// </summary>
        /// <param name="obj">object param</param>
        /// <returns>string object</returns>
        public static string AsString(this object obj, string defaultValue)
        {
            if (string.IsNullOrEmpty(obj.AsString()))
            {
                obj = defaultValue;
            }

            return obj.ToString();
        }

        /// <summary>
        /// Object -> Int32
        /// </summary>
        /// <param name="obj">object param</param>
        /// <returns>integer object</returns>
        public static int AsInt32(this object obj)
        {
            return Convert.ToInt32(obj);
        }

        /// <summary>
        /// Object -> Int32?
        /// </summary>
        /// <param name="obj">object param</param>
        /// <returns>integer object</returns>
        public static int? AsInt32Nullable(this object obj)
        {
            int? returnValue = null;

            if (obj == null)
            {
                obj = string.Empty;
            }

            if (obj.ToString() != string.Empty)
            {
                returnValue = Convert.ToInt32(obj);
            }

            return returnValue;
        }


        /// Object -> Int64?
        /// </summary>
        /// <param name="obj">object param</param>
        /// <returns>integer object</returns>
        public static long? AsInt64Nullable(this object obj)
        {
            long? returnValue = null;

            if (obj == null)
            {
                obj = string.Empty;
            }

            if (obj.ToString() != string.Empty)
            {
                returnValue = Convert.ToInt64(obj);
            }

            return returnValue;
        }

        public static Dictionary<string, Type> htDynamicTypes = new Dictionary<string, Type>();

        public static ConcurrentDictionary<string, Assembly>
            assemblyList = new ConcurrentDictionary<string, Assembly>();

        public static Type GetTypeFromGlobalAssemblyList(this string obj)
        {
            Type reconItemType = null;
            if (!htDynamicTypes.ContainsKey(obj))
            {
                bool flag = false;
                foreach (Assembly asmb in assemblyList.Values)
                {
                    Type t = asmb.GetType(obj);
                    if (t != null)
                    {
                        reconItemType = t;
                        htDynamicTypes.Add(obj, t);
                        flag = true;
                        break;
                    }
                }

                if (!flag)
                {
                    foreach (Assembly asmb in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        Type t = asmb.GetType(obj);
                        if (t != null)
                        {
                            reconItemType = t;
                            htDynamicTypes.Add(obj, t);
                            break;
                        }
                    }
                }
            }
            else
            {
                reconItemType = htDynamicTypes[obj];
            }

            return reconItemType;
        }


        /// <summary>
        /// Object -> Int32
        /// </summary>
        /// <param name="obj">object param</param>
        /// <returns>integer object</returns>
        public static int AsInt32(this DataRow obj, string objectName)
        {
            try
            {
                return obj[objectName].AsInt32();
            }
            catch (Exception ex)
            {
                string exMessage = string.Format("{0} Could not convert to Type = {1} ObjectValue={2}\nException={3}",
                    objectName, System.Reflection.MethodBase.GetCurrentMethod().Name, obj[objectName], ex.Message);
                throw new Exception(exMessage);
            }
        }

        /// <summary>
        /// Object -> Int32
        /// </summary>
        /// <param name="obj">object param</param>
        /// <returns>integer object</returns>
        public static Type AsType(this DataRow obj, string objectName)
        {
            try
            {
                return Type.GetType(obj[objectName].AsString());
            }
            catch (Exception ex)
            {
                string exMessage = string.Format("{0} Could not convert to Type = {1} ObjectValue={2}\nException={3}",
                    objectName, System.Reflection.MethodBase.GetCurrentMethod().Name, obj[objectName], ex.Message);
                throw new Exception(exMessage);
            }
        }


        public static object GetValue(this DataRow obj, string columnName, object value)
        {
            object returnObject = value;

            if (obj.Table.Columns.Contains(columnName))
            {
                if (value.Equals(-1))
                {
                    returnObject = obj[columnName];
                }
            }

            return returnObject;

        }

        /// <summary>
        /// As Time Ticks
        /// </summary>
        /// <param name="obj">Stopwatch param</param>
        /// <returns>time tick obj</returns>
        public static double AsTimeTicks(this Stopwatch obj)
        {
            return (double)(1000000000 * obj.ElapsedTicks / Stopwatch.Frequency) / 1000000;
        }

        /// <summary>
        /// As Time Ticks
        /// </summary>
        /// <param name="obj">Stopwatch param</param>
        /// <returns>time tick obj</returns>
        public static long GetValue(this Stopwatch obj, ref long lastValue)
        {
            lastValue = obj.ElapsedMilliseconds - lastValue;
            return lastValue;
        }

        /// <summary>
        /// Object -> Int64
        /// </summary>
        /// <param name="obj">object param</param>
        /// <returns>returns long</returns>
        public static long AsInt64(this object obj)
        {
            return Convert.ToInt64(obj);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static long TryInt64(this object obj)
        {
            long result;
            long.TryParse(obj.ToString(), out result);
            return result;
        }

        /// <summary>
        /// Object -> Int32
        /// </summary>
        /// <param name="obj">object param</param>
        /// <returns>integer object</returns>
        public static long AsInt64(this DataRow obj, string objectName)
        {
            try
            {
                return obj[objectName].AsInt64();
            }
            catch (Exception ex)
            {
                string exMessage = string.Format("{0} Could not convert to Type = {1} ObjectValue={2}\nException={3}",
                    objectName, System.Reflection.MethodBase.GetCurrentMethod().Name, obj[objectName], ex.Message);
                throw new Exception(exMessage);
            }
        }

        /// <summary>
        /// Object -> Decimal
        /// </summary>
        /// <param name="obj">object param</param>
        /// <returns>returns decimal</returns>
        public static decimal AsDecimal(this object obj)
        {
            decimal returnValue = 0;
            try
            {
                if (obj.GetType() == typeof(string))
                {
                    string NumberDecimalSeparator = ".";
                    if (obj.AsString().Contains(","))
                    {
                        NumberDecimalSeparator = ",";
                    }

                    NumberFormatInfo info = new NumberFormatInfo();
                    info.NumberDecimalSeparator = NumberDecimalSeparator;
                    returnValue = Convert.ToDecimal(obj, info);
                }
                else
                {
                    returnValue = Convert.ToDecimal(obj);
                }
            }
            catch
            {

            }

            return returnValue;
        }

        /// <summary>
        /// Object -> Double
        /// </summary>
        /// <param name="obj">object param</param>
        /// <returns>returns double</returns>
        public static double AsDouble(this object obj)
        {
            double returnValue = 0;
            try
            {
                if (obj.GetType() == typeof(string))
                {
                    string NumberDecimalSeparator = ".";
                    if (obj.AsString().Contains(","))
                    {
                        NumberDecimalSeparator = ",";
                    }

                    NumberFormatInfo info = new NumberFormatInfo();
                    info.NumberDecimalSeparator = NumberDecimalSeparator;
                    returnValue = Convert.ToDouble(obj, info);
                }
                else
                {
                    returnValue = Convert.ToDouble(obj);
                }
            }
            catch
            {

            }

            return returnValue;
        }

        /// <summary>
        /// Object -> AsDouble
        /// </summary>
        /// <param name="obj">object param</param>
        /// <returns>integer object</returns>
        public static double AsDouble(this DataRow obj, string objectName)
        {
            try
            {
                return obj[objectName].AsDouble();
            }
            catch (Exception ex)
            {
                string exMessage = string.Format("{0} Could not convert to Type = {1} ObjectValue={2}\nException={3}",
                    objectName, System.Reflection.MethodBase.GetCurrentMethod().Name, obj[objectName], ex.Message);
                throw new Exception(exMessage);
            }
        }

        /// <summary>
        /// Object -> Date (Saat değeri bulunmuyor)
        /// </summary>
        /// <param name="obj">object param</param>
        /// <returns>returns date</returns>
        public static DateTime AsDate(this object obj)
        {
            return Convert.ToDateTime(obj).Date;
        }



        public static DataSet AsDataSet(this string obj)
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            string columnName = "COLUMN_1";
            dt.Columns.Add(columnName);

            DataRow dr = dt.NewRow();
            dr[columnName] = obj;
            ds.Tables.Add(dt);

            return ds;
        }

        public static bool IsInt32(this object obj)
        {
            Int32 val = 0;
            bool isInt32 = false;
            try
            {
                isInt32 = Int32.TryParse(obj.ToString(), out val);

            }
            catch
            {
                isInt32 = false;
            }

            return isInt32;
        }

        public static bool IsInt64(this object obj)
        {
            Int64 val = 0;
            bool isInt64 = false;
            try
            {
                isInt64 = Int64.TryParse(obj.ToString(), out val);

            }
            catch
            {
                isInt64 = false;
            }

            return isInt64;
        }

        public static object AsNull(this object obj)
        {
            object t = obj;

            if (t == DBNull.Value)
            {
                t = null;
            }

            return t;

        }

        /// <summary>
        /// Object -> AsDouble
        /// </summary>
        /// <param name="obj">object param</param>
        /// <returns>integer object</returns>
        public static DateTime AsDate(this DataRow obj, string objectName)
        {
            try
            {
                return obj[objectName].AsDate();
            }
            catch (Exception ex)
            {
                string exMessage = string.Format("{0} Could not convert to Type = {1} ObjectValue={2}\nException={3}",
                    objectName, System.Reflection.MethodBase.GetCurrentMethod().Name, obj[objectName], ex.Message);
                throw new Exception(exMessage);
            }
        }

        /// <summary>
        /// Object -> DateTime
        /// </summary>
        /// <param name="obj">object param</param>
        /// <returns>returns date time</returns>
        public static DateTime AsDateTime(this object obj)
        {
            return Convert.ToDateTime(obj);
        }

        /// <summary>
        /// Object -> AsDouble
        /// </summary>
        /// <param name="obj">object param</param>
        /// <returns>integer object</returns>
        public static DateTime AsDateTime(this DataRow obj, string objectName)
        {
            try
            {
                return obj[objectName].AsDateTime();
            }
            catch (Exception ex)
            {
                string exMessage = string.Format("{0} Could not convert to Type = {1} ObjectValue={2}\nException={3}",
                    objectName, System.Reflection.MethodBase.GetCurrentMethod().Name, obj[objectName], ex.Message);
                throw new Exception(exMessage);
            }
        }

        /// <summary>
        /// byte[] to Assembly
        /// </summary>
        /// <param name="obj">Byte array object</param>
        /// <returns>Assembly object</returns>
        public static Assembly AsAssembly(this byte[] obj)
        {
            return Assembly.Load(obj);
        }

        public static StringBuilder SetString(this StringBuilder obj, string text)
        {
            string s = obj.ToString();
            obj.Clear();
            obj.AppendFormat("{0}{1}", s, text);
            return obj;
        }

        public static string GetObjectDetails(this object o)
        {
            StringBuilder returnString = new StringBuilder();

            if (o != null && o.GetType().IsArray)
            {
                if (o.GetType().FullName == "System.Byte[]")
                {
                    returnString.AppendFormat(string.Format("<{0}>/{0}>", "ByteArray"));
                }
                else
                {
                    Array a = (o as Array);
                    for (int i = 0; i < a.Length; i++)
                    {
                        returnString.Append(string.Format("<{0}{1}>{2}</{0}{1}>", "Index", i,
                            GetObjectDetails(a.GetValue(i))));
                    }

                    returnString.Insert(0, string.Format("<{0}>", "Array"));
                    returnString.AppendFormat(string.Format("</{0}>", "Array"));
                }
            }
            else
            {
                string s = (o == null ? "param_null" : o.AsString());
                returnString.AppendFormat("{0}", s);
            }

            return returnString.ToString();
        }

        public static void SaveAssembly(this byte[] obj, string filepath)
        {
            File.WriteAllBytes(filepath, obj);
        }



        /// <summary>
        /// object to Assembly
        /// </summary>
        /// <param name="obj">sender object</param>
        /// <returns>returns assembly</returns>
        public static Assembly AsAssembly(this object obj)
        {
            if (obj == null)
            {
                return null;
            }

            return Assembly.Load(obj as byte[]);
        }

        /// <summary>
        /// XML serileştirme işlemini gerçekleştirir
        /// </summary>
        /// <param name="source">Serileştirilecek obje</param>
        /// <param name="removeXmlHeader">Removes xml header if true</param>
        /// <returns>Serileştirilmiş data</returns>
        public static string XMLSerialize(this object source, bool removeXmlHeader)
        {
            string serializedObject = null;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                XmlSerializer xmlSerializer = GetXmlSerializer(source.GetType());
                XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);

                xmlSerializer.Serialize(xmlTextWriter, source);

                serializedObject = Encoding.UTF8.GetString(memoryStream.ToArray());

                xmlTextWriter.Close();
            }

            serializedObject = serializedObject.Replace(Convert.ToChar(65279), '<').Replace("<<", "<");

            if (removeXmlHeader)
            {
                serializedObject =
                    serializedObject.Replace("<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n", string.Empty);
                serializedObject = serializedObject.Replace(
                    " xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"",
                    string.Empty);
            }

            return serializedObject;
        }

        /// <summary>
        /// XML de-serileştirme işlemini gerçekleştirir
        /// </summary>
        /// <param name="source">De-Serileştirilecek data</param>
        /// <param name="t">Gönderilen Obje tipi</param>
        /// <returns>De-Serileştirilmiş obje</returns>
        public static object XMLDeserialize(this string source, Type t)
        {
            object deserializedObject;

            using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(source)))
            {
                XmlSerializer xmlSerializer = GetXmlSerializer(t);
                deserializedObject = xmlSerializer.Deserialize(memoryStream);
            }

            return deserializedObject;
        }

        /// <summary>
        /// XML de-serileştirme işlemini gerçekleştirir
        /// </summary>
        /// <param name="source">De-Serileştirilecek data</param>
        /// <param name="t">Gönderilen Obje tipi</param>
        /// <returns>De-Serileştirilmiş obje</returns>
        public static object XMLDeserialize(this byte[] source, Type t)
        {
            object deserializedObject;

            using (MemoryStream memoryStream = new MemoryStream(source))
            {
                XmlSerializer xmlSerializer = GetXmlSerializer(t);
                deserializedObject = xmlSerializer.Deserialize(memoryStream);
            }

            return deserializedObject;
        }




        public static bool IsContains(this string obj, string containString, bool caseSensitive)
        {
            StringComparison compareType = StringComparison.InvariantCultureIgnoreCase;
            if (caseSensitive)
            {
                compareType = StringComparison.InvariantCulture;
            }

            return obj.IndexOf(containString, compareType) >= 0;
        }

        public static bool IsContains(this string obj, string containString)
        {
            return obj.IsContains(containString, false);
        }



        public static byte[] XmlSerializeAsByte(this object obj)
        {
            XmlSerializer ser = GetXmlSerializer(obj.GetType());

            MemoryStream memStream = new MemoryStream();
            ser.Serialize(memStream, obj);
            memStream.Seek(0, System.IO.SeekOrigin.Begin);
            XmlDocument doc = new XmlDocument();
            doc.Load(memStream);
            memStream.Close();

            return memStream.ToArray();
        }

        public static byte[] XmlSerializeAsByte(this object obj, Type type)
        {
            XmlSerializer ser = GetXmlSerializer(type);

            MemoryStream memStream = new MemoryStream();
            ser.Serialize(memStream, obj);
            memStream.Seek(0, System.IO.SeekOrigin.Begin);
            XmlDocument doc = new XmlDocument();
            doc.Load(memStream);
            memStream.Close();

            return memStream.ToArray();
        }

        public static ConcurrentDictionary<Type, XmlSerializer> serializerList =
            new ConcurrentDictionary<Type, XmlSerializer>();

        public static XmlSerializer GetXmlSerializer(Type type)
        {
            XmlSerializer ser = null;


            if (!serializerList.ContainsKey(type))
            {
                List<Type> tlist = new List<Type>();
                for (int i = 0; i < type.GetGenericArguments().Length; i++)
                {
                    tlist.Add(type.GetGenericArguments()[i]);
                    if (type.GetGenericArguments()[i].IsArray)
                    {
                        tlist.Add(type.Assembly.GetType(type.GetGenericArguments()[i].FullName
                            .Replace("[]", string.Empty)));
                    }
                }

                if (tlist.Count == 0)
                {
                    foreach (var t in type.Assembly.GetTypes())
                    {
                        if (!t.FullName!.EndsWith("Exception") &&
                            !t.FullName.EndsWith("Collection") &&
                            t.GetConstructor(Type.EmptyTypes) != null && t != type)
                        {
                            tlist.Add(t);
                        }
                    }
                }

                try
                {
                    ser = new XmlSerializer(type, tlist.ToArray());
                }
                catch
                {
                    ser = new XmlSerializer(type);
                }

                serializerList.TryAdd(type, ser);
                return ser;
            }

            serializerList.TryGetValue(type, out ser);
            return ser;
        }

        /// <summary>
        /// DateTime -> IsoDateTime (string) 
        /// </summary>
        /// <param name="obj">datetime param</param>
        /// <returns>returns iso date time</returns>
        public static string AsIsoDateTime(this DateTime obj)
        {
            return string.Format("{0:s}", obj);
        }

        /// <summary>
        /// string -> int32
        /// </summary>
        /// <param name="obj">object param</param>
        /// <returns>returns integer</returns>
        public static int AsInt32(this string obj)
        {
            return Convert.ToInt32(obj);
        }

        public static int TryInt32(this string obj)
        {
            int result;
            int.TryParse(obj.ToString(), out result);
            return result;
        }

        public static string AsString(this string obj)
        {
            if (obj == null)
            {
                obj = string.Empty;
            }

            return obj;
        }

        public static string AsString(this StringBuilder obj, int maxLenght)
        {
            if (obj.ToString().Length > 32700)
            {
                return obj.ToString().Substring(0, 32700);
            }

            return obj.ToString();
        }

        public static string AsString(this StringBuilder obj)
        {
            return obj.ToString();
        }

        public static string AsString(this object obj, int maxLenght)
        {
            if (obj == null)
            {
                obj = string.Empty;
            }

            if (obj.ToString().Length > 32700)
            {
                return obj.ToString().Substring(0, 32700);
            }

            return obj.ToString();
        }

        public static string AsStringXml(this DataSet obj)
        {
            StringWriter sw = new StringWriter();
            obj.WriteXml(sw, XmlWriteMode.IgnoreSchema);
            return sw.ToString();
        }

        public static string AsString(this int obj)
        {
            return obj.ToString();
        }

        public static string AsString(this double obj)
        {
            return obj.ToString();
        }

        public static string AsString(this float obj)
        {
            return obj.ToString();
        }

        public static bool AsBoolean(this object obj)
        {
            return Convert.ToBoolean(obj);
        }

        public static object AsObject(this object obj)
        {
            return obj == DBNull.Value ? null : obj;
        }

        public static object AsDbObject(this object obj)
        {
            return obj == null ? DBNull.Value : obj;
        }

        /// <summary>
        /// Object -> Int32
        /// </summary>
        /// <param name="obj">object param</param>
        /// <returns>integer object</returns>
        public static bool AsBoolean(this DataRow obj, string objectName)
        {
            try
            {
                return obj[objectName].AsBoolean();
            }
            catch (Exception ex)
            {
                string exMessage = string.Format("{0} Could not convert to Type = {1} ObjectValue={2}\nException={3}",
                    objectName, System.Reflection.MethodBase.GetCurrentMethod().Name, obj[objectName], ex.Message);
                throw new Exception(exMessage);
            }
        }

        public static bool AsBoolean(this string obj)
        {
            return Convert.ToBoolean(obj);
        }

        /// <summary>
        /// Saat ile birlikte ISO->DateTime çevrimi yapar
        /// </summary>
        /// <param name="dateTimeString">ISO formatta tarih bilgisi</param>
        /// <returns>DateTime tipinde tarih bilgisi</returns>
        public static DateTime ConvertFromISOFormatToDateTimeWithTime(this string dateTimeString)
        {
            return new DateTime(Convert.ToInt32(dateTimeString.Substring(0, 4)),
                Convert.ToInt32(dateTimeString.Substring(5, 2)), Convert.ToInt32(dateTimeString.Substring(8, 2)),
                Convert.ToInt32(dateTimeString.Substring(11, 2)), Convert.ToInt32(dateTimeString.Substring(14, 2)),
                Convert.ToInt32(dateTimeString.Substring(17, 2)));
        }

        /// <summary>
        /// Saat olmaksızın ISO->DateTime çevrimi yapar
        /// </summary>
        /// <param name="dateTimeString">ISO formatta tarih bilgisi</param>
        /// <returns>DateTime tipinde tarih bilgisi</returns>
        public static DateTime ConvertFromISOFormatToDateTimeWithoutTime(this string dateTimeString)
        {
            return new DateTime(Convert.ToInt32(dateTimeString.Substring(0, 4)),
                Convert.ToInt32(dateTimeString.Substring(5, 2)), Convert.ToInt32(dateTimeString.Substring(8, 2)));
        }

        /// <summary>
        /// Saat ile birlikte DateTime->ISO çevrimi yapar
        /// </summary>
        /// <param name="dateTime">DateTime tipinde tarih bilgisi</param>
        /// <returns>ISO formatta tarih bilgisi</returns>
        public static string ConvertToISOFormatWithTime(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-ddTHH:mm:ss");
        }

        /// <summary>
        /// Saat olmaksızın DateTime->ISO çevrimi yapar
        /// </summary>
        /// <param name="dateTime">DateTime tipinde tarih bilgisi</param>
        /// <returns>ISO formatta tarih bilgisi</returns>
        public static string ConvertToISOFormatWithoutTime(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-ddT00:00:00");
        }


        /// <summary>
        /// Verilen metni Hash algoritması ile Encrypt eder
        /// </summary>
        /// <param name="source">Encrypt edilecek metin</param>
        /// <returns>Encrypt edilmiş metin</returns>
        public static string EncryptWithHash(this string source)
        {
            source = source.Replace("\0", String.Empty);

            using (var hashAlgorithm = MD5.Create())
            {
                byte[] bytOut = hashAlgorithm.ComputeHash(ASCIIEncoding.ASCII.GetBytes(source));

                return System.Convert.ToBase64String(bytOut, 0, bytOut.Length);
            }
        }

        public static string GetPropertyNameWithIndex(string[] propertyNameList, int index)
        {
            StringBuilder returnString = new StringBuilder();

            for (int i = index + 1; i < propertyNameList.Length; i++)
            {
                returnString.AppendFormat("{0}{1}", i == index ? string.Empty : ".", propertyNameList[i]);
            }

            return returnString.ToString();
        }

        public static string[] Split(this string str, string splitChar)
        {
            char splitChr = ' ';

            if (splitChar.Length > 0)
            {
                splitChr = splitChar[0];
            }

            return str.Split(splitChr);
        }

        public static string[] Split(this string str, char chr)
        {
            string[] slist = str.Split(new char[] { chr }, StringSplitOptions.RemoveEmptyEntries);
            return slist;
        }

        public static string GetPropertyNameWithIndex(this string propertyName, int index)
        {
            string[] propertyNameList = propertyName.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            StringBuilder returnString = new StringBuilder();

            for (int i = index + 1; i < propertyNameList.Length; i++)
            {
                returnString.AppendFormat("{0}{1}", i == index ? string.Empty : ".", propertyNameList[i]);
            }

            return returnString.ToString();
        }

        public static List<string> ToList(this ICollection<string> list)
        {

            List<string> sList = new List<string>();
            foreach (string s in list)
            {
                sList.Add(s);
            }

            sList.Sort();
            return sList;
        }

        public static dynamic ToList(this Array array)
        {
            dynamic list = null;

            var listType = typeof(List<>);
            var constructedListType = listType.MakeGenericType(array.GetType().GetElementType());

            list = Activator.CreateInstance(constructedListType);

            foreach (dynamic s in array)
            {
                list.Add(s);
            }

            return list;
        }

        public static List<string> ToList(this ICollection list)
        {

            List<string> sList = new List<string>();
            foreach (string s in list)
            {
                sList.Add(s);
            }

            sList.Sort();
            return sList;
        }

        public static List<T> ToList<T>(this T item)
        {
            List<T> list = new List<T>();
            list.Add(item);
            return list;
        }


        public static List<T> ToList<T>(this ObservableCollection<T> item)
        {
            return item.ToArray<T>().ToList<T>();
        }


        public static void SetValueToProperty(this object obj, string propertyName, Type objType, object propertyValue)
        {
            if (obj == null)
            {
                obj = objType.Assembly.CreateInstance(objType.FullName);
            }

            string[] propList = propertyName.Split('.');

            for (int i = 0; i < propList.Length; i++)
            {
                string part = propList[i];
                Type type = obj.GetType();
                PropertyInfo info = type.GetProperty(part);
                if (info == null)
                {
                    break;
                }

                if (i == propList.Length - 1)
                {
                    info.SetValue(obj, propertyValue, null);
                }
            }
        }

        public static void SetArrayValueToProperty(this object obj, string propertyName, Type objType,
            object propertyValue, int size, int index)
        {
            Type arrayItemType = null;

            if (!objType.IsArray || index >= size)
            {
                return;
            }

            arrayItemType = objType.GetElementType();

            if (obj == null)
            {
                obj = Array.CreateInstance(arrayItemType, size);
            }

            obj = (obj as Array).GetValue(index);

            if (obj == null)
            {
                obj = objType.Assembly.CreateInstance(arrayItemType.FullName);
            }

            string[] propList = propertyName.Split('.');

            for (int i = 0; i < propList.Length; i++)
            {
                string part = propList[i];
                Type type = obj.GetType();
                PropertyInfo info = type.GetProperty(part);
                if (info == null)
                {
                    break;
                }

                info.SetValue(obj, propertyValue, null);
            }
        }

        public static bool TryParseXml(this string xml)
        {
            try
            {
                XmlDocument xdoc = new XmlDocument();
                xdoc.XmlResolver = null;
                xdoc.LoadXml(xml);
                return true;
            }
            catch (XmlException e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
        }

        public static bool CheckIsValidDataSetType(this Type type, object obj)
        {
            return (type == typeof(DataSet) || type == typeof(XmlNode) || type == typeof(XmlElement) ||
                    type == typeof(DataTable) ||
                    ((type == typeof(string) && obj.ToString().StartsWith("<?xml version"))
                     || (type == typeof(string) && obj.ToString().StartsWith("<message><mti>"))
                     || (type == typeof(string) && obj.ToString().StartsWith("<") &&
                         (obj.ToString().Contains("</") || obj.ToString().Contains(">")) &&
                         obj.ToString().TryParseXml())));
        }

        public static object GetValueFromDataSet(this object obj, string columnName)
        {
            if (obj.GetType() == typeof(XmlElement))
            {
                obj = obj as XmlNode;
            }

            if (obj.GetType() == typeof(string) && (obj.ToString().StartsWith("<?xml version") ||
                                                    obj.ToString().StartsWith("<message><mti>")
                                                    || obj.ToString().StartsWith("<") &&
                                                    (obj.ToString().Contains("</") || obj.ToString().Contains(">")) &&
                                                    obj.ToString().TryParseXml()))
            {
                DataSet dsData = new DataSet();
                StringReader sr = new StringReader(obj.ToString());
                dsData.ReadXml(sr);
                obj = dsData;
            }

            string[] valueInfo = columnName.Split(".");
            DataSetReader dsReader = new DataSetReader(obj);
            if (valueInfo.Length == 2)
            {
                return dsReader.GetValue(valueInfo[0], valueInfo[1]);
            }
            else if (valueInfo.Length == 1 && dsReader.data.Tables.Contains(columnName))
            {
                return dsReader.data.Tables[columnName].Copy();
            }

            return null;
        }


        public static object GetValueFromDataSet(this object obj, string columnName, int index)
        {
            if (obj.GetType() == typeof(XmlElement))
            {
                obj = obj as XmlNode;
            }

            string[] valueInfo = columnName.Split(".");

            if (valueInfo.Length == 2)
            {
                DataSetReader dsReader = new DataSetReader(obj);
                return dsReader.GetValue(valueInfo[0], valueInfo[1], index);
            }

            return null;
        }

        public static string ByteToHexString(this byte[] input)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte s in input)
            {

                sb.AppendFormat("{0:X} ", Convert.ToInt64(s).ToString().PadLeft(2, '0'));
            }

            return sb.ToString();
        }

        public static string ByteToIntString(this byte[] input)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte s in input)
            {

                sb.AppendFormat("{0} ", Convert.ToInt64(s).ToString().PadLeft(3, '0'));
            }

            return sb.ToString();
        }

        public static object GetValueFromProperty(this object obj, string propertyName)
        {
            if (obj == null)
            {
                return null;
            }



            if (obj.GetType().CheckIsValidDataSetType(obj))
            {
                return obj.GetValueFromDataSet(propertyName);
            }

            string[] propList = propertyName.Split('.');

            for (int i = 0; i < propList.Length; i++)
            {
                if (obj == null)
                {
                    return null;
                }

                string part = propList[i];
                Type type = obj.GetType();

                int index = -1;
                if (part.Contains("[") && part.Contains("]"))
                {

                    string[] arrayPart = part.Split(new char[] { '[' });
                    if (arrayPart.Length == 2)
                    {
                        arrayPart = arrayPart[1].Split(new char[] { ']' }, StringSplitOptions.RemoveEmptyEntries);
                        if (arrayPart.Length == 1)
                        {
                            index = arrayPart[0].AsInt32();
                        }
                    }
                }

                if (index != -1 && type.IsArray)
                {
                    Array innerArray = obj as Array;
                    obj = innerArray.GetValue(index);
                    string innerPropName = GetPropertyNameWithIndex(propList, i);
                    if (string.IsNullOrEmpty(innerPropName))
                    {
                        part = part.Replace(string.Format("[{0}]", index), string.Empty);
                        return obj.GetValueFromProperty(part);
                    }
                    else
                    {
                        return obj.GetValueFromProperty(innerPropName);
                    }
                }

                PropertyInfo info = type.GetProperty(part);
                if (info == null)
                {
                    return null;
                }

                obj = info.GetValue(obj, null);
            }

            return obj;
        }


        public static XmlDocument ToXmlDocument(this XDocument xDocument)
        {
            var xmlDocument = new XmlDocument();
            using (var xmlReader = xDocument.CreateReader())
            {
                xmlDocument.Load(xmlReader);
            }

            return xmlDocument;
        }

        public static XDocument ToXDocument(this XmlDocument xmlDocument)
        {
            using (var nodeReader = new XmlNodeReader(xmlDocument))
            {
                nodeReader.MoveToContent();
                return XDocument.Load(nodeReader);
            }
        }

        public static XDocument ToXDocument(this string value)
        {
            XmlDocument xmlDocument = new XmlDocument();
            try
            {
                xmlDocument.LoadXml(value);
            }
            catch
            {
                value = "<Error>ToXDocument error</Error>";
                xmlDocument.LoadXml(value);
            }

            using (var nodeReader = new XmlNodeReader(xmlDocument))
            {
                nodeReader.MoveToContent();
                return XDocument.Load(nodeReader);
            }
        }


        /// <summary>
        /// Compresses the string.
        /// </summary>
        /// <param name="text">The text to compress</param>
        /// <returns>Resturns zip text</returns>
        public static string CompressToString(this StringBuilder text)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(text.ToString());
            var memoryStream = new MemoryStream();
            using (var getZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
            {
                getZipStream.Write(buffer, 0, buffer.Length);
            }

            memoryStream.Position = 0;

            var compressedData = new byte[memoryStream.Length];
            memoryStream.Read(compressedData, 0, compressedData.Length);

            var getZipBuffer = new byte[compressedData.Length + 4];
            Buffer.BlockCopy(compressedData, 0, getZipBuffer, 4, compressedData.Length);
            Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, getZipBuffer, 0, 4);
            return Convert.ToBase64String(getZipBuffer);
        }

        /// <summary>
        /// Compresses the string.
        /// </summary>
        /// <param name="text">The text to compress</param>
        /// <returns>Resturns zip text</returns>
        public static byte[] CompressToByte(this StringBuilder text)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(text.ToString());
            var memoryStream = new MemoryStream();
            using (var getZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
            {
                getZipStream.Write(buffer, 0, buffer.Length);
            }

            memoryStream.Position = 0;

            var compressedData = new byte[memoryStream.Length];
            memoryStream.Read(compressedData, 0, compressedData.Length);

            var getZipBuffer = new byte[compressedData.Length + 4];
            Buffer.BlockCopy(compressedData, 0, getZipBuffer, 4, compressedData.Length);
            Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, getZipBuffer, 0, 4);
            return getZipBuffer;
        }

        public static string XmlToJson(this string xmlValue)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlValue);
            return JsonConvert.SerializeXmlNode(doc);
        }

        public static string XmlToJson(this XmlDocument xmlValue)
        {
            return JsonConvert.SerializeXmlNode(xmlValue);
        }

        public static string JsonToXmlString(this string jsonValue)
        {
            return JsonConvert.DeserializeXmlNode(jsonValue).InnerXml;
        }

        public static XmlDocument JsonToXmlDocument(this string jsonValue)
        {
            return JsonConvert.DeserializeXmlNode(jsonValue);
        }

        /// <summary>
        /// Compresses the string.
        /// </summary>
        /// <param name="text">The text to compress</param>
        /// <returns>Resturns zip text</returns>
        public static byte[] CompressToByte(this string text)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(text);
            var memoryStream = new MemoryStream();
            using (var getZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
            {
                getZipStream.Write(buffer, 0, buffer.Length);
            }

            memoryStream.Position = 0;

            var compressedData = new byte[memoryStream.Length];
            memoryStream.Read(compressedData, 0, compressedData.Length);

            var getZipBuffer = new byte[compressedData.Length + 4];
            Buffer.BlockCopy(compressedData, 0, getZipBuffer, 4, compressedData.Length);
            Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, getZipBuffer, 0, 4);
            return getZipBuffer;
        }

        /// <summary>
        /// Compresses the string.
        /// </summary>
        /// <param name="text">The text to compress</param>
        /// <returns>Resturns zip text</returns>
        public static byte[] Compress(this byte[] value)
        {
            byte[] buffer = value;
            var memoryStream = new MemoryStream();
            using (var getZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
            {
                getZipStream.Write(buffer, 0, buffer.Length);
            }

            memoryStream.Position = 0;

            var compressedData = new byte[memoryStream.Length];
            memoryStream.Read(compressedData, 0, compressedData.Length);

            var getZipBuffer = new byte[compressedData.Length + 4];
            Buffer.BlockCopy(compressedData, 0, getZipBuffer, 4, compressedData.Length);
            Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, getZipBuffer, 0, 4);
            return getZipBuffer;
        }

        /// <summary>
        /// Decompresses the string.
        /// </summary>
        /// <param name="compressedText">The compressed text.</param>
        /// <returns>decompressed string</returns>
        public static string DecompressFromString(this StringBuilder compressedText)
        {
            byte[] getZipBuffer = Convert.FromBase64String(compressedText.ToString());
            using (var memoryStream = new MemoryStream())
            {
                int dataLength = BitConverter.ToInt32(getZipBuffer, 0);
                memoryStream.Write(getZipBuffer, 4, getZipBuffer.Length - 4);

                var buffer = new byte[dataLength];

                memoryStream.Position = 0;
                using (var getZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                {
                    getZipStream.Read(buffer, 0, buffer.Length);
                }

                return Encoding.UTF8.GetString(buffer);
            }
        }

        /// <summary>
        /// Decompresses the string.
        /// </summary>
        /// <param name="compressedText">The compressed text.</param>
        /// <returns>decompressed string</returns>
        public static string DecompressFromByte(this byte[] compressedText)
        {
            byte[] getZipBuffer = compressedText;
            using (var memoryStream = new MemoryStream())
            {
                int dataLength = BitConverter.ToInt32(getZipBuffer, 0);
                memoryStream.Write(getZipBuffer, 4, getZipBuffer.Length - 4);

                var buffer = new byte[dataLength];

                memoryStream.Position = 0;
                using (var getZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                {
                    getZipStream.Read(buffer, 0, buffer.Length);
                }

                return Encoding.UTF8.GetString(buffer);
            }
        }


        /// <summary>
        /// Decompresses the string.
        /// </summary>
        /// <param name="compressedText">The compressed text.</param>
        /// <returns>decompressed string</returns>
        public static byte[] Decompress(this byte[] compressedText)
        {
            byte[] getZipBuffer = compressedText;
            using (var memoryStream = new MemoryStream())
            {
                int dataLength = BitConverter.ToInt32(getZipBuffer, 0);
                memoryStream.Write(getZipBuffer, 4, getZipBuffer.Length - 4);

                var buffer = new byte[dataLength];

                memoryStream.Position = 0;
                using (var getZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                {
                    getZipStream.Read(buffer, 0, buffer.Length);
                }

                return buffer;
            }
        }


        /// <summary>
        /// Decompresses the string.
        /// </summary>
        /// <param name="compressedText">The compressed text.</param>
        /// <returns>decompressed string</returns>
        public static void DecompressStringToFile(this StringBuilder compressedText, string FileName)
        {
            byte[] getZipBuffer = Convert.FromBase64String(compressedText.ToString());
            using (var memoryStream = new MemoryStream())
            {
                int dataLength = BitConverter.ToInt32(getZipBuffer, 0);
                memoryStream.Write(getZipBuffer, 4, getZipBuffer.Length - 4);

                var buffer = new byte[dataLength];

                memoryStream.Position = 0;
                using (Stream fd = File.Create(FileName))
                using (var getZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                {

                    int nRead;
                    while ((nRead = getZipStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        fd.Write(buffer, 0, nRead);
                    }
                }
            }
        }

        /// <summary>
        /// Compresses the string.
        /// </summary>
        /// <param name="text">The text to compress</param>
        /// <returns>Resturns zip text</returns>
        public static string CompressString(this string text)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(text);
            var memoryStream = new MemoryStream();
            using (var getZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
            {
                getZipStream.Write(buffer, 0, buffer.Length);
            }

            memoryStream.Position = 0;

            var compressedData = new byte[memoryStream.Length];
            memoryStream.Read(compressedData, 0, compressedData.Length);

            var getZipBuffer = new byte[compressedData.Length + 4];
            Buffer.BlockCopy(compressedData, 0, getZipBuffer, 4, compressedData.Length);
            Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, getZipBuffer, 0, 4);
            return Convert.ToBase64String(getZipBuffer);
        }

        /// <summary>
        /// Decompresses the string.
        /// </summary>
        /// <param name="compressedText">The compressed text.</param>
        /// <returns>decompressed string</returns>
        public static string DecompressString(this string compressedText)
        {
            byte[] getZipBuffer = Convert.FromBase64String(compressedText);
            using (var memoryStream = new MemoryStream())
            {
                int dataLength = BitConverter.ToInt32(getZipBuffer, 0);
                memoryStream.Write(getZipBuffer, 4, getZipBuffer.Length - 4);

                var buffer = new byte[dataLength];

                memoryStream.Position = 0;
                using (var getZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                {
                    getZipStream.Read(buffer, 0, buffer.Length);
                }

                return Encoding.UTF8.GetString(buffer);
            }
        }

        private static List<string[]> GetExceptionMessages(Exception ex, List<string[]> exceptionList)
        {
            string[] values = new string[2];
            values[0] = ex.GetType().Name;
            values[1] = ex.Message;
            exceptionList.Add(values);
            if (ex.InnerException != null)
            {
                exceptionList = GetExceptionMessages(ex.InnerException, exceptionList);
            }

            return exceptionList;
        }


        public static string GetExceptionMessagesHtml(this Exception ex)
        {
            List<string[]> messages = GetExceptionMessages(ex, new List<string[]>());
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<table>");
            sb.AppendLine("<tr>");
            sb.AppendFormat("<th>{0}</th>", "Exception Type");
            sb.AppendFormat("<th>{0}</th>", "Exception Detail");
            sb.AppendLine("</tr>");

            foreach (string[] exception in messages)
            {
                sb.AppendLine("<tr>");
                sb.AppendFormat("<td>{0}</td>", exception[0]);
                sb.AppendFormat("<td>{0}</td>", exception[1]);
                sb.AppendLine("</tr>");
            }

            sb.AppendLine("</table>");

            return sb.ToString();
        }

        private static List<SelectListItem> GetSelectListItem(this Dictionary<string, string> items)
        {
            List<SelectListItem> list = new List<SelectListItem>();

            foreach (string key in items.Keys)
            {
                SelectListItem item = new SelectListItem();
                item.Text = items[key];
                item.Value = key;
                list.Add(item);
            }

            return list;
        }


        private static List<SelectListItem> GetSelectListItem(this List<string> items)
        {
            List<SelectListItem> list = new List<SelectListItem>();

            for (int i = 0; i < items.Count; i++)
            {
                SelectListItem item = new SelectListItem();
                item.Text = items[i];
                item.Value = (i + 1).AsString();
                list.Add(item);
            }

            return list;
        }


        private static Dictionary<string, string> FillDictionaryForSelectList(int start, int end)
        {
            Dictionary<string, string> list = new Dictionary<string, string>();
            for (int i = start; i <= end; i++)
            {
                list.Add(i.ToString(), i.ToString());
            }

            return list;
        }


        public static IEnumerable<SelectListItem> AsSelectList(int end)
        {
            return AsSelectList(1, end);
        }

        public static IEnumerable<SelectListItem> AsMultiSelectList(int end)
        {
            return AsMultiSelectList(1, end);
        }

        public static IEnumerable<SelectListItem> AsSelectList(int start, int end)
        {
            return FillDictionaryForSelectList(start, end).AsSelectList();
        }

        public static IEnumerable<SelectListItem> AsMultiSelectList(int start, int end)
        {
            return FillDictionaryForSelectList(start, end).AsMultiSelectList();
        }

        public static IEnumerable<SelectListItem> AsSelectList(this Dictionary<string, string> items)
        {
            return new SelectList(items.GetSelectListItem(), "Value", "Text");
        }

        public static IEnumerable<SelectListItem> AsMultiSelectList(this List<object> items, string valueField,
            string textField)
        {
            Dictionary<string, string> list = new Dictionary<string, string>();
            foreach (object item in items)
            {

                string value = item.GetType().GetProperty(valueField).GetValue(item, null).AsString();
                string text = item.GetType().GetProperty(textField).GetValue(item, null).AsString();
                list.Add(value, text);
            }

            return list.AsMultiSelectList();
        }

        public static IEnumerable<SelectListItem> AsMultiSelectList<T>(this List<T> items, string valueField,
            string textField)
        {
            Dictionary<string, string> list = new Dictionary<string, string>();
            foreach (object item in items)
            {

                string value = item.GetType().GetProperty(valueField).GetValue(item, null).AsString();
                string text = item.GetType().GetProperty(textField).GetValue(item, null).AsString();
                list.Add(value, text);
            }

            return list.AsMultiSelectList();
        }


        public static IEnumerable<SelectListItem> AsMultiSelectList(this DataSet items, string valueField,
            string stringFormat, params string[] textFields)
        {
            Dictionary<string, string> list = new Dictionary<string, string>();
            foreach (DataRow item in items.Tables[0].Rows)
            {

                string value = item[valueField].AsString();
                string[] text = new string[textFields.Length];
                for (int i = 0; i < textFields.Length; i++)
                {

                    text[i] = item[textFields[i]].AsString();
                }

                list.Add(value, string.Format(stringFormat, text));
            }

            return list.AsMultiSelectList();
        }

        public static IEnumerable<SelectListItem> AsMultiSelectList<T>(this ObservableCollection<T> items,
            ComboBoxFormatData format)
        {
            return items.AsMultiSelectList(format.ValueField, format.StringFormat, format.TextFields);
        }


        public static IEnumerable<SelectListItem> AsMultiSelectList<T>(this ObservableCollection<T> items,
            string valueField, string stringFormat, params string[] textFields)
        {
            Dictionary<string, string> list = new Dictionary<string, string>();
            foreach (object item in items)
            {

                string value = item.GetType().GetProperty(valueField).GetValue(item, null).AsString();
                string[] text = new string[textFields.Length];
                for (int i = 0; i < textFields.Length; i++)
                {
                    PropertyInfo pi = item.GetType().GetProperty(textFields[i]);
                    if (pi != null)
                    {
                        text[i] = pi.GetValue(item, null).AsString();
                    }
                    else
                    {
                        text[i] = string.Format("UNDEFINED KEY TEXT = {0}", textFields[i]);
                    }
                }

                list.Add(value, string.Format(stringFormat, text));
            }

            return list.AsMultiSelectList();
        }

        public static IEnumerable<SelectListItem> AsMultiSelectList<T>(this List<T> items, string valueField,
            string stringFormat, params string[] textFields)
        {
            Dictionary<string, string> list = new Dictionary<string, string>();
            foreach (object item in items)
            {

                string value = item.GetType().GetProperty(valueField).GetValue(item, null).AsString();
                string[] text = new string[textFields.Length];
                for (int i = 0; i < textFields.Length; i++)
                {
                    PropertyInfo pi = item.GetType().GetProperty(textFields[i]);
                    if (pi != null)
                    {
                        text[i] = pi.GetValue(item, null).AsString();
                    }
                    else
                    {
                        text[i] = string.Format("UNDEFINED KEY TEXT = {0}", textFields[i]);
                    }
                }

                list.Add(value, string.Format(stringFormat, text));
            }

            return list.AsMultiSelectList();
        }

        public static IEnumerable<SelectListItem> AsSelectList<T>(this List<T> items, string valueField,
            string stringFormat, params string[] textFields)
        {
            Dictionary<string, string> list = new Dictionary<string, string>();
            foreach (object item in items)
            {

                string value = item.GetType().GetProperty(valueField).GetValue(item, null).AsString();
                string[] text = new string[textFields.Length];
                for (int i = 0; i < textFields.Length; i++)
                {
                    text[i] = item.GetType().GetProperty(textFields[i]).GetValue(item, null).AsString();
                }

                list.Add(value, string.Format(stringFormat, text));
            }

            return list.AsSelectList();
        }


        public static IEnumerable<SelectListItem> AsSelectList<T>(this List<T> items, string valueField,
            string textField)
        {
            Dictionary<string, string> list = new Dictionary<string, string>();
            foreach (object item in items)
            {
                string[] valueFields = valueField.Split(new char[] { '|' });
                string text = item.GetType().GetProperty(textField).GetValue(item, null).AsString();

                string value = string.Empty;
                List<string> values = new List<string>();
                foreach (string field in valueFields)
                {
                    values.Add(item.GetType().GetProperty(field).GetValue(item, null).AsString());
                }

                value = string.Join("|", values);

                list.Add(value, text);
            }

            return list.AsSelectList();
        }



        public static IEnumerable<SelectListItem> AsMultiSelectList(this Dictionary<string, string> items)
        {
            return new MultiSelectList(items.GetSelectListItem(), "Value", "Text");
        }

        public static IEnumerable<SelectListItem> AsSelectList(this List<string> items)
        {
            return new SelectList(items.GetSelectListItem(), "Value", "Text");
        }

        public static IEnumerable<SelectListItem> AsMultiSelectList(this List<string> items)
        {
            return new SelectList(items.GetSelectListItem(), "Value", "Text");
        }

        public static IEnumerable<SelectListItem> AsSelectList(this string[] items)
        {
            return AsSelectList(items.ToList<string>());
        }

        public static IEnumerable<SelectListItem> AsMultiSelectList(this string[] items)
        {
            return AsMultiSelectList(items.ToList<string>());
        }

        public static string GetListItemByItemId(this IEnumerable<SelectListItem> items, object itemId)
        {
            string returnValue = "[null]";
            if (itemId != null)
            {
                var selectedItem = from A in items
                    where A.Value == itemId.ToString()
                    select A.Text;

                if (selectedItem.Count() > 0)
                {
                    returnValue = selectedItem.ElementAt(0);
                }
            }

            return returnValue;
        }


        public static string MatchValue(this string value, Dictionary<string, string> list)
        {
            if (list.ContainsKey(value))
            {
                return list[value];
            }

            return value;
        }

        #endregion
    }
}