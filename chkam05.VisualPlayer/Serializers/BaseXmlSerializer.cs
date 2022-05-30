using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace chkam05.VisualPlayer.Serializers
{
    public class BaseXmlSerializer<T> : IDisposable where T : class
    {

        //  CONST

        protected const string ROOT_NAME = "root";
        protected const string TYPE_ATTRIB_NAME = "Type";


        //  VARIABLES

        public XElement Root { get; protected set; }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> BaseXmlSerializer class constructor. </summary>
        /// <param name="rootTitle"> Title of root XML element. </param>
        public BaseXmlSerializer(string rootTitle = ROOT_NAME)
        {
            Root = new XElement(rootTitle);
            Root.Add(GetTypeAttribute(typeof(T)));
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Free, Release, or reset unmanaged resources. </summary>
        public void Dispose()
        {
            Root = null;
        }

        #endregion CLASS METHODS

        #region ATTRIBUTES

        //  --------------------------------------------------------------------------------
        /// <summary> Get property XML Element attribute title. </summary>
        /// <param name="propertyInfo"> Property with XML Element attribute. </param>
        /// <returns> Title of XML Element attribute. </returns>
        protected string GetXmlElementAttribName(PropertyInfo propertyInfo)
        {
            if (propertyInfo.CustomAttributes.Any(a => a.AttributeType == typeof(XmlElementAttribute)))
            {
                var attribute = propertyInfo.CustomAttributes
                    .First(a => a.AttributeType == typeof(XmlElementAttribute));

                if (attribute.ConstructorArguments != null && attribute.ConstructorArguments.Any())
                {
                    var nameArgument = attribute.ConstructorArguments
                        .FirstOrDefault(a => a.Value.GetType() == typeof(string));

                    if (!string.IsNullOrEmpty((string)nameArgument.Value))
                        return (string)nameArgument.Value;
                }
            }

            return null;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Check if property has XML Element attribute. </summary>
        /// <param name="propertyInfo"> Property. </param>
        /// <returns> True - property contains XML Element attribute; False - otherwise. </returns>
        protected bool HasXmlElementAttrib(PropertyInfo propertyInfo)
        {
            return propertyInfo.CustomAttributes
                .Any(a => a.AttributeType == typeof(XmlElementAttribute));
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Check if property has XML Ignore attribute. </summary>
        /// <param name="propertyInfo"> Property. </param>
        /// <returns> True - property contains XML Ignore attribute; False - otherwise. </returns>
        protected bool HasXmlIgnoreAttrib(PropertyInfo propertyInfo)
        {
            return propertyInfo.CustomAttributes
                .Any(a => a.AttributeType == typeof(XmlIgnoreAttribute));
        }

        #endregion ATTRIBUTES

        #region FILES MANAGEMENT

        //  --------------------------------------------------------------------------------
        /// <summary> Load XML from file. </summary>
        /// <param name="filePath"> XML file path. </param>
        public virtual void LoadFromFile(string filePath)
        {
            Root = XElement.Load(filePath);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Save XML to file. </summary>
        /// <param name="filePath"> Path to save XML. </param>
        public virtual void SaveToFile(string filePath)
        {
            Root.Save(filePath);
        }

        #endregion FILES MANAGEMENT

        #region SERIALIZATION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Deserialize XML data structure to list of objects. </summary>
        /// <returns> List of deserialized objects. </returns>
        public virtual List<T> Deserialize()
        {
            throw new NotImplementedException();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Deserialize single XML object data structure to object. </summary>
        /// <param name="xmlObject"> XML object data structure. </param>
        /// <returns> Deserialized object. </returns>
        public virtual T Deserialize(XElement xmlObject)
        {
            throw new NotImplementedException();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Serialize objects collection to XML data structure. </summary>
        /// <param name="objects"> Collection of objects. </param>
        public virtual void Serialize(IEnumerable<T> objects)
        {
            throw new NotImplementedException();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Serialize single object to XML data structure. </summary>
        /// <param name="object"> Object to serialize. </param>
        public virtual XElement Serialize(T @object)
        {
            throw new NotImplementedException();
        }

        #endregion SERIALIZATION METHODS

        #region UTILITY METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Get XML attribute with property Type. </summary>
        /// <param name="type"> Property Type. </param>
        /// <param name="isFullName"> True - use property full name; False - use typical name. </param>
        /// <returns> XML attribute with property Type. </returns>
        protected XAttribute GetTypeAttribute(Type type, bool isFullName = false)
        {
            return new XAttribute(TYPE_ATTRIB_NAME, isFullName ? type.FullName : type.Name);
        }

        #endregion UTILITY METHODS

    }
}
