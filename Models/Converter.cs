using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetEasePlayer_UWP.Models
{
    class Converter
    {

        public static Converter Instance = new Converter();
        public DateTime TimeConverter(long unixTimeStamp)
        {
            //System.DateTime startTime = TimeZoneInfo.ConvertTimeToUtc(new DateTime(1970, 1, 1));
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
            DateTime dt = startTime.AddSeconds(unixTimeStamp);
            //Debug.WriteLine(dt.ToString("yyyy/MM/dd HH:mm:ss:ffff"));
           
            return dt;
        }

    }
}
