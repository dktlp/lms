using System;
using System.Text;
using System.Collections.Generic;
using System.Data;

using MySql.Data;
using MySql.Data.MySqlClient;

using LMS.Model;
using LMS.Model.Resource;
using LMS.Model.Composite;
using LMS.Model.Resource.Enums;

using log4net;

namespace LMS.Data
{
    public class TransactionRepository : Repository<Transaction>
    {
        public static string TypeName
        {
            get
            {
                return typeof(Transaction).Name;
            }
        }

        public TransactionRepository()
            : base()
        {
        }

        public override Transaction Add(Transaction item)
        {
            Log.Info(String.Format("Add new resource '{0}'; Tenant={1}", TypeName, TenantIdentifier));

            try
            {
                Connection.Open();

                MySqlCommand command = (MySqlCommand)CreateCommand(true);

                command.CommandText = "INSERT INTO transaction(`tenant_id`,`account_id`,`statement_id`,`type`,`status`,`amount`,`quarter`,`description`) VALUES(@tenant_id,@account_id,@statement_id,@type,@status,@amount,@quarter,@description); SELECT LAST_INSERT_ID();";
                command.Parameters.AddWithValue("@tenant_id", TenantIdentifier);
                command.Parameters.AddWithValue("@account_id", item.Account.GetId());                
                command.Parameters.AddWithValue("@type", item.Type);
                command.Parameters.AddWithValue("@status", item.Status);
                command.Parameters.AddWithValue("@amount", item.Amount);
                command.Parameters.AddWithValue("@quarter", item.Quarter);
                command.Parameters.AddWithValue("@description", item.Description);

                if (item.Statement != null)
                    command.Parameters.AddWithValue("@statement_id", item.Statement.GetId());
                else
                    command.Parameters.AddWithValue("@statement_id", null);

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

        public override List<Transaction> Find(Transaction query)
        {
            Log.Info(String.Format("Query for resource '{0}'; Tenant={1}", TypeName, TenantIdentifier));

            List<Transaction> result = new List<Transaction>();

            try
            {
                Connection.Open();

                MySqlCommand command = (MySqlCommand)CreateCommand(false);

                if (query == null)
                {
                    command.CommandText = "SELECT * FROM transaction WHERE `tenant_id`=@tenant_id;";
                    command.Parameters.AddWithValue("@tenant_id", TenantIdentifier);
                }
                else
                {
                    Dictionary<string, object> parameters = new Dictionary<string, object>();
                    StringBuilder sql = new StringBuilder();
                    sql.Append("SELECT * FROM transaction WHERE `tenant_id`=@tenant_id");

                    // Dynamically build sql statement and list of parameters
                    if (query.Account != null)
                    {
                        sql.Append(" AND `account_id`=@account_id");
                        parameters.Add("@account_id", query.Account.GetId());
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
                        Transaction item = new Transaction()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Account = new Reference(Reference.AccountUri, reader.GetInt32(reader.GetOrdinal("account_id"))),
                            EffectiveTime = reader.GetDateTime(reader.GetOrdinal("effective_time")),
                            Status = (TransactionStatus)reader.GetByte(reader.GetOrdinal("status")),
                            Type = (TransactionType)reader.GetByte(reader.GetOrdinal("Type")),
                            Amount = reader.GetDouble(reader.GetOrdinal("amount")),
                            Quarter = reader.GetString(reader.GetOrdinal("quarter"))
                        };

                        if (!reader.IsDBNull(reader.GetOrdinal("description")))
                            item.Description = reader.GetString(reader.GetOrdinal("description"));
                        if (!reader.IsDBNull(reader.GetOrdinal("statement_id")))
                            item.Statement = new Reference(Reference.StatementUri, reader.GetInt32(reader.GetOrdinal("statement_id")));

                        Log.Debug(String.Format("Transaction.Id={0} found", item.Id));

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

        public override Transaction Get(int id)
        {
            Log.Info(String.Format("Get resource '{0}'; Tenant={1}; Id={2}", TypeName, TenantIdentifier, id));

            Transaction item = null;

            try
            {
                Connection.Open();

                MySqlCommand command = (MySqlCommand)CreateCommand(false);

                command.CommandText = "SELECT * FROM transaction WHERE `tenant_id`=@tenant_id AND `id`=@id";
                command.Parameters.AddWithValue("@tenant_id", TenantIdentifier);
                command.Parameters.AddWithValue("@id", id);

                using (IDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        item = new Transaction()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Account = new Reference(Reference.AccountUri, reader.GetInt32(reader.GetOrdinal("account_id"))),
                            EffectiveTime = reader.GetDateTime(reader.GetOrdinal("effective_time")),
                            Status = (TransactionStatus)reader.GetByte(reader.GetOrdinal("status")),
                            Type = (TransactionType)reader.GetByte(reader.GetOrdinal("Type")),
                            Amount = reader.GetDouble(reader.GetOrdinal("amount")),
                            Quarter = reader.GetString(reader.GetOrdinal("quarter"))
                        };

                        if (!reader.IsDBNull(reader.GetOrdinal("description")))
                            item.Description = reader.GetString(reader.GetOrdinal("description"));
                        if (!reader.IsDBNull(reader.GetOrdinal("statement_id")))
                            item.Statement = new Reference(Reference.StatementUri, reader.GetInt32(reader.GetOrdinal("statement_id")));

                        Log.Debug(String.Format("Transaction.Id={0} found", item.Id));
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

                command.CommandText = "DELETE FROM transaction WHERE `tenant_id`=@tenant_id AND `id`=@id;";
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

        public override Transaction Update(Transaction item)
        {
            Log.Info(String.Format("Update resource '{0}'; Tenant={1}", TypeName, TenantIdentifier));

            try
            {
                Connection.Open();

                MySqlCommand command = (MySqlCommand)CreateCommand(true);

                command.CommandText = "UPDATE transaction SET `statement_id`=@statement_id,`status`=@status WHERE `tenant_id`=@tenant_id AND `id`=@id;";
                command.Parameters.AddWithValue("@tenant_id", TenantIdentifier);
                command.Parameters.AddWithValue("@id", item.Id);
                command.Parameters.AddWithValue("@status", item.Status);

                if (item.Statement != null)
                    command.Parameters.AddWithValue("@statement_id", item.Statement.GetId());
                else
                    command.Parameters.AddWithValue("@statement_id", null);

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