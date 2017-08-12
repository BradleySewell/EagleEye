using EagleEye.Entities.Details;
using EagleEye.Entities.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EagleEye.Entities.Constants
{
    public static class FileNameConstants
    {
        public const string SAVED_CONFIGURATION_FILENAME = "Configuration.bin";
        public const string SAVED_ACTIONS_FILENAME = "Actions.bin";



        public static string SNAPSHOTS_DIRECTORY_PATH()
        {
            return string.Format(@"{0}\{1}", Directory.GetCurrentDirectory(), "Snapshots");
        }

        public static string SNAPSHOT_FILE_EXTENSION = "png";


        public static string SNAPSHOT_DIRECTORY_FILE_PATH(SnapShotDetails item)
        {
            return string.Format(@"{0}\{1}@{2}.{3}", SNAPSHOTS_DIRECTORY_PATH(), item.Percentage.ToString(), item.DateTime.ToFileNameFormat(), SNAPSHOT_FILE_EXTENSION);
        }
    }
}
