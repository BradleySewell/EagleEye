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
    public class ActionController : AControllers
    {
        public ActionController(AppDataContext appDataContext) : base(appDataContext)
        {
        }

        public override void Initialise()
        {
            AppDataContext.Action = new ActionDetails();
            AppDataContext.Actions= new ObservableCollection<ActionDetails>();
        }

        public override void Start()
        {
            LoadConfiguration();
        }

        private void LoadConfiguration()
        {
            var deSerializedActions = BinarySerializer.LoadFile(string.Format(@"{0}\{1}", AppDomain.CurrentDomain.BaseDirectory, FileNameConstants.SAVED_ACTIONS_FILENAME));
            if (deSerializedActions == null)
            {
                SaveConfiguration();
                return;
            }

            var configuration = (ObservableCollection<ActionDetails>)deSerializedActions;

            AppDataContext.Actions = configuration;
        }

        public void SaveConfiguration()
        {
            BinarySerializer.SaveFile(AppDataContext.Actions, string.Format(@"{0}\{1}", AppDomain.CurrentDomain.BaseDirectory, FileNameConstants.SAVED_ACTIONS_FILENAME));
        }
    }
}
