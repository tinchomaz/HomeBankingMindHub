﻿using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.ModelsDTO
{
    public class LoanDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public double MaxAmmount { get; set; }
        public string Payments { get; set; }

        public LoanDTO(Loan loan)
        {
            Id = loan.Id;
            Name = loan.Name;
            MaxAmmount = loan.MaxAmount;
            Payments = loan.Payments;
        }
    }
}
