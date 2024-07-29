using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toplearn.Core.Convertors
{
    public static class DateConvertor
    {
        public static string ToShamsi(this DateTime time)
        {
            var pc = new PersianCalendar();
            return pc.GetYear(time).ToString() + "/" + pc.GetMonth(time).ToString("00") + "/" + pc.GetDayOfMonth(time).ToString("00");
        }
    }
}
