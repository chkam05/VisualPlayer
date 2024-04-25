using chkam05.VisualPlayer.Data.Files;
using chkam05.VisualPlayer.Utilities;
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
    public class PlayListSerializer : BaseXmlSerializer<IPlayable>
    {

        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> PlayListSerializer class constructor. </summary>
        public PlayListSerializer() : base("PlayList")
        {
            //
        }

        #endregion CLASS METHODS

        #region SERIALIZATION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Deserialize XML data structure to list of IPlayable objects. </summary>
        /// <returns> List of deserialized IPlayable objects. </returns>
        public override List<IPlayable> Deserialize()
        {
            var result = new List<IPlayable>();

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
        /// <summary> Deserialize single XML object data structure to IPlayable object. </summary>
        /// <param name="xmlObject"> XML object data structure. </param>
        /// <returns> IPlayable object. </returns>
        public override IPlayable Deserialize(XElement xmlObject)
        {
            var targetType = GetObjectTargetType(xmlObject);

            if (targetType != null)
            {
                var properties = ObjectUtilities.GetObjectProperties(targetType);

                var result = (IPlayable)Activator.CreateInstance(targetType);

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
                        prop.SetValue(result, xmlProperty.Value);
                }

                return result;
            }

            return null;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Serialize IPlayable objects collection to XML data structure. </summary>
        /// <param name="objects"> Collection of IPlayable objects. </param>
        public override void Serialize(IEnumerable<IPlayable> objects)
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
        /// <summary> Serialize single IPlayable object to XML data structure. </summary>
        /// <param name="object"> IPlayable object to serialize. </param>
        public override XElement Serialize(IPlayable @object)
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
                var xmlProperty = new XElement(propertyName, prop.GetValue(@object));
                xmlProperty.Add(GetTypeAttribute(prop.PropertyType));
                xmlObject.Add(xmlProperty);
            }

            return xmlObject;
        }

        #endregion SERIALIZATION METHODS

        #region UTILITY METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Get assingable object type from XML object. </summary>
        /// <param name="xmlObject"> XML object. </param>
        /// <returns> Type of target object. </returns>
        private Type GetObjectTargetType(XElement xmlObject)
        {
            var assingableTypes = ObjectUtilities.GetAssingableFrom<IPlayable>();

            return ObjectUtilities.CheckTypesByName(
                assingableTypes, xmlObject.Attribute(TYPE_ATTRIB_NAME)?.Value, false);
        }

        #endregion UTILITY METHODS

    }
}
