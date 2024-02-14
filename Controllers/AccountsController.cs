using HomeBankingMindHub.Models;
using HomeBankingMindHub.ModelsDTO;
using HomeBankingMindHub.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;

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

        private IAccountRepository _accountRepository;



        public AccountsController(IAccountRepository accountRepository)

        {

            _accountRepository = accountRepository;

        }

        [HttpGet]

        public IActionResult Get()

        {

            try

            {
                var accounts = _accountRepository.GetAllAccounts();

                var accountsDTO = new List<AccountDTO>();

                if (!(accounts.Count() == 0))
                {
                    foreach (Account account in accounts)

                    {
                        var newAccountDTO = new AccountDTO
                        {

                            Id = account.Id,
                            
                            Number = account.Number,

                            CreationDate = account.CreationDate,

                            Transactions = account.Transactions.Select(ts => new TransactionDTO

                            {

                                Id = ts.Id,

                                Type = ts.Type.ToString(),

                                Amount = ts.Amount,

                                Description = ts.Description,

                                Date = ts.Date

                            }).ToList()

                        };

                        accountsDTO.Add(newAccountDTO);

                    }
                    return Ok(accountsDTO);
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

                var account = _accountRepository.FindById(id);

                if (account == null)
                    return NotFound();

                var accountDTO = new AccountDTO

                {
                        Id = account.Id,

                        Number = account.Number,

                        CreationDate = account.CreationDate,

                        Transactions = account.Transactions.Select(ts => new TransactionDTO

                    {


                            Id = ts.Id,

                            Type = ts.Type.ToString(),

                            Amount = ts.Amount,

                            Description = ts.Description,

                            Date = ts.Date

                        }).ToList()

                };



                return Ok(accountDTO);

            }

            catch (Exception ex)

            {

                return StatusCode(500, ex.Message);

            }

        }





    }

}