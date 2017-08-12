using EagleEye.Entities.DataContext;
using EagleEye.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EagleEye.Entities.Abstracts
{
    public abstract class AControllers : IControllers
    {
        public AppDataContext AppDataContext;

        public AControllers(AppDataContext appDataContext)
        {
            AppDataContext = appDataContext;
            Initialise();
            Start();
        }

        public abstract void Initialise();

        public abstract void Start();
    }
}
