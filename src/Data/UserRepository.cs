using System;
using System.Text;
using System.Collections.Generic;
using System.Data;

using MySql.Data;
using MySql.Data.MySqlClient;

using LMS.Model;
using LMS.Model.Resource;
using LMS.Model.Composite;

namespace LMS.Data
{
    public class UserRepository : Repository<User>
    {
        public static string TypeName
        {
            get
            {
                return typeof(User).Name;
            }
        }

        public UserRepository()
            : base()
        {
        }

        public override User Add(User item)
        {
            Log.Info(String.Format("Add new resource '{0}'; Tenant={1}", TypeName, TenantIdentifier));

            try
            {
                Connection.Open();

                MySqlCommand command = (MySqlCommand)CreateCommand(true);

                command.CommandText = "INSERT INTO user(`tenant_id`,`uid`,`password`) VALUES(@tenant_id,@uid,md5(@password)); SELECT LAST_INSERT_ID();";
                command.Parameters.AddWithValue("@tenant_id", TenantIdentifier);
                command.Parameters.AddWithValue("@uid", item.Username);
                command.Parameters.AddWithValue("@password", item.Password);

                int id = Convert.ToInt32(command.ExecuteScalar());
                if (id > 0)
                    item.Id = id;

                Transaction.Commit();
            }
            catch (Exception e)
            {
                if (Transaction != null)
                    Transaction.Rollback();

                Log.Error(e);

                throw;
            }
            finally
            {
                Connection.Close();
            }

            return item;
        }

        public override List<User> Find(User query)
        {
            Log.Info(String.Format("Query for resource '{0}'; Tenant={1}", TypeName, TenantIdentifier));

            List<User> result = new List<User>();

            try
            {
                Connection.Open();

                MySqlCommand command = (MySqlCommand)CreateCommand(false);

                if (query == null)
                {
                    command.CommandText = "SELECT * FROM user WHERE `tenant_id`=@tenant_id;";
                    command.Parameters.AddWithValue("@tenant_id", TenantIdentifier);
                }
                else
                {
                    Dictionary<string, object> parameters = new Dictionary<string, object>();
                    StringBuilder sql = new StringBuilder();
                    sql.Append("SELECT * FROM user WHERE `tenant_id`=@tenant_id");

                    // Dynamically build sql statement and list of parameters
                    if (query.Username != null && query.Password != null)
                    {
                        sql.Append(" AND `uid`=@uid");
                        parameters.Add("@uid", query.Username);

                        sql.Append(" AND `password`=md5(@password)");
                        parameters.Add("@password", query.Password);
                    }

                    sql.Append(";");

                    command.CommandText = sql.ToString();
                    command.Parameters.AddWithValue("@tenant_id", TenantIdentifier);

                    // Add parameters to sql command
                    foreach (KeyValuePair<string, object> parameter in parameters)
                    {
                        command.Parameters.AddWithValue(parameter.Key, parameter.Value);
                    }
                }

                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        User item = new User()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Username = reader.GetString(reader.GetOrdinal("uid"))
                        };                        

                        Log.Debug(String.Format("User.Id={0} found", item.Id));

                        result.Add(item);
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

            return result;
        }

        public override User Get(int id)
        {
            Log.Info(String.Format("Get resource '{0}'; Tenant={1}; Id={2}", TypeName, TenantIdentifier, id));
            throw new NotImplementedException();
        }

        public override void Remove(int id)
        {
            Log.Info(String.Format("Remove resource '{0}'; Tenant={1}; Id={2}", TypeName, TenantIdentifier, id));

            try
            {
                Connection.Open();

                MySqlCommand command = (MySqlCommand)CreateCommand(true);

                command.CommandText = "DELETE FROM user WHERE `tenant_id`=@tenant_id AND `id`=@id;";
                command.Parameters.AddWithValue("@tenant_id", TenantIdentifier);
                command.Parameters.AddWithValue("@id", id);
                command.ExecuteNonQuery();

                Transaction.Commit();
            }
            catch (Exception e)
            {
                if (Transaction != null)
                    Transaction.Rollback();

                Log.Error(e);

                throw;
            }
            finally
            {
                Connection.Close();
            }
        }

        public override User Update(User item)
        {
            Log.Info(String.Format("Update resource '{0}'; Tenant={1}; Id={2}", TypeName, TenantIdentifier, item.Id));
            throw new NotImplementedException();
        }
    }
}
