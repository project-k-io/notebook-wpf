// Decompiled with JetBrains decompiler
// Type: Vibor.Helpers.WebHelper
// Assembly: Vibor.Helpers, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: E29329B7-F05A-4CC7-B834-7BAFB4348D90
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Vibor.Helpers.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Vibor.Helpers
{
  public class WebHelper
  {
    private static readonly ILog Log = XLogger.GetLogger();

    public static string[] GetLines(string urlString)
    {
      List<string> stringList = new List<string>();
      try
      {
        WebRequest webRequest = WebRequest.Create(new Uri(urlString));
        webRequest.Credentials = (ICredentials) new NetworkCredential("adzond", "QazWsx12");
        WebResponse response = webRequest.GetResponse();
        StreamReader streamReader = new StreamReader(response.GetResponseStream());
        string str;
        while ((str = streamReader.ReadLine()) != null)
          stringList.Add(str);
        response.Close();
        streamReader.Close();
      }
      catch (Exception ex)
      {
        WebHelper.Log.Error(ex.Message + " : " + urlString);
      }
      return stringList.ToArray();
    }
  }
}
