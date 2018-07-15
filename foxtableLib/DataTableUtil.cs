using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web.Script.Serialization;

namespace FoxtableLib
{
    public class DataTableUtil
    {
        public const string native_DataTable = "InnerNativeDataTable";
        public static Dictionary<string, object> VarStore = new Dictionary<string, object>();
        public static DataTable Init()
        {
            String json = "[{\"id\":\"00e58d51\",\"data\":[{\"mac\":\"20:f1:7c:c5:cd:80\",\"rssi\":\"-86\",\"ch\":\"9\"},{\"mac\":\"20:f1:7c:c5:cd:85\",\"rssi\":\"-91\",\"ch\":\"9\"}]},\n" +
                "{\"id\":\"00e58d53\",\"data\":[{\"mac\":\"bc:d1:77:8e:26:78\",\"rssi\":\"-94\",\"ch\":\"11\"},{\"mac\":\"14:d1:1f:3e:bb:ac\",\"rssi\":\"-76\",\"ch\":\"11\"},{\"mac\":\"20:f1:7c:d4:05:41\",\"rssi\":\"-86\",\"ch\":\"12\"}]}]";
            return ToDataTable(json, false);
        }
        public static DataTable Init2()
        {
            String json = "[{\"mac\":\"20:f1:7c:c5:cd:80\",\"rssi\":\"-86\",\"ch\":\"9\"},{\"mac\":\"20:f1:7c:c5:cd:85\",\"rssi\":\"-91\",\"ch\":\"9\"}]";
            return ToDataTable(json);
        }

        public static DataTable Init3()
        {
            String json = "[\n" +
              "    {\n" +
              "        \"id\": \"00e58d51\",\n" +
              "        \"code\":\"cx00e85d382928\",\n" +
              "        \"items\": [\n" +
              "            { \"mac\": \"20:f1:7c:c5:cd:80\", \"rssi\": \"-86\", \"ch\": 9 },\n" +
              "            { \"mac\": \"20:f1:7c:c5:cd:85\", \"rssi\": \"-91\", \"ch\": 9 }\n" +
              "        ]\n" +
              "    }\n" +
              "]";
            return ToDataTable(json);
        }


        /// <summary>
        /// Json 字符串 转换为 DataTable数据集合
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static DataTable ToDataTableWithComplex(string json)
        {
            DataTable dataTable = new DataTable();  //实例化
            DataTable result;
            try
            {
                JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer
                {
                    MaxJsonLength = Int32.MaxValue //取得最大数值
                };
                ArrayList arrayList = javaScriptSerializer.Deserialize<ArrayList>(json);
                if (arrayList.Count > 0)
                {
                    foreach (Dictionary<string, object> dictionary in arrayList)
                    {
                        if (dictionary.Keys.Count<string>() == 0)
                        {
                            result = dataTable;
                            return result;
                        }
                        //Columns
                        if (dataTable.Columns.Count == 0)
                        {
                            foreach (string current in dictionary.Keys)
                            {
                                if (current != "data")
                                    dataTable.Columns.Add(current, dictionary[current].GetType());
                                else
                                {
                                    ArrayList list = dictionary[current] as ArrayList;
                                    foreach (Dictionary<string, object> dic in list)
                                    {
                                        foreach (string key in dic.Keys)
                                        {
                                            dataTable.Columns.Add(key, dic[key].GetType());
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                        //Rows
                        string root = "";
                        foreach (string current in dictionary.Keys)
                        {
                            if (current != "data")
                                root = current;
                            else
                            {
                                ArrayList list = dictionary[current] as ArrayList;
                                foreach (Dictionary<string, object> dic in list)
                                {
                                    DataRow dataRow = dataTable.NewRow();
                                    dataRow[root] = dictionary[root];
                                  
                                    foreach (string key in dic.Keys)
                                    {
                                        dataRow[key] = dic[key];
                                    }
                                    dataTable.Rows.Add(dataRow);
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
            }
            result = dataTable;
            return result;
        }

        /// <summary>
        /// Json 字符串 转换为 DataTable数据集合
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static DataTable ToDataTableWithSimple(string json)
        {
            DataTable dataTable = new DataTable();  //实例化
            DataTable result;
            List<String> tableKeys = new List<string>();
            try
            {
                JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer
                {
                    MaxJsonLength = Int32.MaxValue //取得最大数值
                };
                ArrayList arrayList = javaScriptSerializer.Deserialize<ArrayList>(json);
                if (arrayList.Count > 0)
                {
                    foreach (Dictionary<string, object> dictionary in arrayList)
                    {
                        if (dictionary.Keys.Count<string>() == 0)
                        {
                            result = dataTable;
                            return result;
                        }
                        //Columns
                        if (dataTable.Columns.Count == 0)
                        {
                            foreach (string current in dictionary.Keys)
                            {
                                Type colType = dictionary[current].GetType();
                                if (colType == typeof(ArrayList))
                                {
                                    colType = typeof(DataTable);
                                    tableKeys.Add(current);
                                }
                                dataTable.Columns.Add(current, colType);
                            }
                            dataTable.Columns.Add(native_DataTable, typeof(DataTable));
                        }
                        //Rows
                        DataRow dataRow = dataTable.NewRow();
                        foreach (string current in dictionary.Keys)
                        {
                            if(string.IsNullOrEmpty(tableKeys.SingleOrDefault(k=>k == current)))
                            {
                                dataRow[current] = dictionary[current];
                            }else
                            {
                                dataRow[current] = ToDataTable(dictionary[current] as ArrayList);
                            }
                            
                        }
                        dataRow[native_DataTable] = dataTable;
                        dataTable.Rows.Add(dataRow); //循环添加行到DataTable中
                    }
                }
            }
            catch
            {
            }
            result = dataTable;
            return result;
        }

        public static DataTable ToDataTable(ArrayList arrayList)
        {
            //Foxtable.HttpClient httpClient = new Foxtable.HttpClient("http://localhost:8000/api/XmlToJson");
            //httpClient.ContentType = "application/xml";

            //httpClient.Content = "<Invoice gcInvoice=\"LP1034234324234\"></Invoice>";
            ////Foxtable.OO_00oOO.Tables["abc"]
            DataTable dataTable = new DataTable();  //实例化
            if (arrayList == null) return dataTable;
            DataTable result;
            try
            {
                if (arrayList.Count > 0)
                {
                    foreach (Dictionary<string, object> dictionary in arrayList)
                    {
                        if (dictionary.Keys.Count<string>() == 0)
                        {
                            result = dataTable;
                            return result;
                        }
                        //Columns
                        if (dataTable.Columns.Count == 0)
                        {
                            foreach (string current in dictionary.Keys)
                            {
                                dataTable.Columns.Add(current, dictionary[current].GetType());
                            }
                            dataTable.Columns.Add(native_DataTable, typeof(DataTable));
                        }
                        //Rows
                        DataRow dataRow = dataTable.NewRow();
                        foreach (string current in dictionary.Keys)
                        {
                            dataRow[current] = dictionary[current];
                        }
                        dataRow[native_DataTable] = dataTable;
                        dataTable.Rows.Add(dataRow); //循环添加行到DataTable中
                    }
                }
            }
            catch
            {
            }
            result = dataTable;
            return result;
        }
        public static ArrayList ToArrayList(DataTable dt)
        {
            ArrayList arrayList = new ArrayList();
            if (dt != null && dt.Rows.Count > 0)
            {            
                foreach (DataRow dataRow in dt.Rows)
                {
                    Dictionary<string, object> dictionary = new Dictionary<string, object>();
                    foreach (DataColumn dataColumn in dt.Columns)
                    {
                        if (String.IsNullOrEmpty(excludeFields.SingleOrDefault(f => f.ToLower().Equals(dataColumn.ColumnName.ToLower()))))
                        {
                            dictionary.Add(dataColumn.ColumnName, dataRow[dataColumn.ColumnName].ToString());
                        }
                    }
                    arrayList.Add(dictionary);
                }
                return arrayList;
            }
            return arrayList;
        }
        public static void AddRowToArrayList(ArrayList arrayList, DataRow dataRow,DataColumnCollection columns)
        {
            if (dataRow != null)
            {
                Dictionary<string, object> dictionary = new Dictionary<string, object>();
                foreach (DataColumn dataColumn in columns)
                {
                    if (String.IsNullOrEmpty(excludeFields.SingleOrDefault(f => f.ToLower().Equals(dataColumn.ColumnName.ToLower()))))
                    {
                        dictionary.Add(dataColumn.ColumnName, dataRow[dataColumn.ColumnName].ToString());
                    }
                }
                arrayList.Add(dictionary);
            }
        }
        public static DataTable ToDataTable(string json, bool simple = true)
        {
            return simple ? ToDataTableWithSimple(json) : ToDataTableWithComplex(json);
        }

        public static String[] excludeFields = new string[] {
            "_Locked",
            "System_Filter_Unique",
            "System_Sort_Temporary",
            "System_Filter_Temporary",
             native_DataTable
        };

        public static string ToJson(DataTable table)
        {
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            List<Dictionary<string, object>> parentRow = new List<Dictionary<string, object>>();
            Dictionary<string, object> childRow;
            foreach (DataRow row in table.Rows)
            {
                childRow = new Dictionary<string, object>();
                foreach (DataColumn col in table.Columns)
                {
                    if (String.IsNullOrEmpty(excludeFields.SingleOrDefault(f => f.ToLower().Equals(col.ColumnName.ToLower()))))
                    {
                        if(row[col].GetType() == typeof(DataTable)) //toArrayList
                        {
                            childRow.Add(col.ColumnName, ToJson(row[col] as DataTable));
                        }
                        else
                        {
                            childRow.Add(col.ColumnName, row[col]);
                        }
                        
                    }
                }
                parentRow.Add(childRow);
            }
            return jsSerializer.Serialize(parentRow);
        }

        public static ArrayList ToArrayListModel<T>(DataTable dt) where T : new()
        {
            return new ArrayList(ToListModel<T>(dt).ToArray());
        }
        public static IList<T> ToListModel<T>(DataTable dt) where T : new()
        {
            // 定义集合    
            IList<T> ts = new List<T>();

            // 获得此模型的类型   
            Type type = typeof(T);
            string tempName = "";

            foreach (DataRow dr in dt.Rows)
            {
                T t = new T();
                // 获得此模型的公共属性      
                PropertyInfo[] propertys = t.GetType().GetProperties();
                foreach (PropertyInfo pi in propertys)
                {
                    tempName = pi.Name;  // 检查DataTable是否包含此列    

                    if (dt.Columns.Contains(tempName))
                    {
                        // 判断此属性是否有Setter      
                        if (!pi.CanWrite) continue;

                        object value = dr[tempName];
                        if (value != DBNull.Value)
                            pi.SetValue(t, value, null);
                    }
                }
                ts.Add(t);
            }
            return ts;
        }

        public static string ToJsonWithJsonNet(DataTable table)
        {
            string jsonString = string.Empty;
            jsonString = JsonConvert.SerializeObject(table);
            return jsonString;
        }
        public static DataTable ToDataTableWithJsonNet(string jsonStr)
        {
            DataTable defaultDataTable = new DataTable();
            DataTable dataTable = JsonConvert.DeserializeObject<DataTable>(jsonStr);
            return dataTable ?? defaultDataTable;
        }

        public static string ToJsonWithConverter(DataTable dataTable)
        {
            string json = JsonConvert.SerializeObject(dataTable, new DataTableConverter());
            return json;
        }
        public static DataTable ToDataTableWithConverter(String json)
        {
            DataTable table = JsonConvert.DeserializeObject<DataTable>(json, new DataTableConverter());
            return table;
        }
        public static IList<T> ConvertTo<T>(DataTable table)
        {
            if (table == null)
            {
                return null;
            }

            List<DataRow> rows = new List<DataRow>();

            foreach (DataRow row in table.Rows)
            {
                rows.Add(row);
            }

            return ConvertTo<T>(rows);
        }

        public static IList<T> ConvertTo<T>(IList<DataRow> rows)
        {
            IList<T> list = null;

            if (rows != null)
            {
                list = new List<T>();

                foreach (DataRow row in rows)
                {
                    T item = CreateItem<T>(row);
                    list.Add(item);
                }
            }

            return list;
        }

        public static T CreateItem<T>(DataRow row)
        {
            T obj = default(T);
            if (row != null)
            {
                obj = Activator.CreateInstance<T>();

                foreach (DataColumn column in row.Table.Columns)
                {
                    PropertyInfo prop = obj.GetType().GetProperty(column.ColumnName);
                    try
                    {
                        object value = row[column.ColumnName];
                        prop.SetValue(obj, value, null);
                    }
                    catch
                    {  //You can log something here     
                       //throw;    
                    }
                }
            }

            return obj;
        }
    }
}
