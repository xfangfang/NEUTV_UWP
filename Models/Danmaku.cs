using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetEasePlayer_UWP.Models
{
    class Danmaku
    {
        public string channel_id;// ignore the channel/channel_hd
        public string mode;//danmaku mode: top, scroll, bottom
        public string date;// the teleplay date
        public string begin;//the teleplay begin time
        public int offset;// offset(/s) time from the telepaly start
        public string text;//danmaku text

        public Danmaku(string _channel_id,string _mode,string _date,string _begin,int _offset,string _text)
        {
            channel_id = _channel_id;
            mode = _mode;
            date = _date;
            begin = _begin;
            offset = _offset;
            text = _text;
        }

    }
}
