using HomeBankingMindHub.Models;
using HomeBankingMindHub.ModelsDTO;
using HomeBankingMindHub.Repositories.Interfaces;

namespace HomeBankingMindHub.Services.Implement
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }
        public AccountDTO FindAccountById(long Id)
        {
            return new AccountDTO(_accountRepository.FindById(Id));
        }

        public AccountDTO FindAccountByNumber(string accountNumber)
        {
            return new AccountDTO(_accountRepository.FindByNumber(accountNumber));
        }

        public List<AccountDTO> GetAllAccounts()
        {
            IEnumerable<Account> accounts = _accountRepository.GetAllAccounts();
            List <AccountDTO> accountsDTO = new List<AccountDTO>();
            if(accounts != null)
            {
                foreach(Account account in accounts)
                {
                    AccountDTO accountDTO = new(account);
                    accountsDTO.Add(accountDTO);
                }
                return accountsDTO;
            }
            else
            {
                return null;
            }

        }
    }
}
