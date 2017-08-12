using EagleEye.Entities.DataContext;
using EagleEye.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EagleEye.Entities.Controllers
{
    public class ControllerInitialiser
    {
        private AppDataContext _appDataContext;
        public ConfigurationController ConfigurationController;
        public ActionController ActionController;
        public SnapShotController SnapShotController;


        public ControllerInitialiser(AppDataContext AppDataContext)
        {
            _appDataContext = AppDataContext;
        }

        public List<IControllers> InitialiseControllers()
        {
            ConfigurationController = new ConfigurationController(_appDataContext);
            ActionController = new ActionController(_appDataContext);
            SnapShotController = new SnapShotController(_appDataContext);

            return new List<IControllers>()
            {
                ConfigurationController,
                ActionController,
                SnapShotController
            };
        }
    }
}
