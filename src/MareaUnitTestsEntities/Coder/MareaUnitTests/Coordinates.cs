using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MareaUnitTests
{
    [Serializable]
    public class Coordinates
    {
        public double x;
        public double y;

        public Coordinates() { }
        public Coordinates(double x, double y)
        {
            this.x=x;
            this.y=y;
        }

    }
}
