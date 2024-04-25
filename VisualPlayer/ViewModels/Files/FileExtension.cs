using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using VisualPlayer.Data.Attributes;
using VisualPlayer.Utilities;

namespace VisualPlayer.ViewModels.Files
{
    public class FileExtension : BaseViewModel
    {

        //  VARIABLES

        private ObservableCollection<string> _extensions;
        private string _name;


        //  GETTERS & SETTERS

        public ObservableCollection<string> Extensions
        {
            get => _extensions;
            set
            {
                UpdateProperty(ref _extensions, value);
                _extensions.CollectionChanged += ExtensionsCollectionChanged;
            }
        }

        public string Name
        {
            get => _name;
            set => UpdateProperty(ref _name, value);
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> FileItem class constructor. </summary>
        /// <param name="extensions"> Extensions collection. </param>
        /// <param name="name"> Name. </param>
        public FileExtension(IEnumerable<string> extensions, string name)
        {
            Extensions = new ObservableCollection<string>(extensions);
            Name = name;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get default file extension for all files. </summary>
        public static FileExtension GetDefaultFileExtension()
        {
            return new FileExtension(new string[] { "*.*" }, "All Files");
        }

        #endregion CLASS METHODS

        #region NOTIFICATION PROPERTIES CHANGED METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after changing extension collection. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Notify Collection Changed Event Arguments.. </param>
        protected virtual void ExtensionsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            NotifyPropertyChanged(nameof(Extensions));
        }

        #endregion NOTIFICATION PROPERTIES CHANGED METHODS

    }
}
