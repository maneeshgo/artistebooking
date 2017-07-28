using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Text;
using GMS.Utils;
using System.Xml;
using System.Web;
using System.IO;
using System.Collections.Generic;

namespace GMS.Portal
{
    public static class DataManager
    {
        public static string GetWebPartXml(string webPartName, XmlDocument httpContextXml, string authGuid, string anonGuid, string role)
        {
            DbUtil dbUtil = new DbUtil();
            string xmlText = string.Empty;

            SqlCommand sqlCommand = new SqlCommand("GetWebPartXML");
            sqlCommand.CommandType = CommandType.StoredProcedure;
            try
            {
                sqlCommand.Parameters.Add("@WebPartName", SqlDbType.VarChar, 100);
                sqlCommand.Parameters.Add("@HttpContextXML", SqlDbType.Xml);
                sqlCommand.Parameters.Add("@SiteCode", SqlDbType.VarChar, 100);
                sqlCommand.Parameters.Add("@AppCode", SqlDbType.VarChar, 100);
                sqlCommand.Parameters.Add("@DBCode", SqlDbType.VarChar, 100);
                sqlCommand.Parameters.Add("@AuthGUID", SqlDbType.UniqueIdentifier);
                sqlCommand.Parameters.Add("@AnonGUID", SqlDbType.UniqueIdentifier);
                sqlCommand.Parameters.Add("@Role", SqlDbType.VarChar, 100);

                // sqlCommand.Parameters["@HttpContextXML"].Direction = ParameterDirection.InputOutput;
                sqlCommand.Parameters["@WebPartName"].Value = webPartName;
                sqlCommand.Parameters["@HttpContextXML"].Value = httpContextXml.OuterXml;
                sqlCommand.Parameters["@SiteCode"].Value = Constants.SiteCode;
                sqlCommand.Parameters["@AppCode"].Value = Constants.AppCode;
                sqlCommand.Parameters["@DBCode"].Value = Constants.DBCode;

                if (Constants.IsGuid(authGuid))
                    sqlCommand.Parameters["@AuthGUID"].Value = new Guid(authGuid);

                if (Constants.IsGuid(anonGuid))
                    sqlCommand.Parameters["@AnonGUID"].Value = new Guid(anonGuid);

                sqlCommand.Parameters["@Role"].Value = role;
                xmlText = dbUtil.ExecuteCommandReturnXML(sqlCommand);
            }
            catch (SqlException ex)
            {
                throw;
            }
            finally
            {
                if (sqlCommand == null)
                    sqlCommand.Dispose();
            }
            return xmlText;
        }

        public static DataSet GetWebPartDataSet(string webPartName, XmlDocument httpContextXml, string authGuid, string anonGuid, string role)
        {
            DbUtil dbUtil = new DbUtil();
            string xmlText = string.Empty;
            DataSet dataSet = null;

            SqlCommand sqlCommand = new SqlCommand("GetWebPartXML");
            sqlCommand.CommandType = CommandType.StoredProcedure;
            try
            {
                sqlCommand.Parameters.Add("@WebPartName", SqlDbType.VarChar, 100);
                sqlCommand.Parameters.Add("@HttpContextXML", SqlDbType.Xml);
                sqlCommand.Parameters.Add("@SiteCode", SqlDbType.VarChar, 100);
                sqlCommand.Parameters.Add("@AppCode", SqlDbType.VarChar, 100);
                sqlCommand.Parameters.Add("@DBCode", SqlDbType.VarChar, 100);
                sqlCommand.Parameters.Add("@AuthGUID", SqlDbType.UniqueIdentifier);
                sqlCommand.Parameters.Add("@AnonGUID", SqlDbType.UniqueIdentifier);
                sqlCommand.Parameters.Add("@Role", SqlDbType.VarChar, 100);

                // sqlCommand.Parameters["@HttpContextXML"].Direction = ParameterDirection.InputOutput;
                sqlCommand.Parameters["@WebPartName"].Value = webPartName;
                sqlCommand.Parameters["@HttpContextXML"].Value = httpContextXml.OuterXml;
                sqlCommand.Parameters["@SiteCode"].Value = Constants.SiteCode;
                sqlCommand.Parameters["@AppCode"].Value = Constants.AppCode;
                sqlCommand.Parameters["@DBCode"].Value = Constants.DBCode;

                if (Constants.IsGuid(authGuid))
                    sqlCommand.Parameters["@AuthGUID"].Value = new Guid(authGuid);

                if (Constants.IsGuid(anonGuid))
                    sqlCommand.Parameters["@AnonGUID"].Value = new Guid(anonGuid);

                sqlCommand.Parameters["@Role"].Value = role;
                dataSet = dbUtil.ExecuteCommandReturnDataSet(sqlCommand);
            }
            catch (SqlException ex)
            {
                throw;
            }
            finally
            {
                if (sqlCommand == null)
                    sqlCommand.Dispose();
            }
            return dataSet;
        }

        public static DataTable ValidateUploadTicket(Guid fileTicket, string authGuid, string anonGuid, string role)
        {
            DbUtil dbUtil = new DbUtil();
            DataTable dt = null;

            XmlDocument httpContextXml = GMS.Portal.AppManager.BuildHttpContextXML();

            SqlCommand sqlCommand = new SqlCommand("ValidateUploadTicket");
            sqlCommand.CommandType = CommandType.StoredProcedure;

            try
            {
                sqlCommand.Parameters.Add("@HttpContextXML", SqlDbType.Xml);
                sqlCommand.Parameters.Add("@SiteCode", SqlDbType.VarChar, 100);
                sqlCommand.Parameters.Add("@AppCode", SqlDbType.VarChar, 100);
                sqlCommand.Parameters.Add("@DBCode", SqlDbType.VarChar, 100);
                sqlCommand.Parameters.Add("@FileTicket", SqlDbType.UniqueIdentifier);
                sqlCommand.Parameters.Add("@Role", SqlDbType.VarChar, 100);
                sqlCommand.Parameters.Add("@AuthGUID", SqlDbType.UniqueIdentifier);
                sqlCommand.Parameters.Add("@AnonGUID", SqlDbType.UniqueIdentifier);

                sqlCommand.Parameters["@HttpContextXML"].Value = httpContextXml.OuterXml;
                sqlCommand.Parameters["@SiteCode"].Value = Constants.SiteCode;
                sqlCommand.Parameters["@AppCode"].Value = Constants.AppCode;
                sqlCommand.Parameters["@DBCode"].Value = Constants.DBCode;
                sqlCommand.Parameters["@FileTicket"].Value = fileTicket;
                sqlCommand.Parameters["@Role"].Value = role;

                if (Constants.IsGuid(authGuid))
                    sqlCommand.Parameters["@AuthGUID"].Value = new Guid(authGuid);

                if (Constants.IsGuid(anonGuid))
                    sqlCommand.Parameters["@AnonGUID"].Value = new Guid(anonGuid);

                dt = dbUtil.ExecuteCommand(sqlCommand);
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (sqlCommand == null)
                    sqlCommand.Dispose();
            }
            return dt;
        }

        public static void SaveUploadedFile(Guid fileGuid, string fileName, string fileType, string fileExt, int fileSize, string authGuid, string anonGuid)
        {
            DbUtil dbUtil = new DbUtil();

            XmlDocument httpContextXml = GMS.Portal.AppManager.BuildHttpContextXML();

            SqlCommand sqlCommand = new SqlCommand("SaveUploadedFile");
            sqlCommand.CommandType = CommandType.StoredProcedure;

            try
            {
                sqlCommand.Parameters.Add("@HttpContextXML", SqlDbType.Xml);
                sqlCommand.Parameters.Add("@SiteCode", SqlDbType.VarChar, 100);
                sqlCommand.Parameters.Add("@AppCode", SqlDbType.VarChar, 100);
                sqlCommand.Parameters.Add("@DBCode", SqlDbType.VarChar, 100);
                sqlCommand.Parameters.Add("@GUID", SqlDbType.UniqueIdentifier);
                sqlCommand.Parameters.Add("@FileName", SqlDbType.VarChar, 200);
                sqlCommand.Parameters.Add("@FileType", SqlDbType.VarChar, 100);
                sqlCommand.Parameters.Add("@FileExt", SqlDbType.VarChar, 100);
                sqlCommand.Parameters.Add("@FileSize", SqlDbType.Int, 100);
                sqlCommand.Parameters.Add("@AuthGUID", SqlDbType.UniqueIdentifier);
                sqlCommand.Parameters.Add("@AnonGUID", SqlDbType.UniqueIdentifier);

                sqlCommand.Parameters["@HttpContextXML"].Value = httpContextXml.OuterXml;
                sqlCommand.Parameters["@SiteCode"].Value = Constants.SiteCode;
                sqlCommand.Parameters["@AppCode"].Value = Constants.AppCode;
                sqlCommand.Parameters["@DBCode"].Value = Constants.DBCode;
                sqlCommand.Parameters["@GUID"].Value = fileGuid;
                sqlCommand.Parameters["@FileName"].Value = fileName;
                sqlCommand.Parameters["@FileType"].Value = fileType;
                sqlCommand.Parameters["@FileExt"].Value = fileExt;
                sqlCommand.Parameters["@FileSize"].Value = fileSize;

                if (Constants.IsGuid(authGuid))
                    sqlCommand.Parameters["@AuthGUID"].Value = new Guid(authGuid);

                if (Constants.IsGuid(anonGuid))
                    sqlCommand.Parameters["@AnonGUID"].Value = new Guid(anonGuid);

                dbUtil.ExecuteCommandNoReturn(sqlCommand);
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (sqlCommand == null)
                    sqlCommand.Dispose();
            }
        }

        public static void LogHttpContextXML(string pageName, string authGuid, string anonGuid, string IP, string browserType, string browserVersion)
        {
            DbUtil dbUtil = new DbUtil();

            XmlDocument httpContextXml = GMS.Portal.AppManager.BuildHttpContextXML();

            SqlCommand sqlCommand = new SqlCommand("LogHttpContextXML");
            sqlCommand.CommandType = CommandType.StoredProcedure;

            try
            {
                sqlCommand.Parameters.Add("@HttpContextXML", SqlDbType.Xml);
                sqlCommand.Parameters.Add("@SiteCode", SqlDbType.VarChar, 100);
                sqlCommand.Parameters.Add("@PageName", SqlDbType.VarChar, 200);
                sqlCommand.Parameters.Add("@IP", SqlDbType.VarChar, 20);
                sqlCommand.Parameters.Add("@AuthGuid", SqlDbType.UniqueIdentifier);
                sqlCommand.Parameters.Add("@AnonGuid", SqlDbType.UniqueIdentifier);
                sqlCommand.Parameters.Add("@BrowserType", SqlDbType.VarChar, 100);
                sqlCommand.Parameters.Add("@BrowserVersion", SqlDbType.VarChar, 100);

                sqlCommand.Parameters["@HttpContextXML"].Value = httpContextXml.OuterXml;
                sqlCommand.Parameters["@SiteCode"].Value = Constants.SiteCode;
                sqlCommand.Parameters["@PageName"].Value = pageName;
                sqlCommand.Parameters["@IP"].Value = IP;
                sqlCommand.Parameters["@BrowserType"].Value = browserType;
                sqlCommand.Parameters["@BrowserVersion"].Value = browserVersion;

                if (Constants.IsGuid(authGuid))
                    sqlCommand.Parameters["@AuthGuid"].Value = new Guid(authGuid);

                if (Constants.IsGuid(anonGuid))
                    sqlCommand.Parameters["@AnonGuid"].Value = new Guid(anonGuid);

                dbUtil.ExecuteCommandNoReturn(sqlCommand);
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (sqlCommand == null)
                    sqlCommand.Dispose();
            }
        }

    }
}
