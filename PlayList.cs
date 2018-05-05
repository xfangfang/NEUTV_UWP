using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetEasePlayer_UWP
{
    public class PlayList
    {
        public String Name;
        public String Uid;
        public List<Live> Livelist;
        public PlayList(String _name,String _uid)
        {
            this.Uid = _uid;
            this.Name = _name;
            this.Livelist = new List<Live>();
        }

        public void AddLive(Live live)
        {
            this.Livelist.Add(live);
        }
        public void RemoveLive(Live live)
        {
            this.Livelist.Remove(live);
        }
    }
}
