using chkam05.VisualPlayer.Utilities;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace chkam05.VisualPlayer.Data.Files
{
    public class SongFile : BaseFile, IPlayableFile
    {

        //  VARIABLES

        private ImageSource _cover;
        private bool _isPlaying;
        private string _title = string.Empty;

        public string Album { get; set; }
        public string Artist { get; set; }
        public PackIconKind Icon { get; private set; }

        public ImageSource Cover
        {
            get => _cover;
            set => SetCover(value);
        }

        public bool IsPlaying
        {
            get => _isPlaying;
            set => SetIsPlaying(value);
        }

        public string Title
        {
            get => string.IsNullOrEmpty(_title) ? FileName : _title;
            set => _title = value;
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> SongFile class constructor. </summary>
        /// <param name="filePath"> Path to file. </param>
        /// <param name="loadMetadata"> Load metadata at creation. </param>
        public SongFile(string filePath, bool loadMetadata = true) : base(filePath, loadMetadata)
        {
            UpdateIcon();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Basic SongFile (data free) class constructor. </summary>
        internal SongFile()
        {
            //
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Deserialize object from XML. </summary>
        /// <param name="xmlObject"> Serialized XML object. </param>
        /// <returns> SongFile object with data deserialized from XML. </returns>
        public static new SongFile FromXML(XElement xmlObject)
        {
            var songFile = new SongFile();
            songFile.DeserializeFromXML(xmlObject);
            return songFile;
        }

        #endregion CLASS METHODS

        #region DATA MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Set cover image. </summary>
        /// <param name="cover"> Cover image source. </param>
        private void SetCover(ImageSource cover)
        {
            _cover = cover;
            UpdateIcon();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Set is playing flag. </summary>
        /// <param name="isPlaying"> New is playing flag value. </param>
        private void SetIsPlaying(bool isPlaying)
        {
            _isPlaying = isPlaying;
            UpdateIcon();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update file item icon. </summary>
        private void UpdateIcon()
        {
            if (IsPlaying)
                Icon = PackIconKind.Play;
            else if (_cover != null)
                Icon = PackIconKind.None;
            else
                Icon = PackIconKind.MusicNote;
        }

        #endregion DAT MANAGEMENT METHODS

        #region METADATA MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Fill additional informations about file. </summary>
        public override void FillMetadata()
        {
            UpdateIcon();
        }

        #endregion METADATA MANAGEMENT METHODS

        #region SERIALIZATION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Serialize object to XML. </summary>
        /// <returns> Serialized XElement object. </returns>
        public override XElement SerializeToXML()
        {
            XElement root = base.SerializeToXML();
            root.Name = typeof(SongFile).Name;
            root.Add(new XAttribute(nameof(IsPlaying), _isPlaying));

            root.Add(new XElement(nameof(Album), Album));
            root.Add(new XElement(nameof(Artist), Artist));
            root.Add(new XElement(nameof(Title), _title));

            if (_cover != null)
            {
                string coverBase64 = ImageTools.ImageSourceToBase64(_cover, new PngBitmapEncoder());

                if (!string.IsNullOrEmpty(coverBase64))
                    root.Add(new XElement(nameof(Cover), coverBase64));
            }

            return root;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Deserialize and set object data from XML. </summary>
        /// <param name="xmlObject"> Serialized XML object. </param>
        public override void DeserializeFromXML(XElement xmlObject)
        {
            if (xmlObject.Name != typeof(SongFile).Name)
                throw new ArgumentException("XML object does not represents this kind of object.");

            xmlObject.Name = GetType().BaseType.Name;
            base.DeserializeFromXML(xmlObject);

            Album = xmlObject.Element(nameof(Album)).Value;
            Artist = xmlObject.Element(nameof(Artist)).Value;
            _title = xmlObject.Element(nameof(Title)).Value;

            if (xmlObject.Elements().Any(e => e.Name == nameof(Cover)))
            {
                string coverBase64 = xmlObject.Element(nameof(Cover)).Value;

                if (!string.IsNullOrEmpty(coverBase64))
                    Cover = ImageTools.ImageSourceFromBase64(coverBase64);
            }

            _isPlaying = bool.Parse(xmlObject.Attribute(nameof(IsPlaying)).Value);

            UpdateIcon();
        }

        #endregion SERIALIZATION METHODS

    }
}
