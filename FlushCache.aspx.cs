using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Caching;
using System.Collections;
using System.IO;
using System.Diagnostics;

public partial class FlushCache : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        IDictionaryEnumerator enumerator = Cache.GetEnumerator();

        while (enumerator.MoveNext())
        {
            if (enumerator.Key.ToString().IndexOf(".xslt") != -1 || enumerator.Key.ToString().IndexOf("_translations") != -1)
            {
                Cache.Remove(enumerator.Key.ToString());
            }
        }

        /*
        Process p = new Process();
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.RedirectStandardOutput = true;
        p.StartInfo.FileName = String.Format(@"{0}\get_version.bat", Server.MapPath("~"));
        p.Start();
        string output = p.StandardOutput.ReadToEnd();
        
        Response.Write(output);*/
    }
}