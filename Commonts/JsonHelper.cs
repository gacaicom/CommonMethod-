using System;
using System.Collections.Generic;
using System.Data;
using System.IO; 
using System.Text; 
using System.Web.Script.Serialization;
using System.Runtime.Serialization.Json;
namespace CommonTools.Commonts
{
    public static class JsonHelper
    {
        private static readonly JavaScriptSerializer JavaScriptSerializer = new JavaScriptSerializer();

        #region js序列化

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Serialize(object obj)
        {
            return JavaScriptSerializer.Serialize(obj);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string jsonString)
        {
            //object obj = javaScriptSerializer.DeserializeObject(jsonString);
            //return (T)obj;
            return JavaScriptSerializer.Deserialize<T>(jsonString);
        }

        /// <summary> 
        /// 返回对象序列化 
        /// </summary> 
        /// <param name="obj">源对象</param> 
        /// <returns>json数据</returns> 
        public static string ToJson(object obj)
        {
            JavaScriptSerializer serialize = new JavaScriptSerializer();
            return serialize.Serialize(obj);
        }

        /// <summary> 
        /// 控制深度 
        /// </summary> 
        /// <param name="obj">源对象</param> 
        /// <param name="recursionDepth">深度</param> 
        /// <returns>json数据</returns> 
        public static string ToJson(object obj, int recursionDepth)
        {
            JavaScriptSerializer serialize = new JavaScriptSerializer();
            serialize.RecursionLimit = recursionDepth;
            return serialize.Serialize(obj);
        }

        /// <summary> 
        /// DataTable转为json 
        /// </summary> 
        /// <param name="dt">DataTable</param> 
        /// <returns>json数据</returns> 
        public static string ToJson(this DataTable dt)
        {
            List<object> dic = new List<object>();

            foreach (DataRow dr in dt.Rows)
            {
                Dictionary<string, object> result = new Dictionary<string, object>();

                foreach (DataColumn dc in dt.Columns)
                {
                    result.Add(dc.ColumnName, dr[dc].ToString());
                }
                dic.Add(result);
            }
            return ToJson(dic);
        }
        #endregion

        #region JSON序列化
        /// <summary>
        /// JSON序列化
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="obj">原始对象</param>
        /// <returns>JSON格式的字符串</returns>
        public static string JsonSerialize<T>(T obj)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            using (MemoryStream ms = new MemoryStream())
            {
                ser.WriteObject(ms, obj);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        /// <summary>
        /// JSON反序列化
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="json">JSON格式的字符串</param>
        /// <returns>反序列化后的对象</returns>
        public static T JsonDeserialize<T>(string json)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                T obj = (T)ser.ReadObject(ms);
                return obj;
            }
        }


        #region DataTable序列化
        /// <summary>
        /// DataTable序列化为Json格式字符串
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static string DataTableSerialize( DataTable table)
        {
             var serializer = new  JavaScriptSerializer();
            var list = new List<Dictionary<string, string>>();
            foreach (DataRow dr in table.Rows)
            {
                var result = new Dictionary<string, string>();
                foreach (System.Data.DataColumn dc in table.Columns)
                {
                    if (dr[dc] != null && dr[dc] != DBNull.Value)
                    {
                        result.Add(dc.ColumnName,
                            dc.DataType == typeof(DateTime)
                                ? ((DateTime) dr[dc]).ToString("yyyy-MM-dd HH:mm:ss")
                                : dr[dc].ToString());
                    }
                    else
                    {
                        result.Add(dc.ColumnName, null);
                    }
                }
                list.Add(result);
            }
            return serializer.Serialize(list);
        }
        /// <summary>
        /// 把Json格式序列化为DataTable
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static DataTable DataTableDeserialize(string json)
        {
            var table = new DataTable();

      var serializer = new JavaScriptSerializer();
            System.Collections.ArrayList dic = serializer.Deserialize<System.Collections.ArrayList>(json);
            foreach (Dictionary<string, string> Row in dic)
            {
                if (table.Columns.Count == 0)
                {
                    foreach (string key in Row.Keys)
                    {
                        table.Columns.Add(key, typeof(string));
                    }
                }
              var row = table.NewRow();
                foreach (string key in Row.Keys)
                {
                    row[key] = Row[key];//添加列值
                }
                table.Rows.Add(row);//添加一行
            }

            return table;
        }
        #endregion

        #endregion


    }
}