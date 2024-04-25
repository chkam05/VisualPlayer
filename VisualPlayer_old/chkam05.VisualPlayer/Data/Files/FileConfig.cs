
namespace chkam05.VisualPlayer.Data.Files
{
    public class FileConfig
    {

        //  VARIABLES

        public string Extension { get; private set; }
        public string Name { get; private set; }
        public FileGroup Group { get; private set; }
        public FileKind Kind { get; private set; }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> FileConfig class constructor. </summary>
        /// <param name="name"> File type name. </param>
        /// <param name="extension"> File type extension. </param>
        /// <param name="group"> File type group. </param>
        /// <param name="kind"> File type kind. </param>
        public FileConfig(string name, string extension = "", FileGroup group = FileGroup.NONE, FileKind kind = FileKind.UNKNOWN)
        {
            Name = name;
            Extension = extension;
            Group = group;
            Kind = kind;
        }

        #endregion CLASS METHODS

    }
}
