using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.Storage
{
    class Database
    {
        private readonly string _DatabaseName;
        private readonly uint _PoolMinSize;
        private readonly uint _PoolMaxSize;
        public string DatabaseName
        {
            get
            {
                return this._DatabaseName;
            }
        }
        public uint PoolMinSize
        {
            get
            {
                return this._PoolMinSize;
            }
        }
        public uint PoolMaxSize
        {
            get
            {
                return this._PoolMaxSize;
            }
        }
        public Database(string DatabaseName, uint PoolMinSize, uint PoolMaxSize)
        {
            if (DatabaseName == null || DatabaseName.Length == 0)
                throw new ArgumentException(DatabaseName);
            this._DatabaseName = DatabaseName;
            this._PoolMinSize = PoolMinSize;
            this._PoolMaxSize = PoolMaxSize;
        }
    }
}
