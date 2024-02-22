using HomeBankingMindHub.dtos;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.ModelsDTO;
using HomeBankingMindHub.Repositories;
using HomeBankingMindHub.Repositories.Interfaces;

namespace HomeBankingMindHub.Services.Implement
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IAccountRepository _accountRepository;

        public TransactionService(IClientRepository clientRepository, IAccountRepository accountRepository, ITransactionRepository transactionRepository)
        {
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
        }

        public AccountDTO PostDTO(TransferDTO transferDTO,string email,out int statusCode,out string message)
        {
            Client client = _clientRepository.FindByEmail(email);
            if (client == null)
            {
                statusCode = 403;
                message = "No existe el cliente";
                return null;
            }
            if (transferDTO.FromAccountNumber == string.Empty || transferDTO.ToAccountNumber == string.Empty)
            {
                statusCode = 403;
                message = "Cuenta de origen o cuenta de destino no proporcionada.";
                return null;
            }
            if (transferDTO.FromAccountNumber == transferDTO.ToAccountNumber)
            {
                statusCode = 403;
                message = "No se permite la transferencia a la misma cuenta.";
                return null;
            }
            if (transferDTO.Amount == 0 || transferDTO.Description == string.Empty)
            {
                statusCode = 403;
                message = "Monto o descripción no proporcionados.";
                return null;
            }
            if (transferDTO.Amount < 0)
            {
                statusCode = 403;
                message = "El monto no puede ser negativo";
                return null;
            }
            //buscamos las cuentas
            Account fromAccount = _accountRepository.FindByNumber(transferDTO.FromAccountNumber);
            if (fromAccount == null)
            {
                statusCode = 403;
                message = "Cuenta de origen no existe";
                return null;
            }
            //controlamos el monto
            if (fromAccount.Balance < transferDTO.Amount)
            {
                statusCode = 403;
                message = "Fondos Insuficientes";
                return null;
            }
            //buscamos la cuenta de destino
            Account toAccount = _accountRepository.FindByNumber(transferDTO.ToAccountNumber);
            if (toAccount == null)
            {
                statusCode = 403;
                message = "Cuenta Destino no existe";
                return null;
            }
            //demas logica para guardado
            //comenzamos con la inserrción de las 2 transacciones realizadas
            //desde toAccount se debe generar un debito por lo tanto lo multiplicamos por -1
            _transactionRepository.Save(new Transaction
            {
                Type = TransactionType.DEBIT,
                Amount = transferDTO.Amount * -1,
                Description = transferDTO.Description + " " + toAccount.Number,
                AccountId = fromAccount.Id,
                Date = DateTime.Now,
            });
            //ahora una credito para la cuenta fromAccount
            _transactionRepository.Save(new Transaction
            {
                Type = TransactionType.CREDIT,
                Amount = transferDTO.Amount,
                Description = transferDTO.Description + " " + fromAccount.Number,
                AccountId = toAccount.Id,
                Date = DateTime.Now,
            });
            //seteamos los valores de las cuentas, a la ccuenta de origen le restamos el monto
            fromAccount.Balance = fromAccount.Balance - transferDTO.Amount;
            //actualizamos la cuenta de origen
            _accountRepository.Save(fromAccount);
            //a la cuenta de destino le sumamos el monto
            toAccount.Balance = toAccount.Balance + transferDTO.Amount;
            //actualizamos la cuenta de destino
            _accountRepository.Save(toAccount);

            statusCode = 200;
            message = null;
            return new(fromAccount);
        }
    }
}
