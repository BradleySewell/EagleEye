using EagleEye.Entities.Abstracts;
using EagleEye.Entities.Constants;
using EagleEye.Entities.DataContext;
using EagleEye.Entities.Details;
using EagleEye.Entities.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EagleEye.Entities.Controllers
{
    public class SnapShotController : AControllers
    {
        public SnapShotController(AppDataContext appDataContext) : base(appDataContext)
        {
        }

        public override void Initialise()
        {
            AppDataContext.SnapShots = new ObservableCollection<SnapShotDetails>();
        }
        public override void Start()
        {
            //LoadSnapShots();
        }

        private void LoadSnapShots()
        {
            var directoryPath = FileNameConstants.SNAPSHOTS_DIRECTORY_PATH();
            try
            {
                AppDataContext.SnapShots.Clear();

                var directory = new DirectoryInfo(directoryPath);
                if (directory.Exists)
                {
                    IEnumerable<FileInfo> snapShotFiles =
                        directory.GetFiles(string.Format("*.{0}", FileNameConstants.SNAPSHOT_FILE_EXTENSION),
                            SearchOption.TopDirectoryOnly).OrderByDescending(x => x.CreationTime);

                    foreach (var snapShotFile in snapShotFiles)
                    {
                        var snapShotFilePath = snapShotFile.FullName;
                        var snapShotImage = BitmapSerializer.LoadFile(snapShotFilePath);

                        if (snapShotImage != null)
                        {
                            var splitName = snapShotFile.Name.Split(new char[] { '@' });
                            if (splitName.Count() > 2)
                            {
                                var percentage = splitName[0];
                                var dateTime = splitName[1];
                                AppDataContext.SnapShots.Add(new SnapShotDetails()
                                {
                                    DateTime = Convert.ToDateTime(dateTime),
                                    Percentage = Convert.ToDouble(percentage),
                                    Image = snapShotImage,
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
            }
        }

        public void AddSnapShot(SnapShotDetails snapShot)
        {
            try
            {
                AppDataContext.SnapShots.Add(snapShot);

                //var directoryPath = FileNameConstants.SNAPSHOTS_DIRECTORY_PATH();
                //Directory.CreateDirectory(directoryPath);

                //var filePath = FileNameConstants.SNAPSHOT_DIRECTORY_FILE_PATH(snapShot);
                //BinarySerializer.SaveFile(snapShot.Image, filePath);
            }
            catch (Exception e)
            {
            }
        }


        public void DeleteSnapShot(SnapShotDetails snapShot)
        {
            try
            {
                AppDataContext.SnapShots.Remove(snapShot);

                //var directoryPath = FileNameConstants.SNAPSHOTS_DIRECTORY_PATH();
                //Directory.CreateDirectory(directoryPath);

                //var filePath = FileNameConstants.SNAPSHOT_DIRECTORY_FILE_PATH(snapShot);
                //BinarySerializer.SaveFile(snapShot.Image, filePath);
            }
            catch (Exception e)
            {
            }
        }


        public void DeleteAllSnapShots()
        {
            try
            {
                AppDataContext.SnapShots.Clear();

                //var directoryPath = FileNameConstants.SNAPSHOTS_DIRECTORY_PATH();
                //Directory.CreateDirectory(directoryPath);

                //var filePath = FileNameConstants.SNAPSHOT_DIRECTORY_FILE_PATH(snapShot);
                //BinarySerializer.SaveFile(snapShot.Image, filePath);
            }
            catch (Exception e)
            {
            }
        }

    }
}
