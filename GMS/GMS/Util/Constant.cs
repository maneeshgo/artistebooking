using System;
using System.Configuration;
using System.Collections.Generic;

namespace GMS.Utils
{
    public static class Constants
    {
        public static string LogPath;
        public static string AppCode;
        public static string DBCode;
        public static string SiteCode;
        public static string ERR_MESSAGE = "System Error Occurred.";

        public static bool IsHttpContextXMLLoggingEnabled;
        public static bool IsDebugMode = false;

        public static string UploadPath;
        public static string RelativeUploadPath;
        public static bool ImageResizing;


        public static Guid GuidNull = new Guid("00000000-0000-0000-0000-000000000000");
        public static string ConnectionString;
        private static string _conn = string.Empty;

        public static bool IsGuid(string guidString)
        {
            try
            {
                Guid g = new Guid(guidString);
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
