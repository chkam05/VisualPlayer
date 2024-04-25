using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shell;

namespace chkam05.VisualPlayer.Utilities
{
    public class JumpListCreator
    {

        //  CONST

        public static readonly string RESOURCES_DLL = Path.Combine(
            ApplicationHelper.Instance.GetApplicationPath(), "Resources.dll");


        //  VARIABLES

        private JumpList _jumpList;


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> JumpListCreator class constructor. </summary>
        public JumpListCreator()
        {
            _jumpList = new JumpList();
        }

        #endregion CLASS METHODS

        #region ITEMS MANAGEMENT

        //  --------------------------------------------------------------------------------
        /// <summary> Add jump list task item to application jump list. </summary>
        /// <param name="title"> Title. </param>
        /// <param name="description"> Tooltip description. </param>
        /// <param name="execPath"> Application exec file path. </param>
        /// <param name="execArgs"> Arguments. </param>
        /// <param name="iconIndex"> Index of icon. </param>
        /// <param name="iconsResourcePath"> Path to resource files with icons. </param>
        public void AddJumpListItem(string title, string description, string execPath, string execArgs,
            int iconIndex = 0, string iconsResourcePath = "")
        {
            JumpTask task = new JumpTask();

            task.Title = title;
            task.Description = description;
            task.ApplicationPath = execPath;
            task.Arguments = execArgs;
            task.IconResourcePath = !string.IsNullOrEmpty(iconsResourcePath) ? iconsResourcePath : RESOURCES_DLL;
            task.IconResourceIndex = iconIndex;

            _jumpList.JumpItems.Add(task);
        }

        #endregion ITEMS MANAGEMENT

        #region SETUP METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Setup jump list and assign it to application. </summary>
        /// <param name="application"> Application. </param>
        public void SetupJumpList(App application)
        {
            JumpList.SetJumpList(application, _jumpList);
            _jumpList.Apply();
        }

        #endregion SETUP METHODS

    }
}
