using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace chkam05.VisualPlayer.Data.Files
{
    public class BaseFile : ISerializable
    {

        //  VARIABLES

        public string FilePath { get; internal set; }


        #region GETTERS & SETTERS

        public bool Exists
        {
            get => File.Exists(FilePath);
        }

        public string FileName
        {
            get => !string.IsNullOrEmpty(FilePath)
                ? Path.GetFileNameWithoutExtension(FilePath)
                : null;
        }

        #endregion GETTERS & SETTERS


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> BaseFile class constructor. </summary>
        /// <param name="filePath"> Path to file. </param>
        /// <param name="loadMetadata"> Load metadata at creation. </param>
        public BaseFile(string filePath, bool loadMetadata = true)
        {
            FilePath = filePath;

            if (loadMetadata)
                FillMetadata();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Basic BaseFile (data free) class constructor. </summary>
        internal BaseFile()
        {
            //
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Deserialize object from XML. </summary>
        /// <param name="xmlObject"> Serialized XML object. </param>
        /// <returns> BaseFile object with data deserialized from XML. </returns>
        public static BaseFile FromXML(XElement xmlObject)
        {
            var baseFile = new BaseFile();
            baseFile.DeserializeFromXML(xmlObject);
            return baseFile;
        }

        #endregion CLASS METHODS

        #region METADATA MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Fill additional informations about file. </summary>
        public virtual void FillMetadata()
        {
            //
        }

        #endregion METADATA MANAGEMENT METHODS

        #region SERIALIZATION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Serialize object to XML. </summary>
        /// <returns> Serialized XML object. </returns>
        public virtual XElement SerializeToXML()
        {
            XElement root = new XElement($"{this.GetType().Name}");
            root.Add(new XAttribute("Name", this.FileName));

            root.Add(new XElement(nameof(FilePath), FilePath));

            return root;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Deserialize and set object data from XML. </summary>
        /// <param name="xmlObject"> Serialized XML object. </param>
        public virtual void DeserializeFromXML(XElement xmlObject)
        {
            if (xmlObject.Name != typeof(BaseFile).Name)
                throw new ArgumentException("XML object does not represents this kind of object.");

            FilePath = xmlObject.Element(nameof(FilePath)).Value;
        }

        #endregion SERIALIZATION METHODS

    }
}
