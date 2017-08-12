using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EagleEye.Entities.Extensions
{
    public static class ByteArrayExtensions
    {
        public static string ByteArrayToByteString(this byte[] byteArray)
        {
            string hex = BitConverter.ToString(byteArray);
            return hex.Replace("-", "");
        }
    }
}
