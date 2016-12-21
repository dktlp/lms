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

                command.CommandText = "INSERT INTO transaction(`tenant_id`,`account_id`,`statement_id`,`type`,`status`,`amount`,`quarter`) VALUES(@tenant_id,@account_id,@statement_id,@type,@status,@amount,@quarter); SELECT LAST_INSERT_ID();";
                command.Parameters.AddWithValue("@tenant_id", TenantIdentifier);
                command.Parameters.AddWithValue("@account_id", item.Account.GetId());                
                command.Parameters.AddWithValue("@type", item.Type);
                command.Parameters.AddWithValue("@status", item.Status);
                command.Parameters.AddWithValue("@amount", item.Amount);
                command.Parameters.AddWithValue("@quarter", item.Quarter);

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
            throw new NotImplementedException();
        }

        public override Transaction Get(int id)
        {
            throw new NotImplementedException();
        }

        public override void Remove(int id)
        {
            throw new NotImplementedException();
        }

        public override Transaction Update(Transaction item)
        {
            throw new NotImplementedException();
        }
    }
}