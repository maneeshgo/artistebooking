using System;
using System.Collections;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.IO;
using GMS.Utils;
using System.Configuration;

namespace GMS.Utils
{
    /// <summary>
    /// DbUtil 
    /// </summary>
    public class DbUtil
    {
        private string SqlConnStr = string.Empty;

        private int timeOut = 6000;

        public DbUtil()
        {
            try
            {
                this.SqlConnStr = this.GetConnectionString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetConnectionString()
        {
            return Constants.ConnectionString;
        }

        public DataSet ExecuteCommandReturnDataSet(SqlCommand _sqlCommand)
        {
            SqlConnection objConn = null;

            try
            {
                // Get a connection object which will have an open connection throughout process.
                objConn = GetConnection();

                // associate the connection with the Command Object
                _sqlCommand.Connection = objConn;


                DataSet dataSet = new DataSet();

                SqlDataAdapter adapter = new SqlDataAdapter(_sqlCommand);
                adapter.Fill(dataSet);
                return dataSet;
            }
            catch (SqlException sqlEx)
            {
                string errLogText = "Error executing Sql script '" + _sqlCommand.CommandText + "':" + "\n" + GetSqlErrors(sqlEx);
                // string errLogText = "Error executing Sql script '" + _sqlCommandText + "':" + "\n" + GetSqlErrors(sqlEx);
                // log error to event log (errLogText)
                throw sqlEx;	//throw error to upper levels
            }
            catch (Exception ex)
            {
                string errLogText = "Error executing Sql script '" + _sqlCommand.CommandText + "':";
                throw ex;
            }
            finally
            {
                objConn.Close();
                _sqlCommand.Dispose();
            }
        }//ExecuteCommand

        public DataTable ExecuteCommand(SqlCommand _sqlCommand)
        {
            SqlConnection objConn = null;

            try
            {
                // Get a connection object which will have an open connection throughout process.
                objConn = GetConnection();

                // associate the connection with the Command Object
                _sqlCommand.Connection = objConn;
                DataTable dataTable = new DataTable();

                SqlDataAdapter adapter = new SqlDataAdapter(_sqlCommand);
                adapter.Fill(dataTable);
                return dataTable;
            }
            catch (SqlException sqlEx)
            {
                string errLogText = "Error executing Sql script '" + _sqlCommand.CommandText + "':" + "\n" + GetSqlErrors(sqlEx);
                // string errLogText = "Error executing Sql script '" + _sqlCommandText + "':" + "\n" + GetSqlErrors(sqlEx);
                // log error to event log (errLogText)
                throw sqlEx;  //throw error to upper levels
            }
            catch (Exception ex)
            {
                string errLogText = "Error executing Sql script '" + _sqlCommand.CommandText + "':";
                throw ex;
            }
            finally
            {
                objConn.Close();
                _sqlCommand.Dispose();
            }
        }//ExecuteCommand

        public void ExecuteCommandNoReturn(SqlCommand _sqlCommand)
        {
            SqlConnection objConn = null;

            try
            {
                // Get a connection object which will have an open connection throughout process.
                objConn = GetConnection();

                // associate the connection with the Command Object
                _sqlCommand.Connection = objConn;
                _sqlCommand.ExecuteNonQuery();
            }
            catch (SqlException sqlEx)
            {
                string errLogText = "Error executing Sql script '" + _sqlCommand.CommandText + "':" + "\n" + GetSqlErrors(sqlEx);
                // string errLogText = "Error executing Sql script '" + _sqlCommandText + "':" + "\n" + GetSqlErrors(sqlEx);
                // log error to event log (errLogText)
                throw sqlEx;	//throw error to upper levels
            }
            catch (Exception ex)
            {
                string errLogText = "Error executing Sql script '" + _sqlCommand.CommandText + "':";
                throw ex;
            }
            finally
            {
                objConn.Close();
                _sqlCommand.Dispose();
            }
        }//ExecuteCommand

        public string ExecuteCommandReturnXML(SqlCommand _sqlCommand)
        {
            SqlConnection objConn = null;
            StringBuilder xmlText = new StringBuilder();

            try
            {
                // Get a connection object which will have an open connection throughout process.
                objConn = GetConnection();

                // associate the connection with the Command Object
                _sqlCommand.Connection = objConn;
                _sqlCommand.CommandTimeout = 1800;
                SqlDataReader reader = _sqlCommand.ExecuteReader();
                while (reader.Read())   // i must be dumb
                {
                    //Get the XML value as string
                    xmlText.Append((string)reader.GetValue(0));

                }
                // else
                //	return null;
                return xmlText.ToString();
            }
            catch (SqlException sqlEx)
            {

                string errLogText = "Error executing Sql script '" + _sqlCommand.CommandText + "':" + "\n" + GetSqlErrors(sqlEx);

                // log error to event log (errLogText)
                throw sqlEx;  //throw error to upper levels
            }
            catch (Exception ex)
            {
                string errLogText = "Error executing Sql script '" + _sqlCommand.CommandText + "':";
                throw ex;
            }
            finally
            {
                objConn.Close();
                _sqlCommand.Dispose();
            }
        }//ExecuteCommand

        // returns no value
        private void ExecuteCommand(SqlConnection pobjConn, string _sqlCommandText)
        {
            try
            {
                SqlCommand sqlCommand = new SqlCommand(_sqlCommandText, pobjConn);
                sqlCommand.CommandTimeout = timeOut;
                sqlCommand.ExecuteNonQuery();

                sqlCommand.Dispose();
            }

            catch (SqlException sqlEx)
            {
                if (_sqlCommandText == null)
                    _sqlCommandText = "(null)";

                string errLogText = "Error executing Sql script '" + _sqlCommandText + "':" + "\n" + GetSqlErrors(sqlEx);


                // log error to event log (errLogText)
                throw sqlEx;  //throw error to upper levels
            }
            catch (Exception ex)
            {
                string errLogText = "Error executing Sql script '" + _sqlCommandText + "':";
                throw ex;
            }
        }//ExecuteCommand


        // returns also a populated Dataaset
        private void ExecuteCommand(SqlConnection pobjConn, string _sqlCommandText, ref DataSet _dataSet)
        {
            try
            {
                SqlCommand sqlCommand = new SqlCommand(_sqlCommandText, pobjConn);
                sqlCommand.CommandTimeout = timeOut;
                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = sqlCommand;

                _dataSet = new DataSet();
                adapter.Fill(_dataSet);

                sqlCommand.Dispose();
            }
            catch (SqlException sqlEx)
            {
                if (_sqlCommandText == null)
                    _sqlCommandText = "(null)";

                string errLogText = "Error executing Sql script '" + _sqlCommandText + "':" + "\n" + GetSqlErrors(sqlEx);

                // log error to event log (errLogText)
                throw sqlEx;  //throw error to upper levels
            }
            catch (Exception ex)
            {
                string errLogText = "Error executing Sql script '" + _sqlCommandText + "':";
                throw ex;
            }

        }//ExecuteCommand

        private SqlConnection GetConnection()
        {
            try
            {
                SqlConnection sqlConnection = new SqlConnection(SqlConnStr);
                sqlConnection.Open();
                return sqlConnection;
            }
            catch (SqlException sqlEx)
            {
                string errLogText = string.Format("Error opening Sql connection.Connection String:{0}\nErrors:{1}", SqlConnStr, GetSqlErrors(sqlEx));
                // log error to event log (errLogText)
                throw sqlEx;  //throw error to upper levels
            }
            catch (Exception ex)
            {
                string errLogText = "Error opening Sql connection:";
                ex.GetHashCode();
                throw ex;
            }
        }

        //this method executes SQL separated by "GO" batch separator (similar to what Query Analyzer does)

        public void ExecuteSQLBatch(string _sqlBatch, Hashtable _replacementDict)
        {
            SqlConnection objConn = null;

            try
            {
                // Get a connection object which will have an open connection throughout process.
                objConn = GetConnection();

                // replace parameters if any
                if (_replacementDict != null)
                {
                    //_sqlBatch = this.ReplaceDictinaryParameters(_sqlBatch, _replacementDict);
                }

                // instantiate RegEx for SQL splitting
                Regex myRegex = new Regex(@"\bgo\b\s*\r\n?", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

                // Split SQL file from "GO" statements
                String[] arrSQLStatements = myRegex.Split(_sqlBatch);

                // Loop for all SQL batches
                for (int i = 0; i < arrSQLStatements.Length; i++)
                {
                    if (arrSQLStatements[i].Length > 1)
                    {
                        this.ExecuteCommand(objConn, arrSQLStatements[i]);
                    }
                }//for				

            }
            catch (Exception ex)
            {
                if (_sqlBatch == null)
                    _sqlBatch = "(null)";

                string errLogText = "Error executing Sql script '" + _sqlBatch + "':" + "\n" + ex.Message;
                // log error to event log (errLogText)
                throw ex;  //throw error to upper levels	
            }
            finally
            {
                // close connection whenever you are done.
                if ((objConn != null) && (objConn.State == ConnectionState.Open))
                    objConn.Close();

                objConn.Dispose();
            }
        }


        // checks SQL connectivity, if can connect returns true
        public bool Connect()
        {
            try
            {
                SqlConnection sqlConnection = new SqlConnection(SqlConnStr);
                sqlConnection.Open();
                sqlConnection.Close();
                return true;
            }
            catch (SqlException sqlEx)
            {
                string errLogText = string.Format("Cannot connect to the db server. \n. Errors:{0}", GetSqlErrors(sqlEx));
                // log error to event log (errLogText)
                throw sqlEx;  //throw error to upper levels
            }
            catch (Exception ex)
            {
                string errLogText = string.Format("Cannot connect to the db server. \n. - ");
                throw ex;
            }
        }


        // returns no value
        public void ExecuteNonQuery(string _sqlCommandText)
        {
            SqlConnection objConn = null;

            try
            {
                // Get a connection object which will have an open connection throughout process.
                objConn = GetConnection();
                this.ExecuteCommand(objConn, _sqlCommandText);
                objConn.Close();
            }

            catch (SqlException sqlEx)
            {
                if (_sqlCommandText == null)
                    _sqlCommandText = "(null)";

                string errLogText = "Error executing Sql script '" + _sqlCommandText + "':" + "\n" + GetSqlErrors(sqlEx);

                // log error to event log (errLogText)
                throw sqlEx;  //throw error to upper levels
            }
            catch (Exception ex)
            {
                string errLogText = "Error executing Sql script '" + _sqlCommandText + "':";
                throw ex;
            }
        }//ExecuteCommand

        // returns no value
        public void ExecuteNonQuery(SqlCommand _sqlCommand)
        {
            SqlConnection objConn = null;

            try
            {
                // Get a connection object which will have an open connection throughout process.
                objConn = GetConnection();
                if (objConn.State != ConnectionState.Open) objConn.Open();
                _sqlCommand.Connection = objConn;
                _sqlCommand.ExecuteNonQuery();
            }

            catch (SqlException sqlEx)
            {
                if (_sqlCommand.CommandText == null)
                    _sqlCommand.CommandText = "(null)";

                string errLogText = "Error executing Sql script '" + _sqlCommand.CommandText + "':" + "\n" + GetSqlErrors(sqlEx);

                // log error to event log (errLogText)
                throw sqlEx;  //throw error to upper levels
            }
            catch (Exception ex)
            {
                string errLogText = "Error executing Sql script '" + _sqlCommand.CommandText + "':";
                throw ex;
            }
            finally
            {
                if (objConn.State != ConnectionState.Closed)
                {
                    objConn.Close();
                }
            }
        }//ExecuteCommand

        public void ExecuteSQL(string _sqlCommandText, ref DataSet _dataset)
        {
            SqlConnection objConn = null;

            try
            {
                // Get a connection object which will have an open connection throughout process.
                objConn = GetConnection();

                if (_sqlCommandText.Length > 1)
                {
                    this.ExecuteCommand(objConn, _sqlCommandText, ref _dataset);
                }
            }

            catch (SqlException sqlEx)
            {
                if (_sqlCommandText == null)
                    _sqlCommandText = "(null)";

                string errLogText = "Error executing Sql script '" + _sqlCommandText + "':" + "\n" + GetSqlErrors(sqlEx);

                // log error to event log (errLogText)
                throw sqlEx;  //throw error to upper levels
            }
            catch (Exception ex)
            {
                if (_sqlCommandText == null)
                    _sqlCommandText = "(null)";

                string errLogText = "Error executing SQL: " + _sqlCommandText + ex.Message;

                // log error to event log (errLogText)
                throw ex;  //throw error to upper levels
            }

            finally
            {
                // close connection whenever you are done.
                if ((objConn != null) && (objConn.State == ConnectionState.Open))
                {
                    objConn.Close();
                    objConn.Dispose();
                }
            }
        }

        private string GetSqlErrors(SqlException myException)
        {
            try
            {
                StringBuilder errorText = new StringBuilder();
                for (int i = 0; i < myException.Errors.Count; i++)
                    errorText.Append("Index #" + i + "\n" + "Error: " + myException.Errors[i].ToString() + "\n");

                return errorText.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // this function is used only if SQL returns 0ne-row one-column recordset
        public string ExecuteScalar(string _sqlCommandText)
        {
            SqlConnection objConn = null;
            SqlCommand sqlCommand = null;

            try
            {
                // Get a connection object which will have an open connection throughout process.
                objConn = GetConnection();

                if (_sqlCommandText.Length > 1)
                {
                    sqlCommand = new SqlCommand(_sqlCommandText, objConn);
                    sqlCommand.CommandTimeout = timeOut;

                    object execResult = sqlCommand.ExecuteScalar();
                    if (execResult == null)
                        return string.Empty;
                    else
                        return execResult.ToString();
                }
                else
                {

                    throw new Exception("Empty sql script found");
                }
            }
            catch (SqlException sqlEx)
            {
                if (_sqlCommandText == null)
                    _sqlCommandText = "(null)";

                string errLogText = "Error executing Sql script '" + _sqlCommandText + "':" + "\n" + GetSqlErrors(sqlEx);

                // log error to event log (errLogText)
                throw sqlEx;  //throw error to upper levels				
            }
            catch (Exception ex)
            {
                if (_sqlCommandText == null)
                    _sqlCommandText = "(null)";

                string errLogText = "Error executing SQL: " + _sqlCommandText + ex.Message;

                // log error to event log (errLogText)
                throw ex;  //throw error to upper levels
            }

            finally
            {
                sqlCommand.Dispose();
                // close connection whenever you are done.
                if ((objConn != null) && (objConn.State == ConnectionState.Open))
                {
                    objConn.Close();
                    objConn.Dispose();
                }
            } // try..finally
        } // ExecuteScalar      
    } // class DbUtil


}
