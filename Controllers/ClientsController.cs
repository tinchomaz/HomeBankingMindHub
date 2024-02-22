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
        public ClientsController(IClientService clientService)
        {
            _clientService = clientService;
        }
        private IActionResult Logued()
        {
            string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
            if (email == string.Empty)
            {
                return Forbid();
            }
            var client = _clientService.FindClientByEmail(email);
            if (client == null)
            {
                return Forbid();
            }
            return null;
        }

        [HttpGet]

        public IActionResult Get()
        {
            try
            {
                return Ok(_clientService.GetAllClients());
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
                var clientDTO = _clientService.GetClientById(id);
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
                IActionResult logued = Logued();
                if (logued != null)
                    return logued;
                Client client = _clientService.FindClientByEmail(User.FindFirst("Client").Value);
                return Ok(new ClientDTO(client));
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
                Client user = _clientService.FindClientByEmail(client.Email);
                if (user != null)
                    return StatusCode(403, "Email está en uso");
                _clientService.SaveClient(client);
                return Created("", client);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("current/accounts")]
        public IActionResult GetCurrentAccounts()
        {
            IActionResult logued = Logued();
            if(logued != null)
                return logued;
            try
            {
                var client = _clientService.FindClientByEmail(User.FindFirst("Client").Value);
                return Ok(_clientService.GetAccounts(new ClientDTO(client)));
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
                var logued = Logued();
                if (logued != null)
                    return logued;
                var client = _clientService.FindClientByEmail(User.FindFirst("Client").Value);
                if (client.Accounts.Count == 3)
                    return StatusCode(403, "Tienes el limite de 3 cuentas ya creadas");
                return Created("",_clientService.SaveAccount(client));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("current/cards")]
        public IActionResult GetCurrentCards()
        {
            IActionResult logued = Logued();
            if (logued != null)
                return logued;
            try
            {
                var clientDTO =new ClientDTO(_clientService.FindClientByEmail(User.FindFirst("Client").Value));
                return Ok(_clientService.GetCards(clientDTO));
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
                IActionResult logued = Logued();
                if (logued != null)
                {
                    return logued;
                }
                var client = _clientService.FindClientByEmail(User.FindFirst("Client").Value);
                foreach (Card card in client.Cards)
                {
                    if (card.Color.ToString() == cardDTO.Color)
                    {
                        return StatusCode(403, "el cliente solo puede tener 1 tarjeta por cada color");
                    }
                }
                CardDTO cardCreated = _clientService.SaveCard(client,cardDTO);
                return Created("", cardDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}