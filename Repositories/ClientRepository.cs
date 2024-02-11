using HomeBankingMindHub.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace HomeBankingMindHub.Repositories
{
    public class ClientRepository : RepositoryBase<Client>, IClientRepository
    {
        public ClientRepository(HomeBankingContext repositoryContext) : base(repositoryContext)
        {
        }

        private IQueryable<Client> IncludeMetodo(IQueryable<Client> query)
        {
            return query.Include(client => client.Accounts)
                        .Include(client => client.ClientLoans)
                            .ThenInclude(cl => cl.Loan)
                        .Include(client => client.Cards);
        }

        public Client FindById(long id)
        {
            return IncludeMetodo(FindByCondition(client => client.Id == id))
                .FirstOrDefault();
        }

        public IEnumerable<Client> GetAllClients()
        {
            return IncludeMetodo(FindAll()).ToList();
        }

        public void Save(Client client)
        {
            Create(client);
            SaveChanges();
        }

        public Client FindByEmail(string email)
        {
            return IncludeMetodo(FindByCondition(client =>client.Email.ToUpper() == email.ToUpper()))
            .FirstOrDefault();
        }
    }
}