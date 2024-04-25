using chkam05.VisualPlayer.Utilities;
using System;
using System.IO;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;

namespace chkam05.VisualPlayer.Data.Files
{
    public class SongFile : BaseFile, IPlayable
    {

        //  VARIABLES

        private string _album = "Unknown";
        private string _artist = "Unknown";
        private ImageSource _cover = null;
        private bool _nowPlaying = false;
        private string _title;


        //  GETTERS & SETTERS

        [XmlIgnore]
        public string Album
        {
            get => _album;
            set
            {
                _album = value;
                OnPropertyChanged(nameof(Album));
            }
        }

        [XmlIgnore]
        public string Artist
        {
            get => _artist;
            set
            {
                _artist = value;
                OnPropertyChanged(nameof(Artist));
            }
        }

        [XmlIgnore]
        public ImageSource Cover
        {
            get => _cover;
            set
            {
                _cover = value;
                OnPropertyChanged(nameof(Cover));
            }
        }

        [XmlIgnore]
        public bool NowPlaying
        {
            get => _nowPlaying;
            set
            {
                _nowPlaying = value;
                OnPropertyChanged(nameof(NowPlaying));
            }
        }

        [XmlIgnore]
        public string Title
        {
            get => !string.IsNullOrEmpty(_title) ? _title : FileName;
            set
            {
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        
        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> SongFileModel class constructor. </summary>
        public SongFile() : base()
        {
            //
        }

        //  --------------------------------------------------------------------------------
        /// <summary> SongFileModel class constructor. </summary>
        /// <param name="filePath"> Initial file path. </param>
        /// <param name="loadMetadata"> Load file metadata on create. </param>
        public SongFile(string filePath, bool loadMetadata = true) : base(filePath, loadMetadata)
        {
            //
        }

        #endregion CLASS METHODS

        #region METADATA METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Load file metadata. </summary>
        public override void LoadMetadata()
        {
            //  Load base file metadata.
            base.LoadMetadata();

            //  Load songs metadata.
            if (Exists)
            {
                var fileConfig = FilesManager.GetFileTypeByExtension(FileExtension);

                if (fileConfig != null)
                {
                    switch (fileConfig.Kind)
                    {
                        case FileKind.MP3:
                            LoadMp3Metadata();
                            break;

                        default:
                            break;
                    }
                }
            }
        }

        #region Mp3 Metadata

        //  --------------------------------------------------------------------------------
        /// <summary> Load mp3 file type metadata. </summary>
        private void LoadMp3Metadata()
        {
            try
            {
                TagLib.File fileInfo = TagLib.File.Create(FilePath);

                if (fileInfo != null)
                {
                    Album = !string.IsNullOrEmpty(fileInfo.Tag?.Album) 
                        ? fileInfo.Tag?.Album : GetContainingFolderName();

                    Artist = !string.IsNullOrEmpty(fileInfo.Tag?.FirstPerformer) 
                        ? fileInfo.Tag.FirstPerformer : "Unknown";

                    Cover = LoadMp3CoverMetadata(fileInfo);

                    Title = !string.IsNullOrEmpty(fileInfo.Tag?.Title) 
                        ? fileInfo.Tag.Title : null;
                }
                else
                {
                    throw new Exception($"TagLib.File info not created.");
                }
            }
            catch (Exception)
            {
                Album = GetContainingFolderName();
                Artist = "Unknown";
                Cover = null;
                Title = null;
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Load mp3 file type cover metadata. </summary>
        /// <param name="fileInfo"> TagLib file info object. </param>
        private ImageSource LoadMp3CoverMetadata(TagLib.File fileInfo)
        {
            if (fileInfo.Tag?.Pictures?.Any() ?? false)
            {
                try
                {
                    TagLib.IPicture picture = fileInfo.Tag.Pictures.First();
                    MemoryStream stream = new MemoryStream(picture.Data.Data);
                    stream.Seek(0, SeekOrigin.Begin);

                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = stream;
                    bitmapImage.EndInit();
                    bitmapImage.Freeze();

                    return bitmapImage;
                }
                catch (Exception)
                {
                    //
                }
            }

            return null;
        }

        #endregion Mp3 Metadata

        #endregion METADATA METHODS

    }
}
