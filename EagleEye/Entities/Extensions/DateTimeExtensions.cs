using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EagleEye.Entities.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToFileNameFormat(this DateTime item)
        {
            return string.Format("{0:yyyy-MM-dd_hh-mm-ss-fff-tt}", item);
        }
    }
}
