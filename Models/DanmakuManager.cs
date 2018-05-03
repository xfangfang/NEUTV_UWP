﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetEasePlayer_UWP.Models
{
    class DanmakuManager
    {
        /// <summary>
        /// get/post data from/to DB
        /// before the teleplay, get init Danmakus
        /// when the teleplaying, update Danmaku
        /// </summary>
        /// <returns></returns>
        public List<Danmaku> GetInitDanmaku()
        {
            List<Danmaku> ret = new List<Danmaku>();
            Danmaku dan1 = new Danmaku("23", "scroll", "20180428", "02:08:08", 3, "scroll danmaku test!");
            Danmaku dan2 = new Danmaku("23", "top", "20180428", "02:08:08", 8, "top danmaku test!");
            Danmaku dan3 = new Danmaku("23", "bottom", "20180428", "02:08:08", 15, "bottom danmaku test!");
            Danmaku dan4 = new Danmaku("23", "scroll", "20180428", "02:08:08", 20, "delay scroll danmaku test!");
            ret.Add(dan1);ret.Add(dan2);ret.Add(dan3);ret.Add(dan4);
            return ret;
        }
    }
}
