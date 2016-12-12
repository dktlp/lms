using System;
using System.Collections.Generic;
using System.Data;

namespace LMS.Data
{
    public interface IRepository<T>
    {
        IDbConnection Connection { get; set; }
        IDbTransaction Transaction { get; set; }

        List<T> Find(T query);
        List<T> Find(T query, Predicate<T> filter);
        T Get(int id);
        T Add(T item);
        T Update(T item);
        void Remove(int id);
    }
}