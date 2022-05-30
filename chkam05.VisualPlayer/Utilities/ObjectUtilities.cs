using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace chkam05.VisualPlayer.Utilities
{
    public static class ObjectUtilities
    {

        //  METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Check if type has specified name. </summary>
        /// <param name="type"> Type. </param>
        /// <param name="typeName"> Type name to check. </param>
        /// <param name="isFullName"> True - check by full name; False - check by short name. </param>
        /// <returns> True - type has speicifeid name; False - otherwise. </returns>
        public static bool CheckTypeByName(Type type, string typeName, bool isFullName = false)
        {
            if (type != null && !string.IsNullOrEmpty(typeName))
                return isFullName ? type.FullName == typeName : type.Name == typeName;

            return false;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Check if array of types contains type with specified name. </summary>
        /// <param name="types"> Array of types. </param>
        /// <param name="typeName"> Type name to check. </param>
        /// <param name="isFullName"> True - check by full name; False - check by short name. </param>
        /// <returns> Found type. </returns>
        public static Type CheckTypesByName(Type[] types, string typeName, bool isFullName = false)
        {
            if (types != null && types.Any() && !string.IsNullOrEmpty(typeName))
            {
                var foundType = types.FirstOrDefault(
                    t => isFullName ? t.FullName == typeName : t.Name == typeName);

                return foundType != null ? foundType : null;
            }

            return null;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get assingable class types from interface. </summary>
        /// <typeparam name="T"> Interface type. </typeparam>
        /// <returns> Array of classes assingable from interface. </returns>
        public static Type[] GetAssingableFrom<T>()
        {
            if (!typeof(T).IsInterface)
                return null;

            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => typeof(T).IsAssignableFrom(p) && p.IsClass)
                .ToArray();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get public valuable properties from class object. </summary>
        /// <typeparam name="T"> Class object type. </typeparam>
        /// <param name="obj"> Class object. </param>
        /// <returns> Array of class properties. </returns>
        public static PropertyInfo[] GetObjectProperties<T>(T obj) where T : class
        {
            return obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get public valuable properties from class type. </summary>
        /// <param name="type"> Class type. </param>
        /// <returns> Array of class properties. </returns>
        public static PropertyInfo[] GetObjectProperties(Type type)
        {
            return type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
        }

    }
}
