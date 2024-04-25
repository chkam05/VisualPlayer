using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chkam05.VisualPlayer.Data.Files
{
    public interface IFile : INotifyPropertyChanged
    {

        //  GETTERS & SETTERS

        string FilePath { get; }
        string FileExtension { get; }
        string FileName { get; }
        string FileNameWithExt { get; }
        bool Exists { get; }


        //  VARIABLES

        string GetChecksum();
        void LoadMetadata();

    }
}
