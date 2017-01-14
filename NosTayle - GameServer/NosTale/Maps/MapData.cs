using NosTayleGameServer.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.NosTale.Maps
{
    public class MapData
    {
        internal int x;
        internal int y;
        internal int[,] grid;
        List<MapPoint> mapPoints;
        internal static Random random = new Random(); 
        
        internal static int randomPoint(int min, int max)
        {
            lock(random)
                return random.Next(min, max);
        }

        public MapData()
        {
        }

        public void Initialize(string path)
        {
            using (BinaryReader rdr = new BinaryReader(File.OpenRead(path)))
            {
                long lenght = rdr.BaseStream.Length;
                byte[] buffer = new byte[2];
                int bytesRead = rdr.Read(buffer, 0, sizeof(short));
                this.x = (int)buffer[0];
                buffer = new byte[2];
                bytesRead = rdr.Read(buffer, 0, sizeof(short));
                this.y = (int)buffer[0];
                this.grid = new int[this.y, this.x];
                this.mapPoints = new List<MapPoint>();
                for (int i = 0; i < this.y; i++)
                {
                    for (int t = 0; t < this.x; t++)
                    {
                        byte[] newPos = new byte[1];
                        bytesRead = rdr.Read(newPos, 0, 1);
                        this.grid[i, t] = (int)newPos[0];
                        this.mapPoints.Add(new MapPoint(i, t, (int)newPos[0]));
                    }
                }
            }
        }

        public MapPoint GetRandomWalkPoint()
        {
            List<MapPoint> points = this.mapPoints.FindAll(x => x.Z == 0);
            return points[MapData.randomPoint(0, points.Count)];
        }
    }
}
