using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace NetEasePlayer_UWP.Models
{
    [DataContract]
    class Danmaku
    {
        /*
        public string channel_id;// ignore the channel/channel_hd
        public string mode;//danmaku mode: top, scroll, bottom
        public string date;// the teleplay date
        public string begin;//the teleplay begin time
        public int offset;// offset(/s) time from the telepaly start
        public string text;//danmaku text
        */
        [DataMember(Name = "channel_id")] public string Channel_id { get; set; }
        [DataMember(Name = "type")] public string Mode { get; set; }
        [DataMember(Name = "date")] public TimeSpan Date { get; set; }
        [DataMember(Name = "danmaku")] public string Text { get; set; }
        /*
        public Danmaku(string _channel_id,string _mode,string _date,string _begin,int _offset,string _text)
        {
            channel_id = _channel_id;
            mode = _mode;
            date = _date;
            begin = _begin;
            offset = _offset;// post date begin+offset
            text = _text;
        }
        */
    }
    class DanmakuManager
    {
        public static DanmakuManager Instance = new DanmakuManager();

        private string uri = "http://45.77.107.93:8080";
        private HttpClient client = new HttpClient();
        private DataContractJsonSerializer danmakuSerializer = new DataContractJsonSerializer(typeof(Danmaku));
        private DataContractJsonSerializer danmakuArraySerializer = new DataContractJsonSerializer(typeof(Danmaku[]));

        public async Task Add(Danmaku d)
        {
            var stream = new MemoryStream();
            danmakuSerializer.WriteObject(stream, d);
            stream.Position = 0;
            var streamReader = new StreamReader(stream);
            var json = streamReader.ReadToEnd();
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            Debug.WriteLine("start post danmaku");
            string url = uri+"/upload_danmaku";
            try
            {
                var response = await client.PostAsync(url, content);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                throw;
            }
        }


        public async Task<Danmaku[]> Query()
        {
            var reponse = await client.GetAsync("http://localhost:56887/api/Students");
            var json = await reponse.Content.ReadAsStringAsync();
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));

            return danmakuArraySerializer.ReadObject(stream) as Danmaku[];
        }
    }
}
