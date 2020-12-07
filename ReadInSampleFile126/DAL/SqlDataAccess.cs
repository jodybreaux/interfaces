using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Text;

namespace ReadInSampleFile126.DAL
{
    public class SQLDataAccess
    {

        protected string ConnectionString { get; set; }
        protected SqlConnection _conn;

        public SQLDataAccess()
        {
            //var configuration = new ConfigurationBuilder()
            //    .SetBasePath(Directory.GetParent(Directory.GetCurrentDirectory()) + "/MyProject.DAL").AddJsonFile("config.json", false)
            //    .Build();

            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.dev.json")
                .Build();

            this.ConnectionString = config.GetSection("Connections:connectionString").Value;
            _conn = GetConnection();
        }

        private SqlConnection GetConnection()
        {
            SqlConnection connection = new SqlConnection(this.ConnectionString);
            if (connection.State != ConnectionState.Open)
                connection.Open();
            return connection;
        }
        public int InsertHeader(string hdrFileDate)
        {
            int rows = 0;
         
            try
            {
                if (_conn.State != ConnectionState.Open)
                    _conn.Open();

                string query =
                    $"INSERT INTO tblFas126 (hdrFileDate, dateCreated, comment, fileNumber) VALUES ('{hdrFileDate}', '{DateTime.UtcNow}', '{"Header Data"}', '0')";
                rows = InsertDb(query);
            }
            catch (Exception ex)
            {
                rows = -1;
            }
            return rows;
        }
        public int InsertTrailer(string fileDate, string nRecords)
        {
            int rows = 0;
            try
            {
                if (_conn.State != ConnectionState.Open)
                    _conn.Open();

                string query =
                    $"INSERT INTO tblFas126 (trailerFileDate, trailerNumRecords, dateCreated, comment, fileNumber) VALUES ('{fileDate}', '{nRecords}', '{DateTime.UtcNow}', '{"Trailer Record"}', '0')";
                rows = InsertDb(query);
            }
            catch (Exception ex)
            {
                rows = -1;
            }
            return rows;
        }

        //sql.InsertBody(accountNumber, nameCode, opCode, loanCode,
        //amount, fileDate, fileNumber);
        public int InsertBody(string accountNumber, string nameCode, string opCode,
            string loanCode, string amount, string fileDate, string fileNumber)
        {
            int rows = 0;
            try
            {
                if (_conn.State != ConnectionState.Open)
                    _conn.Open();

                string query =
                    $"INSERT INTO tblFas126 (accountNumber, nameCode, opCode, loanCode, amount, fileDate, fileNumber, dateCreated, comment) " + $"VALUES ('{accountNumber}', '{nameCode}','{opCode}','{loanCode}','{amount}','{fileDate}','{fileNumber}','{DateTime.UtcNow}', '{"Body Record"}')";
                rows = InsertDb(query);
            }
            catch (Exception ex)
            {
                rows = -1;
            }
            return rows;
        }
        public int InsertAuditDb(string rawData)
        {
            int rows = 0;
         
            try
            {
                if (_conn.State != ConnectionState.Open)
                    _conn.Open();

                string query =
                    $"INSERT INTO tblAudit (rawData, timeStamp, comment) VALUES ('{rawData}', '{DateTime.UtcNow}', '{"No comment"}')";
                rows = InsertDb(query);
            }
            catch (Exception ex)
            {
                rows = -1;
            }
            return rows;
        }

        // generic, pass in query like insert into tablename(....) values(....)
        private int InsertDb(string query)
        {
            int nRows = 0;
            try
            {
                if (_conn.State != ConnectionState.Open)
                    _conn.Open();

                SqlCommand command = new SqlCommand(query, _conn);

                if (_conn.State == ConnectionState.Closed)
                    _conn.Open();

                nRows = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
            }

            return nRows;
        }

        // generic, select xyz from tablename where abc
        public void QueryDb(string query)
        {
            using (_conn)
            {
                if (_conn.State != ConnectionState.Open)
                    _conn.Open();
                SqlCommand command = new SqlCommand(
                    query,
                    //"SELECT CategoryID, CategoryName FROM Categories;",
                    _conn);

                if (_conn.State == ConnectionState.Closed)
                    _conn.Open();

                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Console.WriteLine("{0}\t{1}", reader.GetString(0),
                            reader.GetString(1));
                    }
                }
                else
                {
                    Console.WriteLine("No rows found.");
                }
                reader.Close();
            }
        }


        //public DbDataReader GetDataReader(string procedureName, List<SqlParameter> parameters, CommandType commandType = CommandType.StoredProcedure)
        //{
        //    DbDataReader dr = null;
        //    try
        //    {
        //        DbConnection connection = this.GetConnection();
        //        {
        //            //DbCommand cmd = this.GetCommand(connection, procedureName, commandType);
        //            //if (parameters != null && parameters.Count > 0)
        //            //{
        //            //    cmd.Parameters.AddRange(parameters.ToArray());
        //            //}
        //            //dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //    return dr;
        //}
    }
}

