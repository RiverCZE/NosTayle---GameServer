using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.Storage
{
    internal sealed class DatabaseClient : IDisposable
    {
        private DatabaseManager Manager;
        private MySqlConnection Connection;
        private MySqlCommand Command;
        public DatabaseClient(DatabaseManager _Manager)
        {
            Manager = _Manager;
            Connection = new MySqlConnection(_Manager.ConnectionString);
            Command = this.Connection.CreateCommand();
            Connection.Open();
        }
        public void Dispose()
        {
            Connection.Close();
            Command.Dispose();
            Connection.Dispose();
        }
        public void AddParamWithValue(string sParam, object val)
        {
            Command.Parameters.AddWithValue(sParam, val);
        }
        public void ExecuteQuery(string sQuery)
        {
            try
            {
                Command.CommandText = sQuery;
                Command.ExecuteScalar();
                Command.CommandText = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        public DataSet ReadDataSet(string Query)
        {
            try
            {
                DataSet dataSet = new DataSet();
                Command.CommandText = Query;
                using (MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(this.Command))
                    mySqlDataAdapter.Fill(dataSet);
                Command.CommandText = null;
                return dataSet;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }
        public DataTable ReadDataTable(string Query)
        {
            try
            {
                DataTable dataTable = new DataTable();
                Command.CommandText = Query;
                using (MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(this.Command))
                    mySqlDataAdapter.Fill(dataTable);
                Command.CommandText = null;
                return dataTable;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }
        public DataRow ReadDataRow(string Query)
        {
            try
            {
                DataTable dataTable = this.ReadDataTable(Query);
                if (dataTable != null && dataTable.Rows.Count > 0)
                    return dataTable.Rows[0];
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }
        public string ReadString(string Query)
        {
            Command.CommandText = Query;
            string result = this.Command.ExecuteScalar().ToString();
            Command.CommandText = null;
            return result;
        }
        public int ReadInt32(string Query)
        {
            Command.CommandText = Query;
            int result = int.Parse(this.Command.ExecuteScalar().ToString());
            Command.CommandText = null;
            return result;
        }
        public uint ReadUInt32(string Query)
        {
            Command.CommandText = Query;
            uint result = (uint)this.Command.ExecuteScalar();
            Command.CommandText = null;
            return result;
        }
    }
}
