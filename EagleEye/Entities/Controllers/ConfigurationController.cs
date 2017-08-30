using EagleEye.Entities.Abstracts;
using EagleEye.Entities.Configuration;
using EagleEye.Entities.DataContext;
using EagleEye.Entities.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EagleEye.Entities.Controllers
{
    public class ConfigurationController : AControllers
    {
        public ConfigurationController(AppDataContext AppDataContext)
            : base(AppDataContext)
        {
        }

        public override void Initialise()
        {
            AppDataContext.Configuration = new ConfigurationItem()
            {
                Device = string.Empty,
                MirrorImage = true,
                ImageChangePercentageTriggerValue = 0.11,
                AutoCalculatePixelChangeTolerance = true,
                PixelChangeTolerance = 60,
                WaitTimeSeconds = 10,
            };
        }

        public override void Start()
        {
            LoadConfiguration();
        }

        private void LoadConfiguration()
        {
            var deSerializedConfiguration = BinarySerializer.LoadFile(string.Format(@"{0}\{1}", AppDomain.CurrentDomain.BaseDirectory, Constants.FileNameConstants.SAVED_CONFIGURATION_FILENAME));
            if (deSerializedConfiguration == null)
            {
                SaveConfiguration();
                return;
            }

            var configuration = (ConfigurationItem)deSerializedConfiguration;

            AppDataContext.Configuration = configuration;
        }

        public void SaveConfiguration()
        {
            BinarySerializer.SaveFile(AppDataContext.Configuration, string.Format(@"{0}\{1}", AppDomain.CurrentDomain.BaseDirectory, Constants.FileNameConstants.SAVED_CONFIGURATION_FILENAME));
        }
        
    }
}
