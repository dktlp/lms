using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LMS.Model;
using LMS.Model.Resource;
using LMS.Data;

namespace LMS.Test
{
    public class UnitTestContext<T>
        where T : DomainResource
    {
        public List<DomainResource> TestData { get; set; }
        public T Template { get; set; }

        public IRepository<Label> LabelRepository { get; set; }
        public IRepository<Artist> ArtistRepository { get; set; }
        public IRepository<Account> AccountRepository { get; set; }
        public IRepository<Transaction> TransactionRepository { get; set; }
        public IRepository<Statement> StatementRepository { get; set; }
        public IRepository<Invoice> InvoiceRepository { get; set; }

        public UnitTestContext()
        {
            TestData = new List<DomainResource>();
            Template = default(T);

            LabelRepository = RepositoryFactory<Label>.Create();
            ArtistRepository = RepositoryFactory<Artist>.Create();
            AccountRepository = RepositoryFactory<Account>.Create();
            TransactionRepository = RepositoryFactory<Transaction>.Create();
            StatementRepository = RepositoryFactory<Statement>.Create();
            InvoiceRepository = RepositoryFactory<Invoice>.Create();
        }
    }
}