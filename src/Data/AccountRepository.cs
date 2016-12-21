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
    public class AccountRepository : Repository<Account>
    {
        public static string TypeName
        {
            get
            {
                return typeof(Account).Name;
            }
        }

        public AccountRepository()
            : base()
        {
        }

        public override Account Add(Account item)
        {
            Log.Info(String.Format("Add new resource '{0}'; Tenant={1}", TypeName, TenantIdentifier));

            try
            {
                Connection.Open();

                MySqlCommand command = (MySqlCommand)CreateCommand(true);

                command.CommandText = "INSERT INTO account(`tenant_id`,`label_id`,`artist_id`,`name`,`status`) VALUES(@tenant_id,@label_id,@artist_id,@name,@status); SELECT LAST_INSERT_ID();";
                command.Parameters.AddWithValue("@tenant_id", TenantIdentifier);
                command.Parameters.AddWithValue("@label_id", item.Label.GetId());
                command.Parameters.AddWithValue("@artist_id", item.Artist.GetId());
                command.Parameters.AddWithValue("@name", item.Name);
                command.Parameters.AddWithValue("@status", item.Status);

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

        public override List<Account> Find(Account query)
        {
            Log.Info(String.Format("Query for resource '{0}'; Tenant={1}", TypeName, TenantIdentifier));

            List<Account> result = new List<Account>();

            try
            {
                Connection.Open();

                MySqlCommand command = (MySqlCommand)CreateCommand(false);

                if (query == null)
                {
                    command.CommandText = "SELECT account.*, IFNULL((SELECT SUM(transaction.`amount`) FROM transaction WHERE account.`id`= transaction.`account_id` AND transaction.`tenant_id`=@tenant_id), 0) AS 'balance' FROM account WHERE account.`tenant_id`=@tenant_id;";
                    command.Parameters.AddWithValue("@tenant_id", TenantIdentifier);
                }
                else
                {
                    Dictionary<string, object> parameters = new Dictionary<string, object>();
                    StringBuilder sql = new StringBuilder();
                    sql.Append("SELECT account.*, IFNULL((SELECT SUM(transaction.`amount`) FROM transaction WHERE account.`id`= transaction.`account_id` AND transaction.`tenant_id`=@tenant_id), 0) AS 'balance' FROM account WHERE account.`tenant_id`=@tenant_id");

                    // Dynamically build sql statement and list of parameters
                    if (query.Name != null)
                    {
                        sql.Append(" AND account.`name` LIKE @name");
                        parameters.Add("@name", "%" + query.Name + "%");
                    }
                    if (query.Status > 0)
                    {
                        sql.Append(" AND account.`status`=@status");
                        parameters.Add("@status", query.Status);
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
                        Account item = new Account()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Label = new Reference(Reference.LabelUri, reader.GetInt32(reader.GetOrdinal("label_id"))),
                            Artist = new Reference(Reference.ArtistUri, reader.GetInt32(reader.GetOrdinal("artist_id"))),
                            Name = reader.GetString(reader.GetOrdinal("name")),
                            Status = (AccountStatus)reader.GetByte(reader.GetOrdinal("status")),
                            Balance = reader.GetDouble(reader.GetOrdinal("balance"))
                        };

                        Log.Debug(String.Format("Account.Id={0} found", item.Id));

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

        public override Account Get(int id)
        {
            Log.Info(String.Format("Get resource '{0}'; Tenant={1}; Id={2}", TypeName, TenantIdentifier, id));

            Account item = null;

            try
            {
                Connection.Open();

                MySqlCommand command = (MySqlCommand)CreateCommand(false);

                command.CommandText = "SELECT account.`id`, account.`label_id`, account.`artist_id`, account.`name`, account.`status`, IFNULL((SELECT SUM(transaction.`amount`) FROM transaction WHERE account.`id`= transaction.`account_id` AND transaction.`tenant_id`=@tenant_id), 0) AS 'balance' FROM account WHERE account.`tenant_id`=@tenant_id AND `id`=@id;";
                command.Parameters.AddWithValue("@tenant_id", TenantIdentifier);
                command.Parameters.AddWithValue("@id", id);

                using (IDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        item = new Account()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Label = new Reference(Reference.LabelUri, reader.GetInt32(reader.GetOrdinal("label_id"))),
                            Artist = new Reference(Reference.ArtistUri, reader.GetInt32(reader.GetOrdinal("artist_id"))),
                            Name = reader.GetString(reader.GetOrdinal("name")),
                            Status = (AccountStatus)reader.GetByte(reader.GetOrdinal("status")),
                            Balance = reader.GetDouble(reader.GetOrdinal("balance"))
                        };

                        Log.Debug(String.Format("Account.Id={0} found", item.Id));
                    }
                }

                // Load all transactions associated with the account.
                if (item != null && item.Id > 0)
                {
                    List<Transaction> transactions = new List<Transaction>();

                    command = (MySqlCommand)CreateCommand(false);
                    command.CommandText = "SELECT * FROM transaction WHERE `account_id`=@account_id AND `tenant_id`=@tenant_id;";
                    command.Parameters.AddWithValue("@tenant_id", TenantIdentifier);
                    command.Parameters.AddWithValue("@account_id", id);

                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Transaction transaction = new Transaction()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                EffectiveTime = reader.GetDateTime(reader.GetOrdinal("effective_time")),
                                Status = (TransactionStatus)reader.GetByte(reader.GetOrdinal("status")),
                                Type = (TransactionType)reader.GetByte(reader.GetOrdinal("type")),
                                Amount = reader.GetDouble(reader.GetOrdinal("amount")),
                                Quarter = reader.GetString(reader.GetOrdinal("quarter"))
                            };

                            if (!reader.IsDBNull(reader.GetOrdinal("statement_id")))
                                transaction.Statement = new Reference(Reference.StatementUri, reader.GetInt32(reader.GetOrdinal("statement_id")));

                            transactions.Add(transaction);
                        }
                    }

                    if (transactions.Count > 0)
                    {
                        Log.Debug(String.Format("Account.Id={0} has {1} Transaction(s)", item.Id, transactions.Count));
                        item.Transactions = transactions.ToArray();
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

                command.CommandText = "DELETE FROM account WHERE `tenant_id`=@tenant_id AND `id`=@id; DELETE FROM transaction WHERE `tenant_id`=@tenant_id AND `account_id`=@id;";
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

        public override Account Update(Account item)
        {
            Log.Info(String.Format("Update resource '{0}'; Tenant={1}; Id={2}", TypeName, TenantIdentifier, item.Id));

            try
            {
                Connection.Open();

                MySqlCommand command = (MySqlCommand)CreateCommand(true);

                command.CommandText = "UPDATE account SET `name`=@name,`status`=@status WHERE `tenant_id`=@tenant_id AND `id`=@id;";
                command.Parameters.AddWithValue("@tenant_id", TenantIdentifier);
                command.Parameters.AddWithValue("@id", item.Id);
                command.Parameters.AddWithValue("@name", item.Name);
                command.Parameters.AddWithValue("@status", item.Status);
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