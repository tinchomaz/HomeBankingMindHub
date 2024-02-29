using HomeBankingMindHub.dtos;
using HomeBankingMindHub.Lib;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.ModelsDTO;
using HomeBankingMindHub.Repositories.Interfaces;
using HomeBankingMindHub.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;

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
        public void SaveClient(SignUpClientDTO clientDTO, out int statusCode, out string? message)
        {
            Client client = new Client(clientDTO);
            //validamos datos antes
            if (String.IsNullOrEmpty(client.Email) || String.IsNullOrEmpty(client.Password) || String.IsNullOrEmpty(client.FirstName) || String.IsNullOrEmpty(client.LastName))
            {
                statusCode = 401;
                message = "datos inválidos";
                return;
            }
            /*verificamos si el email es valido NO FUNCIONA
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            if (Regex.IsMatch(client.Email, emailPattern))
            {
                statusCode = 401;
                message = "No es un email valido";
                return;
            }*/
            //buscamos si ya existe el usuario
            if (_clientRepository.FindByEmail(client.Email) != null)
            {
                statusCode = 403;
                message = "Email está en uso";
                return;
            }
            client.Password = Hashing.HashPassword(client.Password);
            if(client != null)
            {
                _clientRepository.Save(client);
                statusCode = 201;
                message = "Cliente Creado";
                return;
            }
            else
            {
                statusCode = 400;
                message = "No se pudo crear el cliente";
                return;
            }
        }
        public List<Account> GetAccounts(ClientDTO clientDTO)
        {
            return (List<Account>)_accountRepository.GetAccountsByClient(clientDTO.Id);
        }
        public void SaveAccount(Client client, out int statusCode, out string? message)
        {
            if (client.Accounts.Count == 3)
            {
                statusCode = 403;
                message = "Tienes el limite de 3 cuentas ya creadas";
                return;
            }
            int randomNumber = new Random().Next(100000000);
            string formattedNumber = randomNumber.ToString("D8");
            var newAccount = new Account
            {
                Number = "VIN-" + formattedNumber,
                CreationDate = DateTime.Now,
                Balance = 0,
                ClientId = client.Id
            };
            if (newAccount != null)
            {
                statusCode = 201;
                message = "Cuenta Creada";
                _accountRepository.Save(newAccount);
                return;
            }else
            {
                statusCode = 400;
                message = "No se pudo crear la cuenta";
                return;
            }
        }
        public List<Card> GetCards(ClientDTO clientDTO)
        {
            if(clientDTO != null)
                return (List<Card>)_cardRepository.GetCardsByClient(clientDTO.Id);
            else return null;
        }
        public void SaveCard(Client client,CardDTO cardDTO, out int statusCode, out string? message)
        {
            foreach (Card card in client.Cards)
            {
                if (card.Color.ToString() == cardDTO.Color)
                {
                    if (card.Type.ToString() == cardDTO.Type)
                    {
                        statusCode = 403;
                        message = "el cliente solo puede tener 1 tarjeta por cada color";
                        return;
                    }
                }
            }
            var random = new Random();
            Card newCard = new Card
            {
                CardHolder = client.FirstName + " " + client.LastName,
                Type = (CardType)Enum.Parse(typeof(CardType), cardDTO.Type, true),
                Color = (CardColor)Enum.Parse(typeof(CardColor), cardDTO.Color, true),
                Number = random.Next(10000).ToString() + "-" + random.Next(10000).ToString() + "-" +
                        random.Next(10000).ToString() + "-" + random.Next(10000).ToString(),
                Cvv = random.Next(1000),
                FromDate = DateTime.Now,
                ThruDate = DateTime.Now.AddYears(5),
                ClientId = client.Id
            };
            if(cardDTO != null)
            {
                _cardRepository.Save(newCard);
                statusCode = 201;
                message = "Tarjeta creada";
                return;
            }
            else
            {
                statusCode = 400;
                message = "La tarjeta no se pudo crear";
                return;
            }
        }
    }
}
