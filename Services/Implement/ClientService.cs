using HomeBankingMindHub.dtos;
using HomeBankingMindHub.Lib;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.ModelsDTO;
using HomeBankingMindHub.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace HomeBankingMindHub.Services.Implement
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ICardRepository _cardRepository;
        public ClientService(IClientRepository clientRepository,IAccountRepository accountRepository,ICardRepository cardRepository)
        {
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _cardRepository = cardRepository;
        }
        public Client FindClientByEmail(string email)
        {
            Client client = _clientRepository.FindByEmail(email);

            return client;
        }
        public List<ClientDTO> GetAllClients()
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
        public ClientDTO GetClientById(long id)
        {
            var client = _clientRepository.FindById(id);
            if (client != null)
                return null;
            var ClientDTO = new ClientDTO(client);
            return ClientDTO;
        }
        public ClientDTO SaveClient(SignUpClientDTO clientDTO)
        {
            Client client = new Client(clientDTO);
            _clientRepository.Save(client);
            return new ClientDTO(client);
        }
        public List<Account> GetAccounts(ClientDTO clientDTO)
        {
            return (List<Account>)_accountRepository.GetAccountsByClient(clientDTO.Id);
        }
        public Account SaveAccount(Client client)
        {
            var newAccount = new Account
            {
                Number = "VIN-" + new Random().Next(1000000).ToString(),
                CreationDate = DateTime.Now,
                Balance = 0,
                ClientId = client.Id
            };

            _accountRepository.Save(newAccount);
            return newAccount;
        }
        public List<Card> GetCards(ClientDTO clientDTO)
        {
            if(clientDTO != null)
                return (List<Card>)_cardRepository.GetCardsByClient(clientDTO.Id);
            else return null;
        }
        public CardDTO SaveCard(Client client,CardDTO cardDTO)
        {
            var random = new Random();
            Card newCard = new Card
            {
                CardHolder = client.FirstName + " " + client.LastName,
                Type = Enum.Parse<CardType>(cardDTO.Type),
                Color = Enum.Parse<CardColor>(cardDTO.Color),
                Number = random.Next(10000).ToString() + "-" + random.Next(10000).ToString() + "-" +
                        random.Next(10000).ToString() + "-" + random.Next(10000).ToString(),
                Cvv = random.Next(1000),
                FromDate = DateTime.Now,
                ThruDate = DateTime.Now.AddYears(5),
                ClientId = client.Id
            };
            _cardRepository.Save(newCard);
            return new CardDTO(newCard);
        }
    }
}
