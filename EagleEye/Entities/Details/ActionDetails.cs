using EagleEye.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace EagleEye.Entities.Details
{
    [Serializable]
    public class ActionDetails : INotifyPropertyChanged
    {
        private ActionTypes _actionType;
        public ActionTypes ActionType
        {
            get { return _actionType; }
            set
            {
                _actionType = value;
                OnPropertyChanged("ActionType");
            }
        }

        private string _actionValue;
        public string ActionValue
        {
            get { return _actionValue; }
            set
            {
                _actionValue = value;
                OnPropertyChanged("ActionValue");
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
