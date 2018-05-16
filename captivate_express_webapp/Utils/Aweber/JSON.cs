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
using System.Linq;
using System.Text;
using System.Xml;
using System.Runtime.Serialization.Json;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Aweber
{

    /// <summary>
    /// Provides JSON decoding
    /// </summary>
    internal class JSON
    {
        /// <summary>
        /// Returns the JSON string into an XML element
        /// </summary>
        /// <param name="xml">the JSON string response from AWeber after an API call</param>
        /// <returns></returns>
        public static XmlDocument Read(String xml)
        {

            if (!String.IsNullOrEmpty(xml))
            {
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(xml);
                XmlReader reader =
                    JsonReaderWriterFactory
                        .CreateJsonReader(buffer, new XmlDictionaryReaderQuotas());

                XmlDocument doc = new XmlDocument();
                doc.Load(reader);
                return doc;
            }
            else
            {
                return null;
            }
        }

    }
}
