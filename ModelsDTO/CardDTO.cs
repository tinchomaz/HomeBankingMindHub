using HomeBankingMindHub.Models;
using System;

namespace HomeBankingMindHub.dtos
{
    public class CardDTO
    {
        public string CardHolder { get; set; }
        public string Type { get; set; }
        public string Color { get; set; }
        public string Number { get; set; }
        public int Cvv { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ThruDate { get; set; }

        public CardDTO()
        {

        }
        public CardDTO(Card card)
        {
            CardHolder = card.CardHolder;
            Type = card.Type.ToString();
            Color = card.Color.ToString();
            Number = card.Number.ToString();
            Cvv = card.Cvv;
            FromDate = card.FromDate;
            ThruDate = card.ThruDate;
        }
    }
}