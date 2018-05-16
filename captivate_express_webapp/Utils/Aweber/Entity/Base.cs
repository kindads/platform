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
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.IO;
using System.Reflection;

namespace Aweber.Entity
{
    /// <summary>
    /// The base entity that all entities are inherited from
    /// Detects changed fields and provides generic functions
    /// applicable to all entities
    /// </summary>
    public class Base : INotifyPropertyChanged
    {

        #region Adapter

        private IAdapter _api = null;
        public IAdapter api { get { return _api; } }

        public Base(IAdapter adapter)
        {
            _api = adapter;
            dirtyList = new List<string>();
        }

        #endregion


        /// <summary>
        /// List of dirty fields
        /// </summary>
        private List<String> dirtyList { get; set; }

        /// <summary>
        /// The value of the HTTP ETag for this resource.
        /// </summary>
        public String http_etag { get { return _http_etag; } }
        private String _http_etag = String.Empty;

        /// <summary>
        /// id
        /// </summary>
        public Int32 id { get { return _id; } }
        private Int32 _id = 0;

        /// <summary>
        /// The unique URL of this resource
        /// </summary>
        public String self_link { get { return _self_link; } }
        private String _self_link = String.Empty;

        /// <summary>
        /// The link to the WADL description of this resource
        /// </summary>
        public String resource_type_link { get { return _resource_type_link; } }
        private String _resource_type_link = String.Empty;



        // Declare the event
        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(String name)
        {

            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }

            if (dirtyList == null)
            {
                dirtyList = new List<string>();
            }

            if (!dirtyList.Contains(name))
            {
                dirtyList.Add(name);
            }
        }

        public void clear_dirty()
        {
            dirtyList = new List<string>();
        }

        public List<String> get_dirty()
        {
            return dirtyList;
        }

        /// <summary>
        /// Loads the property values of the object
        /// </summary>
        /// <param name="values"></param>
        public void load_values(SortedList<String, String> values)
        {
            foreach (String key in values.Keys)
            {
                FieldInfo info = null;

                if (key == "id" || key == "http_etag" || key == "resource_type_link" || key == "self_link")
                {
                    info = typeof(Base).GetField("_" + key, BindingFlags.NonPublic | BindingFlags.Instance);
                }
                else
                {
                    info = this.GetType().GetField("_" + key, BindingFlags.NonPublic | BindingFlags.Instance);
                }


                object value = null;
                if (info != null)
                {
                    if (!String.IsNullOrEmpty(values[key]))
                    {
                        Type property = info.MemberType.GetType();
                        if (info.FieldType == true.GetType())
                        {
                            Int32 result = 0;
                            if (Int32.TryParse(values[key], out result))
                            {
                                value = Convert.ToBoolean(result);
                            }
                            else
                            {
                                value = Convert.ToBoolean(values[key]);
                            }
                        }
                        else
                        {
                            value = Convert.ChangeType(values[key], Nullable.GetUnderlyingType(info.FieldType) ?? info.FieldType);
                        }
                    }
                    info.SetValue(this, value);
                }
            }
        }

        /// <summary>
        /// Will load this object from 
        /// </summary>
        /// <param name="url"></param>
        public void load_from_url(String url)
        {

            // Notes: I know the use of reflection seems rather obscure however this was done to keep as much in line as possible to the PHP 
            // SDK (the first and original SDK by Aweber) and also so less changes need to be made to this .NET SDK in the future.
            // Since PHP is a very dynamic language where .NET is very strongly typed a few ways of doing things in this code will seem different 
            // than the ways .NET programmers normally code, this is just to keep in sync. All of this is invisible from the end user so its ease of use
            // when deployed will remain the same

            // Due to the intensive processing power required for reflection this will run slower than if it was done strongly typed
            // However due to the very small work load it will not represent any visible slow downs.


            var d1 = Type.GetType("Aweber.Factory.Base`1"); // GenericTest was my namespace, add yours 
            Type[] typeArgs = { this.GetType() };

            var makeme = d1.MakeGenericType(typeArgs);
            object factory = Activator.CreateInstance(makeme);


            Object instance = factory.GetType().GetMethod("Build").Invoke(factory, new object[] { JSON.Read(api.GetResponse(url)), api });

            SortedList<String, String> values = new SortedList<string, string>();

            // Loop through each property in instance and match to this property
            foreach (PropertyInfo info in instance.GetType().GetProperties())
            {
                PropertyInfo currentProperty = this.GetType().GetProperty(info.Name);

                if (currentProperty != null)
                {
                    object value = info.GetValue(instance, null);

                    if (value != null)
                    {
                        values.Add(info.Name, Convert.ToString(value));
                    }
                }
            }

            load_values(values);

            clear_dirty();

            // Finished loading entity

        }


    }
}
