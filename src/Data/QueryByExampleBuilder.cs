using System;
using System.Collections.Generic;
using System.Reflection;

using LMS.Model;
using LMS.Model.Resource;

namespace LMS.Data
{
    public class QueryByExampleBuilder<T>
        where T : DomainResource
    {
        public QueryByExampleBuilder()
        {
        }

        public T GetQueryExample(string q)
        {
            Dictionary<string, string> queryParameters = GetQueryParameters(q);
            T query = default(T);

            if (queryParameters.Count > 0)
            {
                // Create instance of query by example object
                query = Activator.CreateInstance<T>();
                query.Id = 0;

                // Initialize the query by example object (always search active resources by default)
                PropertyInfo activeProp = typeof(T).GetProperty("Active");
                if (activeProp != null)
                    activeProp.SetValue(query, true, null);

                // Set property values based on query parameters
                foreach (KeyValuePair<string, string> qp in queryParameters)
                {
                    if (qp.Key.Contains("."))
                    {
                        // ?q=artist:reference.uri|...
                        string propertyName = ((qp.Key.Split(':').Length == 2) ? qp.Key.Split(':')[0] : null);
                        string typeName = ((propertyName != null) ? qp.Key.Replace(propertyName + ":", "").Split('.')[0] : null);
                        string nestedPropertyName = ((propertyName != null) ? qp.Key.Replace(propertyName + ":", "").Split('.')[1] : null);

                        // Nested object
                        if (propertyName != null && typeName != null && nestedPropertyName != null)
                        {
                            propertyName = char.ToUpper(propertyName[0]) + propertyName.Substring(1);
                            typeName = char.ToUpper(typeName[0]) + typeName.Substring(1);
                            nestedPropertyName = char.ToUpper(nestedPropertyName[0]) + nestedPropertyName.Substring(1);

                            Assembly assembly = Assembly.Load("LMS.Model");
                            Type[] types = assembly.GetTypes();
                            foreach (Type type in types)
                            {
                                if (type.Name != typeName)
                                    continue;

                                var obj = Activator.CreateInstance(type);

                                PropertyInfo prop = type.GetProperty(nestedPropertyName);
                                if (prop != null)
                                {
                                    if (prop.PropertyType.IsEnum)
                                        prop.SetValue(obj, Enum.Parse(prop.PropertyType, qp.Value), null);
                                    else
                                        prop.SetValue(obj, Convert.ChangeType(qp.Value, prop.PropertyType), null);
                                }
                                prop = typeof(T).GetProperty(propertyName);
                                if (prop != null)
                                    prop.SetValue(query, obj, null);
                            }
                        }
                    }
                    else
                    {
                        // Property
                        PropertyInfo prop = typeof(T).GetProperty(qp.Key);
                        if (prop != null)
                        {
                            if (prop.PropertyType.IsEnum)
                                prop.SetValue(query, Enum.Parse(prop.PropertyType, qp.Value), null);
                            else
                                prop.SetValue(query, Convert.ChangeType(qp.Value, prop.PropertyType), null);
                        }
                    }
                }
            }

            return query;
        }

        private Dictionary<string, string> GetQueryParameters(string q)
        {
            Dictionary<string, string> queryParameters = new Dictionary<string, string>();

            // Build query parameters based on query string, fx q=name|value;name|value
            string[] pairs = q.Split(';');
            foreach(string pair in pairs)
            {
                string[] p = pair.Split('|');
                if (p.Length != 2)
                    continue;

                queryParameters.Add(char.ToUpper(p[0][0]) + p[0].Substring(1), p[1]);
            }

            return queryParameters;
        }
    }
}