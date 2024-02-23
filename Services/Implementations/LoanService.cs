using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories.Interfaces;
using HomeBankingMindHub.Repositories;
using Microsoft.IdentityModel.Tokens;
using HomeBankingMindHub.ModelsDTO;
using HomeBankingMindHub.Services.Interfaces;

namespace HomeBankingMindHub.Services.Implement
{
    public class LoanService : ILoanService
    {
        private IClientRepository _clientRepository;

        private IAccountRepository _accountRepository;

        private ILoanRepositry _loanRepository;

        private IClientLoanRepository _clientLoanRepository;

        private ITransactionRepository _transactionRepository;

        public LoanService(IClientRepository clientRepository, IAccountRepository accountRepository, ILoanRepositry loanRepository, IClientLoanRepository clientLoanRepository, ITransactionRepository transactionRepository)
        {
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _loanRepository = loanRepository;
            _clientLoanRepository = clientLoanRepository;
            _transactionRepository = transactionRepository;
        }
        public List<LoanDTO> GetAllLoans(out int statusCode, out string? message)
        {
         IEnumerable<Loan> loans =_loanRepository.GetAll();
         List<LoanDTO> loansDTO = new();
            if(loans != null)
            {
                foreach (Loan loan in loans)
                {
                    LoanDTO loanDTO = new(loan);
                    loansDTO.Add(loanDTO);
                }
            }
            if (loansDTO.IsNullOrEmpty())
            {
                statusCode = 204;
                message = "No Loans";
                return null;
            }
            statusCode = 200;
            message = null;
            return loansDTO;
        }

        public void SaveLoan(LoanApplicationDTO loanApplicationDTO, out int statusCode, out string? message,string email)
        {
            var client = _clientRepository.FindByEmail(email);
            if (client == null)
                statusCode = 403;
                message = "Usuario no encontrado";

            var loan = _loanRepository.FindById(loanApplicationDTO.LoanId);
            if (loan == null) 
            {
                statusCode = 403;
                message = "El prestamo no existe";
            }
            //debe recibir un LoanDto del front tambien?
            if (loanApplicationDTO.Amount <= 0 || loanApplicationDTO.Amount > loan.MaxAmount)
            {
                statusCode = 403;
                message = "El monto no puede ser negativo, 0 o mayor a el maximo del prestamo";
            }
            if (loanApplicationDTO.Payments.IsNullOrEmpty())
            {
                statusCode = 403;
                message = "Payments vacios";
            }
            var account = _accountRepository.FindByNumber(loanApplicationDTO.ToAccountNumber);
            if (account == null)
            {
                statusCode = 403;
                message = "La cuenta destino esta vacia o no existe";
            }
            if (account.ClientId != client.Id)
            {
                statusCode = 403;
                message = "La cuenta no pertenece al cliente";
            }
            var clientLoan = new ClientLoan();
            clientLoan.Amount = loanApplicationDTO.Amount * 1.20;
            clientLoan.Payments = loanApplicationDTO.Payments;
            clientLoan.ClientId = client.Id;
            clientLoan.LoanId = loanApplicationDTO.LoanId;

            _clientLoanRepository.Save(clientLoan);

            var transaction = new Transaction();
            transaction.Type = TransactionType.CREDIT;
            transaction.Amount = loanApplicationDTO.Amount;
            transaction.Description = "Deposit for Loan " + loan.Name;
            transaction.AccountId = account.Id;

            _transactionRepository.Save(transaction);

            account.Balance += loanApplicationDTO.Amount;

            _accountRepository.Save(account);

            statusCode = 200;
            message = "Prestamo otorgado";
        }
    }
}
