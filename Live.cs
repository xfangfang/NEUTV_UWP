using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace NetEasePlayer_UWP
{
    public class Live
    {
        public String Url;
        public String Name;
        public String Uid;
        
        public Live(String _url,String _name,String _uid)
        {
            this.Url = _url;
            this.Name = _name;
            this.Uid = _uid;
        }

        public String GetSimpleName()
        {
            return Live.GetLiveName(this.GetUrlList()[0]);
        }

        public static String GetLiveName(String url)
        {
            string pattern = @"([^/]+).m3u8";
            Regex r = new Regex(pattern, RegexOptions.IgnoreCase);
            Match m = r.Match(url);
            return m.Value.Replace(".m3u8","");
        }

        public List<String> GetUrlList()
        {
            List<String> list = new List<string>(this.Url.Split("#"));
            return list;
        }
    }
}
