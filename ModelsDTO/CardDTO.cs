﻿using HomeBankingMindHub.Models;
using System;

namespace HomeBankingMindHub.dtos
{
        public class CardDTO
        {
            public long Id { get; set; }
            public string CardHolder { get; set; }
            public CardType Type { get; set; }
            public CardColor Color { get; set; }
            public string Number { get; set; }
            public int Cvv { get; set; }
            public DateTime? FromDate { get; set; }
            public DateTime? ThruDate { get; set; }
        }
}