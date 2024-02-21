using HomeBankingMindHub.ModelsDTO;

namespace HomeBankingMindHub.Services
{
    public interface IClientService
    {
        ClientDTO getClientById(long id);
        List<ClientDTO> getAllClients();
        ClientDTO findClientByEmail(String email);
    }
}
