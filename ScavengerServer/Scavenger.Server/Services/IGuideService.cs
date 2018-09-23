using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scavenger.Server.GrainInterfaces;

namespace Scavenger.Server.Services
{
    public interface IGuideService
    {
        Task Start();
        Task ScavengerFoundEgg(Guid guideId);
    }

}
