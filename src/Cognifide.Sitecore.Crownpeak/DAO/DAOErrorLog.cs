using Crownpeak.Sitecore.DQM.Logic;
using System;
using System.IO;
using System.Web.Hosting;

namespace Crownpeak.Sitecore.DQM.DAO
{
    public class DAOErrorLog
    {
        private static DAOErrorLog instance;

        private DAOErrorLog() { }

        public static DAOErrorLog GetInstance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DAOErrorLog();
                }
                return instance;
            }
        }

        internal void ErrorLogWrite(string className, string methodName, string errorMsg)
        {
            if (Settings.Log == "1")
            {
                string errorMessage = string.Format("Log at : {0}| Method Name : {1} | {2} --> {3}", DateTime.Now.ToString(), className, methodName, errorMsg);

                string filePath = HostingEnvironment.ApplicationPhysicalPath + @"/Error_Log.txt";
                StreamWriter sw = new StreamWriter(filePath, true);

                sw.WriteLine(errorMessage);
                sw.Flush();
                sw.Close();
            }
        }
    }

}