using System;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.IO;
using GMS.Utils;
using System.Text;
using System.Web;
using System.Web.Caching;

namespace GMS.Utils
{
    #region class XSLTUtil: handles XSLT transformation
    public class XSLTUtil
    {
        #region RenderXslt: Method to combine Xml data with XSLT style sheet 
        public static string RenderXslt(string xsltRelativePath, MemoryStream xmlText, bool useXsltCaching)
        {
            try
            {
                XPathDocument xPathDoc = new XPathDocument(xmlText);

                XPathNavigator xmlNavigator = xPathDoc.CreateNavigator();

                string xsltFilePath = HttpContext.Current.Server.MapPath(xsltRelativePath);

                return Transform(xmlNavigator, xsltFilePath, useXsltCaching);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        #region Transform: returns string - gets XPathNavigator xml and string path for xsl file
        public static string Transform(XPathNavigator xml, string xsltPath, bool cacheXSLTPath)
        {
            // XsltSettings xslt_settings = new XsltSettings();
            // xslt_settings.EnableScript = true;
            // XslCompiledTransform xslt = new XslCompiledTransform();
            // XPathDocument webPartXSL = new XPathDocument(xslPath);



            // XPathNavigator xslNavigator = webPartXSL.CreateNavigator();
            XslCompiledTransform xsltTransformer = GetXsltTransformer(xsltPath, cacheXSLTPath);

            try
            {
                StringBuilder myStringBuilder = new StringBuilder();
                StringWriter myStringWriter = new StringWriter(myStringBuilder);

                xsltTransformer.Transform(xml, null, myStringWriter);

                return myStringBuilder.ToString();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion Transform

        #region Transform: returns string - gets XPathNavigator xml, string path for xsl file and xslt args
        public static string Transform(XPathNavigator xml, string xsltPath, bool cacheXSLTPath, XsltArgumentList args)
        {
            XslCompiledTransform xsltTransformer = GetXsltTransformer(xsltPath, cacheXSLTPath);

            try
            {
                StringBuilder myStringBuilder = new StringBuilder();
                StringWriter myStringWriter = new StringWriter(myStringBuilder);

                xsltTransformer.Transform(xml, args, myStringWriter);

                return myStringBuilder.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion Transform

        private static XslCompiledTransform GetXsltTransformer(string xsltPath, bool cacheXSLTPath)
        {
            Cache cache = HttpContext.Current.Cache;

            if (cache[xsltPath] != null)
            {
                return (XslCompiledTransform)cache[xsltPath];
            }

            if (!File.Exists(xsltPath))
            {
                throw new Exception(string.Format(@"Cannot find xslt file in file system. Please check the file path:({0})", xsltPath));
            }

            /*
            string xslContent = string.Empty;            
            
            try
            {
                xslContent = System.IO.File.ReadAllText(xsltPath);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            XmlDocument xsltDocument = new XmlDocument();
            xsltDocument.LoadXml(xslContent);
            XPathNavigator xsltNavigator = xsltDocument.CreateNavigator();
            */

            XsltSettings xslt_settings = new XsltSettings();
            xslt_settings.EnableScript = true;
            xslt_settings.EnableDocumentFunction = true;
            XslCompiledTransform xsltTransformer = new XslCompiledTransform();

            xsltTransformer.Load(xsltPath, xslt_settings, new XmlUrlResolver());
            // xsltTransformer.Load(xsltNavigator, xslt_settings, new XmlUrlResolver());

            if (cacheXSLTPath)
            { // if xslt cache true, cache xslt path
                cache[xsltPath] = xsltTransformer;
            }

            return xsltTransformer;
        }

        public static XPathNodeIterator Parse(string data)
        {

            if (data == null || data.Length == 0)
            {
                data = "&lt;Empty /&gt;";
            }

            try
            {
                StringReader stringReader = new StringReader("<Root>" + data + "</Root>");
                XPathDocument xPathDocument = new XPathDocument(stringReader);
                XPathNavigator xPathNavigator = xPathDocument.CreateNavigator();
                XPathExpression xPathExpression = xPathNavigator.Compile("/");

                XPathNodeIterator xPathNodeIterator = xPathNavigator.Select(xPathExpression);

                return xPathNodeIterator;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
    #endregion class XSLTUtil
}
