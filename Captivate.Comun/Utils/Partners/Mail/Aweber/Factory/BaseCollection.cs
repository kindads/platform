/*
* AWeber API .NET SDK v1.0
* Providing the ability to connect a .NET application to the AWeber API.
* 
* Copyright (c) 2011 - Binkd
* Licensed under the GNU General Public License (GNU GPL v3.0)
* 
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.XPath;
using System.Xml;
using System.Reflection;

namespace Captivate.Comun.Utils.Partners.Mail.Aweber.Factory
{
    /// <summary>
    /// Generic Factory for the collection entities
    /// 
    /// Even though all collections contain the same properties at the moment
    /// Each collection has its own factory incase of future changes where
    /// a collection may be modified to have more properties
    /// </summary>
    internal class BaseCollection<T>
    {

        public static Entity.BaseCollection<T> Build(String url, XmlDocument xml, IAdapter adapter)
        {

            Entity.BaseCollection<T> collection = new Entity.BaseCollection<T>(adapter);

            collection.self_link = url;

            foreach (XmlNode e in xml.ChildNodes[0].ChildNodes)
            {
                PropertyInfo info = collection.GetType().GetProperty(e.Name.ToString());

                object value = null;
                if (info != null)
                {
                    if (!String.IsNullOrEmpty(e.InnerText) && !info.PropertyType.IsGenericType)
                    {
                        Type property = info.PropertyType;

                        value = Convert.ChangeType(e.InnerText, Nullable.GetUnderlyingType(info.PropertyType) ?? info.PropertyType);


                    }
                    info.SetValue(collection, value, null);
                }


            }
            collection.entries = new List<T>();
        
            // Build entries
            foreach (XmlNode entry in xml.SelectNodes("//item"))
            {

                T entity = Base<T>.Build(entry, ref adapter);

                if (entity != null)
                {
                    collection.entries.Add(entity);
                }
            }

            return collection;

        }


    }
}
