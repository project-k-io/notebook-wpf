using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ProjectK.Logging;
using ProjectK.Notebook.ViewModels;

namespace ProjectK.Notebook
{
    public class ConfigHelper
    {
        private static readonly ILogger Logger = LogManager.GetLogger<ConfigHelper>();

        static string ReadSetting(string key)
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                var result = appSettings[key];
                Logger.LogDebug(result);
                return result;
            }
            catch (ConfigurationErrorsException)
            {
                Logger.LogError("Error reading app settings");
                return "";
            }
        }
    }
}
