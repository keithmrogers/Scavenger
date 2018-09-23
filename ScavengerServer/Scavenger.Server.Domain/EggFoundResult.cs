using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scavenger.Server.Domain
{
    public class EggFoundResult
    {
        public double Distance { get; internal set; }
        public int TimeMs { get; internal set; }
    }
}
