using System;

using MySql.Data.MySqlClient;

using LMS.Model;
using LMS.Model.Resource;
using LMS.Data;
using LMS.Data.Cloud;
using LMS.Configuration;

namespace LMS.Data
{
    public class RepositoryFactory<T>
    {
        public static IRepository<T> Create()
        {
            IRepository<T> repository = null;

            if (typeof(T).Name == TenantRepository.TypeName)
                repository = (IRepository<T>)Activator.CreateInstance<TenantRepository>();
            if (typeof(T).Name == ArtistRepository.TypeName)
                repository = (IRepository<T>)Activator.CreateInstance<ArtistRepository>();
            if (typeof(T).Name == LabelRepository.TypeName)
                repository = (IRepository<T>)Activator.CreateInstance<LabelRepository>();
            if (typeof(T).Name == AccountRepository.TypeName)
                repository = (IRepository<T>)Activator.CreateInstance<AccountRepository>();
            if (typeof(T).Name == TransactionRepository.TypeName)
                repository = (IRepository<T>)Activator.CreateInstance<TransactionRepository>();
            if (typeof(T).Name == StatementRepository.TypeName)
                repository = (IRepository<T>)Activator.CreateInstance<StatementRepository>();
            if (typeof(T).Name == InvoiceRepository.TypeName)
                repository = (IRepository<T>)Activator.CreateInstance<InvoiceRepository>();
            if (typeof(T).Name == UserRepository.TypeName)
                repository = (IRepository<T>)Activator.CreateInstance<UserRepository>();

            if (repository == null)
                throw new Exception(String.Format("Unable to locate repository for type '{0}'.", typeof(T).Name));

            //repository.Connection = new MySqlConnection(WebConfigurationManager.AppSettings["DatabaseConnection"]);
            repository.Connection = new MySqlConnection(ConfigurationManager.Database.ToString());
            repository.Transaction = null;

            return repository;
        }
    }
}
