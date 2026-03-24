using AutoCount.Data;
using Microsoft.Owin.Hosting;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace ostautocountws
{
    class Program
    {
        static void Main(string[] args)
        {

            try
            {
                //  updateConfig();


                AutoCount.MainEntry.UIStartup startup = new AutoCount.MainEntry.UIStartup();
                AutoCount.Data.DBSetting dbSetting = new AutoCount.Data.DBSetting(
                                                      DBServerType.SQL2000, ConfigurationManager.AppSettings["SQLServer"], ConfigurationManager.AppSettings["SQLDBUID"], ConfigurationManager.AppSettings["SQLDBPWD"], ConfigurationManager.AppSettings["SQLDBName"]);
                AutoCount.Authentication.UserSession userSession = new
                                                     AutoCount.Authentication.UserSession(dbSetting);
                if (userSession.Login(SettingsMain.DefaultAutoCountID, SettingsMain.DefaultAutoCountPWD))
                {
                    //2nd parameter is to load plug-in when value is true.
                    //set 2nd parameter to false if do not want to load plug-in.
                    startup.SubProjectStartup(userSession);
                }

                Console.WriteLine($"Is Login:{userSession.IsLogin}");
                SettingsMain.MyDbSession = userSession;


            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception:{ex.Message}");
                Console.ReadLine();
                //    throw ex;
            }
            //string baseAddress = SettingsMain.BaseUrl ;

            //// Start OWIN host 
            //using (WebApp.Start<Startup>(url: baseAddress))
            //{
            //    // Create HttpCient and make a request to api/values 
            //    //HttpClient client = new HttpClient();

            //    //var response = client.GetAsync(baseAddress + "api/values").Result;

            //    //Console.WriteLine(response);
            //    //Console.WriteLine(response.Content.ReadAsStringAsync().Result);
            //    Console.WriteLine("Service Starting");
            //    Console.WriteLine("Service Started");
            //    AutoCount.MainEntry.Startup.Default.SubProjectStartup(SettingsMain.MyDbSession, SettingsMain.MydbSetting);
            //    Console.ReadKey();
            //    AutoCount.MainEntry.Startup.Default.Shutdown();
            //}

            HostFactory.Run(host =>
            {
              
                switch (SettingsMain.AccLink)
                {
                    //case "SQLACC":
                    //    {
                    //        host.SetServiceName("OstSqlAccount"); //cannot contain spaces or / or \
                    //        host.SetDisplayName("OstSqlAccount");
                    //        host.SetDescription("OstSqlAccount");
                    //        host.StartAutomatically();
                    //        host.Service<OstSqlAccountService>();
                    //        break;
                    //    }
                    case "AUTOCOUNT":
                        {
                            host.SetServiceName("OstAutoCount"); //cannot contain spaces or / or \
                            host.SetDisplayName("OstAutoCount");
                            host.SetDescription("OstAutoCount");
                            host.StartAutomatically();
                            host.Service<OstAutoCountService>();
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
              
            });


        }

        private static void updateConfig()
        {
           
            const string userRoot = "HKEY_LOCAL_MACHINE";
            const string subkey = @"Software\Accstream\ostAutoCount\Settings";
            const string keyName = userRoot + "\\" + subkey;
            Console.WriteLine(Registry.GetValue(keyName, "wsUrl", "IDontKnow"));
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["SQLServer"].Value = Registry.GetValue(keyName, "MSSQLServer", "").ToString();
            config.AppSettings.Settings["SQLDBName"].Value = Registry.GetValue(keyName, "DBName", "").ToString();
            config.AppSettings.Settings["BaseUrl"].Value = Registry.GetValue(keyName, "wsUrl", "").ToString();
            config.AppSettings.Settings["DefaultAutoCountID"].Value = Registry.GetValue(keyName, "AutoCountID", "").ToString();
            config.AppSettings.Settings["DefaultAutoCountPWD"].Value = Registry.GetValue(keyName, "AutoCountPWD", "").ToString();
         
            config.Save(ConfigurationSaveMode.Modified);
            Registry.SetValue(keyName, "appConfigDone", "yes");
        }
    }
}
