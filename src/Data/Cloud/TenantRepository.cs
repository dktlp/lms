using System;
using System.Text;
using System.Collections.Generic;
using System.Data;

using MySql.Data;
using MySql.Data.MySqlClient;

namespace LMS.Data.Cloud
{
    public class TenantRepository : Repository<Tenant>
    {
        public static string TypeName
        {
            get
            {
                return typeof(Tenant).Name;
            }
        }

        public TenantRepository()
            : base()
        {
        }

        public override Tenant Add(Tenant item)
        {
            Log.Info(String.Format("Add new resource '{0}'", TypeName));
            throw new NotImplementedException();
        }

        public override List<Tenant> Find(Tenant query)
        {
            Log.Info(String.Format("Query for resource '{0}'", TypeName));
            throw new NotImplementedException();
        }

        public override Tenant Get(int id)
        {
            Log.Info(String.Format("Get resource '{0}'; Id={1}", TypeName, id));

            Tenant item = null;

            try
            {
                Connection.Open();

                MySqlCommand command = (MySqlCommand)CreateCommand(false);

                command.CommandText = "SELECT * FROM tenant WHERE `id`=@id;";
                command.Parameters.AddWithValue("@id", id);

                using (IDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        item = new Tenant()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id"))
                        };
                        
                        Log.Debug(String.Format("Tenant.Id={0} found", item.Id));
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e);

                throw;
            }
            finally
            {
                Connection.Close();
            }

            return item;
        }

        public override void Remove(int id)
        {
            Log.Info(String.Format("Remove resource '{0}'; Id={1}", TypeName, id));
            throw new NotImplementedException();
        }

        public override Tenant Update(Tenant item)
        {
            Log.Info(String.Format("Update resource '{0}'; Id={1}", TypeName, item.Id));
            throw new NotImplementedException();
        }
    }
}