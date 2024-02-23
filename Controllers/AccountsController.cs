using HomeBankingMindHub.Models;
using HomeBankingMindHub.ModelsDTO;
using HomeBankingMindHub.Repositories.Interfaces;
using HomeBankingMindHub.Services.Interfaces;
using Microsoft.AspNetCore.Http;
//Permite usar las etiquetas http
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;

using System.Collections.Generic;

using System.Linq;

namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private IAccountService _accountService;
        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var accounts = _accountService.GetAllAccounts();
                if (!(accounts.Count() == 0))
                {
                    return Ok(accounts);
                }
                //Si no existen accounts devuelve el mensaje,Funciona pero no hay nada,si no funciona seria un 404
                else
                {
                    return StatusCode(204, "Sin Cuentas en el sistema");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public IActionResult Get(long id)
        {
            try
            {
                var account = _accountService.FindAccountById(id);
                if (account == null)
                    return NotFound();
                return Ok(account);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}