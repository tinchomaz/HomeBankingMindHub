using HomeBankingMindHub.ModelsDTO;
using HomeBankingMindHub.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace HomeBankingMindHub.Services.Implement
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;
        public ClientService(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }
        public ClientDTO findClientByEmail(string email)
        {
            var email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
            if (email == string.Empty)
            {
                return null;
            }

            var clientDTO = new ClientDTO(_clientRepository.FindByEmail(email));
            if (clientDTO == null)
            {
                return null;
            }
            return clientDTO;
        }

        public List<ClientDTO> getAllClients()
        {
            var clients = _clientRepository.GetAllClients();
            var clientsDTO = new List<ClientDTO>();
            foreach (var client in clients)
            {
                ClientDTO clientDTO = new ClientDTO(client);
                clientsDTO.Add(clientDTO);
            }
            return clientsDTO;
        }

        public ClientDTO getClientById(long id)
        {
            var client = _clientRepository.FindById(id);
            if (client != null)
                return null;
            var ClientDTO = new ClientDTO(client);
            return ClientDTO;
        }

    }
}
