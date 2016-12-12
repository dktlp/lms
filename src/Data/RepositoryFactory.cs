﻿using System;
using System.Web.Configuration;

using MySql.Data.MySqlClient;

using LMS.Model;
using LMS.Model.Resource;
using LMS.Data;

namespace LMS.Data
{
    public class RepositoryFactory<T>
    {
        public static IRepository<T> Create()
        {
            IRepository<T> repository = null;

            if (typeof(T).Name == ArtistRepository.TypeName)
                repository = (IRepository<T>)Activator.CreateInstance<ArtistRepository>();

            if (repository == null)
                throw new Exception(String.Format("Unable to locate repository for type '{0}'.", typeof(T).Name));

            repository.Connection = new MySqlConnection(WebConfigurationManager.AppSettings["DatabaseConnection"]);
            repository.Transaction = null;

            return repository;
        }
    }
}
