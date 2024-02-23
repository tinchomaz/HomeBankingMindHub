using HomeBankingMindHub.Models;
using HomeBankingMindHub.ModelsDTO;

namespace HomeBankingMindHub.Services.Interfaces
{
    public interface IAccountService
    {
        List<AccountDTO> GetAllAccounts();
        AccountDTO FindAccountById(long Id);
        AccountDTO FindAccountByNumber(string accountNumber);
    }
}
