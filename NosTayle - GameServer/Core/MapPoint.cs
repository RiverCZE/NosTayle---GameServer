using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.Core
{
    public class MapPoint 
    {
        internal int X;
        internal int Y;
        internal int Z;

        public MapPoint(int X, int Y, int Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }
    }
}
