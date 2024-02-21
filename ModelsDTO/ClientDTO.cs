﻿using HomeBankingMindHub.dtos;
using HomeBankingMindHub.Models;
using System.Collections.Generic;

using System.Text.Json.Serialization;



namespace HomeBankingMindHub.ModelsDTO

{

    public class ClientDTO

    {
        [JsonIgnore]

        public long Id { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public ICollection<AccountDTO> Accounts { get; set; }
        public ICollection<ClientLoanDTO> Credits { get; set; }
        public ICollection<CardDTO> Cards { get; set; }

        public ClientDTO(Client client) 
        { 
            Id = client.Id;
            FirstName = client.FirstName;
            LastName = client.LastName;
            Email = client.Email;
            Accounts = client.Accounts.Select(ac => new AccountDTO(ac)).ToList();
            Credits = client.ClientLoans.Select(cr => new ClientLoanDTO(cr)).ToList();
            Cards = client.Cards.Select(cd => new CardDTO(cd)).ToList();
        }
    }

}