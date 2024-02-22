using HomeBankingMindHub.dtos;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.ModelsDTO;

namespace HomeBankingMindHub.Services
{
    public interface IClientService
    {
        ClientDTO GetClientById(long id);
        List<ClientDTO> GetAllClients();
        Client FindClientByEmail(String email);
        ClientDTO SaveClient(SignUpClientDTO client);
        List<Account> GetAccounts(ClientDTO clientDTO);
        Account SaveAccount(Client client);
        List<Card> GetCards(ClientDTO clientDTO);
        CardDTO SaveCard(Client client,CardDTO cardDTO);
        
    }
}
