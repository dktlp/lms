using System;
using System.Text;
using System.Collections.Generic;
using System.Data;
using System.IO;

using MySql.Data;
using MySql.Data.MySqlClient;

using LMS.Model;
using LMS.Model.Resource;
using LMS.Model.Composite;
using LMS.Model.Resource.Enums;

using log4net;

using Newtonsoft.Json;

namespace LMS.Data
{
    public class StatementRepository : Repository<Statement>
    {
        public static string TypeName
        {
            get
            {
                return typeof(Statement).Name;
            }
        }

        public StatementRepository()
            : base()
        {
        }

        public override Statement Add(Statement item)
        {
            Log.Info(String.Format("Add new resource '{0}'; Tenant={1}", TypeName, TenantIdentifier));
            
            try
            {
                Connection.Open();

                MySqlCommand command = (MySqlCommand)CreateCommand(true);

                // Create Statement resource in database.
                command.CommandText = "INSERT INTO statement(`tenant_id`,`label_id`,`artist_id`,`quarter`,`reference`,`amount`,`status`) VALUES(@tenant_id,@label_id,@artist_id,@quarter,@reference,@amount,@status); SELECT LAST_INSERT_ID();";
                command.Parameters.AddWithValue("@tenant_id", TenantIdentifier);
                command.Parameters.AddWithValue("@label_id", item.Label.GetId());
                command.Parameters.AddWithValue("@artist_id", item.Artist.GetId());
                command.Parameters.AddWithValue("@quarter", item.Quarter);
                command.Parameters.AddWithValue("@reference", item.Reference);
                command.Parameters.AddWithValue("@amount", item.Amount);
                command.Parameters.AddWithValue("@status", item.Status);

                int id = Convert.ToInt32(command.ExecuteScalar());
                if (id > 0)
                    item.Id = id;

                // Update Transactions in Statement.Data with reference to newly created statement.
                foreach(Account account in item.Data)
                {
                    foreach(Transaction transaction in account.Transactions)
                    {
                        transaction.Statement = new Reference(Reference.StatementUri, item.Id);

                        // Update Transaction resource with Statement identifier.
                        command.CommandText = "UPDATE transaction SET `statement_id`=@statement_id WHERE `tenant_id`=@tenant_id AND `id`=@id;";
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@tenant_id", TenantIdentifier);
                        command.Parameters.AddWithValue("@id", transaction.Id);
                        command.Parameters.AddWithValue("@statement_id", transaction.Statement.GetId());
                        command.ExecuteNonQuery();
                    }
                }

                // Serialize Statement.Data to JSON.
                string data = null;
                if (item.Data != null)
                {
                    JsonSerializer serializer = new JsonSerializer();
                    using (StringWriter json = new StringWriter())
                    {
                        using (JsonWriter jsonWriter = new JsonTextWriter(json))
                            serializer.Serialize(jsonWriter, item.Data);

                        data = json.GetStringBuilder().ToString();
                    }

                    Log.Debug("Statement.Data serialized as JSON");
                }

                // Update Statement resource in database with Statement.Data as JSON.
                command.CommandText = "UPDATE statement SET `data`=@data WHERE `tenant_id`=@tenant_id AND `id`=@id;";
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@tenant_id", TenantIdentifier);
                command.Parameters.AddWithValue("@id", item.Id);
                command.Parameters.AddWithValue("@data", data);
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

        public override List<Statement> Find(Statement query)
        {
            Log.Info(String.Format("Query for resource '{0}'; Tenant={1}", TypeName, TenantIdentifier));

            List<Statement> result = new List<Statement>();

            try
            {
                Connection.Open();

                MySqlCommand command = (MySqlCommand)CreateCommand(false);

                if (query == null)
                {
                    command.CommandText = "SELECT * FROM statement WHERE `tenant_id`=@tenant_id;";
                    command.Parameters.AddWithValue("@tenant_id", TenantIdentifier);
                }
                else
                {
                    Dictionary<string, object> parameters = new Dictionary<string, object>();
                    StringBuilder sql = new StringBuilder();
                    sql.Append("SELECT * FROM statement WHERE `tenant_id`=@tenant_id");

                    // Dynamically build sql statement and list of parameters
                    if (query.Status > 0)
                    {
                        sql.Append(" AND `status`=@status");
                        parameters.Add("@status", query.Status);
                    }
                    if (query.Quarter != null)
                    {
                        sql.Append(" AND `quarter` LIKE @quarter");
                        parameters.Add("@quarter", "%" + query.Quarter + "%");
                    }
                    if (query.Reference != null)
                    {
                        sql.Append(" AND `reference` LIKE @reference");
                        parameters.Add("@reference", "%" + query.Reference + "%");
                    }
                    if (query.Artist != null)
                    {
                        sql.Append(" AND `artist_id`=@artist_id");
                        parameters.Add("@artist_id", query.Artist.GetId());
                    }
                    if (query.Label != null)
                    {
                        sql.Append(" AND `label_id`=@label_id");
                        parameters.Add("@label_id", query.Label.GetId());
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
                        Statement item = new Statement()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Amount = reader.GetDouble(reader.GetOrdinal("amount")),
                            Label = new Reference(Reference.LabelUri, reader.GetInt32(reader.GetOrdinal("label_id"))),
                            Artist = new Reference(Reference.ArtistUri, reader.GetInt32(reader.GetOrdinal("artist_id"))),
                            EffectiveTime = reader.GetDateTime(reader.GetOrdinal("effective_time")),
                            Quarter = reader.GetString(reader.GetOrdinal("quarter")),
                            Reference = reader.GetString(reader.GetOrdinal("reference")),
                            Status = (StatementStatus)reader.GetByte(reader.GetOrdinal("status"))
                        };

                        if (!reader.IsDBNull(reader.GetOrdinal("invoice_id")))
                            item.Invoice = new Reference(Reference.InvoiceUri, reader.GetInt32(reader.GetOrdinal("invoice_id")));

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

        public override Statement Get(int id)
        {
            Log.Info(String.Format("Get resource '{0}'; Tenant={1}; Id={2}", TypeName, TenantIdentifier, id));

            Statement item = null;

            try
            {
                Connection.Open();

                MySqlCommand command = (MySqlCommand)CreateCommand(false);

                command.CommandText = "SELECT * FROM statement WHERE `tenant_id`=@tenant_id AND `id`=@id;";
                command.Parameters.AddWithValue("@tenant_id", TenantIdentifier);
                command.Parameters.AddWithValue("@id", id);

                using (IDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        item = new Statement()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Amount = reader.GetDouble(reader.GetOrdinal("amount")),
                            Label = new Reference(Reference.LabelUri, reader.GetInt32(reader.GetOrdinal("label_id"))),
                            Artist = new Reference(Reference.ArtistUri, reader.GetInt32(reader.GetOrdinal("artist_id"))),
                            EffectiveTime = reader.GetDateTime(reader.GetOrdinal("effective_time")),
                            Quarter = reader.GetString(reader.GetOrdinal("quarter")),
                            Reference = reader.GetString(reader.GetOrdinal("reference")),
                            Status = (StatementStatus)reader.GetByte(reader.GetOrdinal("status"))
                        };

                        if (!reader.IsDBNull(reader.GetOrdinal("invoice_id")))
                            item.Invoice = new Reference(Reference.InvoiceUri, reader.GetInt32(reader.GetOrdinal("invoice_id")));

                        // Read and deserialize Statement.Data which is formatted as JSON.
                        if (!reader.IsDBNull(reader.GetOrdinal("data")))
                        {
                            string data = reader.GetString(reader.GetOrdinal("data"));

                            JsonSerializer serializer = new JsonSerializer();
                            using (StringReader json = new StringReader(data))
                            {
                                using (JsonReader jsonReader = new JsonTextReader(json))
                                    item.Data = serializer.Deserialize<Account[]>(jsonReader);
                            }

                            Log.Debug("Statement.Data deserialized from JSON");
                        }

                        Log.Debug(String.Format("Statement.Id={0} found", item.Id));
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

                command.CommandText = "DELETE FROM statement WHERE `tenant_id`=@tenant_id AND `id`=@id;";
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

        public override Statement Update(Statement item)
        {
            Log.Info(String.Format("Update resource '{0}'; Tenant={1}; Id={2}", TypeName, TenantIdentifier, item.Id));

            try
            {
                Connection.Open();

                MySqlCommand command = (MySqlCommand)CreateCommand(true);

                command.CommandText = "UPDATE statement SET `status`=@status WHERE `tenant_id`=@tenant_id AND `id`=@id;";
                command.Parameters.AddWithValue("@tenant_id", TenantIdentifier);
                command.Parameters.AddWithValue("@id", item.Id);
                command.Parameters.AddWithValue("@status", item.Status);
                command.ExecuteNonQuery();

                // If Statement.Status='cancalled', then break link between Statement resource and Transaction resources
                if (item.Status == StatementStatus.Cancelled)
                {
                    command.CommandText = "UPDATE transaction SET `statement_id`=NULL WHERE `tenant_id`=@tenant_id AND `statement_id`=@statement_id;";
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@tenant_id", TenantIdentifier);
                    command.Parameters.AddWithValue("@statement_id", item.Id);
                    command.ExecuteNonQuery();
                }

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