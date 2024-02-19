using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Repositories.Interfaces
{
    public interface ILoanRepositry
    {
        IEnumerable<Loan> GetAll();
        Loan FindById(long id);
    }
}
