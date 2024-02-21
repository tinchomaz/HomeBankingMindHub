using HomeBankingMindHub.dtos;
using HomeBankingMindHub.Lib;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.ModelsDTO;
using HomeBankingMindHub.Repositories;
using HomeBankingMindHub.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;

using Microsoft.AspNetCore.Mvc;

using System;

using System.Collections.Generic;

using System.Linq;



namespace HomeBankingMindHub.Controllers

{

    [Route("api/[controller]")]

    [ApiController]

    public class ClientsController : ControllerBase

    {

        private IClientRepository _clientRepository;

        private IAccountRepository _accountRepository;

        private ICardRepository _cardRepository;

        public ClientsController(IClientRepository clientRepository, IAccountRepository accountRepository, ICardRepository cardRepository)
        {
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _cardRepository = cardRepository;
        }

        private Client GetClientByEmail(out IActionResult result)
        {
            string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
            if (email == string.Empty)
            {
                result = Forbid();
                return null;
            }

            var client = _clientRepository.FindByEmail(email);
            if (client == null)
            {
                result = Forbid();
                return null;
            }

            result = null;
            return client;
        }

        [HttpGet]

        public IActionResult Get()

        {
            try
            {
                var clients = _clientRepository.GetAllClients();

                var clientsDTO = new List<ClientDTO>();

                foreach (Client client in clients)

                {
                    var newClientDTO = new ClientDTO
                    {
                        Email = client.Email,

                        FirstName = client.FirstName,

                        LastName = client.LastName,

                        Accounts = client.Accounts.Select(ac => new AccountDTO

                        {
                            Balance = ac.Balance,

                            CreationDate = ac.CreationDate,

                            Number = ac.Number

                        }).ToList(),

                        Loans = client.ClientLoans.Select(cl => new ClientLoanDTO
                        {
                            LoanId = cl.LoanId,
                            Name = cl.Loan.Name,
                            Amount = cl.Amount,
                            Payments = int.Parse(cl.Payments)
                        }).ToList(),

                        Credits = client.ClientLoans.Select(cl => new ClientLoanDTO
                        {
                            LoanId = cl.LoanId,
                            Name = cl.Loan.Name,
                            Amount = cl.Amount,
                            Payments = int.Parse(cl.Payments)
                        }).ToList(),

                        Cards = client.Cards.Select(c => new CardDTO
                        {
                            CardHolder = c.CardHolder,
                            Color = c.Color.ToString(),
                            Cvv = c.Cvv,
                            FromDate = c.FromDate,
                            Number = c.Number,
                            ThruDate = c.ThruDate,
                            Type = c.Type.ToString()
                        }).ToList()
                    };



                    clientsDTO.Add(newClientDTO);

                }
                //PREGUNTAR POR QUE NO DEVUELVE EL MENSAJE
                if (clientsDTO.Count() == 0)
                    return StatusCode(204,"No hay Clientes");

                return Ok(clientsDTO);

            }

            catch (Exception ex)

            {

                return StatusCode(500, ex.Message);

            }

        }

        //Get para recibir por id
        [HttpGet("{id}")]

        public IActionResult Get(long id)

        {

            try

            {

                var client = _clientRepository.FindById(id);

                if (client == null)

                {
                    return NotFound();
                }

                var clientDTO = new ClientDTO

                {

                    Email = client.Email,

                    FirstName = client.FirstName,

                    LastName = client.LastName,

                    Accounts = client.Accounts.Select(ac => new AccountDTO

                    {
                        Balance = ac.Balance,
                        CreationDate = ac.CreationDate,
                        Number = ac.Number
                    }).ToList(),

                    Loans = client.ClientLoans.Select(cl => new ClientLoanDTO
                    {
                        LoanId = cl.LoanId,
                        Name = cl.Loan.Name,
                        Amount = cl.Amount,
                        Payments = int.Parse(cl.Payments)
                    }).ToList(),

                    Credits = client.ClientLoans.Select(cl => new ClientLoanDTO
                    {
                        LoanId = cl.LoanId,
                        Name = cl.Loan.Name,
                        Amount = cl.Amount,
                        Payments = int.Parse(cl.Payments)
                    }).ToList(),

                    Cards = client.Cards.Select(c => new CardDTO
                    {
                        CardHolder = c.CardHolder,
                        Color = c.Color.ToString(),
                        Cvv = c.Cvv,
                        FromDate = c.FromDate,
                        Number = c.Number,
                        ThruDate = c.ThruDate,
                        Type = c.Type.ToString()
                    }).ToList()

                };



                return Ok(clientDTO);

            }

            catch (Exception ex)

            {

                return StatusCode(500, ex.Message);

            }

        }

        [HttpGet("current")]
        public IActionResult GetCurrent()
        {
            try
            {
                var client = GetClientByEmail(out IActionResult forbidResult);
                if (forbidResult != null)
                {
                    return forbidResult;
                }

                var clientDTO = new ClientDTO
                {
                    Email = client.Email,
                    FirstName = client.FirstName,
                    LastName = client.LastName,
                    Accounts = client.Accounts.Select(ac => new AccountDTO
                    {
                        Id = ac.Id,
                        Balance = ac.Balance,
                        CreationDate = ac.CreationDate,
                        Number = ac.Number
                    }).ToList(),
                    Credits = client.ClientLoans.Select(cl => new ClientLoanDTO
                    {
                        LoanId = cl.LoanId,
                        Name = cl.Loan.Name,
                        Amount = cl.Amount,
                        Payments = int.Parse(cl.Payments)
                    }).ToList(),
                    Cards = client.Cards.Select(c => new CardDTO
                    {
                        CardHolder = c.CardHolder,
                        Color = c.Color.ToString(),
                        Cvv = c.Cvv,
                        FromDate = c.FromDate,
                        Number = c.Number,
                        ThruDate = c.ThruDate,
                        Type = c.Type.ToString(),
                    }).ToList()
                };

                return Ok(clientDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] SignUpClientDTO client)
        {
            try
            {
                //validamos datos antes
                if (String.IsNullOrEmpty(client.Email) || String.IsNullOrEmpty(client.Password) || String.IsNullOrEmpty(client.FirstName) || String.IsNullOrEmpty(client.LastName))
                    return StatusCode(401, "datos inválidos");

                //buscamos si ya existe el usuario
                Client user = _clientRepository.FindByEmail(client.Email);

                if (user != null)
                    return StatusCode(403, "Email está en uso");

                Client newClient = new Client
                {
                    Email = client.Email,
                    Password = Hashing.HashPassword(client.Password),
                    FirstName = client.FirstName,
                    LastName = client.LastName,
                };

                _clientRepository.Save(newClient);
                return Created("", newClient);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("current/accounts")]
        public IActionResult GetCurrentAccounts()
        {
            var client = GetClientByEmail(out IActionResult forbidResult);
            if (forbidResult != null)
            {
                return forbidResult;
            }
            try
            {
                var clientAccounts = client.Accounts.Select(ac => new AccountDTO
                {
                    Balance = ac.Balance,
                    CreationDate = ac.CreationDate,
                    Number = ac.Number
                }).ToList();

                return Ok(clientAccounts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("current/accounts")]
        public IActionResult CreateAccount()
        {
            try
            {
                var client = GetClientByEmail(out IActionResult forbidResult);
                if (forbidResult != null)
                {
                    return forbidResult;
                }

                if (client.Accounts.Count == 3)
                    return StatusCode(403, "Tienes el limite de 3 cuentas ya creadas");
                string random;
                do
                {
                    random = "VIN-" + new Random().Next(1, 100000000).ToString("D8");
                } while (_accountRepository.FindAccountByNumber(random) != null);
                var newAccount = new Account
                {
                    Number = random,
                    CreationDate = DateTime.Now,
                    Balance = 0,
                    ClientId = client.Id
                };

                _accountRepository.Save(newAccount);

                return Created("", newAccount);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("current/cards")]
        public IActionResult GetCurrentCards()
        {
            var client = GetClientByEmail(out IActionResult forbidResult);
            if (forbidResult != null)
            {
                return forbidResult;
            }
            try
            {
                var clientCards = client.Cards.Select(card => new CardDTO
                {
                    CardHolder = card.CardHolder,
                    Type = card.Type.ToString(),
                    Color = card.Color.ToString(),
                    Number = card.Number,
                    Cvv = card.Cvv,
                    FromDate = card.FromDate,
                    ThruDate = card.ThruDate
                }).ToList();

                return Ok(clientCards);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("current/cards")]
        public IActionResult CreateCard([FromBody] CardDTO cardDTO)
        {
            try
            {
                var client = GetClientByEmail(out IActionResult forbidResult);
                if (forbidResult != null)
                {
                    return forbidResult;
                }

                var contCards = 0;

                foreach(Card card in client.Cards)
                {
                    if(card.Type.ToString() == cardDTO.Type)
                    {
                        contCards++;
                    }
                }

                if (contCards >= 3)
                    return StatusCode(403, "el cliente ya tiene 3 tarjetas del mismo tipo");

                var random = new Random();

                var newCard = new Card
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

                return Created("", newCard);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}