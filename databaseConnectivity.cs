using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using GraphSpace;
using locationSpace;
using ServiceSpace;
namespace DBNamespace
{
    class databaseConnectivity
    {


            System.Data.OleDb.OleDbConnection conn;
            OleDbCommand command;
            public void connectToDatabase()
            {
                conn = new System.Data.OleDb.OleDbConnection();
                // TODO: Modify the connection string and include any
                // additional required properties for your database.
                conn.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;" +
                    @"Data source= " +
                    @"emap.mdb";
                try
                {

                    // Insert code to process data.
                    //MessageBox.Show("Connection Established");
                    command = new OleDbCommand();
                    conn.Close();
                }
                catch (Exception ex)
                {
                   // MessageBox.Show(ex.Message);
                }
                finally
                {
                    conn.Close();
                }


            }


            public void insertToLocation(int id, string loc, int xcord, int ycord)
            {
                command.CommandText = "INSERT INTO location VALUES ('" + id + "','" + loc + "','" + xcord + "','" + ycord + "')";
                conn.Open();
                command.Connection = conn;
                command.ExecuteNonQuery();
                conn.Close();

            }

            public void readLocation(Location []locArray)
            {
                command.CommandText = "select * from location";
                conn.Close();
                conn.Open();
                command.Connection = conn;
                OleDbDataReader reader = command.ExecuteReader();
               // int index=0;              
                while (reader.Read())
                {
                    locArray[Convert.ToInt32(reader[0])] = new Location(Convert.ToInt32(reader[0]), reader[1].ToString(), Convert.ToInt32(reader[2]), Convert.ToInt32(reader[3]));

                }
                    conn.Close();

            }

            public void readEdgeTable( LocationNode graph,Location[] locArray)

            {
                command.CommandText = "select * from EdgeTable";
                conn.Close();
                conn.Open();
                command.Connection = conn;
                OleDbDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    graph.addEdge( Convert.ToInt32(reader[0]),Convert.ToInt32(reader[1]),locArray);

                }

            }

            public void readService(Services[] serviceArray)
            {
                command.CommandText = "select * from service where service not like 'nil'";
                conn.Close();
                conn.Open();
                command.Connection = conn;
                OleDbDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    
                  serviceArray[Convert.ToInt32(reader[0])]=new Services(Convert.ToInt32(reader[0]),(reader[1]).ToString());

                }

            }
            public int getEdgeCount()
            {
                command.CommandText = "select count(*) from EdgeTable";
                conn.Open();
                command.Connection = conn;
                OleDbDataReader reader = command.ExecuteReader();
                reader.Read();
                return   Convert.ToInt32( reader[0]);

            }
            public int getLocationCount()
            {
                command.CommandText = "select count(*) from location";
                conn.Close();
                conn.Open();
                command.Connection = conn;
                OleDbDataReader reader = command.ExecuteReader();
                reader.Read();
                return Convert.ToInt32(reader[0]);

            }
            void getSourceDestination()
            {

            }


        }

    }

