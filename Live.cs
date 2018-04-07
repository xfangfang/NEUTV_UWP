using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
