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
            // TODO: Implement support for nested objects, fx CompositeDataType

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
                    PropertyInfo prop = typeof(T).GetProperty(qp.Key);
                    if (prop != null)                  
                        prop.SetValue(query, Convert.ChangeType(qp.Value, prop.PropertyType), null);
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
                if (p.Length != 2 || p[0].Contains("."))
                    continue;

                queryParameters.Add(char.ToUpper(p[0][0]) + p[0].Substring(1), p[1]);
            }

            return queryParameters;
        }
    }
}