using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.IO;

using System.Web.UI.HtmlControls;
using GMS.Control;
using GMS.Utils;

namespace GMS.Utils
{
    public class WebPartParam
    {
        public string Key;
        public string Value;
    }

    public class WebPartUtil
    {
        public WebPartUtil()
        {

        }

        public static string Render(string dataSourceType, string dataSource, string outputRenderType, string outputRenderFile, string extParameters, string includeUtils, string htmlCacheID)
        {
            WebPart webPart = new WebPart();
            webPart.DataSource = dataSource;
            webPart.OutputRenderFile = outputRenderFile;
            webPart.HtmlOutputCacheId = htmlCacheID;
            switch (dataSourceType)
            {
                case "None":
                    webPart.DataSourceType = DataSourceTypeEnum.None;
                    break;
                case "DbXml":
                    webPart.DataSourceType = DataSourceTypeEnum.DbXml;
                    break;
                case "LocalXml":
                    webPart.DataSourceType = DataSourceTypeEnum.LocalXml;
                    break;
                case "HttpXml":
                    webPart.DataSourceType = DataSourceTypeEnum.HttpXml;
                    break;
                case "ContextXml":
                    webPart.DataSourceType = DataSourceTypeEnum.ContextXml;
                    break;
                case "StringXml":
                    webPart.DataSourceType = DataSourceTypeEnum.StringXml;
                    break;
                case "DbDataSet":
                    webPart.DataSourceType = DataSourceTypeEnum.DbDataSet;
                    break;
            }

            switch (outputRenderType)
            {
                case "None":
                    webPart.OutputRenderType = OutputRenderTypeEnum.None;
                    break;
                case "Html":
                    webPart.OutputRenderType = OutputRenderTypeEnum.Html;
                    break;
                case "Json":
                    webPart.OutputRenderType = OutputRenderTypeEnum.Json;
                    break;
                case "Xslt":
                    webPart.OutputRenderType = OutputRenderTypeEnum.Xslt;
                    break;
            }

            if (!String.IsNullOrEmpty(extParameters))
            {

                string[] allParams = extParameters.Split(',');

                foreach (string param in allParams)
                {
                    string[] paramCollection = param.Split(':');
                    HtmlGenericControl htmlGenCtrl = new HtmlGenericControl();
                    htmlGenCtrl.TagName = paramCollection[0];
                    htmlGenCtrl.InnerText = paramCollection[1];
                    webPart.Params.Add(htmlGenCtrl);
                }
            }

            webPart.IncludeUtils = includeUtils;

            StringWriter stringWriter = new StringWriter();
            HtmlTextWriter htmlWriter = new HtmlTextWriter(stringWriter);
            webPart.Render(htmlWriter, true);
            return stringWriter.ToString();
        }

        #region Render: Static method for rendering Sword Webpart externally
        public static string Render(string dataSourceType, string dataSource, string outputRenderType, string outputRenderFile, string extParameters, string includeUtils)
        {
            WebPart webPart = new WebPart();
            webPart.DataSource = dataSource;
            webPart.OutputRenderFile = outputRenderFile;

            switch (dataSourceType)
            {
                case "None":
                    webPart.DataSourceType = DataSourceTypeEnum.None;
                    break;
                case "DbXml":
                    webPart.DataSourceType = DataSourceTypeEnum.DbXml;
                    break;
                case "LocalXml":
                    webPart.DataSourceType = DataSourceTypeEnum.LocalXml;
                    break;
                case "HttpXml":
                    webPart.DataSourceType = DataSourceTypeEnum.HttpXml;
                    break;
                case "ContextXml":
                    webPart.DataSourceType = DataSourceTypeEnum.ContextXml;
                    break;
                case "StringXml":
                    webPart.DataSourceType = DataSourceTypeEnum.StringXml;
                    break;
                case "DbDataSet":
                    webPart.DataSourceType = DataSourceTypeEnum.DbDataSet;
                    break;
            }

            switch (outputRenderType)
            {
                case "None":
                    webPart.OutputRenderType = OutputRenderTypeEnum.None;
                    break;
                case "Html":
                    webPart.OutputRenderType = OutputRenderTypeEnum.Html;
                    break;
                case "Json":
                    webPart.OutputRenderType = OutputRenderTypeEnum.Json;
                    break;
                case "Xslt":
                    webPart.OutputRenderType = OutputRenderTypeEnum.Xslt;
                    break;
            }

            if (!String.IsNullOrEmpty(extParameters))
            {

                string[] allParams = extParameters.Split(',');

                foreach (string param in allParams)
                {
                    string[] paramCollection = param.Split(':');
                    HtmlGenericControl htmlGenCtrl = new HtmlGenericControl();
                    htmlGenCtrl.TagName = paramCollection[0];
                    htmlGenCtrl.InnerText = paramCollection[1];
                    webPart.Params.Add(htmlGenCtrl);
                }
            }

            webPart.IncludeUtils = includeUtils;

            StringWriter stringWriter = new StringWriter();
            HtmlTextWriter htmlWriter = new HtmlTextWriter(stringWriter);
            webPart.Render(htmlWriter, true);
            return stringWriter.ToString();
        }
        #endregion

        #region Render: Static method for rendering Sword Webpart externally
        public static string Render(string dataSource, string outputRenderFile, string extParameters, string includeUtils)
        {
            return Render("DbXml", dataSource, "Xslt", outputRenderFile, extParameters, includeUtils);
        }
        #endregion

        #region Render: Static method for rendering Sword Webpart externally
        public static string Render(string dataSource, string outputRenderFile, string extParameters)
        {
            return Render("DbXml", dataSource, "Xslt", outputRenderFile, extParameters, "0");
        }
        #endregion
    }
}