using System;
using System.Collections.Generic;
using System.Deployment.Application;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace chkam05.VisualPlayer.Utilities
{
    public class ApplicationHelper : IDisposable
    {

        //  VARIABLES

        private static ApplicationHelper _instance;
        private Mutex _mutex;


        //  GETTERS & SETTERS

        public static ApplicationHelper Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ApplicationHelper();

                return _instance;
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> ApplicationHelper private class singleton constructor. </summary>
        private ApplicationHelper()
        {
            //
        }

        #endregion CLASS METHODS

        #region DISPOSABLE INTERFACE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Dispose interface method. </summary>
        public void Dispose()
        {
            DisposeMutex();
        }

        #endregion DISPOSABLE INTERFACE METHODS

        #region INFORMATION GETTERS

        //  --------------------------------------------------------------------------------
        /// <summary> Get application assembly name. </summary>
        /// <returns> Application assembly name. </returns>
        public string GetApplicationName()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var assemblyName = assembly.GetName();
            return assemblyName?.Name;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get application assembly company. </summary>
        /// <returns> Application assembly company. </returns>
        public string GetApplicationCompany()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var attributes = assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), true);

            if (attributes.Length > 0)
                return ((AssemblyCompanyAttribute)attributes.FirstOrDefault())?.Company;

            return null;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get application assembly copyright. </summary>
        /// <returns> Application assembly copyright. </returns>
        public string GetApplicationCopyright()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var attributes = assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), true);

            if (attributes.Length > 0)
                return ((AssemblyCopyrightAttribute)attributes.FirstOrDefault())?.Copyright;

            return null;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get application executable file location path. </summary>
        /// <returns> Application executable file location path. </returns>
        public string GetApplicationLocation()
        {
            return Assembly.GetEntryAssembly().Location;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get application executable file directory path. </summary>
        /// <returns> Application executable file location path. </returns>
        public string GetApplicationPath()
        {
            return Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get application assembly title. </summary>
        /// <returns> Application assembly title. </returns>
        public string GetApplicationTitle()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var attributes = assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), true);

            if (attributes.Length > 0)
                return ((AssemblyTitleAttribute)attributes.FirstOrDefault())?.Title;

            return null;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get application verion. </summary>
        /// <returns> Application version. </returns>
        public Version GetApplicationVersion()
        {
            try
            {
                return ApplicationDeployment.CurrentDeployment.CurrentVersion;
            }
            catch (Exception)
            {
                //
            }

            var assembly = Assembly.GetExecutingAssembly();
            var assemblyName = assembly.GetName();

            return assemblyName?.Version;
        }

        #endregion INFORMATION GETTERS

        #region INSTANCES MANAGEMENT

        //  --------------------------------------------------------------------------------
        /// <summary> Check if instance of application is running. </summary>
        /// <returns> True - instance of application is already running; False - otherwise. </returns>
        public bool IsApplicationInstanceRunning()
        {
            var appName = GetApplicationName();

            DisposeMutex();

            _mutex = new Mutex(true, appName, out bool isNewInstance);

            if (!isNewInstance)
                DisposeMutex();

            return !isNewInstance;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Dispose current instance of Mutex. </summary>
        private void DisposeMutex()
        {
            if (_mutex != null)
                _mutex.Dispose();

            _mutex = null;
        }

        #endregion INSTANCES MANAGEMENT

    }
}
