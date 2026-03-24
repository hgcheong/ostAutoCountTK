using AutoCount.Authentication;
using AutoCount.Data;
using FirebirdSql.Data.FirebirdClient;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ostautocountws
{
   public static class SettingsMain
    {
        private static DBSetting _mydbSetting;
        private static UserSession _mydbSession;
        private static ILog mLogger;
        private static string _baseUrl;
        private static string _DefaultAutoCountID;
        private static string _DefaultAutoCountPWD;
        private static string _AccLink;
        private static FbConnection _ostendoCnn;
        //For SQLacc
        private static dynamic _ComServer;
        private static FbConnection _sqlAccCnn;
       
       static  Int32 lBuildNo;
        static Type lBizType;

        //From Sql Accounting
        public static void KillApp()
        {
            try
            {
                foreach (Process prc in Process.GetProcessesByName("SQLAcc"))
                {
                    prc.Kill(); //Make sure no other SQLAcc is running
                }
            }
            catch (Exception ex)
            {
                throw new Exception("KillApp failed" + ex.Message);
            }
        }
 
        public static void FreeBiz(object AbizObj)
        {
            System.Runtime.InteropServices.Marshal.ReleaseComObject(AbizObj);
        }

        public static FbConnection OstendoCnn
        {
            get
            {
                if (_ostendoCnn == null)
                {
                    FbConnectionStringBuilder fbstr = new FbConnectionStringBuilder();
                    fbstr.UserID = ConfigurationManager.AppSettings["OstendoUID"];
                    fbstr.Password = ConfigurationManager.AppSettings["OstendoPWD"];
                    fbstr.Database = ConfigurationManager.AppSettings["OstendoDbPath"];
                    fbstr.DataSource = ConfigurationManager.AppSettings["OstendoHost"];

                    fbstr.Port = int.Parse(ConfigurationManager.AppSettings["OstendoPort"]);
                    _ostendoCnn = new FbConnection(fbstr.ConnectionString);
                }
                return _ostendoCnn;
            }
        }
        public static FbConnection SqlAccCnn
        {
            get
            {
                if (_sqlAccCnn == null)
                {
                    FbConnectionStringBuilder fbstr = new FbConnectionStringBuilder();
                    fbstr.UserID = ConfigurationManager.AppSettings["SqlAccUID"];
                    fbstr.Password = ConfigurationManager.AppSettings["SqlAccPWD"];
                    fbstr.Database = ConfigurationManager.AppSettings["SqlAccDbPath"];
                    fbstr.DataSource = ConfigurationManager.AppSettings["SqlAccHost"];
                   
                    fbstr.Port = int.Parse(ConfigurationManager.AppSettings["SqlAccPort"]);
                    _sqlAccCnn = new FbConnection(fbstr.ConnectionString);
                }
                return _sqlAccCnn;
            }
        }

     


        public static void LogoutSqlAcc()
        {
            _ComServer.Logout();
            FreeBiz(_ComServer);
            _ComServer = null;
        }


        public static DBSetting MydbSetting
        {
            get
            {
                if(_mydbSetting == null)
                {
                    _mydbSetting = new DBSetting(DBServerType.SQL2000, ConfigurationManager.AppSettings["SQLServer"], ConfigurationManager.AppSettings["SQLDBUID"], ConfigurationManager.AppSettings["SQLDBPWD"], ConfigurationManager.AppSettings["SQLDBName"]);
                }
                return _mydbSetting;
            }

            set
            {
                _mydbSetting = value;
            }
        }

        public static UserSession MyDbSession
        {
            get
            {
      
                return _mydbSession;
            }

            set
            {
                _mydbSession = value;
            }
        }

        public static string AccLink
        {
            get
            {
                return ConfigurationManager.AppSettings["AccLink"];
            }
        }

        public static ILog MLogger
        {
            get
            {
                if(mLogger == null)
                {
                    try
                    {
                        log4net.Config.XmlConfigurator.Configure();
                        mLogger = LogManager.GetLogger("servicelog");
                     
                    }
                    catch (Exception ex)
                    {

                        throw ex;
                    }
                
                }
                return mLogger;
            }

        }

        public static string BaseUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["BaseUrl"];
            }

            set
            {
                _baseUrl = value;
            }
        }

        public static string DefaultAutoCountID
        {
            get
            {
                return ConfigurationManager.AppSettings["DefaultAutoCountID"];
            }

            set
            {
                _DefaultAutoCountID = value;
            }
        }

        public static string DefaultAutoCountPWD
        {
            get
            {
                return ConfigurationManager.AppSettings["DefaultAutoCountPWD"];
            }

            set
            {
                _DefaultAutoCountPWD = value;
            }
        }
    }
}
