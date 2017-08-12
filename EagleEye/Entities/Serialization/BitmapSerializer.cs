using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace EagleEye.Entities.Serialization
{
    public class BitmapSerializer
    {
        public static void SaveFile(BitmapImage ItemToSerialize, string SaveFileLocation)
        {
            using (FileStream stream = new FileStream(SaveFileLocation, FileMode.Create))
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(ItemToSerialize));
                encoder.Save(stream);
            }

        }

        public static BitmapImage LoadFile(string FilePathToDeserialize)
        {
            try
            {
                if (File.Exists(FilePathToDeserialize))
                {
                    var image = new BitmapImage();
                    image.BeginInit();

                    // overwrite cache if already exists, to refresh image
                    image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                    // load into memory and unlock file
                    image.CacheOption = BitmapCacheOption.OnLoad;

                    image.UriSource = new Uri(FilePathToDeserialize);
                    image.EndInit();

                    return image;
                }
                return null;
            }
            catch(Exception e)
            {
                return null;
            }
        }
    }
}
