using HomeBankingMindHub.dtos;
using HomeBankingMindHub.Lib;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.ModelsDTO;
using HomeBankingMindHub.Repositories;
using HomeBankingMindHub.Repositories.Interfaces;
using HomeBankingMindHub.Services.Interfaces;
using Microsoft.AspNetCore.Http;

using Microsoft.AspNetCore.Mvc;

using System;

using System.Collections.Generic;

using System.Linq;
using System.Text.RegularExpressions;



namespace HomeBankingMindHub.Controllers

{
    //[Authorize] me da error al probarlo,devuelve un 302
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
                ClientDTO client = new ClientDTO(_clientService.FindClientByEmail(User.FindFirst("Client").Value));
                return Ok(client);
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
                _clientService.SaveClient(client,out int statusCode,out string? message);
                return StatusCode(statusCode,message);
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
                _clientService.SaveAccount(client,out int statusCode,out string? message);
                return StatusCode(statusCode, message);
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
            Console.WriteLine("prueba");
            try
            {
                IActionResult logued = Logued();
                if (logued != null)
                    return logued;
                var client = _clientService.FindClientByEmail(User.FindFirst("Client").Value);
                _clientService.SaveCard(client,cardDTO,out int statusCode,out string? message);
                return StatusCode(statusCode,message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}