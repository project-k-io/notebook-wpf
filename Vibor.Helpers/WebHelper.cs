// Decompiled with JetBrains decompiler
// Type: Vibor.Helpers.WebHelper
// Assembly: Vibor.Helpers, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: E29329B7-F05A-4CC7-B834-7BAFB4348D90
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Vibor.Helpers.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Vibor.Logging;

namespace Vibor.Helpers
{
    public class WebHelper
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