using HomeBankingMindHub.dtos;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.ModelsDTO;
using HomeBankingMindHub.Repositories;
using HomeBankingMindHub.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoansController : ControllerBase
    {
        private IClientRepository _clientRepository;

        private IAccountRepository _accountRepository;

        private ILoanRepositry _loanRepository;

        private IClientLoanRepository _clientLoanRepository;

        private ITransactionRepository _transactionRepository;

        public LoansController(IClientRepository clientRepository, IAccountRepository accountRepository, ILoanRepositry loanRepository, IClientLoanRepository clientLoanRepository, ITransactionRepository transactionRepository)
        {
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _loanRepository = loanRepository;
            _clientLoanRepository = clientLoanRepository;
            _transactionRepository = transactionRepository;
        }

        [HttpGet]
        public IActionResult GetLoans() {
            try
            {
                var loans = _loanRepository.GetAll();
                if (loans.IsNullOrEmpty())
                    return StatusCode(204, "No Loans");

                return Ok(loans);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public IActionResult CreateLoan([FromBody] LoanApplicationDTO loanApplicationDTO)
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email == string.Empty)
                    return Forbid();

                var client = _clientRepository.FindByEmail(email);
                if (client == null)
                    return Forbid();

                var loan = _loanRepository.FindById(loanApplicationDTO.LoanId);
                if (loan == null) 
                    return StatusCode(403,"El prestamo no existe");

                //debe recibir un LoanDto del front tambien?
                if(loanApplicationDTO.Amount <=0 || loanApplicationDTO.Amount > loan.MaxAmount)
                    return StatusCode(403,"El monto no puede ser negativo, 0 o mayor a el maximo del prestamo");

                if (loanApplicationDTO.Payments.IsNullOrEmpty())
                    return StatusCode(403, "Payments vacios");

                var account = _accountRepository.FinByNumber(loanApplicationDTO.ToAccountNumber);
                if (account == null)
                    return StatusCode(403, "La cuenta destino esta vacia o no existe");

                if (account.ClientId != client.Id)
                    return StatusCode(403, "La cuenta no pertenece al cliente");

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

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
