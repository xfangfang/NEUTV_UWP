using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace NetEasePlayer_UWP.Models
{
    [DataContract]
    class Danmaku
    {
        [DataMember(Name = "channel_id")] public string Channel_id { get; set; }
        [DataMember(Name = "type")] public string Mode { get; set; }
        [DataMember(Name = "date")] public DateTime Date { get; set; }
        [DataMember(Name = "danmaku")] public string Text { get; set; }
        [DataMember(Name = "offset")] public TimeSpan Offset { get; set; }
    }
    // offset = Date - beginTime
    class DanmakuManager
    {
        public static DanmakuManager Instance = new DanmakuManager();

        private string uri = "http://45.77.107.93:8080";
        private HttpClient client = new HttpClient();
        private DataContractJsonSerializer danmakuSerializer = new DataContractJsonSerializer(typeof(Danmaku));
        private DataContractJsonSerializer danmakuArraySerializer = new DataContractJsonSerializer(typeof(Danmaku[]));

        public List<Danmaku> GetInitDanmaku()
        {
            List<Danmaku> ret = new List<Danmaku>();
            Danmaku d1 = new Danmaku
            {
                Channel_id = "test",
                Mode = "scroll",
                Date = new DateTime(2018, 05, 05),
                Text = "test1",
                Offset = TimeSpan.FromSeconds(3)
            };
            Danmaku d2 = new Danmaku
            {
                Channel_id = "test",
                Mode = "scroll",
                Date = new DateTime(2018, 05, 05),
                Text = "test222",
                Offset = TimeSpan.FromSeconds(10)
            };
            ret.Add(d1);ret.Add(d2);
            return ret;
        }
        //向服务器 POST 弹幕
        public async void AddDanmakuAsync(Danmaku d)
        {
            string url = uri + "/upload_danmaku";
            HttpClient httpClient = new HttpClient();

            var content = new FormUrlEncodedContent(new[]
           {
                new KeyValuePair<string, string>("channel_id", d.Channel_id.ToString()),
                new KeyValuePair<string, string>("type", d.Mode.ToString()),
                new KeyValuePair<string, string>("date", d.Date.ToString("yyyy-MM-dd HH:mm:ss")),
                new KeyValuePair<string, string>("danmaku", d.Text.ToString()),


            });
            try
            {
                var res = await httpClient.PostAsync(url, content);

            }catch(Exception e)
            {

            }

        }
        //从服务器请求弹幕
        // POST channel_id&beg_date&end_date
        // 解析 返回的xml文件
        public async Task<List<Danmaku>> QueryDanmaku(String channel_id, DateTime begin, DateTime end)
        {
            List<Danmaku> ret = new List<Danmaku>();
            string url = uri + "/download_danmaku";
            HttpClient httpClient = new HttpClient();

            var content = new FormUrlEncodedContent(new[]
           {

                new KeyValuePair<string, string>("channel_id", channel_id),
                new KeyValuePair<string, string>("beg_date", begin.ToString("yyyy-MM-dd HH:mm:ss")),
                new KeyValuePair<string, string>("end_date", end.ToString("yyyy-MM-dd HH:mm:ss"))

            });
            try
            {
                var res = await httpClient.PostAsync(url, content);
                var response = await res.Content.ReadAsStringAsync();

                //解析xml获得List<Danmaku>
                //....
                //计算每个Danmaku.offset
                ret = Xml2DanmakuList(response);
                if (ret != null)
                {
                    Debug.WriteLine("parse danmaku is not null ");
                    foreach (var item in ret)
                    {
                        Debug.WriteLine(item.Text);
                        item.Offset = item.Date - begin;
                    }
                }
                else
                {
                    Debug.WriteLine("daikun zz");
                }


                Debug.WriteLine(response);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                throw;
            }

            return ret;
        }
        public async Task<Danmaku[]> Query()
        {
            var reponse = await client.GetAsync("http://localhost:56887/api/Students");
            var json = await reponse.Content.ReadAsStringAsync();
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));

            return danmakuArraySerializer.ReadObject(stream) as Danmaku[];
        }
        public static List<Danmaku> Xml2DanmakuList(string xmlSourceStr)
        {
            List<Danmaku> tmpList = new List<Danmaku>();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlSourceStr);
            XmlNodeList nodelist = xmlDoc.SelectNodes("DanmakuList/Danmaku");
            foreach (XmlNode node in nodelist)
            {
                Danmaku entity = new Danmaku();
                entity.Mode = HttpUtility.HtmlEncode(node["type"].InnerText);
                entity.Channel_id = HttpUtility.HtmlEncode(node["channel_id"].InnerText);

                DateTimeFormatInfo dtFormat = new DateTimeFormatInfo();
                dtFormat.ShortDatePattern = "yyyy-MM-dd hh:mm:ss";

                entity.Date = Convert.ToDateTime(node["date"].InnerText, dtFormat);
                entity.Text = node["content"].InnerText;
                tmpList.Add(entity);
            }
            return tmpList;
        }
    }
}
