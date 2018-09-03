using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Reflection;
using System.Globalization;

namespace Aweber.Factory
{
    /// <summary>
    /// Generic Factory for a singular entity
    /// </summary>
    internal class Base<T>
    {

        /// <summary>
        /// Will build the appropriate entity from the XML (converted JSON) received from Aweber
        /// </summary>
        /// <param name="entry">The xml node for an entry</param>
        /// <param name="adapter">The reference to the adapter (connection to the API)</param>
        /// <returns>An instance of the object</returns>
        public static T Build(XmlNode entry, ref IAdapter adapter)
        {

            Type typeParameterType = typeof(T);

            T entity = (T)Activator.CreateInstance(typeParameterType, adapter);

            XmlDocument doc = new XmlDocument();

            String xml = entry.InnerXml;

            if (entry.FirstChild.Name != "root")
            {
                xml = "<root>" + entry.InnerXml + "</root>";
            }

            doc.LoadXml(xml);

            SortedList<String, String> assignValues = new SortedList<string, string>();

            foreach (XmlNode e in doc.ChildNodes[0].ChildNodes)
            {

                assignValues.Add(e.Name.ToString(), e.InnerText);

            }

            (entity as Entity.Base).clear_dirty();
            (entity as Entity.Base).load_values(assignValues);

            return entity;

        }

    }
}
