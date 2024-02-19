using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories.Interfaces;

namespace HomeBankingMindHub.Repositories
{
    public class LoanRepository : RepositoryBase<Loan>, ILoanRepositry
    {
        public LoanRepository(HomeBankingContext repositoryContext) : base(repositoryContext)
        {
        }

        public Loan FindById(long id)
        {
            return FindByCondition(loan => loan.Id == id).FirstOrDefault();
        }

        public IEnumerable<Loan> GetAll()
        {
            return FindAll().ToList();
        }
    }
}
