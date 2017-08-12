using EagleEye.Entities.Details;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EagleEye.Entities.Events
{
    public class SnapShotEventArgs : EventArgs
    {
        public SnapShotDetails SnapShot { get; set; }
    }
}
