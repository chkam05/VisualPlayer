using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace chkam05.VisualPlayer.Utilities
{
    public static class EnumTool<T> where T : Enum
    {

        //  VARIABLES

        public static int Count
        {
            get => Items.Count;
        }

        public static List<string> Names
        {
            get => Enum.GetNames(typeof(T)).ToList();
        }

        public static List<T> Items
        {
            get => Enum.GetValues(typeof(T)).Cast<T>().ToList();
        }

        public static List<string> Descriptions
        {
            get => Enum.GetValues(typeof(T)).Cast<Enum>().Select(x => GetDescription(x)).ToList();
        }


        //  METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Get index of enum object. </summary>
        /// <param name="enumObject"> Enum object. </param>
        /// <returns> Index of enum object. </returns>
        public static int GetIndex(T enumObject)
        {
            return Array.IndexOf(Enum.GetValues(typeof(T)), enumObject);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get enum object by index. </summary>
        /// <param name="enumIndex"> Index of enum object. </param>
        /// <returns> Enum object. </returns>
        public static T GetByIndex(int enumIndex)
        {
            return Items.FirstOrDefault(v => Items.IndexOf(v) == enumIndex);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get enum object description attribute value. </summary>
        /// <param name="enumObject"> Enum object. </param>
        /// <returns> Enum object description attribute value. </returns>
        private static string GetDescription(Enum enumObject)
        {
            string enumName = Enum.GetName(typeof(T), enumObject);

            if (enumName != null)
            {
                FieldInfo field = typeof(T).GetField(enumName);

                if (field != null)
                {
                    var attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;

                    if (attribute != null)
                        return attribute.Description;
                }

                return enumName;
            }

            return string.Empty;
        }

    }
}
