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
    public class LabelRepository : Repository<Label>
    {
        public static string TypeName
        {
            get
            {
                return typeof(Label).Name;
            }
        }

        public LabelRepository()
            : base()
        {
        }

        public override Label Add(Label item)
        {
            Log.Info(String.Format("Add new resource '{0}'; Tenant={1}", TypeName, TenantIdentifier));

            try
            {
                Connection.Open();

                MySqlCommand command = (MySqlCommand)CreateCommand(true);

                command.CommandText = "INSERT INTO label(`tenant_id`,`name`,`address`,`city`,`district`,`state`,`postalcode`,`country`,`email`,`telecom`) VALUES(@tenant_id,@name,@address,@city,@district,@state,@postalcode,@country,@email,@telecom); SELECT LAST_INSERT_ID();";
                command.Parameters.AddWithValue("@tenant_id", TenantIdentifier);
                command.Parameters.AddWithValue("@name", item.Name);
                command.Parameters.AddWithValue("@address", ((item.Address != null && item.Address.Line != null) ? string.Join(";", item.Address.Line) : null));
                command.Parameters.AddWithValue("@city", ((item.Address != null) ? item.Address.City : null));
                command.Parameters.AddWithValue("@district", ((item.Address != null) ? item.Address.District : null));
                command.Parameters.AddWithValue("@state", ((item.Address != null) ? item.Address.State : null));
                command.Parameters.AddWithValue("@postalcode", ((item.Address != null) ? item.Address.PostalCode : null));
                command.Parameters.AddWithValue("@country", ((item.Address != null) ? item.Address.Country : null));
                command.Parameters.AddWithValue("@email", item.Email);
                command.Parameters.AddWithValue("@telecom", item.Telecom);

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

        public override List<Label> Find(Label query)
        {
            Log.Info(String.Format("Query for resource '{0}'; Tenant={1}", TypeName, TenantIdentifier));

            List<Label> result = new List<Label>();

            try
            {
                Connection.Open();

                MySqlCommand command = (MySqlCommand)CreateCommand(false);

                if (query == null)
                {
                    command.CommandText = "SELECT * FROM label WHERE `tenant_id`=@tenant_id;";
                    command.Parameters.AddWithValue("@tenant_id", TenantIdentifier);
                }
                else
                {
                    Dictionary<string, object> parameters = new Dictionary<string, object>();
                    StringBuilder sql = new StringBuilder();
                    sql.Append("SELECT * FROM label WHERE `tenant_id`=@tenant_id");

                    // Dynamically build sql statement and list of parameters
                    if (query.Name != null)
                    {
                        sql.Append(" AND `name` LIKE @name");
                        parameters.Add("@name", "%" + query.Name + "%");
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
                        Label item = new Label()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Name = reader.GetString(reader.GetOrdinal("name")),
                            Email = reader.GetString(reader.GetOrdinal("email")),
                            Address = new Address()
                        };

                        if (!reader.IsDBNull(reader.GetOrdinal("address")))
                        {
                            string address = reader.GetString(reader.GetOrdinal("address"));
                            if (address != null)
                                item.Address.Line = address.Split(';');
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("postalcode")))
                            item.Address.PostalCode = reader.GetString(reader.GetOrdinal("postalcode"));
                        if (!reader.IsDBNull(reader.GetOrdinal("city")))
                            item.Address.City = reader.GetString(reader.GetOrdinal("city"));
                        if (!reader.IsDBNull(reader.GetOrdinal("district")))
                            item.Address.District = reader.GetString(reader.GetOrdinal("district"));
                        if (!reader.IsDBNull(reader.GetOrdinal("state")))
                            item.Address.State = reader.GetString(reader.GetOrdinal("state"));
                        if (!reader.IsDBNull(reader.GetOrdinal("country")))
                            item.Address.Country = reader.GetString(reader.GetOrdinal("country"));
                        if (!reader.IsDBNull(reader.GetOrdinal("telecom")))
                            item.Address.Country = reader.GetString(reader.GetOrdinal("telecom"));

                        Log.Debug(String.Format("Label.Id={0} found", item.Id));

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

        public override Label Get(int id)
        {
            Log.Info(String.Format("Get resource '{0}'; Tenant={1}; Id={2}", TypeName, TenantIdentifier, id));

            Label item = null;

            try
            {
                Connection.Open();

                MySqlCommand command = (MySqlCommand)CreateCommand(false);

                command.CommandText = "SELECT * FROM label WHERE `tenant_id`=@tenant_id AND `id`=@id;";
                command.Parameters.AddWithValue("@tenant_id", TenantIdentifier);
                command.Parameters.AddWithValue("@id", id);

                using (IDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        item = new Label()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Name = reader.GetString(reader.GetOrdinal("name")),
                            Email = reader.GetString(reader.GetOrdinal("email")),
                            Address = new Address()
                        };

                        if (!reader.IsDBNull(reader.GetOrdinal("address")))
                        {
                            string address = reader.GetString(reader.GetOrdinal("address"));
                            if (address != null)
                                item.Address.Line = address.Split(';');
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("postalcode")))
                            item.Address.PostalCode = reader.GetString(reader.GetOrdinal("postalcode"));
                        if (!reader.IsDBNull(reader.GetOrdinal("city")))
                            item.Address.City = reader.GetString(reader.GetOrdinal("city"));
                        if (!reader.IsDBNull(reader.GetOrdinal("district")))
                            item.Address.District = reader.GetString(reader.GetOrdinal("district"));
                        if (!reader.IsDBNull(reader.GetOrdinal("state")))
                            item.Address.State = reader.GetString(reader.GetOrdinal("state"));
                        if (!reader.IsDBNull(reader.GetOrdinal("country")))
                            item.Address.Country = reader.GetString(reader.GetOrdinal("country"));

                        if (!reader.IsDBNull(reader.GetOrdinal("telecom")))
                            item.Telecom = reader.GetString(reader.GetOrdinal("telecom"));

                        Log.Debug(String.Format("Label.Id={0} found", item.Id));
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
            Log.Info(String.Format("Remove resource '{0}'; Tenant={1}; Id={2}", TypeName, TenantIdentifier, id));

            try
            {
                Connection.Open();

                MySqlCommand command = (MySqlCommand)CreateCommand(true);

                command.CommandText = "DELETE FROM label WHERE `tenant_id`=@tenant_id AND `id`=@id;";
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

        public override Label Update(Label item)
        {
            Log.Info(String.Format("Update resource '{0}'; Tenant={1}; Id={2}", TypeName, TenantIdentifier, item.Id));

            try
            {
                Connection.Open();

                MySqlCommand command = (MySqlCommand)CreateCommand(true);

                command.CommandText = "UPDATE label SET `name`=@name,`address`=@address,`city`=@city,`district`=@district,`state`=@state,`postalcode`=@postalcode,`country`=@country,`email`=@email,`telecom`=@telecom WHERE `tenant_id`=@tenant_id AND `id`=@id;";
                command.Parameters.AddWithValue("@tenant_id", TenantIdentifier);
                command.Parameters.AddWithValue("@id", item.Id);
                command.Parameters.AddWithValue("@name", item.Name);
                command.Parameters.AddWithValue("@address", ((item.Address != null && item.Address.Line != null) ? string.Join(";", item.Address.Line) : null));
                command.Parameters.AddWithValue("@city", ((item.Address != null) ? item.Address.City : null));
                command.Parameters.AddWithValue("@district", ((item.Address != null) ? item.Address.District : null));
                command.Parameters.AddWithValue("@state", ((item.Address != null) ? item.Address.State : null));
                command.Parameters.AddWithValue("@postalcode", ((item.Address != null) ? item.Address.PostalCode : null));
                command.Parameters.AddWithValue("@country", ((item.Address != null) ? item.Address.Country : null));
                command.Parameters.AddWithValue("@email", item.Email);
                command.Parameters.AddWithValue("@telecom", item.Telecom);
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

            return item;
        }
    }
}