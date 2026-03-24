using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace ostautocountws
{
    public class OstAutoCountService : ServiceControl
    {
        bool ServiceControl.Start(HostControl hostControl)
        {
            try
            {
                SettingsMain.MLogger.Info("Starting Service");
                string baseAddress = SettingsMain.BaseUrl;
                WebApp.Start<Startup>(url: baseAddress);
                SettingsMain.MLogger.Info("Started Service");
                SettingsMain.MLogger.Info("Starting AutoCount Main Entry");
            //  AutoCount.MainEntry.Startup.Default.SubProjectStartup(SettingsMain.MyDbSession);
                SettingsMain.MLogger.Info("Started AutoCount Main Entry");
                Console.WriteLine("V2.0-B43-R215-16");
                return true;
            }
            catch (Exception ex)
            {

                SettingsMain.MLogger.Error(ex.Message);
                SettingsMain.MLogger.Error(ex.StackTrace);
                return false;
            }
         
            
           
        }

        bool ServiceControl.Stop(HostControl hostControl)
        {
            try
            {
                SettingsMain.MLogger.Info("Stopping Service");
                //AutoCount.MainEntry.Startup.Default.Shutdown();
                AutoCount.MainEntry.MainStartup.Default.Shutdown();
                SettingsMain.MLogger.Info("Service Stopeed");
                return true;
            }
            catch (Exception ex)
            {
                SettingsMain.MLogger.Error(ex.Message);
                SettingsMain.MLogger.Error(ex.StackTrace);
                return false;
            }
           
            
          
        }
    }
}
