using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Net;
using System.Web.Security;
using System.Data;

using GMS.Portal;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using GMS.Utils;
using System.Collections;
using System.Web.UI.HtmlControls;

namespace GMS.Control
{
    #region enum declarations for WebPart properties
    public enum DataSourceTypeEnum { None, HttpXml, LocalXml, DbXml, ContextXml, StringXml, DbDataSet }
    public enum OutputRenderTypeEnum { None, Html, Xslt, Json }
    #endregion

    #region XmlLoadedEventArgs class: holds WebPartXml data for XmlLoadedEvent
    // The class to holds the webPartXml data loaded for the webpart    
    public class XmlLoadedEventArgs : EventArgs
    {
        public readonly XmlDocument WebPartXml;
        public XmlLoadedEventArgs(XmlDocument webPartXML)
        {
            this.WebPartXml = webPartXML;
        }

    }
    #endregion XmlLoadedEventArgs class: holds WebPartXml data for XmlLoadedEvent

    #region XsltTransformEventArgs class: holds XslArgs for XsltTransformEvent
    public class XsltTransformEventArgs : EventArgs
    {
        public readonly XsltArgumentList XslArgs;
        public XsltTransformEventArgs(XsltArgumentList xslArgs)
        {
            this.XslArgs = xslArgs;
        }
    }
    #endregion XsltTransformEventArgs class

    #region WebPart class
    [Description("GMS Webpart"), ToolboxData("<{0}:WebPart runat=server></{0}:WebPart>"), ParseChildren(true, "Params")]
    // public class WebPart : System.Web.UI.WebControls.WebControl
    public class WebPart : System.Web.UI.Control
    {
        #region Private propery declarations
        private DataSourceTypeEnum _DataSourceType;
        private OutputRenderTypeEnum _OutputRenderType;
        private string _dataSource;
        private string _outputRenderFile;
        private XmlDocument _httpContextXml;
        private XmlDocument _webPartXml;
        private string _excludeHttpContext;
        private string _includeSessionContext;
        private string _includeUtils;
        private bool _cacheXsltPath = true;
        private string _htmlOutputCacheId;
        private string _htmlOutputRemoveIDs;

        private ArrayList parameters = new ArrayList();

        #endregion

        #region Public Property Set Methods

        public ArrayList Params
        {
            get
            {
                return parameters;
            }
        }

        // [DefaultValue("DbXml")]
        public DataSourceTypeEnum DataSourceType
        {
            set
            {
                _DataSourceType = value;
            }
        }

        [DefaultValue("XSLT")]
        public OutputRenderTypeEnum OutputRenderType
        {
            set
            {
                _OutputRenderType = value;
            }
        }

        public string DataSource
        {
            set
            {
                _dataSource = value;
            }
        }

        public string OutputRenderFile
        {
            set
            {
                _outputRenderFile = value;
            }
        }

        public string ExcludeHttpContext
        {
            set
            {
                _excludeHttpContext = value;
            }
        }

        public string IncludeSessionContext
        {
            set
            {
                _includeSessionContext = value;
            }
        }

        public string IncludeUtils
        {
            set
            {
                _includeUtils = value;
            }
        }

        public bool CacheXSLTPath
        {
            set
            {
                _cacheXsltPath = value;
            }
        }

        public string HtmlOutputCacheId
        {
            set
            {
                _htmlOutputCacheId = value;
            }
        }

        public string HtmlOutputRemoveIDs
        {
            set
            {
                _htmlOutputRemoveIDs = value;
            }
        }

        public delegate void XsltTransformEventHandler(object sender, XsltTransformEventArgs e);

        public event XsltTransformEventHandler _xsltTransformEventHandler;

        #endregion

        #region XmlLoadedEvent related section
        // A delegate type for hooking up change notifications.
        public delegate void XmlLoadedEventHandler(object sender, XmlLoadedEventArgs e);

        public event XmlLoadedEventHandler XmlLoaded;

        protected void onXmlLoaded(object sender, XmlLoadedEventArgs e)
        {
            if (XmlLoaded != null)
            {
                XmlLoaded(sender, e);
            }
        }
        #endregion XmlLoadedEvent related section

        #region ClearCache
        protected void ClearCache(string cacheID)
        {
            Cache cache = HttpContext.Current.Cache;
            string[] ids = cacheID.Split(',');
            for (int i = 0; i < ids.Length; i++)
            {
                cache.Remove(ids[i]);
            }
        }
        #endregion

        #region Render: WebControl method to spit out control output
        protected override void Render(HtmlTextWriter writer)
        {
            _httpContextXml = AppManager.BuildHttpContextXML();
            string result = string.Empty;

            if (!string.IsNullOrEmpty(_htmlOutputRemoveIDs))
            {
                ClearCache(_htmlOutputRemoveIDs);
            }

            try
            {
                Cache cache = HttpContext.Current.Cache;
                if (_OutputRenderType == OutputRenderTypeEnum.Xslt)

                    if (!string.IsNullOrEmpty(_htmlOutputCacheId))
                    {
                        if (cache[_htmlOutputCacheId] != null)
                        {
                            result = cache[_htmlOutputCacheId].ToString();
                        }
                        else
                        {
                            cache[_htmlOutputCacheId] = RenderXslt();
                            result = cache[_htmlOutputCacheId].ToString();
                        }
                    }
                    else
                    {
                        result = RenderXslt();
                    }

                else if (_OutputRenderType == OutputRenderTypeEnum.Html && _DataSourceType == DataSourceTypeEnum.None)
                    if (!string.IsNullOrEmpty(_htmlOutputCacheId))
                    {
                        if (cache[_htmlOutputCacheId] != null)
                        {
                            result = cache[_htmlOutputCacheId].ToString();
                        }
                        else
                        {
                            cache[_htmlOutputCacheId] = RenderHtml();
                            result = cache[_htmlOutputCacheId].ToString();
                        }
                    }
                    else
                    {
                        result = RenderHtml();
                    }
                else if (_OutputRenderType == OutputRenderTypeEnum.Json && _DataSourceType != DataSourceTypeEnum.None)
                    if (!string.IsNullOrEmpty(_htmlOutputCacheId))
                    {
                        if (cache[_htmlOutputCacheId] != null)
                        {
                            result = cache[_htmlOutputCacheId].ToString();
                        }
                        else
                        {
                            cache[_htmlOutputCacheId] = RenderJson();
                            result = cache[_htmlOutputCacheId].ToString();
                        }
                    }
                    else
                    {
                        result = RenderJson();
                    }
                else if (_OutputRenderType == OutputRenderTypeEnum.None && _DataSourceType == DataSourceTypeEnum.DbDataSet)
                    result = string.Empty;
                else if (_OutputRenderType == OutputRenderTypeEnum.None && _DataSourceType != DataSourceTypeEnum.None)
                    result = RenderXml();
                else
                {
                    result = "Invalid Data Source or Output Type specified.";
                }// error            

            }
            catch (Exception ex)
            {
                result = string.Format("Error occurred. Error Message:{0}", ex.Message);
            }
            finally
            {
                // output.Write(result);
                // RenderContents(writer);
                writer.Write(result);
            }
        }
        #endregion

        #region Render: Public version of Render method
        public void Render(HtmlTextWriter writer, bool overloadArg)
        {
            this.Render(writer);
        }
        #endregion

        #region AppendWebPartParams
        protected void AppendWebPartParams()
        {
            try
            {
                if (this.Params.Count > 0)
                {
                    XmlNode webpartNode = _httpContextXml.CreateElement("WebPart");
                    XmlAttribute nameAttribute = _httpContextXml.CreateAttribute("Id");
                    nameAttribute.Value = this.ID;
                    webpartNode.Attributes.Append(nameAttribute);

                    // WebPart Params
                    for (int i = 0; i < this.Params.Count; i++)
                    {
                        if (this.Params[i] is HtmlGenericControl)
                        {
                            HtmlGenericControl htmlCtl = (HtmlGenericControl)this.Params[i];
                            XmlNode KeyNode = _httpContextXml.CreateElement("Param");
                            nameAttribute = _httpContextXml.CreateAttribute("Name");
                            nameAttribute.Value = htmlCtl.TagName;
                            KeyNode.Attributes.Append(nameAttribute);
                            XmlText xmlText = _httpContextXml.CreateTextNode(htmlCtl.InnerText);
                            KeyNode.AppendChild(xmlText);
                            webpartNode.AppendChild(KeyNode);
                        }
                        else if (this.Params[i] is WebPartParam)
                        {
                            XmlNode KeyNode = _httpContextXml.CreateElement("Param");
                            nameAttribute = _httpContextXml.CreateAttribute("Name");
                            nameAttribute.Value = ((WebPartParam)this.Params[i]).Key;
                            KeyNode.Attributes.Append(nameAttribute);
                            XmlText xmlText = _httpContextXml.CreateTextNode(((WebPartParam)this.Params[i]).Value);
                            KeyNode.AppendChild(xmlText);
                            webpartNode.AppendChild(KeyNode);
                        }
                    }
                    //AppendChild(RootNode);
                    _httpContextXml.DocumentElement.AppendChild(webpartNode);
                }
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }
        #endregion

        #region RemoveWebPartParams
        protected void RemoveWebPartParams()
        {
            try
            {
                if (this.Params.Count > 0)
                {
                    XmlNodeList webpartNodes = _httpContextXml.DocumentElement.SelectNodes("/HttpContextXML/WebPart[@Id='" + this.ID + "']");

                    for (int i = 0; i < webpartNodes.Count; i++)
                    {
                        _httpContextXml.DocumentElement.RemoveChild(webpartNodes[i]);
                    }
                }
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }
        #endregion

        #region RenderXslt: Method to combine Xml data with XSLT style sheet to generate WebPart output
        private string RenderXslt()
        {
            try
            {
                XmlDocument webPartXmlDoc = new XmlDocument();
                XPathNavigator webPartXmlNavigator = null;

                if (_webPartXml == null)
                {
                    if (_DataSourceType == DataSourceTypeEnum.None)
                    {
                        webPartXmlDoc.LoadXml(@"<Root/>");
                    }
                    else
                    {
                        webPartXmlDoc = this.GetWebPartXml();
                    }
                }
                else
                {
                    webPartXmlDoc = _webPartXml;
                }

                webPartXmlNavigator = webPartXmlDoc.CreateNavigator();

                string xsltFilePath;

                if (this.Page != null)
                {
                    xsltFilePath = this.Page.Server.MapPath(_outputRenderFile);
                }
                else
                {
                    xsltFilePath = HttpContext.Current.Server.MapPath(_outputRenderFile);
                }

                if (_includeUtils == "1")
                {
                    XsltArgumentList xslArgs = new XsltArgumentList();
                    xslArgs.AddExtensionObject("urn:webpartutil", new WebPartUtil());
                    xslArgs.AddExtensionObject("urn:xsltutil", new XSLTUtil());
                    xslArgs.AddExtensionObject("urn:webpart", this);

                    if (_xsltTransformEventHandler != null)
                    {
                        XsltTransformEventArgs e = new XsltTransformEventArgs(xslArgs);
                        _xsltTransformEventHandler(this, e);
                    }

                    return XSLTUtil.Transform(webPartXmlNavigator, xsltFilePath, _cacheXsltPath, xslArgs);
                }
                else
                {
                    if (_xsltTransformEventHandler != null)
                    {
                        XsltTransformEventArgs e = new XsltTransformEventArgs(null);
                        _xsltTransformEventHandler(this, e);
                    }

                    return XSLTUtil.Transform(webPartXmlNavigator, xsltFilePath, _cacheXsltPath);
                }
            }
            catch (Exception ex)
            {
                
                throw;
            }
        }
        #endregion

        #region RenderHtml: reads from local HTML file spits that out
        private string RenderHtml()
        {
            string filePath = this.Page.Server.MapPath(_outputRenderFile);
            try
            {
                return System.IO.File.ReadAllText(filePath);
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }
        #endregion

        #region RenderJson: Gets WebPartXml, converts to Json and returns Json string 
        private string RenderJson()
        {
            try
            {
                XmlDocument webPartXmlDoc = new XmlDocument();

                if (_webPartXml == null)
                {
                    webPartXmlDoc = this.GetWebPartXml();
                }
                else
                {
                    webPartXmlDoc = _webPartXml;
                }

                return GMS.Utils.XmlUtil.XmlToJson(webPartXmlDoc);
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }
        #endregion

        #region RenderXml: Gets WebPartXml and spits out
        private string RenderXml()
        {
            try
            {
                XmlDocument webPartXmlDoc = new XmlDocument();

                if (_webPartXml == null)
                {
                    webPartXmlDoc = this.GetWebPartXml();
                }
                else
                {
                    webPartXmlDoc = _webPartXml;
                }

                return webPartXmlDoc.OuterXml;
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }
        #endregion

        #region GetWebPartXml: calls different methods based on Xml source type and returns XmlDocument
        private XmlDocument GetWebPartXml()
        {
            if (_DataSourceType == DataSourceTypeEnum.None)
            {
                throw new Exception("Invalid Xml Data Source Type");
            }

            XmlDocument resultXmlDoc = null;

            try
            {
                switch (_DataSourceType)
                {
                    case DataSourceTypeEnum.DbXml:
                        resultXmlDoc = GetWebPartXmlFromDB();
                        break;

                    case DataSourceTypeEnum.HttpXml:
                        resultXmlDoc = GetWebPartXmlFromRemote();
                        break;

                    case DataSourceTypeEnum.LocalXml:
                        resultXmlDoc = GetWebPartXmlFromLocal();
                        break;

                    case DataSourceTypeEnum.StringXml:
                        resultXmlDoc = GetStringXml();
                        break;

                    case DataSourceTypeEnum.ContextXml:
                        resultXmlDoc = _httpContextXml;
                        break;

                }
                onXmlLoaded(this, new XmlLoadedEventArgs(resultXmlDoc));
                _webPartXml = resultXmlDoc;
                return resultXmlDoc;
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }
        #endregion

        #region GetWebPartXmlFromDB - returns XmlDocument
        private XmlDocument GetWebPartXmlFromDB()
        {
            try
            {
                // below function appends web part params to the HttpContext if any such exists
                AppendWebPartParams();

                string anonGuid = HttpContext.Current.Request.AnonymousID;
                string authGuid = HttpContext.Current.Items["AuthGuid"] != null ? HttpContext.Current.Items["AuthGuid"].ToString() : null;
                string role = HttpContext.Current.Items["Role"] != null ? HttpContext.Current.Items["Role"].ToString() : null;

                XmlDocument webPartXmlDoc = new XmlDocument();
                string xmlText = DataManager.GetWebPartXml(_dataSource, _httpContextXml, authGuid, anonGuid, role);
                if (xmlText.Equals(string.Empty))
                    xmlText = @"<Root />";

                webPartXmlDoc.LoadXml(xmlText);
                if (_OutputRenderType != OutputRenderTypeEnum.Json && _excludeHttpContext != "1")
                {
                    XmlNode importedHttpContextNode = webPartXmlDoc.ImportNode(_httpContextXml.DocumentElement, true);
                    webPartXmlDoc.DocumentElement.AppendChild(importedHttpContextNode);
                }

                if (_includeSessionContext == "1" && HttpContext.Current.Session["ContextXML"] is XmlDocument)
                {
                    XmlDocument sessionContextXml = (XmlDocument)HttpContext.Current.Session["ContextXML"];
                    XmlNode importedSessionContextNode = webPartXmlDoc.ImportNode(sessionContextXml.DocumentElement, true);
                    webPartXmlDoc.DocumentElement.AppendChild(importedSessionContextNode);
                }

                // below function removes web part params to the HttpContext if any such exists
                RemoveWebPartParams();

                return webPartXmlDoc;
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }
        #endregion GetWebPartXmlFromDB

        #region GetWebPartDataSet - gets DataSet as SP call result
        public DataSet GetWebPartDataSet()
        {
            DataSet dataSet = null;
            _httpContextXml = AppManager.BuildHttpContextXML();

            // for consistency DataSourceType should be set as DbDataSet
            if (_dataSource.Length == 0)
                throw new Exception("Error in GetWebPartDataSet: Data Source is not specified");

            try
            {
                // below function appends web part params to the HttpContext if any such exists
                AppendWebPartParams();

                string anonGuid = HttpContext.Current.Request.AnonymousID;
                string authGuid = HttpContext.Current.Items["AuthGuid"].ToString();
                string role = HttpContext.Current.Items["Role"] != null ? HttpContext.Current.Items["Role"].ToString() : null;

                // XmlDocument webPartXmlDoc = new XmlDocument();
                dataSet = DataManager.GetWebPartDataSet(_dataSource, _httpContextXml, authGuid, anonGuid, role);

                // below function removes web part params to the HttpContext if any such exists
                RemoveWebPartParams();

                return dataSet;
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }
        #endregion GetWebPartXmlFromDB

        #region GetWebPartXmlFromRemote - returns XmlDocument
        private XmlDocument GetWebPartXmlFromRemote()
        {
            string result = string.Empty;
            XmlDocument webPartXmlDoc = new XmlDocument();

            try
            {
                HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(_dataSource);

                HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();
                using (StreamReader sr = new StreamReader(objResponse.GetResponseStream()))
                {
                    result = sr.ReadToEnd();

                    // Close and clean up the StreamReader
                    sr.Close();
                }
                webPartXmlDoc.LoadXml(result);

                if (_OutputRenderType != OutputRenderTypeEnum.Json && _excludeHttpContext != "1")
                {
                    XmlNode importedHttpContextNode = webPartXmlDoc.ImportNode(_httpContextXml.DocumentElement, true);
                    webPartXmlDoc.DocumentElement.AppendChild(importedHttpContextNode);
                }

                return webPartXmlDoc;
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }
        #endregion GetWebPartXmlFromRemote

        #region GetWebPartXmlFromLocal - returns XmlDocument
        private XmlDocument GetWebPartXmlFromLocal()
        {
            string localXmlData = string.Empty;

            try
            {
                localXmlData = System.IO.File.ReadAllText(this.Page.Server.MapPath(_dataSource));
            }
            catch (Exception ex)
            {
                
                throw ex;
            }

            try
            {

                XmlDocument webPartXmlDoc = new XmlDocument();
                webPartXmlDoc.LoadXml(localXmlData);

                if (_OutputRenderType != OutputRenderTypeEnum.Json && _excludeHttpContext != "1")
                {
                    XmlNode importedHttpContextNode = webPartXmlDoc.ImportNode(_httpContextXml.DocumentElement, true);
                    webPartXmlDoc.DocumentElement.AppendChild(importedHttpContextNode);
                }

                return webPartXmlDoc;
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }
        #endregion GetWebPartXmlFromLocal

        #region GetStringXml - returns XmlDocument
        private XmlDocument GetStringXml()
        {
            string xmlData = string.Empty;

            try
            {
                xmlData = _dataSource;
            }
            catch (Exception ex)
            {
                
                throw ex;
            }

            try
            {

                XmlDocument webPartXmlDoc = new XmlDocument();
                webPartXmlDoc.LoadXml(xmlData);

                if (_OutputRenderType != OutputRenderTypeEnum.Json && _excludeHttpContext != "1")
                {
                    XmlNode importedHttpContextNode = webPartXmlDoc.ImportNode(_httpContextXml.DocumentElement, true);
                    webPartXmlDoc.DocumentElement.AppendChild(importedHttpContextNode);
                }

                return webPartXmlDoc;
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }
        #endregion GetStringXml

        #region RenderWithDifferentXslt
        public string RenderWithDifferentXslt(string outputRenderFile, string includeUtils)
        {
            _outputRenderFile = outputRenderFile;
            _includeUtils = includeUtils;
            StringWriter stringWriter = new StringWriter();
            HtmlTextWriter htmlWriter = new HtmlTextWriter(stringWriter);
            this.Render(htmlWriter);
            return stringWriter.ToString();
        }
        #endregion
    }
    #endregion WebPart class

}
