using HomeBankingMindHub.dtos;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.ModelsDTO;

namespace HomeBankingMindHub.Services.Interfaces
{
    public interface IClientService
    {
        ClientDTO GetClientById(long id);
        List<ClientDTO> GetAllClients();
        Client FindClientByEmail(string email);
        void SaveClient(SignUpClientDTO client, out int statusCode, out string? message);
        List<Account> GetAccounts(ClientDTO clientDTO);
        void SaveAccount(Client client, out int statusCode, out string? message);
        List<Card> GetCards(ClientDTO clientDTO);
        void SaveCard(Client client, CardDTO cardDTO, out int statusCode, out string? message);

    }
}
