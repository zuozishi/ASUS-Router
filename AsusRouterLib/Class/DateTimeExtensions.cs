using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsusRouterApp.Class
{
    public static class DateTimeExtensions
    {
        private static DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// 产生一个当前的毫秒
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static long CurrentTimeMillis(this DateTime d)
        {
            return (long)((DateTime.UtcNow - Jan1st1970).TotalMilliseconds);
        }
    }
}
