using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace CoreWebAPI.Common.Helper
{
    public class JsonHelper
    {
        /// <summary>
        /// Json对Object序列化操作函数
        /// </summary>
        /// <param name="Obj"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string ObjectToJson(object Obj)
        {
            try
            {
                return JsonConvert.SerializeObject(Obj);
            }
            catch
            {
                return "\"\"";
            }
        }

        /// <summary>
        /// Json对Object反序列化操作函数
        /// </summary>
        /// <param name="Json"></param>
        /// <param name="Type"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static object JsonToObject(string Json, Type Type)
        {
            try
            {
                return JsonConvert.DeserializeObject(Json, Type);
            }
            catch
            {
                return new object();
            }
        }
        /// <summary>
        /// Json对Object反序列化操作函数
        /// </summary>
        /// <param name="Json"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static TEntity JsonToObject<TEntity>(string Json)
        {
            try
            {
                return JsonConvert.DeserializeObject<TEntity>(Json);
            }
            catch(Exception ex)
            {
                return default;
            }
        }

        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static byte[] Serialize(object item)
        {
            var jsonString = JsonConvert.SerializeObject(item);

            return Encoding.UTF8.GetBytes(jsonString);
        }
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static TEntity Deserialize<TEntity>(byte[] value)
        {
            if (value == null)
            {
                return default;
            }
            var jsonString = Encoding.UTF8.GetString(value);
            return JsonConvert.DeserializeObject<TEntity>(jsonString);
        }

        /// <summary>
        /// Json对DataTable序列化操作函数
        /// </summary>
        /// <param name="DataTable">需要转换的DataTable</param>
        /// <returns>返回一个序列化后的字符串</returns>
        /// <remarks></remarks>
        public static string DataTableToJson(DataTable DataTable)
        {
            try
            {
                var DataSet = new DataSet("DataSet");
                DataSet.Tables.Add(DataTable.Copy());
                return JsonConvert.SerializeObject(DataSet);
            }
            catch (Exception)
            {
                return "\"\"";
            }
        }

        /// <summary>
        /// Json对DataTable序列化后的Json字符串进行反序列化操作函数
        /// </summary>
        /// <param name="JsonString">DataTable序列化后的Json字符串</param>
        /// <returns>返回DataSet</returns>
        /// <remarks></remarks>
        public static DataSet JsonToDataSet(string JsonString)
        {
            try
            {
                DataSet DataSet = new DataSet();
                DataSet = (DataSet)JsonConvert.DeserializeObject(JsonString, typeof(DataSet));
                return DataSet;
            }
            catch
            {
                return new DataSet();
            }
        }

        /// <summary>
        ///     ''' Json对DataTable序列化后的Json字符串进行反序列化操作函数
        ///     ''' </summary>
        ///     ''' <param name="JsonString">DataTable序列化后的Json字符串</param>
        ///     ''' <returns>返回转换的DataTable为DataSet第一个表</returns>
        ///     ''' <remarks></remarks>
        public static DataTable JsonToDataTable(string JsonString)
        {
            try
            {
                DataSet DataSet = new DataSet();
                DataSet = JsonToDataSet(JsonString);
                if (DataSet.Tables.Count == 1)
                    return DataSet.Tables[0];
                else
                    return new DataTable();
            }
            catch (Exception)
            {
                return new DataTable();
            }
        }

        /// <summary>
        /// 将json转换为DataTable
        /// </summary>
        /// <param name="strJson">得到的json</param>
        /// <returns></returns>

        public static DataTable NewJsonToDataTable(string strJson)
        {
            DataTable table = new DataTable();
            try
            {
                JArray array = JsonConvert.DeserializeObject(strJson) as JArray;
                // 反序列化为数组
                if (array.Count > 0)
                {
                    StringBuilder columns = new StringBuilder();

                    JObject objColumns = array[0] as JObject;
                    // 构造表头
                    foreach (JToken jkon in objColumns.AsJEnumerable())
                    {
                        JProperty jp = (JProperty)jkon;
                        string name = jp.Name;
                        columns.Append(name + Convert.ToString(","));
                        table.Columns.Add(name);
                    }
                    // 向表中添加数据
                    for (int i = 0; i <= array.Count - 1; i++)
                    {
                        DataRow row = table.NewRow();
                        JObject obj = array[i] as JObject;
                        foreach (JToken jkon in obj.AsJEnumerable())
                        {
                            JProperty jp = (JProperty)jkon;
                            string name = jp.Name;
                            if (table.Columns.Contains(name) == false)
                                table.Columns.Add(name);
                            string value = jp.Value.ToString();
                            row[name] = value;
                        }
                        table.Rows.Add(row);
                    }
                }
            }
            catch
            {
            }
            return table;
        }


        /// <summary>
        /// 转换对象为JSON格式数据
        /// </summary>
        /// <typeparam name="T">类</typeparam>
        /// <param name="obj">对象</param>
        /// <returns>字符格式的JSON数据</returns>
        public static string GetJSON<T>(object obj)
        {
            string result = String.Empty;
            try
            {
                System.Runtime.Serialization.Json.DataContractJsonSerializer serializer =
                new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(T));
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    serializer.WriteObject(ms, obj);
                    result = System.Text.Encoding.UTF8.GetString(ms.ToArray());
                }
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }
        /// <summary>
        /// 转换List&lt;T>的数据为JSON格式
        /// </summary>
        /// <typeparam name="T">类</typeparam>
        /// <param name="vals">列表值</param>
        /// <returns>JSON格式数据</returns>
        public static string JSON<T>(List<T> vals)
        {
            System.Text.StringBuilder st = new System.Text.StringBuilder();
            try
            {
                System.Runtime.Serialization.Json.DataContractJsonSerializer s = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(T));

                foreach (T city in vals)
                {
                    using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                    {
                        s.WriteObject(ms, city);
                        st.Append(System.Text.Encoding.UTF8.GetString(ms.ToArray()));
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return st.ToString();
        }
        /// <summary>
        /// JSON格式字符转换为T类型的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonStr"></param>
        /// <returns></returns>
        public static T ParseFormByJson<T>(string jsonStr)
        {
            T obj = Activator.CreateInstance<T>();
            using (System.IO.MemoryStream ms =
            new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(jsonStr)))
            {
                System.Runtime.Serialization.Json.DataContractJsonSerializer serializer =
                new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(T));
                return (T)serializer.ReadObject(ms);
            }
        }

        public static string JSON1<SendData>(List<SendData> vals)
        {
            System.Text.StringBuilder st = new System.Text.StringBuilder();
            try
            {
                System.Runtime.Serialization.Json.DataContractJsonSerializer s = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(SendData));

                foreach (SendData city in vals)
                {
                    using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                    {
                        s.WriteObject(ms, city);
                        st.Append(System.Text.Encoding.UTF8.GetString(ms.ToArray()));
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return st.ToString();
        }

        #region 提取json对象
        /// <summary>
        /// 提取json字符串对象
        /// 例如输入：{"1":1,"a":"aa","aa":"{\"2\":2,\"bb\":\"{\\\"3\\\":3,\\\"cc\\\":\\\"ccc\\\"}\"}"}
        /// 例如输出：{"1":1,"a":"aa","aa":{"2":2,"bb":{"3":3,"cc":"ccc"}}}
        /// </summary>
        public static JObject ExtractObj(string jsonObject)
        {
            return ExtractObj(JObject.Parse(jsonObject));
        }
        /// <summary>
        /// 提取json对象
        /// 例如输入：{"1":1,"a":"aa","aa":"{\"2\":2,\"bb\":\"{\\\"3\\\":3,\\\"cc\\\":\\\"ccc\\\"}\"}"}
        /// 例如输出：{"1":1,"a":"aa","aa":{"2":2,"bb":{"3":3,"cc":"ccc"}}}
        /// </summary>
        public static JObject ExtractObj(JObject job)
        {
            //方法一：慢（3700个字符耗时2.2-2.4秒）
            //foreach (var item in job)
            //{
            //    try
            //    {
            //        string itemStr = item.Value.ToString();
            //        JObject itemJObj = JObject.Parse(itemStr);
            //        JObject robj = ExtractObj(itemJObj);
            //        job[item.Key] = robj;
            //    }
            //    catch
            //    {
            //        try
            //        {
            //            string itemStr = item.Value.ToString();
            //            JArray itemJArr = JArray.Parse(itemStr);
            //            JArray rArr = ExtractArr(itemJArr);
            //            job[item.Key] = rArr;
            //        }
            //        catch
            //        {
            //        }
            //    }
            //}
            //return job;

            //方法二：快（3700个字符耗时40-60毫秒）
            foreach (var item in job)
            {
                var itemV = item.Value;
                if (itemV.Type == JTokenType.String)
                {
                    var jtStr = itemV.ToString();
                    if (!IsJson(jtStr))
                        continue;

                    JToken jToken = JToken.Parse(jtStr);
                    if (jToken.Type == JTokenType.Object)
                        job[item.Key] = ExtractObj((JObject)jToken);
                    else if (jToken.Type == JTokenType.Array)
                        job[item.Key] = ExtractArr((JArray)jToken);

                }
                else if (itemV.Type == JTokenType.Object)
                {
                    job[item.Key] = ExtractObj((JObject)itemV);
                }
                else if (itemV.Type == JTokenType.Array)
                {
                    job[item.Key] = ExtractArr((JArray)itemV);
                }
            }
            return job;
        }
        #endregion

        #region 提取json数组
        /// <summary>
        /// 提取json字符串数组
        /// 例如输入：["5","6","[\"3\",\"4\",\"[\\\"1\\\",\\\"2\\\"]\"]"]
        /// 例如输出：["5","6",["3","4",["1","2"]]]
        /// </summary>
        public static JArray ExtractArr(string jsonArr)
        {
            return ExtractArr(JArray.Parse(jsonArr));
        }
        /// <summary>
        /// 提取json数组
        /// 例如输入：["5","6","[\"3\",\"4\",\"[\\\"1\\\",\\\"2\\\"]\"]"]
        /// 例如输出：["5","6",["3","4",["1","2"]]]
        /// </summary>
        /// <param name="jArr"></param>
        /// <returns></returns>
        public static JArray ExtractArr(JArray jArr)
        {
            //方法一：慢（3700个字符耗时2.2-2.4秒）
            //for (int i = 0; i < jArr.Count; i++)
            //{
            //    try
            //    {
            //        string itemStr = jArr[i].ToString();
            //        JArray itemJArr = JArray.Parse(itemStr);
            //        JArray rArr = ExtractArr(itemJArr);
            //        jArr[i] = rArr;
            //    }
            //    catch
            //    {
            //        try
            //        {
            //            string itemStr = jArr[i].ToString();
            //            JObject itemJObj = JObject.Parse(itemStr);
            //            JObject robj = ExtractObj(itemJObj);
            //            jArr[i] = robj;
            //        }
            //        catch
            //        {
            //        }
            //    }
            //}
            //return jArr;

            //方法二：快（3700个字符耗时40-60毫秒）
            for (int i = 0; i < jArr.Count; i++)
            {
                JToken jToken = jArr[i];
                if (jToken.Type == JTokenType.String)
                {
                    var jtStr = jToken.ToString();
                    if (!IsJson(jtStr))
                        continue;

                    JToken jToken2 = JToken.Parse(jtStr);
                    if (jToken2.Type == JTokenType.Array)
                    {
                        jArr[i] = ExtractArr((JArray)jToken2);
                    }
                    else if (jToken2.Type == JTokenType.Object)
                    {
                        jArr[i] = ExtractObj((JObject)jToken2);
                    }
                }
                else if (jToken.Type == JTokenType.Array)
                {
                    jArr[i] = ExtractArr((JArray)jToken);
                }
                else if (jToken.Type == JTokenType.Object)
                {
                    jArr[i] = ExtractObj((JObject)jToken);
                }
            }
            return jArr;
        }
        #endregion

        #region 提取json对象或数组
        /// <summary>
        /// 提取json字符串（支持json字符串的对象、数组、字符串）
        /// 例如输入：["5","6","[\"3\",\"4\",\"[\\\"1\\\",\\\"2\\\"]\"]","{\"1\":2,\"a\":\"ab\"}"]
        /// 例如输出：["5","6",["3","4",["1","2"]],{"1":2,"a":"ab"}]
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static JToken ExtractAll(string json)
        {
            try
            {
                return ExtractAll(JToken.Parse(json));
            }
            catch
            {
                throw new Exception("不是有效的JToken对象");
            }
        }
        /// <summary>
        /// 提取json（支持json对象、数组、字符串）
        /// 例如输入：["5","6","[\"3\",\"4\",\"[\\\"1\\\",\\\"2\\\"]\"]","{\"1\":2,\"a\":\"ab\"}"]
        /// 例如输出：["5","6",["3","4",["1","2"]],{"1":2,"a":"ab"}]
        /// </summary>
        /// <param name="jToken"></param>
        /// <returns></returns>
        public static JToken ExtractAll(JToken jToken)
        {
            if (jToken.Type == JTokenType.String)
            {
                jToken = JToken.Parse(jToken.ToString());
            }

            if (jToken.Type == JTokenType.Object)
            {
                return ExtractObj((JObject)jToken);
            }
            else if (jToken.Type == JTokenType.Array)
            {
                return ExtractArr((JArray)jToken);
            }
            else
            {
                throw new Exception("暂不支持提取[" + jToken.Type.ToString() + "]类型");
            }
        }

        /// <summary>
        /// 是否为json（开头是'{','['的）
        /// </summary>
        public static bool IsJson(string json)
        {
            json = json.Trim();
            if (string.IsNullOrEmpty(json))
                return false;

            var t = json.First();
            if (t == '{' || t == '[')
                return true;

            return false;
        }
        #endregion


        //格式化json字符串
        public static string JsonFormat(string str)
        {
            try
            {
                JsonSerializer serializer = new JsonSerializer();
                TextReader tr = new StringReader(str);
                JsonTextReader jtr = new JsonTextReader(tr);
                object obj = serializer.Deserialize(jtr);
                if (obj != null)
                {
                    StringWriter textWriter = new StringWriter();
                    JsonTextWriter jsonWriter = new JsonTextWriter(textWriter)
                    {
                        Formatting = Formatting.Indented,
                        Indentation = 4,
                        IndentChar = ' '
                    };
                    serializer.Serialize(jsonWriter, obj);
                    return textWriter.ToString();
                }
                else
                {
                    return str;
                }
            }
            catch
            {
                return str;
            }
            
        }
    }
}
