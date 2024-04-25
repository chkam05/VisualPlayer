using chkam05.VisualPlayer.Data.Files;
using chkam05.VisualPlayer.Data.Lyrics;
using chkam05.VisualPlayer.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace chkam05.VisualPlayer.Serializers
{
    public class LyricsSerializer : BaseXmlSerializer<Lyrics>
    {

        //  CONST

        protected const string FILE_IDENTIFIER = "fileId";
        protected const string FILE_NAME = "fileName";
        protected const string TIMESPAN_FORMAT = "hh\\:mm\\:ss\\.fff";


        //  VARIABLES

        private Type objectTargetType = typeof(Lyrics);
        private Type timeSpanType = typeof(TimeSpan);

        public string FileIdentifier { get; private set; }
        public string FileName { get; private set; }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> LyricsSerializer class constructor. </summary>
        public LyricsSerializer() : base("Lyrics")
        {
            //
        }

        //  --------------------------------------------------------------------------------
        /// <summary> LyricsSerializer class constructor. </summary>
        /// <param name="file"> File related to lyrics. </param>
        public LyricsSerializer(IFile file) : base("Lyrics")
        {
            FileName = file.FileName;
            FileIdentifier = file.GetChecksum();

            Root.Add(new XAttribute(FILE_IDENTIFIER, FileIdentifier));
            Root.Add(new XAttribute(FILE_NAME, FileName));
        }

        #endregion CLASS METHODS

        #region DATA CHECK METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Check if lyrics belong to file by identifier is correct. </summary>
        /// <param name="file"> Checking file. </param>
        /// <returns> True - file identifier is correct; False - otherwise. </returns>
        public bool IsCorrectFileId(IFile file)
        {
            var checksum = FileIdentifier = file.GetChecksum();
            return checksum == FileIdentifier;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Check if lyrics belong to file by name is correct. </summary>
        /// <param name="file"> Checking file. </param>
        /// <returns> True - file name is correct; False - otherwise. </returns>
        public bool IsCorrectFileName(IFile file)
        {
            return file.FileName == FileName;
        }

        #endregion DATA CHECK METHODS

        #region FILES MANAGEMENT

        //  --------------------------------------------------------------------------------
        /// <summary> Load XML from file. </summary>
        /// <param name="filePath"> XML file path. </param>
        public override void LoadFromFile(string filePath)
        {
            Root = XElement.Load(filePath);

            var fileId = Root.Attribute(FILE_IDENTIFIER);
            if (fileId != null)
                FileIdentifier = fileId.Value;

            var fileName = Root.Attribute(FILE_NAME);
            if (fileName != null)
                FileName = fileName.Value;
        }

        #endregion FILES MANAGEMENT

        #region SERIALIZATION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Deserialize XML data structure to list of Lyrics objects. </summary>
        /// <returns> List of deserialized Lyrics objects. </returns>
        public override List<Lyrics> Deserialize()
        {
            var result = new List<Lyrics>();

            if (Root.HasElements)
            {
                foreach (var xmlObject in Root.Elements())
                {
                    //  Deserialize single XML object.
                    var @object = Deserialize(xmlObject);

                    //  Add deserialized object to return collection.
                    if (@object != null)
                        result.Add(@object);
                }
            }

            return result.Any() ? result : null;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Deserialize single XML object data structure to Lyrics object. </summary>
        /// <param name="xmlObject"> XML object data structure. </param>
        /// <returns> Lyrics object. </returns>
        public override Lyrics Deserialize(XElement xmlObject)
        {
            if (ObjectUtilities.CheckTypeByName(objectTargetType, xmlObject.Attribute(TYPE_ATTRIB_NAME)?.Value, false))
            {
                var properties = ObjectUtilities.GetObjectProperties(objectTargetType);

                var result = (Lyrics)Activator.CreateInstance(objectTargetType);

                foreach (var prop in properties)
                {
                    var propertyName = prop.Name;

                    //  Check if property contains XmlIgnore attribute.
                    if (HasXmlIgnoreAttrib(prop))
                        continue;

                    //  Check if property contains XmlElement attribute, and get name.
                    if (HasXmlElementAttrib(prop))
                    {
                        var customName = GetXmlElementAttribName(prop);

                        if (!string.IsNullOrEmpty(customName))
                            propertyName = customName;
                    }

                    //  Get XML property and set value.
                    var xmlProperty = xmlObject.Elements().FirstOrDefault(e => e.Name == propertyName);

                    if (xmlProperty != null)
                    {
                        //  Special check for TimeSpan.
                        if (prop.PropertyType == timeSpanType)
                        {
                            if (TimeSpan.TryParseExact(xmlProperty.Value, TIMESPAN_FORMAT, CultureInfo.InvariantCulture, out TimeSpan timeSpan))
                                prop.SetValue(result, timeSpan);
                            else
                                return null;
                        }
                        else
                        {
                            prop.SetValue(result, xmlProperty.Value);
                        }
                    }
                }

                return result;
            }

            return null;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Serialize Lyrics objects collection to XML data structure. </summary>
        /// <param name="objects"> Collection of Lyrics objects. </param>
        public override void Serialize(IEnumerable<Lyrics> objects)
        {
            if (objects != null && objects.Any())
            {
                foreach (var @object in objects)
                {
                    //  Serialize single object.
                    var xmlObject = Serialize(@object);

                    //  Add to root XML items container.
                    if (xmlObject != null)
                        Root.Add(xmlObject);
                }
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Serialize single Lyrics object to XML data structure. </summary>
        /// <param name="object"> Lyrics object to serialize. </param>
        public override XElement Serialize(Lyrics @object)
        {
            var properties = ObjectUtilities.GetObjectProperties(@object);

            var xmlObject = new XElement(@object.GetType().Name);
            xmlObject.Add(GetTypeAttribute(@object.GetType()));

            foreach (var prop in properties)
            {
                var propertyName = prop.Name;

                //  Check if property contains XmlIgnore attribute.
                if (HasXmlIgnoreAttrib(prop))
                    continue;

                //  Check if property contains XmlElement attribute, and get name.
                if (HasXmlElementAttrib(prop))
                {
                    var customName = GetXmlElementAttribName(prop);

                    if (!string.IsNullOrEmpty(customName))
                        propertyName = customName;
                }

                //  Create property and add to XML item.
                XElement xmlProperty = null;

                //  Special check for TimeSpan.
                if (prop.PropertyType == timeSpanType)
                {
                    var timeSpan = (TimeSpan) prop.GetValue(@object);
                    xmlProperty = new XElement(propertyName, timeSpan.ToString(TIMESPAN_FORMAT));
                }
                else
                {
                    xmlProperty = new XElement(propertyName, prop.GetValue(@object));
                }

                xmlProperty.Add(GetTypeAttribute(prop.PropertyType));
                xmlObject.Add(xmlProperty);
            }

            return xmlObject;
        }

        #endregion SERIALIZATION METHODS

    }
}
