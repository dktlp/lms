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
            //Log.Info(String.Format("Query for resource '{0}'; Merchant={1}", TypeName, MerchantIdentifier));

            //List<Category> result = new List<Category>();

            //try
            //{
            //    Connection.Open();

            //    MySqlCommand command = (MySqlCommand)CreateCommand(false);

            //    if (query == null || (query != null && query.PartOf != null))
            //    {
            //        command.CommandText = "SELECT * FROM category WHERE `merchant_id`=@merchant_id AND `parent_id`=@parent_id;";
            //        command.Parameters.AddWithValue("@merchant_id", MerchantIdentifier);
            //        command.Parameters.AddWithValue("@parent_id", ((query != null && query.PartOf != null) ? query.PartOf.Id : 0));
            //    }
            //    else
            //    {
            //        Dictionary<string, object> parameters = new Dictionary<string, object>();
            //        StringBuilder sql = new StringBuilder();
            //        sql.Append("SELECT * FROM category WHERE `merchant_id`=@merchant_id");

            //        // Dynamically build sql statement and list of parameters
            //        if (query.Name != null)
            //        {
            //            sql.Append(" AND `name` LIKE @name");
            //            parameters.Add("@name", "%" + query.Name + "%");
            //        }

            //        sql.Append(" AND `active`=@active");
            //        parameters.Add("@active", query.Active);                    

            //        sql.Append(";");

            //        command.CommandText = sql.ToString();
            //        command.Parameters.AddWithValue("@merchant_id", MerchantIdentifier);

            //        // Add parameters to sql command
            //        foreach (KeyValuePair<string, object> parameter in parameters)
            //        {
            //            command.Parameters.AddWithValue(parameter.Key, parameter.Value);
            //        }
            //    }

            //    using (IDataReader reader = command.ExecuteReader())
            //    {
            //        while (reader.Read())
            //        {
            //            Category item = new Category()
            //            {
            //                Id = reader.GetInt32(reader.GetOrdinal("id")),
            //                Name = reader.GetString(reader.GetOrdinal("name")),
            //                Active = reader.GetBoolean(reader.GetOrdinal("active")),
            //                Period = null,
            //                PartOf = null,
            //                Subcategories = null
            //            };

            //            if (!reader.IsDBNull(reader.GetOrdinal("period_start")) && !reader.IsDBNull(reader.GetOrdinal("period_end")))
            //            {
            //                item.Period = new Period()
            //                {
            //                    Start = reader.GetDateTime(reader.GetOrdinal("period_start")),
            //                    End = reader.GetDateTime(reader.GetOrdinal("period_end"))
            //                };
            //            }

            //            Log.Debug(String.Format("Category.Id={0} found", item.Id));

            //            result.Add(item);
            //        }
            //    }                
            //}
            //catch (Exception e)
            //{
            //    Log.Error(e);

            //    throw;
            //}
            //finally
            //{
            //    Connection.Close();
            //}

            //// Load all subcategories recursively
            //foreach (Category category in result)
            //{
            //    Log.Debug(String.Format("Load subcategories for Category.Id={0}", category.Id));

            //    // Get all children and add to subcategories on parent.
            //    List<Category> subcategories = Find(new Category() { PartOf = new Category() { Id = category.Id } });
            //    if (subcategories != null && subcategories.Count > 0)
            //    {
            //        category.Subcategories = subcategories.ToArray();

            //        // Set partOf of child to parent
            //        foreach (Category subcategory in category.Subcategories)
            //        {
            //            subcategory.PartOf = new Category()
            //            {
            //                Id = category.Id,
            //                Name = category.Name,
            //                Active = category.Active,
            //                Period = category.Period
            //            };
            //        }
            //    }
            //}

            //return result;

            throw new NotImplementedException();
        }
                
        public override Artist Get(int id)
        {
            //Log.Info(String.Format("Get resource '{0}'; Category.Id={1}, Merchant={2}", TypeName, id, MerchantIdentifier));

            //Category item = null;

            //try
            //{
            //    Connection.Open();

            //    MySqlCommand command = (MySqlCommand)CreateCommand(false);

            //    command.CommandText = "SELECT * FROM category WHERE `merchant_id`=@merchant_id AND `id`=@id;";
            //    command.Parameters.AddWithValue("@merchant_id", MerchantIdentifier);
            //    command.Parameters.AddWithValue("@id", id);

            //    using (IDataReader reader = command.ExecuteReader())
            //    {
            //        if (reader.Read())
            //        {
            //            item = new Category()
            //            {
            //                Id = reader.GetInt32(reader.GetOrdinal("id")),
            //                Name = reader.GetString(reader.GetOrdinal("name")),
            //                Active = reader.GetBoolean(reader.GetOrdinal("active")),
            //                Period = null,
            //                PartOf = null,
            //                Subcategories = null
            //            };

            //            if (!reader.IsDBNull(reader.GetOrdinal("period_start")) && !reader.IsDBNull(reader.GetOrdinal("period_end")))
            //            {
            //                item.Period = new Period()
            //                {
            //                    Start = reader.GetDateTime(reader.GetOrdinal("period_start")),
            //                    End = reader.GetDateTime(reader.GetOrdinal("period_end"))
            //                };
            //            }

            //            if (reader.GetInt32(reader.GetOrdinal("parent_id")) > 0)
            //            {
            //                item.PartOf = new Category()
            //                {
            //                    Id = reader.GetInt32(reader.GetOrdinal("parent_id"))
            //                };
            //            }

            //            Log.Debug(String.Format("Category.Id={0} found", item.Id));
            //        }
            //    }
            //}
            //catch (Exception e)
            //{
            //    Log.Error(e);

            //    throw;
            //}
            //finally
            //{
            //    Connection.Close();
            //}

            //// Load subcategories
            //if (item != null)
            //{
            //    Log.Debug(String.Format("Load subcategories for Category.Id={0}", item.Id));

            //    // Get all children and add to subcategories on parent.
            //    List<Category> subcategories = Find(new Category() { PartOf = new Category() { Id = item.Id } });
            //    if (subcategories != null && subcategories.Count > 0)
            //    {
            //        item.Subcategories = subcategories.ToArray();

            //        // Set partOf of child to parent
            //        foreach (Category subcategory in item.Subcategories)
            //        {
            //            subcategory.PartOf = new Category()
            //            {
            //                Id = item.Id,
            //                Name = item.Name,
            //                Active = item.Active,
            //                Period = item.Period
            //            };
            //        }
            //    }

            //    // Get parent and set partOf
            //    if (item.PartOf != null)
            //    {
            //        Category parent = Get(item.PartOf.Id);
            //        if (parent != null)
            //        {
            //            item.PartOf.Name = parent.Name;
            //            item.Active = parent.Active;
            //            item.Period = parent.Period;
            //        }
            //    }
            //}

            //return item;

            throw new NotImplementedException();
        }

        public override void Remove(int id)
        {
            //Log.Info(String.Format("Remove resource '{0}'; Category.Id={1}, Merchant={2}", TypeName, id, MerchantIdentifier));

            //try
            //{
            //    Connection.Open();

            //    MySqlCommand command = (MySqlCommand)CreateCommand(true);

            //    command.CommandText = "DELETE FROM category WHERE `merchant_id`=@merchant_id AND `id`=@id;";
            //    command.Parameters.AddWithValue("@merchant_id", MerchantIdentifier);
            //    command.Parameters.AddWithValue("@id", id);
            //    command.ExecuteNonQuery();

            //    Transaction.Commit();
            //}
            //catch (Exception e)
            //{
            //    if (Transaction != null)
            //        Transaction.Rollback();

            //    Log.Error(e);

            //    throw;
            //}
            //finally
            //{
            //    Connection.Close();
            //}

            throw new NotImplementedException();
        }

        public override Artist Update(Artist item)
        {
            //Log.Info(String.Format("Update resource '{0}'; Category.Id={1}, Category.PartOf={2}, Merchant={3}", TypeName, item.Id, ((item.PartOf != null) ? item.PartOf.Id : 0), MerchantIdentifier));

            //try
            //{
            //    Connection.Open();

            //    MySqlCommand command = (MySqlCommand)CreateCommand(true);

            //    command.CommandText = "UPDATE category SET `parent_id`=@parent_id,`name`=@name,`active`=@active,`period_start`=@period_start,`period_end`=@period_end WHERE `merchant_id`=@merchant_id AND `id`=@id";
            //    command.Parameters.AddWithValue("@merchant_id", MerchantIdentifier);
            //    command.Parameters.AddWithValue("@id", item.Id);
            //    command.Parameters.AddWithValue("@parent_id", ((item.PartOf != null) ? item.PartOf.Id : 0));
            //    command.Parameters.AddWithValue("@name", item.Name);
            //    command.Parameters.AddWithValue("@active", item.Active);
            //    command.Parameters.AddWithValue("@period_start", ((item.Period != null) ? item.Period.Start : null));
            //    command.Parameters.AddWithValue("@period_end", ((item.Period != null) ? item.Period.End : null));
            //    command.ExecuteNonQuery();                

            //    Transaction.Commit();
            //}
            //catch (Exception e)
            //{
            //    if (Transaction != null)
            //        Transaction.Rollback();

            //    Log.Error(e);

            //    throw;
            //}
            //finally
            //{
            //    Connection.Close();
            //}

            //return item;

            throw new NotImplementedException();
        }
    }
}
