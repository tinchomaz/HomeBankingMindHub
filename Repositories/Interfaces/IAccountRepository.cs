﻿using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Repositories.Interfaces
{
    public interface IAccountRepository
    {
        IEnumerable<Account> GetAllAccounts();
        void Save(Account account);
        Account FindById(long id);
        Account FinByNumber(string number);
        IEnumerable<Account> GetAccountsByClient(long clientId);
    }
}
