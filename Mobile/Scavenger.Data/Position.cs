using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Scavenger.Data
{
    public class Position
    {
        public Position(double x, double y)
        {
            X = x;
            Y = y;
        }
        public double X { get; set; }
        public double Y { get; set; }

        public override string ToString()
        {
            return $"{X:###0.0#}, {Y:###0.0#}";
        }
    }
}
