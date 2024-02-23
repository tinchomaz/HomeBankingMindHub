using HomeBankingMindHub.dtos;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.ModelsDTO;
using HomeBankingMindHub.Repositories;
using HomeBankingMindHub.Repositories.Interfaces;
using HomeBankingMindHub.Services.Interfaces;

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

        public void PostDTO(TransferDTO transferDTO,string email,out int statusCode,out string message)
        {
            Client client = _clientRepository.FindByEmail(email);
            message = null;
            statusCode = 403;
            if (client == null)
            {
                message = "No existe el cliente";
                return;
            }
            if (transferDTO.FromAccountNumber == string.Empty || transferDTO.ToAccountNumber == string.Empty)
            {
                message = "Cuenta de origen o cuenta de destino no proporcionada.";
                return;
            }
            //buscamos la cuenta de destino
            Account toAccount = _accountRepository.FindByNumber(transferDTO.ToAccountNumber);
            if (toAccount == null)
            {
                message = "Cuenta Destino no existe";
                return;
            }
            //buscamos las cuentas
            Account fromAccount = _accountRepository.FindByNumber(transferDTO.FromAccountNumber);
            if (fromAccount == null)
            {
                message = "Cuenta de origen no existe";
                return;
            }
            if (fromAccount.ClientId != client.Id)
            {
                message = "La cuenta no pertenece al cliente";
                return;
            }

            if (transferDTO.FromAccountNumber == transferDTO.ToAccountNumber)
            {
                message = "No se permite la transferencia a la misma cuenta.";
                return;
            }
            if (transferDTO.Amount == 0 || transferDTO.Description == string.Empty)
            {
                message = "Monto o descripción no proporcionados.";
                return;
            }
            if (transferDTO.Amount < 0)
            {
                message = "El monto no puede ser negativo";
                return;
            }
            //controlamos el monto
            if (fromAccount.Balance < transferDTO.Amount)
            {
                message = "Fondos Insuficientes";
                return;
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
            if(message == null)
            {
                //seteamos los valores de las cuentas, a la ccuenta de origen le restamos el monto
                fromAccount.Balance = fromAccount.Balance - transferDTO.Amount;
                //actualizamos la cuenta de origen
                _accountRepository.Save(fromAccount);
                //a la cuenta de destino le sumamos el monto
                toAccount.Balance = toAccount.Balance + transferDTO.Amount;
                //actualizamos la cuenta de destino
                _accountRepository.Save(toAccount);
                statusCode = 200;
                message = "Transaccion realizada";
            }

        }
    }
}
