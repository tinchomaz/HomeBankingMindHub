using HomeBankingMindHub.dtos;
using HomeBankingMindHub.Lib;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.ModelsDTO;
using HomeBankingMindHub.Repositories;
using HomeBankingMindHub.Repositories.Interfaces;
using HomeBankingMindHub.Services;
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
        private readonly IClientService _clientService;
        private IAccountRepository _accountRepository;

        private ICardRepository _cardRepository;

        public ClientsController(IClientService clientService,IAccountRepository accountRepository, ICardRepository cardRepository)
        {
            _clientService = clientService;
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
                return Ok(_clientService.getAllClients());
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
                if (id < 0)
                    return BadRequest("id invalido");
                var clientDTO = _clientService.getClientById(id);
                if (clientDTO == null)
                    return NotFound();
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

                var newAccount = new Account
                {
                    Number = "VIN-" + new Random().Next(1000000).ToString(),
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

                foreach (Card card in client.Cards)
                {
                    if (card.Color.ToString() == cardDTO.Color)
                    {
                        return StatusCode(403, "el cliente solo puede tener 1 tarjeta por cada color");
                    }
                }
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