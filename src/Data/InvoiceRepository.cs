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
    public class InvoiceRepository : Repository<Invoice>
    {
        public static string TypeName
        {
            get
            {
                return typeof(Invoice).Name;
            }
        }

        public InvoiceRepository()
            : base()
        {
        }

        public override Invoice Add(Invoice item)
        {
            Log.Info(String.Format("Add new resource '{0}'; Tenant={1}", TypeName, TenantIdentifier));

            try
            {
                Connection.Open();

                MySqlCommand command = (MySqlCommand)CreateCommand(true);

                command.CommandText = "INSERT INTO invoice(`tenant_id`,`invoice_number`,`status`,`paypal_address`) VALUES(@tenant_id,@invoice_number,@status,@paypal_address); SELECT LAST_INSERT_ID();";
                command.Parameters.AddWithValue("@tenant_id", TenantIdentifier);
                command.Parameters.AddWithValue("@invoice_number", item.InvoiceNumber);
                command.Parameters.AddWithValue("@status", item.Status);
                command.Parameters.AddWithValue("@paypal_address", item.PaypalAddress);

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

        public override List<Invoice> Find(Invoice query)
        {
            Log.Info(String.Format("Query for resource '{0}'; Tenant={1}", TypeName, TenantIdentifier));
            throw new NotImplementedException();
        }

        public override Invoice Get(int id)
        {
            Log.Info(String.Format("Get resource '{0}'; Tenant={1}; Id={2}", TypeName, TenantIdentifier, id));

            Invoice item = null;

            try
            {
                Connection.Open();

                MySqlCommand command = (MySqlCommand)CreateCommand(false);

                command.CommandText = "SELECT invoice.*, (SELECT statement.`id` FROM statement WHERE statement.`tenant_id`=@tenant_id AND statement.`invoice_id`=invoice.`id`) AS 'statement_id' FROM invoice WHERE invoice.`tenant_id`=@tenant_id AND invoice.`id`=@id;";
                command.Parameters.AddWithValue("@tenant_id", TenantIdentifier);
                command.Parameters.AddWithValue("@id", id);

                using (IDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        item = new Invoice()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            InvoiceNumber = reader.GetString(reader.GetOrdinal("invoice_number")),
                            Status = (InvoiceStatus)reader.GetByte(reader.GetOrdinal("status")),
                            EffectiveTime = reader.GetDateTime(reader.GetOrdinal("effective_time")),
                            PaypalAddress = reader.GetString(reader.GetOrdinal("paypal_address"))
                        };

                        if (!reader.IsDBNull(reader.GetOrdinal("statement_id")))
                            item.Statement = new Reference(Reference.StatementUri, reader.GetInt32(reader.GetOrdinal("statement_id")));

                        Log.Debug(String.Format("Invoice.Id={0} found", item.Id));
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

                command.CommandText = "DELETE FROM invoice WHERE `tenant_id`=@tenant_id AND `id`=@id;";
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

        public override Invoice Update(Invoice item)
        {
            Log.Info(String.Format("Update resource '{0}'; Tenant={1}; Id={2}", TypeName, TenantIdentifier, item.Id));

            try
            {
                Connection.Open();

                MySqlCommand command = (MySqlCommand)CreateCommand(true);

                command.CommandText = "UPDATE invoice SET `status`=@status WHERE `tenant_id`=@tenant_id AND `id`=@id;";
                command.Parameters.AddWithValue("@tenant_id", TenantIdentifier);
                command.Parameters.AddWithValue("@id", item.Id);
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