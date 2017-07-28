using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Xsl;
using GMS.Control;

/// <summary>
/// Summary description for WebPart
/// </summary>
/// 
namespace Artist.Control
{
    public class WebPart : GMS.Control.WebPart
    {
        public WebPart()
            : base()
        {
            this._xsltTransformEventHandler += new XsltTransformEventHandler(addAsansorUtilInstanceToXsltArgs);
        }

        public void addAsansorUtilInstanceToXsltArgs(object sender, XsltTransformEventArgs e)
        {
            XsltArgumentList xslArgs = (XsltArgumentList)e.XslArgs;

            if (xslArgs != null)
            {
                //xslArgs.AddExtensionObject("urn:asansorutil", new Asansor.Util());
            }
        }
    }
}