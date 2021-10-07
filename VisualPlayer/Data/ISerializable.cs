using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace chkam05.VisualPlayer.Data
{
    public interface ISerializable
    {

        //  METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Serialize object to XML. </summary>
        /// <returns> Serialized XElement object. </returns>
        XElement SerializeToXML();

        //  --------------------------------------------------------------------------------
        /// <summary> Deserialize and set object data from XML. </summary>
        /// <param name="xmlObject"> Serialized XML object. </param>
        void DeserializeFromXML(XElement xmlObject);

    }
}
