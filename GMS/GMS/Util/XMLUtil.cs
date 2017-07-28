using System;
using System.Xml;
using System.Xml.Xsl;
using System.IO;
using System.Text;
using System.Collections;
using GMS.Utils;

namespace GMS.Utils
{
    public class XmlUtil
    {
        private XmlUtil()
        {
        }

        public static string GetXmlNodeInnerText(XmlDocument xmlDocument, string xPath)
        {
            string retval = null;
            XmlNode xmlRoot = xmlDocument.DocumentElement;
            XmlNode xmlNode = xmlRoot.SelectSingleNode(xPath);
            if (xmlNode != null) retval = xmlNode.InnerXml;
            return retval;
        }

        public static XmlNodeList GetXmlNodes(string xmlContext, string xPath)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xmlContext);
            XmlNode root = xmlDocument.DocumentElement;
            XmlNodeList webPartNodes = xmlDocument.SelectNodes(xPath);
            return webPartNodes;
        }

        public static string GetXmlNodeAttribEdit(string xmlContext, string xPath, string key, string val)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xmlContext);
            XmlNode root = xmlDocument.DocumentElement;
            XmlNodeList nodes = root.SelectNodes(xPath);
            foreach (XmlNode node in nodes)
            {
                node.Attributes[key].Value = val;
            }
            return xmlDocument.OuterXml;
        }

        public static string XmlToJson(XmlDocument xmlDoc)
        {
            StringBuilder sbJson = new StringBuilder();

            if (xmlDoc.SelectNodes(@"//*|//@*").Count == 1)
            {
                sbJson.Append("{ ");
                sbJson.Append("\"" + xmlDoc.DocumentElement.Name + "\": \"" + xmlDoc.DocumentElement.InnerText + "\" ");
                sbJson.Append("}");
            }
            else
            {
                if (xmlDoc.DocumentElement.HasAttribute("Json_NoOutput"))
                {
                    XmlToJsonnode(sbJson, xmlDoc.DocumentElement, false);
                }
                else
                {
                    sbJson.Append("{ ");
                    XmlToJsonnode(sbJson, xmlDoc.DocumentElement, true);
                    sbJson.Append("}");
                }
            }

            return sbJson.ToString();
        }

        //  XmlToJsonnode:  Output an XmlElement, possibly as part of a higher array
        private static void XmlToJsonnode(StringBuilder sbJson, XmlElement node, bool showNodeName)
        {
            bool childAdded = false;

            if (showNodeName)
                sbJson.Append("\"" + SafeJson(node.Name) + "\": ");

            sbJson.Append("{");
            // Build a sorted list of key-value pairs
            // where key is case-sensitive nodeName
            // value is an ArrayList of string or XmlElement
            // so that we know whether the nodeName is an array or not.
            SortedList childNodeNames = new SortedList();

            //  Add in all node attributes
            if (node.Attributes != null)
                foreach (XmlAttribute attr in node.Attributes)
                    StoreChildNode(childNodeNames, attr.Name, attr.InnerText);

            //  Add in all nodes
            foreach (XmlNode cnode in node.ChildNodes)
            {
                if (cnode is XmlText)
                    StoreChildNode(childNodeNames, "value", cnode.InnerText);
                else if (cnode is XmlElement)
                    StoreChildNode(childNodeNames, cnode.Name, cnode);
            }

            // Now output all stored info
            foreach (string childname in childNodeNames.Keys)
            {
                ArrayList alChild = (ArrayList)childNodeNames[childname];
                if (alChild.Count == 1 && !childname.Contains("AsArray") && !childname.Equals("item"))
                    OutputNode(childname, alChild[0], sbJson, true);
                else
                {
                    sbJson.Append(" \"" + SafeJson(childname) + "\": [ ");
                    foreach (object Child in alChild)
                        OutputNode(childname, Child, sbJson, false);
                    sbJson.Remove(sbJson.Length - 2, 2);
                    sbJson.Append(" ], ");
                }
            }
            sbJson.Remove(sbJson.Length - 2, 2);
            sbJson.Append(" }");

        }

        //  StoreChildNode: Store data associated with each nodeName
        //                  so that we know whether the nodeName is an array or not.
        private static void StoreChildNode(SortedList childNodeNames, string nodeName, object nodeValue)
        {
            // Pre-process contraction of XmlElement-s
            if (nodeValue is XmlElement)
            {
                // Convert  <aa></aa> into "aa":null
                //          <aa>xx</aa> into "aa":"xx"
                XmlNode cnode = (XmlNode)nodeValue;
                if (cnode.Attributes.Count == 0)
                {
                    XmlNodeList children = cnode.ChildNodes;
                    if (children.Count == 0)
                        nodeValue = null;
                    else if (children.Count == 1 && (children[0] is XmlText))
                        nodeValue = ((XmlText)(children[0])).InnerText;
                }
            }
            // Add nodeValue to ArrayList associated with each nodeName
            // If nodeName doesn't exist then add it
            object oValuesAL = childNodeNames[nodeName];
            ArrayList ValuesAL;
            if (oValuesAL == null)
            {
                ValuesAL = new ArrayList();
                childNodeNames[nodeName] = ValuesAL;
            }
            else
                ValuesAL = (ArrayList)oValuesAL;
            ValuesAL.Add(nodeValue);
        }

        private static void OutputNode(string childname, object alChild, StringBuilder sbJson, bool showNodeName)
        {
            if (childname.Equals("Json_NoOutput"))
                return;

            if (alChild == null)
            {
                if (showNodeName)
                    sbJson.Append("\"" + SafeJson(childname) + "\": ");
                sbJson.Append("null");
            }
            else if (alChild is string)
            {
                if (showNodeName)
                    sbJson.Append("\"" + SafeJson(childname) + "\": ");
                string sChild = (string)alChild;
                sChild = sChild.Trim();
                sbJson.Append("\"" + SafeJson(sChild) + "\"");
            }
            else
                XmlToJsonnode(sbJson, (XmlElement)alChild, showNodeName);

            sbJson.Append(", ");
        }

        // Make a string safe for Json
        private static string SafeJson(string sIn)
        {
            StringBuilder sbOut = new StringBuilder(sIn.Length);
            foreach (char ch in sIn)
            {
                if (Char.IsControl(ch) || ch == '\'')
                {
                    int ich = (int)ch;
                    sbOut.Append(@"\u" + ich.ToString("x4"));
                    continue;
                }
                else if (ch == '\"' || ch == '\\' || ch == '/')
                {
                    sbOut.Append('\\');
                }
                sbOut.Append(ch);
            }
            return sbOut.ToString();
        }
    }
}