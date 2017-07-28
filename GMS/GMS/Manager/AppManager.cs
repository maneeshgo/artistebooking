using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Collections.Specialized;
using System.Web;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Profile;
using GMS.Utils;
using System.Web.Security;
using System.Collections.Generic;
using System.Drawing;
using Newtonsoft.Json;

namespace GMS.Portal
{
    public static class AppManager
    {
        private static bool AppInitializedFlag = false;

        #region BuildHttpContextXML - returns XmlDocument
        public static XmlDocument BuildHttpContextXML()
        {
            string authGuid = string.Empty;
            string anonGuid = string.Empty;
            string IP = string.Empty;
            string browserType = HttpContext.Current.Request.Browser.Type;
            string browserVersion = HttpContext.Current.Request.Browser.Version;

            if (HttpContext.Current.Items["HttpContextXML"] != null)
                return (XmlDocument)HttpContext.Current.Items["HttpContextXML"];

            try
            {
                XmlDocument XmlDoc = new XmlDocument();
                XmlNode RootNode = XmlDoc.CreateElement("HttpContextXML");
                XmlNode QueryStringNode = XmlDoc.CreateElement("QueryString");
                XmlNode FormNode = XmlDoc.CreateElement("Form");
                XmlNode FilesNode = XmlDoc.CreateElement("File");
                XmlNode HttpHeaderNode = XmlDoc.CreateElement("HttpHeader");
                XmlNode ProfileNode = XmlDoc.CreateElement("Profile");

                // Q U E R Y   S T R I N G
                for (int i = 0; i < HttpContext.Current.Request.QueryString.Count; i++)
                {
                    if (HttpContext.Current.Request.QueryString.Keys[i].ToLower().Equals("json"))
                    {
                        try
                        {
                            XmlDocument jsonDoc = (XmlDocument)JsonConvert.DeserializeXmlNode(HttpContext.Current.Request.QueryString[i], "json");
                            XmlNode imported = QueryStringNode.OwnerDocument.ImportNode(jsonDoc.DocumentElement, true);
                            QueryStringNode.AppendChild(imported);
                        }
                        catch (Exception ex)
                        {
                            XmlNode KeyNode = XmlDoc.CreateElement("Param");
                            XmlAttribute NameAttribute = XmlDoc.CreateAttribute("Name");
                            NameAttribute.Value = HttpContext.Current.Request.QueryString.Keys[i];
                            KeyNode.Attributes.Append(NameAttribute);
                            XmlText xmlText = XmlDoc.CreateTextNode(HttpContext.Current.Request.QueryString[i]);
                            KeyNode.AppendChild(xmlText);
                            QueryStringNode.AppendChild(KeyNode);
                        }
                    }
                    else
                    {
                        XmlNode KeyNode = XmlDoc.CreateElement("Param");
                        XmlAttribute NameAttribute = XmlDoc.CreateAttribute("Name");
                        NameAttribute.Value = HttpContext.Current.Request.QueryString.Keys[i];
                        KeyNode.Attributes.Append(NameAttribute);
                        XmlText xmlText = XmlDoc.CreateTextNode(HttpContext.Current.Request.QueryString[i]);
                        KeyNode.AppendChild(xmlText);
                        QueryStringNode.AppendChild(KeyNode);
                    }
                }

                // F O R M
                for (int i = 0; i < HttpContext.Current.Request.Form.Count; i++)
                {
                    if (HttpContext.Current.Request.Form.Keys[i].ToLower().Equals("json"))
                    {
                        try
                        {
                            XmlDocument jsonDoc = (XmlDocument)JsonConvert.DeserializeXmlNode(HttpContext.Current.Request.Form[i], "json");
                            XmlNode imported = FormNode.OwnerDocument.ImportNode(jsonDoc.DocumentElement, true);
                            FormNode.AppendChild(imported);
                        }
                        catch (Exception ex)
                        {
                            XmlNode KeyNode = XmlDoc.CreateElement("Param");
                            XmlAttribute NameAttribute = XmlDoc.CreateAttribute("Name");
                            NameAttribute.Value = HttpContext.Current.Request.Form.Keys[i];
                            KeyNode.Attributes.Append(NameAttribute);
                            XmlText xmlText = XmlDoc.CreateTextNode(HttpContext.Current.Request.Form[i]);
                            KeyNode.AppendChild(xmlText);
                            FormNode.AppendChild(KeyNode);
                        }
                    }
                    else
                    {
                        XmlNode KeyNode = XmlDoc.CreateElement("Param");
                        XmlAttribute NameAttribute = XmlDoc.CreateAttribute("Name");
                        NameAttribute.Value = HttpContext.Current.Request.Form.Keys[i];
                        KeyNode.Attributes.Append(NameAttribute);
                        XmlText xmlText = XmlDoc.CreateTextNode(HttpContext.Current.Request.Form[i]);
                        KeyNode.AppendChild(xmlText);
                        FormNode.AppendChild(KeyNode);
                    }
                }

                // F I L E S
                for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                {
                    if (HttpContext.Current.Request.Files[i].FileName.Length > 0 && HttpContext.Current.Request.Files[i].ContentLength > 0)
                    {
                        XmlNode FileNode = XmlDoc.CreateElement("Item");

                        XmlAttribute NameAttribute = XmlDoc.CreateAttribute("Index");
                        NameAttribute.Value = Convert.ToString(i);
                        FileNode.Attributes.Append(NameAttribute);

                        NameAttribute = XmlDoc.CreateAttribute("FileName");
                        NameAttribute.Value = HttpContext.Current.Request.Files[i].FileName;
                        FileNode.Attributes.Append(NameAttribute);

                        NameAttribute = XmlDoc.CreateAttribute("ContentType");
                        NameAttribute.Value = HttpContext.Current.Request.Files[i].ContentType;
                        FileNode.Attributes.Append(NameAttribute);

                        NameAttribute = XmlDoc.CreateAttribute("ContentLength");
                        NameAttribute.Value = Convert.ToString(HttpContext.Current.Request.Files[i].ContentLength);
                        FileNode.Attributes.Append(NameAttribute);

                        FilesNode.AppendChild(FileNode);
                    }
                }

                // H T T P   H E A D E R - HTTP_HOST
                XmlNode HttpHostNode = XmlDoc.CreateElement("Param");
                XmlAttribute HttpHostAttribute = XmlDoc.CreateAttribute("Name");
                HttpHostAttribute.Value = "HTTP_HOST";
                HttpHostNode.Attributes.Append(HttpHostAttribute);
                XmlText HttpHostNodeXmlText = XmlDoc.CreateTextNode(HttpContext.Current.Request.ServerVariables["HTTP_HOST"]);
                HttpHostNode.AppendChild(HttpHostNodeXmlText);
                HttpHeaderNode.AppendChild(HttpHostNode);

                // H T T P   H E A D E R - REMOTE_ADDR
                HttpHostNode = XmlDoc.CreateElement("Param");
                HttpHostAttribute = XmlDoc.CreateAttribute("Name");
                HttpHostAttribute.Value = "REMOTE_ADDR";
                HttpHostNode.Attributes.Append(HttpHostAttribute);
                IP = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                HttpHostNodeXmlText = XmlDoc.CreateTextNode(IP);
                HttpHostNode.AppendChild(HttpHostNodeXmlText);
                HttpHeaderNode.AppendChild(HttpHostNode);

                // H T T P   H E A D E R - HTTP_REFERER
                HttpHostNode = XmlDoc.CreateElement("Param");
                HttpHostAttribute = XmlDoc.CreateAttribute("Name");
                HttpHostAttribute.Value = "HTTP_REFERER";
                HttpHostNode.Attributes.Append(HttpHostAttribute);
                HttpHostNodeXmlText = XmlDoc.CreateTextNode(HttpContext.Current.Request.ServerVariables["HTTP_REFERER"]);
                HttpHostNode.AppendChild(HttpHostNodeXmlText);
                HttpHeaderNode.AppendChild(HttpHostNode);

                // H T T P   H E A D E R - HTTP_USER_AGENT
                HttpHostNode = XmlDoc.CreateElement("Param");
                HttpHostAttribute = XmlDoc.CreateAttribute("Name");
                HttpHostAttribute.Value = "HTTP_USER_AGENT";
                HttpHostNode.Attributes.Append(HttpHostAttribute);
                HttpHostNodeXmlText = XmlDoc.CreateTextNode(HttpContext.Current.Request.ServerVariables["HTTP_USER_AGENT"]);
                HttpHostNode.AppendChild(HttpHostNodeXmlText);
                HttpHeaderNode.AppendChild(HttpHostNode);

                //// H T T P   H E A D E R - BROWSER
                //HttpHostNode = XmlDoc.CreateElement("Param");
                //HttpHostAttribute = XmlDoc.CreateAttribute("Name");
                //HttpHostAttribute.Value = "BROWSER";
                //HttpHostNode.Attributes.Append(HttpHostAttribute);
                //HttpHostNodeXmlText = XmlDoc.CreateTextNode(HttpContext.Current.Request.Browser.Browser);
                //HttpHostNode.AppendChild(HttpHostNodeXmlText);
                //HttpHeaderNode.AppendChild(HttpHostNode);

                //// H T T P   H E A D E R - BROWSER_VERSION
                //HttpHostNode = XmlDoc.CreateElement("Param");
                //HttpHostAttribute = XmlDoc.CreateAttribute("Name");
                //HttpHostAttribute.Value = "BROWSER_VERSION";
                //HttpHostNode.Attributes.Append(HttpHostAttribute);
                //HttpHostNodeXmlText = XmlDoc.CreateTextNode(HttpContext.Current.Request.Browser.Version);
                //HttpHostNode.AppendChild(HttpHostNodeXmlText);
                //HttpHeaderNode.AppendChild(HttpHostNode);

                /*
                 * <Profile>
                 *      <Auth Guid="" UserName=""></Auth>
                 *      <Anon Guid="" />
                 *      <Cookie Name=""></Cookie>
                 *      <Cookie Name=""></Cookie>
                 * </Profile>
                 * 
                 */

                HttpCookie authCookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];

                if (authCookie != null)
                {
                    FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);

                    string userName = authTicket.Name;
                    authGuid = Membership.GetUser(userName).ProviderUserKey.ToString().ToUpper();
                    HttpContext.Current.Items["Role"] = String.Join(",", Roles.GetRolesForUser());

                    // HttpContext.Current.Items["AuthGuid"] = authGuid;

                    string userData = authTicket.UserData.ToUpper();

                    XmlNode authNode = XmlDoc.CreateElement("Auth");
                    XmlText authText = XmlDoc.CreateTextNode(userData);
                    authNode.AppendChild(authText);

                    XmlAttribute authGuidAttr = XmlDoc.CreateAttribute("Guid");
                    authGuidAttr.Value = authGuid;

                    authNode.Attributes.Append(authGuidAttr);

                    ProfileNode.AppendChild(authNode);
                }
                else
                {
                    XmlNode anonNode = XmlDoc.CreateElement("Anon");

                    XmlAttribute anonGuidAttr = XmlDoc.CreateAttribute("Guid");
                    anonGuid = HttpContext.Current.Request.AnonymousID;
                    anonGuidAttr.Value = anonGuid;

                    anonNode.Attributes.Append(anonGuidAttr);

                    ProfileNode.AppendChild(anonNode);

                    HttpContext.Current.Items["AuthGuid"] = string.Empty;
                    HttpContext.Current.Items["Role"] = string.Empty;// String.Join(",", Roles.GetRolesForUser());
                }

                foreach (string cookieName in HttpContext.Current.Request.Cookies)
                {
                    if (cookieName != ".ASPXANONYMOUS" && cookieName != FormsAuthentication.FormsCookieName)
                    {
                        HttpCookie httpCookie = HttpContext.Current.Request.Cookies[cookieName];

                        XmlNode cookieNode = XmlDoc.CreateElement("Cookie");
                        XmlText cookieText = XmlDoc.CreateTextNode(httpCookie.Value);
                        cookieNode.AppendChild(cookieText);

                        XmlAttribute nameAttr = XmlDoc.CreateAttribute("Name");
                        nameAttr.Value = httpCookie.Name;

                        cookieNode.Attributes.Append(nameAttr);

                        ProfileNode.AppendChild(cookieNode);
                    }
                }

                // F I N A L I Z E
                RootNode.AppendChild(QueryStringNode);
                RootNode.AppendChild(FormNode);
                RootNode.AppendChild(FilesNode);
                RootNode.AppendChild(HttpHeaderNode);
                RootNode.AppendChild(ProfileNode);
                XmlDoc.AppendChild(RootNode);

                // set HttpContextXML to the current HttpContext for the next call
                HttpContext.Current.Items["HttpContextXML"] = XmlDoc;
                if (Constants.IsHttpContextXMLLoggingEnabled)
                {
                    DataManager.LogHttpContextXML(HttpContext.Current.Request.FilePath, authGuid, anonGuid, IP, browserType, browserVersion);
                }

                return XmlDoc;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion BuildHttpContextXML

        #region InitializeWebApplication: Initializes web application
        public static bool InitializeWebApplication()
        {
            // if already initialized then exit
            if (AppInitializedFlag)
                return AppInitializedFlag;

            string logPathFromConfig = string.Empty;
            string logPhysicalPath = System.Web.HttpContext.Current.Request.PhysicalApplicationPath;
            string uploadPathFromConfig = string.Empty;
            string uploadPhysicalPath = System.Web.HttpContext.Current.Request.PhysicalApplicationPath;
            string imageResizingFromConfig = string.Empty;
            string imageSizesFromConfig = string.Empty;
            string siteCode = string.Empty;
            string connStrCode = string.Empty;
            string siteURL = string.Empty;
            string httpContextXMLLogging = string.Empty;
            string debugMode = string.Empty;

            try
            {
                //URLSectionHandler objURLSectionHandler = (URLSectionHandler)System.Configuration.ConfigurationManager.GetSection("URLSection");
                //SiteSectionHandler objSiteSectionHandler = (SiteSectionHandler)System.Configuration.ConfigurationManager.GetSection("SiteSection");

                siteURL = System.Web.HttpContext.Current.Request.Url.Authority;

                //if (objURLSectionHandler.collection.Item(siteURL) == null)
                //{
                //    throw new Exception(string.Format(@"Cannot initialize web application. SiteCode cannot be found in web.config for the URL:({0}).", siteURL));
                //}

                siteCode = "asansor_dev";

                //if (String.IsNullOrEmpty(siteCode))
                //{
                //    throw new Exception(string.Format(@"Cannot initialize web application. SiteCode cannot be found in web.config for the URL:({0}).", siteURL));
                //}

                GMS.Utils.Constants.SiteCode = "";
                GMS.Utils.Constants.AppCode = "";
                GMS.Utils.Constants.DBCode = "";

                httpContextXMLLogging = "TRUE"; // TRUE OR FALSE // 0 or 1 // YES or NO

                if (httpContextXMLLogging.Equals("0") || httpContextXMLLogging.Equals("FALSE") || httpContextXMLLogging.Equals("NO") || httpContextXMLLogging.Equals("OFF") || httpContextXMLLogging.Equals("DISABLED"))
                    GMS.Utils.Constants.IsHttpContextXMLLoggingEnabled = false;
                else
                    GMS.Utils.Constants.IsHttpContextXMLLoggingEnabled = true;

                connStrCode = "asansor_dev";
                logPathFromConfig = "PortalLog";
                uploadPathFromConfig = "/FileStore";
                imageResizingFromConfig = "true";
                imageSizesFromConfig = "50,100,300,600,1000";

                if (String.IsNullOrEmpty(connStrCode))
                {
                    throw new Exception(string.Format(@"Cannot initialize web application. Connection string information cannot be found in web.config for the Site Code:({0}).", siteCode));
                }

                GMS.Utils.Constants.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings[connStrCode].ToString();

                // validate connection string
                try
                {
                    DbUtil dbUtil = new DbUtil();

                    if (!dbUtil.Connect())
                    {
                        throw new Exception(string.Format(@"Cannot initialize web application. Cannot connect to the database server:({0}).", connStrCode));
                    }

                }
                catch
                {
                    throw new Exception(string.Format(@"Cannot initialize web application. Cannot connect to the database server:({0}).", connStrCode));
                }

                try
                {
                    logPhysicalPath = System.Web.HttpContext.Current.Server.MapPath(logPathFromConfig);

                    if (!Directory.Exists(logPhysicalPath))
                    {
                        logPhysicalPath = System.Web.HttpContext.Current.Request.PhysicalApplicationPath;
                    }

                    GMS.Utils.Constants.LogPath = logPhysicalPath;

                }
                catch (Exception ex)
                {
                    logPhysicalPath = System.Web.HttpContext.Current.Request.PhysicalApplicationPath;
                    GMS.Utils.Constants.LogPath = logPhysicalPath;
                }

                try
                {
                    uploadPhysicalPath = System.Web.HttpContext.Current.Server.MapPath(uploadPathFromConfig);

                    if (!Directory.Exists(uploadPhysicalPath))
                    {
                        uploadPhysicalPath = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "upload";
                        Directory.CreateDirectory(uploadPhysicalPath);
                        GMS.Utils.Constants.RelativeUploadPath = "/upload";
                    }
                    else
                    {
                        GMS.Utils.Constants.RelativeUploadPath = uploadPathFromConfig;
                    }

                    GMS.Utils.Constants.UploadPath = uploadPhysicalPath;

                }
                catch (Exception ex)
                {
                    uploadPhysicalPath = System.Web.HttpContext.Current.Request.PhysicalApplicationPath;
                    GMS.Utils.Constants.UploadPath = uploadPhysicalPath;
                }

                if (String.IsNullOrEmpty(imageResizingFromConfig))
                {
                    Constants.ImageResizing = false;
                }
                else
                {
                    switch (imageResizingFromConfig.ToLower())
                    {
                        case "true":
                        case "yes":
                        case "on":
                            Constants.ImageResizing = true;
                            string imageUploadPhysicalPath = uploadPhysicalPath + @"\images";
                            if (!Directory.Exists(imageUploadPhysicalPath))
                            {
                                Directory.CreateDirectory(imageUploadPhysicalPath);
                                Directory.CreateDirectory(imageUploadPhysicalPath + @"\original");
                            }
                            break;
                        default:
                            Constants.ImageResizing = false;
                            break;
                    }
                }

                if (Constants.ImageResizing == true)
                {
                    if (!String.IsNullOrEmpty(imageSizesFromConfig))
                    {
                        string[] sizes = imageSizesFromConfig.Split(',');
                        foreach (string size in sizes)
                        {
                            string _size = size.Trim();
                            string directory = uploadPhysicalPath + @"\images\" + _size;
                            if (!Directory.Exists(directory))
                            {
                                Directory.CreateDirectory(directory);
                            }
                        }
                    }
                }

                if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["ERR_MESSAGE"]))
                    Constants.ERR_MESSAGE = System.Configuration.ConfigurationManager.AppSettings["ERR_MESSAGE"].ToString();

                AppInitializedFlag = true;

                return AppInitializedFlag;
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        #endregion
    }
}
