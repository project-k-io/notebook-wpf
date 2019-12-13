using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Vibor.Logging;

namespace Vibor.Helpers
{
    internal class WebHelper
    {
        private static readonly ILog Log = LogManager.GetLogger("WebHelper");

        public static string[] GetLines(string urlString)
        {
            var stringList = new List<string>();
            try
            {
                var webRequest = WebRequest.Create(new Uri(urlString));
                webRequest.Credentials = new NetworkCredential("adzond", "QazWsx12");
                var response = webRequest.GetResponse();
                var streamReader = new StreamReader(response.GetResponseStream());
                string str;
                while ((str = streamReader.ReadLine()) != null)
                    stringList.Add(str);
                response.Close();
                streamReader.Close();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message + " : " + urlString);
            }

            return stringList.ToArray();
        }
    }
}