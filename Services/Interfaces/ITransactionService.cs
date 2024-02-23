using HomeBankingMindHub.dtos;
using HomeBankingMindHub.ModelsDTO;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;

namespace HomeBankingMindHub.Services
{
    public interface ITransactionService
    {
        void PostDTO(TransferDTO transferDTO,string email,out int statusCode,out string? message);
    }
}
