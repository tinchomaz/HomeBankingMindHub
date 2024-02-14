﻿using HomeBankingMindHub.Models;
using System;



namespace HomeBankingMindHub.ModelsDTO

{

    public class AccountDTO

    {

        public string Number { get; set; }

        public DateTime CreationDate { get; set; }

        public double Balance { get; set; }

        public ICollection<TransactionDTO> Transactions { get; set; }

    }

}