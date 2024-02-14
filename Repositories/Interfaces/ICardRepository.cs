﻿using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Repositories.Interfaces
{
    public interface ICardRepository
    {
        IEnumerable<Card> GetAllCards();
        void Save(Card card);
        Card FindById(long id);
        IEnumerable<Card> GetCardsByClient(long clientId);
    }
}
