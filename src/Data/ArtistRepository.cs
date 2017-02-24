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
    public class ArtistRepository : Repository<Artist>
    {
        public static string TypeName
        {
            get
            {
                return typeof(Artist).Name;
            }
        }

        public ArtistRepository()
            : base()
        {
        }

        public override Artist Add(Artist item)
        {
            Log.Info(String.Format("Add new resource '{0}'; Tenant={1}", TypeName, TenantIdentifier));

            try
            {
                Connection.Open();

                MySqlCommand command = (MySqlCommand)CreateCommand(true);

                command.CommandText = "INSERT INTO artist(`tenant_id`,`stage_name`,`given_name`,`family_name`,`address`,`city`,`district`,`state`,`postalcode`,`country`,`email`,`telecom`) VALUES(@tenant_id,@stage_name,@given_name,@family_name,@address,@city,@district,@state,@postalcode,@country,@email,@telecom); SELECT LAST_INSERT_ID();";
                command.Parameters.AddWithValue("@tenant_id", TenantIdentifier);
                command.Parameters.AddWithValue("@stage_name", item.StageName);
                command.Parameters.AddWithValue("@given_name", string.Join(" ", item.Name.Given));
                command.Parameters.AddWithValue("@family_name", string.Join(" ", item.Name.Family));
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

        public override List<Artist> Find(Artist query)
        {
            Log.Info(String.Format("Query for resource '{0}'; Tenant={1}", TypeName, TenantIdentifier));

            List<Artist> result = new List<Artist>();

            try
            {
                Connection.Open();

                MySqlCommand command = (MySqlCommand)CreateCommand(false);

                if (query == null)
                {
                    command.CommandText = "SELECT * FROM artist WHERE `tenant_id`=@tenant_id;";
                    command.Parameters.AddWithValue("@tenant_id", TenantIdentifier);
                }
                else
                {
                    Dictionary<string, object> parameters = new Dictionary<string, object>();
                    StringBuilder sql = new StringBuilder();
                    sql.Append("SELECT * FROM artist WHERE `tenant_id`=@tenant_id");

                    // Dynamically build sql statement and list of parameters
                    if (query.StageName != null)
                    {
                        sql.Append(" AND `stage_name` LIKE @stage_name");
                        parameters.Add("@stage_name", "%" + query.StageName + "%");
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
                        Artist item = new Artist()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            StageName = reader.GetString(reader.GetOrdinal("stage_name")),
                            Email = reader.GetString(reader.GetOrdinal("email")),
                            Name = new HumanName(),
                            Address = new Address()
                        };

                        string givenName = reader.GetString(reader.GetOrdinal("given_name"));
                        string familyName = reader.GetString(reader.GetOrdinal("family_name"));

                        if (givenName != null)
                            item.Name.Given = givenName.Split(' ');
                        if (familyName != null)
                            item.Name.Family = familyName.Split(' ');

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

                        Log.Debug(String.Format("Artist.Id={0} found", item.Id));

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
                
        public override Artist Get(int id)
        {
            Log.Info(String.Format("Get resource '{0}'; Tenant={1}; Id={2}", TypeName, TenantIdentifier, id));

            Artist item = null;

            try
            {
                Connection.Open();

                MySqlCommand command = (MySqlCommand)CreateCommand(false);

                command.CommandText = "SELECT * FROM artist WHERE `tenant_id`=@tenant_id AND `id`=@id;";
                command.Parameters.AddWithValue("@tenant_id", TenantIdentifier);
                command.Parameters.AddWithValue("@id", id);

                using (IDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        item = new Artist()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            StageName = reader.GetString(reader.GetOrdinal("stage_name")),
                            Email = reader.GetString(reader.GetOrdinal("email")),
                            Name = new HumanName(),
                            Address = new Address()
                        };

                        string givenName = reader.GetString(reader.GetOrdinal("given_name"));
                        string familyName = reader.GetString(reader.GetOrdinal("family_name"));

                        if (givenName != null)
                            item.Name.Given = givenName.Split(' ');
                        if (familyName != null)
                            item.Name.Family = familyName.Split(' ');

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
            Log.Info(String.Format("Remove resource '{0}'; Tenant={1}; Id={2}", TypeName, TenantIdentifier, id));

            try
            {
                Connection.Open();

                MySqlCommand command = (MySqlCommand)CreateCommand(true);

                command.CommandText = "DELETE FROM artist WHERE `tenant_id`=@tenant_id AND `id`=@id;";
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

        public override Artist Update(Artist item)
        {
            Log.Info(String.Format("Update resource '{0}'; Tenant={1}; Id={2}", TypeName, TenantIdentifier, item.Id));

            try
            {
                Connection.Open();

                MySqlCommand command = (MySqlCommand)CreateCommand(true);

                command.CommandText = "UPDATE artist SET `stage_name`=@stage_name,`given_name`=@given_name,`family_name`=@family_name,`address`=@address,`city`=@city,`district`=@district,`state`=@state,`postalcode`=@postalcode,`country`=@country,`email`=@email,`telecom`=@telecom WHERE `tenant_id`=@tenant_id AND `id`=@id;";
                command.Parameters.AddWithValue("@tenant_id", TenantIdentifier);
                command.Parameters.AddWithValue("@id", item.Id);
                command.Parameters.AddWithValue("@stage_name", item.StageName);
                command.Parameters.AddWithValue("@given_name", string.Join(" ", item.Name.Given));
                command.Parameters.AddWithValue("@family_name", string.Join(" ", item.Name.Family));
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
