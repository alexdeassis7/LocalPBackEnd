using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools
{
    public class DateUtils
    {
        public static string ParseDateHour(string date, string[] hour) 
        {
            var datetime = date != null ? DateTime.Parse(date) : DateTime.Parse(DateTime.Now.ToString("MM/dd/yyyy"));
            var timeSpan = new TimeSpan(Int32.Parse(hour[0]), Int32.Parse(hour[1]), 0);
            datetime = datetime.Date + timeSpan;
            var convertedDateTime = TimeZoneInfo.ConvertTime(datetime, TimeZoneInfo.FindSystemTimeZoneById("Argentina Standard Time"), TimeZoneInfo.Utc);
            return convertedDateTime.ToString();
        }
    }
}
