using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EagleEye.Entities.Configuration
{
    [Serializable]
    public class ConfigurationItem : INotifyPropertyChanged
    {
        private string device;
        public string Device
        {
            get { return device; }
            set
            {
                device = value;
                OnPropertyChanged("Device");
            }
        }

        private double imageChangePercentageTriggerValue;
        public double ImageChangePercentageTriggerValue
        {
            get { return imageChangePercentageTriggerValue; }
            set
            {
                imageChangePercentageTriggerValue = value;
                OnPropertyChanged("ImageChangePercentageTriggerValue");
            }
        }

        private bool autoCalculatePixelChangeTolerance;
        public bool AutoCalculatePixelChangeTolerance
        {
            get { return autoCalculatePixelChangeTolerance; }
            set
            {
                autoCalculatePixelChangeTolerance = value;
                OnPropertyChanged("AutoCalculatePixelChangeTolerance");
            }
        }

        private int pixelChangeTolerance;
        public int PixelChangeTolerance
        {
            get { return pixelChangeTolerance; }
            set
            {
                pixelChangeTolerance = value;
                OnPropertyChanged("PixelChangeTolerance");
            }
        }

        private int waitTimeSeconds;
        public int WaitTimeSeconds
        {
            get { return waitTimeSeconds; }
            set
            {
                waitTimeSeconds = value;
                OnPropertyChanged("WaitTimeSeconds");
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
