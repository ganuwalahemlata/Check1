using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using KontinuityCRM.Models.Enums;

namespace KontinuityCRM.Helpers
{
    /// <summary>
    /// Kontinuity CRM Configurations
    /// </summary>
    public static class KontinuityCRMConfiguration
    {
        /// <summary>
        /// Mail Server
        /// </summary>
        public static string MailServer
        {
            get
            {
                return ConfigurationManager.AppSettings["MailServer"];
            }
        }
        /// <summary>
        /// Port Specified
        /// </summary>
        public static int MailPort
        {
            get
            {
                return int.Parse(ConfigurationManager.AppSettings["MailPort"]);
            }
        }
        /// <summary>
        /// Mail User Name
        /// </summary>
        public static string MailUsername
        {
            get
            {
                return ConfigurationManager.AppSettings["MailUsername"];
            }
        }
        /// <summary>
        /// Mail Password
        /// </summary>
        public static string MailPassword
        {
            get
            {
                return ConfigurationManager.AppSettings["MailPassword"];
            }
        }
        /// <summary>
        /// Mail To
        /// </summary>
        public static string MailTo
        {
            get
            {
                return ConfigurationManager.AppSettings["MailTo"];
            }
        }
        /// <summary>
        /// SSL enabled in mail or not
        /// </summary>
        public static bool MailEnableSsl
        {
            get
            {
                return ConfigurationManager.AppSettings["MailEnableSsl"].ToUpper() == "YES";
            }
        }
        /// <summary>
        /// Interval For Rebill in Hours
        /// </summary>
        public static int RebillIntervalHour
        {
            get
            {
                try
                {
                    return int.Parse(ConfigurationManager.AppSettings["RebillIntervalHour"]);
                }
                catch {
                    return 24;
                }
            }
        }
        /// <summary>
        /// Indicates Rebill Start Hour
        /// </summary>
        public static int RebillStartHour
        {
            get
            {
                try
                {
                    return int.Parse(ConfigurationManager.AppSettings["RebillStartHour"]);
                }
                catch {
                    return 1;
                }
            }
        }
        /// <summary>
        /// Indicates Rebill Start Minute
        /// </summary>
        public static int RebillStartMinute
        {
            get
            {
                try
                {
                    return int.Parse(ConfigurationManager.AppSettings["RebillStartMinute"]);
                }
                catch
                {
                    return 0;
                }
            }
        }
        /// <summary>
        /// Indicates whether to create Dynamic Assembly or not
        /// </summary>
        public static bool CreateDynamicAssembly
        {
            get
            {
                if (ConfigurationManager.AppSettings["CreateDynamicAssembly"] != null)
                    return bool.Parse(ConfigurationManager.AppSettings["CreateDynamicAssembly"]);
                return false;
            }
        }
        /// <summary>
        /// Default Salvage Profile for KontinuityCRM
        /// </summary>
        public static Models.SalvageProfile DefaultSalvageProfile
        { 
            get {

                //bool on = ConfigurationManager.AppSettings["DeclineRuleDefault"] == "on";

                return new Models.SalvageProfile
                {
                    BillType = EnumHelper<BillType>.Parse(ConfigurationManager.AppSettings["DeclineRuleBillType"]), 
                    BillValue = int.Parse(ConfigurationManager.AppSettings["DeclineRuleBillValue"]),
                    CancelAfter = int.Parse(ConfigurationManager.AppSettings["DeclineRuleCancelAfter"]),
                    Name = "Default",
                };
            } 
        }
    }
}