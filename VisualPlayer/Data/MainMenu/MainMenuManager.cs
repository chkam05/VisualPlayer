using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualPlayer.Data.MainMenu
{
    public class MainMenuManager
    {

        //  VARIABLES

        private static MainMenuManager _instance;
        private static object _instanceLock = new object();

        private MainMenuDataContext _dataContext;


        //  GETTERS & SETTERS

        public static MainMenuManager Instance
        {
            get
            {
                lock (_instanceLock)
                {
                    if (_instance == null)
                        _instance = new MainMenuManager();

                    return _instance;
                }
            }
        }

        public MainMenuDataContext DataContext
        {
            get => _dataContext;
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Private MainMenuManager class constructor. </summary>
        private MainMenuManager()
        {
            _dataContext = new MainMenuDataContext();
        }

        #endregion CLASS METHODS

    }
}
