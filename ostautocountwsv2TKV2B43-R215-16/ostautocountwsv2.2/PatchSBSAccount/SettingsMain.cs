

using AutoCount.Authentication;
using AutoCount.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatchSBSAccount
{
   public static class SettingsMain
    {
        private static DBSetting _mydbSetting;
        private static UserSession _mydbSession;
        private static string _baseUrl;
        private static string _DefaultAutoCountID;
        private static string _DefaultAutoCountPWD;

        public static UserSession MyDbSession
        {
            get
            {
                if (_mydbSession == null)
                {
                    _mydbSession = new UserSession(MydbSetting);
                }
                return _mydbSession;
            }

            set
            {
                _mydbSession = value;
            }
        }

        public static DBSetting MydbSetting
        {
            get
            {
                if(_mydbSetting == null)
                {
                    _mydbSetting = new DBSetting(DBServerType.SQL2000, ConfigurationManager.AppSettings["SQLServer"], ConfigurationManager.AppSettings["SQLDBUser"], ConfigurationManager.AppSettings["SQLDBPWD"], ConfigurationManager.AppSettings["SQLDBName"]);
                }
                return _mydbSetting;
            }

            set
            {
                _mydbSetting = value;
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
