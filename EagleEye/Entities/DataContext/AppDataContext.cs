using EagleEye.Entities.Configuration;
using EagleEye.Entities.Controllers;
using EagleEye.Entities.Details;
using EagleEye.Startup;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Ink;
using System.Windows.Media.Imaging;

namespace EagleEye.Entities.DataContext
{
    public class AppDataContext : INotifyPropertyChanged
    {
        public ControllerInitialiser Controllers;

        public NotifyIconHandler NotifyIconHandler { get; set; }

        private ConfigurationItem configuration;
        public ConfigurationItem Configuration
        {
            get { return configuration; }
            set
            {
                configuration = value;
                OnPropertyChanged("Configuration");
            }
        }

        private ObservableCollection<string> devices;
        public ObservableCollection<string> Devices
        {
            get { return devices; }
            set
            {
                devices = value;
                OnPropertyChanged("Devices");
            }
        }

        private BitmapImage videoImage;
        public BitmapImage VideoImage
        {
            get { return videoImage; }
            set
            {
                videoImage = value;
                OnPropertyChanged("VideoImage");
            }
        }

        private StrokeCollection strokeCollection;
        public StrokeCollection StrokeCollection
        {
            get { return strokeCollection; }
            set
            {
                strokeCollection = value;
                OnPropertyChanged("StrokeCollection");
            }
        }

        private BitmapImage videoSelectedImage;
        public BitmapImage VideoSelectedImage
        {
            get { return videoSelectedImage; }
            set
            {
                videoSelectedImage = value;
                OnPropertyChanged("videoSelectedImage");
            }
        }

        private BitmapImage imageDifference;
        public BitmapImage ImageDifference
        {
            get { return imageDifference; }
            set
            {
                imageDifference = value;
                OnPropertyChanged("ImageDifference");
            }
        }
        
        private int frameRate;
        public int FrameRate
        {
            get { return frameRate; }
            set
            {
                frameRate = value;
                OnPropertyChanged("FrameRate");
            }
        }

        private double imageChangePercentage;
        public double ImageChangePercentage
        {
            get { return imageChangePercentage; }
            set
            {
                imageChangePercentage = value;
                OnPropertyChanged("ImageChangePercentage");
            }
        }

        private string status;
        public string Status
        {
            get { return status; }
            set
            {
                status = value;
                OnPropertyChanged("Status");
            }
        }


        private ObservableCollection<SnapShotDetails> snapShots;
        public ObservableCollection<SnapShotDetails> SnapShots
        {
            get { return snapShots; }
            set
            {
                snapShots = value;
                OnPropertyChanged("SnapShots");
            }
        }


        private ObservableCollection<ActionDetails> actions;
        public ObservableCollection<ActionDetails> Actions
        {
            get { return actions; }
            set
            {
                actions = value;
                OnPropertyChanged("Actions");
            }
        }

        private ActionDetails action;
        public ActionDetails Action
        {
            get { return action; }
            set
            {
                action = value;
                OnPropertyChanged("Action");
            }
        }




        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
