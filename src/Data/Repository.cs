using System;
using System.Collections.Generic;
using System.Web;
using System.Data;

using log4net;

namespace LMS.Data
{
    public abstract class Repository<T> : IRepository<T>
    {
        protected static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public IDbConnection Connection { get; set; }
        public IDbTransaction Transaction { get; set; }

        public string TenantIdentifier
        {
            get
            {
                return HttpContext.Current.Request.Headers["lms.tenant.identifier"];
            }
        }

        public IDbCommand CreateCommand()
        {
            return CreateCommand(false);
        }

        public IDbCommand CreateCommand(bool beginTransaction)
        {
            Transaction = (beginTransaction) ? Connection.BeginTransaction(IsolationLevel.Serializable) : null;

            IDbCommand command = Connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.Transaction = Transaction;

            return command;
        }

        public abstract T Get(int id);
        public abstract T Add(T item);
        public abstract T Update(T item);
        public abstract void Remove(int id);
        public abstract List<T> Find(T query);

        public List<T> Find(T query, Predicate<T> filter)
        {
            return Find(query).FindAll(filter);
        }       

        public Repository()
        {
        }
    }
}